// ****************************************************************************
//
// Copyright (C) 2014-2016 TautCony (TautCony@vcb-s.com)
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
namespace ChapterTool.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;
    using ChapterTool.Properties;
    using ChapterTool.Util;
    using ChapterTool.Util.ChapterData;
    using Microsoft.Win32;
    using static ChapterTool.Util.Logger;
    using static ChapterTool.Util.ToolKits;

    public partial class Form1 : Form
    {
        #region Form1
        public Form1()
        {
            const string key = "Language";
            var lang = RegistryStorage.Load(name: key);
            if (!string.IsNullOrEmpty(lang))
            {
                LanguageHelper.SetLang(lang, this, typeof(Form1));
            }
            InitializeComponent();
            AddCommand();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        public Form1(string args)
        {
            InitializeComponent();
            AddCommand();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            FilePath = args;
            Log(string.Format(Resources.Log_Load_File_Via_Args, args));
        }
        #endregion

        #region HotKey
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData ^ Keys.Control) >= Keys.D0 && (keyData ^ Keys.Control) <= Keys.D9)
            {
                SwitchByHotKey(keyData);
                return true;
            }
            if ((keyData ^ Keys.Alt) >= Keys.D0 && (keyData ^ Keys.Alt) <= Keys.D9)
            {
                SwitchTypeByHotKey(keyData);
                return true;
            }
            switch (keyData)
            {
                case Keys.Control | Keys.S:
                    SaveFile(SelectedSaveType);
                    return true;
                case Keys.Alt | Keys.S:
                    if (comboBox2.SelectedIndex + 1 < comboBox2.Items.Count)
                    {
                        SaveFile(SelectedSaveType);
                        ++comboBox2.SelectedIndex;
                        comboBox2_SelectionChangeCommitted(null, EventArgs.Empty);
                        if (comboBox2.SelectedIndex + 1 == comboBox2.Items.Count)
                        {
                            SaveFile(SelectedSaveType);
                        }
                    }
                    return true;
                case Keys.Control | Keys.O:
                    btnLoad_Click(null, EventArgs.Empty);
                    return true;
                case Keys.Control | Keys.R:
                case Keys.F5:
                    UpdateGridView();
                    return true;
                case Keys.PageDown:
                    if (comboBox2.SelectedIndex + 1 < comboBox2.Items.Count)
                    {
                        ++comboBox2.SelectedIndex;
                        comboBox2_SelectionChangeCommitted(null, EventArgs.Empty);
                    }
                    return true;
                case Keys.PageUp:
                    if (comboBox2.SelectedIndex > 0)
                    {
                        --comboBox2.SelectedIndex;
                        comboBox2_SelectionChangeCommitted(null, EventArgs.Empty);
                    }
                    return true;
                case Keys.Control | Keys.L:
                    btnLog_Click(null, EventArgs.Empty);
                    return true;
                case Keys.Control | Keys.PageUp:
                    comboBoxExpression.SelectedIndex =
                        (comboBoxExpression.SelectedIndex + 1) % comboBoxExpression.Items.Count;
                    break;
                case Keys.Control | Keys.PageDown:
                    comboBoxExpression.SelectedIndex =
                        (comboBoxExpression.SelectedIndex + comboBoxExpression.Items.Count - 1) %
                        comboBoxExpression.Items.Count;
                    break;
                case Keys.F11:
                    Form1_Resize();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool SwitchByHotKey(int index)
        {
            if (index >= comboBox2.Items.Count)
            {
                return false;
            }
            comboBox2.SelectedIndex = index;
            comboBox2_SelectionChangeCommitted(null, EventArgs.Empty);
            return true;
        }

        private void SwitchByHotKey(Keys keyData)
        {
            var numKey = keyData ^ Keys.Control;
            Debug.WriteLine(numKey);
            if (numKey < Keys.D0 || numKey > Keys.D9) return;

            // shift D0 to 9
            if (!SwitchByHotKey((numKey - Keys.D1 + 10) % 10))
            {
                tsTips.Text = Resources.Tips_Out_Of_Range;
            }
        }

        private void SwitchTypeByHotKey(Keys keyData)
        {
            var numKey = keyData ^ Keys.Alt;
            Debug.WriteLine(numKey);
            var index = numKey - Keys.D0;
            if (index < 0 || index > savingType.Items.Count) return;
            savingType.SelectedIndex = index - 1;
        }
        #endregion

        #region Inital
        private void Form1_Load(object sender, EventArgs e)
        {
            Screen.PrimaryScreen.GetDpi(NativeMethods.DpiType.MDT_DEFAULT, out uint x, out _);
            double factor = x / 96.0;
            dataGridView1.ColumnHeadersHeight = (int)(dataGridView1.ColumnHeadersHeight * factor);
            TargetHeight[0] = (int)(Height - (66 * factor));
            TargetHeight[1] = Height;
            lbPath.Height = (int)(lbPath.Height / factor);
            lbPath.Width = (int)(lbPath.Width / factor);

            Text = $@"[VCB-Studio] ChapterTool v{Assembly.GetExecutingAssembly().GetName().Version}";
            InitialLog();
            if (!IsRunningOnMono)
            {
                var saved = String2Point(RegistryStorage.Load(@"Software\ChapterTool", "location"));
                if (saved != new Point(-32000, -32000))
                {
                    Location = saved;
                    Log(string.Format(Resources.Log_Load_Position_Successful, saved));
                }
            }
            LoadSaveType();
            LanguageSelectionContainer.LoadLang(xmlLang);
            InsertAccuracyItems();
            SetDefault();
            this.LoadColor();
            Size = new Size(Size.Width, TargetHeight[0]);
            ExtensionPanelShow = false;
            savingType.SelectedIndex = 0;
            btnTrans.Text = Environment.TickCount % 2 == 0 ? "↺" : "↻";
            if (!IsRunningOnMono) folderBrowserDialog1.SelectedPath = RegistryStorage.Load();

            // Log(Updater.CheckUpdateWeekly("ChapterTool") ? Resources.Log_Update_Checked : Resources.Log_Update_Skiped);
            if (string.IsNullOrEmpty(FilePath)) return;
            if (LoadFile()) UpdateGridView();
            if (!IsRunningOnMono) RegistryStorage.Save(Resources.Message_How_Can_You_Find_Here, @"Software\ChapterTool", string.Empty);
        }

        private void InsertAccuracyItems()
        {
            tsmAccuracy.DropDownItems.Add(new ToolStripMenuItem("0.01") { Tag = 0.01 });
            tsmAccuracy.DropDownItems.Add(new ToolStripSeparator());
            var items = new List<double> { 0.05, 0.10, 0.15, 0.20, 0.25, 0.30 };
            items.ForEach(item => tsmAccuracy.DropDownItems.Add(new ToolStripMenuItem($"{item:F2}")
            { Tag = item, Checked = Math.Abs(item - 0.15) < 1e-5 }));
        }

        private static void InitialLog()
        {
            Log($"{Environment.UserName}{Resources.Log_Hello}");
            Log($"{Environment.OSVersion}");

            if (!IsRunningOnMono) Log(NativeMethods.IsUserAnAdmin() ? Resources.Log_With_Admin : Resources.Log_Without_Admin);

            if (Environment.GetLogicalDrives().Length > 10) Log(Resources.Log_Hard_Drive_Plz);

            if (!IsRunningOnMono)
            {
                using (var registryKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
                {
                    Log((string)registryKey?.GetValue("ProcessorNameString"));
                }
            }

            foreach (var screen in Screen.AllScreens)
            {
                Log($"{screen.DeviceName}{Resources.Log_Resolution}{screen.Bounds.Width}*{screen.Bounds.Height}");
            }
            if (!IsRunningOnMono) Log(string.Format(Resources.Log_Boot_Count, RegistryStorage.RegistryAddCount(@"Software\ChapterTool\Statistics", @"Count")));
        }

        private void SetDefault()
        {
            comboBox2.Enabled = false;
            comboBox2.Visible = false;

            comboBox2.SelectedIndex = -1;
            comboBox1.SelectedIndex = -1;

            cbShift.Checked = false;

            _infoGroup = null;
            _info = null;
            _bdmvTitle = null;

            _newRowInserted = false;

            dataGridView1.Rows.Clear();
        }
        #endregion

        #region Update

        private SystemMenu _systemMenu;

        private void AddCommand()
        {
            if (IsRunningOnMono) return;
            _systemMenu = new SystemMenu(this);

            // _systemMenu.AddCommand(Resources.Update_Check, Updater.CheckUpdate, true);
            var resPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty, "en-US");
            if (Directory.Exists(resPath))
            {
                _systemMenu.AddCommand(Resources.Menu_Switch_Language, () =>
                {
                    if (!File.Exists(Path.Combine(resPath, "ChapterTool.resources.dll")))
                    {
                        Notification.ShowInfo("No valid language resource file found");
                        return;
                    }
                    const string key = "Language";
                    var lang = RegistryStorage.Load(name: key) ?? string.Empty;
                    RegistryStorage.Save(name: key, value: string.IsNullOrEmpty(lang) ? "en-US" : string.Empty);
                    Process.Start(Application.ExecutablePath);
                    Process.GetCurrentProcess().Kill();
                }, true);
            }
        }

        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg);

            // Let it know all messages so it can handle WM_SYSCOMMAND
            // (This method is inlined)
            if (IsRunningOnMono) return;
            _systemMenu.HandleMessage(ref msg);
        }

        #endregion

        #region About Form
        private readonly int[] _poi = { 0, 10 };

        private void progressBar1_Click(object sender, EventArgs e)
        {
            ++_poi[0];
            Log(string.Format(Resources.Log_About_Form_Click, _poi[0]));
            if (_poi[0] >= _poi[1])
            {
                var version = new FormAbout();
                Log(Resources.Log_About_Form_Opened);
                version.Show();
                _poi[0] = 00;
                _poi[1] += 10;
                Log(Resources.Log_About_Form_Click_Reset);
            }
            if (_poi[0] < 3 && _poi[1] == 10)
            {
                MessageBox.Show(@"Something happened", @"Something happened");
            }
        }
        #endregion

        #region Load File
        #region dragLoad
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            _paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (string.IsNullOrEmpty(FilePath)) return;
            if (Directory.Exists(FilePath))
            {
                _isUrl = true;
                LoadBDMVAsync();
                return;
            }
            _isUrl = false;
            if (!IsPathValid) return;
            Log(string.Format(Resources.Log_Load_File_Via_Dragging, FilePath));
            comboBox2.Items.Clear();
            if (LoadFile()) UpdateGridView();
        }

        private bool _isUrl;

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }
        #endregion

        private string[] _paths = new string[20];

        private string FilePath
        {
            get => _paths[0];
            set => _paths[0] = value;
        }

        private bool IsPathValid
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    tsTips.Text = Resources.File_Unloaded;
                    return false;
                }
                if (_isUrl) return true;
                if (RFileType.Value.IsMatch(FilePath)) return true;
                tsTips.Text = Resources.Tips_InValid_Type;
                Log(Resources.Tips_InValid_Type_Log + $"[{Path.GetFileName(FilePath)}]");
                FilePath = string.Empty;
                lbPath.Text = Resources.File_Unloaded;
                return false;
            }
        }

        private static readonly Lazy<Regex> RFileType = new Lazy<Regex>(() =>
        {
            var allType = SupportTypes.SelectMany(supportType => supportType.Value).Aggregate(string.Empty, (current, type) => current + $"{type}|");
            return new Regex($"\\.({allType.TrimEnd('|')})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        });

        private enum FileType
        {
            Mpls, Xml, Txt, Ifo, Mkv, Mka, Tak, Flac, Cue, Xpl, Mp4, M4a, M4v, VTT
        }

        private static readonly Dictionary<string, string[]> SupportTypes = new Dictionary<string, string[]>
        {
            [Resources.File_Filter_Chapter_File] = new[] { "txt", "xml", "mpls", "ifo", "xpl" },
            [Resources.File_Filter_Cue_File] = new[] { "cue", "tak", "flac" },
            [Resources.File_Filter_Matroska_File] = new[] { "mkv", "mka" },
            [Resources.File_Filter_Mp4_File] = new[] { "mp4", "m4a", "m4v" },
            [Resources.File_Filter_VTT_File] = new[] { "vtt" }
        };

        private static readonly Lazy<string> MainFilter = new Lazy<string>(() =>
        {
            string GetType(IEnumerable<string> enumerable) => enumerable.Aggregate(string.Empty, (current, type) => current + $"*.{type};");
            var ret = new StringBuilder(Resources.File_Filter_All_Support);
            var types = GetType(SupportTypes.SelectMany(supportType => supportType.Value));
            ret.Append($" ({types.TrimEnd(';')})|{types}");
            foreach (var supportType in SupportTypes)
            {
                types = GetType(supportType.Value);
                ret.Append($"|{supportType.Key} ({types.TrimEnd(';')})|{types}");
            }
            return ret.ToString();
        });

        private void btnLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = MainFilter.Value;
            try
            {
                openFileDialog1.FileName = string.Empty;
                if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
                FilePath = openFileDialog1.FileName;
                Log(string.Format(Resources.Log_Load_File_Via_Button, FilePath));
                comboBox2.Items.Clear();
                if (LoadFile()) UpdateGridView();
            }
            catch (Exception exception)
            {
                Notification.ShowError($"Exception caught in loading file: {FilePath}", exception);
                Log($"ERROR(btnLoad_Click) {FilePath} {exception.Message}");
                FilePath = string.Empty;
            }
        }

        private ChapterInfoGroup _infoGroup;
        private ChapterInfo _info;

        private bool CombineChapter
        {
            get => combineToolStripMenuItem.Checked;
            set => combineToolStripMenuItem.Checked = value;
        }

        private bool LoadFile()
        {
            if (!IsPathValid) return false;
            var fileName = Path.GetFileName(FilePath);
            lbPath.Text = fileName?.Length > 55 ? $"{fileName.Substring(0, 40)}…{fileName.Substring(fileName.Length - 15, 15)}" : fileName;
            SetDefault();
            Cursor = Cursors.AppStarting;
            try
            {
                FileType fileType;
                try
                {
                    fileType = (FileType)Enum.Parse(
                        typeof(FileType),
                        Path.GetExtension(FilePath)?.ToLowerInvariant().TrimStart('.') ?? string.Empty,
                        true);
                }
                catch
                {
                    throw new Exception("Invalid File Format");
                }
                switch (fileType)
                {
                    case FileType.Mpls: LoadMpls(); break;
                    case FileType.Xml: LoadXml(); break;
                    case FileType.Txt: LoadOgm(); break;
                    case FileType.Ifo: LoadIfo(); break;
                    case FileType.Mkv:
                    case FileType.Mka: LoadMatroska(); break;
                    case FileType.Tak:
                    case FileType.Flac:
                    case FileType.Cue: LoadCue(); break;
                    case FileType.Xpl: LoadXpl(); break;
                    case FileType.Mp4:
                    case FileType.M4a:
                    case FileType.M4v: LoadMp4(); break;
                    case FileType.VTT: LoadWebVTT(); break;
                    default: throw new Exception("Invalid File Format");
                }
                if (_info == null) return false;
                _info.UpdateInfo(_chapterNameTemplate);
            }
            catch (Exception exception)
            {
                Notification.ShowError(@"Exception caught in Function LoadFile", exception);
                Log($"ERROR(LoadFile) {exception.Message}");
                FilePath = string.Empty;
                tsProgressBar1.Value = 0;
                lbPath.Text = Resources.File_Unloaded;
                Cursor = Cursors.Default;
                return false;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
            return true;
        }

        private MplsGroup LoadMpls(bool setGlobal = true, bool addToComboBox = false, string customPath = "")
        {
            if (setGlobal) addToComboBox = true;
            try
            {
                MplsData.OnLog += Log;
                if (string.IsNullOrEmpty(customPath)) customPath = FilePath;
                var mplsGroup = new MplsData(customPath).GetChapters();
                Log(Resources.Log_MPLS_Load_Success);
                Log(string.Format(Resources.Log_MPLS_Clip_Count, mplsGroup.Count));
                if (setGlobal)
                {
                    _infoGroup = mplsGroup;
                    comboBox2.Enabled = comboBox2.Visible = mplsGroup.Count != 0;
                    if (!comboBox2.Enabled)
                    {
                        _infoGroup = null;
                        return mplsGroup;
                    }
                    comboBox2.Items.Clear();
                }
                foreach (var item in mplsGroup)
                {
                    if (addToComboBox) comboBox2.Items.Add($"{item.SourceName}__{item.Chapters.Count}");
                    Log($" |+{item.SourceName} Duration[{item.Duration.Time2String()}]{{{item.Duration.TotalSeconds}}}");
                    Log(string.Format(Resources.Log_TimeStamp_Count, item.Chapters.Count));
                }
                if (setGlobal)
                {
                    comboBox2.SelectedIndex = ClipSelectIndex;
                    GetChapterInfoFromMpls(ClipSelectIndex);
                }
                return mplsGroup;
            }
            finally
            {
                MplsData.OnLog -= Log;
            }
        }

        private void LoadIfo()
        {
            _infoGroup = new IfoGroup();
            _infoGroup.AddRange(IfoData.GetStreams(FilePath).Where(item => item != null));
            if (_infoGroup.Count == 0)
            {
                throw new Exception("No Chapter detected in ifo file");
            }

            comboBox2.Items.Clear();
            comboBox2.Enabled = comboBox2.Visible = _infoGroup.Count >= 1;
            Log(string.Format(Resources.Log_IFO_Clip_Count, _infoGroup.Count));
            foreach (var item in _infoGroup)
            {
                comboBox2.Items.Add($"{item.SourceName}__{item.Chapters.Count}");
                var index = 0;
                item.Chapters.ForEach(chapter => chapter.Number = ++index);
                Log($" |+{item.SourceName} Duration[{item.Duration.Time2String()}]");
                Log(string.Format(Resources.Log_TimeStamp_Count, item.Chapters.Count));
            }
            _info = CombineChapter ? ChapterInfo.CombineChapter(_infoGroup) : _infoGroup.First();
            comboBox2.SelectedIndex = ClipSelectIndex;
            if (_infoGroup.Count < 1)
            {
                tsTips.Text = Resources.Tips_Chapter_Not_find;
                return;
            }
#if false
            if (Math.Abs(_infoGroup.First().FramesPerSecond - 25) < 1e-5M)
            {
                tsTips.Text = Resources.Tips_IFO_Waring_Unfix;
            }
            else
            {
                comboBoxExpression.Text = Resources.Expression_factor_1001;
                cbShift.Checked = true;
                tsTips.Text = Resources.Tips_IFO_Waring_Fixed;
            }
#endif
        }

        private void LoadXpl()
        {
            _infoGroup = new XplGroup();
            _infoGroup.AddRange(XplData.GetStreams(FilePath));
            if (_infoGroup.Count == 0)
            {
                throw new Exception("No Chapter detected in xpl file");
            }

            comboBox2.Items.Clear();
            comboBox2.Enabled = comboBox2.Visible = _infoGroup.Count >= 1;
            Log(string.Format(Resources.Log_XPL_Clip_Count, _infoGroup.Count));
            foreach (var item in _infoGroup)
            {
                comboBox2.Items.Add($"{item.Title}__{item.Chapters.Count}");
                var index = 0;
                item.Chapters.ForEach(chapter => chapter.Number = ++index);
            }
            _info = _infoGroup.First();
            comboBox2.SelectedIndex = ClipSelectIndex;
            tsTips.Text = comboBox2.SelectedIndex == -1 ? Resources.Tips_Chapter_Not_find : Resources.Tips_Load_Success;
        }

        private void LoadMp4()
        {
            if (!File.Exists("libmp4v2.dll"))
            {
                FilePath = string.Empty;
                var choice = Notification.ShowInfo(Resources.Log_Mp4v2_Not_Find);
                if (choice == DialogResult.Yes)
                {
                    Process.Start("http://tautcony.github.io/tcupdate.html#ChapterTool");
                }
                return;
            }
            var linkedFile = Path.Combine(Path.GetPathRoot(FilePath) ?? string.Empty, Guid.NewGuid().ToString());
            try
            {
                Knuckleball.MP4File.OnLog += Log;
                NativeMethods.CreateHardLinkCMD(linkedFile, FilePath);
                _info = new Mp4Data(linkedFile).Chapter;
            }
            catch (Exception exception)
            {
                Notification.ShowError(Resources.Message_Unable_To_Read_Mp4_File, exception);
            }
            finally
            {
                Knuckleball.MP4File.OnLog -= Log;
                if (File.Exists(linkedFile)) File.Delete(linkedFile);
            }
        }

        private void LoadOgm()
        {
            try
            {
                OgmData.OnLog += Log;
                _info = OgmData.GetChapterInfo(File.ReadAllBytes(FilePath).GetUTFString());
                _info.UpdateInfo((int)numericUpDown1.Value);
                tsProgressBar1.Value = 33;
                tsTips.Text = Resources.Tips_Load_Success;
            }
            finally
            {
                OgmData.OnLog -= Log;
            }
        }

        private void LoadXml()
        {
            var doc = new XmlDocument();
            doc.Load(FilePath);
            GetChapterInfoFromXml(doc);
        }

        private void LoadMatroska()
        {
            try
            {
                MatroskaData.OnLog += Log;
                var matroska = new MatroskaData();
                GetChapterInfoFromXml(matroska.GetXml(FilePath));
            }
            catch (Exception exception)
            {
                if (exception.Message == "No Chapter Found")
                {
                    Notification.ShowInfo(exception.Message, MessageBoxButtons.OK);
                }
                else
                {
                    Notification.ShowError(@"Exception caught in function LoadMatroska", exception);
                    Log($"ERROR(LoadMatroska) {exception.Message}");
                }
                FilePath = string.Empty;
            }
            finally
            {
                MatroskaData.OnLog -= Log;
            }
        }

        private void LoadCue()
        {
            try
            {
                _info = new CueData(FilePath, Log).Chapter;
                tsProgressBar1.Value = 33;

                tsTips.Text = Resources.Tips_Load_Success;
            }
            catch (Exception exception)
            {
                Notification.ShowError(@"Exception caught in function LoadCue", exception);
                Log($"ERROR(LoadCue) {exception.Message}");
                FilePath = string.Empty;
            }
        }

        private void LoadWebVTT()
        {
            _info = VTTData.GetChapterInfo(File.ReadAllBytes(FilePath).GetUTFString());
            _info.UpdateInfo((int)numericUpDown1.Value);
            tsProgressBar1.Value = 33;
            tsTips.Text = Resources.Tips_Load_Success;
        }

        private string _bdmvTitle;

        private async void LoadBDMVAsync()
        {
            SetDefault();
            try
            {
                tsTips.Text = Resources.Tips_Loading;
                Application.DoEvents();
                try
                {
                    BDMVData.OnLog += Log;
                    var ret = await BDMVData.GetChapterAsync(FilePath);
                    _bdmvTitle = ret.Key;
                    _infoGroup = ret.Value;
                    if (_infoGroup == null || _infoGroup.Count == 0)
                    {
                        _infoGroup = null;
                        tsTips.Text = Resources.Tips_Load_Fail;
                        return;
                    }
                    _info = _infoGroup.First();
                }
                finally
                {
                    BDMVData.OnLog -= Log;
                }
            }
            catch (Exception exception)
            {
                Notification.ShowError("Exception thrown while loading BluRay disc", exception);
                return;
            }
            tsTips.Text = Resources.Tips_Load_Success;
            Debug.Assert(_infoGroup != null, "info group must not be null");
            comboBox2.Enabled = comboBox2.Visible = _infoGroup.Count >= 1;
            if (!comboBox2.Enabled) return;
            comboBox2.Items.Clear();
            _infoGroup.ForEach(item =>
            {
                comboBox2.Items.Add($"{item.SourceName}__{item.Chapters.Count}");
            });
            comboBox2.SelectedIndex = ClipSelectIndex;
            UpdateGridView();
        }

        #endregion

        #region AppendFile
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FilePath)) return;
            if (_isUrl) LoadBDMVAsync();
            else if (LoadFile()) UpdateGridView();
        }

        private void appendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(_infoGroup is MplsGroup)) return;
            var dir = Path.GetDirectoryName(FilePath);
            openFileDialog1.Filter = @"Appendable file(mpls file)|*.mpls";
            openFileDialog1.InitialDirectory = dir;
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            var newFile = openFileDialog1.FileName;

            var mplsGroup = LoadMpls(setGlobal: false, addToComboBox: true, customPath: newFile);
            _infoGroup.AddRange(mplsGroup);

            CombineChapter = true;
            GetChapterInfoFromMpls(ClipSelectIndex);
            UpdateGridView();
        }
        #endregion

        #region Global status
        private bool AutoGenName => cbAutoGenName.Checked;

        private bool Shift => cbShift.Checked;

        private bool Round => cbRound.Checked;
        #endregion

        #region Save File
        private void btnSave_Click(object sender, EventArgs e) => SaveFile((SaveTypeEnum)savingType.SelectedIndex);

        private string _customSavingPath = string.Empty;

        private SaveTypeEnum SelectedSaveType => (SaveTypeEnum)savingType.SelectedIndex;

        private void btnSave_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Right || folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;
                _customSavingPath = folderBrowserDialog1.SelectedPath;
                RegistryStorage.Save(_customSavingPath);
                Log(string.Format(Resources.Log_Set_Saving_Path, _customSavingPath));
            }
            catch (Exception exception)
            {
                Notification.ShowError($"Exception caught while saving Path: {_customSavingPath}", exception);
                Log($"ERROR(btnSave_MouseUp) {_customSavingPath}: {exception.Message}");
                _customSavingPath = string.Empty;
            }
        }

        private void SaveInfoLog(string savePath)
        {
            Log(Resources.Log_Save_Info);

            switch (_info.SourceType)
            {
                case "MPLS":
                case "DVD":
                    Log(string.Format(Resources.Log_Save_Correspond_File, _info.SourceName));
                    break;
            }
            Log(string.Format(Resources.Log_Save_File_Name, savePath));
            Log(string.Format(Resources.Log_Save_Type, savingType.SelectedItem));
            if (savingType.SelectedIndex == 1)
            {
                Log(string.Format(Resources.Log_Save_Language, xmlLang.Items[xmlLang.SelectedIndex]));
            }
            Log(string.Format(Resources.Log_Save_Is_Use_Chapter_Name, !AutoGenName));
            Log(string.Format(Resources.Log_Save_Is_Use_Chapter_Name_Template, cbChapterName.Checked));
            Log(string.Format(Resources.Log_Save_Chapter_Order_Shift, numericUpDown1.Value));
            Log(string.Format(Resources.Log_Save_Time_Shift, Shift));
            if (Shift)
            {
                Log(string.Format(Resources.Log_Save_Time_Shift_Amount, _info.Expr));
            }
        }

        private string GeneRateSavePath(SaveTypeEnum saveType)
        {
            var rootPath = string.IsNullOrWhiteSpace(_customSavingPath) ? Path.GetDirectoryName(FilePath) : _customSavingPath;
            var fileName = _bdmvTitle ?? Path.GetFileNameWithoutExtension(FilePath);
            Debug.Assert(rootPath != null && fileName != null, "root path and file name must not be null");
            var savePath = Path.Combine(rootPath, fileName);

            var ext = Path.GetExtension(FilePath)?.ToLowerInvariant();
            if (ext == ".mpls" || ext == ".ifo")
                savePath += $"__{_info.SourceName}";

            var index = 1;
            while (File.Exists($"{savePath}_{index}{SaveTypeSuffix[saveType]}")) ++index;
            savePath += $"_{index}{SaveTypeSuffix[saveType]}";

            return savePath;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed.")]
        private enum SaveTypeEnum
        {
            TXT, XML, QPF, TimeCodes, TsmuxerMeta, CUE, JSON
        }

        private static readonly Dictionary<SaveTypeEnum, string> SaveTypeSuffix = new Dictionary<SaveTypeEnum, string>
        {
            [SaveTypeEnum.TXT] = ".txt",
            [SaveTypeEnum.XML] = ".xml",
            [SaveTypeEnum.QPF] = ".qpf",
            [SaveTypeEnum.TimeCodes] = ".TimeCodes.txt",
            [SaveTypeEnum.TsmuxerMeta] = ".TsMuxeR_Meta.txt",
            [SaveTypeEnum.CUE] = ".cue",
            [SaveTypeEnum.JSON] = ".json"
        };

        private void LoadSaveType()
        {
            foreach (var type in Enum.GetNames(typeof(SaveTypeEnum)))
                savingType.Items.Add(type);
        }

        private static readonly Regex RLang = new Regex(@"\((?<lang>.+)\)", RegexOptions.Compiled);

        private void SaveFile(SaveTypeEnum saveType)
        {
            if (!IsPathValid) return; // 防止保存先于载入
            UpdateGridView();
            var savePath = GeneRateSavePath(saveType);

            SaveInfoLog(savePath);

            try
            {
                switch (saveType)
                {
                    case SaveTypeEnum.TXT:
                        _info.GetText(AutoGenName).SaveAs(savePath);
                        break;
                    case SaveTypeEnum.XML:
                        var key = RLang.Match(xmlLang.Items[xmlLang.SelectedIndex].ToString()).Groups["lang"].ToString();
                        _info.SaveXml(savePath, string.IsNullOrWhiteSpace(key) ? string.Empty : LanguageSelectionContainer.Languages[key], AutoGenName);
                        break;
                    case SaveTypeEnum.QPF:
                        // Write qpf file without bom
                        _info.GetQpfile().SaveAs(savePath, false);
                        break;
                    case SaveTypeEnum.TimeCodes:
                        _info.GetTimecodes().SaveAs(savePath);
                        break;
                    case SaveTypeEnum.TsmuxerMeta:
                        _info.GetTsmuxerMeta().SaveAs(savePath);
                        break;
                    case SaveTypeEnum.CUE:
                        _info.GetCue(Path.GetFileName(FilePath), AutoGenName).SaveAs(savePath);
                        break;
                    case SaveTypeEnum.JSON:
                        _info.GetJson(AutoGenName).SaveAs(savePath);
                        break;
                }
                tsProgressBar1.Value = 100;
                tsTips.Text = Resources.Tips_Save_Success;
            }
            catch (Exception exception)
            {
                Notification.ShowError(@"Exception caught while saving file", exception);
                Log($"ERROR(SaveFile) {exception.Message}");
                tsProgressBar1.Value = 60;
                tsTips.Text = Resources.Tips_Save_Fail;
            }
        }
        #endregion

        #region Contorl Panel
        private int ClipSelectIndex => comboBox2.SelectedIndex < 0 ? 0 : comboBox2.SelectedIndex;

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // reserved fps
            if (comboBox1.SelectedIndex == 4)
            {
                UpdateGridView(comboBox1.SelectedIndex); // exactly is 29.970fps
            }
            else
            {
                UpdateGridView(comboBox1.SelectedIndex + 1);
            }
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (_infoGroup is MplsGroup) GetChapterInfoFromMpls(ClipSelectIndex);
            else if (_infoGroup is IfoGroup) GetChapterInfoFromIFO(ClipSelectIndex);
            else _info = _infoGroup[ClipSelectIndex];
            if (Shift) cbShift_CheckedChanged(null, null);
            UpdateGridView();
        }

        private void combineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_infoGroup is MplsGroup || _infoGroup is IfoGroup)
            {
                CombineChapter = !CombineChapter;
                comboBox2_SelectionChangeCommitted(null, null);
            }
        }

        private void refresh_Click(object sender, EventArgs e) => UpdateGridView();
        #endregion

        #region GeneRate Chapter Info
        private void GetChapterInfoFromMpls(int index)
        {
            _info = CombineChapter ? ChapterInfo.CombineChapter(_infoGroup, "MPLS") : _infoGroup[index];
            tsTips.Text = _info.Chapters.Count < 2 ? Resources.Tips_Chapter_Not_find : Resources.Tips_Load_Success;
            _info.UpdateInfo(_chapterNameTemplate);
        }

        private void GetChapterInfoFromIFO(int index)
        {
            _info = CombineChapter ? ChapterInfo.CombineChapter(_infoGroup) : _infoGroup[index];
            tsTips.Text = _info.Chapters.Count < 2 ? Resources.Tips_Chapter_Not_find : Resources.Tips_Load_Success;
            _info.UpdateInfo(_chapterNameTemplate);
        }

        private void GetChapterInfoFromXml(XmlDocument doc)
        {
            _infoGroup = new XmlGroup();
            _infoGroup.AddRange(XmlData.ParseXml(doc));
            comboBox2.Enabled = comboBox2.Visible = _infoGroup.Count >= 1;
            if (comboBox2.Enabled)
            {
                comboBox2.Items.Clear();
                var i = 1;
                foreach (var item in _infoGroup)
                {
                    var name = $"Edition {i++ :D2}";
                    comboBox2.Items.Add(name);
                    Log($" |+{name}");
                    Log(string.Format(Resources.Log_TimeStamp_Count, item.Chapters.Count));
                }
            }
            _info = _infoGroup.First();
            comboBox2.SelectedIndex = ClipSelectIndex;
            tsTips.Text = Resources.Tips_Load_Success;
        }
        #endregion

        #region Grid View

        private bool _newRowInserted;

        private void UpdateGridView(int fpsIndex = 0, bool updateFrameInfo = true)
        {
            if (!IsPathValid || _info == null) return;
            if (!updateFrameInfo) goto SKIP;

            switch (_info.SourceType)
            {
                case "DVD":
                case "MPLS":
                    GetFrameInfo(ConvertFr2Index(_info.FramesPerSecond));
                    comboBox1.Enabled = false;
                    break;
                default:
                    GetFrameInfo(fpsIndex);
                    _info.FramesPerSecond = MplsData.FrameRate[comboBox1.SelectedIndex];
                    comboBox1.Enabled = true;
                    break;
            }

        SKIP:
            var clearRows = _info.Chapters.Count != dataGridView1.Rows.Count || _newRowInserted;
            if (clearRows) dataGridView1.Rows.Clear();
            for (var i = 0; i < _info.Chapters.Count; i++)
            {
                if (clearRows)
                {
                    dataGridView1.Rows.Add(_info.GetRow(i, AutoGenName));
                }
                else
                {
                    _info.EditRow(dataGridView1.Rows[i], AutoGenName);
                }
                Application.DoEvents();
            }
            tsProgressBar1.Value = dataGridView1.RowCount > 1 ? 66 : 33;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var rowIndex = e.RowIndex;
            if (rowIndex < 0)
            {
                return;
            }
            var columnIndex = e.ColumnIndex;
            var row = dataGridView1.Rows[rowIndex];

            var chapter = row.Tag as Chapter;
            Debug.Assert(chapter != null, "Chapter should not be empty");
            var newValue = row.Cells[columnIndex].Value?.ToString() ?? string.Empty;

            TimeSpan newTime;
            var fpsIndex = comboBox1.SelectedIndex + 1;
            switch (columnIndex)
            {
                case 1: // Time edited
                    chapter.Time = TimeSpan.Zero;
                    if (TimeSpan.TryParse(newValue, out newTime))
                    {
                        UpdateTime(newTime);
                    }
                    break;
                case 2: // Chapter Name edited
                    Log(string.Format(Resources.Log_Rename, chapter.Name, row.Cells[columnIndex].Value));
                    chapter.Name = newValue;
                    break;
                case 3: // Frame edited
                    chapter.Time = TimeSpan.Zero;
                    int newFrame;
                    if (int.TryParse(Regex.Match(newValue, @"\d+").Value, out newFrame))
                    {
                        newTime = TimeSpan.FromTicks((long)Math.Round(newFrame / MplsData.FrameRate[fpsIndex] * TimeSpan.TicksPerSecond));
                        UpdateTime(newTime);
                    }
                    break;
                default:
                    break;
            }
            try
            {
                UpdateGridView(fpsIndex);
            }
            catch (InvalidOperationException ex)
            {
                Log(ex.Message);
            }

            void UpdateTime(TimeSpan time)
            {
                if (time > TimeSpan.FromDays(1))
                {
                    chapter.Time = TimeSpan.Zero;
                }
                else
                {
                    chapter.Time = time;
                }
            }
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            _info.Chapters.Remove(e.Row.Tag as Chapter);
            _info.UpdateInfo((int)numericUpDown1.Value);
            if (_info.Chapters.Count < 1 || e.Row.Index != 0) return;
            var newInitialTime = _info.Chapters.First().Time;
            _info.UpdateInfo(newInitialTime);
            if ((_infoGroup is MplsGroup || _infoGroup is IfoGroup) && string.IsNullOrWhiteSpace(_chapterNameTemplate))
            {
                var name = ChapterName.GetChapterName();
                _info.Chapters.ForEach(item => item.Name = name());
            }
            Application.DoEvents();
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            Log(string.Format(Resources.Log_Row_Delete, e.RowCount, e.RowIndex));
        }
        #endregion

        #region Frame Info

        private decimal CostumeAccuracy => decimal.Parse(tsmAccuracy.DropDownItems.OfType<ToolStripMenuItem>().First(item => item.Checked).Tag.ToString());

        private void Accuracy_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (var menuItem in tsmAccuracy.DropDownItems.OfType<ToolStripMenuItem>())
            {
                menuItem.Checked = false;
            }
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
        }

        private void GetFrameInfo(int index = 0)
        {
            var settingAccuracy = CostumeAccuracy;

            if (Round)
            {
                // 当未手动提供帧率[del]并且不是mpls或ifo这种已知帧率的，[/del]才进行蒙帧率操作
                index = index == 0/* && _rawMpls == null && _ifoGroup == null */ ? GetAutofps(settingAccuracy) : index;

                // if (index > 5) { --index; } // 跳过在30与50中间的空项
                comboBox1.SelectedIndex = index - 1;
            }
            else
            {
                index = comboBox1.SelectedIndex + 1; // 未勾选舍入时将帧率直接设置为下拉框当前帧率
            }

            foreach (var chapter in _info.Chapters)
            {
                var frames = _info.Expr.Eval(chapter.Time.TotalSeconds, _info.FramesPerSecond) * MplsData.FrameRate[index];
                if (Round)
                {
                    var rounded = Round ? Math.Round(frames, MidpointRounding.AwayFromZero) : frames;
                    var accuracy = Math.Abs(frames - rounded) < settingAccuracy;
                    chapter.FramesInfo = $"{rounded}{(accuracy ? " K" : " *")}";
                }
                else
                {
                    chapter.FramesInfo = $"{frames}";
                }
            }
        }

        private int GetAutofps(decimal accuracy)
        {
            Log(string.Format(Resources.Log_FPS_Detect_Begin, accuracy));
            var result = MplsData.FrameRate.Select(fps =>
                        _info.Chapters.Sum(item =>
                        item.IsAccuracy(fps, accuracy, _info.Expr))).ToList();
            result[0] = 0; // skip two invalid frame rate.
            result[5] = 0;
            result.ForEach(count => Log(string.Format(Resources.Log_FPS_Detect_Count, count)));
            var autofpsCode = result.IndexOf(result.Max());
            _info.FramesPerSecond = MplsData.FrameRate[autofpsCode];
            Log(string.Format(Resources.Log_FPS_Detect_Result, MplsData.FrameRate[autofpsCode]));
            return autofpsCode == 0 ? 1 : autofpsCode;
        }

        private void FrameShiftForward()
        {
            if (!IsPathValid) return;
            var fpsIndex = comboBox1.SelectedIndex + 1;
            if (fpsIndex < 1) return;
            var shiftFramesString = Notification.InputBox("向前平移N帧，小于0的将被删除", "请输入所需平移的帧数", "0");
            if (!int.TryParse(shiftFramesString, out int shiftFrames)) return;
            var shiftTime = TimeSpan.FromTicks((long)Math.Round(shiftFrames / MplsData.FrameRate[fpsIndex] * TimeSpan.TicksPerSecond));
            _info.UpdateInfo(shiftTime);
            _info.Chapters = _info.Chapters.SkipWhile(item => item.Time < TimeSpan.Zero).ToList();
            UpdateGridView();
        }

        private void ShiftForwardToolStripMenuItem_Click(object sender, EventArgs e) => FrameShiftForward();

        #endregion

        #region Form Color
        private FormColor _fcolor;

        private void Color_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (_fcolor == null)
            {
                _fcolor = new FormColor(this);
            }
            Log(Resources.Log_Color_Setting_Form_Open);
            _fcolor.Show();
            _fcolor.Focus();
            _fcolor.Select();
        }

        public List<Color> CurrentColor => new List<Color>
        {
            BackChange,
            TextBack,
            MouseOverColor,
            MouseDownColor,
            BorderBackColor,
            TextFrontColor
        };

        public Color BackChange
        {
            set
            {
                BackColor = value;
                statusStrip1.BackColor = value;
            }

            private get { return BackColor; }
        }

        public Color TextBack
        {
            set
            {
                dataGridView1.BackgroundColor = value;
                numericUpDown1.BackColor = value;
                comboBoxExpression.BackColor = value;
                comboBox1.BackColor = value;
                comboBox2.BackColor = value;
                xmlLang.BackColor = value;
                savingType.BackColor = value;
            }

            private get { return dataGridView1.BackgroundColor; }
        }

        public Color MouseOverColor
        {
            set
            {
                btnLoad.FlatAppearance.MouseOverBackColor = value;
                btnSave.FlatAppearance.MouseOverBackColor = value;
                btnTrans.FlatAppearance.MouseOverBackColor = value;
                btnLog.FlatAppearance.MouseOverBackColor = value;
                btnPreview.FlatAppearance.MouseOverBackColor = value;
            }

            private get { return btnLoad.FlatAppearance.MouseOverBackColor; }
        }

        public Color MouseDownColor
        {
            set
            {
                btnLoad.FlatAppearance.MouseDownBackColor = value;
                btnSave.FlatAppearance.MouseDownBackColor = value;
                btnTrans.FlatAppearance.MouseDownBackColor = value;
                btnLog.FlatAppearance.MouseDownBackColor = value;
                btnPreview.FlatAppearance.MouseDownBackColor = value;
            }

            private get { return btnLoad.FlatAppearance.MouseDownBackColor; }
        }

        public Color BorderBackColor
        {
            set
            {
                btnLoad.FlatAppearance.BorderColor = value;
                btnSave.FlatAppearance.BorderColor = value;
                btnTrans.FlatAppearance.BorderColor = value;
                btnLog.FlatAppearance.BorderColor = value;
                btnPreview.FlatAppearance.BorderColor = value;
                dataGridView1.GridColor = value;
            }

            private get { return btnLoad.FlatAppearance.BorderColor; }
        }

        public Color TextFrontColor
        {
            set
            {
                ForeColor = value;
                numericUpDown1.ForeColor = value;
                comboBoxExpression.ForeColor = value;
                comboBox1.ForeColor = value;
                comboBox2.ForeColor = value;
                xmlLang.ForeColor = value;
                savingType.ForeColor = value;
                dataGridView1.ForeColor = value;
            }

            private get { return ForeColor; }
        }
        #endregion

        #region Tips
        private void lbPath_MouseEnter(object sender, EventArgs e) => toolTip1.Show(FilePath ?? string.Empty, (IWin32Window)sender);

        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            if (!(_infoGroup is MplsGroup) || !IsPathValid || !FilePath.ToLower().EndsWith(".mpls")) return;
            if (_info.Chapters.Count != 2) return;

            var deltaTime = _info.Duration - _info.Chapters.Last().Time;
            if (deltaTime.Seconds > 5) return;
            toolTip1.Show($"{Resources.ToolTips_Useless_Chapter}", (IWin32Window)sender);
        }

        private void comboBox2_MouseEnter(object sender, EventArgs e)
        {
            var menuMpls = _infoGroup is MplsGroup && _infoGroup.Sum(i => i.Chapters.Count) < 5 && comboBox2.Items.Count > 20;
            toolTip1.Show(menuMpls ? Resources.Tips_Menu_Clip : $"[{comboBox2.Text}] " + string.Format(Resources.Tips_Clip_Count, comboBox2.Items.Count), (IWin32Window)sender);
        }

        private void ToolTipRemoveAll(object sender, EventArgs e) => toolTip1.Hide((IWin32Window)sender);
        #endregion

        #region Close Form
        private static void FormMove(int forward, ref Point p)
        {
            switch (forward)
            {
                case 1: ++p.X; break;
                case 2: --p.X; break;
                case 3: ++p.Y; break;
                case 4: --p.Y; break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegistryStorage.Save(Location.ToString(), @"Software\ChapterTool", "Location");
            if (_poi[0] <= 0 || _poi[0] >= 3 || _poi[1] != 10) return;
            var origin = Location;
            var forward = new Random();
            var forward2 = forward.Next(1, 5);
            if (forward2 % 2 == 0 || Environment.OSVersion.Version.Major == 5)
            {
                for (var i = 0; i < 100; ++i)
                {
                    FormMove(forward.Next(1, 5), ref origin);
                    Location = origin;
                    Application.DoEvents();
                    Thread.Sleep(5);
                }
                return;
            }
            while (Opacity > 0)
            {
                Opacity -= 0.02;
                FormMove(forward2, ref origin);
                Location = origin;
                Application.DoEvents();
                Thread.Sleep(5);
            }
        }
        #endregion

        #region Extension Panel
        #region form resize
        private bool ExtensionPanelShow
        {
            set => panel1.Visible = value;
        }

        private void btnExpand_Click(object sender, EventArgs e) => Form1_Resize();

        private static readonly int[] TargetHeight = new int[2];

        private void Form1_Resize()
        {
            if (TargetHeight.All(item => item != Height)) return;
            tsBtnExpand.Image = Resources.unfold_more;
            if (Height == TargetHeight[0])
            {
                while (Height < TargetHeight[1])
                {
                    Height += 2;
                    Application.DoEvents();
                }
                Height = TargetHeight[1];
            }
            else if (Height == TargetHeight[1])
            {
                while (Height > TargetHeight[0])
                {
                    Height -= 2;
                    Application.DoEvents();
                }
                Height = TargetHeight[0];
            }
            ExtensionPanelShow = Height == TargetHeight[1];
            tsBtnExpand.Image = Height == TargetHeight[0] ? Resources.arrow_drop_down : Resources.arrow_drop_up;
        }
        #endregion

        private void savingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            xmlLang.Enabled = savingType.SelectedIndex == 1;
            if (xmlLang.Enabled && xmlLang.SelectedIndex == -1)
            {
                xmlLang.SelectedIndex = 2;
            }
        }

        private void xmlLang_SelectionChangeCommitted(object sender, EventArgs e)
        {
            switch (xmlLang.SelectedIndex)
            {
                case 0:
                    xmlLang.SelectedIndex = 2;
                    break;
                case 5:
                    xmlLang.SelectedIndex = 6;
                    break;
            }
        }

        #region ChapterNameTemplate
        private string LoadChapterName()
        {
            openFileDialog1.Filter = Resources.File_Filter_Text + @"(*.txt)|*.txt|" +
                                     Resources.File_Filter_All + @"(*.*)|*.*";
            openFileDialog1.FileName = string.Empty;
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var chapterPath = openFileDialog1.FileName;
                    Log(string.Format(Resources.Log_Chapter_Name_Template, chapterPath));

                    return File.ReadAllBytes(chapterPath).GetUTFString();
                }
                cbChapterName.CheckState = CheckState.Unchecked;
                return string.Empty;
            }
            catch (Exception exception)
            {
                Notification.ShowError($"Exception caught while opening file {FilePath}", exception);
                Log($"ERROR(LoadChapterName) {exception.Message}");
                return string.Empty;
            }
        }

        private string _chapterNameTemplate;

        private void cbChapterName_CheckedChanged(object sender, EventArgs e)
        {
            _chapterNameTemplate = cbChapterName.Checked ? LoadChapterName() : string.Empty;
            if (!IsPathValid) return;
            _info.UpdateInfo(_chapterNameTemplate);
            UpdateGridView(0, false);
        }

        #endregion

        private void cbAutoGenName_CheckedChanged(object sender, EventArgs e) => UpdateGridView(0, false);

        private Expression ParseExpression(string expr)
        {
            Expression ret;
            try
            {
                if (!cbPostFix.Checked)
                {
                    ret = new Expression(expr);
                    Log($"Parse result: {ret}");
                }
                else
                {
                    ret = new Expression(expr.Split());
                    Log($"Parse result: {Expression.Postfix2Infix(ret.ToString())}");
                }
            }
            catch (Exception exception)
            {
                Log($"Parse Failed: {exception.Message}");
                ret = Expression.Empty;
                tsTips.Text = Resources.Tips_Invalid_Shift_Time;
            }
            return ret;
        }

        private void cbShift_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsPathValid)
            {
                if (Shift)
                    ParseExpression(comboBoxExpression.Text);
                return;
            }
            if (_info == null) return;
            _info.Expr = Shift ? ParseExpression(comboBoxExpression.Text) : Expression.Empty;
            UpdateGridView();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!IsPathValid) return;
            _info.UpdateInfo((int)numericUpDown1.Value);
            UpdateGridView(0, false);
        }

        private readonly Regex _validExpression = new Regex(@"^[+\-*/\^%\.,\(\)\s\da-zA-Z_]*(?:$|(?://.*))", RegexOptions.Compiled);
        private readonly Regex _invalidVariable = new Regex(@"(?:^|[+\-*/\^%\s])\d+[a-zA-Z_]+", RegexOptions.Compiled);
        private readonly Regex _balanceBrackets = new Regex(@"^[^\(\)]*(((?'Open'\()[^\(\)]*)+((?'Close-Open'\))[^\(\)]*)+)*(?(Open)(?!))(?:$|(?://.*))", RegexOptions.Compiled);

        private void comboBoxExpression_TextChanged(object sender, EventArgs e)
        {
            var isValid = _validExpression.IsMatch(comboBoxExpression.Text) &&
                          _balanceBrackets.IsMatch(comboBoxExpression.Text) &&
                         !_invalidVariable.IsMatch(comboBoxExpression.Text);
            tsTips.Text = isValid ? "Valid expression" : "Invalid expression";
        }

        #endregion

        #region LogForm
        private FormLog _logForm;

        private void btnLog_Click(object sender, EventArgs e)
        {
            if (_logForm == null)
            {
                _logForm = new FormLog();
            }
            _logForm.Show();
            _logForm.Focus();
            _logForm.Select();
        }

        #endregion

        #region PreviewForm
        private FormPreview _previewForm;

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (!IsPathValid) return;
            if (_previewForm == null)
            {
                _previewForm = new FormPreview(this)
                {
                    TopMost = true
                };
            }
            _previewForm.UpdateText(_info.GetText(AutoGenName));
            _previewForm.Show();
            _previewForm.Focus();
            _previewForm.Select();
        }

        private void btnPreview_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || !RunAsAdministrator()) return;
            if (Notification.ShowInfo(Resources.Message_Open_With_CT) == DialogResult.Yes)
            {
                RegistryStorage.SetOpenMethod(Assembly.GetExecutingAssembly().Location, ".mpls", "ChapterTool.Mpls", "ChapterTool");
            }
        }
        #endregion

        #region Open Video

        private static void OpenFile(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch (Exception exception)
            {
                Notification.ShowError($"Exception caught while trying to open {Path.GetFullPath(path)}", exception);
                Log($"Exception caught while trying to open {Path.GetFullPath(path)}");
            }
        }

        private void InsertMpls()
        {
            var basePath = Path.GetDirectoryName(FilePath);
            Debug.Assert(basePath != null, "base path must not be null");
            var targetPath = Path.Combine(basePath, "..\\STREAM");
            if (!Directory.Exists(targetPath)) return;

            combineMenuStrip.Items.Add(new ToolStripSeparator());
            var fileLine = comboBox2.Text;
            foreach (var file in fileLine.Substring(0, fileLine.LastIndexOf('_') - 1).Split('&'))
            {
                var fMenuItem = new ToolStripMenuItem(string.Format(Resources.Menu_Open_File, $"{file}.m2ts"));
                fMenuItem.Click += (sender, args) =>
                {
                    var targetFile = $"{targetPath}\\{file}.m2ts";
                    OpenFile(targetFile);
                };
                combineMenuStrip.Items.Add(fMenuItem);
            }
        }

        private void InsertIfo()
        {
            combineMenuStrip.Items.Add(new ToolStripSeparator());
            var fileLine = comboBox2.Text;
            var file = fileLine.Substring(0, fileLine.LastIndexOf('_') - 1) + ".VOB";
            var fMenuItem = new ToolStripMenuItem(string.Format(Resources.Menu_Open_File, file));
            fMenuItem.Click += (sender, args) =>
            {
                var targetFile = Path.GetDirectoryName(FilePath) + $"\\{file}";
                OpenFile(targetFile);
            };
            combineMenuStrip.Items.Add(fMenuItem);
        }

        private void InsertXpl()
        {
            var basePath = Path.GetDirectoryName(FilePath);
            Debug.Assert(basePath != null, "base path must not be null");
            var targetPath = Path.Combine(basePath, "..\\HVDVD_TS");
            if (!Directory.Exists(targetPath)) return;

            combineMenuStrip.Items.Add(new ToolStripSeparator());
            var file = Path.GetFileName(_info.SourceName);
            var fMenuItem = new ToolStripMenuItem(string.Format(Resources.Menu_Open_File, file));
            fMenuItem.Click += (sender, args) =>
            {
                var targetFile = $"{targetPath}\\{file}";
                OpenFile(targetFile);
            };
            combineMenuStrip.Items.Add(fMenuItem);
        }

        private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_infoGroup is MplsGroup) InsertMpls();
            else if (_infoGroup is IfoGroup) InsertIfo();
            else if (_infoGroup is XplGroup) InsertXpl();
        }

        private void contextMenuStrip2_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            Debug.WriteLine(e.CloseReason);
            var combine = combineMenuStrip.Items[0];
            combineMenuStrip.Items.Clear();
            combineMenuStrip.Items.Add(combine);
        }
        #endregion

        #region Zones
        private void createZonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count < 1) return;

            var zoneRange = new List<KeyValuePair<int, int>>();
            cbRound.Checked = true;
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var rowIndex = dataGridView1.Rows.IndexOf(row);
                var nextRowIndex = rowIndex + 1;

                // todo: make last time stamp use the length of clip info.
                if (rowIndex >= dataGridView1.RowCount - 1)
                {
                    --nextRowIndex;
                }
                var currRow = _info.Chapters[rowIndex].FramesInfo;
                var nextRow = _info.Chapters[nextRowIndex].FramesInfo;

                var beginFrames = int.Parse(currRow.Substring(0, currRow.IndexOf(' ')));
                var endFrames = int.Parse(nextRow.Substring(0, nextRow.IndexOf(' ')));
                zoneRange.Add(new KeyValuePair<int, int>(beginFrames, endFrames - 1));
            }
            var zones = zoneRange.OrderBy(item => item.Key).Aggregate(string.Empty, (current, zone) => current + $"/{zone.Key},{zone.Value},");
            var ret = "--zones " + zones.TrimStart('/');
            var result = Notification.ShowInfo($"{ret}\n{Resources.Zones_Copy_To_Clip_Board}");
            if (result == DialogResult.Yes)
            {
                Clipboard.SetText(ret);
            }
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.Button == MouseButtons.Right)
            {
                createZonestMenuStrip.Show(MousePosition);
            }
        }
        #endregion

        private void InsertChapterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1) return;
            var row = dataGridView1.SelectedRows[0];
            var split = new Chapter("New Chapter", TimeSpan.Zero, 0);
            _info.Chapters.Insert(row.Index, split);
            _info.UpdateInfo((int)numericUpDown1.Value);
            _newRowInserted = true;
            UpdateGridView();
        }

        private void comboBoxExpression_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbShift_CheckedChanged(sender, e);
        }
    }
}
