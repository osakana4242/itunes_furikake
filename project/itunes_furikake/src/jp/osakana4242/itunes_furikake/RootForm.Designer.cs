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
			this.checkBoxRubyAddOverwrite = new System.Windows.Forms.CheckBox();
			this.buttonApply = new System.Windows.Forms.Button();
			this.checkBoxRubyAdd = new System.Windows.Forms.CheckBox();
			this.console = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.コマンドToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItemRemoveUnexistsTrack = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemVersionInfo = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.labelHead = new System.Windows.Forms.Label();
			this.checkBoxRubyRemove = new System.Windows.Forms.CheckBox();
			this.checkBoxTrim = new System.Windows.Forms.CheckBox();
			this.checkBoxZenToHan = new System.Windows.Forms.CheckBox();
			this.groupBoxRubyAdd = new System.Windows.Forms.GroupBox();
			this.groupBoxRubyAddType = new System.Windows.Forms.GroupBox();
			this.radioButtonRubyAddTypeAlphabet = new System.Windows.Forms.RadioButton();
			this.radioButtonRubyAddTypeHiragana = new System.Windows.Forms.RadioButton();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBoxRubyAdd.SuspendLayout();
			this.groupBoxRubyAddType.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 6200;
			this.toolTip1.InitialDelay = 10;
			this.toolTip1.ReshowDelay = 124;
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
			// buttonApply
			// 
			this.buttonApply.Location = new System.Drawing.Point(217, 271);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(75, 23);
			this.buttonApply.TabIndex = 14;
			this.buttonApply.Text = "適用";
			this.buttonApply.UseVisualStyleBackColor = true;
			this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
			// 
			// checkBoxRubyAdd
			// 
			this.checkBoxRubyAdd.Location = new System.Drawing.Point(21, 40);
			this.checkBoxRubyAdd.Name = "checkBoxRubyAdd";
			this.checkBoxRubyAdd.Size = new System.Drawing.Size(77, 23);
			this.checkBoxRubyAdd.TabIndex = 16;
			this.checkBoxRubyAdd.Text = "読みを振る";
			this.checkBoxRubyAdd.UseVisualStyleBackColor = true;
			this.checkBoxRubyAdd.CheckedChanged += new System.EventHandler(this.checkBoxRubyAdd_CheckedChanged);
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
            this.toolStripMenuItemHelp});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(304, 24);
			this.menuStrip1.TabIndex = 8;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// コマンドToolStripMenuItem
			// 
			this.コマンドToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemRemoveUnexistsTrack});
			this.コマンドToolStripMenuItem.Name = "コマンドToolStripMenuItem";
			this.コマンドToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
			this.コマンドToolStripMenuItem.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommand;
			// 
			// ToolStripMenuItemRemoveUnexistsTrack
			// 
			this.ToolStripMenuItemRemoveUnexistsTrack.Name = "ToolStripMenuItemRemoveUnexistsTrack";
			this.ToolStripMenuItemRemoveUnexistsTrack.Size = new System.Drawing.Size(197, 22);
			this.ToolStripMenuItemRemoveUnexistsTrack.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommandDelete;
			this.ToolStripMenuItemRemoveUnexistsTrack.Click += new System.EventHandler(this.ToolStripMenuItemRemoveunexistsTrack_Click);
			// 
			// toolStripMenuItemHelp
			// 
			this.toolStripMenuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemVersionInfo});
			this.toolStripMenuItemHelp.Name = "toolStripMenuItemHelp";
			this.toolStripMenuItemHelp.Size = new System.Drawing.Size(48, 20);
			this.toolStripMenuItemHelp.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuHelp;
			// 
			// toolStripMenuItemVersionInfo
			// 
			this.toolStripMenuItemVersionInfo.Name = "toolStripMenuItemVersionInfo";
			this.toolStripMenuItemVersionInfo.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItemVersionInfo.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuHelpVersion;
			this.toolStripMenuItemVersionInfo.Click += new System.EventHandler(this.ToolStripMenuItemVersionInfo_Click);
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
			this.splitContainer1.Panel1.Controls.Add(this.labelHead);
			this.splitContainer1.Panel1.Controls.Add(this.checkBoxRubyRemove);
			this.splitContainer1.Panel1.Controls.Add(this.checkBoxTrim);
			this.splitContainer1.Panel1.Controls.Add(this.checkBoxZenToHan);
			this.splitContainer1.Panel1.Controls.Add(this.checkBoxRubyAdd);
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
			// labelHead
			// 
			this.labelHead.AutoSize = true;
			this.labelHead.Location = new System.Drawing.Point(10, 12);
			this.labelHead.Name = "labelHead";
			this.labelHead.Size = new System.Drawing.Size(206, 12);
			this.labelHead.TabIndex = 18;
			this.labelHead.Text = "選択したトラックに下記の処理を適用します";
			// 
			// checkBoxRubyRemove
			// 
			this.checkBoxRubyRemove.AutoSize = true;
			this.checkBoxRubyRemove.Location = new System.Drawing.Point(12, 172);
			this.checkBoxRubyRemove.Name = "checkBoxRubyRemove";
			this.checkBoxRubyRemove.Size = new System.Drawing.Size(78, 16);
			this.checkBoxRubyRemove.TabIndex = 17;
			this.checkBoxRubyRemove.Text = "読みを消す";
			this.checkBoxRubyRemove.UseVisualStyleBackColor = true;
			this.checkBoxRubyRemove.CheckedChanged += new System.EventHandler(this.checkBoxRubyRemove_CheckedChanged);
			// 
			// checkBoxTrim
			// 
			this.checkBoxTrim.AutoSize = true;
			this.checkBoxTrim.Location = new System.Drawing.Point(12, 238);
			this.checkBoxTrim.Name = "checkBoxTrim";
			this.checkBoxTrim.Size = new System.Drawing.Size(134, 16);
			this.checkBoxTrim.TabIndex = 1;
			this.checkBoxTrim.Text = "両端の空白を除去する";
			this.checkBoxTrim.UseVisualStyleBackColor = true;
			this.checkBoxTrim.CheckedChanged += new System.EventHandler(this.checkBoxTrim_CheckedChanged);
			// 
			// checkBoxZenToHan
			// 
			this.checkBoxZenToHan.AutoSize = true;
			this.checkBoxZenToHan.Location = new System.Drawing.Point(12, 216);
			this.checkBoxZenToHan.Name = "checkBoxZenToHan";
			this.checkBoxZenToHan.Size = new System.Drawing.Size(133, 16);
			this.checkBoxZenToHan.TabIndex = 0;
			this.checkBoxZenToHan.Text = "全角英数を半角にする";
			this.checkBoxZenToHan.UseVisualStyleBackColor = true;
			this.checkBoxZenToHan.CheckedChanged += new System.EventHandler(this.checkBoxZenToHan_CheckedChanged);
			// 
			// groupBoxRubyAdd
			// 
			this.groupBoxRubyAdd.Controls.Add(this.groupBoxRubyAddType);
			this.groupBoxRubyAdd.Controls.Add(this.checkBoxRubyAddOverwrite);
			this.groupBoxRubyAdd.Location = new System.Drawing.Point(12, 45);
			this.groupBoxRubyAdd.Name = "groupBoxRubyAdd";
			this.groupBoxRubyAdd.Size = new System.Drawing.Size(280, 121);
			this.groupBoxRubyAdd.TabIndex = 10;
			this.groupBoxRubyAdd.TabStop = false;
			// 
			// groupBoxRubyAddType
			// 
			this.groupBoxRubyAddType.Controls.Add(this.radioButtonRubyAddTypeAlphabet);
			this.groupBoxRubyAddType.Controls.Add(this.radioButtonRubyAddTypeHiragana);
			this.groupBoxRubyAddType.Location = new System.Drawing.Point(9, 51);
			this.groupBoxRubyAddType.Name = "groupBoxRubyAddType";
			this.groupBoxRubyAddType.Size = new System.Drawing.Size(265, 62);
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
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemVersionInfo;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemRemoveUnexistsTrack;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.CheckBox checkBoxRubyAdd;
		private System.Windows.Forms.GroupBox groupBoxRubyAdd;
		private System.Windows.Forms.GroupBox groupBoxRubyAddType;
		private System.Windows.Forms.RadioButton radioButtonRubyAddTypeAlphabet;
		private System.Windows.Forms.RadioButton radioButtonRubyAddTypeHiragana;
		private System.Windows.Forms.CheckBox checkBoxRubyAddOverwrite;
		private System.Windows.Forms.CheckBox checkBoxTrim;
		private System.Windows.Forms.CheckBox checkBoxZenToHan;
		private System.Windows.Forms.CheckBox checkBoxRubyRemove;
		private System.Windows.Forms.Label labelHead;
	}
}

