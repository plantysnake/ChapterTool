using System;
using System.Drawing;
using ChapterTool.Util;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ChapterTool.Forms
{

    public partial class Form3 : Form
    {
        Form1 magic;
        List<Color> currentSetting;
        public Form3(Form1 mainWindow)
        {
            MaximizeBox = false;
            InitializeComponent();
            magic = mainWindow;
            currentSetting = mainWindow.currentColor;
            setDefault();
        }
        void setDefault()
        {
            back.BackColor = currentSetting[0];
            textBack.BackColor = currentSetting[1];
            overBack.BackColor = currentSetting[2];
            downBack.BackColor = currentSetting[3];
            bordBack.BackColor = currentSetting[4];
            textFront.BackColor = currentSetting[5];
        }



        private void back_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                back.BackColor = colorDialog1.Color;
            }
            magic.BackChange = back.BackColor;
        }
        private void textBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textBack.BackColor = colorDialog1.Color;
            }
            magic.TextBack = textBack.BackColor;
        }
        private void overBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                overBack.BackColor = colorDialog1.Color;
            }
            magic.MouseOverColor = overBack.BackColor;
        }
        private void downBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                downBack.BackColor = colorDialog1.Color;
            }

            magic.MouseDownColor = downBack.BackColor;
        }
        private void bordBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                bordBack.BackColor = colorDialog1.Color;
            }

            magic.BordBackColor = bordBack.BackColor;
        }
        private void textFront_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textFront.BackColor = colorDialog1.Color;
            }

            magic.TextFrontColor = textFront.BackColor;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            convertMethod.saveColor(magic.currentColor);
            e.Cancel = true;
            base.Hide();
        }
    }
}
