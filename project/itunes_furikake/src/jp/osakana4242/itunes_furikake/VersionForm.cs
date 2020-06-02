using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

using jp.osakana4242.core.LogOperator;

namespace jp.osakana4242.itunes_furikake
{
    public partial class VersionForm : Form
    {
        static TraceSource log = LogOperator.get();
        public VersionForm()
        {
            InitializeComponent();



            //自分自身のAssemblyを取得
            System.Reflection.Assembly asm =
                System.Reflection.Assembly.GetExecutingAssembly();

            AssemblyName appInfo = asm.GetName();

            //AssemblyTitleの取得
            System.Reflection.AssemblyTitleAttribute asmttl = 
                (System.Reflection.AssemblyTitleAttribute)
                Attribute.GetCustomAttribute(
                System.Reflection.Assembly.GetExecutingAssembly(), 
                typeof(System.Reflection.AssemblyTitleAttribute));


            StringBuilder sb = new StringBuilder(Properties.Resources.StrAppInfo);
            sb.Replace("${app.version}", "" + appInfo.Version);
            sb.Replace("${app.displayName}", "" + asmttl.Title);
            this.textBox1.Text = sb.ToString();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void VersionForm_Load(object sender, EventArgs e)
        {

        }
    }
}
