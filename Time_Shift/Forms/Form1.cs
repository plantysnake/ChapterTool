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
using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Drawing;
using Microsoft.Win32;
using ChapterTool.Util;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using ChapterTool.Properties;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static ChapterTool.Util.CTLogger;
using static ChapterTool.Util.ConvertMethod;


namespace ChapterTool.Forms
{
    public partial class Form1 : Form
    {
        #region Form1
        public Form1()
        {
            InitializeComponent();
            //LanguageHelper.SetLang("en-US", this, typeof(Form1));
            AddCommand();
        }

        public Form1(string args)
        {
            InitializeComponent();
            FilePath = args;
            Log(string.Format(Resources.Log_Load_File_Via_Args, args));
            AddCommand();
        }
        #endregion

        #region HotKey
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.S:
                    SaveFile(savingType.SelectedIndex);
                    return true;
                case Keys.Alt | Keys.S:
                    if (comboBox2.SelectedIndex + 1 < comboBox2.Items.Count)
                    {
                        SaveFile(savingType.SelectedIndex);
                        ++comboBox2.SelectedIndex;
                        comboBox2_SelectionChangeCommitted(null, EventArgs.Empty);
                        if (comboBox2.SelectedIndex + 1 == comboBox2.Items.Count)
                        {
                            SaveFile(savingType.SelectedIndex);
                        }
                    }
                    return true;
                case Keys.Control | Keys.O:
                    btnLoad_Click(null, EventArgs.Empty);
                    return true;
                case Keys.Control | Keys.R:
                    UpdataGridView();
                    return true;
                case Keys.Control | Keys.D0:
                case Keys.Control | Keys.D1:
                case Keys.Control | Keys.D2:
                case Keys.Control | Keys.D3:
                case Keys.Control | Keys.D4:
                case Keys.Control | Keys.D5:
                case Keys.Control | Keys.D6:
                case Keys.Control | Keys.D7:
                case Keys.Control | Keys.D8:
                case Keys.Control | Keys.D9:
                    SwitchByHotKey(keyData);
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
            Keys numKey = keyData ^ Keys.Control;
            Debug.WriteLine(numKey);
            if (numKey < Keys.D0 || numKey > Keys.D9) return;
            if (!SwitchByHotKey((numKey - Keys.D1 + 10) % 10)) //shift D0 to 9
            {
                tsTips.Text = Resources.Tips_Out_Of_Range;
            }
        }
        #endregion

        #region Inital
        private void Form1_Load(object sender, EventArgs e)
        {
            TargetHeight[0] = Height - 66;
            TargetHeight[1] = Height;
            Text = $"[VCB-Studio] ChapterTool v{Assembly.GetExecutingAssembly().GetName().Version}";
            InitialLog();
            Point saved = String2Point(RegistryStorage.Load(@"Software\ChapterTool", "location"));
            if (saved != new Point(-32000, -32000))
            {
                Location = saved;
                Log(string.Format(Resources.Log_Load_Position_Successful, saved));
            }
            LanguageSelectionContainer.LoadLang(xmlLang);
            InsertAccuracyItems();
            SetDefault();
            this.LoadColor();
            Size                              = new Size(Size.Width, TargetHeight[0]);
            ExtensionPanelShow                = false;
            savingType.SelectedIndex          = 0;
            btnTrans.Text                     = Environment.TickCount % 2 == 0 ? "↺" : "↻";
            folderBrowserDialog1.SelectedPath = RegistryStorage.Load();
            Log(Updater.CheckUpdateWeekly("ChapterTool") ? Resources.Log_Update_Checked : Resources.Log_Update_Skiped);
            if (string.IsNullOrEmpty(FilePath)) return;
            if (Loadfile()) UpdataGridView();
            RegistryStorage.Save(Resources.Message_How_Can_You_Find_Here, @"Software\ChapterTool", string.Empty);
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
            Log(Environment.UserName.ToLowerInvariant().IndexOf("yzy", StringComparison.Ordinal) > 0
                ? Resources.Log_Wu_Zong : $"{Environment.UserName}{Resources.Log_Hello}");
            Log($"{Environment.OSVersion}");

            Log(NativeMethods.IsUserAnAdmin() ? Resources.Log_With_Admin : Resources.Log_Without_Admin);

            if (Environment.GetLogicalDrives().Length > 10) Log(Resources.Log_Hard_Drive_Plz);

            using (var registryKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
            {
                Log((string)registryKey?.GetValue("ProcessorNameString"));
            }

            foreach (var screen in Screen.AllScreens)
            {
                Log($"{screen.DeviceName}{Resources.Log_Resolution}{screen.Bounds.Width}*{screen.Bounds.Height}");
            }
            Log(string.Format(Resources.Log_Boot_Count, RegistryStorage.RegistryAddCount(@"Software\ChapterTool\Statistics", @"Count")));
        }

        private void SetDefault()
        {
            comboBox2.Enabled       = false;
            comboBox2.Visible       = false;

            comboBox2.SelectedIndex = -1;
            comboBox1.SelectedIndex = -1;

            cbMul1k1.Checked        = false;

            _rawMpls                = null;
            _ifoGroup               = null;
            _xmlGroup               = null;
            _info                   = null;
            _fullIfoChapter         = null;
            _xplGroup               = null;

            dataGridView1.Rows.Clear();
        }
        #endregion

        #region Update

        private SystemMenu _systemMenu;

        private void AddCommand()
        {
            _systemMenu = new SystemMenu(this);
            _systemMenu.AddCommand(Resources.Update_Check, Updater.CheckUpdate, true);
        }

        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg);

