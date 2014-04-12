using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {//мод в 1821 строку расшатал за ~2 минуты
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length > 0 && isNumber())
            {
                int i = 0, j = 0, max_size = Convert.ToInt32(textBox1.Text);
                if (max_size > 10 || max_size < 0) return;
                string line = string.Empty;
                string text = string.Empty;
                bool blocked = false;

                Stopwatch watching = new Stopwatch();

                watching.Start();

                #region первичная обработка
                for ( ; i != richTextBox1.Lines.Length; i++)
                {
                    line = richTextBox1.Lines[i].Trim();
                    for (j = 0; j != line.Length; j++)
                    {
                        char symbol = line[j];
                        if (symbol == '"' || symbol == '\'') blocked = !blocked;
                        if (!blocked)
                        {
                            if (symbol == '{' || symbol == '}')
                            {
                                line = line.Remove(j, 1);
                                line = line.Insert(j, "\n" + symbol + "\n");
                                j++;
                            }
                        }  
                    }

                    text = text + "\n" + line;
                }
                richTextBox1.Text = text;
                #endregion

                #region вторичная обработка
                line = string.Empty;
                text = string.Empty;

                for (i = 0; i != richTextBox1.Lines.Length; i++)
                {
                    line = richTextBox1.Lines[i].Trim();
                    if (line.Length == 0) continue;
                    text = text + "\n" + line;
                }
                richTextBox1.Text = text;
                #endregion

                #region последняя обработка
                line = string.Empty;
                text = string.Empty;
                blocked = false;
                int level = 0;
                bool tabulated = false;

                for (i = 0; i != richTextBox1.Lines.Length; i++)
                {
                    tabulated = false;
                    line = richTextBox1.Lines[i];

                    for (j = 0; j != line.Length; j++)
                    {
                        if (line[j] == '"' || line[j] == '\'') blocked = !blocked;
                        if (blocked) continue;
                        if ((!tabulated) && level > 0)
                        {
                            if (line[j] == '}') level = level - 1;
                            line = "".PadLeft(level * max_size) + line;
                            tabulated = true;
                        }

                        if (line[j] == '{') level = level + 1;
                    }
                    
                    text = text + '\n' + line;
                }
                richTextBox1.Text = text;
                #endregion

                watching.Stop();
                label2.Text = "Ваш код был обработан за " + watching.ElapsedMilliseconds.ToString() + "ms.";
            }
        }
        bool isNumber()
        {
            if (textBox1.Text.Length == 0) return false;
            try
            {
                int value = Convert.ToInt32(textBox1.Text);
                if (value < 0 || value > 20) return false;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
