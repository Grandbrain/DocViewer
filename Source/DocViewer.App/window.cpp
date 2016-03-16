#include "about.h"
#include "window.h"
#include "ui_window.h"

Window::Window(QWidget* parent) : QMainWindow(parent), ui(new Ui::Window)
{
    ui->setupUi(this);

    engineView = nullptr;
    if(assignViewerPath())
    {
        engineView = new QWebEngineView(this);
        engineView->load(QUrl(viewerPath));
        engineView->show();
        ui->layoutCentralWidget->addWidget(engineView);
    }

    ui->treeDocuments->setModel(&documentsModel);
    ui->treeDocuments->header()->resizeSection(0, 200);

    documentsModel.setNameFilters(QStringList() << "*.pdf");
    documentsModel.setNameFilterDisables(false);

    connect(ui->actionAbout, SIGNAL(triggered(bool)), SLOT(OnAbout()));
    connect(ui->actionDocuments, SIGNAL(toggled(bool)), SLOT(OnDocumentsToggle(bool)));
    connect(ui->dockDocuments, SIGNAL(visibilityChanged(bool)), SLOT(OnDocumentsVisible(bool)));
    connect(ui->editPath, SIGNAL(editingFinished()), SLOT(OnDocumentsTree()));
    connect(ui->buttonExplore, SIGNAL(released()), SLOT(OnExplore()));
    connect(ui->treeDocuments, SIGNAL(doubleClicked(QModelIndex)), SLOT(OnDocumentsDoubleClicked(QModelIndex)));

    readSettings();
}

Window::~Window()
{
    delete ui;
}

void Window::OnAbout()
{
    About about(this);
    about.exec();
}

void Window::OnExplore()
{
    QFileDialog fileDialog(this);
    fileDialog.setFileMode(QFileDialog::Directory);
    fileDialog.setOption(QFileDialog::ShowDirsOnly);

    if(fileDialog.exec() == QFileDialog::Accepted)
    {
        QStringList selected = fileDialog.selectedFiles();
        if(!selected.isEmpty()) ui->editPath->setText(selected.first());
        OnDocumentsTree();
    }
}

void Window::OnDocumentsToggle(bool toggled)
{
    if(toggled) ui->dockDocuments->show();
    else ui->dockDocuments->hide();
}

void Window::OnDocumentsVisible(bool visible)
{
    if(visible) ui->actionDocuments->setChecked(true);
    else ui->actionDocuments->setChecked(false);
}

void Window::OnDocumentsTree()
{
    QString path = ui->editPath->text();
    QDir directory(path);
    if(!path.isEmpty() || directory.isReadable() || directory.isAbsolute())
    {
        QModelIndex index = documentsModel.setRootPath(path);
        ui->treeDocuments->setRootIndex(index);
    }
}

void Window::OnDocumentsDoubleClicked(const QModelIndex& index)
{
    if(!engineView)
    {
        printWarning();
        return;
    }
    if(index.isValid())
    {
        if(documentsModel.fileInfo(index).isFile())
        {
            currentDocument = documentsModel.fileInfo(index).absoluteFilePath();
            engineView->load(QUrl(viewerPath + "?file=" + currentDocument));
        }
    }
    else
    {
        if(!QFileInfo::exists(currentDocument)) return;
        engineView->load(QUrl(viewerPath + "?file=" + currentDocument));
    }
}

bool Window::assignViewerPath()
{
    QString path = QApplication::applicationDirPath();
    QDir dir(path);
    if(!dir.cd("pdf.js")) return false;
    if(!dir.cd("web")) return false;
    viewerPath = dir.filePath("viewer.html");
    if(!QFileInfo::exists(viewerPath)) return false;
    viewerPath = "file:///" + viewerPath;
    return true;
}

void Window::printWarning()
{
    QMessageBox::warning(this, tr("Предупреждение"),
                         tr("Компоненты приложения повреждены. Попробуйте переустановить программу"));
}

void Window::readSettings()
{
    QSettings settings("DocViewer");
    settings.beginGroup("General");

    QStringList standard = QStandardPaths::standardLocations(QStandardPaths::DocumentsLocation);
    QString root = standard.isEmpty() ? QDir::rootPath() : standard.first();
    ui->editPath->setText(settings.value("path", root).toString());
    OnDocumentsTree();

    currentDocument = settings.value("document", QString()).toString();
    OnDocumentsDoubleClicked(QModelIndex());

    restoreState(settings.value("state").toByteArray());
    ui->actionDocuments->blockSignals(true);
    ui->actionDocuments->setChecked(ui->dockDocuments->isVisible());
    ui->actionDocuments->blockSignals(false);

    restoreGeometry(settings.value("geometry").toByteArray());

    settings.endGroup();
}

void Window::writeSettings()
{
    QSettings settings("DocViewer");
    settings.beginGroup("General");

    settings.setValue("path", ui->editPath->text());
    settings.setValue("document", currentDocument);
    settings.setValue("state", saveState());
    settings.setValue("geometry", saveGeometry());

    settings.endGroup();
}

void Window::closeEvent(QCloseEvent* event)
{
    writeSettings();
    event->accept();
}