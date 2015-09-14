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
using System.Drawing;
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
            //string Culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            //if (Culture != "zh-CN" && Culture != "Zh-TW" && Culture != "Zh-HK" && System.Environment.UserName != "Taut_Cony") { engVersion(); }
        }
        public Form1(string args)
        {
            InitializeComponent();
            paths[0] = args;
            CTLogger.Log("+从运行参数中载入文件:" + paths[0]);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.OthercTextBox = textBox2;
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
            //checkMkvExtract();
            if (!string.IsNullOrEmpty(paths[0]))
            {
                Loadfile();
                Transfer();
                registryStorage.Save("你好呀，找到这里来干嘛呀", @"Software\ChapterTool", string.Empty);
            }
            //Location = new Point()
            moreModeShow = false;
            //ClientSize = new Size(580, 471);
            //MessageBox.Show(ClientSize.ToString());
            Size = new Size(Size.Width, Size.Height - 80);
        }

        public string NewLine
        {
            get
            {
                return Environment.NewLine;//     获取为此环境定义的换行字符串。
            }
        }




        string SnotLoaded = "尚未载入文件";
        string SFPSShow3 = "就是 ";
        string SFPSShow4 = " fps 不对就是圆盘的锅 ";
        string SchapterFitter
        {
            get
            {
                if (File.Exists("mkvextract.exe"))
                {
                    mkvEX = true;
                    return "所有支持的类型(*.txt,*.xml,*.mpls,*.mkv,*.mka)|*.txt;*.xml;*.mpls;*.mkv;*.mka|章节文件(*.txt,*.xml,*.mpls)|*.txt;*.xml;*.mpls|Matroska文件(*.mkv,*.mka)|*.mkv;*.mka";
                }
                else
                {
                    mkvEX = false;
                    return "章节文件(*.txt,*.xml,*.mpls)|*.txt;*.xml;*.mpls";
                }
            }
        }
        string SnameFitter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
        string SwhatsThis1 = "请别喂一些奇怪的东西 ( つ⁰﹏⁰)つ";
        string Swhatsthis2 = "当前片段并没有章节 (¬_¬)";
        string SinvalidTime = "位移时间不科学的样子";


        void setDefault()
        {
            //if (File.Exists("MediaInfo.dll"))
            //{
            //    SchapterFitter  = "章节文件(*.txt,*.xml,*.mpls)|*.txt;*.xml;*.mpls";
            //}
            //Form1_Resize();
            cbMore.CheckState = CheckState.Unchecked;
            //Form1_Resize();

            moreModeShow = false;
            comboBox2.Enabled = comboBox2.Visible = false;
            textBox1.ScrollBars = ScrollBars.None;
            textBox2.ScrollBars = ScrollBars.None;

            cbFramCal.CheckState = CheckState.Unchecked;
            comboBox1.SelectedIndex = -1;
            btnSave.Enabled = btnSave.Visible = true;
            btnAUTO.Visible = btnAUTO.Enabled = false;
            comboBox1.Visible = false;
            progressBar1.Visible = true;
            //checkBox4.CheckState = System.Windows.Forms.CheckState.Unchecked;
            //checkBox7.CheckState = System.Windows.Forms.CheckState.Unchecked;//should do after the file is loaded
            cbFramCal.Enabled = true;
            cbMore.Enabled = true;
            mplsValid = true;
            xmlValid = true;
            //isMPLS = false;
            //AUTOMODE = false;

            //AccuratePiont = 0;
            //InAccuratePiont = 0;
            fps = 0;

            folderBrowserDialog1.SelectedPath = registryStorage.Load();
            
        }
        //Regex RPath    = new Regex(@"^[a-zA-Z];[\\/]((?! )(?![^\\/]*\s+[\\/])[\w -]+[\\/])*(?! )(?![^.]+\s+\.)[\w -]+$")
        Regex RLineOne    = new Regex(@"CHAPTER\d+=\d+:\d+:\d+\.\d+");
        Regex RLineTwo    = new Regex(@"CHAPTER\d+NAME=(?<chapterName>.*)");
        Regex RTimeFormat = new Regex(@"(?<Hour>\d+):(?<Minute>\d+):(?<Second>\d+)\.(?<Millisecond>\d{3})");

        string[] paths = new string[20];
        void SetScroll()
        {
            if (textBox1.GetLineFromCharIndex(textBox1.Text.Length) + 1 < 20)
            {
                textBox1.ScrollBars = ScrollBars.Horizontal;
                textBox2.ScrollBars = ScrollBars.Horizontal;
            }
            else
            {
                textBox1.ScrollBars = ScrollBars.Both;
                textBox2.ScrollBars = ScrollBars.Both;
            }
        }
        void Form1_DragDrop(object sender,  DragEventArgs e)
        {
            paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            //IsTimeFomat = true;
            if (isPathValid)
            {
                CTLogger.Log("+从窗口拖拽中载入文件:" + paths[0]);
                textBox1.Clear();
                comboBox2.Items.Clear();
                Loadfile();
                SetScroll();
                if (!cbFramCal.Checked && mplsValid) { Transfer(); }//未勾选帧数计算时自动转换
                else
                {
                    if (cbFramCal.Checked) { FPS_Transfer(); }
                }
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
                    if ( !(paths[0].ToLowerInvariant().EndsWith(".txt") ||
                        paths[0].ToLowerInvariant().EndsWith(".xml") ||
                        paths[0].ToLowerInvariant().EndsWith(".mpls") ||
                        paths[0].ToLowerInvariant().EndsWith(".mkv") ||
                        paths[0].ToLowerInvariant().EndsWith(".mka")) )
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

        bool mplsValid = true;

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
                if  (paths[0].ToLowerInvariant().EndsWith(".mpls")) { mplsValid = loadMPLS(); }
                if  (paths[0].ToLowerInvariant().EndsWith(".xml" )) { loadXML(); }
                if ((paths[0].ToLowerInvariant().EndsWith(".mkv" ) || 
                     paths[0].ToLowerInvariant().EndsWith(".mka" )) && mkvEX) { xmlValid = loadMatroska(); }
                if ( paths[0].ToLowerInvariant().EndsWith(".txt" )) { loadOGM(); }
                if (cbFramCal.Checked) { cbFramCal.CheckState = CheckState.Unchecked; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                CTLogger.Log("ERROR: " + ex.Message);
                label1.Text = SnotLoaded;
            }
            Cursor = Cursors.Default;
            
        }

        void loadOGM()
        {
            //button4.Enabled = true;
            using (StreamReader sr = new StreamReader(paths[0], Encoding.Default))
            {
                textBox1.Text = sr.ReadToEnd();
            }
            /*
            FileStream fs = new FileStream(paths[0], FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fs);
            textBox1.Text = sr.ReadToEnd();
            textBox1.AcceptsReturn = true;
            sr.Close();
            fs.Close();
            */
            //fs.Dispose();
            progressBar1.Value = 33;
            Tips.Text = "载入完成 (≧▽≦)";
            CTLogger.Log("|成功载入OGM格式章节文件，共" + totalLine.ToString() + "行");
        }

        void btnLoad_Click(object sender, EventArgs e)                  //载入键
        {
            //OpenFileDialog op = new OpenFileDialog();
            openFileDialog1.Filter = SchapterFitter;
            //openFileDialog1.FilterIndex = 1;
            //op.ShowDialog();

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Clear();
                paths[0] = openFileDialog1.FileName;
                CTLogger.Log("+从载入键中载入文件:" + paths[0]);
                //IsTimeFomat = true;
                comboBox2.Items.Clear();
                Loadfile();
                SetScroll();
                //if (checkBox7.Checked)
                //{
                //    getChapterNameStream();
                //}
                if (mplsValid) { Transfer(); }
                
            }
            //op.Dispose();
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

                        //MessageBox.Show(CUSTsavingPath);
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
            {
                savePath = CUSTsavingPath + paths[0].Substring(slashPosition, paths[0].LastIndexOf(".") - slashPosition);
            }

            if (paths[0].ToLowerInvariant().EndsWith(".mpls") && !combineToolStripMenuItem.Checked)
            {
                savePath += "__" + RawData.chapterClips[mplsFileSeletIndex].Name;
            }
            
            if (paths[0].ToLowerInvariant().EndsWith(".mpls") && cbFramCal.Checked)
            {
                while (File.Exists(savePath + ".qpf")) { savePath += "_"; }
                savePath += ".qpf";
            }
            else
            {
                while (File.Exists(savePath + ".txt")) { savePath += "_"; }
                savePath += ".txt";
            }
            try
            {
                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.Write(textBox2.Text);
                    sw.Flush();
                    sw.Close();
                }
                //FileStream fs = new FileStream(savePath, FileMode.Create);

                //fs.Close();
                //fs.Dispose();
                Tips.Text = "保存完成 (≧▽≦)";
                CTLogger.Log(savePath+"保存成功");
                progressBar1.Value = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error saving file {0}: {1}{2}", savePath, ex.Message, Environment.NewLine), "ChapterTool Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                CTLogger.Log(string.Format("Error saving file {0}: {1}", savePath, ex.Message));
            }
        }
        void btnTrans_Click(object sender, EventArgs e)                  //转换键
        {
            if (!isPathValid || !mplsValid) { return; }
            if (cbShift.Checked)//更新位移时间
            {
                _ShiftTime = SpecialCalForTimeShift();
            }
            if (cbFramCal.Checked)
            {
                FPS_Transfer();
            }
            else
            {
                if(cbChapterName.Checked)
                {
                    UseChapter = true;
                    getChapterNameStream();
                }
                Transfer();
            }
        }

        //DateTime InitialTime;
        TimeSpan _InitialTime;
        void cbReserveName_CheckedChanged(object sender, EventArgs e)       //保留原章节名
        {
            if (!cbFramCal.Checked)
            {
                Transfer();
            }
        }       

        string GetFirstLine(int totalLine,out int i)//i 为章节实际开始的行号
        {
            i = 0;
            foreach (var item in textBox1.Lines)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    return item;
                }
                ++i;
            }
            return string.Empty;
            /*
            i = 0;
            try
            {
                while (i < totalLine && string.IsNullOrEmpty(buffer1))
                { buffer1 = textBox1.Lines[i++]; }
                
            }
            catch
            {
                CTLogger.Log("ERROR：文件为空");
                return string.Empty;
            }
            return buffer1;
            */
        }

        void OffsetCal(string line)//获取第一行的时间
        {
            if (RLineOne.IsMatch(line))
            {
                _InitialTime = convertMethod.string2Time(RTimeFormat.Match(line).ToString());
            }
            else
            {
                CTLogger.Log("ERROR: " + line + " <-该行与时间行格式不匹配");
                Tips.Text = SwhatsThis1;
            }
        }

        int totalLine
        {
            get
            {
                return textBox1.GetLineFromCharIndex(textBox1.Text.Length) + 1;
            }
        }

        void Transfer()
        {
            if (isPathValid&&mplsValid&&xmlValid)
            {
                int i;
                textBox2.Clear();
                OffsetCal(GetFirstLine(totalLine,out i));
                int order = 1 + (int)numericUpDown1.Value;
                TransferCore(order,i);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="order">起始的章节编号</param>
        /// <param name="i">实际的起始行号</param>
        void TransferCore(int order,int i)
        {
            if (totalLine == 1) { return; }
            string buffer1, buffer2;
            if (RLineOne.IsMatch(textBox1.Lines[i]))
            {
                for (; i + 1 < totalLine; i += 2, ++order)
                {
                    buffer1 = textBox1.Lines[i];
                    buffer2 = textBox1.Lines[i + 1];
                    if (string.IsNullOrEmpty(buffer1) || string.IsNullOrEmpty(buffer2)) { break; }
                    WriteToTextBox(buffer1, order);
                    WriteToTextBox(buffer2, order);
                }

                Tips.Text = "转换完成 (≧▽≦)";
                if(UseChapter)
                {
                    ChapterStreamReader.Close();
                    UseChapter = false;
                }
                progressBar1.Value = 66;
            }
        }


        TimeSpan TimeTransfer(string line)
        {
            TimeSpan _current = convertMethod.string2Time(RTimeFormat.Match(line).ToString()) - _InitialTime;
            if(cbShift.Checked)
            {
                _current +=_ShiftTime;
            }
            return _current;
        }

        
        void WriteToTextBox(string line, int order)
        {
            if (string.IsNullOrEmpty(line)) { return; }
            string buffer = string.Empty;
            if (RLineTwo.IsMatch(line))                     //章节标题行
            {
                buffer += "CHAPTER" + order.ToString("00") + "NAME=";
                string ChapterName = string.Empty;
                switch (UseChapter)
                {
                    case true:
                        ChapterName = ChapterStreamReader.ReadLine(); break;
                    case false:
                        switch (cbReserveName.Checked)
                        {
                            case true:
                                ChapterName = RLineTwo.Match(line).Groups["chapterName"].Value; break;
                            case false:
                                ChapterName = "Chapter " + order.ToString("00"); break;
                        } break;
                }
                buffer += ChapterName;
            }
            if (RLineOne.IsMatch(line)) 
            {
                TimeSpan temp = TimeTransfer(line);
                buffer += "CHAPTER" + order.ToString("00") + "=";
                buffer += (cbMul1k1.Checked ? convertMethod.time2string((decimal)temp.TotalSeconds * 1.001M) : convertMethod.time2string(temp));
            }
            textBox2.Text += buffer + NewLine;
        }

        /// FPS Cal Part /////////////////////

        void FPS_Transfer()
        {
            if (!isPathValid || !mplsValid) { return; }
            if(fps == 0) { MessageBox.Show("请选择帧率！"); return; }
            
            //int totalLine = textBox1.GetLineFromCharIndex(textBox1.Text.Length);
            textBox2.Clear();
            string buffer1 = string.Empty;
            int i;
            buffer1 = GetFirstLine(totalLine, out i);
            for (; i < totalLine && buffer1 != string.Empty; i += 2)
            {
                buffer1 = textBox1.Lines[i];
                WriteToTextBox(buffer1);
            }
        }

        decimal costumeAccuracy = 0.15M;

        void getFramAndPrintFram(TimeSpan input,out decimal Frams,out decimal answer)
        {
            Frams = ((decimal)input.TotalMilliseconds * fps / 1000M);
            answer = cbRound.Checked ? Math.Round(Frams, MidpointRounding.AwayFromZero) : Frams;
        }

        void WriteToTextBox(string line)                     //framCal output
        {
            if (string.IsNullOrEmpty(line)) { return; }
            if (!RLineOne.IsMatch(line))
            {
                Tips.Text = SwhatsThis1;
                CTLogger.Log("ERROR: " + line + " <-该行与时间行格式不匹配");
                return;
            }
            TimeSpan _current = convertMethod.string2Time(RTimeFormat.Match(line).ToString());
            decimal Frams, answer;
            getFramAndPrintFram(_current, out Frams, out answer);
            string  buffer = answer.ToString();
            bool      accu = (Math.Abs(Frams - answer) < costumeAccuracy);
            buffer        += (accu ? " K" : " *");
            textBox2.Text += buffer + NewLine;
        }

        void getAccuracy(string line, ref int AccuratePiont, ref int InAccuratePiont)//framCal
        {
            if (string.IsNullOrEmpty(line) || !RLineOne.IsMatch(line)) { return; }
            TimeSpan _current = convertMethod.string2Time(RTimeFormat.Match(line).ToString());
            decimal Frams, answer;
            getFramAndPrintFram(_current, out Frams, out answer);
            if (Math.Abs(Frams - answer) < costumeAccuracy)
                 { ++AccuratePiont;   }
            else { ++InAccuratePiont; }
        }

        decimal fps = 0M;
        
        void comboBox1_SelectedIndexChanged(object sender, EventArgs e) //设置fps
        {
            if (!cbFramCal.Checked) { return; }
            fps = FrameRate[comboBox1.SelectedIndex + 1];//the first number in framerate is 0
            Tips.Text = fps.ToString("00.0000") + " fps";
            FPS_Transfer(); 
        }

        void cbRound_CheckedChanged(object sender, EventArgs e)       //四舍五入
        {
            if (isPathValid)// && !AUTOMODE)
            {
                FPS_Transfer();
            }
        }

        //private bool AUTOMODE = false;
        void btnAUTO_Click(object sender, EventArgs e)                  //AUTO
        {
            if (!isPathValid) { return; }

            
            fps = FrameRate[1];
            cbRound.CheckState = CheckState.Checked;
            int currentMaxOne = 0; int AUTOFPS_code = 1;
            CTLogger.Log("|+自动帧率识别开始"+"，允许误差为："+ costumeAccuracy.ToString());
            for (int j = 1; j < 7; ++j)
            {
                int AccuratePiont   = 0;
                int InAccuratePiont = 0;
                fps = FrameRate[j];

                //int totalLine = textBox1.GetLineFromCharIndex(textBox1.Text.Length);
                //textBox2.Clear();
                string buffer1 = string.Empty;//, buffer2;
                for (int i = 0; i < totalLine ; i += 2)
                {
                    getAccuracy(textBox1.Lines[i], ref AccuratePiont, ref InAccuratePiont);
                }
                if(currentMaxOne < AccuratePiont)
                {
                    AUTOFPS_code = j;
                    currentMaxOne = AccuratePiont;
                }
                CTLogger.Log(" |fps=" + FrameRate[j].ToString("00.000") + "时，精确点："+AccuratePiont.ToString("00")+"个，非精确点："+InAccuratePiont.ToString("00")+"个");
            }
            
            fps = FrameRate[AUTOFPS_code];
            FPS_Transfer();
            comboBox1.SelectedIndex = AUTOFPS_code - 1;
            Tips.Text = "大概是 " + fps.ToString("00.0000") + " fps 吧";
            CTLogger.Log(" |自动识别结果为" + fps.ToString("00.0000") + " fps");
            //AUTOMODE = false;
        }

        void qpfAbout() //对mpls中fps读取
        {
            if (isPathValid && paths[0].ToLowerInvariant().EndsWith(".mpls") && mplsValid)
            {
                fps = FrameRate[RawData.chapterClips[comboBox2.SelectedIndex].fps];
                
                btnSave.Enabled = btnSave.Visible = true;
                btnAUTO.Visible = btnAUTO.Enabled = false;
                comboBox1.Visible = false;
                cbRound.CheckState = CheckState.Checked;
                Tips.Text = SFPSShow3 + fps.ToString("00.0000") + SFPSShow4;
                CTLogger.Log("|成功读"+RawData.chapterClips[comboBox2.SelectedIndex].Name +"的帧率：" + fps.ToString("00.0000") + " fps");
                //FPS_Transfer();
            }
            else
            {
                //checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
                btnSave.Enabled = btnSave.Visible = false;
                btnAUTO.Visible = btnAUTO.Enabled = true;
                comboBox1.Visible = true;
            }
            //FPS_Transfer();
        }

        //精度控制 part
        void cbRound_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition);
            }
        }

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
            switch (e.ClickedItem.ToString())
            {
                case "0.01": TSD = 0; break;
                case "0.05": TSD = 1; break;
                case "0.10": TSD = 2; break;
                case "0.15": TSD = 3; break;
                case "0.20": TSD = 4; break;
                case "0.25": TSD = 5; break;
                case "0.30": TSD = 6; break;
            }
        }

        /*
        void TSD_0unit_Click(object sender, EventArgs e) { TSD = 0; }
        void TSD_1unit_Click(object sender, EventArgs e) { TSD = 1; }
        void TSD_2unit_Click(object sender, EventArgs e) { TSD = 2; }
        void TSD_3unit_Click(object sender, EventArgs e) { TSD = 3; }
        void TSD_4unit_Click(object sender, EventArgs e) { TSD = 4; }
        void TSD_5unit_Click(object sender, EventArgs e) { TSD = 5; }
        void TSD_6unit_Click(object sender, EventArgs e) { TSD = 6; }
        */

        bool fpsMode
        {
            set
            {
                btnSave.Visible = btnSave.Enabled = !value;
                cbMore.Visible = !value;

                cbReserveName.Visible = !value;
                progressBar1.Visible = !value;

                comboBox1.Visible = value;
                cbRound.Visible = value;
                btnAUTO.Visible = btnAUTO.Enabled = value;
            }
        }


        void cbFramCal_CheckedChanged(object sender, EventArgs e)       //MODE CHANGE
        {
            cbMore.CheckState = CheckState.Unchecked;//收起more
            Tips.Text = string.Empty;
            textBox2.Clear();
            fpsMode = cbFramCal.Checked;
            if (cbFramCal.Checked)
            {
                qpfAbout();
            }
            else
            {
                cbRound.CheckState = CheckState.Unchecked;//帧数取整
                if (mplsValid) { Transfer(); }//恢复章节模式
            }
            
        }
        
        void cbShift_CheckedChanged(object sender, EventArgs e)
        {
            if(cbShift.Checked)
            {
                _ShiftTime = SpecialCalForTimeShift();
            }
            Transfer();
        }

        void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) { Tips.Text = SinvalidTime; }

        TimeSpan SpecialCalForTimeShift()
        {
            if (RTimeFormat.IsMatch(maskedTextBox1.Text))
            {
                return convertMethod.string2Time(maskedTextBox1.Text);
            }
            else
            {
                //IsTimeFomat = false;
                Tips.Text = SinvalidTime;
                return TimeSpan.Zero;
                //return convertMethod.string2Time("00:00:00.000");
            }
        }

        //DateTime ShiftTime;
        TimeSpan _ShiftTime;


        bool moreModeShow
        {
            set
            {
                cbMul1k1.Visible = value;
                label3.Visible = value;
                numericUpDown1.Visible = value;

                cbShift.Visible = value;
                cbChapterName.Visible = value;
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

        void contentUpdate(object sender, EventArgs e) { Transfer(); }


        string ChapterPath;
        bool UseChapter = false;

        //FileStream ChapterFileStream;
        StreamReader ChapterStreamReader;

       void loadChapterName()
        {
            //OpenFileDialog op = new OpenFileDialog();
            openFileDialog1.Filter = SnameFitter;
            //openFileDialog1.FilterIndex = 1;
            //op.ShowDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ChapterPath = openFileDialog1.FileName;
                CTLogger.Log("+载入自定义章节名模板："+ ChapterPath);
                UseChapter  = true;
                //MessageBox.Show(op.FileName);
                getChapterNameStream();
            }
            else
            {
                cbChapterName.CheckState = CheckState.Unchecked;
            }
        }

       void getChapterNameStream()                                     //载入章节名文件
        {
            try
            {
                //ChapterFileStream = new FileStream(ChapterPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                ChapterStreamReader = new StreamReader(ChapterPath ,Encoding.Default);
            }
            catch(Exception ex)
            {
                Tips.Text = "文件被占用";
                CTLogger.Log("|载入自定义章节名模板失败：" + ex.Message);
            }
        }

       void cbChapterName_CheckedChanged(object sender, EventArgs e)       //使用客制化章节名
        {
            if(cbChapterName.Checked)
            {
                loadChapterName();
            }
            else
            {
                UseChapter = false;
            }
            Transfer();
        }         

        /////////////////XML support
        bool xmlValid = true;
        void loadXML()
        {
            textBox2.Clear();
            XmlDocument doc       = new XmlDocument();
            doc.Load(paths[0]);
            XmlElement  root      = doc.DocumentElement;
            XmlNodeList TimeNodes = root.SelectNodes("/Chapters/EditionEntry/ChapterAtom/ChapterTimeStart");
            XmlNodeList NameNodes = root.SelectNodes("/Chapters/EditionEntry/ChapterAtom/ChapterDisplay/ChapterString");
            if (TimeNodes.Count * NameNodes.Count == 0)
            {
                xmlValid = false;
                Tips.Text = SwhatsThis1;
                CTLogger.Log("|无效的XML章节文件");
                return;
            }
            else { xmlValid = true; }
            int i = 1, j = 0;
            string text = string.Empty;
            foreach (XmlNode timenode in TimeNodes)
            {
                text += "CHAPTER" + i.ToString("00") + "=" + RTimeFormat.Match(timenode.InnerText) + NewLine;
                text += "CHAPTER" + i++.ToString("00") + "NAME=" + NameNodes[j++].InnerText + NewLine;
            }
            CTLogger.Log("tb1在loadXML处更新");
            textBox1.Text = text;
            if (xmlValid)
            {
                CTLogger.Log("|成功载入XML格式章节文件，共" + totalLine.ToString() + "行");
            }
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

        bool loadMPLS()
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
            bool isValide = mpls2box();
            comboBox2.SelectedIndex = mplsFileSeletIndex;
            return isValide;
        }

        private bool mplsEnable
        {
            set
            {
                mplsValid = value;
                btnSave.Enabled = value;
                cbReserveName.Enabled = value;
                cbFramCal.Enabled = value;
                cbMore.Enabled = value;
            }
        }

        bool mpls2box()
        {
            textBox1.Clear();
            textBox2.Clear();

            List<int> current;
            if (combineToolStripMenuItem.Checked)
            {
                current = RawData.entireTimeStamp;
            }
            else
            {
                current = RawData.chapterClips[mplsFileSeletIndex].timeStamp;
            }
            

            if (current.Count < 2)
            {
                mplsEnable = false;
                Tips.Text = Swhatsthis2;
                progressBar1.Value = 33;
                cbMore.CheckState = CheckState.Unchecked;
                return false;
            }
            mplsEnable = true;
            

            int offset = current[0];
            
            int default_order = 1;
            string text = string.Empty;
            foreach (int item in current)
            {
                var sDO = default_order.ToString("00");
                text += "CHAPTER" + sDO + "=" + convertMethod.time2string(item - offset) + NewLine;
                text += "CHAPTER" + sDO + "NAME=Chapter " + sDO + NewLine;
                ++default_order;
            }
            textBox1.Text = text;
            return true;
        }

        void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            mpls2box();
            switch (cbFramCal.Checked)
            {
                case true:
                    switch (mplsValid)
                    {
                        case true:
                            fps = FrameRate[RawData.chapterClips[comboBox2.SelectedIndex].fps];
                            FPS_Transfer();
                            Tips.Text = SFPSShow3 + fps.ToString("00.0000") + SFPSShow4;
                            break;
                        case false:
                            cbFramCal.CheckState = CheckState.Unchecked; break;
                    } break;
                case false:
                    Transfer(); break;
            }
        }

        private void comboBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip2.Show(MousePosition);
            }
        }

        private void combineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            combineToolStripMenuItem.Checked = !combineToolStripMenuItem.Checked;
            mpls2box();
            switch (cbFramCal.Checked)
            {
                case true:  FPS_Transfer(); break;
                case false: Transfer();     break;
            }
            SetScroll();
        }

        //////////matroska support
        bool loadMatroska()
        {
            //if (!File.Exists(@"C:\Program Files\MKVToolNix\mkvextract.exe"))
            //{
            //    Tips.Text = "请正确设置MKVToolNix路径";
            //    return false;
            //}
            matroskaInfo matroska = new matroskaInfo(paths[0]);

            textBox2.Clear();
            if (string.IsNullOrEmpty(matroska.result))
            {
                CTLogger.Log("|MKV中无章节文件");
                Tips.Text = Swhatsthis2;
                return false;
            }
            CTLogger.Log("|成功载入MKV中的章节文件");
            textBox1.Text = matroska.result;

            return true;
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
                temp.Add(textBox1.BackColor);
                temp.Add(btnLoad.FlatAppearance.MouseOverBackColor);
                temp.Add(btnLoad.FlatAppearance.MouseDownBackColor);
                temp.Add(btnLoad.FlatAppearance.BorderColor);
                temp.Add(textBox1.ForeColor);
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
                textBox1.BackColor = textBox2.BackColor = value;
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
                btnAUTO.FlatAppearance.MouseOverBackColor = value;
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
                btnAUTO.FlatAppearance.MouseDownBackColor = value;
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
                btnAUTO.FlatAppearance.BorderColor = value;
                cbMore.FlatAppearance.BorderColor = value;
            }
        }
        public Color TextFrontColor
        {
            set
            {
                ForeColor = value;
                textBox1.ForeColor = value;
                textBox2.ForeColor = value;
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
        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (cbFramCal.Checked)
            {
                string lineNumber = "呐，知道吗？这是第" + (textBox2.GetLineFromCharIndex(textBox2.GetCharIndexFromPosition(e.Location)) + 1).ToString() + "个章节点哦";
                toolTip1.Show(lineNumber, textBox2,2000);
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

        private void Form1_Move(object sender, EventArgs e)
        {
            #if false
                CTLogger.Log("移动窗体到" + Location.ToString());
            #endif
        }

    }
}