namespace ChapterTool
{
    class registryStorage
    {
        public static string Load(string subKey = @"Software\ChapterTool", string name = "SavingPath")
        {
            Microsoft.Win32.RegistryKey registryKey;
            string path = string.Empty;

            // HKCU_CURRENT_USER\Software\
            registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(subKey);
            if (registryKey != null)
            {
                path = (string)registryKey.GetValue(name);
                registryKey.Close();
            }
            return path;
        }


        public static void Save(string value, string subKey = @"Software\ChapterTool", string name = "SavingPath")
        {
            Microsoft.Win32.RegistryKey registryKey;

            // HKCU_CURRENT_USER\Software\
            registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(subKey);
            registryKey.SetValue(name, value);
            registryKey.Close();
        }
        public static void Save(System.Collections.Generic.List<System.Drawing.Color> value, string subKey = @"Software\ChapterTool", string name = "Color")
        {
            Microsoft.Win32.RegistryKey registryKey;

            // HKCU_CURRENT_USER\Software\
            registryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(subKey);
            registryKey.SetValue(name, value);
            registryKey.Close();
        }
    }
}
