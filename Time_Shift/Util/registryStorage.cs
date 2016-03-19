// ****************************************************************************
//
// Copyright (C) 2014-2015 TautCony (TautCony@vcb-s.com)
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// ****************************************************************************
using Microsoft.Win32;

namespace ChapterTool.Util
{
    public static class RegistryStorage
    {
        public static string Load(string subKey = @"Software\ChapterTool", string name = "SavingPath")
        {
            string path = string.Empty;
            // HKCU_CURRENT_USER\Software\
            var registryKey = Registry.CurrentUser.OpenSubKey(subKey);
            if (registryKey == null) return path;
            path = (string)registryKey.GetValue(name);
            registryKey.Close();
            return path;
        }

        public static void Save(string value, string subKey = @"Software\ChapterTool", string name = "SavingPath")
        {
            // HKCU_CURRENT_USER\Software\
            var registryKey = Registry.CurrentUser.CreateSubKey(subKey);
            registryKey?.SetValue(name, value);
            registryKey?.Close();
        }


        public static void SetOpenMethod()
        {
            const string strProject = "ChapterTool";
            Registry.ClassesRoot.CreateSubKey(".mpls")?.SetValue("ChapterTool.MPLS", strProject, RegistryValueKind.String);
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(strProject))
            {
                key?.CreateSubKey(@"Shell\Open\Command")?.SetValue("", System.Reflection.Assembly.GetExecutingAssembly().Location + " \"%1\"", RegistryValueKind.ExpandString);
            }
        }


        public static int RegistryAddCount(string subKey, string name, int delta = 1)
        {
            var countS = Load(subKey, name);
            int count = string.IsNullOrEmpty(countS) ? 0 : int.Parse(countS);
            count += delta;
            Save(count.ToString(), subKey, name);
            return count - delta;
        }

    }
}
