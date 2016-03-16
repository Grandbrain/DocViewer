using System;
using System.Drawing;
using System.Windows.Forms;
using Inventor;
using Microsoft.VisualBasic.Compatibility.VB6;

namespace DocViewer
{
    internal abstract class Button
    {
        public static Inventor.Application InventorApplication { set; get; }

        public ButtonDefinition ButtonDefinition { get; private set; }

        protected Button(string displayName, string internalName, CommandTypesEnum commandType, string clientId, 
            string description, string tooltip, Icon standardIcon, Icon largeIcon, ButtonDisplayEnum buttonDisplayType)
        {
            try
            {
                var standardIconIPictureDisp = (stdole.IPictureDisp)Support.IconToIPicture(standardIcon);

                var largeIconIPictureDisp = (stdole.IPictureDisp)Support.IconToIPicture(largeIcon);

                ButtonDefinition = InventorApplication.CommandManager.ControlDefinitions.AddButtonDefinition(displayName, internalName, 
                    commandType, clientId, description, tooltip, standardIconIPictureDisp, largeIconIPictureDisp, buttonDisplayType);

                ButtonDefinition.Enabled = true;

                ButtonDefinition.OnExecute += ButtonDefinition_OnExecute;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        protected abstract void ButtonDefinition_OnExecute(NameValueMap context);
    }

}