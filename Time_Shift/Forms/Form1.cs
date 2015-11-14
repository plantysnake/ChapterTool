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
using ChapterTool.Util;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;


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
            paths[0] = args;
            CTLogger.Log(string.Format("+从运行参数中载入文件:{0}", args));
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Point saved = convertMethod.string2point(registryStorage.Load(@"Software\ChapterTool", "location"));
            if (saved != new Point(-32000, -32000))
            {
                Location = saved;
                CTLogger.Log(string.Format("成功载入保存的窗体位置{0}", saved));
            }

            initialLog();
            loadLang();
            setDefault();
            convertMethod.loadColor(this);
            moreModeShow = false;
            Size = new Size(Size.Width, Size.Height - 80);
            savingType.SelectedIndex = 0;
            btnTrans.Text = ((Environment.TickCount % 2 == 0) ? "↺" : "↻");
            folderBrowserDialog1.SelectedPath = registryStorage.Load();

            if (!string.IsNullOrEmpty(paths[0]))
            {
                Loadfile();
                updataGridView();
                registryStorage.Save("你好呀，找到这里来干嘛呀", @"Software\ChapterTool", string.Empty);
            }
        }

        void initialLog()
        {
            if (Environment.UserName.ToLowerInvariant().IndexOf("yzy") > 0) { CTLogger.Log("武总好~"); }
            else { CTLogger.Log(Environment.UserName + "，你好呀"); }
            CTLogger.Log(Environment.OSVersion.ToString());
            if (Environment.GetLogicalDrives().Length > 10) { CTLogger.Log("硬盘壕，给我块硬盘呗~"); }
            using (Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
            {
                string processor = string.Empty;
                //\HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0
                if (registryKey != null)
                {
                    processor = (string)registryKey.GetValue("ProcessorNameString");
                    registryKey.Close();
                }
                CTLogger.Log(processor);
            }
            Screen.AllScreens.ToList().ForEach(item => CTLogger.Log(string.Format("{0} 分辨率：{1}*{2}", item.DeviceName, item.Bounds.Width, item.Bounds.Height)));
        }


        void loadLang()
        {
            xmlLang.Items.Add("---常用---");
            xmlLang.Items.Add("und (Undetermined)");
            xmlLang.Items.Add("eng (English)");
            xmlLang.Items.Add("jpn (Japanese)");
            xmlLang.Items.Add("chi (Chinese)");
            xmlLang.Items.Add("---全部---");

            foreach (var item in LanguageSelectionContainer.Languages)
            {
                xmlLang.Items.Add(string.Format("{0} ({1})", item.Value, item.Key));
            }
        }


        ChapterInfo info;

        string SchapterFitter
        {
            get
            {
                if (File.Exists("mkvextract.exe"))
                {
                    return "所有支持的类型(*.txt,*.xml,*.mpls,*.ifo,*.mkv,*.mka)|*.txt;*.xml;*.mpls;*.ifo;*.mkv;*.mka|章节文件(*.txt,*.xml,*.mpls,*.ifo)|*.txt;*.xml;*.mpls;*.ifo|Matroska文件(*.mkv,*.mka)|*.mkv;*.mka";
                }
                else
                {
                    return "章节文件(*.txt,*.xml,*.mpls,*.ifo)|*.txt;*.xml;*.mpls;*.ifo";
                }
            }
        }
        string SnotLoaded = "尚未载入文件";
        string Ssuccess = "载入完成 (≧▽≦)";
        string Swhatsthis2 = "当前片段并没有章节 (¬_¬)";


        void setDefault()
        {
            cbMore.CheckState = CheckState.Unchecked;
            moreModeShow = false;
            comboBox2.Enabled = comboBox2.Visible = false;

            comboBox1.SelectedIndex = -1;
            btnSave.Enabled = btnSave.Visible = true;

            progressBar1.Visible = true;
            cbMore.Enabled = true;
            cbMul1k1.Enabled = true;


            RawData  = null;
            XMLGroup = null;
            xmlLang.SelectedIndex = 2;
        }
        Regex RLineOne    = new Regex(@"CHAPTER\d+=\d+:\d+:\d+\.\d+");
        Regex RLineTwo    = new Regex(@"CHAPTER\d+NAME=(?<chapterName>.*)");

        string[] paths = new string[20];

        void Form1_DragDrop(object sender,  DragEventArgs e)
        {
            paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (!isPathValid) { return; }
            CTLogger.Log("+从窗口拖拽中载入文件:" + paths[0]);
            comboBox2.Items.Clear();
            Loadfile();
            updataGridView();
        }
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) { e.Effect = DragDropEffects.Copy; }
            else { e.Effect = DragDropEffects.None; }
        }


        int  poi = 0, nico = 10;

        void progressBar1_Click(object sender, EventArgs e)
        {
            ++poi;
            CTLogger.Log(string.Format("点击了 {0} 次进度条", poi));
            if (poi >= nico)
            {
                Form2 version = new Form2();
                CTLogger.Log("打开了关于界面");
                version.Show();
                poi   = 0;
                nico += 10;
                CTLogger.Log("进度条点击计数清零");
            }
            if (poi < 3 && nico == 10)
            {
                MessageBox.Show("Something happened", "Something happened");
            }
        }

        Regex RFileType = new Regex(@".(txt|xml|mpls|ifo|mkv|mka)$");

        bool isPathValid
        {
            get
            {
                if (string.IsNullOrEmpty(paths[0]))
                {
                    Tips.Text = "文件还没载入呢";
                    return false;
                }
                if (!RFileType.IsMatch(paths[0].ToLowerInvariant()))
                {
                    Tips.Text = "这个文件我不认识啊 _ (:3」∠)_";
                    CTLogger.Log("文件格式非法");
                    paths[0] = string.Empty;
                    label1.Text = SnotLoaded;
                    return false;
                }
                return true;
            }
        }


        void Loadfile()
        {
            if (!isPathValid) { return; }
            label1.Text = (paths[0].Length > 55) ? paths[0].Substring(0, 40) + "……" + paths[0].Substring(paths[0].Length - 15, 15) : paths[0];

            setDefault();
            Cursor = Cursors.AppStarting;
            try
            {
                switch (RFileType.Match(paths[0].ToLowerInvariant()).Value)
                {
                    case ".mpls": loadMPLS(); break;
                    case ".xml":   loadXML(); break;
                    case ".txt":   loadOGM(); break;
                    case ".ifo":   LoadIFO(); break;
                    case ".mkv":
                    case ".mka": if (File.Exists("mkvextract.exe")) { loadMatroska(); } break;
                }
                if (!string.IsNullOrEmpty(chapterNameTemplate)) { updataInfo(chapterNameTemplate); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CTLogger.Log("ERROR: " + ex.Message);
                label1.Text = SnotLoaded;
            }
            Cursor = Cursors.Default;

        }

        List<ChapterInfo> Rawifo;

        void LoadIFO()
        {
            Rawifo = new ifoData().GetStreams(paths[0]);
            if (Rawifo[0].FramesPerSecond != 25)
            {
                IFOmul1k1();
            }
            comboBox2.Items.Clear();
            comboBox2.Enabled = comboBox2.Visible = (Rawifo.Count >= 1);
            foreach (var item in Rawifo)
            {
                if (comboBox2.Enabled && item != null)
                {
                    comboBox2.Items.Add(string.Format("{0}__{1}", item.SourceName, item.Chapters.Count));
                    CTLogger.Log(string.Format(" |+{0}", item.SourceName));
                    CTLogger.Log(string.Format("  |+包含 {0} 个时间戳", item.Chapters.Count));
                }
            }
            int i = 0;
            foreach (var item in Rawifo)
            {
                if (item != null)
                {
                    info = item;
                    geneRateCI(i, true);
                    comboBox2.SelectedIndex = i;
                    //updataInfo(1.001M);
                    break;
                }
                ++i;
            }
            Tips.Text = (comboBox2.SelectedIndex == -1) ? Swhatsthis2 : Ssuccess;
        }

        void IFOmul1k1()
        {
            foreach (var item in Rawifo.Where(item => item != null))
            {
                item.Chapters.ForEach((item2) => item2.Time = convertMethod.pts2Time((int)((decimal)item2.Time.TotalSeconds * 1.001M * 45000M)));
            }
        }



        void loadOGM()
        {
            byte[] buffer = File.ReadAllBytes(paths[0]);
            geneRateCI(convertMethod.GetUTF8String(buffer));
            progressBar1.Value = 33;
            Tips.Text = Ssuccess;
        }

        void btnLoad_Click(object sender, EventArgs e)                  //载入键
        {
            openFileDialog1.Filter = SchapterFitter;

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                paths[0] = openFileDialog1.FileName;
                CTLogger.Log("+从载入键中载入文件:" + paths[0]);
                comboBox2.Items.Clear();
                Loadfile();
                updataGridView();
            }
        }

        void btnSave_Click(object sender, EventArgs e) { saveFile(); }  //输出保存键

        string CUSTsavingPath = string.Empty;

        private void btnSave_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        CUSTsavingPath = folderBrowserDialog1.SelectedPath;
                        registryStorage.Save(CUSTsavingPath);
                        CTLogger.Log("设置保存路径为:" + CUSTsavingPath);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Error opening path {0}: {1}{2}", CUSTsavingPath, exception.Message, Environment.NewLine), "ChapterTool Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                CTLogger.Log(string.Format("Error opening path {0}: {1}", CUSTsavingPath, exception.Message));
            }
        }

        Regex RLang = new Regex(@"\((?<lang>.+)\)");

        void saveFile()
        {
            if (!isPathValid) { return; }


            string PN = paths[0].Substring(0, paths[0].LastIndexOf("."));
            //modify for custom saving path
            int slashPosition = paths[0].LastIndexOf(@"\");
            if (!string.IsNullOrEmpty(CUSTsavingPath))
                PN = CUSTsavingPath + paths[0].Substring(slashPosition, paths[0].LastIndexOf(".") - slashPosition);
            StringBuilder SavePath = new StringBuilder(PN);

            if (paths[0].ToLowerInvariant().EndsWith(".mpls") && !combineToolStripMenuItem.Checked)
                SavePath.Append("__" + RawData.chapterClips[mplsFileSeletIndex].Name);
            if (paths[0].ToLowerInvariant().EndsWith(".ifo"))
                SavePath.Append("__" + Rawifo[mplsFileSeletIndex].SourceName);

            switch (savingType.SelectedIndex)
            {
                case 0://TXT
                    while (File.Exists(SavePath + ".txt")) { SavePath.Append("_"); }
                    SavePath.Append(".txt");
                    info.SaveText(SavePath.ToString(), cbAutoGenName.Checked);
                    break;
                case 1://XML
                    while (File.Exists(SavePath + ".xml")) { SavePath.Append("_"); }
                    SavePath.Append(".xml");
                    string key = RLang.Match(xmlLang.Items[xmlLang.SelectedIndex].ToString()).Groups["lang"].ToString();
                    info.SaveXml(SavePath.ToString(),string.IsNullOrEmpty(key)? "": LanguageSelectionContainer.Languages[key]);
                    break;
                case 2://QPF
                    while (File.Exists(SavePath + ".qpf")) { SavePath.Append("_"); }
                    SavePath.Append(".qpf");
                    info.SaveQpfile(SavePath.ToString());
                    break;
            }
            progressBar1.Value = 100;
            Tips.Text = "保存成功";
            //MessageBox.Show(savePath);
        }

        private void savingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            xmlLang.Enabled = (savingType.SelectedIndex == 1);
        }

        void refresh_Click(object sender, EventArgs e) { updataGridView(); }


        string GetFirstLine_new(string[] OGMdata, out int i)//i 为章节实际开始的行号
        {
            i = 0;
            foreach (var item in OGMdata)
            {
                if (!string.IsNullOrEmpty(item)) { return item; }
                ++i;
            }
            return string.Empty;
        }

        TimeSpan OffsetCal_new(string line)//获取第一行的时间
        {
            if (RLineOne.IsMatch(line))
            {
                return convertMethod.string2Time(convertMethod.RTimeFormat.Match(line).Value);
            }
            else
            {
                CTLogger.Log(string.Format("ERROR: {0} <-该行与时间行格式不匹配", line));
                return TimeSpan.Zero;
            }
        }
        Chapter WriteToChapterInfo(string line, string line2, int order,TimeSpan iniTime)
        {
            Chapter temp = new Chapter();
            if (RLineTwo.IsMatch(line2))                     //章节标题行
            {
                switch (!cbAutoGenName.Checked)
                {
                    case true:
                        temp.Name = RLineTwo.Match(line2).Groups["chapterName"].Value; break;
                    case false:
                        temp.Name = string.Format("Chapter {0:D2}", order); break;
                }
            }
            if (RLineOne.IsMatch(line))
            {
                temp.Time = convertMethod.string2Time(convertMethod.RTimeFormat.Match(line).Value) - iniTime;
            }
            temp.Number = order;
            return temp;
        }
        #region geneRateCI
        void geneRateCI(string text)
        {
            string[] OGMdata = text.Split('\n');
            info = new ChapterInfo();
            info.SourceHash = ifoData.ComputeMD5Sum(paths[0]);

            info.SourceType = "OGM";
            int i;
            TimeSpan iniTime =  OffsetCal_new(GetFirstLine_new(OGMdata, out i));
            int order = 1 + (int)numericUpDown1.Value;
            if (OGMdata.Length == 1) { return; }
            string buffer1, buffer2;
            if (RLineOne.IsMatch(OGMdata[i]))
            {
                for (; i + 1 < OGMdata.Length; i += 2, ++order)
                {
                    buffer1 = OGMdata[i];
                    buffer2 = OGMdata[i + 1];
                    if (string.IsNullOrEmpty(buffer1) || string.IsNullOrEmpty(buffer2)) { break; }
                    info.Chapters.Add(WriteToChapterInfo(buffer1, buffer2, order, iniTime));
                }
            }
            if (info.Chapters.Count>1)
            {
                info.Duration = info.Chapters[info.Chapters.Count - 1].Time;
            }
        }
        void geneRateCI(int index)
        {
            Clip mplsClip = RawData.chapterClips[index];
            info = new ChapterInfo();
            info.SourceHash = ifoData.ComputeMD5Sum(paths[0]);
            info.Duration = convertMethod.pts2Time(mplsClip.TimeOut - mplsClip.TimeIn);
            info.SourceType = "MPLS";
            info.FramesPerSecond = (double)FrameRate[RawData.chapterClips[index].fps];
            List<int> current;
            if (combineToolStripMenuItem.Checked)
            {
                current = RawData.entireTimeStamp;
                info.Title = "FULL Chapter";
            }
            else
            {
                current = mplsClip.timeStamp;
                info.Title = RawData.chapterClips[index].Name;
            }
            if (current.Count < 2)
            {
                Tips.Text = Swhatsthis2;
                return;
            }
            else
            {
                Tips.Text = Ssuccess;
            }
            int offset = current[0];

            int default_order = 1;
            foreach (int item in current)
            {
                Chapter temp = new Chapter();
                temp.Time = convertMethod.pts2Time(item - offset);
                temp.Name = string.Format("Chapter {0:D2}", default_order);
                temp.Number = default_order++;
                info.Chapters.Add(temp);
            }
            if (!string.IsNullOrEmpty(chapterNameTemplate)) { updataInfo(chapterNameTemplate); }
        }


        List<ChapterInfo> XMLGroup;


        void geneRateCI(int index, bool DVD, bool XML)
        {
            info = XMLGroup[index];
        }

        void geneRateCI(XmlDocument doc)
        {
            XMLGroup = convertMethod.PraseXML(doc);
            comboBox2.Enabled = comboBox2.Visible = (XMLGroup.Count >= 1);
            if (comboBox2.Enabled)
            {
                comboBox2.Items.Clear();
                int i = 1;
                foreach (var item in XMLGroup)
                {
                    string name = string.Format("Edition {0:D2}", i++);
                    comboBox2.Items.Add(name);
                    CTLogger.Log(string.Format(" |+{0}", name));
                    CTLogger.Log(string.Format("  |+包含 {0} 个时间戳", item.Chapters.Count));
                }
            }
            info = XMLGroup[0];
            comboBox2.SelectedIndex = mplsFileSeletIndex;
        }

        void geneRateCI(int index,bool DVD)
        {
            if (Rawifo[index] != null)
            {
                info = Rawifo[index];
                updataGridView();
            }
            else
            {
                Tips.Text = Swhatsthis2;
            }
        }
        #endregion

        #region updataInfo
        void updataInfo(TimeSpan shift)
        {
            if (!isPathValid) { return; }
            info.Chapters.ForEach(item => item.Time -= shift);
        }
        void updataInfo(int shift)
        {
            if (!isPathValid) { return; }
            int i = 1;
            info.Chapters.ForEach(item => item.Number = i++ + shift);
        }
        void updataInfo(string chapterName)
        {
            if (!isPathValid) { return; }
            List<string>.Enumerator cn = chapterName.Split('\n').ToList().GetEnumerator();
            foreach (var item in info.Chapters)
            {
                if(cn.MoveNext())
                {
                    item.Name = cn.Current;
                }
            }
        }
        void updataInfo(decimal coefficient)
        {
            if (!isPathValid) { return; }
            info.Chapters.ForEach(item => item.Time = convertMethod.pts2Time((int)((decimal)item.Time.TotalSeconds * coefficient * 45000M)));
        }
        #endregion



        void updataGridView(int fpsIndex = 0)
        {
            if (!isPathValid || info == null) { return; }
            switch (info.SourceType)
            {
                case "DVD":
                    getFramInfo(convertMethod.convertFR2Index(info.FramesPerSecond));
                    comboBox1.Enabled = false;
                    break;
                case "MPLS":
                    int _index = RawData.chapterClips[mplsFileSeletIndex].fps;
                    getFramInfo(_index);
                    comboBox1.Enabled = false;
                    break;
                default:
                    getFramInfo(fpsIndex);
                    info.FramesPerSecond = (double)FrameRate[comboBox1.SelectedIndex];
                    comboBox1.Enabled = true;
                    break;
            }

            bool ClearOrNot = info.Chapters.Count != dataGridView1.Rows.Count;
            if (ClearOrNot)
            {
                dataGridView1.Rows.Clear();
            }

            for (int i = 0; i < info.Chapters.Count; i++)
            {
                if (ClearOrNot) { dataGridView1.Rows.Add(); }
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = ((i % 2 == 0) ? Color.FromArgb(0x92,0xaa,0xf3) : Color.FromArgb(0xf3, 0xf7, 0xf7));
                addRow(info.Chapters[i], i);
            }
            progressBar1.Value = (dataGridView1.RowCount > 1) ? 66 : 33;
        }


        void addRow(Chapter item,int index)
        {
            dataGridView1.Rows[index].Tag = item;
            dataGridView1.Rows[index].Cells[0].Value = item.Number.ToString("00");
            dataGridView1.Rows[index].Cells[1].Value = convertMethod.time2string(item.Time + info.offset);
            if (cbAutoGenName.Checked)
                dataGridView1.Rows[index].Cells[2].Value = string.Format("Chapter {0:D2}", (index + 1));
            else
                dataGridView1.Rows[index].Cells[2].Value = item.Name;
            dataGridView1.Rows[index].Cells[3].Value = item.FramsInfo;
        }

        /// FPS Cal Part /////////////////////
        decimal costumeAccuracy = 0.15M;
        void getFramInfo(int index = 0)
        {
            if (index == 0)
            {
                index = getAUTOFPS();
                comboBox1.SelectedIndex = index - 1;
            }
            else
            {
                comboBox1.SelectedIndex = index - 1;
            }

            foreach (var item in info.Chapters)
            {
                TimeSpan _current = item.Time;
                decimal Frams     = ((decimal)_current.TotalMilliseconds * FrameRate[index] / 1000M);
                decimal answer    = cbRound.Checked ? Math.Round(Frams, MidpointRounding.AwayFromZero) : Frams;
                bool accuracy     = (Math.Abs(Frams - answer) < costumeAccuracy);
                item.FramsInfo = string.Format("{0}{1}", answer, (accuracy ? " K" : " *"));
            }
        }
        int getAUTOFPS()
        {
            int currentMaxOne = 0; int AUTOFPS_code = 1;
            CTLogger.Log(string.Format("|+自动帧率识别开始，允许误差为：{0}", costumeAccuracy));
            for (int j = 1; j < 7; ++j)
            {
                int AccuratePiont = 0;
                int InAccuratePiont = 0;

                info.Chapters.ForEach((item) => getAccuracy(item.Time, ref AccuratePiont, ref InAccuratePiont, j));

                if (currentMaxOne < AccuratePiont)
                {
                    AUTOFPS_code = j;
                    currentMaxOne = AccuratePiont;
                }
                CTLogger.Log(string.Format(" |fps= {0:F4} 时，精确点：{1:D2} 个，非精确点：{2:D2} 个", FrameRate[j], AccuratePiont, InAccuratePiont));
            }
            CTLogger.Log(string.Format(" |自动识别结果为 {0:F4} fps", FrameRate[AUTOFPS_code]));
            return AUTOFPS_code;
        }


        void getAccuracy(TimeSpan time, ref int AccuratePiont, ref int InAccuratePiont,int index)//framCal
        {
            decimal Frams = ((decimal)time.TotalMilliseconds * FrameRate[index] / 1000M);
            decimal answer = cbRound.Checked ? Math.Round(Frams, MidpointRounding.AwayFromZero) : Frams;
            if (Math.Abs(Frams - answer) < costumeAccuracy)
                 { ++AccuratePiont;   }
            else { ++InAccuratePiont; }
        }


        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            updataGridView(comboBox1.SelectedIndex + 1);
        }


        /// FPS Cal Part /////////////////////

        decimal[] Accuracy = { 0.01M, 0.05M, 0.10M, 0.15M, 0.20M, 0.25M, 0.30M };
        private void Accuracy_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (var item in toolStripMenuItem1.DropDownItems)
            {
                if (!Equals(item, toolStripSeparator1))
                {
                    (item as ToolStripMenuItem).Checked = false;
                }
            }
            (e.ClickedItem as ToolStripMenuItem).Checked = true;
            costumeAccuracy = Accuracy[int.Parse(e.ClickedItem.Tag.ToString())];
        }


        void cbShift_CheckedChanged(object sender, EventArgs e)
        {
            if (!isPathValid) { return; }
            info.offset = cbShift.Checked ? getOffsetFromMaskedTextBox() : TimeSpan.Zero;
            updataGridView();
        }

        void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) { Tips.Text = "位移时间不科学的样子"; }

        TimeSpan getOffsetFromMaskedTextBox()
        {
            if (convertMethod.RTimeFormat.IsMatch(maskedTextBox1.Text))
            {
                return convertMethod.string2Time(maskedTextBox1.Text);
            }
            else
            {
                Tips.Text = "位移时间不科学的样子";
                return TimeSpan.Zero;
            }
        }


        #region form resize support
        bool moreModeShow
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
        void Form1_Resize()
        {
            if (cbMore.Checked)
            {
                Enumerable.Range(Size.Height, 80).ToList().ForEach((x) => Size = new Size(Size.Width, x));
            }
            else
            {
                Enumerable.Range(Size.Height - 80, 80).Reverse().ToList().ForEach((x) => Size = new Size(Size.Width, x));
            }
            moreModeShow = cbMore.Checked;
        }
        void cbMore_CheckedChanged(object sender, EventArgs e)
        {
            Form1_Resize();
            cbMore.Text = cbMore.Checked ? "∧" : "∨";
        }
        #endregion

        string loadChapterName()
        {
            openFileDialog1.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            string temp = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string ChapterPath = openFileDialog1.FileName;
                CTLogger.Log("+载入自定义章节名模板："+ ChapterPath);
                byte[] buffer = File.ReadAllBytes(ChapterPath);
                temp = convertMethod.GetUTF8String(buffer);
            }
            else
            {
                cbChapterName.CheckState = CheckState.Unchecked;
            }
            return temp;
        }

        string chapterNameTemplate;

       private void cbChapterName_CheckedChanged(object sender, EventArgs e)       //载入客章节模板或清除
       {
            if(cbChapterName.Checked)
            {
                chapterNameTemplate = loadChapterName();
                updataInfo(chapterNameTemplate);
            }
            else
            {
                chapterNameTemplate = string.Empty;
            }
            updataGridView();
        }

        /////////////////XML support
        void loadXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(paths[0]);
            geneRateCI(doc);
        }

        /////////////////mpls support

        decimal[] FrameRate = { 0M, 24000M / 1001, 24000M / 1000,
                                    25000M / 1000, 30000M / 1001,
                                    50000M / 1000, 60000M / 1001 };

        mplsData RawData;

        int mplsFileSeletIndex
        {
            get
            {
                return comboBox2.SelectedIndex == -1 ? 0 : comboBox2.SelectedIndex;
            }
        }

        void loadMPLS()
        {
            RawData = new mplsData(paths[0]);
            CTLogger.Log("+成功载入MPLS格式章节文件");
            CTLogger.Log(string.Format("|+MPLS中共有 {0} 个m2ts片段", RawData.chapterClips.Count));

            comboBox2.Enabled = comboBox2.Visible = (RawData.chapterClips.Count >= 1);
            if (comboBox2.Enabled)
            {
                comboBox2.Items.Clear();
                foreach (var item in RawData.chapterClips)
                {
                    comboBox2.Items.Add(string.Format("{0}__{1}", item.Name.Replace("M2TS", ".m2ts"), item.timeStamp.Count));
                    CTLogger.Log(string.Format(" |+{0}", item.Name));
                    CTLogger.Log(string.Format("  |+包含 {0} 个时间戳", item.timeStamp.Count));
                }
            }
            comboBox2.SelectedIndex = mplsFileSeletIndex;
            geneRateCI(mplsFileSeletIndex);
        }


        void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RawData != null)
            {
                geneRateCI(comboBox2.SelectedIndex);
            }
            else
            {
                if (XMLGroup != null)
                {
                    geneRateCI(comboBox2.SelectedIndex, false, true);
                }
                else
                {
                    geneRateCI(comboBox2.SelectedIndex, true);
                }
            }
            updataGridView();
        }


        private void combineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RawData != null)
            {
                combineToolStripMenuItem.Checked = !combineToolStripMenuItem.Checked;
                geneRateCI(comboBox2.SelectedIndex);
                updataGridView();
            }
        }

        /// <summary>
        /// generate the chapter info from a matroska file
        /// </summary>
        void loadMatroska()
        {
            matroskaInfo matroska = new matroskaInfo(paths[0]);
            geneRateCI(matroska.result);
        }

        #region color support
        Form3 Fcolor;

        private void Color_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Fcolor == null)
                {
                    Fcolor = new Form3(this);
                }
                CTLogger.Log("颜色设置窗口被打开");
                Fcolor.Show();
                Fcolor.Focus();
                Fcolor.Select();
            }
        }
        public List<Color> currentColor
        {
            get
            {
                List<Color> temp = new List<Color>();
                temp.Add(BackChange);
                temp.Add(TextBack);
                temp.Add(MouseOverColor);
                temp.Add(MouseDownColor);
                temp.Add(BordBackColor);
                temp.Add(TextFrontColor);
                return temp;
            }
        }
        public Color BackChange
        {
            set
            {
                BackColor                                    = value;
                cbMore.BackColor                             = value;
            }
            get
            {
                return BackColor;
            }
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
            get
            {
                return dataGridView1.BackgroundColor;
            }
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
                cbMore.FlatAppearance.MouseOverBackColor     = value;
            }
            get
            {
                return btnLoad.FlatAppearance.MouseOverBackColor;
            }
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
                cbMore.FlatAppearance.MouseDownBackColor     = value;
            }
            get
            {
                return btnLoad.FlatAppearance.MouseDownBackColor;
            }

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
                cbMore.FlatAppearance.BorderColor            = value;
                dataGridView1.GridColor                      = value;
            }
            get
            {
                return btnLoad.FlatAppearance.BorderColor;
            }
        }
        public Color TextFrontColor
        {
            set
            {
                ForeColor                                    = value;
                numericUpDown1.ForeColor                     = value;
                maskedTextBox1.ForeColor                     = value;
                cbMore.ForeColor                             = value;
                comboBox1.ForeColor                          = value;
                comboBox2.ForeColor                          = value;
                xmlLang.ForeColor                            = value;
                savingType.ForeColor                         = value;
                dataGridView1.ForeColor                      = value;
            }
            get
            {
                return ForeColor;
            }
        }
        #endregion

        #region tips support
        private void label1_MouseEnter(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(paths[0]))
            {
                toolTip1.Show(paths[0], label1);
            }
        }
        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            string SFakeChapter1 = "本片段时长为";
            string SFakeChapter2 = string.Format("但是第二个章节点{0}离视频结尾太近了呢，应该没有用处吧 (-｡-;)"   , Environment.NewLine);
            string SFakeChapter3 = string.Format("虽然只有两个章节点{0}应该还是能安心的呢 (～￣▽￣)→))*￣▽￣*)o", Environment.NewLine);
            if (!string.IsNullOrEmpty(paths[0]) && paths[0].ToLowerInvariant().EndsWith(".mpls"))
            {
                int index = mplsFileSeletIndex;
                if (RawData.chapterClips[index].timeStamp.Count == 2)
                {
                    Clip streamClip = RawData.chapterClips[index];
                    string lastTime = convertMethod.time2string(streamClip.TimeOut - streamClip.TimeIn);
                    if (((streamClip.TimeOut - streamClip.TimeIn) - (streamClip.timeStamp[1] - streamClip.timeStamp[0])) <= 5 * 45000)
                    {
                        toolTip1.Show(string.Format("{0}: {1}，{2}", SFakeChapter1, lastTime, SFakeChapter2), btnSave);
                    }
                    else
                    {
                        toolTip1.Show(string.Format("{0}: {1}，{2}", SFakeChapter1, lastTime, SFakeChapter3), btnSave);
                    }
                }
            }
        }

        private void comboBox2_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show((comboBox2.Items.Count > 20) ? "不用看了，这是播放菜单的mpls" : comboBox2.Items.Count.ToString(), comboBox2);
        }
        private void cbMul1k1_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("用于DVD Decrypter提取的Chapter", cbMul1k1);
        }
        private void toolTipRemoveAll(object sender, EventArgs e) { toolTip1.RemoveAll(); }
        #endregion

        #region closing animation support
        public int SystemVersion
        {
            //Windows95/98/Me	     	 4
            //Windows2000/XP/2003        5
            //WindowsVista/7/8/8.1/10 	 6
            get
            {
                return Environment.OSVersion.Version.Major;
            }
        }
        void FormMove(int forward,ref Point p)
        {
            switch (forward)
            {
                case 1: ++p.X; break;
                case 2: --p.X; break;
                case 3: ++p.Y; break;
                case 4: --p.Y; break;
                default:       break;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            registryStorage.Save(Location.ToString(), @"Software\ChapterTool", "Location");
            if (poi > 0 && poi < 3 && nico == 10)
            {
                Point origin   = Location;
                Random forward = new Random();
                int forward2   = forward.Next(1, 5);
                if (forward2 % 2 == 0 || SystemVersion == 5)
                {
                    foreach (var i in Enumerable.Range(0,50))
                    {
                        FormMove(forward.Next(1, 5), ref origin);
                        System.Threading.Thread.Sleep(4);
                        Location = origin;
                    }
                    return;
                }
                while (Opacity > 0)
                {
                    Opacity -= 0.02;
                    FormMove(forward2, ref origin);
                    System.Threading.Thread.Sleep(4);
                    Location = origin;
                }
            }
        }
        #endregion

        FormLog _LogForm;
        private void btnLog_Click(object sender, EventArgs e)
        {
            if (_LogForm == null)
            {
                _LogForm = new FormLog();
            }
            _LogForm.Show();
            _LogForm.Focus();
            _LogForm.Select();
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            updataInfo((int)numericUpDown1.Value);
            updataGridView();
        }
        private void cbMul1k1_CheckedChanged(object sender, EventArgs e)
        {
            if (info != null && info.SourceType != "DVD")
            {
                if (cbMul1k1.Checked)
                {
                    updataInfo(1.001M);
                }
                else
                {
                    updataInfo(1 / 1.001M);
                }
                updataGridView();
            }
        }
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CTLogger.Log(string.Format("+更名: {0} -> {1}", info.Chapters[e.RowIndex].Name, dataGridView1.Rows[e.RowIndex].Cells[2].Value));
            info.Chapters[e.RowIndex].Name = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
        }
        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            foreach (DataGridViewRow item in dataGridView1.SelectedRows)
            {
                info.Chapters.Remove((Chapter)item.Tag);
            }
            updataInfo((int)numericUpDown1.Value);
            if (info.Chapters.Count > 1)
            {
                TimeSpan ini = info.Chapters[0].Time;
                updataInfo(ini);
            }
        }
        private void Form1_Move(object sender, EventArgs e)
        {
            if (_PreviewForm != null)
            {
                _PreviewForm.Location = new Point(Location.X - 230, Location.Y);
            }
        }

        FormPreview _PreviewForm;
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (!isPathValid) { return; }
            if (_PreviewForm == null)
            {
                _PreviewForm = new FormPreview(info.getText(cbAutoGenName.Checked), Location);
            }
            _PreviewForm.UpdateText(info.getText(cbAutoGenName.Checked));
            _PreviewForm.Show();
            _PreviewForm.Focus();
            _PreviewForm.Select();
        }

        private void cbAutoGenName_CheckedChanged(object sender, EventArgs e)
        {
            int index = 1;
            if (cbAutoGenName.Checked)
            {
                foreach (var item in dataGridView1.Rows)
                {
                    (item as DataGridViewRow).Cells[2].Value = string.Format("Chapter {0:D2}", index++);
                }
                return;
            }
            updataGridView();
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CTLogger.Log(string.Format("+ {0} 行被删除", e.RowCount));
        }
    }
}