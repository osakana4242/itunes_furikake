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
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.console = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button5 = new System.Windows.Forms.Button();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.コマンドToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.全角英数を半角にするToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.存在しないトラックを削除するToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ヘルプToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.バージョン情報ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(12, 47);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(191, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommandRubyHiragana;
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(12, 105);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(191, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommandRubyRome;
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(24, 170);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(191, 23);
			this.button3.TabIndex = 2;
			this.button3.Text = "読みを消す";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(17, 18);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(111, 23);
			this.checkBox1.TabIndex = 3;
			this.checkBox1.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommandRubyOverwrite;
			this.toolTip1.SetToolTip(this.checkBox1, global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommandRubyOverwriteTooltip);
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// console
			// 
			this.console.Dock = System.Windows.Forms.DockStyle.Fill;
			this.console.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.console.HideSelection = false;
			this.console.Location = new System.Drawing.Point(0, 0);
			this.console.Multiline = true;
			this.console.Name = "console";
			this.console.ReadOnly = true;
			this.console.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.console.Size = new System.Drawing.Size(304, 173);
			this.console.TabIndex = 5;
			this.console.WordWrap = false;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(1, 227);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(23, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "ログ";
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(12, 76);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(191, 23);
			this.button5.TabIndex = 7;
			this.button5.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommandRubyKatakana;
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
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
            this.全角英数を半角にするToolStripMenuItem,
            this.存在しないトラックを削除するToolStripMenuItem});
			this.コマンドToolStripMenuItem.Name = "コマンドToolStripMenuItem";
			this.コマンドToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
			this.コマンドToolStripMenuItem.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommand;
			// 
			// 全角英数を半角にするToolStripMenuItem
			// 
			this.全角英数を半角にするToolStripMenuItem.Name = "全角英数を半角にするToolStripMenuItem";
			this.全角英数を半角にするToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.全角英数を半角にするToolStripMenuItem.Text = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrMenuCommandZenkakuToHankaku;
			this.全角英数を半角にするToolStripMenuItem.Click += new System.EventHandler(this.全角英数を半角にするToolStripMenuItem_Click);
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
			this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.button3);
			this.splitContainer1.Panel1MinSize = 150;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
			this.splitContainer1.Panel2.Controls.Add(this.console);
			this.splitContainer1.Size = new System.Drawing.Size(304, 417);
			this.splitContainer1.SplitterDistance = 240;
			this.splitContainer1.TabIndex = 9;
			this.splitContainer1.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Controls.Add(this.button2);
			this.groupBox1.Controls.Add(this.button5);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Location = new System.Drawing.Point(12, 21);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(212, 143);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "読みを降る";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 151);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(304, 22);
			this.statusStrip1.TabIndex = 6;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
			this.toolStripStatusLabel1.Text = "";
			// 
			// RootForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(304, 441);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.MinimumSize = new System.Drawing.Size(320, 480);
			this.Name = "RootForm";
			this.Text = "iTunesふりかけ";
			this.Activated += new System.EventHandler(this.RootForm_Activated);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox console;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem コマンドToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ヘルプToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem バージョン情報ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 全角英数を半角にするToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 存在しないトラックを削除するToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
	}
}

