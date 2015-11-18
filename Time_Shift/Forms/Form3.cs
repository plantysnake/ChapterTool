using System;
using System.Drawing;
using ChapterTool.Util;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ChapterTool.Forms
{

    public partial class Form3 : Form
    {
        Form1 _mainWindow;
        List<Color> _currentSetting;
        public Form3(Form1 mainWindow)
        {
            MaximizeBox = false;
            InitializeComponent();
            _mainWindow = mainWindow;
            _currentSetting = mainWindow.CurrentColor;
            SetDefault();
        }
        void SetDefault()
        {
            back.BackColor      = _currentSetting[0];
            textBack.BackColor  = _currentSetting[1];
            overBack.BackColor  = _currentSetting[2];
            downBack.BackColor  = _currentSetting[3];
            bordBack.BackColor  = _currentSetting[4];
            textFront.BackColor = _currentSetting[5];
        }



        private void back_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                back.BackColor = colorDialog1.Color;
            }
            _mainWindow.BackChange = back.BackColor;
        }
        private void textBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textBack.BackColor = colorDialog1.Color;
            }
            _mainWindow.TextBack = textBack.BackColor;
        }
        private void overBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                overBack.BackColor = colorDialog1.Color;
            }
            _mainWindow.MouseOverColor = overBack.BackColor;
        }
        private void downBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                downBack.BackColor = colorDialog1.Color;
            }
            _mainWindow.MouseDownColor = downBack.BackColor;
        }
        private void bordBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                bordBack.BackColor = colorDialog1.Color;
            }
            _mainWindow.BordBackColor = bordBack.BackColor;
        }
        private void textFront_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textFront.BackColor = colorDialog1.Color;
            }
            _mainWindow.TextFrontColor = textFront.BackColor;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConvertMethod.SaveColor(_mainWindow.CurrentColor);
            e.Cancel = true;
            Hide();
        }
    }
}
