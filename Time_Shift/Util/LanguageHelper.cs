namespace ChapterTool.Util
{
    using System;
    using System.Reflection;
    using System.Windows.Forms;

    public class LanguageHelper
    {
        #region SetAllLang

        /// <summary>
        /// Set language
        /// </summary>
        /// <param name="lang">language:zh-CN, en-US</param>
        private static void SetAllLang(string lang)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);

            var name = "Form1";

            var frm = (Form)Assembly.Load("CameraTest").CreateInstance(name);
            if (frm == null) return;

            var resources = new System.ComponentModel.ComponentResourceManager();
            resources.ApplyResources(frm, "$this");
            AppLang(frm, resources);
        }
        #endregion

        #region SetLang

        /// <summary>
        ///
        /// </summary>
        /// <param name="lang">language:zh-CN, en-US</param>
        /// <param name="form">the form you need to set</param>
        /// <param name="formType">the type of the form </param>
        public static void SetLang(string lang, Form form, Type formType)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            if (form == null) return;
            var resources = new System.ComponentModel.ComponentResourceManager(formType);
            resources.ApplyResources(form, "$this");
            AppLang(form, resources);
        }

        public static void SetLang(string lang, Control control, Type formType)
        {
            // System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            if (control == null) return;
            var resources = new System.ComponentModel.ComponentResourceManager(formType);
            AppLang(control, resources);
        }

        #endregion

        #region AppLang for control

        /// <summary>
        ///  loop set the property of the control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resources"></param>
        private static void AppLang(Control control, System.ComponentModel.ComponentResourceManager resources)
        {
            switch (control)
            {
                case MenuStrip menuStrip:
                    resources.ApplyResources(menuStrip, menuStrip.Name);
                    foreach (ToolStripMenuItem c in menuStrip.Items)
                    {
                        AppLang(c, resources);
                    }
                    break;
                case ContextMenuStrip contextMenuStrip:
                    resources.ApplyResources(contextMenuStrip, contextMenuStrip.Name);
                    foreach (ToolStripMenuItem c in contextMenuStrip.Items)
                    {
                        AppLang(c, resources);
                    }
                    break;
                case DataGridView gridView:
                    foreach (DataGridViewColumn c in gridView.Columns)
                    {
                        resources.ApplyResources(c, c.Name);
                    }
                    break;
            }

            foreach (Control c in control.Controls)
            {
                resources.ApplyResources(c, c.Name);
                AppLang(c, resources);
            }
        }
        #endregion

        #region AppLang for menuitem

        /// <summary>
        /// set the toolscript
        /// </summary>
        /// <param name="item"></param>
        /// <param name="resources"></param>
        private static void AppLang(ToolStripMenuItem item, System.ComponentModel.ComponentResourceManager resources)
        {
            if (item == null) return;
            resources.ApplyResources(item, item.Name);
            foreach (ToolStripMenuItem c in item.DropDownItems)
            {
                AppLang(c, resources);
            }
        }
        #endregion
    }
}