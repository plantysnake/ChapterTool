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
using System.Text;
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
        }

        public Form1(string args)
        {
            InitializeComponent();
            FilePath = args;
            Log($"+从运行参数中载入文件:{args}");
        }
        #endregion

        #region Inital
        private void Form1_Load(object sender, EventArgs e)
        {
            TargetHeight[0] = Height - 80;
            TargetHeight[1] = Height;
            Text = $"[VCB-Studio] ChapterTool v{Assembly.GetExecutingAssembly().GetName().Version}";
            InitialLog();
            Point saved = String2Point(RegistryStorage.Load(@"Software\ChapterTool", "location"));
            if (saved != new Point(-32000, -32000))
            {
                Location = saved;
                Log($"{Resources.Load_Position_Successful}{saved}");
            }
            LanguageSelectionContainer.LoadLang(xmlLang);
            SetDefault();
            this.LoadColor();
            Size                              = new Size(Size.Width, TargetHeight[0]);
            ExtensionPanelShow                = false;
            savingType.SelectedIndex          = 0;
            btnTrans.Text                     = Environment.TickCount % 2 == 0 ? "↺" : "↻";
            folderBrowserDialog1.SelectedPath = RegistryStorage.Load();

            if (string.IsNullOrEmpty(FilePath)) return;
            if (Loadfile()) UpdataGridView();
            RegistryStorage.Save(Resources.How_Can_You_Find_Here, @"Software\ChapterTool", string.Empty);
        }

        private static void InitialLog()
        {
            Log(Environment.UserName.ToLowerInvariant().IndexOf("yzy", StringComparison.Ordinal) > 0
                ? Resources.Ye_Zong
                : $"{Environment.UserName}{Resources.Helloo}");
            Log($"{Environment.OSVersion}");

            Log(IsAdministrator() ? "噫，有权限( •̀ ω •́ )y，可以瞎搞了" : "哎，木有权限，好伤心");

            if (Environment.GetLogicalDrives().Length > 10) Log(Resources.Hard_Drive_Plz);

            using (var registryKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
            {
                Log((string)registryKey?.GetValue("ProcessorNameString"));
            }

            foreach (var screen in Screen.AllScreens)
            {
                Log($"{screen.DeviceName}{Resources.Resolution}{screen.Bounds.Width}*{screen.Bounds.Height}");
            }

            Log($"这是第 {RegistryStorage.RegistryAddCount(@"Software\ChapterTool\Statistics", @"Count")} 次启动 Chapter Tool.");
        }

        private void SetDefault()
        {
            comboBox2.Enabled       = false;
            comboBox2.Visible       = false;

            comboBox2.SelectedIndex = -1;
            comboBox1.SelectedIndex = -1;

            _rawMpls                = null;
            _ifoGroup                 = null;
            _xmlGroup               = null;
            _info                   = null;
            _fullIfoChapter         = null;

            dataGridView1.Rows.Clear();
        }
        #endregion

        #region About Form
        private readonly int[] _poi = { 0, 10 };

        private void progressBar1_Click(object sender, EventArgs e)
        {
            ++_poi[0];
            progressBar1.SetState(_poi[0]%2 == 0?1:3);
            Log($"点击了 {_poi[0]} 次进度条");
            if (_poi[0] >= _poi[1])
            {
                Form2 version = new Form2();
                Log("打开了关于界面");
                version.Show();
                _poi[0]  = 00;
                _poi[1] += 10;
                Log("进度条点击计数清零");
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
            Log($"+{Resources.Load_File_By_Dragging}{FilePath}");
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
                    Tips.Text = Resources.File_Unloaded;
                    return false;
                }
                if (_rFileType.IsMatch(FilePath)) return true;
                Tips.Text = Resources.InValid_Type;
                Log(Resources.InValid_Type_Log + $"[{Path.GetFileName(FilePath)}]");
                FilePath = string.Empty;
                label1.Text = Resources.File_Unloaded;
                return false;
            }
        }

        private readonly Regex _rFileType = new Regex(@"\.(txt|xml|mpls|ifo|mkv|mka|cue|tak|flac)$", RegexOptions.IgnoreCase);

        private void btnLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = @"所有支持的类型(*.txt,*.xml,*.mpls,*.ifo,*.cue,*tak,*flac,*.mkv,*.mka)|*.txt;*.xml;*.mpls;*.ifo;*.cue;*.tak;*.flac;*.mkv;*.mka|" +
                                     @"章节文件(*.txt,*.xml,*.mpls,*.ifo)|*.txt;*.xml;*.mpls;*.ifo|" +
                                     @"Cue文件[包括内嵌](*.cue,*.tak,*.flac)|*.cue;*.tak;*.flac|" +
                                     @"Matroska文件(*.mkv,*.mka)|*.mkv;*.mka";
            try
            {
                if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
                FilePath = openFileDialog1.FileName;
                Log($"+从载入键中载入文件: {FilePath}");
                comboBox2.Items.Clear();
                if (Loadfile()) UpdataGridView();
                progressBar1.SetState(1);
            }
            catch (Exception exception)
            {
                progressBar1.SetState(2);
                MessageBox.Show(caption: Resources.ChapterTool_Error,
                    text: $"Error opening file {FilePath}:{Environment.NewLine} {exception.Message}",
                    buttons: MessageBoxButtons.OK,icon: MessageBoxIcon.Hand);
                Log($"Error opening file {FilePath}: {exception.Message}");
            }
        }

        private List<ChapterInfo> _ifoGroup;
        private MplsData          _rawMpls;
        private ChapterInfo       _info;
        private ChapterInfo       _fullIfoChapter;

        private bool Loadfile()
        {
            if (!IsPathValid) return false;
            var fileName = Path.GetFileName(FilePath);
            label1.Text = fileName?.Length > 55 ? $"{fileName.Substring(0, 40)}…{fileName.Substring(fileName.Length - 15, 15)}" : fileName;
            SetDefault();
            Cursor = Cursors.AppStarting;
            try
            {
                switch (Path.GetExtension(FilePath)?.ToLowerInvariant())
                {
                    case ".mpls": LoadMpls();      break;
                    case ".xml":  LoadXml();       break;
                    case ".txt":  LoadOgm();       break;
                    case ".ifo":  LoadIfo();       break;
                    case ".mkv":
                    case ".mka":  LoadMatroska();  break;
                    case ".tak":
                    case ".flac":
                    case ".cue":  LoadCue();       break;
                    default:
                        throw new Exception("Invalid File Format");
                }
                if (_info == null) return false;
                _info.UpdataInfo(_chapterNameTemplate);
                progressBar1.SetState(1);
            }
            catch (Exception exception)
            {
                progressBar1.SetState(2);
                MessageBox.Show(caption: Resources.ChapterTool_Error, text: exception.Message,
                    buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Hand);
                progressBar1.Value = 0;
                FilePath = string.Empty;
                Log($"ERROR: {exception.Message}");
                label1.Text = Resources.File_Unloaded;
                Cursor = Cursors.Default;
                return false;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
            return true;
        }

        private void LoadMpls()
        {
            _rawMpls = new MplsData(FilePath);
            Log("+成功载入MPLS格式章节文件");
            Log($"|+MPLS中共有 {_rawMpls.ChapterClips.Count} 个m2ts片段");

            comboBox2.Enabled = comboBox2.Visible = _rawMpls.ChapterClips.Count >= 1;
            if (comboBox2.Enabled)
            {
                comboBox2.Items.Clear();
                _rawMpls.ChapterClips.ForEach(item =>
                {
                    comboBox2.Items.Add($"{item.Name}__{item.TimeStamp.Count}");
                    Log($" |+{item.Name} Duration[{Pts2Time(item.Length).Time2String()}]");
                    Log($"  |+包含 {item.TimeStamp.Count} 个时间戳");
                });
            }
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
            Log($"|+IFO中共有 {_ifoGroup.Count} 个VOB片段");
            foreach (var item in _ifoGroup)
            {
                comboBox2.Items.Add($"{item.Title}_{item.SourceName}__{item.Chapters.Count}");
                int index = 0;
                item.Chapters.ForEach(chapter => chapter.Number = ++index);
                Log($" |+{item.SourceName} Duration[{item.Duration.Time2String()}]");
                Log($"  |+包含 {item.Chapters.Count} 个时间戳");
            }
            _info = combineToolStripMenuItem.Checked ? _fullIfoChapter : _ifoGroup.First();
            comboBox2.SelectedIndex = ClipSeletIndex;
            Tips.Text = comboBox2.SelectedIndex == -1 ? Resources.Chapter_Not_find : Resources.IFO_WARNING;
        }

        private void LoadOgm()
        {
            _info = OgmData.GetChapterInfo(File.ReadAllBytes(FilePath).GetUTF8String());
            _info.UpdataInfo((int)numericUpDown1.Value);
            progressBar1.Value = 33;
            Tips.Text = Resources.Load_Success;
        }

        private void LoadXml()
        {
            var doc = new XmlDocument();
            doc.Load(FilePath);
            GetChapterInfoFromXml(doc);
        }

        private void LoadMatroska()
        {
            var matroska = new MatroskaData();
            try
            {
                GetChapterInfoFromXml(matroska.GetXml(FilePath));
                progressBar1.SetState(1);
            }
            catch (Exception exception)
            {
                MessageBox.Show(caption: Resources.ChapterTool_Error, text: exception.Message,
                                buttons: MessageBoxButtons.OK,icon: MessageBoxIcon.Hand);
                Log($"ERROR: {exception.Message}");
                progressBar1.SetState(3);
            }
        }

        private void LoadCue()
        {
            try
            {
                var cue = new CueData(FilePath);
                _info = cue.Chapter;
                progressBar1.Value = 33;
                Tips.Text = Resources.Load_Success;
            }
            catch (Exception exception)
            {
                MessageBox.Show(caption: Resources.ChapterTool_Error, text: exception.Message,
                                buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                Log($"ERROR: {exception.Message}");
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
                Log($"设置保存路径为: {_customSavingPath}");
                progressBar1.SetState(1);
            }
            catch (Exception exception)
            {
                progressBar1.SetState(2);
                MessageBox.Show(caption: Resources.ChapterTool_Error,
                    text: $"Error opening path {_customSavingPath}:{Environment.NewLine} {exception.Message}",
                    buttons: MessageBoxButtons.OK,icon: MessageBoxIcon.Hand);
                Log($"Error opening path {_customSavingPath}: {exception.Message}");
            }
        }

        private void SaveInfoLog(string savePath)
        {
            Log("+保存信息");
            switch (_info.SourceType)
            {
                case "MPLS":
                    Log($"|+对应视频文件: {_info.Title}");
                    break;
                case "DVD":
                    Log($"|+对应视频文件: {_info.Title}_{_info.SourceName}");
                    break;
                case "OGM":
                    break;
            }
            Log($"|+保存文件名: {savePath}");
            Log($" |+保存格式: {savingType.SelectedItem}");
            if (savingType.SelectedIndex == 1)
            {
                Log($"  |+语言选择: {xmlLang.Items[xmlLang.SelectedIndex]}");
            }
            Log($"|+使用章节名: {!cbAutoGenName.Checked}");
            Log($" |+使用章节名模板: {cbChapterName.Checked}");
            Log($"|+章节号平移: {numericUpDown1.Value}");
            Log($"|+章节开始时间 x 1.001: {cbMul1k1.Checked}");
            Log($"|+时间平移: {cbShift.Checked}");
            if (cbShift.Checked)
            {
                Log($" |+平移量 {_info.Offset.Time2String()}");
            }
        }

        private string GeneRateSavePath(int saveType)
        {
            var rootPath = string.IsNullOrWhiteSpace(_customSavingPath) ? Path.GetDirectoryName(FilePath) : _customSavingPath;
            var fileName = Path.GetFileNameWithoutExtension(FilePath);
            StringBuilder savePath = new StringBuilder($"{rootPath}\\{fileName}");

            var ext = Path.GetExtension(FilePath)?.ToLowerInvariant();
            if (ext == ".mpls")
                savePath.Append($"__{_info.Title}");
            if (ext == ".ifo")
                savePath.Append($"__{_info.Title}_{_info.SourceName}");

            string[] saveingTypeSuffix = { ".txt", ".xml", ".qpf", ".TimeCodes.txt", ".TsMuxeR_Meta.txt" };
            while (File.Exists($"{savePath}{saveingTypeSuffix[saveType]}")) savePath.Append("_");
            savePath.Append(saveingTypeSuffix[saveType]);

             return savePath.ToString();
        }

        private readonly Regex _rLang = new Regex(@"\((?<lang>.+)\)");

        private void SaveFile(int saveType)
        {
            if (!IsPathValid) return;//防止保存先于载入

            var savePath = GeneRateSavePath(saveType);

            SaveInfoLog(savePath);

            switch (saveType)
            {
                case 0://TXT
                    _info.SaveText(savePath, cbAutoGenName.Checked);
                    break;
                case 1://XML
                    string key = _rLang.Match(xmlLang.Items[xmlLang.SelectedIndex].ToString()).Groups["lang"].ToString();
                    _info.SaveXml(savePath, string.IsNullOrWhiteSpace(key)? "": LanguageSelectionContainer.Languages[key], cbAutoGenName.Checked);
                    break;
                case 2://QPF
                    _info.SaveQpfile(savePath);
                    break;
                case 3://Time Codes
                    _info.SaveTimecodes(savePath);
                    break;
                case 4://Tsmuxer
                    _info.SaveTsmuxerMeta(savePath);
                    break;
            }
            progressBar1.Value = 100;
            Tips.Text = @"保存成功";
        }
        #endregion

        #region Contorl Panel
        private int ClipSeletIndex => comboBox2.SelectedIndex < 0 ? 0 : comboBox2.SelectedIndex;

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e) => UpdataGridView(comboBox1.SelectedIndex + 1);

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
            _info.Mul1K1 = cbMul1k1.Checked;
            UpdataGridView();
        }

        private void combineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_rawMpls == null && _ifoGroup == null) return;
            combineToolStripMenuItem.Checked = !combineToolStripMenuItem.Checked;
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
            _info = _rawMpls.ToChapterInfo(index, combineToolStripMenuItem.Checked);
            Tips.Text = _info.Chapters.Count < 2 ? Resources.Chapter_Not_find : Resources.Load_Success;
            _info.UpdataInfo(_chapterNameTemplate);
        }

        private void GetChapterInfoFromIFO(int index)
        {
            _info = combineToolStripMenuItem.Checked ? _fullIfoChapter : _ifoGroup[index];
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
                _xmlGroup.ForEach(item =>
                {
                    var name = $"Edition {i++:D2}";
                    comboBox2.Items.Add(name);
                    Log($" |+{name}");
                    Log($"  |+包含 {item.Chapters.Count} 个时间戳");
                });
            }
            _info = _xmlGroup.First();
            comboBox2.SelectedIndex = ClipSeletIndex;
            Tips.Text = Resources.Load_Success;
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
                    _info.FramesPerSecond = (double)_frameRate[comboBox1.SelectedIndex];
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
                    dataGridView1.Rows[i].EditRow(_info, cbAutoGenName.Checked);
                }
                Application.DoEvents();
            }
            progressBar1.Value = dataGridView1.RowCount > 1 ? 66 : 33;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var chapter = dataGridView1.Rows[e.RowIndex].Tag as Chapter;
            Debug.Assert(chapter != null);
            Log($"+更名: {chapter.Name} -> {dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value}");
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
                var name = new ChapterName();
                _info.Chapters.ForEach(item => item.Name = name.Get());
            }
            Application.DoEvents();
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            Log($"+ {e.RowCount} 行(i = {e.RowIndex})被删除");
        }
        #endregion

        #region Frame Info
        private readonly List<decimal> _frameRate = new List<decimal> { 0M, 24000M / 1001, 24M, 25M, 30000M / 1001, 50M, 60000M / 1001 };

        private decimal CostumeAccuracy => decimal.Parse(toolStripMenuItem1.DropDownItems.OfType<ToolStripMenuItem>().First(item => item.Checked).Tag.ToString());

        private void Accuracy_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStripMenuItem1.DropDownItems.OfType<ToolStripMenuItem>().ToList().ForEach(item => item.Checked = false);
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
        }

        private void GetFramInfo(int index = 0)
        {
            var settingAccuracy = CostumeAccuracy;

            if (cbRound.Checked)
            {
                //当未手动提供帧率并且不是mpls或ifo这种已知帧率的，才进行蒙帧率操作
                index = index == 0 && _rawMpls == null && _ifoGroup == null ? GetAutofps(settingAccuracy) : index;
                comboBox1.SelectedIndex = index - 1;
            }
            else
            {
                index = comboBox1.SelectedIndex + 1;    //未勾选舍入时将帧率直接设置为下拉框当前帧率
            }

            var coefficient = _info.Mul1K1 ? 1.001M : 1M;
            foreach (var chapter in _info.Chapters)
            {
                var frams = (decimal)(chapter.Time + _info.Offset).TotalMilliseconds * coefficient * _frameRate[index] / 1000M;
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
            Log($"|+自动帧率识别开始，允许误差为：{accuracy}");
            var result = _frameRate.Select(fps  =>
                        _info.Chapters.Sum(item =>
                        item.GetAccuracy(fps, accuracy))).ToList();
            result[0] = 0;
            result.ForEach(count => Log($" | {count:D2} 个精确点"));
            int autofpsCode = result.IndexOf(result.Max());
            Log($" |自动帧率识别结果为 {_frameRate[autofpsCode]:F4} fps");
            return autofpsCode == 0 ? 1 : autofpsCode;
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
            Log("颜色设置窗口被打开");
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
                btnExpand.BackColor                          = value;
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
                btnExpand.FlatAppearance.MouseOverBackColor  = value;
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
                btnExpand.FlatAppearance.MouseDownBackColor  = value;
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
                btnExpand.FlatAppearance.BorderColor         = value;
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
                btnExpand.ForeColor                          = value;
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
        private void label1_MouseEnter(object sender, EventArgs e)        => toolTip1.Show(FilePath ?? "", (IWin32Window)sender);

        private readonly string _fakeChapter = $"但是这第二个章节点{Environment.NewLine}离视频结尾太近了呢，应该没有用处吧 (-｡-;)";
        private readonly string _trueChapter = $"虽然只有两个章节点{Environment.NewLine}应该还是能安心的呢 (～￣▽￣)→))*￣▽￣*)o";

        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            if (_rawMpls == null || !IsPathValid || !FilePath.ToLowerInvariant().EndsWith(".mpls")) return;
            var streamClip    = _rawMpls.ChapterClips[ClipSeletIndex];
            if (streamClip.TimeStamp.Count != 2) return;
            int totalTime     = streamClip.TimeOut - streamClip.TimeIn;
            int ptsDelta      = totalTime - streamClip.TimeStamp.Last();
            toolTip1.Show($"本片段时长为: {Pts2Time(totalTime).Time2String()}, {(ptsDelta <= 5*45000 ? _fakeChapter : _trueChapter)}", (IWin32Window) sender);
        }

        private void comboBox2_MouseEnter(object sender, EventArgs e)
        {
            var menuMpls = _rawMpls != null && _rawMpls.EntireTimeStamp.Count < 5 && comboBox2.Items.Count > 20;
            toolTip1.Show(menuMpls ? "不用看了，这是播放菜单的mpls" : $"共 {comboBox2.Items.Count} 个片段", (IWin32Window)sender);
        }

        private void cbMul1k1_MouseEnter(object sender, EventArgs e)      => toolTip1.Show("用于DVD Decrypter提取的Chapter", (IWin32Window)sender);

        private void cbChapterName_MouseEnter(object sender, EventArgs e) => toolTip1.Show("不取消勾选时将持续生效", (IWin32Window)sender);

        private void cbAutoGenName_MouseEnter(object sender, EventArgs e) => toolTip1.Show("将章节名重新从Chapter 01开始标记", (IWin32Window)sender);

        private void cbRound_MouseEnter(object sender, EventArgs e)       => toolTip1.Show("右键菜单可设置误差范围", (IWin32Window)sender);

        private void ToolTipRemoveAll(object sender, EventArgs e)         => toolTip1.Hide((IWin32Window)sender);
        #endregion

        #region Close Form
        private static void FormMove(int forward,ref Point p)
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
            set
            {
                label2.Visible = value;
                savingType.Visible = value;
                cbAutoGenName.Visible = value;
                label3.Visible = value;
                numericUpDown1.Visible = value;
                cbMul1k1.Visible = value;
                cbChapterName.Visible = value;
                cbShift.Visible = value;
                maskedTextBox1.Visible = value;
                btnLog.Visible = value;
                label4.Visible = value;
                xmlLang.Visible = value;
            }
        }

        private void btnExpand_Click(object sender, EventArgs e) => Form1_Resize();

        private static readonly int[] TargetHeight = new int[2];

        private void Form1_Resize()
        {
            if (!TargetHeight.Any(item => item == Height)) return;
            btnExpand.Text = @"#";
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
            btnExpand.Text = Height == TargetHeight[0] ? "∨" : "∧";
        }
        #endregion

        private void savingType_SelectedIndexChanged(object sender, EventArgs e) => xmlLang.Enabled = savingType.SelectedIndex == 1;

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
            openFileDialog1.Filter = @"文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string chapterPath = openFileDialog1.FileName;
                    Log($"+载入自定义章节名模板：{chapterPath}");
                    return File.ReadAllBytes(chapterPath).GetUTF8String();
                }
                cbChapterName.CheckState = CheckState.Unchecked;
                progressBar1.SetState(1);
                return string.Empty;
            }
            catch (Exception exception)
            {
                progressBar1.SetState(2);
                MessageBox.Show(caption: Resources.ChapterTool_Error,
                    text: $"Error opening file {FilePath}:{Environment.NewLine} {exception.Message}",
                    buttons: MessageBoxButtons.OK,icon: MessageBoxIcon.Hand);
                Log($"Error opening file {FilePath}: {exception.Message}");
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
                    Tips.Text = @"位移时间不科学的样子";
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

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) => Tips.Text = @"位移时间不科学的样子";
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
            if (MessageBox.Show(Resources.Open_With_CT, Resources.ChapterTool_Info, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                RegistryStorage.SetOpenMethod();
            }
        }
        #endregion
    }
}
