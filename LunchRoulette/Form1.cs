using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace LunchRoulette
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private String path = "";
        private Boolean isChanged = false;
        
        private void Save()
        {
            if (this.path.Length == 0)
                throw new Exception("No File Linked");
            try
            {
                using (StreamWriter sw = new StreamWriter(this.path, false, Encoding.Default))
                {
                    foreach (var tmp in listBox1.Items)
                    {
                        sw.WriteLine(tmp);
                    }
                }
                isChanged = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }

        }

        private void SaveAS()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "다른 이름으로 저장",
                Filter = "텍스트 문서(*.txt)|*.txt"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.path = saveFileDialog.FileName;
                this.Text = this.path.Split('\\')[this.path.Split('\\').Length - 1] + " - LunchRoulette";
                this.Save();
            }
        }

        private void Open(String path)
        {
            this.path = path;
            this.Text = path.Split('\\')[path.Split('\\').Length - 1] + " - LunchRoulette";
            listBox1.Items.Clear();
            try
            {
                using (StreamReader sr = new StreamReader(this.path, Encoding.Default))
                {
                    String tmp;
                    while ((tmp = sr.ReadLine()) != null)
                    {
                        if (tmp.Length == 0)
                            continue;
                        listBox1.Items.Add(tmp);
                    }
                }
                isChanged = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void OpenDialog()
        {
            try
            {
                if (this.SaveCheck() == DialogResult.Cancel)
                    return;


            }
            catch (Exception)
            {

            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "열기",
                Filter = "텍스트 문서(*.txt)|*.txt"

            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.Open(openFileDialog.FileName);
            }
        }

        private void ChooseRandomPick()
        {
            try
            {
                label1.Text = listBox1.Items[(new Random().Next()) % listBox1.Items.Count].ToString();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void AddItems(String text)
        {
            if (text.Length == 0)
                return;
            listBox1.Items.Add(text);
            textBox1.Text = "";
            isChanged = true;
        }

        private void DeleteItems(Object value)
        {
            listBox1.Items.Remove(value);
            isChanged = true;
            
        }

        private DialogResult SaveCheck()
        {
            if (isChanged == false || (this.path.Length == 0 && listBox1.Items.Count == 0)) 
                throw new Exception();

            DialogResult dr = MessageBox.Show(("변경 내용을 " + (this.path.Length == 0 ? "제목 없음" : this.path) + "에 저장하시겠습니까?"), "LunchRoulette", MessageBoxButtons.YesNoCancel);
           
            if(dr==DialogResult.Yes)
            {
                try
                {
                    this.Save();
                }
                catch (Exception ex)
                {
                    if (ex.Message == "No File Linked")
                        this.SaveAS();
                }
            }

            return dr;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
                return;
            button1.Visible = false;
            for (int i = 1; i < 100; i++) 
            {

                this.ChooseRandomPick();
                Application.DoEvents();
                Thread.Sleep(i);
            }
            label1.Text = "";
            Application.DoEvents();
            Thread.Sleep(500);
            MessageBox.Show(listBox1.Items[(new Random().Next()) % listBox1.Items.Count].ToString(), "Result");
            button1.Visible = true;
        }

        private void 열기OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenDialog();
        }

        private void 다른이름으로저장AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveAS();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.AddItems(textBox1.Text);
        }

        private void 저장SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Save();
            }
            catch(Exception ex)
            {
                if (ex.Message == "No File Linked")
                    this.SaveAS();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.SaveCheck() == DialogResult.Cancel)
                    e.Cancel = true;
                
            }
            catch(Exception)
            {

            }
        }

        private void 새로만들기NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1_FormClosing(sender, null);
            Form1_Load(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.path = "";
            this.isChanged = false;
            this.Text = "제목 없음 - LunchRoulette";
            listBox1.Items.Clear();
            label1.Text = "";
            textBox1.Text = "";
            Form1_SizeChanged(null, null);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                int loop = listBox1.Items.Count;
                for (int i = 0; i < loop; i++) 
                    this.DeleteItems(listBox1.Items[listBox1.SelectedIndex]);

            }
            catch(Exception)
            {

            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    if (this.SaveCheck() == DialogResult.Cancel)
                        return;

                }
                catch (Exception)
                {

                }

                this.Open(((String[])e.Data.GetData(DataFormats.FileDrop))[0]);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy | DragDropEffects.Scroll;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            listBox1.Height = this.Height - 100;
            listBox1.Width = this.Width - 150;
            label1.Location = new Point(listBox1.Width + 25, label1.Location.Y);
            button1.Location = new Point(listBox1.Width + 50, button1.Location.Y);
            button2.Location = new Point(listBox1.Width + 50, button2.Location.Y);
            button3.Location = new Point(listBox1.Width + 50, button3.Location.Y);
            textBox1.Location = new Point(listBox1.Width + 25, textBox1.Location.Y);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = "Items.Count : " + listBox1.Items.Count.ToString();
        }
    }
}
