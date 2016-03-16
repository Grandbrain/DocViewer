using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Inventor;

namespace DocViewer
{
    [Guid("13e401b1-b2da-48b1-ba31-70b73022ce2c")]
    public class StandardAddInServer : ApplicationAddInServer
    {
        private Inventor.Application mInventorApplication;

        private ButtonDefinition mButtonDefinition;

        private UserInterfaceEvents mInterfaceEvents;

        private string mClientId;

        public void Activate(ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            try
            {
                mInventorApplication = addInSiteObject.Application;
                mInterfaceEvents = mInventorApplication.UserInterfaceManager.UserInterfaceEvents;
                Button.InventorApplication = mInventorApplication;

                mInterfaceEvents.OnResetCommandBars += UserInterfaceEvents_OnResetCommandBars;
                mInterfaceEvents.OnResetEnvironments += UserInterfaceEvents_OnResetEnvironments;
                mInterfaceEvents.OnResetRibbonInterface += UserInterfaceEvents_OnResetRibbonInterface;

                var addInClsid = (GuidAttribute)System.Attribute.GetCustomAttribute(typeof(StandardAddInServer), typeof(GuidAttribute));
                mClientId = "{" + addInClsid.Value + "}";

                CreateUserInterface();
                LoadUserInterface();
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void CreateUserInterface()
        {
            try
            {
                var sIcon = new Icon(GetType(), "icon16x16.ico");

                var lIcon = new Icon(GetType(), "icon32x32.ico");

                var button = new DocButton(Resources.DisplayName, "DocViewer:OpenViewer", CommandTypesEnum.kQueryOnlyCmdType,
                    mClientId, Resources.Description, Resources.DisplayName, sIcon, lIcon, ButtonDisplayEnum.kDisplayTextInLearningMode);

                mButtonDefinition = button.ButtonDefinition;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void LoadUserInterface()
        {
            try
            {
                foreach (var ribbon in from object ribbonObject in mInventorApplication.UserInterfaceManager.Ribbons select ribbonObject as Ribbon)
                {
                    var ribbonTab = ribbon.RibbonTabs.Add("DocViewer", "DocViewer:TabViewer", mClientId);
                    var ribbonPanel = ribbonTab.RibbonPanels.Add(Resources.PanelName, "DocViewer:PanelViewer", mClientId);
                    ribbonPanel.CommandControls.AddButton(mButtonDefinition, true);
                    ribbon.QuickAccessControls.AddButton(mButtonDefinition);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void UserInterfaceEvents_OnResetCommandBars(ObjectsEnumerator commandBars, NameValueMap context)
        {
            try
            {
                LoadUserInterface();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void UserInterfaceEvents_OnResetEnvironments(ObjectsEnumerator environments, NameValueMap context)
        {
            try
            {
                LoadUserInterface();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void UserInterfaceEvents_OnResetRibbonInterface(NameValueMap context)
        {
            try
            {
                LoadUserInterface();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void Deactivate()
        {
            Marshal.ReleaseComObject(mInventorApplication);
            mInventorApplication = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandId)
        {

        }

        public object Automation
        {
            get { return null; }
        }
    }
}