            // Let it know all messages so it can handle WM_SYSCOMMAND
            // (This method is inlined)
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
                Form2 version = new Form2();
                Log(Resources.Log_About_Form_Opened);
                version.Show();
                _poi[0]  = 00;
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
            if (!IsPathValid) return;
            Log(string.Format(Resources.Log_Load_File_Via_Dragging, FilePath));
            comboBox2.Items.Clear();
            if (Loadfile()) UpdataGridView();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }
        #endregion

        private string[] _paths = new string[20];

        private string FilePath
        {
            get { return _paths[0]; }
            set { _paths[0] = value; }
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
                if (RFileType.IsMatch(FilePath)) return true;
                tsTips.Text = Resources.Tips_InValid_Type;
                Log(Resources.Tips_InValid_Type_Log + $"[{Path.GetFileName(FilePath)}]");
                FilePath = string.Empty;
                lbPath.Text = Resources.File_Unloaded;
                return false;
            }
        }

        private static readonly Regex RFileType = new Regex(@"\.(txt|xml|mpls|ifo|mkv|mka|cue|tak|flac|xpl|mp4|m4a|m4v)$", RegexOptions.IgnoreCase);

        private void btnLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = Resources.File_Filter_All_Support + @"(*.txt,*.xml,*.mpls,*.ifo,*.xpl,*.cue,*tak,*flac,*.mkv,*.mka,*.mp4,*.m4a,*.m4v)|*.txt;*.xml;*.mpls;*.ifo;*.xpl;*.cue;*.tak;*.flac;*.mkv;*.mka;*.mp4;*.m4a;*.m4v|" +
                                     Resources.File_Filter_Chapter_File + @"(*.txt,*.xml,*.mpls,*.ifo,*.xpl)|*.txt;*.xml;*.mpls;*.ifo;*.xpl|" +
                                     Resources.File_Filter_Cue_File +  @"(*.cue,*.tak,*.flac)|*.cue;*.tak;*.flac|" +
                                     Resources.File_Filter_Matroska_File + @"(*.mkv,*.mka)|*.mkv;*.mka|" +
                                     Resources.File_Filter_Mp4_File + @"(*.mp4,*.m4a,*.m4v)|*.mp4;*.m4a;*.m4v";
            try
            {
                openFileDialog1.FileName = string.Empty;
                if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
                FilePath = openFileDialog1.FileName;
                Log(string.Format(Resources.Log_Load_File_Via_Button, FilePath));
                comboBox2.Items.Clear();
                if (Loadfile()) UpdataGridView();
            }
            catch (Exception exception)
            {
                Notification.ShowError($"Exception catched in loading file: {FilePath}", exception);
                Log($"ERROR(btnLoad_Click) {FilePath} {exception.Message}");
                FilePath = string.Empty;
            }
        }

        private List<ChapterInfo> _ifoGroup;
        private List<ChapterInfo> _xplGroup;
        private MplsData          _rawMpls;
        private ChapterInfo       _info;
        private ChapterInfo       _fullIfoChapter;

        private bool CombineChapter
        {
            get { return combineToolStripMenuItem.Checked; }
            set { combineToolStripMenuItem.Checked = value; }
        }

        private enum FileType
        {
            Mpls, Xml, Txt, Ifo, Mkv, Mka, Tak, Flac, Cue, Xpl, Mp4, M4a, M4v
        }

        private bool Loadfile()
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
                    fileType = (FileType) Enum.Parse(typeof (FileType),
                                Path.GetExtension(FilePath)?.ToLowerInvariant().TrimStart('.') ?? "", true);
                }
                catch
                {
                    throw new Exception("Invalid File Format");
                }
                switch (fileType)
                {
                    case FileType.Mpls: LoadMpls();     break;
                    case FileType.Xml : LoadXml();      break;
                    case FileType.Txt : LoadOgm();      break;
                    case FileType.Ifo : LoadIfo();      break;
                    case FileType.Mkv :
                    case FileType.Mka : LoadMatroska(); break;
                    case FileType.Tak :
                    case FileType.Flac:
                    case FileType.Cue : LoadCue();      break;
                    case FileType.Xpl : LoadXpl();      break;
                    case FileType.Mp4 :
                    case FileType.M4a :
                    case FileType.M4v : LoadMp4();      break;
                    default : throw new Exception("Invalid File Format");
                }
                if (_info == null) return false;
                _info.UpdataInfo(_chapterNameTemplate);
            }
            catch (Exception exception)
            {

                Notification.ShowError(@"Exception catched in Function LoadFile", exception);
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

        private void btnLoad_MouseUp(object sender, MouseEventArgs e)
        {
            //Reload File
            if (e.Button != MouseButtons.Right || string.IsNullOrEmpty(FilePath)) return;
            if (Loadfile()) UpdataGridView();
        }

        private void LoadMpls()
        {
            MplsData.OnLog += Log;
            _rawMpls = new MplsData(FilePath);
            MplsData.OnLog -= Log;
            Log(Resources.Log_MPLS_Load_Success);
            Log(string.Format(Resources.Log_MPLS_Clip_Count, _rawMpls.ChapterClips.Count));

            comboBox2.Enabled = comboBox2.Visible = _rawMpls.ChapterClips.Count >= 1;
            if (!comboBox2.Enabled) return;
            comboBox2.Items.Clear();
            _rawMpls.ChapterClips.ForEach(item =>
            {
                comboBox2.Items.Add($"{item.Name}__{item.TimeStamp.Count}");
                Log($" |+{item.Name} Duration[{MplsData.Pts2Time(item.Length).Time2String()}]");
                Log(string.Format(Resources.Log_TimeStamp_Count, item.TimeStamp.Count));
            });
            comboBox2.SelectedIndex = ClipSeletIndex;
            GetChapterInfoFromMpls(ClipSeletIndex);
        }

        private void LoadIfo()
        {
            _ifoGroup = IfoData.GetStreams(FilePath).Where(item => item != null).ToList();
            if (_ifoGroup.Count == 0)
            {
                throw new Exception("No Chapter detected in ifo file");
            }

            _fullIfoChapter = ChapterInfo.CombineChapter(_ifoGroup);

            comboBox2.Items.Clear();
            comboBox2.Enabled = comboBox2.Visible = _ifoGroup.Count >= 1;
            Log(string.Format(Resources.Log_IFO_Clip_Count, _ifoGroup.Count));
            foreach (var item in _ifoGroup)
            {
                comboBox2.Items.Add($"{item.SourceName}__{item.Chapters.Count}");
                int index = 0;
                item.Chapters.ForEach(chapter => chapter.Number = ++index);
                Log($" |+{item.SourceName} Duration[{item.Duration.Time2String()}]");
                Log(string.Format(Resources.Log_TimeStamp_Count, item.Chapters.Count));
            }
            _info = CombineChapter ? _fullIfoChapter : _ifoGroup.First();
            comboBox2.SelectedIndex = ClipSeletIndex;
            if (_ifoGroup.Count < 1)
            {
                tsTips.Text = Resources.Tips_Chapter_Not_find;
                return;
            }
            if (Math.Abs(_ifoGroup.First().FramesPerSecond - 25.0) < 1e-5)
            {
                tsTips.Text = Resources.Tips_IFO_Waring_Unfix;
            }
            else
            {
                cbMul1k1.Checked = true;
                tsTips.Text = Resources.Tips_IFO_Waring_Fixed;
            }
        }

        private void LoadXpl()
        {
            _xplGroup = XplData.GetStreams(FilePath).ToList();
            if (_xplGroup.Count == 0)
            {
                throw new Exception("No Chapter detected in xpl file");
            }

            comboBox2.Items.Clear();
            comboBox2.Enabled = comboBox2.Visible = _xplGroup.Count >= 1;
            Log(string.Format(Resources.Log_XPL_Clip_Count, _xplGroup.Count));
            foreach (var item in _xplGroup)
            {
                comboBox2.Items.Add($"{item.Title}__{item.Chapters.Count}");
                int index = 0;
                item.Chapters.ForEach(chapter => chapter.Number = ++index);
            }
            _info = _xplGroup.First();
            comboBox2.SelectedIndex = ClipSeletIndex;
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
            try
            {
                Knuckleball.ChapterList.OnLog += Log;
                var mp4 = new Mp4Data(FilePath);
                Knuckleball.ChapterList.OnLog -= Log;
                _info = mp4.Chapter;
            }
            catch (Exception exception)
            {
                Notification.ShowError(Resources.Message_Unable_To_Read_Mp4_File, exception);
            }
        }

        private void LoadOgm()
        {
            OgmData.OnLog += Log;
            _info = OgmData.GetChapterInfo(File.ReadAllBytes(FilePath).GetUTF8String());
            OgmData.OnLog -= Log;
            _info.UpdataInfo((int)numericUpDown1.Value);
            tsProgressBar1.Value = 33;
            tsTips.Text = Resources.Tips_Load_Success;
        }

        private void LoadXml()
        {
            var doc = new XmlDocument();
            doc.Load(FilePath);
            GetChapterInfoFromXml(doc);
        }

        private void LoadMatroska()
        {
            MatroskaData.OnLog += Log;
            var matroska = new MatroskaData();
            MatroskaData.OnLog -= Log;
            try
            {
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
                    Notification.ShowError(@"Exception catched in fuction LoadMatroska", exception);
                    Log($"ERROR(LoadMatroska) {exception.Message}");
                }
                FilePath = string.Empty;
            }
        }

        private void LoadCue()
        {
            try
            {
                _info = new CueData(FilePath).Chapter;
                tsProgressBar1.Value = 33;

                tsTips.Text = Resources.Tips_Load_Success;
            }
            catch (Exception exception)
            {

                Notification.ShowError(@"Exception catched in fuction LoadCue", exception);
                Log($"ERROR(LoadCue) {exception.Message}");
                FilePath = string.Empty;
            }
        }
        #endregion

        #region Save File
        private void btnSave_Click(object sender, EventArgs e) => SaveFile(savingType.SelectedIndex);

        private string _customSavingPath = string.Empty;

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

                Notification.ShowError($"Exception catched while saving Path: {_customSavingPath}", exception);
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
            Log(string.Format(Resources.Log_Save_Is_Use_Chapter_Name, !cbAutoGenName.Checked));
            Log(string.Format(Resources.Log_Save_Is_Use_Chapter_Name_Template, cbChapterName.Checked));
            Log(string.Format(Resources.Log_Save_Chapter_Order_Shift, numericUpDown1.Value));
            Log(string.Format(Resources.Log_Save_Time_Factor, cbMul1k1.Checked));
            Log(string.Format(Resources.Log_Save_Time_Shift, cbShift.Checked));
            if (cbShift.Checked)
            {
                Log(string.Format(Resources.Log_Save_Time_Shift_Amount,_info.Offset.Time2String()));
            }
        }

        private string GeneRateSavePath(int saveType)
        {
            var rootPath = string.IsNullOrWhiteSpace(_customSavingPath) ? Path.GetDirectoryName(FilePath) : _customSavingPath;
            var fileName = Path.GetFileNameWithoutExtension(FilePath);
            Debug.Assert(rootPath != null && fileName != null);
            var savePath = Path.Combine(rootPath, fileName);

            var ext = Path.GetExtension(FilePath)?.ToLowerInvariant();
            if (ext == ".mpls" || ext == ".ifo")
                savePath += $"__{_info.SourceName}";

            string[] saveingTypeSuffix = { ".txt", ".xml", ".qpf", ".TimeCodes.txt", ".TsMuxeR_Meta.txt", ".cue" };
            int index = 1;
            while (File.Exists($"{savePath}_{index}{saveingTypeSuffix[saveType]}")) ++index;
            savePath += $"_{index}{saveingTypeSuffix[saveType]}";

             return savePath;
        }

        private static readonly Regex RLang = new Regex(@"\((?<lang>.+)\)");

        private void SaveFile(int saveType)
        {
            if (!IsPathValid) return;//防止保存先于载入

            var savePath = GeneRateSavePath(saveType);

            SaveInfoLog(savePath);

            try
            {
                switch (saveType)
                {
                    case 0: //TXT
                        _info.SaveText(savePath, cbAutoGenName.Checked);
                        break;
                    case 1: //XML
                        string key = RLang.Match(xmlLang.Items[xmlLang.SelectedIndex].ToString()).Groups["lang"].ToString();
                        _info.SaveXml(savePath, string.IsNullOrWhiteSpace(key) ? "" : LanguageSelectionContainer.Languages[key], cbAutoGenName.Checked);
                        break;
                    case 2: //QPF
                        _info.SaveQpfile(savePath);
                        break;
                    case 3: //Time Codes
                        _info.SaveTimecodes(savePath);
                        break;
                    case 4: //Tsmuxer
                        _info.SaveTsmuxerMeta(savePath);
                        break;
                    case 5: //CUE
                        _info.SaveCue(Path.GetFileName(FilePath), savePath, cbAutoGenName.Checked);
                        break;
                }
                tsProgressBar1.Value = 100;
                tsTips.Text = Resources.Tips_Save_Success;
            }
            catch (Exception exception)
            {
                Notification.ShowError(@"Exception catched while saving file", exception);
                Log($"ERROR(SaveFile) {exception.Message}");
                tsProgressBar1.Value = 60;
                tsTips.Text = Resources.Tips_Save_Fail;
            }
        }
        #endregion

        #region Contorl Panel
        private int ClipSeletIndex => comboBox2.SelectedIndex < 0 ? 0 : comboBox2.SelectedIndex;

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 4)//reserved fps
            {
                UpdataGridView(comboBox1.SelectedIndex);//exactly is 29.970fps
            }
            else
            {
                UpdataGridView(comboBox1.SelectedIndex + 1);
            }
        }


        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (_rawMpls != null)
            {
                GetChapterInfoFromMpls(ClipSeletIndex);
            }
            else if (_xmlGroup != null)
            {
                _info = _xmlGroup[ClipSeletIndex];
            }
            else if (_ifoGroup != null)
            {
                GetChapterInfoFromIFO(ClipSeletIndex);
            }
            else if (_xplGroup != null)
            {
                _info = _xplGroup[ClipSeletIndex];
            }
            _info.Mul1K1 = cbMul1k1.Checked;
            UpdataGridView();
        }

        private void combineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_rawMpls == null && _ifoGroup == null) return;
            CombineChapter = !CombineChapter;
            if (_rawMpls != null)
            {
                GetChapterInfoFromMpls(ClipSeletIndex);
            }
            else if (_ifoGroup != null)
            {
                GetChapterInfoFromIFO(ClipSeletIndex);
            }
            _info.Mul1K1 = cbMul1k1.Checked;
            UpdataGridView();
        }

        private void refresh_Click(object sender, EventArgs e) => UpdataGridView();
        #endregion

        #region GeneRate Chapter Info
        private void GetChapterInfoFromMpls(int index)
        {
            MplsData.OnLog += Log;
            _info = _rawMpls.ToChapterInfo(index, CombineChapter);
            MplsData.OnLog -= Log;
            tsTips.Text = _info.Chapters.Count < 2 ? Resources.Tips_Chapter_Not_find : Resources.Tips_Load_Success;
            _info.UpdataInfo(_chapterNameTemplate);
        }

        private void GetChapterInfoFromIFO(int index)
        {
            _info = CombineChapter ? _fullIfoChapter : _ifoGroup[index];
        }

        private List<ChapterInfo> _xmlGroup;

        private void GetChapterInfoFromXml(XmlDocument doc)
        {
            _xmlGroup = XmlData.PraseXml(doc).ToList();
            comboBox2.Enabled = comboBox2.Visible = _xmlGroup.Count >= 1;
            if (comboBox2.Enabled)
            {
                comboBox2.Items.Clear();
                int i = 1;
                foreach (var item in _xmlGroup)
                {
                    var name = $"Edition {i++:D2}";
                    comboBox2.Items.Add(name);
                    Log($" |+{name}");
                    Log(string.Format(Resources.Log_TimeStamp_Count, item.Chapters.Count));
                }
            }
            _info = _xmlGroup.First();
            comboBox2.SelectedIndex = ClipSeletIndex;
            tsTips.Text = Resources.Tips_Load_Success;
        }
        #endregion

        #region Grid View
        private void UpdataGridView(int fpsIndex = 0, bool updateFrameInfo = true)
        {
            if (!IsPathValid || _info == null) return;
            if (!updateFrameInfo) goto SKIP;

            switch (_info.SourceType)
            {
                case "DVD":
                    GetFramInfo(ConvertFr2Index(_info.FramesPerSecond));
                    comboBox1.Enabled     = false;
                    break;
                case "MPLS":
                    GetFramInfo(_rawMpls.ChapterClips[ClipSeletIndex].Fps);
                    comboBox1.Enabled     = false;
                    break;
                default:
                    GetFramInfo(fpsIndex);
                    _info.FramesPerSecond = (double)MplsData.FrameRate[comboBox1.SelectedIndex];
                    comboBox1.Enabled     = true;
                    break;
            }

            SKIP:
            bool clearRows = _info.Chapters.Count != dataGridView1.Rows.Count;
            if (clearRows) dataGridView1.Rows.Clear();
            for (var i = 0; i < _info.Chapters.Count; i++)
            {
                if (clearRows)
                {
                    dataGridView1.Rows.Add(_info.GetRow(i, cbAutoGenName.Checked));
                }
                else
                {
                    _info.EditRow(dataGridView1.Rows[i], cbAutoGenName.Checked);
                }
                Application.DoEvents();
            }
            tsProgressBar1.Value = dataGridView1.RowCount > 1 ? 66 : 33;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var chapter = dataGridView1.Rows[e.RowIndex].Tag as Chapter;
            Debug.Assert(chapter != null);
            Log(string.Format(Resources.Log_Rename, chapter.Name, dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value));
            chapter.Name = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            _info.Chapters.Remove(e.Row.Tag as Chapter);
            _info.UpdataInfo((int)numericUpDown1.Value);
            if (_info.Chapters.Count < 1 || e.Row.Index != 0) return;
            TimeSpan newInitialTime = _info.Chapters.First().Time;
            _info.UpdataInfo(newInitialTime);
            if ((_rawMpls != null || _ifoGroup != null) && string.IsNullOrWhiteSpace(_chapterNameTemplate))
            {
                var name = ChapterName.GetChapterName("Chapter");
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

        private void GetFramInfo(int index = 0)
        {
            var settingAccuracy = CostumeAccuracy;

            if (cbRound.Checked)
            {
                //当未手动提供帧率[del]并且不是mpls或ifo这种已知帧率的，[/del]才进行蒙帧率操作
                index = index == 0/* && _rawMpls == null && _ifoGroup == null */? GetAutofps(settingAccuracy) : index;
                //if (index > 5) { --index; }// 跳过在30与50中间的空项
                comboBox1.SelectedIndex = index - 1;
            }
            else
            {
                index = comboBox1.SelectedIndex + 1;    //未勾选舍入时将帧率直接设置为下拉框当前帧率
            }

            var coefficient = _info.Mul1K1 ? 1.001M : 1M;
            foreach (var chapter in _info.Chapters)
            {
                var frams = (decimal) (chapter.Time.TotalMilliseconds + _info.Offset.TotalMilliseconds)
                                      *coefficient*MplsData.FrameRate[index]/1000M;
                if (cbRound.Checked)
                {
                    var rounded       = cbRound.Checked ? Math.Round(frams, MidpointRounding.AwayFromZero) : frams;
                    bool accuracy     = Math.Abs(frams - rounded) < settingAccuracy;
                    chapter.FramsInfo = $"{rounded}{(accuracy ? " K" : " *")}";
                }
                else
                {
                    chapter.FramsInfo = $"{frams}";
                }
            }
        }

        private int GetAutofps(decimal accuracy)
        {
            Log(string.Format(Resources.Log_FPS_Detect_Begin, accuracy));
            var result = MplsData.FrameRate.Select(fps  =>
                        _info.Chapters.Sum(item =>
                        item.IsAccuracy(fps, accuracy))).ToList();
            result[0] = 0; result[5] = 0; //skip two invalid frame rate.
            result.ForEach(count => Log(string.Format(Resources.Log_FPS_Detect_Count, count)));
            int autofpsCode = result.IndexOf(result.Max());
            _info.FramesPerSecond = (double) MplsData.FrameRate[autofpsCode];
            Log(string.Format(Resources.Log_FPS_Detect_Result, MplsData.FrameRate[autofpsCode]));
            return autofpsCode == 0 ? 1 : autofpsCode;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 3) return;
            Clipboard.SetText((dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string ?? "").TrimEnd('K', '*', ' '));
        }
        #endregion

        #region Form Color
        private Form3 _fcolor;

        private void Color_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (_fcolor == null)
            {
                _fcolor = new Form3(this);
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
            BordBackColor,
            TextFrontColor
        };

        public Color BackChange
        {
            set
            {
                BackColor                                    = value;
                statusStrip1.BackColor                       = value;
                //btnExpand.BackColor                          = value;
            }
            private get { return BackColor; }
        }
        public Color TextBack
        {
            set
            {
                dataGridView1.BackgroundColor                = value;
                numericUpDown1.BackColor                     = value;
                maskedTextBox1.BackColor                     = value;
                comboBox1.BackColor                          = value;
                comboBox2.BackColor                          = value;
                xmlLang.BackColor                            = value;
                savingType.BackColor                         = value;
            }
            private get { return dataGridView1.BackgroundColor; }
        }
        public Color MouseOverColor
        {
            set
            {
                btnLoad.FlatAppearance.MouseOverBackColor    = value;
                btnSave.FlatAppearance.MouseOverBackColor    = value;
                btnTrans.FlatAppearance.MouseOverBackColor   = value;
                btnLog.FlatAppearance.MouseOverBackColor     = value;
                btnPreview.FlatAppearance.MouseOverBackColor = value;
                //btnExpand.FlatAppearance.MouseOverBackColor  = value;
            }
            private get { return btnLoad.FlatAppearance.MouseOverBackColor; }
        }
        public Color MouseDownColor
        {
            set
            {
                btnLoad.FlatAppearance.MouseDownBackColor    = value;
                btnSave.FlatAppearance.MouseDownBackColor    = value;
                btnTrans.FlatAppearance.MouseDownBackColor   = value;
                btnLog.FlatAppearance.MouseDownBackColor     = value;
                btnPreview.FlatAppearance.MouseDownBackColor = value;
                //btnExpand.FlatAppearance.MouseDownBackColor  = value;
            }
            private get { return btnLoad.FlatAppearance.MouseDownBackColor; }
        }
        public Color BordBackColor
        {
            set
            {
                btnLoad.FlatAppearance.BorderColor           = value;
                btnSave.FlatAppearance.BorderColor           = value;
                btnTrans.FlatAppearance.BorderColor          = value;
                btnLog.FlatAppearance.BorderColor            = value;
                btnPreview.FlatAppearance.BorderColor        = value;
                //btnExpand.FlatAppearance.BorderColor         = value;
                dataGridView1.GridColor                      = value;
            }
            private get { return btnLoad.FlatAppearance.BorderColor; }
        }
        public Color TextFrontColor
        {
            set
            {
                ForeColor                                    = value;
                numericUpDown1.ForeColor                     = value;
                maskedTextBox1.ForeColor                     = value;
                //btnExpand.ForeColor                          = value;
                comboBox1.ForeColor                          = value;
                comboBox2.ForeColor                          = value;
                xmlLang.ForeColor                            = value;
                savingType.ForeColor                         = value;
                dataGridView1.ForeColor                      = value;
            }
            private get { return ForeColor; }
        }
        #endregion

        #region Tips
        private void lbPath_MouseEnter(object sender, EventArgs e) => toolTip1.Show(FilePath ?? "", (IWin32Window)sender);

        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            if (_rawMpls == null || !IsPathValid || !FilePath.ToLower().EndsWith(".mpls")) return;
            var streamClip = _rawMpls.ChapterClips[ClipSeletIndex];
            if (streamClip.TimeStamp.Count != 2) return;
            var deltaTime  = streamClip.TimeOut - streamClip.TimeIn - streamClip.TimeStamp.Last();
            if (deltaTime > 45000*5) return;
            toolTip1.Show($"{Resources.ToolTips_Useless_Chapter}", (IWin32Window) sender);
        }

        private void comboBox2_MouseEnter(object sender, EventArgs e)
        {
            var menuMpls = _rawMpls != null && _rawMpls.EntireClip.TimeStamp.Count < 5 && comboBox2.Items.Count > 20;
            toolTip1.Show(menuMpls ? Resources.Tips_Menu_Clip : string.Format(Resources.Tips_Clip_Count, comboBox2.Items.Count), (IWin32Window)sender);
        }

        private void ToolTipRemoveAll(object sender, EventArgs e)  => toolTip1.Hide((IWin32Window)sender);
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
            Point origin   = Location;
            Random forward = new Random();
            int forward2   = forward.Next(1, 5);
            if (forward2 % 2 == 0 || Environment.OSVersion.Version.Major == 5)
            {
                for(var i = 0; i < 100; ++i)
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
            set { panel1.Visible = value; }
        }

        private void btnExpand_Click(object sender, EventArgs e) => Form1_Resize();

        private static readonly int[] TargetHeight = new int[2];

        private void Form1_Resize()
        {
            if (!TargetHeight.Any(item => item == Height)) return;
            tsBtnExpand.Image = Resources.unfold_more;
            //btnExpand.Text = @"#";
            if (Height == TargetHeight[0])
            {
                while (Height < TargetHeight[1])
                {
                    Height += 2;
                    Application.DoEvents();
                }
            }
            else if (Height == TargetHeight[1])
            {
                while (Height > TargetHeight[0])
                {
                    Height -= 2;
                    Application.DoEvents();
                }
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
                                     Resources.File_Filter_All  + @"(*.*)|*.*";
            openFileDialog1.FileName = string.Empty;
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string chapterPath = openFileDialog1.FileName;
                    Log(string.Format(Resources.Log_Chapter_Name_Template, chapterPath));

                    return File.ReadAllBytes(chapterPath).GetUTF8String();
                }
                cbChapterName.CheckState = CheckState.Unchecked;
                return string.Empty;
            }
            catch (Exception exception)
            {
                Notification.ShowError($"Exception catched while opening file {FilePath}", exception);
                Log($"ERROR(LoadChapterName) {exception.Message}");
                return string.Empty;
            }
        }

        private string _chapterNameTemplate;

        private void cbChapterName_CheckedChanged(object sender, EventArgs e)
        {
            _chapterNameTemplate = cbChapterName.Checked ? LoadChapterName() : string.Empty;
            if (!IsPathValid) return;
            _info.UpdataInfo(_chapterNameTemplate);
            UpdataGridView(0, false);
        }

        #endregion

        private void cbMul1k1_CheckedChanged(object sender, EventArgs e)
        {
            if (_info == null) return;
            _info.Mul1K1 = cbMul1k1.Checked;
            UpdataGridView();
        }

        private void cbAutoGenName_CheckedChanged(object sender, EventArgs e) => UpdataGridView(0, false);

        private void cbShift_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsPathValid) return;
            if (cbShift.Checked)
            {
                try
                {
                    _info.Offset = maskedTextBox1.Text.ToTimeSpan();
                }
                catch (Exception)
                {
                    _info.Offset = TimeSpan.Zero;
                    tsTips.Text = Resources.Tips_Invalid_Shift_Time;
                }
            }
            else
            {
                _info.Offset = TimeSpan.Zero;
            }
            UpdataGridView();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!IsPathValid) return;
            _info.UpdataInfo((int)numericUpDown1.Value);
            UpdataGridView(0, false);
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) => tsTips.Text = Resources.Tips_Invalid_Shift_Time;
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
                _previewForm = new FormPreview(_info.GetText(cbAutoGenName.Checked), this);
            }
            _previewForm.UpdateText(_info.GetText(cbAutoGenName.Checked));
            _previewForm.Show();
            _previewForm.Focus();
            _previewForm.Select();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if (_previewForm != null)
            {
                _previewForm.Location = new Point(Location.X - _previewForm.Width, Location.Y);
            }
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
                Notification.ShowError($"Exception catched while trying to open {Path.GetFullPath(path)}", exception);
                Log($"Exception catched while trying to open {Path.GetFullPath(path)}");
            }
        }

        private void InsertMpls()
        {
            var targetPath = Path.GetDirectoryName(FilePath) + "\\..\\STREAM";
            Debug.Assert(targetPath != null);

            combineMenuStrip.Items.Add(new ToolStripSeparator());
            var fileLine = comboBox2.Text;
            foreach (var file in fileLine.Substring(0, fileLine.LastIndexOf('_') - 1).Split('&'))
            {
                ToolStripMenuItem fMenuItem = new ToolStripMenuItem(string.Format(Resources.Menu_Open_File, $"{file}.m2ts"));
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
            ToolStripMenuItem fMenuItem = new ToolStripMenuItem(string.Format(Resources.Menu_Open_File, file));
            fMenuItem.Click += (sender, args) =>
            {
                var targetFile = Path.GetDirectoryName(FilePath) + $"\\{file}";
                OpenFile(targetFile);
            };
            combineMenuStrip.Items.Add(fMenuItem);
        }

        private void InsertXpl()
        {
            var targetPath = Path.GetDirectoryName(FilePath) + "\\..\\HVDVD_TS";
            Debug.Assert(targetPath != null);
            combineMenuStrip.Items.Add(new ToolStripSeparator());
            var file = Path.GetFileName(_info.SourceName);
            ToolStripMenuItem fMenuItem = new ToolStripMenuItem(string.Format(Resources.Menu_Open_File, file));
            fMenuItem.Click += (sender, args) =>
            {
                var targetFile = $"{targetPath}\\{file}";
                OpenFile(targetFile);
            };
            combineMenuStrip.Items.Add(fMenuItem);
        }

        private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_rawMpls != null)
            {
                InsertMpls();
            }
            else if (_ifoGroup != null)
            {
                InsertIfo();
            }
            else if (_xplGroup != null)
            {
                InsertXpl();
            }
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
        private void creatZonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count < 1) return;

            var zoneRange = new List<KeyValuePair<int, int>>();
            cbRound.Checked = true;
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var rowIndex = dataGridView1.Rows.IndexOf(row);
                int nextRowIndex = rowIndex + 1;
                //todo: make last time stamp use the length of clip info.
                if (rowIndex >= dataGridView1.RowCount - 1)
                {
                    --nextRowIndex;
                }
                var currRow = _info.Chapters[rowIndex].FramsInfo;
                var nextRow = _info.Chapters[nextRowIndex].FramsInfo;

                var beginFrames = int.Parse(currRow.Substring(0, currRow.IndexOf(' ')));
                var endFrames   = int.Parse(nextRow.Substring(0, nextRow.IndexOf(' ')));
                zoneRange.Add(new KeyValuePair<int, int>(beginFrames, endFrames - 1));
            }
            string zones = zoneRange.OrderBy(item => item.Key).Aggregate(string.Empty, (current, zone) => current + $"/{zone.Key},{zone.Value},");
            string ret = "--zones " + zones.TrimStart('/');
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
    }
}
