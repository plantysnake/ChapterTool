// ****************************************************************************
//
// Copyright (C) 2014-2015 TautCony (TautCony@vcb-s.com)
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
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
        public Form1()
        {
            InitializeComponent();
        }
        public Form1(string args)
        {
            InitializeComponent();
            _paths[0] = args;
            Log($"+从运行参数中载入文件:{args}");
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            TargetHeight[0] = Height - 80;
            TargetHeight[1] = Height;
            Text = $"ChapterTool v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
            InitialLog();
            Point saved = String2Point(RegistryStorage.Load(@"Software\ChapterTool", "location"));
            if (saved  != new Point(-32000, -32000))
            {
                Location = saved;
                Log($"{Resources.Load_Position_Successful}{saved}");
            }
            LoadLang(xmlLang);
            SetDefault();
            LoadColor(this);
            Size                              = new Size(Size.Width, TargetHeight[0]);
            MoreModeShow                      = false;
            savingType.SelectedIndex          = 0;
            btnTrans.Text                     = Environment.TickCount % 2 == 0 ? "↺" : "↻";
            folderBrowserDialog1.SelectedPath = RegistryStorage.Load();
            if (!string.IsNullOrEmpty(_paths[0]))
            {
                if (Loadfile())
                {
                    UpdataGridView();
                }
                RegistryStorage.Save(Resources.How_Can_You_Find_Here, @"Software\ChapterTool", string.Empty);
            }
            var countS = RegistryStorage.Load(@"Software\ChapterTool\Statistics", @"Count");
            int count  = string.IsNullOrEmpty(countS) ? 0: int.Parse(countS);
            Log($"这是第 {count} 次启动 Chapter Tool.");
            RegistryStorage.Save($"{++count}", @"Software\ChapterTool\Statistics", @"Count");
        }

        private static void InitialLog()
        {
            Log(Environment.UserName.ToLowerInvariant().IndexOf("yzy", StringComparison.Ordinal) > 0
                ? Resources.Ye_Zong
                : $"{Environment.UserName}{Resources.Helloo}");
            Log($"{Environment.OSVersion}");

            Log(IsAdministrator() ? "噫，有权限( •̀ ω •́ )y，可以瞎搞了" : "哎，木有权限，好伤心");

            if (Environment.GetLogicalDrives().Length > 10) { Log(Resources.Hard_Drive_Plz); }
            using (var registryKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
            {
                //\HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0
                if (registryKey != null)
                {
                    Log((string)registryKey.GetValue("ProcessorNameString"));
                }
            }
            Screen.AllScreens.ToList().ForEach(item =>
                        Log($"{item.DeviceName}{Resources.Resolution}{item.Bounds.Width}*{item.Bounds.Height}"));
        }

        private static void LoadLang(ComboBox target)
        {
            target.Items.Add("----常用----"      );
            target.Items.Add("und (Undetermined)");
            target.Items.Add("eng (English     )");
            target.Items.Add("jpn (Japanese    )");
            target.Items.Add("chi (Chinese     )");
            target.Items.Add("----全部----"      );
            LanguageSelectionContainer.Languages.ToList()
                .ForEach(item => target.Items.Add($"{item.Value} ({item.Key})"));
        }

        private ChapterInfo _info;

        private void SetDefault()
        {
            //Size = new Size(Size.Width, TargetHeight[0]);
            //MoreModeShow = false;
            comboBox2.Enabled       = comboBox2.Visible = false;

            comboBox1.SelectedIndex = -1;
            btnSave.Enabled         = btnSave.Visible   = true;

            progressBar1.Visible    = true;
            cbMul1k1.Enabled        = true;

            _rawMpls                = null;
            _rawIfo                 = null;
            _xmlGroup               = null;
            _info                   = null;

            dataGridView1.Rows.Clear();
            xmlLang.SelectedIndex   = 2;
        }


        private string[] _paths = new string[20];

        private void Form1_DragDrop(object sender,  DragEventArgs e)
        {
            _paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (!IsPathValid) { return; }
            Log($"+{Resources.Load_File_By_Dragging}{_paths?[0]}");
            comboBox2.Items.Clear();
            if (Loadfile())
            {
                UpdataGridView();
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

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

        private readonly Regex _rFileType = new Regex(@"\.(txt|xml|mpls|ifo|mkv|mka)$");

        private bool IsPathValid
        {
            get
            {
                if (string.IsNullOrEmpty(_paths[0]))
                {
                    Tips.Text = Resources.File_Unloaded;
                    return false;
                }
                if (_rFileType.IsMatch(_paths[0].ToLowerInvariant())) return true;
                Tips.Text = Resources.InValid_Type;
                Log(Resources.InValid_Type_Log);
                _paths[0] = string.Empty;
                label1.Text = Resources.File_Unloaded;
                return false;
            }
        }

        private bool Loadfile()
        {
            if (!IsPathValid) { return false; }
            var fileName = Path.GetFileName(_paths[0]);
            label1.Text   = fileName?.Length > 55 ? $"{fileName.Substring(0, 40)}…{fileName.Substring(fileName.Length - 15, 15)}" : fileName;
            SetDefault();
            Cursor = Cursors.AppStarting;
            try
            {
                switch (Path.GetExtension(_paths[0])?.ToLowerInvariant())
                {
                    case ".mpls": LoadMpls();     break;
                    case ".xml":   LoadXml();     break;
                    case ".txt":   LoadOgm();     break;
                    case ".ifo":   LoadIfo();     break;
                    case ".mkv":
                    case ".mka":  LoadMatroska(); break;
                    default:
                        throw new Exception("Invalid File Format");
                }
                UpdataInfo(_chapterNameTemplate);
                progressBar1.SetState(1);
            }
            catch (Exception ex)
            {
                progressBar1.SetState(2);
                MessageBox.Show(ex.Message, Resources.ChapterTool_Error, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                progressBar1.Value = 0;
                _paths[0]   = string.Empty;
                Log($"ERROR: {ex.Message}");
                label1.Text = Resources.File_Unloaded;
                Cursor      = Cursors.Default;
                return false;
            }
            Cursor = Cursors.Default;
            return true;
        }

        private List<ChapterInfo> _rawIfo;
        private ChapterInfo _fullIfoChapter;

        private static ChapterInfo CombineChapter(List<ChapterInfo> source)
        {
            var fullChapter = new ChapterInfo
            {
                Title = "FULL Chapter",
                SourceType = "DVD",
                FramesPerSecond = source.First().FramesPerSecond
            };
            TimeSpan duration = TimeSpan.Zero;
            int index = 0;
            source.ForEach(chapterClip =>
            {
                chapterClip.Chapters.ForEach(item =>
                    fullChapter.Chapters.Add( new Chapter
                        {
                            Time = duration + item.Time,
                            Number = ++index,
                            Name = $"Chapter {index:D2}"
                        }));
                duration += chapterClip.Duration;
            });
            fullChapter.Duration = duration;
            return fullChapter;
        }

        private void LoadIfo()
        {
            _rawIfo = new IfoData().GetStreams(_paths[0]).Where(item => item != null).ToList();
            if (_rawIfo.Count == 0)
            {
                throw new Exception("No Chapter in this Ifo at all");
            }

            _fullIfoChapter = CombineChapter(_rawIfo);

            comboBox2.Items.Clear();
            comboBox2.Enabled = comboBox2.Visible = _rawIfo.Count >= 1;
            _rawIfo.ForEach(item =>
            {
                comboBox2.Items.Add($"{item.Title}_{item.SourceName}__{item.Chapters.Count}");
                int index = 0;
                item.Chapters.ForEach(chapter => chapter.Number = ++index);
                Log($" |+{item.SourceName}");
                Log($"  |+包含 {item.Chapters.Count} 个时间戳");
            });
            _info                   = _rawIfo.First();
            comboBox2.SelectedIndex = _rawIfo.IndexOf(_info);
            Tips.Text = comboBox2.SelectedIndex == -1 ? Resources.Chapter_Not_find : Resources.IFO_WARNING;
        }

        private void LoadOgm()
        {
            _info = GenerateChapterInfoFromOgm(GetUTF8String(File.ReadAllBytes(_paths[0])), (int)numericUpDown1.Value);
            progressBar1.Value = 33;
            Tips.Text = Resources.Load_Success;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = @"所有支持的类型(*.txt,*.xml,*.mpls,*.ifo,*.mkv,*.mka)|*.txt;*.xml;*.mpls;*.ifo;*.mkv;*.mka|章节文件(*.txt,*.xml,*.mpls,*.ifo)|*.txt;*.xml;*.mpls;*.ifo|Matroska文件(*.mkv,*.mka)|*.mkv;*.mka";
            try
            {
                if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
                _paths[0] = openFileDialog1.FileName;
                Log($"+从载入键中载入文件: {_paths[0]}");
                comboBox2.Items.Clear();
                if (Loadfile())
                {
                    UpdataGridView();
                }
                progressBar1.SetState(1);
            }
            catch (Exception exception)
            {
                progressBar1.SetState(2);
                MessageBox.Show($"Error opening file {_paths[0]}: {exception.Message}{Environment.NewLine}", Resources.ChapterTool_Error, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Log($"Error opening file {_paths[0]}: {exception.Message}");
            }
        }

        private void btnSave_Click(object sender, EventArgs e) => SaveFile();   //输出保存键


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
                MessageBox.Show($"Error opening path {_customSavingPath}: {exception.Message}{Environment.NewLine}", Resources.ChapterTool_Error, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Log($"Error opening path {_customSavingPath}: {exception.Message}");
            }
        }

        private readonly Regex _rLang = new Regex(@"\((?<lang>.+)\)");

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

        private void SaveFile()
        {
            if (!IsPathValid) { return; }
            string fileName = Path.GetFileNameWithoutExtension(_paths[0]);

            StringBuilder savePath =
                new StringBuilder(
                    $"{(string.IsNullOrEmpty(_customSavingPath) ? Path.GetDirectoryName(_paths[0]) : _customSavingPath)}//{fileName}");

            string ext = Path.GetExtension(_paths[0])?.ToLowerInvariant();
            if (ext == ".mpls")
                savePath.Append($"__{_info.Title}");
            if (ext == ".ifo")
                savePath.Append($"__{_info.Title}_{_info.SourceName}");

            string[] saveingTypeSuffix = { ".txt", ".xml", ".qpf", ".TimeCodes.txt", ".TsMuxeR_Meta.txt" };
            while (File.Exists($"{savePath}{saveingTypeSuffix[savingType.SelectedIndex]}")) { savePath.Append("_"); }
            savePath.Append(saveingTypeSuffix[savingType.SelectedIndex]);

            var savePathS = savePath.ToString();

            SaveInfoLog(savePathS);

            switch (savingType.SelectedIndex)
            {
                case 0://TXT
                    _info.SaveText(savePathS, cbAutoGenName.Checked);
                    break;
                case 1://XML
                    string key = _rLang.Match(xmlLang.Items[xmlLang.SelectedIndex].ToString()).Groups["lang"].ToString();
                    _info.SaveXml(savePathS, string.IsNullOrEmpty(key)? "": LanguageSelectionContainer.Languages[key], cbAutoGenName.Checked);
                    break;
                case 2://QPF
                    _info.SaveQpfile(savePathS);
                    break;
                case 3://Time Codes
                    _info.SaveTimecodes(savePathS);
                    break;
                case 4://Tsmuxer
                    _info.SaveTsmuxerMeta(savePathS);
                    break;
            }
            progressBar1.Value = 100;
            Tips.Text = @"保存成功";
        }

        private void savingType_SelectedIndexChanged(object sender, EventArgs e) => xmlLang.Enabled = savingType.SelectedIndex == 1;

        private void refresh_Click(object sender, EventArgs e) => UpdataGridView();


        #region geneRateCI

        private ChapterInfo GenerateChapterInfoFromOgm(string text, int orderOffset)
        {
            var info = new ChapterInfo { SourceType = "OGM" };
            var ogmData = text.Trim(' ', '\r', '\n').Split('\n').SkipWhile(string.IsNullOrEmpty).GetEnumerator();
            if (!ogmData.MoveNext()) return info;
            TimeSpan iniTime = OffsetCal(ogmData.Current);
            do
            {
                string buffer1 = ogmData.Current;
                ogmData.MoveNext();
                string buffer2 = ogmData.Current;
                if (string.IsNullOrEmpty(buffer1) || string.IsNullOrEmpty(buffer2))
                {
                    Log($"interrupt at '{buffer1}'  '{buffer2}'");
                    break;
                }
                if (RLineOne.IsMatch(buffer1) && RLineTwo.IsMatch(buffer2))
                {
                    info.Chapters.Add(ChapterInfo.WriteToChapterInfo(buffer1, buffer2, ++orderOffset, iniTime, cbAutoGenName.Checked));
                }
                else
                {
                    throw new FormatException($"invalid format: \n'{buffer1}' \n'{buffer2}' ");
                }
            } while (ogmData.MoveNext());
            if (info.Chapters.Count>1)
            {
                info.Duration = info.Chapters.Last().Time;
            }
            ogmData.Dispose();
            return info;
        }

        private void GetChapterInfoFromMpls(int index)
        {
            Clip mplsClip = _rawMpls.ChapterClips[index];
            _info = new ChapterInfo
            {
                Title = combineToolStripMenuItem.Checked ? "FULL Chapter" : mplsClip.Name,
                SourceType = "MPLS",
                Duration = Pts2Time(mplsClip.TimeOut - mplsClip.TimeIn),
                FramesPerSecond = (double) _frameRate[mplsClip.Fps]
            };
            var current = combineToolStripMenuItem.Checked ? _rawMpls.EntireTimeStamp : mplsClip.TimeStamp;
            if (current.Count < 2)
            {
                Tips.Text = Resources.Chapter_Not_find;
                return;
            }
            Tips.Text = Resources.Load_Success;

            int defaultOrder = 1;
            _info.Chapters = current.Select(item => new Chapter
            {
                Time   = Pts2Time(item - current.First()),
                Name   = $"Chapter {defaultOrder:D2}",
                Number = defaultOrder++
            }).ToList();
            UpdataInfo(_chapterNameTemplate);
        }

        private List<ChapterInfo> _xmlGroup;

        private void GetChapterInfoFromXml(XmlDocument doc)
        {
            _xmlGroup = PraseXml(doc);
            comboBox2.Enabled = comboBox2.Visible = _xmlGroup.Count >= 1;
            if (comboBox2.Enabled)
            {
                comboBox2.Items.Clear();
                int i = 1;
                _xmlGroup.ForEach(item =>
                {
                    string name = $"Edition {i++:D2}";
                    comboBox2.Items.Add(name);
                    Log($" |+{name}");
                    Log($"  |+包含 {item.Chapters.Count} 个时间戳");
                });
            }
            _info = _xmlGroup.First();
            //comboBox2.SelectedIndex = MplsFileSeletIndex;
        }

        private void GetChapterInfoFromIFO(int index)
        {
            _info = combineToolStripMenuItem.Checked ? _fullIfoChapter : _rawIfo[index];
        }

        #endregion

        #region updataInfo

        private void UpdataInfo(TimeSpan shift)
        {
            if (!IsPathValid) { return; }
            _info.Chapters.ForEach(item => item.Time -= shift);
        }

        private void UpdataInfo(int shift)
        {
            if (!IsPathValid) { return; }
            int index = 0;
            _info.Chapters.ForEach(item => item.Number = ++index + shift);
        }

        private void UpdataInfo(string chapterNameTemplate)
        {
            if (!IsPathValid || string.IsNullOrEmpty(chapterNameTemplate)) { return; }
            var cn = chapterNameTemplate.Trim(' ', '\r', '\n').Split('\n').ToList().GetEnumerator();
            _info.Chapters.ForEach(item => item.Name = cn.MoveNext() ? cn.Current : item.Name);
            cn.Dispose();
        }

        #endregion

        private void UpdataGridView(int fpsIndex = 0)
        {
            if (!IsPathValid || _info == null) { return; }
            switch (_info.SourceType)
            {
                case "DVD":
                    GetFramInfo(ConvertFr2Index(_info.FramesPerSecond));
                    comboBox1.Enabled     = false;
                    break;
                case "MPLS":
                    GetFramInfo(_rawMpls.ChapterClips[MplsFileSeletIndex].Fps);
                    comboBox1.Enabled     = false;
                    break;
                default:
                    GetFramInfo(fpsIndex);
                    _info.FramesPerSecond = (double)_frameRate[comboBox1.SelectedIndex];
                    comboBox1.Enabled     = true;
                    break;
            }

            bool clearAllRows = _info.Chapters.Count != dataGridView1.Rows.Count;
            if (clearAllRows) {  dataGridView1.Rows.Clear(); }
            for (var i = 0; i < _info.Chapters.Count; i++)
            {
                if (clearAllRows) { dataGridView1.Rows.Add(); }
                AddRow(_info.Chapters[i], dataGridView1.Rows[i]);
            }
            progressBar1.Value = dataGridView1.RowCount > 1 ? 66 : 33;
        }

        private void AddRow(Chapter item, DataGridViewRow row)
        {
            row.Tag = item;
            row.DefaultCellStyle.BackColor = row.Index%2 == 0
                ? ColorTranslator.FromHtml("#92AAF3")
                : ColorTranslator.FromHtml("#F3F7F7");
            row.Cells[0].Value = $"{item.Number:D2}";
            row.Cells[1].Value = item.Time2String(_info.Offset, _info.Mul1K1);
            row.Cells[2].Value = cbAutoGenName.Checked ? $"Chapter {row.Index + 1:D2}" : item.Name;
            row.Cells[3].Value = item.FramsInfo;
        }


        private decimal CostumeAccuracy => decimal.Parse(toolStripMenuItem1.DropDownItems.OfType<ToolStripMenuItem>().First(item => item.Checked).Tag.ToString());

        private void GetFramInfo(int index = 0)
        {
            var settingAccuracy = CostumeAccuracy;
            var coefficient = _info.Mul1K1 ? 1.001M : 1M;
            index = index == 0 && _rawMpls == null ? GetAutofps(settingAccuracy) : index;
            comboBox1.SelectedIndex = index - 1;
            _info.Chapters.ForEach(item =>
            {
                var frams      = (decimal) (item.Time + _info.Offset).TotalMilliseconds * coefficient * _frameRate[index]/1000M;
                var answer     = cbRound.Checked ? Math.Round(frams, MidpointRounding.AwayFromZero) : frams;
                bool accuracy  = Math.Abs(frams - answer) < settingAccuracy;
                item.FramsInfo = $"{answer}{(accuracy ? " K" : " *")}";
            });
        }

        private int GetAutofps(decimal accuracy)
        {
            Log($"|+自动帧率识别开始，允许误差为：{accuracy}");
            var settingAccuracy = CostumeAccuracy;
            var result = _frameRate.Select(fps  =>
                        _info.Chapters.Sum(item =>
                        GetAccuracy(item.Time, fps, settingAccuracy, cbRound.Checked))).ToList();
            result[0] = 0;
            result.ForEach(count => Log($" | {count:D2} 个精确点"));
            int autofpsCode = result.IndexOf(result.Max());
            Log($" |自动帧率识别结果为 {_frameRate[autofpsCode]:F4} fps");
            return autofpsCode == 0 ? 1 : autofpsCode;
        }


        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e) => UpdataGridView(comboBox1.SelectedIndex + 1);

        private void Accuracy_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStripMenuItem1.DropDownItems.OfType<ToolStripMenuItem>().ToList().ForEach(item => item.Checked = false);
            ((ToolStripMenuItem) e.ClickedItem).Checked = true;
        }

        private void cbShift_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsPathValid) { return; }
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

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) => Tips.Text = @"位移时间不科学的样子";


        #region form resize support

        private bool MoreModeShow
        {
            set
            {
                label2.Visible         = value;
                savingType.Visible     = value;
                cbAutoGenName.Visible  = value;
                label3.Visible         = value;
                numericUpDown1.Visible = value;
                cbMul1k1.Visible       = value;
                cbChapterName.Visible  = value;
                cbShift.Visible        = value;
                maskedTextBox1.Visible = value;
                btnLog.Visible         = value;
                label4.Visible         = value;
                xmlLang.Visible        = value;
            }
        }

        private void btnExpand_Click(object sender, EventArgs e) => Form1_Resize();

        private static readonly int[] TargetHeight = new int[2];

        private void Form1_Resize()
        {
            if (!TargetHeight.Any(item => item == Height)) { return; }
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
            MoreModeShow = Height == TargetHeight[1];
            btnExpand.Text  = Height == TargetHeight[0] ? "∨" : "∧";
        }

        #endregion

        private string LoadChapterName()
        {
            openFileDialog1.Filter = @"文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string chapterPath = openFileDialog1.FileName;
                    Log($"+载入自定义章节名模板：{chapterPath}");
                    return GetUTF8String(File.ReadAllBytes(chapterPath));
                }
                cbChapterName.CheckState = CheckState.Unchecked;
                progressBar1.SetState(1);
                return string.Empty;
            }
            catch (Exception exception)
            {
                progressBar1.SetState(2);
                MessageBox.Show($"Error opening file {_paths[0]}: {exception.Message}{Environment.NewLine}", Resources.ChapterTool_Error, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Log($"Error opening file {_paths[0]}: {exception.Message}");
                return string.Empty;
            }
        }

        private string _chapterNameTemplate;

       private void cbChapterName_CheckedChanged(object sender, EventArgs e)       //载入客章节模板或清除
       {
            _chapterNameTemplate = cbChapterName.Checked ? LoadChapterName() : string.Empty;
            UpdataInfo(_chapterNameTemplate);
            UpdataGridView();
        }

        private void LoadXml()
        {
            var doc = new XmlDocument();
            doc.Load(_paths[0]);
            GetChapterInfoFromXml(doc);
        }

        private readonly List<decimal> _frameRate = new List<decimal> { 0M, 24000M / 1001, 24M, 25M, 30000M / 1001, 50M, 60000M / 1001 };

        private MplsData _rawMpls;

        private int MplsFileSeletIndex => comboBox2.SelectedIndex == -1 ? 0 : comboBox2.SelectedIndex;

        private void LoadMpls()
        {
            _rawMpls = new MplsData(_paths[0]);
            Log("+成功载入MPLS格式章节文件");
            Log($"|+MPLS中共有 {_rawMpls.ChapterClips.Count} 个m2ts片段");

            comboBox2.Enabled = comboBox2.Visible = _rawMpls.ChapterClips.Count >= 1;
            if (comboBox2.Enabled)
            {
                comboBox2.Items.Clear();
                _rawMpls.ChapterClips.ForEach(item =>
                {
                    comboBox2.Items.Add($"{item.Name.Replace("M2TS", ".m2ts")}__{item.TimeStamp.Count}");
                    Log($" |+{item.Name}");
                    Log($"  |+包含 {item.TimeStamp.Count} 个时间戳");
                });
            }
            comboBox2.SelectedIndex = MplsFileSeletIndex;
            GetChapterInfoFromMpls(MplsFileSeletIndex);
        }
        /// <summary>
        /// Load chapter according to the selected index
        /// </summary>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rawMpls  != null)
            {
                GetChapterInfoFromMpls(comboBox2.SelectedIndex);
            }
            if (_xmlGroup != null)
            {
                _info = _xmlGroup[comboBox2.SelectedIndex];
            }
            if (_rawIfo   != null)
            {
                GetChapterInfoFromIFO(comboBox2.SelectedIndex);
            }
            UpdataGridView();
        }

        private void combineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_rawMpls == null && _rawIfo == null) { return; }
            combineToolStripMenuItem.Checked = !combineToolStripMenuItem.Checked;
            if (_rawMpls != null)
            {
                GetChapterInfoFromMpls(comboBox2.SelectedIndex);
            }
            if (_rawIfo  != null)
            {
                GetChapterInfoFromIFO(comboBox2.SelectedIndex);
            }
            UpdataGridView();
        }

        /// <summary>
        /// generate the chapter info from a matroska file
        /// </summary>
        private void LoadMatroska()
        {
            try
            {
                string mkvToolnixPath = RegistryStorage.Load(@"Software\ChapterTool", "mkvToolnixPath");
                if (string.IsNullOrEmpty(mkvToolnixPath) && File.Exists($"{mkvToolnixPath}/mkvextract.exe"))
                {
                    mkvToolnixPath = MatroskaInfo.GetMkvToolnixPathViaRegistry();
                    RegistryStorage.Save(mkvToolnixPath, @"Software\ChapterTool", "mkvToolnixPath");
                }
                var matroska = new MatroskaInfo(_paths[0], $"{mkvToolnixPath}/mkvextract.exe");
                GetChapterInfoFromXml(matroska.Result);
                progressBar1.SetState(1);
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                if (File.Exists("mkvextract.exe"))
                {
                    var matroska = new MatroskaInfo(_paths[0], "mkvextract.exe");
                    GetChapterInfoFromXml(matroska.Result);
                }
                else
                {
                    progressBar1.SetState(3);
                    MessageBox.Show(@"无可用 MkvExtract, 安装个呗~", Resources.ChapterTool_Error, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        #region color support

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

        #region tips support

        private void label1_MouseEnter(object sender, EventArgs e) => toolTip1.Show(_paths[0] ?? "", label1);

        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            if (!IsPathValid || !_paths[0].ToLowerInvariant().EndsWith(".mpls") || _rawMpls == null) return;
            int index = MplsFileSeletIndex;
            Clip streamClip = _rawMpls.ChapterClips[index];
            if (streamClip.TimeStamp.Count != 2) return;
            string sFakeChapter2 = $"但是这第二个章节点{Environment.NewLine}离视频结尾太近了呢，应该没有用处吧 (-｡-;)";
            string sFakeChapter3 = $"虽然只有两个章节点{Environment.NewLine}应该还是能安心的呢 (～￣▽￣)→))*￣▽￣*)o";
            string lastTime = Time2String(streamClip.TimeOut - streamClip.TimeIn);
            toolTip1.Show(
                streamClip.TimeOut - streamClip.TimeIn - (streamClip.TimeStamp.Last() - streamClip.TimeStamp.First()) <=
                5*45000
                    ? $"本片段时长为: {lastTime}，{sFakeChapter2}"
                    : $"本片段时长为: {lastTime}，{sFakeChapter3}", btnSave);
        }

        private void comboBox2_MouseEnter(object sender, EventArgs e) => toolTip1.Show(comboBox2.Items.Count > 20 ? "不用看了，这是播放菜单的mpls" : comboBox2.Items.Count.ToString(), comboBox2);
        private void cbMul1k1_MouseEnter(object sender, EventArgs e)  => toolTip1.Show("用于DVD Decrypter提取的Chapter", cbMul1k1);
        private void ToolTipRemoveAll(object sender, EventArgs e)     => toolTip1.RemoveAll();
        #endregion

        #region closing animation support

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
                }
                return;
            }
            while (Opacity > 0)
            {
                Opacity -= 0.02;
                FormMove(forward2, ref origin);
                Location = origin;
                Application.DoEvents();
            }
        }
        #endregion

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
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            UpdataInfo((int)numericUpDown1.Value);
            UpdataGridView();
        }
        private void cbMul1k1_CheckedChanged(object sender, EventArgs e)
        {
            if (_info == null) return;
            _info.Mul1K1 = cbMul1k1.Checked;
            //UpdataInfo(cbMul1k1.Checked ? 1.001M : 1/1.001M);
            UpdataGridView();
        }
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Log($"+更名: {_info.Chapters[e.RowIndex].Name} -> {dataGridView1.Rows[e.RowIndex].Cells[2].Value}");
            _info.Chapters[e.RowIndex].Name = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
        }
        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var toBeDelete = new List<int>();
            foreach (DataGridViewRow item in dataGridView1.SelectedRows)
            {
                toBeDelete.Add(item.Index);
                _info.Chapters.Remove((Chapter)item.Tag);
            }
            UpdataInfo((int)numericUpDown1.Value);
            if (_info.Chapters.Count <= 1 || toBeDelete.IndexOf(0) < 0) return;
            TimeSpan ini = _info.Chapters.First().Time;
            UpdataInfo(ini);
        }
        private void Form1_Move(object sender, EventArgs e)
        {
            if (_previewForm != null)
            {
                _previewForm.Location = new Point(Location.X - _previewForm.Width, Location.Y);
            }
        }

        private FormPreview _previewForm;
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (!IsPathValid) { return; }
            if (_previewForm == null)
            {
                _previewForm = new FormPreview(_info.GetText(cbAutoGenName.Checked), Location);
            }
            _previewForm.UpdateText(_info.GetText(cbAutoGenName.Checked));
            _previewForm.Show();
            _previewForm.Focus();
            _previewForm.Select();
        }

        private void cbAutoGenName_CheckedChanged(object sender, EventArgs e) => UpdataGridView();

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) => Log($"+ {e.RowCount} 行被删除");

        private void btnPreview_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || !RunAsAdministrator())    return;
            if (MessageBox.Show(Resources.Open_With_CT, Resources.ChapterTool_Info, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                RegistryStorage.SetOpenMethod();
            }
        }
    }
}