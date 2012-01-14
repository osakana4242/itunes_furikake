using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace App
{
    /** エラーダイアログ。標準のダイアログって、テキストが選択できないから嫌なので作った。
    */
    public partial class ErrorDialog : Form
    {
        public ErrorDialog()
        {
            InitializeComponent();
            this.textBox1.Text = "superunko\nunko";
        }

        public static void Show(String title, String text)
        {
            ErrorDialog dlg = new ErrorDialog();
            dlg.Text = title;
            dlg.textBox1.Text = text;
            dlg.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
