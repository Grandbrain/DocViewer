#ifndef WINDOW_H
#define WINDOW_H

#include <QtWebEngineWidgets>
#include <QFileSystemModel>
#include <QMessageBox>
#include <QMainWindow>
#include <QStyle>
#include <QDir>

namespace Ui
{
    class Window;
}

class Window : public QMainWindow
{
    Q_OBJECT

public:
    explicit Window(QWidget* = 0);
    virtual ~Window();

private slots:
    void OnAbout();
    void OnExplore();
    void OnDocumentsToggle(bool);
    void OnDocumentsVisible(bool);
    void OnDocumentsTree();
    void OnDocumentsDoubleClicked(const QModelIndex&);

private:
    bool assignViewerPath();
    void printWarning();
    void readSettings();
    void writeSettings();
    void closeEvent(QCloseEvent*);

private:
    Ui::Window* ui;
    QWebEngineView* engineView;
    QFileSystemModel documentsModel;
    QString viewerPath;
    QString currentDocument;
};

#endif