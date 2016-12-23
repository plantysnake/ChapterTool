using System;
using System.Reflection;
using System.Windows.Forms;

namespace ChapterTool.Util
{
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

            string name = "MainForm";

            var frm = (Form)Assembly.Load("CameraTest").CreateInstance(name);
            if (frm == null) return;

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager();
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
            if (form != null)
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(formType);
                resources.ApplyResources(form, "$this");
                AppLang(form, resources);
            }
        }
        #endregion

        #region AppLang for control
        /// <summary>
        ///  loop set the propery of the control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resources"></param>
        private static void AppLang(Control control, System.ComponentModel.ComponentResourceManager resources)
        {
            MenuStrip menuStrip = control as MenuStrip;
            if (menuStrip != null)
            {
                resources.ApplyResources(menuStrip, menuStrip.Name);
                if (menuStrip.Items.Count > 0)
                {
                    foreach (ToolStripMenuItem c in menuStrip.Items)
                    {
                        AppLang(c, resources);
                    }
                }
            }

            DataGridView gridView = control as DataGridView;
            if (gridView != null)
            {
                foreach (DataGridViewColumn column in gridView.Columns)
                {
                    resources.ApplyResources(column, column.Name);
                }
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
            if (item.DropDownItems.Count <= 0) return;
            foreach (ToolStripMenuItem c in item.DropDownItems)
            {
                AppLang(c, resources);
            }
        }
        #endregion
    }
}