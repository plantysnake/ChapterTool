//
//                       _oo0oo_
//                      o8888888o
//                      88" . "88
//                      (| -_- |)
//                      0\  =  /0
//                    ___/`---'\___
//                  .' \\|     |// '.
//                 / \\|||  :  |||// \
//                / _||||| -:- |||||- \
//               |   | \\\  -  /// |   |
//               | \_|  ''\---/''  |_/ |
//               \  .-\__  '-'  ___/-. /
//             ___'. .'  /--.--\  `. .'___
//          ."" '<  `.___\_<|>_/___.' >' "".
//         | | :  `- \`.;`\ _ /`;.`/ - ` : | |
//         \  \ `_.   \_ __\ /__ _/   .-` /  /
//     =====`-.____`.___ \_____/___.-`___.-'=====
//                       `=---='
//
//
//     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//
//               佛祖保佑         永无BUG
//
//
//
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using ChapterTool.Forms;
using System.Drawing;
using ChapterTool.Util;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace ChapterTool
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
            CTLogger.Log("+从运行参数中载入文件:" + paths[0]);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            CTLogger.Log(Environment.UserName + "，你好呀");
            CTLogger.Log(Environment.OSVersion.ToString());
            foreach (var item in Screen.AllScreens)
            {
                CTLogger.Log(item.DeviceName + " 分辨率：" + item.Bounds.Width + "*" + item.Bounds.Height);
            }

            Point saved = convertMethod.string2point(registryStorage.Load(@"Software\ChapterTool", "location"));
            if (saved != new Point(-32000, -32000))
            {
                Location = saved;
                CTLogger.Log("成功载入保存的窗体位置"+saved.ToString());
            }

            setDefault();
            if (!string.IsNullOrEmpty(paths[0]))
            {
                Loadfile();
                updataGridView();
                registryStorage.Save("你好呀，找到这里来干嘛呀", @"Software\ChapterTool", string.Empty);
            }
            moreModeShow = false;
            Size = new Size(Size.Width, Size.Height - 80);
            savingType.SelectedIndex = 0;
            //listView1.Columns.Add("时间点", 90, HorizontalAlignment.Left);
            //listView1.Columns.Add("章节名", 180, HorizontalAlignment.Left);
        }
        public string NewLine
        {
            get
            {
                return Environment.NewLine;//     获取为此环境定义的换行字符串。
            }
        }

        ChapterInfo info;

        string SnotLoaded = "尚未载入文件";
        //string SFPSShow3 = "就是 ";
        //string SFPSShow4 = " fps 不对就是圆盘的锅 ";
        string SchapterFitter
        {
            get
            {
                if (File.Exists("mkvextract.exe"))
                {
                    mkvEX = true;
                    return "所有支持的类型(*.txt,*.xml,*.mpls,*.ifo,*.mkv,*.mka)|*.txt;*.xml;*.mpls;*.ifo;*.mkv;*.mka|章节文件(*.txt,*.xml,*.mpls,*.ifo)|*.txt;*.xml;*.mpls*.ifo;|Matroska文件(*.mkv,*.mka)|*.mkv;*.mka";
                }
                else
                {
                    mkvEX = false;
                    return "章节文件(*.txt,*.xml,*.mpls,*.ifo)|*.txt;*.xml;*.mpls*.ifo;";
                }
            }
        }
        string SnameFitter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
        //string SwhatsThis1 = "请别喂一些奇怪的东西 ( つ⁰﹏⁰)つ";
        //string Swhatsthis2 = "当前片段并没有章节 (¬_¬)";
        string SinvalidTime = "位移时间不科学的样子";


        void setDefault()
        {
            cbMore.CheckState = CheckState.Unchecked;
            dataGridView1.Rows.Clear();
            moreModeShow = false;
            comboBox2.Enabled = comboBox2.Visible = false;

            comboBox1.SelectedIndex = -1;
            btnSave.Enabled = btnSave.Visible = true;

            progressBar1.Visible = true;
            cbMore.Enabled = true;
            //mplsValid = true;
            //cbMul1k1.Checked = false;
            cbMul1k1.Enabled = true;

            folderBrowserDialog1.SelectedPath = registryStorage.Load();
        }
        Regex RLineOne    = new Regex(@"CHAPTER\d+=\d+:\d+:\d+\.\d+");
        Regex RLineTwo    = new Regex(@"CHAPTER\d+NAME=(?<chapterName>.*)");
        Regex RTimeFormat = new Regex(@"(?<Hour>\d+):(?<Minute>\d+):(?<Second>\d+)\.(?<Millisecond>\d{3})");

        string[] paths = new string[20];

        void Form1_DragDrop(object sender,  DragEventArgs e)
        {
            paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            //IsTimeFomat = true;
            if (isPathValid)
            {
                CTLogger.Log("+从窗口拖拽中载入文件:" + paths[0]);
                comboBox2.Items.Clear();
                Loadfile();
                
                updataGridView();

            }
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
            CTLogger.Log("点击了" + poi.ToString() + "次进度条");
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

        bool isPathValid
        {
            get
            {
                if (string.IsNullOrEmpty(paths[0]))
                {
                    Tips.Text = "文件还没载入呢";
                    return false;
                }
                else
                {
                    if (!(paths[0].ToLowerInvariant().EndsWith(".txt") ||
                        paths[0].ToLowerInvariant().EndsWith(".xml") ||
                        paths[0].ToLowerInvariant().EndsWith(".mpls") ||
                        paths[0].ToLowerInvariant().EndsWith(".mkv") ||
                        paths[0].ToLowerInvariant().EndsWith(".mka") ||
                        paths[0].ToLowerInvariant().EndsWith(".ifo")) )
                    {
                        Tips.Text = "这个文件我不认识啊 _ (:3」∠)_";
                        CTLogger.Log("文件格式非法");
                        paths[0] = string.Empty;
                        label1.Text = SnotLoaded;
                        return false;
                    }
                }
                return true;
            }
        }
        //bool mplsValid = true;
        bool mkvEX = true;

        
        void Loadfile()
        {
            if (!isPathValid) { return; }
            string shortedPath = paths[0].Substring(0, 20)  + "……" + paths[0].Substring(paths[0].Length - 12, 12);
            label1.Text = shortedPath;
            setDefault();
            Cursor = Cursors.AppStarting;
            try
            {
                if  (paths[0].ToLowerInvariant().EndsWith(".mpls")) { loadMPLS(); }
                if  (paths[0].ToLowerInvariant().EndsWith(".xml" )) { loadXML(); }
                if ((paths[0].ToLowerInvariant().EndsWith(".mkv" ) || 
                     paths[0].ToLowerInvariant().EndsWith(".mka" )) && mkvEX) { loadMatroska(); }
                if ( paths[0].ToLowerInvariant().EndsWith(".txt" )) { loadOGM(); }
                if ( paths[0].ToLowerInvariant().EndsWith(".ifo")) { LoadIFO(); }
                if (!string.IsNullOrEmpty(chapterNameTemplate))   { updataInfo(chapterNameTemplate); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CTLogger.Log("ERROR: " + ex.Message);
                label1.Text = SnotLoaded;
            }
            Cursor = Cursors.Default;
            
        }

        void LoadIFO()
        {
            List<ChapterInfo> Rawifo = new ifoData().GetStreams(paths[0]);
            info = Rawifo[0];
        }

        void loadOGM()
        {
            using (StreamReader sr = new StreamReader(paths[0], Encoding.Default))
            {
                string temp = sr.ReadToEnd();
                geneRateCI(temp);
                
                //textBox1.Text = temp;
            }
            
            progressBar1.Value = 33;
            Tips.Text = "载入完成 (≧▽≦)";
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
                if (info != null)
                {
                    getFramInfo(0);
                    updataGridView();
                }
                else
                {
                    Tips.Text = "载入的文件为空";
                }

            }
        }

        void btnSave_Click(object sender, EventArgs e) { saveFile(); }  //输出保存键

        string CUSTsavingPath = string.Empty;
        void btnSave_MouseDown(object sender, MouseEventArgs e)         //设置保存路径
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        CUSTsavingPath = folderBrowserDialog1.SelectedPath;
                        try
                        {
                            registryStorage.Save(CUSTsavingPath);
                            CTLogger.Log("设置保存路径为:" + CUSTsavingPath);
                        }
                        catch(Exception ex)
                        {
                            Tips.Text = "由于某种原因，它炸了";
                            CTLogger.Log("设置保存路过程中出现错误：" + ex.Message);
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(string.Format("Error opening path {0}: {1}{2}", CUSTsavingPath, exception.Message, Environment.NewLine), "ChapterTool Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    CTLogger.Log(string.Format("Error opening file {0}: {1}", CUSTsavingPath, exception.Message));
                }
            }
        }

        void saveFile()
        {
            if (!isPathValid) { return; }
            string savePath = paths[0].Substring(0, paths[0].LastIndexOf("."));
            //modify for custom saving path
            int slashPosition = paths[0].LastIndexOf(@"\");
            if (!string.IsNullOrEmpty(CUSTsavingPath))
                savePath = CUSTsavingPath + paths[0].Substring(slashPosition, paths[0].LastIndexOf(".") - slashPosition);

            if (paths[0].ToLowerInvariant().EndsWith(".mpls") && !combineToolStripMenuItem.Checked)
                savePath += "__" + RawData.chapterClips[mplsFileSeletIndex].Name;

            switch (savingType.SelectedIndex)
            {
                case 0://TXT
                    while (File.Exists(savePath + ".txt")) { savePath += "_"; }
                    savePath += ".txt";
                    info.SaveText(savePath);
                    break;
                case 1://XML
                    while (File.Exists(savePath + ".xml")) { savePath += "_"; }
                    savePath += ".xml";
                    info.SaveXml(savePath);
                    break;
                case 2://QPF
                    while (File.Exists(savePath + ".qpf")) { savePath += "_"; }
                    savePath += ".qpf";
                    info.SaveQpfile(savePath);
                    break;
            }

            MessageBox.Show(savePath);
        }
        void refresh_Click(object sender, EventArgs e) { updataGridView(); }


        string GetFirstLine_new(string[] OGMdata, out int i)//i 为章节实际开始的行号
        {
            i = 0;
            foreach (var item in OGMdata)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    return item;
                }
                ++i;
            }
            return string.Empty;
        }

        TimeSpan OffsetCal_new(string line)//获取第一行的时间
        {
            if (RLineOne.IsMatch(line))
            {
                return convertMethod.string2Time(RTimeFormat.Match(line).ToString());
            }
            else
            {
                CTLogger.Log("ERROR: " + line + " <-该行与时间行格式不匹配");
                return TimeSpan.Zero;
            }
        }
        Chapter WriteToChapterInfo(string line, string line2, int order,TimeSpan iniTime)
        {
            Chapter temp = new Chapter();
            if (RLineTwo.IsMatch(line2))                     //章节标题行
            {
                switch (cbAutoGenName.Checked)
                {
                    case true:
                        temp.Name = RLineTwo.Match(line).Groups["chapterName"].Value; break;
                    case false:
                        temp.Name = "Chapter " + order.ToString("00"); break;
                }
            }
            if (RLineOne.IsMatch(line))
            {
                temp.Time = convertMethod.string2Time(RTimeFormat.Match(line).ToString()) - iniTime;
            }
            temp.Number = order;
            return temp;
        }

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
            info.Duration = info.Chapters[info.Chapters.Count - 1].Time;
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
            if (current.Count < 2) { return; }
            int offset = current[0];

            int default_order = 1;
            foreach (int item in current)
            {
                Chapter temp = new Chapter();
                var sDO = default_order.ToString("00");
                temp.Time = convertMethod.pts2Time(item - offset);
                temp.Name = "Chapter " + sDO;
                temp.Number = default_order++;
                info.Chapters.Add(temp);
            }
        }
        void geneRateCI(XmlDocument doc)
        {
            info = new ChapterInfo();
            info.SourceType = "XML";
            info.SourceHash = ifoData.ComputeMD5Sum(paths[0]);
            XmlElement root = doc.DocumentElement;
            XmlNodeList TimeNodes = root.SelectNodes("/Chapters/EditionEntry/ChapterAtom/ChapterTimeStart");
            XmlNodeList NameNodes = root.SelectNodes("/Chapters/EditionEntry/ChapterAtom/ChapterDisplay/ChapterString");
            if (TimeNodes.Count * NameNodes.Count == 0)
            {
                return;
            }
            int j = 0;
            foreach (XmlNode timenode in TimeNodes)
            {
                Chapter temp = new Chapter();
                temp.Time = convertMethod.string2Time(RTimeFormat.Match(timenode.InnerText).ToString());
                temp.Name = NameNodes[j++].InnerText.ToString();
                temp.Number = j;
                info.Chapters.Add(temp);
            }
            info.Duration = info.Chapters[info.Chapters.Count - 1].Time;
        }


        void updataInfo(TimeSpan shift)
        {
            if (!isPathValid) { return; }
            foreach (var item in info.Chapters)
            {
                item.Time -= shift;
            }
        }
        void updataInfo(int shift)
        {
            if (!isPathValid) { return; }
            int i = 1;
            foreach (var item in info.Chapters)
            {
                item.Number = i++ + shift;
            }
        }
        void updataInfo(string chapterName)
        {
            if (!isPathValid) { return; }
            string[] cn = chapterName.Split('\n');
            int i = 0;
            foreach (var item in info.Chapters)
            {
                item.Name = cn[i++];
                if (i == cn.Length) { break; }
            }
        }
        void updataInfo(decimal coefficient)
        {
            if (!isPathValid) { return; }
            foreach (var item in info.Chapters)
            {
                item.Time = convertMethod.pts2Time((int)((decimal)item.Time.TotalSeconds * coefficient * 45000M));
            }
        }

        void updataGridView(int fpsIndex = 0)
        {
            if (!isPathValid || info == null) { return; }
            dataGridView1.Rows.Clear();
            if (info.SourceType == "DVD")
            {
                cbMul1k1.Checked = true;
            }
            getFramInfo(fpsIndex);
            info.FramesPerSecond = (double)FrameRate[comboBox1.SelectedIndex];
            foreach (var item in info.Chapters)
            {
                int index = this.dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Tag = item;
                dataGridView1.Rows[index].Cells[0].Value = item.Number.ToString("00");
                dataGridView1.Rows[index].Cells[1].Value = convertMethod.time2string(item.Time + info.offset);
                if (cbAutoGenName.Checked)
                    dataGridView1.Rows[index].Cells[2].Value = "Chapter " + (index+1).ToString("00");
                else
                    dataGridView1.Rows[index].Cells[2].Value = item.Name;
                dataGridView1.Rows[index].Cells[3].Value = item.FramsInfo;
                //dataGridView1.Rows[index].Cells[0].Style.BackColor = Color.FromArgb(0xff, 0xe6, 0xe6, 0xe6);
                //MessageBox.Show(item.Number.ToString()+"   "+index.ToString() + "    " + dataGridView1.Rows.Count.ToString());
            }
        }


        /// FPS Cal Part /////////////////////
        decimal costumeAccuracy = 0.15M;
        void getFramInfo(int index = 0)
        {
            index = (index == 0)? getAUTOFPS(): index;

            foreach (var item in info.Chapters)
            {
                TimeSpan _current = item.Time;
                decimal Frams = ((decimal)_current.TotalMilliseconds * FrameRate[index] / 1000M);
                decimal answer = cbRound.Checked ? Math.Round(Frams, MidpointRounding.AwayFromZero) : Frams;
                string buffer = cbRound.Checked? answer.ToString():Frams.ToString();
                bool accu = (Math.Abs(Frams - answer) < costumeAccuracy);
                buffer += (accu ? " K" : " *");
                item.FramsInfo = buffer;
            }
        }
        int getAUTOFPS()
        {
            decimal FPStemp = FrameRate[1];
            //cbRound.CheckState = CheckState.Checked;
            int currentMaxOne = 0; int AUTOFPS_code = 1;
            CTLogger.Log("|+自动帧率识别开始" + "，允许误差为：" + costumeAccuracy.ToString());
            for (int j = 1; j < 7; ++j)
            {
                int AccuratePiont = 0;
                int InAccuratePiont = 0;
                FPStemp = FrameRate[j];

                string buffer1 = string.Empty;
                foreach (var item in info.Chapters)
                {
                    getAccuracy(item.Time, ref AccuratePiont, ref InAccuratePiont, j);
                }
                if (currentMaxOne < AccuratePiont)
                {
                    AUTOFPS_code = j;
                    currentMaxOne = AccuratePiont;
                }
                CTLogger.Log(" |fps=" + FrameRate[j].ToString("00.000") + "时，精确点：" + AccuratePiont.ToString("00") + "个，非精确点：" + InAccuratePiont.ToString("00") + "个");
            }
            
            comboBox1.SelectedIndex = AUTOFPS_code - 1;
            CTLogger.Log(" |自动识别结果为" + FPStemp.ToString("00.0000") + " fps");
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


        int TSD
        {
            set
            {
                TSD_0unit.Checked = false;
                TSD_1unit.Checked = false;
                TSD_2unit.Checked = false;
                TSD_3unit.Checked = false;
                TSD_4unit.Checked = false;
                TSD_5unit.Checked = false;
                TSD_6unit.Checked = false;
                switch (value)
                {
                    case 0:
                        costumeAccuracy = 0.01M;
                        TSD_0unit.Checked = true;
                        break;
                    case 1:
                        costumeAccuracy = 0.05M;
                        TSD_1unit.Checked = true;
                        break;
                    case 2:
                        costumeAccuracy = 0.10M;
                        TSD_2unit.Checked = true;
                        break;
                    case 3:
                        costumeAccuracy = 0.15M;
                        TSD_3unit.Checked = true;
                        break;
                    case 4:
                        costumeAccuracy = 0.20M;
                        TSD_4unit.Checked = true;
                        break;
                    case 5:
                        costumeAccuracy = 0.25M;
                        TSD_5unit.Checked = true;
                        break;
                    case 6:
                        costumeAccuracy = 0.30M;
                        TSD_6unit.Checked = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void Accuracy_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            TSD = int.Parse(e.ClickedItem.Tag.ToString());
        }


        void cbShift_CheckedChanged(object sender, EventArgs e)
        {
            if (!isPathValid) { return; }
            if (cbShift.Checked)
            {
                info.offset = getOffsetFromMaskedTextBox();
            }
            else
            {
                info.offset = TimeSpan.Zero;
            }
            updataGridView();
        }

        void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) { Tips.Text = SinvalidTime; }

        TimeSpan getOffsetFromMaskedTextBox()
        {
            if (RTimeFormat.IsMatch(maskedTextBox1.Text))
            {
                return convertMethod.string2Time(maskedTextBox1.Text);
            }
            else
            {
                Tips.Text = SinvalidTime;
                return TimeSpan.Zero;
            }
        }

        //TimeSpan _ShiftTime;


        bool moreModeShow
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
            }

        }
        void Form1_Resize()
        {
            Size change = new Size(Size.Width,0);
            switch (cbMore.Checked)  
            {
                case true:
                    int range = Size.Height + 80;
                    for (int i = Size.Height; i <= range; ++i)
                    {
                        change.Height = i;
                        Size = change;
                    }
                    break;
                case false:
                    int range2 = Size.Height - 80;
                    for (int i = Size.Height; i >= range2; --i)
                    {
                        change.Height = i;
                        Size = change;
                    }
                    break;
            }
            moreModeShow = cbMore.Checked;
        }
        void cbMore_CheckedChanged(object sender, EventArgs e) //MORE
        {
            Form1_Resize();
            cbMore.Text = cbMore.Checked ? "∧" : "∨";
        }

       string loadChapterName()
        {
            openFileDialog1.Filter = SnameFitter;
            string temp = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string ChapterPath = openFileDialog1.FileName;
                CTLogger.Log("+载入自定义章节名模板："+ ChapterPath);
                using (StreamReader sr = new StreamReader(ChapterPath, Encoding.Default))
                {
                    temp = sr.ReadToEnd();
                }
            }
            else
            {
                cbChapterName.CheckState = CheckState.Unchecked;
            }
            return temp;
        }

        string chapterNameTemplate;

       void cbChapterName_CheckedChanged(object sender, EventArgs e)       //载入客章节模板或清除
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
            CTLogger.Log("|+MPLS中共有" + RawData.chapterClips.Count.ToString() + "个m2ts片段");

            comboBox2.Enabled = comboBox2.Visible = (RawData.chapterClips.Count >= 1);
            if (comboBox2.Enabled)
            {
                comboBox2.Items.Clear();
                foreach (var item in RawData.chapterClips)
                {
                    comboBox2.Items.Add(item.Name.Replace("M2TS",".m2ts") + "__" + item.timeStamp.Count.ToString());
                    CTLogger.Log(" |+" + item.Name);
                    CTLogger.Log("  |+包含 " + item.timeStamp.Count.ToString() + " 个时间戳");
                }
            }
            //bool isValide = mpls2box();
            comboBox2.SelectedIndex = mplsFileSeletIndex;
            geneRateCI(mplsFileSeletIndex);
        }


        void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            geneRateCI(comboBox2.SelectedIndex);
            updataGridView();
        }


        private void combineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            combineToolStripMenuItem.Checked = !combineToolStripMenuItem.Checked;
            geneRateCI(comboBox2.SelectedIndex);
            updataGridView();
        }

        //////////matroska support
        void loadMatroska()
        {
            matroskaInfo matroska = new matroskaInfo(paths[0]);
            info = matroska.result;
        }

        //color support
        Form3 Fcolor;
        private void Color_MouseDown(object sender, MouseEventArgs e)
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
                temp.Add(BackColor);
                temp.Add(dataGridView1.BackColor);
                temp.Add(btnLoad.FlatAppearance.MouseOverBackColor);
                temp.Add(btnLoad.FlatAppearance.MouseDownBackColor);
                temp.Add(btnLoad.FlatAppearance.BorderColor);
                temp.Add(this.ForeColor);
                //registryStorage.Save(temp);

                return temp;
            }
        }
        public Color BackChange
        {
            set
            {
                BackColor = value;
                cbMore.BackColor = value;
            }
        }
        public Color TextBack
        {
            set
            {
                dataGridView1.BackColor = value;
                numericUpDown1.BackColor = maskedTextBox1.BackColor = value;
                comboBox1.BackColor = comboBox2.BackColor = value;
            }
            
        }
        public Color MouseOverColor
        {
            set
            {
                btnLoad.FlatAppearance.MouseOverBackColor = value;
                btnSave.FlatAppearance.MouseOverBackColor = value;
                btnTrans.FlatAppearance.MouseOverBackColor = value;
                btnLog.FlatAppearance.MouseOverBackColor = value;
                cbMore.FlatAppearance.MouseOverBackColor = value;
            }
        }
        public Color MouseDownColor
        {
            set
            {
                btnLoad.FlatAppearance.MouseDownBackColor = value;
                btnSave.FlatAppearance.MouseDownBackColor = value;
                btnTrans.FlatAppearance.MouseDownBackColor = value;
                btnLog.FlatAppearance.MouseDownBackColor = value;
                cbMore.FlatAppearance.MouseDownBackColor = value;
            }
            
        }
        public Color BordBackColor
        {
            set
            {
                btnLoad.FlatAppearance.BorderColor = value;
                btnSave.FlatAppearance.BorderColor = value;
                btnTrans.FlatAppearance.BorderColor = value;
                btnLog.FlatAppearance.BorderColor = value;
                cbMore.FlatAppearance.BorderColor = value;
            }
        }
        public Color TextFrontColor
        {
            set
            {
                ForeColor = value;
                numericUpDown1.ForeColor = value;
                maskedTextBox1.ForeColor = value;
                cbMore.ForeColor = value;
                comboBox1.ForeColor = value;
                comboBox2.ForeColor = value;
            }
        }

        //tips part
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
            string SFakeChapter2 = "，但是第二个章节点\r\n离视频结尾太近了呢，应该没有用处吧 (-｡-;)";
            string SFakeChapter3 = "，虽然只有两个章节点\r\n应该还是能安心的呢 (～￣▽￣)→))*￣▽￣*)o";
            if (!string.IsNullOrEmpty(paths[0]) && paths[0].ToLowerInvariant().EndsWith(".mpls"))
            {
                int index = (comboBox2.SelectedIndex == -1) ? 0 : comboBox2.SelectedIndex;
                if (RawData.chapterClips[index].timeStamp.Count == 2)
                {
                    Clip streamClip = RawData.chapterClips[index];
                    string lastTime = convertMethod.time2string(streamClip.TimeOut - streamClip.TimeIn);
                    if (((streamClip.TimeOut - streamClip.TimeIn) - (streamClip.timeStamp[1] - streamClip.timeStamp[0])) <= 5 * 45000)
                    {
                        toolTip1.Show(SFakeChapter1 + lastTime + SFakeChapter2, btnSave);
                    }
                    else
                    {
                        toolTip1.Show(SFakeChapter1 + lastTime + SFakeChapter3, btnSave);
                    }
                }
            }
        }

        private void comboBox2_MouseEnter(object sender, EventArgs e)
        {
            if (comboBox2.Items.Count > 20)
            {
                toolTip1.Show("不用看了，这是播放菜单的mpls", comboBox2);
            }
            else
            {
                toolTip1.Show(comboBox2.Items.Count.ToString(), comboBox2);
            }
            
        }
        private void cbMul1k1_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("用于DVD Decrypter提取的Chapter", cbMul1k1);
        }
        private void toolTipRemoveAll(object sender, EventArgs e) { toolTip1.RemoveAll(); }

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
                Point origin = Location;
                Random forward = new Random();
                int forward2 = forward.Next(1, 5);
                if (forward2 % 2 == 0 || SystemVersion == 5)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        FormMove(forward.Next(1, 5), ref origin);
                        System.Threading.Thread.Sleep(4);
                        Location = origin;
                    }
                }
                else
                {
                    while (this.Opacity > 0)
                    {
                        this.Opacity -= 0.02;
                        FormMove(forward2, ref origin);
                        System.Threading.Thread.Sleep(4);
                        Location = origin;
                    }
                }
            }
        }
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
            if (cbMul1k1.Checked)   
            {
                updataInfo(1.001M);
            }
            else
            {
                updataInfo(1/1.001M);
            }
            updataGridView();
        }
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            info.Chapters[e.RowIndex].Name = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
        }
        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            foreach (DataGridViewRow item in dataGridView1.SelectedRows)
            {
                info.Chapters.Remove((Chapter)item.Tag);
                dataGridView1.Rows.Remove(item);
            }
            updataInfo((int)numericUpDown1.Value);
            if (info.Chapters.Count > 1)
            {
                TimeSpan ini = info.Chapters[0].Time;
                updataInfo(ini);
            }
            updataGridView();
        }
        private void Form1_Move(object sender, EventArgs e)
        {
            if (_PreviewForm != null)   
            {
                _PreviewForm.Location = new Point(Location.X - 230, Location.Y);
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (i % 2 == 0)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Gainsboro; ;
                }
                else
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.PowderBlue;
                }
            }
        }
        FormPreview _PreviewForm;
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (!isPathValid) { return; }
            if (_PreviewForm == null)
            {
                _PreviewForm = new FormPreview(info.getText(), Location);
            }
            _PreviewForm.UpdateText(info.getText());
            _PreviewForm.Show();
            _PreviewForm.Focus();
            _PreviewForm.Select();
        }



        Color cellColorTemp = Color.FromArgb(0,230,230,230);

    }
}