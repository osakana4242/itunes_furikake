using jp.osakana4242.itunes_furikake.Properties;

namespace jp.osakana4242.itunes_furikake
{
    partial class RootForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxRubyAdd = new System.Windows.Forms.CheckBox();
            this.checkBoxRubyAddOverwrite = new System.Windows.Forms.CheckBox();
            this.console = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.コマンドToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.存在しないトラックを削除するToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ヘルプToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.バージョン情報ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBoxOption = new System.Windows.Forms.GroupBox();
            this.checkBoxTrim = new System.Windows.Forms.CheckBox();
            this.checkBoxZenToHan = new System.Windows.Forms.CheckBox();
            this.groupBoxRubyAdd = new System.Windows.Forms.GroupBox();
            this.groupBoxRubyAddType = new System.Windows.Forms.GroupBox();
            this.radioButtonRubyAddTypeAlphabet = new System.Windows.Forms.RadioButton();
            this.radioButtonRubyAddTypeHiragana = new System.Windows.Forms.RadioButton();
            this.buttonApply = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBoxOption.SuspendLayout();
            this.groupBoxRubyAdd.SuspendLayout();
            this.groupBoxRubyAddType.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxRubyAdd
            // 
            this.checkBoxRubyAdd.Location = new System.Drawing.Point(21, 25);
            this.checkBoxRubyAdd.Name = "checkBoxRubyAdd";
            this.checkBoxRubyAdd.Size = new System.Drawing.Size(77, 23);
            this.checkBoxRubyAdd.TabIndex = 16;
            this.checkBoxRubyAdd.Text = "読みを振る";
            this.toolTip1.SetToolTip(this.checkBoxRubyAdd, global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommandRubyOverwriteTooltip);
            this.checkBoxRubyAdd.UseVisualStyleBackColor = true;
            this.checkBoxRubyAdd.CheckedChanged += new System.EventHandler(this.checkBoxRubyAddEnabled_CheckedChanged);
            // 
            // checkBoxRubyAddOverwrite
            // 
            this.checkBoxRubyAddOverwrite.Location = new System.Drawing.Point(15, 22);
            this.checkBoxRubyAddOverwrite.Name = "checkBoxRubyAddOverwrite";
            this.checkBoxRubyAddOverwrite.Size = new System.Drawing.Size(111, 23);
            this.checkBoxRubyAddOverwrite.TabIndex = 3;
            this.checkBoxRubyAddOverwrite.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommandRubyOverwrite;
            this.toolTip1.SetToolTip(this.checkBoxRubyAddOverwrite, global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommandRubyOverwriteTooltip);
            this.checkBoxRubyAddOverwrite.UseVisualStyleBackColor = true;
            this.checkBoxRubyAddOverwrite.CheckedChanged += new System.EventHandler(this.checkBoxRubyAddOverwrite_CheckedChanged);
            // 
            // console
            // 
            this.console.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.console.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.console.HideSelection = false;
            this.console.Location = new System.Drawing.Point(0, 0);
            this.console.Multiline = true;
            this.console.Name = "console";
            this.console.ReadOnly = true;
            this.console.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.console.Size = new System.Drawing.Size(304, 186);
            this.console.TabIndex = 5;
            this.console.WordWrap = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 307);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "ログ";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.コマンドToolStripMenuItem,
            this.ヘルプToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(304, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // コマンドToolStripMenuItem
            // 
            this.コマンドToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.存在しないトラックを削除するToolStripMenuItem});
            this.コマンドToolStripMenuItem.Name = "コマンドToolStripMenuItem";
            this.コマンドToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
            this.コマンドToolStripMenuItem.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommand;
            // 
            // 存在しないトラックを削除するToolStripMenuItem
            // 
            this.存在しないトラックを削除するToolStripMenuItem.Name = "存在しないトラックを削除するToolStripMenuItem";
            this.存在しないトラックを削除するToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.存在しないトラックを削除するToolStripMenuItem.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommandDelete;
            this.存在しないトラックを削除するToolStripMenuItem.Click += new System.EventHandler(this.存在しないトラックを削除するToolStripMenuItem_Click);
            // 
            // ヘルプToolStripMenuItem
            // 
            this.ヘルプToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.バージョン情報ToolStripMenuItem});
            this.ヘルプToolStripMenuItem.Name = "ヘルプToolStripMenuItem";
            this.ヘルプToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.ヘルプToolStripMenuItem.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuHelp;
            // 
            // バージョン情報ToolStripMenuItem
            // 
            this.バージョン情報ToolStripMenuItem.Name = "バージョン情報ToolStripMenuItem";
            this.バージョン情報ToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.バージョン情報ToolStripMenuItem.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuHelpVersion;
            this.バージョン情報ToolStripMenuItem.Click += new System.EventHandler(this.バージョン情報ToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxRubyAdd);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxOption);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxRubyAdd);
            this.splitContainer1.Panel1.Controls.Add(this.buttonApply);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint);
            this.splitContainer1.Panel1MinSize = 320;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Panel2.Controls.Add(this.console);
            this.splitContainer1.Size = new System.Drawing.Size(304, 535);
            this.splitContainer1.SplitterDistance = 320;
            this.splitContainer1.TabIndex = 9;
            this.splitContainer1.TabStop = false;
            // 
            // groupBoxOption
            // 
            this.groupBoxOption.Controls.Add(this.checkBoxTrim);
            this.groupBoxOption.Controls.Add(this.checkBoxZenToHan);
            this.groupBoxOption.Location = new System.Drawing.Point(12, 191);
            this.groupBoxOption.Name = "groupBoxOption";
            this.groupBoxOption.Size = new System.Drawing.Size(207, 84);
            this.groupBoxOption.TabIndex = 15;
            this.groupBoxOption.TabStop = false;
            this.groupBoxOption.Text = "オプション";
            // 
            // checkBoxTrim
            // 
            this.checkBoxTrim.AutoSize = true;
            this.checkBoxTrim.Location = new System.Drawing.Point(3, 37);
            this.checkBoxTrim.Name = "checkBoxTrim";
            this.checkBoxTrim.Size = new System.Drawing.Size(134, 16);
            this.checkBoxTrim.TabIndex = 1;
            this.checkBoxTrim.Text = "両端の空白を除去する";
            this.checkBoxTrim.UseVisualStyleBackColor = true;
            // 
            // checkBoxZenToHan
            // 
            this.checkBoxZenToHan.AutoSize = true;
            this.checkBoxZenToHan.Location = new System.Drawing.Point(3, 15);
            this.checkBoxZenToHan.Name = "checkBoxZenToHan";
            this.checkBoxZenToHan.Size = new System.Drawing.Size(133, 16);
            this.checkBoxZenToHan.TabIndex = 0;
            this.checkBoxZenToHan.Text = "全角英数を半角にする";
            this.checkBoxZenToHan.UseVisualStyleBackColor = true;
            // 
            // groupBoxRubyAdd
            // 
            this.groupBoxRubyAdd.Controls.Add(this.groupBoxRubyAddType);
            this.groupBoxRubyAdd.Controls.Add(this.checkBoxRubyAddOverwrite);
            this.groupBoxRubyAdd.Location = new System.Drawing.Point(12, 30);
            this.groupBoxRubyAdd.Name = "groupBoxRubyAdd";
            this.groupBoxRubyAdd.Size = new System.Drawing.Size(207, 137);
            this.groupBoxRubyAdd.TabIndex = 10;
            this.groupBoxRubyAdd.TabStop = false;
            // 
            // groupBoxRubyAddType
            // 
            this.groupBoxRubyAddType.Controls.Add(this.radioButtonRubyAddTypeAlphabet);
            this.groupBoxRubyAddType.Controls.Add(this.radioButtonRubyAddTypeHiragana);
            this.groupBoxRubyAddType.Location = new System.Drawing.Point(9, 51);
            this.groupBoxRubyAddType.Name = "groupBoxRubyAddType";
            this.groupBoxRubyAddType.Size = new System.Drawing.Size(192, 72);
            this.groupBoxRubyAddType.TabIndex = 11;
            this.groupBoxRubyAddType.TabStop = false;
            this.groupBoxRubyAddType.Text = "読みの種類";
            // 
            // radioButtonRubyAddTypeAlphabet
            // 
            this.radioButtonRubyAddTypeAlphabet.AutoSize = true;
            this.radioButtonRubyAddTypeAlphabet.Location = new System.Drawing.Point(6, 40);
            this.radioButtonRubyAddTypeAlphabet.Name = "radioButtonRubyAddTypeAlphabet";
            this.radioButtonRubyAddTypeAlphabet.Size = new System.Drawing.Size(63, 16);
            this.radioButtonRubyAddTypeAlphabet.TabIndex = 15;
            this.radioButtonRubyAddTypeAlphabet.TabStop = true;
            this.radioButtonRubyAddTypeAlphabet.Text = "ローマ字";
            this.radioButtonRubyAddTypeAlphabet.UseVisualStyleBackColor = true;
            // 
            // radioButtonRubyAddTypeHiragana
            // 
            this.radioButtonRubyAddTypeHiragana.AutoSize = true;
            this.radioButtonRubyAddTypeHiragana.Location = new System.Drawing.Point(6, 18);
            this.radioButtonRubyAddTypeHiragana.Name = "radioButtonRubyAddTypeHiragana";
            this.radioButtonRubyAddTypeHiragana.Size = new System.Drawing.Size(61, 16);
            this.radioButtonRubyAddTypeHiragana.TabIndex = 14;
            this.radioButtonRubyAddTypeHiragana.TabStop = true;
            this.radioButtonRubyAddTypeHiragana.Text = "ひらがな";
            this.radioButtonRubyAddTypeHiragana.UseVisualStyleBackColor = true;
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(217, 281);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 14;
            this.buttonApply.Text = "適用";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 189);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(304, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // RootForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(304, 559);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(320, 480);
            this.Name = "RootForm";
            this.Text = "iTunesふりかけ";
            this.Activated += new System.EventHandler(this.RootForm_Activated);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.RootForm_Shown);
            this.SizeChanged += new System.EventHandler(this.RootForm_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBoxOption.ResumeLayout(false);
            this.groupBoxOption.PerformLayout();
            this.groupBoxRubyAdd.ResumeLayout(false);
            this.groupBoxRubyAddType.ResumeLayout(false);
            this.groupBoxRubyAddType.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox console;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem コマンドToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ヘルプToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem バージョン情報ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 存在しないトラックを削除するToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.GroupBox groupBoxOption;
		private System.Windows.Forms.CheckBox checkBoxTrim;
		private System.Windows.Forms.CheckBox checkBoxZenToHan;
		private System.Windows.Forms.CheckBox checkBoxRubyAdd;
		private System.Windows.Forms.GroupBox groupBoxRubyAdd;
		private System.Windows.Forms.GroupBox groupBoxRubyAddType;
		private System.Windows.Forms.RadioButton radioButtonRubyAddTypeAlphabet;
		private System.Windows.Forms.RadioButton radioButtonRubyAddTypeHiragana;
		private System.Windows.Forms.CheckBox checkBoxRubyAddOverwrite;
	}
}

