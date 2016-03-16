using System;
using System.Drawing;
using System.Windows.Forms;
using Inventor;

namespace DocViewer
{
    internal class DocButton : Button
    {
        public DocButton(string displayName, string internalName, CommandTypesEnum commandType, string clientId, 
            string description, string tooltip, Icon standardIcon, Icon largeIcon, ButtonDisplayEnum buttonDisplayType)

            : base(displayName, internalName, commandType, clientId, description, tooltip, standardIcon, largeIcon, buttonDisplayType)
        {

        }

        protected override void ButtonDefinition_OnExecute(NameValueMap context)
        {
            try
            {
                var directory = System.Environment.GetEnvironmentVariable("DOC_VIEWER");
                if (directory == null) return;
                var absoluteFilePath = System.IO.Path.Combine(directory, System.IO.Path.GetFileName("DocViewer.exe"));
                System.Diagnostics.Process.Start(absoluteFilePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
