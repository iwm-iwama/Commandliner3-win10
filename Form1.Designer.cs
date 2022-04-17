namespace iwm_Commandliner3
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.BtnAllClear = new System.Windows.Forms.Button();
			this.CmsNull = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.BtnCmdExec = new System.Windows.Forms.Button();
			this.BtnCmdExecStream = new System.Windows.Forms.Button();
			this.BtnCmdExecUndo = new System.Windows.Forms.Button();
			this.BtnDgvCmd = new System.Windows.Forms.Button();
			this.BtnDgvSearch = new System.Windows.Forms.Button();
			this.BtnDgvMacro = new System.Windows.Forms.Button();
			this.BtnPasteFilename = new System.Windows.Forms.Button();
			this.BtnPasteTextfile = new System.Windows.Forms.Button();
			this.BtnResult1 = new System.Windows.Forms.Button();
			this.BtnResult2 = new System.Windows.Forms.Button();
			this.BtnResult3 = new System.Windows.Forms.Button();
			this.BtnResult4 = new System.Windows.Forms.Button();
			this.BtnResult5 = new System.Windows.Forms.Button();
			this.CbCmdHistory = new System.Windows.Forms.ComboBox();
			this.CbResultHistory = new System.Windows.Forms.ComboBox();
			this.CmsCmd = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CmsCmd_クリア = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_全コピー = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_上書き = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_tss01 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd_貼り付け = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_マクロ変数 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_tss02 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd_フォルダ選択 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_ファイル選択 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_tss03 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd_コマンドをグループ化 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_コマンドをグループ化_追加 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_コマンドをグループ化_出力 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_コマンドをグループ化_クリア = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_コマンドを保存 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_コマンドを読込 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_コマンドを読込_再読込 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CmsCmd2_閉じる = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_tss01 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd2_タブ = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_改行 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_ダブルクォーテーション = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_セミコロン = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_tss02 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd2_現時間 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_日付 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_時間 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_ミリ秒 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_年 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_月 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_日 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_時 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_分 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_秒 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_tss03 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd2_出力の行データ = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_出力の行番号 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd2_tss04 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd2_一時変数 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmdLog = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CmsCmdLog_クリア = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmdLog_全コピー = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmdLog_上書き = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmdLog_tss01 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmdLog_貼り付け = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmdLog_tss02 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmdLog_拡大 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmdLog_元に戻す = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CmsResult_全選択 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_tss01 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsResult_クリア = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_全コピー = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_上書き = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_tss02 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsResult_貼り付け = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_ファイル名を貼り付け = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_tss03 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsResult_出力へコピー = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_出力へコピー_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_出力へコピー_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_出力へコピー_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_出力へコピー_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_出力へコピー_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_tss04 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsResult_名前を付けて保存 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_名前を付けて保存_SJIS = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_名前を付けて保存_UTF8N = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTbCurDir = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CmsTbCurDir_全コピー = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTbDgvSearch = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CmsTbDgvSearch_クリア = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTbDgvSearch_貼り付け = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CmsTextSelect_Cancel = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_tss01 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsTextSelect_コピー = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_切り取り = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_tss02 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsTextSelect_貼り付け = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_tss03 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsTextSelect_DQで囲む = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_DQを消去 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_tss04 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsTextSelect_ネット検索 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_ネット検索_URLを開く = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_ネット検索_tss01 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsTextSelect_ネット検索_Google = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_ネット検索_Google翻訳 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_ネット検索_Googleマップ = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_ネット検索_YouTube = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_ネット検索_Wikipedia = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsTextSelect_tss05 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsTextSelect_関連付けられたアプリケーションで開く = new System.Windows.Forms.ToolStripMenuItem();
			this.DgvCmd = new System.Windows.Forms.DataGridView();
			this.DgvTb21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DgvMacro = new System.Windows.Forms.DataGridView();
			this.DgvTb11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DgvTb12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Lbl_F1 = new System.Windows.Forms.Label();
			this.Lbl_F2 = new System.Windows.Forms.Label();
			this.Lbl_F3 = new System.Windows.Forms.Label();
			this.Lbl_F5 = new System.Windows.Forms.Label();
			this.Lbl_F6 = new System.Windows.Forms.Label();
			this.Lbl_F7 = new System.Windows.Forms.Label();
			this.Lbl_F8 = new System.Windows.Forms.Label();
			this.LblCmd = new System.Windows.Forms.Label();
			this.LblCmdLog = new System.Windows.Forms.Label();
			this.LblCurDir = new System.Windows.Forms.Label();
			this.LblFontSize = new System.Windows.Forms.Label();
			this.LblResult = new System.Windows.Forms.Label();
			this.LblWait = new System.Windows.Forms.Label();
			this.NudFontSize = new System.Windows.Forms.NumericUpDown();
			this.RtbCmdLog = new System.Windows.Forms.RichTextBox();
			this.ScrTbResult = new System.Windows.Forms.SplitContainer();
			this.TbCmd = new System.Windows.Forms.TextBox();
			this.TbCurDir = new System.Windows.Forms.TextBox();
			this.TbDgvSearch = new System.Windows.Forms.TextBox();
			this.TbInfo = new System.Windows.Forms.TextBox();
			this.TbResult = new System.Windows.Forms.TextBox();
			this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.ChkTopMost = new System.Windows.Forms.CheckBox();
			this.CmsCmd.SuspendLayout();
			this.CmsCmd2.SuspendLayout();
			this.CmsCmdLog.SuspendLayout();
			this.CmsResult.SuspendLayout();
			this.CmsTbCurDir.SuspendLayout();
			this.CmsTbDgvSearch.SuspendLayout();
			this.CmsTextSelect.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.DgvCmd)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.DgvMacro)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NudFontSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ScrTbResult)).BeginInit();
			this.ScrTbResult.Panel1.SuspendLayout();
			this.ScrTbResult.Panel2.SuspendLayout();
			this.ScrTbResult.SuspendLayout();
			this.SuspendLayout();
			// 
			// BtnAllClear
			// 
			this.BtnAllClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnAllClear.BackColor = System.Drawing.Color.Crimson;
			this.BtnAllClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BtnAllClear.ContextMenuStrip = this.CmsNull;
			this.BtnAllClear.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.BtnAllClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnAllClear.Font = new System.Drawing.Font("Yu Gothic UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnAllClear.ForeColor = System.Drawing.Color.White;
			this.BtnAllClear.Location = new System.Drawing.Point(595, 134);
			this.BtnAllClear.Margin = new System.Windows.Forms.Padding(0);
			this.BtnAllClear.Name = "BtnAllClear";
			this.BtnAllClear.Size = new System.Drawing.Size(22, 22);
			this.BtnAllClear.TabIndex = 0;
			this.BtnAllClear.TabStop = false;
			this.BtnAllClear.Text = "✖";
			this.ToolTip.SetToolTip(this.BtnAllClear, "[F7] 出力をクリア");
			this.BtnAllClear.UseVisualStyleBackColor = false;
			this.BtnAllClear.Click += new System.EventHandler(this.BtnClear_Click);
			// 
			// CmsNull
			// 
			this.CmsNull.Name = "contextMenuStrip0";
			this.CmsNull.Size = new System.Drawing.Size(61, 4);
			// 
			// BtnCmdExec
			// 
			this.BtnCmdExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnCmdExec.BackColor = System.Drawing.Color.RoyalBlue;
			this.BtnCmdExec.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BtnCmdExec.ContextMenuStrip = this.CmsNull;
			this.BtnCmdExec.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.BtnCmdExec.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnCmdExec.Font = new System.Drawing.Font("Yu Gothic UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnCmdExec.ForeColor = System.Drawing.Color.White;
			this.BtnCmdExec.Location = new System.Drawing.Point(537, 134);
			this.BtnCmdExec.Margin = new System.Windows.Forms.Padding(0);
			this.BtnCmdExec.Name = "BtnCmdExec";
			this.BtnCmdExec.Size = new System.Drawing.Size(22, 22);
			this.BtnCmdExec.TabIndex = 0;
			this.BtnCmdExec.TabStop = false;
			this.BtnCmdExec.Text = "▶";
			this.BtnCmdExec.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.ToolTip.SetToolTip(this.BtnCmdExec, " [F5] 実行");
			this.BtnCmdExec.UseVisualStyleBackColor = false;
			this.BtnCmdExec.Click += new System.EventHandler(this.BtnCmdExec_Click);
			// 
			// BtnCmdExecStream
			// 
			this.BtnCmdExecStream.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.BtnCmdExecStream.BackColor = System.Drawing.Color.RoyalBlue;
			this.BtnCmdExecStream.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.BtnCmdExecStream.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
			this.BtnCmdExecStream.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.BtnCmdExecStream.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnCmdExecStream.Font = new System.Drawing.Font("Yu Gothic UI", 6F);
			this.BtnCmdExecStream.ForeColor = System.Drawing.Color.White;
			this.BtnCmdExecStream.Location = new System.Drawing.Point(290, 44);
			this.BtnCmdExecStream.Margin = new System.Windows.Forms.Padding(0);
			this.BtnCmdExecStream.Name = "BtnCmdExecStream";
			this.BtnCmdExecStream.Size = new System.Drawing.Size(45, 21);
			this.BtnCmdExecStream.TabIndex = 0;
			this.BtnCmdExecStream.TabStop = false;
			this.BtnCmdExecStream.Text = "✖";
			this.ToolTip.SetToolTip(this.BtnCmdExecStream, "ダブルクリックで停止");
			this.BtnCmdExecStream.UseVisualStyleBackColor = false;
			this.BtnCmdExecStream.Visible = false;
			this.BtnCmdExecStream.Click += new System.EventHandler(this.BtnCmdExecStream_Click);
			// 
			// BtnCmdExecUndo
			// 
			this.BtnCmdExecUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnCmdExecUndo.BackColor = System.Drawing.Color.DimGray;
			this.BtnCmdExecUndo.ContextMenuStrip = this.CmsNull;
			this.BtnCmdExecUndo.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.BtnCmdExecUndo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnCmdExecUndo.Font = new System.Drawing.Font("Yu Gothic UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnCmdExecUndo.ForeColor = System.Drawing.Color.White;
			this.BtnCmdExecUndo.Location = new System.Drawing.Point(566, 134);
			this.BtnCmdExecUndo.Margin = new System.Windows.Forms.Padding(0);
			this.BtnCmdExecUndo.Name = "BtnCmdExecUndo";
			this.BtnCmdExecUndo.Size = new System.Drawing.Size(22, 22);
			this.BtnCmdExecUndo.TabIndex = 0;
			this.BtnCmdExecUndo.TabStop = false;
			this.BtnCmdExecUndo.Text = "◀";
			this.BtnCmdExecUndo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.ToolTip.SetToolTip(this.BtnCmdExecUndo, "[F6] 出力を実行前に戻す");
			this.BtnCmdExecUndo.UseVisualStyleBackColor = false;
			this.BtnCmdExecUndo.Click += new System.EventHandler(this.BtnCmdExecUndo_Click);
			// 
			// BtnDgvCmd
			// 
			this.BtnDgvCmd.BackColor = System.Drawing.Color.RoyalBlue;
			this.BtnDgvCmd.ContextMenuStrip = this.CmsNull;
			this.BtnDgvCmd.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.BtnDgvCmd.FlatAppearance.CheckedBackColor = System.Drawing.Color.Crimson;
			this.BtnDgvCmd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Crimson;
			this.BtnDgvCmd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Crimson;
			this.BtnDgvCmd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnDgvCmd.Font = new System.Drawing.Font("Yu Gothic UI", 3.75F);
			this.BtnDgvCmd.ForeColor = System.Drawing.Color.Black;
			this.BtnDgvCmd.Location = new System.Drawing.Point(160, 138);
			this.BtnDgvCmd.Margin = new System.Windows.Forms.Padding(0);
			this.BtnDgvCmd.Name = "BtnDgvCmd";
			this.BtnDgvCmd.Size = new System.Drawing.Size(14, 14);
			this.BtnDgvCmd.TabIndex = 0;
			this.BtnDgvCmd.TabStop = false;
			this.ToolTip.SetToolTip(this.BtnDgvCmd, "[F3] コマンド");
			this.BtnDgvCmd.UseVisualStyleBackColor = false;
			this.BtnDgvCmd.Click += new System.EventHandler(this.BtnDgvCmd_Click);
			// 
			// BtnDgvSearch
			// 
			this.BtnDgvSearch.BackColor = System.Drawing.SystemColors.Window;
			this.BtnDgvSearch.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.BtnDgvSearch.FlatAppearance.BorderSize = 0;
			this.BtnDgvSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
			this.BtnDgvSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
			this.BtnDgvSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnDgvSearch.ForeColor = System.Drawing.Color.White;
			this.BtnDgvSearch.Location = new System.Drawing.Point(360, 135);
			this.BtnDgvSearch.Margin = new System.Windows.Forms.Padding(0);
			this.BtnDgvSearch.Name = "BtnDgvSearch";
			this.BtnDgvSearch.Size = new System.Drawing.Size(19, 17);
			this.BtnDgvSearch.TabIndex = 0;
			this.BtnDgvSearch.TabStop = false;
			this.ToolTip.SetToolTip(this.BtnDgvSearch, "検索開始");
			this.BtnDgvSearch.UseVisualStyleBackColor = false;
			this.BtnDgvSearch.Click += new System.EventHandler(this.BtnDgvSearch_Click);
			// 
			// BtnDgvMacro
			// 
			this.BtnDgvMacro.BackColor = System.Drawing.Color.RoyalBlue;
			this.BtnDgvMacro.ContextMenuStrip = this.CmsNull;
			this.BtnDgvMacro.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.BtnDgvMacro.FlatAppearance.CheckedBackColor = System.Drawing.Color.Crimson;
			this.BtnDgvMacro.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Crimson;
			this.BtnDgvMacro.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Crimson;
			this.BtnDgvMacro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnDgvMacro.Font = new System.Drawing.Font("Yu Gothic UI", 3.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnDgvMacro.ForeColor = System.Drawing.Color.Black;
			this.BtnDgvMacro.Location = new System.Drawing.Point(73, 138);
			this.BtnDgvMacro.Margin = new System.Windows.Forms.Padding(0);
			this.BtnDgvMacro.Name = "BtnDgvMacro";
			this.BtnDgvMacro.Size = new System.Drawing.Size(14, 14);
			this.BtnDgvMacro.TabIndex = 0;
			this.BtnDgvMacro.TabStop = false;
			this.ToolTip.SetToolTip(this.BtnDgvMacro, "[F2] マクロ");
			this.BtnDgvMacro.UseVisualStyleBackColor = false;
			this.BtnDgvMacro.Click += new System.EventHandler(this.BtnDgvMacro_Click);
			// 
			// BtnPasteFilename
			// 
			this.BtnPasteFilename.AllowDrop = true;
			this.BtnPasteFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnPasteFilename.BackColor = System.Drawing.Color.RoyalBlue;
			this.BtnPasteFilename.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.BtnPasteFilename.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
			this.BtnPasteFilename.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BtnPasteFilename.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnPasteFilename.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnPasteFilename.ForeColor = System.Drawing.Color.White;
			this.BtnPasteFilename.Location = new System.Drawing.Point(21, 23);
			this.BtnPasteFilename.Margin = new System.Windows.Forms.Padding(0);
			this.BtnPasteFilename.Name = "BtnPasteFilename";
			this.BtnPasteFilename.Size = new System.Drawing.Size(260, 300);
			this.BtnPasteFilename.TabIndex = 2;
			this.BtnPasteFilename.TabStop = false;
			this.BtnPasteFilename.Text = "ファイル名";
			this.BtnPasteFilename.UseVisualStyleBackColor = false;
			this.BtnPasteFilename.Click += new System.EventHandler(this.BtnPasteFilename_Click);
			this.BtnPasteFilename.DragDrop += new System.Windows.Forms.DragEventHandler(this.BtnPasteFilename_DragDrop);
			this.BtnPasteFilename.DragEnter += new System.Windows.Forms.DragEventHandler(this.BtnPasteFilename_DragEnter);
			// 
			// BtnPasteTextfile
			// 
			this.BtnPasteTextfile.AllowDrop = true;
			this.BtnPasteTextfile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnPasteTextfile.BackColor = System.Drawing.Color.Crimson;
			this.BtnPasteTextfile.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.BtnPasteTextfile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Crimson;
			this.BtnPasteTextfile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
			this.BtnPasteTextfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnPasteTextfile.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnPasteTextfile.ForeColor = System.Drawing.Color.White;
			this.BtnPasteTextfile.Location = new System.Drawing.Point(21, 23);
			this.BtnPasteTextfile.Margin = new System.Windows.Forms.Padding(0);
			this.BtnPasteTextfile.Name = "BtnPasteTextfile";
			this.BtnPasteTextfile.Size = new System.Drawing.Size(260, 300);
			this.BtnPasteTextfile.TabIndex = 3;
			this.BtnPasteTextfile.TabStop = false;
			this.BtnPasteTextfile.Text = "テキスト";
			this.BtnPasteTextfile.UseVisualStyleBackColor = false;
			this.BtnPasteTextfile.Click += new System.EventHandler(this.BtnPasteTextfile_Click);
			this.BtnPasteTextfile.DragDrop += new System.Windows.Forms.DragEventHandler(this.BtnPasteTextfile_DragDrop);
			this.BtnPasteTextfile.DragEnter += new System.Windows.Forms.DragEventHandler(this.BtnPasteTextfile_DragEnter);
			// 
			// BtnResult1
			// 
			this.BtnResult1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.BtnResult1.BackColor = System.Drawing.Color.DimGray;
			this.BtnResult1.FlatAppearance.BorderColor = System.Drawing.Color.Crimson;
			this.BtnResult1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnResult1.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BtnResult1.ForeColor = System.Drawing.Color.White;
			this.BtnResult1.Location = new System.Drawing.Point(164, 522);
			this.BtnResult1.Margin = new System.Windows.Forms.Padding(0);
			this.BtnResult1.Name = "BtnResult1";
			this.BtnResult1.Size = new System.Drawing.Size(60, 18);
			this.BtnResult1.TabIndex = 0;
			this.BtnResult1.TabStop = false;
			this.BtnResult1.Text = "1";
			this.BtnResult1.UseVisualStyleBackColor = false;
			this.BtnResult1.Click += new System.EventHandler(this.BtnResult1_Click);
			// 
			// BtnResult2
			// 
			this.BtnResult2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.BtnResult2.BackColor = System.Drawing.Color.DimGray;
			this.BtnResult2.FlatAppearance.BorderColor = System.Drawing.Color.Crimson;
			this.BtnResult2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnResult2.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BtnResult2.ForeColor = System.Drawing.Color.White;
			this.BtnResult2.Location = new System.Drawing.Point(223, 522);
			this.BtnResult2.Margin = new System.Windows.Forms.Padding(0);
			this.BtnResult2.Name = "BtnResult2";
			this.BtnResult2.Size = new System.Drawing.Size(60, 18);
			this.BtnResult2.TabIndex = 0;
			this.BtnResult2.TabStop = false;
			this.BtnResult2.Text = "2";
			this.BtnResult2.UseVisualStyleBackColor = false;
			this.BtnResult2.Click += new System.EventHandler(this.BtnResult2_Click);
			// 
			// BtnResult3
			// 
			this.BtnResult3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.BtnResult3.BackColor = System.Drawing.Color.DimGray;
			this.BtnResult3.FlatAppearance.BorderColor = System.Drawing.Color.Crimson;
			this.BtnResult3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnResult3.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BtnResult3.ForeColor = System.Drawing.Color.White;
			this.BtnResult3.Location = new System.Drawing.Point(282, 522);
			this.BtnResult3.Margin = new System.Windows.Forms.Padding(0);
			this.BtnResult3.Name = "BtnResult3";
			this.BtnResult3.Size = new System.Drawing.Size(60, 18);
			this.BtnResult3.TabIndex = 0;
			this.BtnResult3.TabStop = false;
			this.BtnResult3.Text = "3";
			this.BtnResult3.UseVisualStyleBackColor = false;
			this.BtnResult3.Click += new System.EventHandler(this.BtnResult3_Click);
			// 
			// BtnResult4
			// 
			this.BtnResult4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.BtnResult4.BackColor = System.Drawing.Color.DimGray;
			this.BtnResult4.FlatAppearance.BorderColor = System.Drawing.Color.Crimson;
			this.BtnResult4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnResult4.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BtnResult4.ForeColor = System.Drawing.Color.White;
			this.BtnResult4.Location = new System.Drawing.Point(341, 522);
			this.BtnResult4.Margin = new System.Windows.Forms.Padding(0);
			this.BtnResult4.Name = "BtnResult4";
			this.BtnResult4.Size = new System.Drawing.Size(60, 18);
			this.BtnResult4.TabIndex = 0;
			this.BtnResult4.TabStop = false;
			this.BtnResult4.Text = "4";
			this.BtnResult4.UseVisualStyleBackColor = false;
			this.BtnResult4.Click += new System.EventHandler(this.BtnResult4_Click);
			// 
			// BtnResult5
			// 
			this.BtnResult5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.BtnResult5.BackColor = System.Drawing.Color.DimGray;
			this.BtnResult5.FlatAppearance.BorderColor = System.Drawing.Color.Crimson;
			this.BtnResult5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnResult5.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BtnResult5.ForeColor = System.Drawing.Color.White;
			this.BtnResult5.Location = new System.Drawing.Point(400, 522);
			this.BtnResult5.Margin = new System.Windows.Forms.Padding(0);
			this.BtnResult5.Name = "BtnResult5";
			this.BtnResult5.Size = new System.Drawing.Size(60, 18);
			this.BtnResult5.TabIndex = 0;
			this.BtnResult5.TabStop = false;
			this.BtnResult5.Text = "5";
			this.BtnResult5.UseVisualStyleBackColor = false;
			this.BtnResult5.Click += new System.EventHandler(this.BtnResult5_Click);
			// 
			// CbCmdHistory
			// 
			this.CbCmdHistory.BackColor = System.Drawing.Color.WhiteSmoke;
			this.CbCmdHistory.ContextMenuStrip = this.CmsNull;
			this.CbCmdHistory.DropDownHeight = 120;
			this.CbCmdHistory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CbCmdHistory.DropDownWidth = 490;
			this.CbCmdHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CbCmdHistory.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.CbCmdHistory.ForeColor = System.Drawing.Color.Black;
			this.CbCmdHistory.FormattingEnabled = true;
			this.CbCmdHistory.IntegralHeight = false;
			this.CbCmdHistory.ItemHeight = 13;
			this.CbCmdHistory.Location = new System.Drawing.Point(11, 135);
			this.CbCmdHistory.Margin = new System.Windows.Forms.Padding(0);
			this.CbCmdHistory.MaxDropDownItems = 10;
			this.CbCmdHistory.Name = "CbCmdHistory";
			this.CbCmdHistory.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.CbCmdHistory.Size = new System.Drawing.Size(55, 21);
			this.CbCmdHistory.TabIndex = 0;
			this.CbCmdHistory.TabStop = false;
			this.ToolTip.SetToolTip(this.CbCmdHistory, "[F1] マクロ・コマンド履歴");
			this.CbCmdHistory.DropDownClosed += new System.EventHandler(this.CbCmdHistory_DropDownClosed);
			this.CbCmdHistory.Enter += new System.EventHandler(this.CbCmdHistory_Enter);
			this.CbCmdHistory.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CbCmdHistory_KeyUp);
			this.CbCmdHistory.Leave += new System.EventHandler(this.CbCmdHistory_Leave);
			// 
			// CbResultHistory
			// 
			this.CbResultHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.CbResultHistory.BackColor = System.Drawing.Color.WhiteSmoke;
			this.CbResultHistory.ContextMenuStrip = this.CmsNull;
			this.CbResultHistory.DropDownHeight = 140;
			this.CbResultHistory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CbResultHistory.DropDownWidth = 490;
			this.CbResultHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CbResultHistory.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.CbResultHistory.ForeColor = System.Drawing.Color.Black;
			this.CbResultHistory.FormattingEnabled = true;
			this.CbResultHistory.IntegralHeight = false;
			this.CbResultHistory.ItemHeight = 12;
			this.CbResultHistory.Location = new System.Drawing.Point(11, 536);
			this.CbResultHistory.Margin = new System.Windows.Forms.Padding(0);
			this.CbResultHistory.MaxDropDownItems = 10;
			this.CbResultHistory.Name = "CbResultHistory";
			this.CbResultHistory.Size = new System.Drawing.Size(70, 20);
			this.CbResultHistory.TabIndex = 0;
			this.CbResultHistory.TabStop = false;
			this.ToolTip.SetToolTip(this.CbResultHistory, "[F8] 出力履歴");
			this.CbResultHistory.DropDownClosed += new System.EventHandler(this.CbResultHistory_DropDownClosed);
			this.CbResultHistory.Enter += new System.EventHandler(this.CbResultHistory_Enter);
			this.CbResultHistory.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CbResultHistory_KeyUp);
			this.CbResultHistory.Leave += new System.EventHandler(this.CbResultHistory_Leave);
			// 
			// CmsCmd
			// 
			this.CmsCmd.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsCmd_クリア,
            this.CmsCmd_全コピー,
            this.CmsCmd_上書き,
            this.CmsCmd_tss01,
            this.CmsCmd_貼り付け,
            this.CmsCmd_マクロ変数,
            this.CmsCmd_tss02,
            this.CmsCmd_フォルダ選択,
            this.CmsCmd_ファイル選択,
            this.CmsCmd_tss03,
            this.CmsCmd_コマンドをグループ化,
            this.CmsCmd_コマンドを保存,
            this.CmsCmd_コマンドを読込});
			this.CmsCmd.Name = "CmsResult";
			this.CmsCmd.Size = new System.Drawing.Size(166, 242);
			// 
			// CmsCmd_クリア
			// 
			this.CmsCmd_クリア.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_クリア.Image")));
			this.CmsCmd_クリア.Name = "CmsCmd_クリア";
			this.CmsCmd_クリア.Size = new System.Drawing.Size(165, 22);
			this.CmsCmd_クリア.Text = "クリア";
			this.CmsCmd_クリア.Click += new System.EventHandler(this.CmsCmd_クリア_Click);
			// 
			// CmsCmd_全コピー
			// 
			this.CmsCmd_全コピー.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_全コピー.Image")));
			this.CmsCmd_全コピー.Name = "CmsCmd_全コピー";
			this.CmsCmd_全コピー.Size = new System.Drawing.Size(165, 22);
			this.CmsCmd_全コピー.Text = "全コピー";
			this.CmsCmd_全コピー.Click += new System.EventHandler(this.CmsCmd_全コピー_Click);
			// 
			// CmsCmd_上書き
			// 
			this.CmsCmd_上書き.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_上書き.Image")));
			this.CmsCmd_上書き.Name = "CmsCmd_上書き";
			this.CmsCmd_上書き.Size = new System.Drawing.Size(165, 22);
			this.CmsCmd_上書き.Text = "上書き";
			this.CmsCmd_上書き.Click += new System.EventHandler(this.CmsCmd_上書き_Click);
			// 
			// CmsCmd_tss01
			// 
			this.CmsCmd_tss01.Name = "CmsCmd_tss01";
			this.CmsCmd_tss01.Size = new System.Drawing.Size(162, 6);
			// 
			// CmsCmd_貼り付け
			// 
			this.CmsCmd_貼り付け.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_貼り付け.Image")));
			this.CmsCmd_貼り付け.Name = "CmsCmd_貼り付け";
			this.CmsCmd_貼り付け.Size = new System.Drawing.Size(165, 22);
			this.CmsCmd_貼り付け.Text = "貼り付け";
			this.CmsCmd_貼り付け.Click += new System.EventHandler(this.CmsCmd_貼り付け_Click);
			// 
			// CmsCmd_マクロ変数
			// 
			this.CmsCmd_マクロ変数.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_マクロ変数.Image")));
			this.CmsCmd_マクロ変数.Name = "CmsCmd_マクロ変数";
			this.CmsCmd_マクロ変数.Size = new System.Drawing.Size(165, 22);
			this.CmsCmd_マクロ変数.Text = "マクロ変数";
			this.CmsCmd_マクロ変数.Click += new System.EventHandler(this.CmsCmd_マクロ変数_Click);
			// 
			// CmsCmd_tss02
			// 
			this.CmsCmd_tss02.Name = "CmsCmd_tss02";
			this.CmsCmd_tss02.Size = new System.Drawing.Size(162, 6);
			// 
			// CmsCmd_フォルダ選択
			// 
			this.CmsCmd_フォルダ選択.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_フォルダ選択.Image")));
			this.CmsCmd_フォルダ選択.Name = "CmsCmd_フォルダ選択";
			this.CmsCmd_フォルダ選択.Size = new System.Drawing.Size(165, 22);
			this.CmsCmd_フォルダ選択.Text = "フォルダ選択";
			this.CmsCmd_フォルダ選択.Click += new System.EventHandler(this.CmsCmd_フォルダ選択_Click);
			// 
			// CmsCmd_ファイル選択
			// 
			this.CmsCmd_ファイル選択.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_ファイル選択.Image")));
			this.CmsCmd_ファイル選択.Name = "CmsCmd_ファイル選択";
			this.CmsCmd_ファイル選択.Size = new System.Drawing.Size(165, 22);
			this.CmsCmd_ファイル選択.Text = "ファイル選択";
			this.CmsCmd_ファイル選択.Click += new System.EventHandler(this.CmsCmd_ファイル選択_Click);
			// 
			// CmsCmd_tss03
			// 
			this.CmsCmd_tss03.Name = "CmsCmd_tss03";
			this.CmsCmd_tss03.Size = new System.Drawing.Size(162, 6);
			// 
			// CmsCmd_コマンドをグループ化
			// 
			this.CmsCmd_コマンドをグループ化.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsCmd_コマンドをグループ化_追加,
            this.CmsCmd_コマンドをグループ化_出力,
            this.CmsCmd_コマンドをグループ化_クリア});
			this.CmsCmd_コマンドをグループ化.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_コマンドをグループ化.Image")));
			this.CmsCmd_コマンドをグループ化.Name = "CmsCmd_コマンドをグループ化";
			this.CmsCmd_コマンドをグループ化.Size = new System.Drawing.Size(165, 22);
			this.CmsCmd_コマンドをグループ化.Text = "コマンドをグループ化";
			// 
			// CmsCmd_コマンドをグループ化_追加
			// 
			this.CmsCmd_コマンドをグループ化_追加.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_コマンドをグループ化_追加.Image")));
			this.CmsCmd_コマンドをグループ化_追加.Name = "CmsCmd_コマンドをグループ化_追加";
			this.CmsCmd_コマンドをグループ化_追加.Size = new System.Drawing.Size(152, 22);
			this.CmsCmd_コマンドをグループ化_追加.Text = "キャッシュに追加";
			this.CmsCmd_コマンドをグループ化_追加.Click += new System.EventHandler(this.CmsCmd_コマンドをグループ化_追加_Click);
			// 
			// CmsCmd_コマンドをグループ化_出力
			// 
			this.CmsCmd_コマンドをグループ化_出力.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_コマンドをグループ化_出力.Image")));
			this.CmsCmd_コマンドをグループ化_出力.Name = "CmsCmd_コマンドをグループ化_出力";
			this.CmsCmd_コマンドをグループ化_出力.Size = new System.Drawing.Size(152, 22);
			this.CmsCmd_コマンドをグループ化_出力.Text = "出力";
			this.CmsCmd_コマンドをグループ化_出力.Click += new System.EventHandler(this.CmsCmd_コマンドをグループ化_出力_Click);
			// 
			// CmsCmd_コマンドをグループ化_クリア
			// 
			this.CmsCmd_コマンドをグループ化_クリア.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_コマンドをグループ化_クリア.Image")));
			this.CmsCmd_コマンドをグループ化_クリア.Name = "CmsCmd_コマンドをグループ化_クリア";
			this.CmsCmd_コマンドをグループ化_クリア.Size = new System.Drawing.Size(152, 22);
			this.CmsCmd_コマンドをグループ化_クリア.Text = "キャッシュをクリア";
			this.CmsCmd_コマンドをグループ化_クリア.Click += new System.EventHandler(this.CmsCmd_コマンドをグループ化_クリア_Click);
			// 
			// CmsCmd_コマンドを保存
			// 
			this.CmsCmd_コマンドを保存.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_コマンドを保存.Image")));
			this.CmsCmd_コマンドを保存.Name = "CmsCmd_コマンドを保存";
			this.CmsCmd_コマンドを保存.Size = new System.Drawing.Size(165, 22);
			this.CmsCmd_コマンドを保存.Text = "コマンドを保存";
			this.CmsCmd_コマンドを保存.Click += new System.EventHandler(this.CmsCmd_コマンドを保存_Click);
			// 
			// CmsCmd_コマンドを読込
			// 
			this.CmsCmd_コマンドを読込.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsCmd_コマンドを読込_再読込});
			this.CmsCmd_コマンドを読込.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_コマンドを読込.Image")));
			this.CmsCmd_コマンドを読込.Name = "CmsCmd_コマンドを読込";
			this.CmsCmd_コマンドを読込.Size = new System.Drawing.Size(165, 22);
			this.CmsCmd_コマンドを読込.Text = "コマンドを読込";
			this.CmsCmd_コマンドを読込.Click += new System.EventHandler(this.CmsCmd_コマンドを読込_Click);
			// 
			// CmsCmd_コマンドを読込_再読込
			// 
			this.CmsCmd_コマンドを読込_再読込.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd_コマンドを読込_再読込.Image")));
			this.CmsCmd_コマンドを読込_再読込.Name = "CmsCmd_コマンドを読込_再読込";
			this.CmsCmd_コマンドを読込_再読込.Size = new System.Drawing.Size(110, 22);
			this.CmsCmd_コマンドを読込_再読込.Text = "再読込";
			this.CmsCmd_コマンドを読込_再読込.Click += new System.EventHandler(this.CmsCmd_コマンドを読込_再読込_Click);
			// 
			// CmsCmd2
			// 
			this.CmsCmd2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsCmd2_閉じる,
            this.CmsCmd2_tss01,
            this.CmsCmd2_タブ,
            this.CmsCmd2_改行,
            this.CmsCmd2_ダブルクォーテーション,
            this.CmsCmd2_セミコロン,
            this.CmsCmd2_tss02,
            this.CmsCmd2_現時間,
            this.CmsCmd2_日付,
            this.CmsCmd2_時間,
            this.CmsCmd2_ミリ秒,
            this.CmsCmd2_年,
            this.CmsCmd2_月,
            this.CmsCmd2_日,
            this.CmsCmd2_時,
            this.CmsCmd2_分,
            this.CmsCmd2_秒,
            this.CmsCmd2_tss03,
            this.CmsCmd2_出力の行データ,
            this.CmsCmd2_出力の行番号,
            this.CmsCmd2_tss04,
            this.CmsCmd2_一時変数});
			this.CmsCmd2.Name = "CmsResult";
			this.CmsCmd2.Size = new System.Drawing.Size(305, 424);
			this.CmsCmd2.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.CmsCmd2_Closing);
			this.CmsCmd2.Opening += new System.ComponentModel.CancelEventHandler(this.CmsCmd2_Opening);
			this.CmsCmd2.Opened += new System.EventHandler(this.CmsCmd2_Opened);
			// 
			// CmsCmd2_閉じる
			// 
			this.CmsCmd2_閉じる.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_閉じる.Image")));
			this.CmsCmd2_閉じる.Name = "CmsCmd2_閉じる";
			this.CmsCmd2_閉じる.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_閉じる.Text = "閉じる";
			this.CmsCmd2_閉じる.Click += new System.EventHandler(this.CmsCmd2_閉じる_Click);
			// 
			// CmsCmd2_tss01
			// 
			this.CmsCmd2_tss01.Name = "CmsCmd2_tss01";
			this.CmsCmd2_tss01.Size = new System.Drawing.Size(301, 6);
			// 
			// CmsCmd2_タブ
			// 
			this.CmsCmd2_タブ.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_タブ.Image")));
			this.CmsCmd2_タブ.Name = "CmsCmd2_タブ";
			this.CmsCmd2_タブ.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_タブ.Text = "タブ( \\t ) #{tab}";
			this.CmsCmd2_タブ.Click += new System.EventHandler(this.CmsCmd2_タブ_Click);
			// 
			// CmsCmd2_改行
			// 
			this.CmsCmd2_改行.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_改行.Image")));
			this.CmsCmd2_改行.Name = "CmsCmd2_改行";
			this.CmsCmd2_改行.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_改行.Text = "改行( \\r\\n ) #{nl}";
			this.CmsCmd2_改行.Click += new System.EventHandler(this.CmsCmd2_改行_Click);
			// 
			// CmsCmd2_ダブルクォーテーション
			// 
			this.CmsCmd2_ダブルクォーテーション.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_ダブルクォーテーション.Image")));
			this.CmsCmd2_ダブルクォーテーション.Name = "CmsCmd2_ダブルクォーテーション";
			this.CmsCmd2_ダブルクォーテーション.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_ダブルクォーテーション.Text = "ダブルクォーテーション( \" ) #{dq}";
			this.CmsCmd2_ダブルクォーテーション.Click += new System.EventHandler(this.CmsCmd2_ダブルクォーテーション_Click);
			// 
			// CmsCmd2_セミコロン
			// 
			this.CmsCmd2_セミコロン.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_セミコロン.Image")));
			this.CmsCmd2_セミコロン.Name = "CmsCmd2_セミコロン";
			this.CmsCmd2_セミコロン.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_セミコロン.Text = "セミコロン( ; ) #{sc}";
			this.CmsCmd2_セミコロン.Click += new System.EventHandler(this.CmsCmd2_セミコロン_Click);
			// 
			// CmsCmd2_tss02
			// 
			this.CmsCmd2_tss02.Name = "CmsCmd2_tss02";
			this.CmsCmd2_tss02.Size = new System.Drawing.Size(301, 6);
			// 
			// CmsCmd2_現時間
			// 
			this.CmsCmd2_現時間.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_現時間.Image")));
			this.CmsCmd2_現時間.Name = "CmsCmd2_現時間";
			this.CmsCmd2_現時間.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_現時間.Text = "現時間 #{now}";
			this.CmsCmd2_現時間.Click += new System.EventHandler(this.CmsCmd2_現時間_Click);
			// 
			// CmsCmd2_日付
			// 
			this.CmsCmd2_日付.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_日付.Image")));
			this.CmsCmd2_日付.Name = "CmsCmd2_日付";
			this.CmsCmd2_日付.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_日付.Text = "日付 #{ymd}";
			this.CmsCmd2_日付.Click += new System.EventHandler(this.CmsCmd2_日付_Click);
			// 
			// CmsCmd2_時間
			// 
			this.CmsCmd2_時間.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_時間.Image")));
			this.CmsCmd2_時間.Name = "CmsCmd2_時間";
			this.CmsCmd2_時間.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_時間.Text = "時間 #{hns}";
			this.CmsCmd2_時間.Click += new System.EventHandler(this.CmsCmd2_時間_Click);
			// 
			// CmsCmd2_ミリ秒
			// 
			this.CmsCmd2_ミリ秒.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_ミリ秒.Image")));
			this.CmsCmd2_ミリ秒.Name = "CmsCmd2_ミリ秒";
			this.CmsCmd2_ミリ秒.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_ミリ秒.Text = "ミリ秒 #{msec}";
			this.CmsCmd2_ミリ秒.Click += new System.EventHandler(this.CmsCmd2_ミリ秒_Click);
			// 
			// CmsCmd2_年
			// 
			this.CmsCmd2_年.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_年.Image")));
			this.CmsCmd2_年.Name = "CmsCmd2_年";
			this.CmsCmd2_年.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_年.Text = "年 #{y}";
			this.CmsCmd2_年.Click += new System.EventHandler(this.CmsCmd2_年_Click);
			// 
			// CmsCmd2_月
			// 
			this.CmsCmd2_月.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_月.Image")));
			this.CmsCmd2_月.Name = "CmsCmd2_月";
			this.CmsCmd2_月.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_月.Text = "月 #{m}";
			this.CmsCmd2_月.Click += new System.EventHandler(this.CmsCmd2_月_Click);
			// 
			// CmsCmd2_日
			// 
			this.CmsCmd2_日.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_日.Image")));
			this.CmsCmd2_日.Name = "CmsCmd2_日";
			this.CmsCmd2_日.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_日.Text = "日 #{d}";
			this.CmsCmd2_日.Click += new System.EventHandler(this.CmsCmd2_日_Click);
			// 
			// CmsCmd2_時
			// 
			this.CmsCmd2_時.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_時.Image")));
			this.CmsCmd2_時.Name = "CmsCmd2_時";
			this.CmsCmd2_時.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_時.Text = "時 #{h}";
			this.CmsCmd2_時.Click += new System.EventHandler(this.CmsCmd2_時_Click);
			// 
			// CmsCmd2_分
			// 
			this.CmsCmd2_分.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_分.Image")));
			this.CmsCmd2_分.Name = "CmsCmd2_分";
			this.CmsCmd2_分.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_分.Text = "分 #{n}";
			this.CmsCmd2_分.Click += new System.EventHandler(this.CmsCmd2_分_Click);
			// 
			// CmsCmd2_秒
			// 
			this.CmsCmd2_秒.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_秒.Image")));
			this.CmsCmd2_秒.Name = "CmsCmd2_秒";
			this.CmsCmd2_秒.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_秒.Text = "秒 #{s}";
			this.CmsCmd2_秒.Click += new System.EventHandler(this.CmsCmd2_秒_Click);
			// 
			// CmsCmd2_tss03
			// 
			this.CmsCmd2_tss03.Name = "CmsCmd2_tss03";
			this.CmsCmd2_tss03.Size = new System.Drawing.Size(301, 6);
			// 
			// CmsCmd2_出力の行データ
			// 
			this.CmsCmd2_出力の行データ.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_出力の行データ.Image")));
			this.CmsCmd2_出力の行データ.Name = "CmsCmd2_出力の行データ";
			this.CmsCmd2_出力の行データ.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_出力の行データ.Text = "出力の行データ #{}";
			this.CmsCmd2_出力の行データ.Click += new System.EventHandler(this.CmsCmd2_出力の行データ_Click);
			// 
			// CmsCmd2_出力の行番号
			// 
			this.CmsCmd2_出力の行番号.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_出力の行番号.Image")));
			this.CmsCmd2_出力の行番号.Name = "CmsCmd2_出力の行番号";
			this.CmsCmd2_出力の行番号.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_出力の行番号.Text = "出力の行番号 #{line,[ゼロ埋め桁数],[加算値]}";
			this.CmsCmd2_出力の行番号.Click += new System.EventHandler(this.CmsCmd2_出力の行番号_Click);
			// 
			// CmsCmd2_tss04
			// 
			this.CmsCmd2_tss04.Name = "CmsCmd2_tss04";
			this.CmsCmd2_tss04.Size = new System.Drawing.Size(301, 6);
			// 
			// CmsCmd2_一時変数
			// 
			this.CmsCmd2_一時変数.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmd2_一時変数.Image")));
			this.CmsCmd2_一時変数.Name = "CmsCmd2_一時変数";
			this.CmsCmd2_一時変数.Size = new System.Drawing.Size(304, 22);
			this.CmsCmd2_一時変数.Text = "一時変数 #{%[キー]}";
			// 
			// CmsCmdLog
			// 
			this.CmsCmdLog.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsCmdLog_クリア,
            this.CmsCmdLog_全コピー,
            this.CmsCmdLog_上書き,
            this.CmsCmdLog_tss01,
            this.CmsCmdLog_貼り付け,
            this.CmsCmdLog_tss02,
            this.CmsCmdLog_拡大,
            this.CmsCmdLog_元に戻す});
			this.CmsCmdLog.Name = "CmsResult";
			this.CmsCmdLog.Size = new System.Drawing.Size(118, 148);
			this.CmsCmdLog.Opened += new System.EventHandler(this.CmsCmdLog_Opened);
			// 
			// CmsCmdLog_クリア
			// 
			this.CmsCmdLog_クリア.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmdLog_クリア.Image")));
			this.CmsCmdLog_クリア.Name = "CmsCmdLog_クリア";
			this.CmsCmdLog_クリア.Size = new System.Drawing.Size(117, 22);
			this.CmsCmdLog_クリア.Text = "クリア";
			this.CmsCmdLog_クリア.Click += new System.EventHandler(this.CmsCmdLog_クリア_Click);
			// 
			// CmsCmdLog_全コピー
			// 
			this.CmsCmdLog_全コピー.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmdLog_全コピー.Image")));
			this.CmsCmdLog_全コピー.Name = "CmsCmdLog_全コピー";
			this.CmsCmdLog_全コピー.Size = new System.Drawing.Size(117, 22);
			this.CmsCmdLog_全コピー.Text = "全コピー";
			this.CmsCmdLog_全コピー.Click += new System.EventHandler(this.CmsCmdLog_全コピー_Click);
			// 
			// CmsCmdLog_上書き
			// 
			this.CmsCmdLog_上書き.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmdLog_上書き.Image")));
			this.CmsCmdLog_上書き.Name = "CmsCmdLog_上書き";
			this.CmsCmdLog_上書き.Size = new System.Drawing.Size(117, 22);
			this.CmsCmdLog_上書き.Text = "上書き";
			this.CmsCmdLog_上書き.Click += new System.EventHandler(this.CmsCmdLog_上書き_Click);
			// 
			// CmsCmdLog_tss01
			// 
			this.CmsCmdLog_tss01.Name = "CmsCmdLog_tss01";
			this.CmsCmdLog_tss01.Size = new System.Drawing.Size(114, 6);
			// 
			// CmsCmdLog_貼り付け
			// 
			this.CmsCmdLog_貼り付け.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmdLog_貼り付け.Image")));
			this.CmsCmdLog_貼り付け.Name = "CmsCmdLog_貼り付け";
			this.CmsCmdLog_貼り付け.Size = new System.Drawing.Size(117, 22);
			this.CmsCmdLog_貼り付け.Text = "貼り付け";
			this.CmsCmdLog_貼り付け.Click += new System.EventHandler(this.CmsCmdLog_貼り付け_Click);
			// 
			// CmsCmdLog_tss02
			// 
			this.CmsCmdLog_tss02.Name = "CmsCmdLog_tss02";
			this.CmsCmdLog_tss02.Size = new System.Drawing.Size(114, 6);
			// 
			// CmsCmdLog_拡大
			// 
			this.CmsCmdLog_拡大.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmdLog_拡大.Image")));
			this.CmsCmdLog_拡大.Name = "CmsCmdLog_拡大";
			this.CmsCmdLog_拡大.Size = new System.Drawing.Size(117, 22);
			this.CmsCmdLog_拡大.Text = "拡大";
			this.CmsCmdLog_拡大.Click += new System.EventHandler(this.CmsCmdLog_拡大_Click);
			// 
			// CmsCmdLog_元に戻す
			// 
			this.CmsCmdLog_元に戻す.Image = ((System.Drawing.Image)(resources.GetObject("CmsCmdLog_元に戻す.Image")));
			this.CmsCmdLog_元に戻す.Name = "CmsCmdLog_元に戻す";
			this.CmsCmdLog_元に戻す.Size = new System.Drawing.Size(117, 22);
			this.CmsCmdLog_元に戻す.Text = "元に戻す";
			this.CmsCmdLog_元に戻す.Click += new System.EventHandler(this.CmsCmdLog_元に戻す_Click);
			// 
			// CmsResult
			// 
			this.CmsResult.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsResult_全選択,
            this.CmsResult_tss01,
            this.CmsResult_クリア,
            this.CmsResult_全コピー,
            this.CmsResult_上書き,
            this.CmsResult_tss02,
            this.CmsResult_貼り付け,
            this.CmsResult_ファイル名を貼り付け,
            this.CmsResult_tss03,
            this.CmsResult_出力へコピー,
            this.CmsResult_tss04,
            this.CmsResult_名前を付けて保存});
			this.CmsResult.Name = "CmsResult";
			this.CmsResult.Size = new System.Drawing.Size(171, 204);
			this.CmsResult.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.CmsResult_Closed);
			this.CmsResult.Opened += new System.EventHandler(this.CmsResult_Opened);
			// 
			// CmsResult_全選択
			// 
			this.CmsResult_全選択.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_全選択.Image")));
			this.CmsResult_全選択.Name = "CmsResult_全選択";
			this.CmsResult_全選択.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_全選択.Text = "全選択";
			this.CmsResult_全選択.Click += new System.EventHandler(this.CmsResult_全選択_Click);
			// 
			// CmsResult_tss01
			// 
			this.CmsResult_tss01.Name = "CmsResult_tss01";
			this.CmsResult_tss01.Size = new System.Drawing.Size(167, 6);
			// 
			// CmsResult_クリア
			// 
			this.CmsResult_クリア.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_クリア.Image")));
			this.CmsResult_クリア.Name = "CmsResult_クリア";
			this.CmsResult_クリア.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_クリア.Text = "クリア";
			this.CmsResult_クリア.Click += new System.EventHandler(this.CmsResult_クリア_Click);
			// 
			// CmsResult_全コピー
			// 
			this.CmsResult_全コピー.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_全コピー.Image")));
			this.CmsResult_全コピー.Name = "CmsResult_全コピー";
			this.CmsResult_全コピー.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_全コピー.Text = "全コピー";
			this.CmsResult_全コピー.Click += new System.EventHandler(this.CmsResult_全コピー_Click);
			// 
			// CmsResult_上書き
			// 
			this.CmsResult_上書き.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_上書き.Image")));
			this.CmsResult_上書き.Name = "CmsResult_上書き";
			this.CmsResult_上書き.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_上書き.Text = "上書き";
			this.CmsResult_上書き.Click += new System.EventHandler(this.CmsResult_上書き_Click);
			// 
			// CmsResult_tss02
			// 
			this.CmsResult_tss02.Name = "CmsResult_tss02";
			this.CmsResult_tss02.Size = new System.Drawing.Size(167, 6);
			// 
			// CmsResult_貼り付け
			// 
			this.CmsResult_貼り付け.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_貼り付け.Image")));
			this.CmsResult_貼り付け.Name = "CmsResult_貼り付け";
			this.CmsResult_貼り付け.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_貼り付け.Text = "貼り付け";
			this.CmsResult_貼り付け.Click += new System.EventHandler(this.CmsResult_貼り付け_Click);
			// 
			// CmsResult_ファイル名を貼り付け
			// 
			this.CmsResult_ファイル名を貼り付け.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_ファイル名を貼り付け.Image")));
			this.CmsResult_ファイル名を貼り付け.Name = "CmsResult_ファイル名を貼り付け";
			this.CmsResult_ファイル名を貼り付け.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_ファイル名を貼り付け.Text = "ファイル名を貼り付け";
			this.CmsResult_ファイル名を貼り付け.Click += new System.EventHandler(this.CmsResult_ファイル名を貼り付け_Click);
			// 
			// CmsResult_tss03
			// 
			this.CmsResult_tss03.Name = "CmsResult_tss03";
			this.CmsResult_tss03.Size = new System.Drawing.Size(167, 6);
			// 
			// CmsResult_出力へコピー
			// 
			this.CmsResult_出力へコピー.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsResult_出力へコピー_1,
            this.CmsResult_出力へコピー_2,
            this.CmsResult_出力へコピー_3,
            this.CmsResult_出力へコピー_4,
            this.CmsResult_出力へコピー_5});
			this.CmsResult_出力へコピー.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_出力へコピー.Image")));
			this.CmsResult_出力へコピー.Name = "CmsResult_出力へコピー";
			this.CmsResult_出力へコピー.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_出力へコピー.Text = "出力へコピー";
			// 
			// CmsResult_出力へコピー_1
			// 
			this.CmsResult_出力へコピー_1.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_出力へコピー_1.Image")));
			this.CmsResult_出力へコピー_1.Name = "CmsResult_出力へコピー_1";
			this.CmsResult_出力へコピー_1.Size = new System.Drawing.Size(80, 22);
			this.CmsResult_出力へコピー_1.Text = "1";
			this.CmsResult_出力へコピー_1.Click += new System.EventHandler(this.CmsResult_出力へコピー_1_Click);
			// 
			// CmsResult_出力へコピー_2
			// 
			this.CmsResult_出力へコピー_2.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_出力へコピー_2.Image")));
			this.CmsResult_出力へコピー_2.Name = "CmsResult_出力へコピー_2";
			this.CmsResult_出力へコピー_2.Size = new System.Drawing.Size(80, 22);
			this.CmsResult_出力へコピー_2.Text = "2";
			this.CmsResult_出力へコピー_2.Click += new System.EventHandler(this.CmsResult_出力へコピー_2_Click);
			// 
			// CmsResult_出力へコピー_3
			// 
			this.CmsResult_出力へコピー_3.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_出力へコピー_3.Image")));
			this.CmsResult_出力へコピー_3.Name = "CmsResult_出力へコピー_3";
			this.CmsResult_出力へコピー_3.Size = new System.Drawing.Size(80, 22);
			this.CmsResult_出力へコピー_3.Text = "3";
			this.CmsResult_出力へコピー_3.Click += new System.EventHandler(this.CmsResult_出力へコピー_3_Click);
			// 
			// CmsResult_出力へコピー_4
			// 
			this.CmsResult_出力へコピー_4.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_出力へコピー_4.Image")));
			this.CmsResult_出力へコピー_4.Name = "CmsResult_出力へコピー_4";
			this.CmsResult_出力へコピー_4.Size = new System.Drawing.Size(80, 22);
			this.CmsResult_出力へコピー_4.Text = "4";
			this.CmsResult_出力へコピー_4.Click += new System.EventHandler(this.CmsResult_出力へコピー_4_Click);
			// 
			// CmsResult_出力へコピー_5
			// 
			this.CmsResult_出力へコピー_5.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_出力へコピー_5.Image")));
			this.CmsResult_出力へコピー_5.Name = "CmsResult_出力へコピー_5";
			this.CmsResult_出力へコピー_5.Size = new System.Drawing.Size(80, 22);
			this.CmsResult_出力へコピー_5.Text = "5";
			this.CmsResult_出力へコピー_5.Click += new System.EventHandler(this.CmsResult_出力へコピー_5_Click);
			// 
			// CmsResult_tss04
			// 
			this.CmsResult_tss04.Name = "CmsResult_tss04";
			this.CmsResult_tss04.Size = new System.Drawing.Size(167, 6);
			// 
			// CmsResult_名前を付けて保存
			// 
			this.CmsResult_名前を付けて保存.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsResult_名前を付けて保存_SJIS,
            this.CmsResult_名前を付けて保存_UTF8N});
			this.CmsResult_名前を付けて保存.ForeColor = System.Drawing.SystemColors.ControlText;
			this.CmsResult_名前を付けて保存.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_名前を付けて保存.Image")));
			this.CmsResult_名前を付けて保存.Name = "CmsResult_名前を付けて保存";
			this.CmsResult_名前を付けて保存.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_名前を付けて保存.Text = "名前を付けて保存";
			// 
			// CmsResult_名前を付けて保存_SJIS
			// 
			this.CmsResult_名前を付けて保存_SJIS.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_名前を付けて保存_SJIS.Image")));
			this.CmsResult_名前を付けて保存_SJIS.Name = "CmsResult_名前を付けて保存_SJIS";
			this.CmsResult_名前を付けて保存_SJIS.Size = new System.Drawing.Size(116, 22);
			this.CmsResult_名前を付けて保存_SJIS.Text = "Shift_JIS";
			this.CmsResult_名前を付けて保存_SJIS.Click += new System.EventHandler(this.CmsResult_名前を付けて保存_SJIS_Click);
			// 
			// CmsResult_名前を付けて保存_UTF8N
			// 
			this.CmsResult_名前を付けて保存_UTF8N.Image = ((System.Drawing.Image)(resources.GetObject("CmsResult_名前を付けて保存_UTF8N.Image")));
			this.CmsResult_名前を付けて保存_UTF8N.Name = "CmsResult_名前を付けて保存_UTF8N";
			this.CmsResult_名前を付けて保存_UTF8N.Size = new System.Drawing.Size(116, 22);
			this.CmsResult_名前を付けて保存_UTF8N.Text = "UTF-8N";
			this.CmsResult_名前を付けて保存_UTF8N.Click += new System.EventHandler(this.CmsResult_名前を付けて保存_UTF8N_Click);
			// 
			// CmsTbCurDir
			// 
			this.CmsTbCurDir.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsTbCurDir_全コピー});
			this.CmsTbCurDir.Name = "CmsResult";
			this.CmsTbCurDir.Size = new System.Drawing.Size(112, 26);
			// 
			// CmsTbCurDir_全コピー
			// 
			this.CmsTbCurDir_全コピー.Image = ((System.Drawing.Image)(resources.GetObject("CmsTbCurDir_全コピー.Image")));
			this.CmsTbCurDir_全コピー.Name = "CmsTbCurDir_全コピー";
			this.CmsTbCurDir_全コピー.Size = new System.Drawing.Size(111, 22);
			this.CmsTbCurDir_全コピー.Text = "全コピー";
			this.CmsTbCurDir_全コピー.Click += new System.EventHandler(this.CmsTbCurDir_全コピー_Click);
			// 
			// CmsTbDgvSearch
			// 
			this.CmsTbDgvSearch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsTbDgvSearch_クリア,
            this.CmsTbDgvSearch_貼り付け});
			this.CmsTbDgvSearch.Name = "CmsResult";
			this.CmsTbDgvSearch.Size = new System.Drawing.Size(116, 48);
			this.CmsTbDgvSearch.Opened += new System.EventHandler(this.CmsTbDgvSearch_Opened);
			// 
			// CmsTbDgvSearch_クリア
			// 
			this.CmsTbDgvSearch_クリア.Image = ((System.Drawing.Image)(resources.GetObject("CmsTbDgvSearch_クリア.Image")));
			this.CmsTbDgvSearch_クリア.Name = "CmsTbDgvSearch_クリア";
			this.CmsTbDgvSearch_クリア.Size = new System.Drawing.Size(115, 22);
			this.CmsTbDgvSearch_クリア.Text = "クリア";
			this.CmsTbDgvSearch_クリア.Click += new System.EventHandler(this.CmsTbDgvSearch_クリア_Click);
			// 
			// CmsTbDgvSearch_貼り付け
			// 
			this.CmsTbDgvSearch_貼り付け.Image = ((System.Drawing.Image)(resources.GetObject("CmsTbDgvSearch_貼り付け.Image")));
			this.CmsTbDgvSearch_貼り付け.Name = "CmsTbDgvSearch_貼り付け";
			this.CmsTbDgvSearch_貼り付け.Size = new System.Drawing.Size(115, 22);
			this.CmsTbDgvSearch_貼り付け.Text = "貼り付け";
			this.CmsTbDgvSearch_貼り付け.Click += new System.EventHandler(this.CmsTbDgvSearch_貼り付け_Click);
			// 
			// CmsTextSelect
			// 
			this.CmsTextSelect.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsTextSelect_Cancel,
            this.CmsTextSelect_tss01,
            this.CmsTextSelect_コピー,
            this.CmsTextSelect_切り取り,
            this.CmsTextSelect_tss02,
            this.CmsTextSelect_貼り付け,
            this.CmsTextSelect_tss03,
            this.CmsTextSelect_DQで囲む,
            this.CmsTextSelect_DQを消去,
            this.CmsTextSelect_tss04,
            this.CmsTextSelect_ネット検索,
            this.CmsTextSelect_tss05,
            this.CmsTextSelect_関連付けられたアプリケーションで開く});
			this.CmsTextSelect.Name = "CmsResult";
			this.CmsTextSelect.Size = new System.Drawing.Size(247, 210);
			this.CmsTextSelect.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.CmsTextSelect_PreviewKeyDown);
			// 
			// CmsTextSelect_Cancel
			// 
			this.CmsTextSelect_Cancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.CmsTextSelect_Cancel.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_Cancel.Image")));
			this.CmsTextSelect_Cancel.Name = "CmsTextSelect_Cancel";
			this.CmsTextSelect_Cancel.Size = new System.Drawing.Size(246, 22);
			this.CmsTextSelect_Cancel.Text = "選択";
			this.CmsTextSelect_Cancel.Click += new System.EventHandler(this.CmsTextSelect_Cancel_Click);
			// 
			// CmsTextSelect_tss01
			// 
			this.CmsTextSelect_tss01.Name = "CmsTextSelect_tss01";
			this.CmsTextSelect_tss01.Size = new System.Drawing.Size(243, 6);
			// 
			// CmsTextSelect_コピー
			// 
			this.CmsTextSelect_コピー.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_コピー.Image")));
			this.CmsTextSelect_コピー.Name = "CmsTextSelect_コピー";
			this.CmsTextSelect_コピー.Size = new System.Drawing.Size(246, 22);
			this.CmsTextSelect_コピー.Text = "コピー";
			this.CmsTextSelect_コピー.Click += new System.EventHandler(this.CmsTextSelect_コピー_Click);
			// 
			// CmsTextSelect_切り取り
			// 
			this.CmsTextSelect_切り取り.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_切り取り.Image")));
			this.CmsTextSelect_切り取り.Name = "CmsTextSelect_切り取り";
			this.CmsTextSelect_切り取り.Size = new System.Drawing.Size(246, 22);
			this.CmsTextSelect_切り取り.Text = "切り取り";
			this.CmsTextSelect_切り取り.Click += new System.EventHandler(this.CmsTextSelect_切り取り_Click);
			// 
			// CmsTextSelect_tss02
			// 
			this.CmsTextSelect_tss02.Name = "CmsTextSelect_tss02";
			this.CmsTextSelect_tss02.Size = new System.Drawing.Size(243, 6);
			// 
			// CmsTextSelect_貼り付け
			// 
			this.CmsTextSelect_貼り付け.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_貼り付け.Image")));
			this.CmsTextSelect_貼り付け.Name = "CmsTextSelect_貼り付け";
			this.CmsTextSelect_貼り付け.Size = new System.Drawing.Size(246, 22);
			this.CmsTextSelect_貼り付け.Text = "貼り付け";
			this.CmsTextSelect_貼り付け.Click += new System.EventHandler(this.CmsTextSelect_貼り付け_Click);
			// 
			// CmsTextSelect_tss03
			// 
			this.CmsTextSelect_tss03.Name = "CmsTextSelect_tss03";
			this.CmsTextSelect_tss03.Size = new System.Drawing.Size(243, 6);
			// 
			// CmsTextSelect_DQで囲む
			// 
			this.CmsTextSelect_DQで囲む.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_DQで囲む.Image")));
			this.CmsTextSelect_DQで囲む.Name = "CmsTextSelect_DQで囲む";
			this.CmsTextSelect_DQで囲む.Size = new System.Drawing.Size(246, 22);
			this.CmsTextSelect_DQで囲む.Text = "選択範囲を \" で囲む";
			this.CmsTextSelect_DQで囲む.Click += new System.EventHandler(this.CmsTextSelect_DQで囲む_Click);
			// 
			// CmsTextSelect_DQを消去
			// 
			this.CmsTextSelect_DQを消去.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_DQを消去.Image")));
			this.CmsTextSelect_DQを消去.Name = "CmsTextSelect_DQを消去";
			this.CmsTextSelect_DQを消去.Size = new System.Drawing.Size(246, 22);
			this.CmsTextSelect_DQを消去.Text = "選択範囲の \" を消去";
			this.CmsTextSelect_DQを消去.Click += new System.EventHandler(this.CmsTextSelect_DQを消去_Click);
			// 
			// CmsTextSelect_tss04
			// 
			this.CmsTextSelect_tss04.Name = "CmsTextSelect_tss04";
			this.CmsTextSelect_tss04.Size = new System.Drawing.Size(243, 6);
			// 
			// CmsTextSelect_ネット検索
			// 
			this.CmsTextSelect_ネット検索.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsTextSelect_ネット検索_URLを開く,
            this.CmsTextSelect_ネット検索_tss01,
            this.CmsTextSelect_ネット検索_Google,
            this.CmsTextSelect_ネット検索_Google翻訳,
            this.CmsTextSelect_ネット検索_Googleマップ,
            this.CmsTextSelect_ネット検索_YouTube,
            this.CmsTextSelect_ネット検索_Wikipedia});
			this.CmsTextSelect_ネット検索.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_ネット検索.Image")));
			this.CmsTextSelect_ネット検索.Name = "CmsTextSelect_ネット検索";
			this.CmsTextSelect_ネット検索.Size = new System.Drawing.Size(246, 22);
			this.CmsTextSelect_ネット検索.Text = "ネット検索";
			// 
			// CmsTextSelect_ネット検索_URLを開く
			// 
			this.CmsTextSelect_ネット検索_URLを開く.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_ネット検索_URLを開く.Image")));
			this.CmsTextSelect_ネット検索_URLを開く.Name = "CmsTextSelect_ネット検索_URLを開く";
			this.CmsTextSelect_ネット検索_URLを開く.Size = new System.Drawing.Size(141, 22);
			this.CmsTextSelect_ネット検索_URLを開く.Text = "URLを開く";
			this.CmsTextSelect_ネット検索_URLを開く.Click += new System.EventHandler(this.CmsTextSelect_ネット検索_URLを開く_Click);
			// 
			// CmsTextSelect_ネット検索_tss01
			// 
			this.CmsTextSelect_ネット検索_tss01.Name = "CmsTextSelect_ネット検索_tss01";
			this.CmsTextSelect_ネット検索_tss01.Size = new System.Drawing.Size(138, 6);
			// 
			// CmsTextSelect_ネット検索_Google
			// 
			this.CmsTextSelect_ネット検索_Google.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_ネット検索_Google.Image")));
			this.CmsTextSelect_ネット検索_Google.Name = "CmsTextSelect_ネット検索_Google";
			this.CmsTextSelect_ネット検索_Google.Size = new System.Drawing.Size(141, 22);
			this.CmsTextSelect_ネット検索_Google.Text = "Google";
			this.CmsTextSelect_ネット検索_Google.Click += new System.EventHandler(this.CmsTextSelect_ネット検索_Google_Click);
			// 
			// CmsTextSelect_ネット検索_Google翻訳
			// 
			this.CmsTextSelect_ネット検索_Google翻訳.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_ネット検索_Google翻訳.Image")));
			this.CmsTextSelect_ネット検索_Google翻訳.Name = "CmsTextSelect_ネット検索_Google翻訳";
			this.CmsTextSelect_ネット検索_Google翻訳.Size = new System.Drawing.Size(141, 22);
			this.CmsTextSelect_ネット検索_Google翻訳.Text = "Google 翻訳";
			this.CmsTextSelect_ネット検索_Google翻訳.Click += new System.EventHandler(this.CmsTextSelect_ネット検索_Google翻訳_Click);
			// 
			// CmsTextSelect_ネット検索_Googleマップ
			// 
			this.CmsTextSelect_ネット検索_Googleマップ.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_ネット検索_Googleマップ.Image")));
			this.CmsTextSelect_ネット検索_Googleマップ.Name = "CmsTextSelect_ネット検索_Googleマップ";
			this.CmsTextSelect_ネット検索_Googleマップ.Size = new System.Drawing.Size(141, 22);
			this.CmsTextSelect_ネット検索_Googleマップ.Text = "Google マップ";
			this.CmsTextSelect_ネット検索_Googleマップ.Click += new System.EventHandler(this.CmsTextSelect_ネット検索_Googleマップ_Click);
			// 
			// CmsTextSelect_ネット検索_YouTube
			// 
			this.CmsTextSelect_ネット検索_YouTube.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_ネット検索_YouTube.Image")));
			this.CmsTextSelect_ネット検索_YouTube.Name = "CmsTextSelect_ネット検索_YouTube";
			this.CmsTextSelect_ネット検索_YouTube.Size = new System.Drawing.Size(141, 22);
			this.CmsTextSelect_ネット検索_YouTube.Text = "YouTube";
			this.CmsTextSelect_ネット検索_YouTube.Click += new System.EventHandler(this.CmsTextSelect_ネット検索_YouTube_Click);
			// 
			// CmsTextSelect_ネット検索_Wikipedia
			// 
			this.CmsTextSelect_ネット検索_Wikipedia.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_ネット検索_Wikipedia.Image")));
			this.CmsTextSelect_ネット検索_Wikipedia.Name = "CmsTextSelect_ネット検索_Wikipedia";
			this.CmsTextSelect_ネット検索_Wikipedia.Size = new System.Drawing.Size(141, 22);
			this.CmsTextSelect_ネット検索_Wikipedia.Text = "Wikipedia";
			this.CmsTextSelect_ネット検索_Wikipedia.Click += new System.EventHandler(this.CmsTextSelect_ネット検索_Wikipedia_Click);
			// 
			// CmsTextSelect_tss05
			// 
			this.CmsTextSelect_tss05.Name = "CmsTextSelect_tss05";
			this.CmsTextSelect_tss05.Size = new System.Drawing.Size(243, 6);
			// 
			// CmsTextSelect_関連付けられたアプリケーションで開く
			// 
			this.CmsTextSelect_関連付けられたアプリケーションで開く.Image = ((System.Drawing.Image)(resources.GetObject("CmsTextSelect_関連付けられたアプリケーションで開く.Image")));
			this.CmsTextSelect_関連付けられたアプリケーションで開く.Name = "CmsTextSelect_関連付けられたアプリケーションで開く";
			this.CmsTextSelect_関連付けられたアプリケーションで開く.Size = new System.Drawing.Size(246, 22);
			this.CmsTextSelect_関連付けられたアプリケーションで開く.Text = "関連付けられたアプリケーションで開く";
			this.CmsTextSelect_関連付けられたアプリケーションで開く.Click += new System.EventHandler(this.CmsTextSelect_関連付けられたアプリケーションで開く_Click);
			// 
			// DgvCmd
			// 
			this.DgvCmd.AllowUserToAddRows = false;
			this.DgvCmd.AllowUserToDeleteRows = false;
			this.DgvCmd.AllowUserToResizeColumns = false;
			this.DgvCmd.AllowUserToResizeRows = false;
			this.DgvCmd.BackgroundColor = System.Drawing.Color.LightGray;
			this.DgvCmd.ColumnHeadersHeight = 20;
			this.DgvCmd.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.DgvCmd.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DgvTb21});
			this.DgvCmd.ContextMenuStrip = this.CmsNull;
			this.DgvCmd.GridColor = System.Drawing.Color.LightGray;
			this.DgvCmd.Location = new System.Drawing.Point(174, 133);
			this.DgvCmd.Margin = new System.Windows.Forms.Padding(0);
			this.DgvCmd.MultiSelect = false;
			this.DgvCmd.Name = "DgvCmd";
			this.DgvCmd.ReadOnly = true;
			this.DgvCmd.RowHeadersVisible = false;
			this.DgvCmd.RowTemplate.Height = 21;
			this.DgvCmd.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.DgvCmd.Size = new System.Drawing.Size(68, 23);
			this.DgvCmd.TabIndex = 0;
			this.DgvCmd.TabStop = false;
			this.DgvCmd.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvCmd_CellEnter);
			this.DgvCmd.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvCmd_CellFormatting);
			this.DgvCmd.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvCmd_CellLeave);
			this.DgvCmd.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvCmd_CellMouseClick);
			this.DgvCmd.Enter += new System.EventHandler(this.DgvCmd_Enter);
			this.DgvCmd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgvCmd_KeyDown);
			this.DgvCmd.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DgvCmd_KeyUp);
			this.DgvCmd.Leave += new System.EventHandler(this.DgvCmd_Leave);
			// 
			// DgvTb21
			// 
			this.DgvTb21.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.DgvTb21.ContextMenuStrip = this.CmsNull;
			this.DgvTb21.HeaderText = "コマンド";
			this.DgvTb21.MinimumWidth = 450;
			this.DgvTb21.Name = "DgvTb21";
			this.DgvTb21.ReadOnly = true;
			this.DgvTb21.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.DgvTb21.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.DgvTb21.Width = 450;
			// 
			// DgvMacro
			// 
			this.DgvMacro.AllowUserToAddRows = false;
			this.DgvMacro.AllowUserToDeleteRows = false;
			this.DgvMacro.AllowUserToResizeColumns = false;
			this.DgvMacro.AllowUserToResizeRows = false;
			this.DgvMacro.BackgroundColor = System.Drawing.Color.LightGray;
			this.DgvMacro.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.DgvMacro.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DgvTb11,
            this.DgvTb12});
			this.DgvMacro.ContextMenuStrip = this.CmsNull;
			this.DgvMacro.GridColor = System.Drawing.Color.LightGray;
			this.DgvMacro.Location = new System.Drawing.Point(87, 133);
			this.DgvMacro.Margin = new System.Windows.Forms.Padding(0);
			this.DgvMacro.MultiSelect = false;
			this.DgvMacro.Name = "DgvMacro";
			this.DgvMacro.ReadOnly = true;
			this.DgvMacro.RowHeadersVisible = false;
			this.DgvMacro.RowTemplate.Height = 21;
			this.DgvMacro.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.DgvMacro.Size = new System.Drawing.Size(68, 23);
			this.DgvMacro.StandardTab = true;
			this.DgvMacro.TabIndex = 0;
			this.DgvMacro.TabStop = false;
			this.DgvMacro.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvMacro_CellEnter);
			this.DgvMacro.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvMacro_CellFormatting);
			this.DgvMacro.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvMacro_CellLeave);
			this.DgvMacro.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvMacro_CellMouseClick);
			this.DgvMacro.Enter += new System.EventHandler(this.DgvMacro_Enter);
			this.DgvMacro.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgvMacro_KeyDown);
			this.DgvMacro.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DgvMacro_KeyUp);
			this.DgvMacro.Leave += new System.EventHandler(this.DgvMacro_Leave);
			// 
			// DgvTb11
			// 
			this.DgvTb11.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.DgvTb11.ContextMenuStrip = this.CmsNull;
			this.DgvTb11.FillWeight = 150F;
			this.DgvTb11.HeaderText = "マクロ";
			this.DgvTb11.MinimumWidth = 85;
			this.DgvTb11.Name = "DgvTb11";
			this.DgvTb11.ReadOnly = true;
			this.DgvTb11.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.DgvTb11.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.DgvTb11.Width = 85;
			// 
			// DgvTb12
			// 
			this.DgvTb12.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.DgvTb12.ContextMenuStrip = this.CmsNull;
			this.DgvTb12.FillWeight = 150F;
			this.DgvTb12.HeaderText = "説明";
			this.DgvTb12.MinimumWidth = 700;
			this.DgvTb12.Name = "DgvTb12";
			this.DgvTb12.ReadOnly = true;
			this.DgvTb12.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.DgvTb12.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.DgvTb12.Width = 700;
			// 
			// Lbl_F1
			// 
			this.Lbl_F1.AutoSize = true;
			this.Lbl_F1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F1.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F1.ForeColor = System.Drawing.Color.White;
			this.Lbl_F1.Location = new System.Drawing.Point(10, 125);
			this.Lbl_F1.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F1.Name = "Lbl_F1";
			this.Lbl_F1.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F1.TabIndex = 0;
			this.Lbl_F1.Text = "F1";
			// 
			// Lbl_F2
			// 
			this.Lbl_F2.AutoSize = true;
			this.Lbl_F2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F2.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F2.ForeColor = System.Drawing.Color.White;
			this.Lbl_F2.Location = new System.Drawing.Point(87, 125);
			this.Lbl_F2.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F2.Name = "Lbl_F2";
			this.Lbl_F2.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F2.TabIndex = 0;
			this.Lbl_F2.Text = "F2";
			// 
			// Lbl_F3
			// 
			this.Lbl_F3.AutoSize = true;
			this.Lbl_F3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F3.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F3.ForeColor = System.Drawing.Color.White;
			this.Lbl_F3.Location = new System.Drawing.Point(174, 125);
			this.Lbl_F3.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F3.Name = "Lbl_F3";
			this.Lbl_F3.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F3.TabIndex = 0;
			this.Lbl_F3.Text = "F3";
			// 
			// Lbl_F5
			// 
			this.Lbl_F5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Lbl_F5.AutoSize = true;
			this.Lbl_F5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F5.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F5.ForeColor = System.Drawing.Color.White;
			this.Lbl_F5.Location = new System.Drawing.Point(536, 125);
			this.Lbl_F5.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F5.Name = "Lbl_F5";
			this.Lbl_F5.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F5.TabIndex = 0;
			this.Lbl_F5.Text = "F5";
			// 
			// Lbl_F6
			// 
			this.Lbl_F6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Lbl_F6.AutoSize = true;
			this.Lbl_F6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F6.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F6.ForeColor = System.Drawing.Color.White;
			this.Lbl_F6.Location = new System.Drawing.Point(565, 125);
			this.Lbl_F6.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F6.Name = "Lbl_F6";
			this.Lbl_F6.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F6.TabIndex = 0;
			this.Lbl_F6.Text = "F6";
			// 
			// Lbl_F7
			// 
			this.Lbl_F7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Lbl_F7.AutoSize = true;
			this.Lbl_F7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F7.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F7.ForeColor = System.Drawing.Color.White;
			this.Lbl_F7.Location = new System.Drawing.Point(594, 125);
			this.Lbl_F7.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F7.Name = "Lbl_F7";
			this.Lbl_F7.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F7.TabIndex = 0;
			this.Lbl_F7.Text = "F7";
			// 
			// Lbl_F8
			// 
			this.Lbl_F8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.Lbl_F8.AutoSize = true;
			this.Lbl_F8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F8.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F8.ForeColor = System.Drawing.Color.White;
			this.Lbl_F8.Location = new System.Drawing.Point(10, 526);
			this.Lbl_F8.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F8.Name = "Lbl_F8";
			this.Lbl_F8.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F8.TabIndex = 0;
			this.Lbl_F8.Text = "F8";
			// 
			// LblCmd
			// 
			this.LblCmd.AutoSize = true;
			this.LblCmd.BackColor = System.Drawing.Color.DimGray;
			this.LblCmd.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.LblCmd.ForeColor = System.Drawing.Color.Red;
			this.LblCmd.Location = new System.Drawing.Point(-1, 23);
			this.LblCmd.Margin = new System.Windows.Forms.Padding(0);
			this.LblCmd.Name = "LblCmd";
			this.LblCmd.Size = new System.Drawing.Size(17, 11);
			this.LblCmd.TabIndex = 0;
			this.LblCmd.Text = "●";
			this.LblCmd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// LblCmdLog
			// 
			this.LblCmdLog.AutoSize = true;
			this.LblCmdLog.BackColor = System.Drawing.Color.DimGray;
			this.LblCmdLog.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.LblCmdLog.ForeColor = System.Drawing.Color.Red;
			this.LblCmdLog.Location = new System.Drawing.Point(-1, 76);
			this.LblCmdLog.Margin = new System.Windows.Forms.Padding(0);
			this.LblCmdLog.Name = "LblCmdLog";
			this.LblCmdLog.Size = new System.Drawing.Size(17, 11);
			this.LblCmdLog.TabIndex = 0;
			this.LblCmdLog.Text = "●";
			this.LblCmdLog.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// LblCurDir
			// 
			this.LblCurDir.AutoSize = true;
			this.LblCurDir.BackColor = System.Drawing.Color.DimGray;
			this.LblCurDir.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.LblCurDir.ForeColor = System.Drawing.Color.Red;
			this.LblCurDir.Location = new System.Drawing.Point(-1, 6);
			this.LblCurDir.Margin = new System.Windows.Forms.Padding(0);
			this.LblCurDir.Name = "LblCurDir";
			this.LblCurDir.Size = new System.Drawing.Size(17, 11);
			this.LblCurDir.TabIndex = 4;
			this.LblCurDir.Text = "●";
			this.LblCurDir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// LblFontSize
			// 
			this.LblFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.LblFontSize.AutoSize = true;
			this.LblFontSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.LblFontSize.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.LblFontSize.ForeColor = System.Drawing.Color.White;
			this.LblFontSize.Location = new System.Drawing.Point(605, 536);
			this.LblFontSize.Margin = new System.Windows.Forms.Padding(0);
			this.LblFontSize.Name = "LblFontSize";
			this.LblFontSize.Size = new System.Drawing.Size(20, 15);
			this.LblFontSize.TabIndex = 0;
			this.LblFontSize.Text = "pt";
			this.LblFontSize.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// LblResult
			// 
			this.LblResult.AutoSize = true;
			this.LblResult.BackColor = System.Drawing.Color.DimGray;
			this.LblResult.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.LblResult.ForeColor = System.Drawing.Color.Red;
			this.LblResult.Location = new System.Drawing.Point(-1, 163);
			this.LblResult.Margin = new System.Windows.Forms.Padding(0);
			this.LblResult.Name = "LblResult";
			this.LblResult.Size = new System.Drawing.Size(17, 11);
			this.LblResult.TabIndex = 0;
			this.LblResult.Text = "●";
			this.LblResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// LblWait
			// 
			this.LblWait.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.LblWait.AutoSize = true;
			this.LblWait.BackColor = System.Drawing.Color.Crimson;
			this.LblWait.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LblWait.Font = new System.Drawing.Font("メイリオ", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.LblWait.ForeColor = System.Drawing.Color.White;
			this.LblWait.Location = new System.Drawing.Point(262, 21);
			this.LblWait.Margin = new System.Windows.Forms.Padding(0);
			this.LblWait.Name = "LblWait";
			this.LblWait.Padding = new System.Windows.Forms.Padding(20, 0, 10, 0);
			this.LblWait.Size = new System.Drawing.Size(100, 23);
			this.LblWait.TabIndex = 0;
			this.LblWait.Text = "実行中...";
			this.LblWait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.LblWait.Visible = false;
			// 
			// NudFontSize
			// 
			this.NudFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.NudFontSize.BackColor = System.Drawing.Color.DimGray;
			this.NudFontSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.NudFontSize.ContextMenuStrip = this.CmsNull;
			this.NudFontSize.Cursor = System.Windows.Forms.Cursors.Default;
			this.NudFontSize.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.NudFontSize.ForeColor = System.Drawing.Color.White;
			this.NudFontSize.Location = new System.Drawing.Point(571, 536);
			this.NudFontSize.Margin = new System.Windows.Forms.Padding(0);
			this.NudFontSize.Maximum = new decimal(new int[] {
            22,
            0,
            0,
            0});
			this.NudFontSize.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.NudFontSize.Name = "NudFontSize";
			this.NudFontSize.Size = new System.Drawing.Size(36, 19);
			this.NudFontSize.TabIndex = 0;
			this.NudFontSize.TabStop = false;
			this.NudFontSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.NudFontSize.ValueChanged += new System.EventHandler(this.NudFontSize_ValueChanged);
			this.NudFontSize.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NudFontSize_KeyUp);
			// 
			// RtbCmdLog
			// 
			this.RtbCmdLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.RtbCmdLog.BackColor = System.Drawing.Color.Black;
			this.RtbCmdLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.RtbCmdLog.ContextMenuStrip = this.CmsCmdLog;
			this.RtbCmdLog.DetectUrls = false;
			this.RtbCmdLog.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.RtbCmdLog.ForeColor = System.Drawing.Color.Yellow;
			this.RtbCmdLog.Location = new System.Drawing.Point(9, 72);
			this.RtbCmdLog.Margin = new System.Windows.Forms.Padding(0);
			this.RtbCmdLog.Name = "RtbCmdLog";
			this.RtbCmdLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			this.RtbCmdLog.Size = new System.Drawing.Size(610, 50);
			this.RtbCmdLog.TabIndex = 0;
			this.RtbCmdLog.TabStop = false;
			this.RtbCmdLog.Text = "";
			this.RtbCmdLog.WordWrap = false;
			this.RtbCmdLog.Enter += new System.EventHandler(this.RtbCmdLog_Enter);
			this.RtbCmdLog.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RtbCmdLog_KeyUp);
			this.RtbCmdLog.Leave += new System.EventHandler(this.RtbCmdLog_Leave);
			this.RtbCmdLog.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RtbCmdLog_MouseUp);
			// 
			// ScrTbResult
			// 
			this.ScrTbResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ScrTbResult.BackColor = System.Drawing.Color.Black;
			this.ScrTbResult.ForeColor = System.Drawing.Color.White;
			this.ScrTbResult.IsSplitterFixed = true;
			this.ScrTbResult.Location = new System.Drawing.Point(9, 160);
			this.ScrTbResult.Margin = new System.Windows.Forms.Padding(0);
			this.ScrTbResult.Name = "ScrTbResult";
			// 
			// ScrTbResult.Panel1
			// 
			this.ScrTbResult.Panel1.AllowDrop = true;
			this.ScrTbResult.Panel1.BackColor = System.Drawing.Color.Black;
			this.ScrTbResult.Panel1.Controls.Add(this.BtnPasteFilename);
			this.ScrTbResult.Panel1.ForeColor = System.Drawing.Color.White;
			this.ScrTbResult.Panel1.Click += new System.EventHandler(this.ScrTbResult_Panel1_Click);
			this.ScrTbResult.Panel1.DragLeave += new System.EventHandler(this.ScrTbResult_Panel1_DragLeave);
			// 
			// ScrTbResult.Panel2
			// 
			this.ScrTbResult.Panel2.AllowDrop = true;
			this.ScrTbResult.Panel2.BackColor = System.Drawing.Color.Black;
			this.ScrTbResult.Panel2.Controls.Add(this.BtnPasteTextfile);
			this.ScrTbResult.Panel2.ForeColor = System.Drawing.Color.White;
			this.ScrTbResult.Panel2.Click += new System.EventHandler(this.ScrTbResult_Panel2_Click);
			this.ScrTbResult.Panel2.DragLeave += new System.EventHandler(this.ScrTbResult_Panel2_DragLeave);
			this.ScrTbResult.Size = new System.Drawing.Size(610, 346);
			this.ScrTbResult.SplitterDistance = 303;
			this.ScrTbResult.TabIndex = 0;
			this.ScrTbResult.TabStop = false;
			// 
			// TbCmd
			// 
			this.TbCmd.AllowDrop = true;
			this.TbCmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TbCmd.BackColor = System.Drawing.Color.White;
			this.TbCmd.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.TbCmd.ContextMenuStrip = this.CmsCmd;
			this.TbCmd.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.TbCmd.ForeColor = System.Drawing.Color.Black;
			this.TbCmd.Location = new System.Drawing.Point(9, 21);
			this.TbCmd.Margin = new System.Windows.Forms.Padding(0);
			this.TbCmd.Multiline = true;
			this.TbCmd.Name = "TbCmd";
			this.TbCmd.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.TbCmd.Size = new System.Drawing.Size(610, 50);
			this.TbCmd.TabIndex = 0;
			this.TbCmd.WordWrap = false;
			this.TbCmd.TextChanged += new System.EventHandler(this.TbCmd_TextChanged);
			this.TbCmd.DragDrop += new System.Windows.Forms.DragEventHandler(this.TbCmd_DragDrop);
			this.TbCmd.DragEnter += new System.Windows.Forms.DragEventHandler(this.TbCmd_DragEnter);
			this.TbCmd.Enter += new System.EventHandler(this.TbCmd_Enter);
			this.TbCmd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbCmd_KeyDown);
			this.TbCmd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbCmd_KeyPress);
			this.TbCmd.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbCmd_KeyUp);
			this.TbCmd.Leave += new System.EventHandler(this.TbCmd_Leave);
			this.TbCmd.MouseEnter += new System.EventHandler(this.TbCmd_MouseEnter);
			this.TbCmd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TbCmd_MouseUp);
			// 
			// TbCurDir
			// 
			this.TbCurDir.AllowDrop = true;
			this.TbCurDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TbCurDir.BackColor = System.Drawing.Color.DimGray;
			this.TbCurDir.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.TbCurDir.ContextMenuStrip = this.CmsTbCurDir;
			this.TbCurDir.Cursor = System.Windows.Forms.Cursors.Default;
			this.TbCurDir.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.TbCurDir.ForeColor = System.Drawing.Color.White;
			this.TbCurDir.Location = new System.Drawing.Point(9, 3);
			this.TbCurDir.Margin = new System.Windows.Forms.Padding(0);
			this.TbCurDir.Name = "TbCurDir";
			this.TbCurDir.ReadOnly = true;
			this.TbCurDir.Size = new System.Drawing.Size(595, 13);
			this.TbCurDir.TabIndex = 0;
			this.TbCurDir.TabStop = false;
			this.TbCurDir.Text = "TbCurDir";
			this.TbCurDir.WordWrap = false;
			this.TbCurDir.Click += new System.EventHandler(this.TbCurDir_Click);
			this.TbCurDir.DragDrop += new System.Windows.Forms.DragEventHandler(this.TbCurDir_DragDrop);
			this.TbCurDir.DragEnter += new System.Windows.Forms.DragEventHandler(this.TbCurDir_DragEnter);
			this.TbCurDir.MouseEnter += new System.EventHandler(this.TbCurDir_MouseEnter);
			this.TbCurDir.MouseLeave += new System.EventHandler(this.TbCurDir_MouseLeave);
			// 
			// TbDgvSearch
			// 
			this.TbDgvSearch.BackColor = System.Drawing.Color.LightYellow;
			this.TbDgvSearch.ContextMenuStrip = this.CmsTbDgvSearch;
			this.TbDgvSearch.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.TbDgvSearch.ForeColor = System.Drawing.Color.Black;
			this.TbDgvSearch.Location = new System.Drawing.Point(260, 134);
			this.TbDgvSearch.Margin = new System.Windows.Forms.Padding(0);
			this.TbDgvSearch.Name = "TbDgvSearch";
			this.TbDgvSearch.Size = new System.Drawing.Size(100, 19);
			this.TbDgvSearch.TabIndex = 0;
			this.TbDgvSearch.TabStop = false;
			this.ToolTip.SetToolTip(this.TbDgvSearch, "部分一致による検索");
			this.TbDgvSearch.WordWrap = false;
			this.TbDgvSearch.TextChanged += new System.EventHandler(this.TbDgvSearch_TextChanged);
			this.TbDgvSearch.Enter += new System.EventHandler(this.TbDgvSearch_Enter);
			this.TbDgvSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbDgvSearch_KeyPress);
			this.TbDgvSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbDgvSearch_KeyUp);
			this.TbDgvSearch.Leave += new System.EventHandler(this.TbDgvSearch_Leave);
			// 
			// TbInfo
			// 
			this.TbInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TbInfo.BackColor = System.Drawing.Color.DimGray;
			this.TbInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.TbInfo.ContextMenuStrip = this.CmsNull;
			this.TbInfo.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.TbInfo.ForeColor = System.Drawing.Color.Gainsboro;
			this.TbInfo.Location = new System.Drawing.Point(105, 545);
			this.TbInfo.Margin = new System.Windows.Forms.Padding(0);
			this.TbInfo.Name = "TbInfo";
			this.TbInfo.ReadOnly = true;
			this.TbInfo.Size = new System.Drawing.Size(415, 12);
			this.TbInfo.TabIndex = 0;
			this.TbInfo.TabStop = false;
			this.TbInfo.Text = "TbInfo";
			this.TbInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TbInfo.WordWrap = false;
			// 
			// TbResult
			// 
			this.TbResult.AcceptsTab = true;
			this.TbResult.AllowDrop = true;
			this.TbResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TbResult.BackColor = System.Drawing.Color.Black;
			this.TbResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.TbResult.ContextMenuStrip = this.CmsResult;
			this.TbResult.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.TbResult.ForeColor = System.Drawing.Color.Lime;
			this.TbResult.Location = new System.Drawing.Point(9, 160);
			this.TbResult.Margin = new System.Windows.Forms.Padding(0);
			this.TbResult.MaxLength = 2147483647;
			this.TbResult.Multiline = true;
			this.TbResult.Name = "TbResult";
			this.TbResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TbResult.Size = new System.Drawing.Size(610, 363);
			this.TbResult.TabIndex = 0;
			this.TbResult.TabStop = false;
			this.TbResult.WordWrap = false;
			this.TbResult.TextChanged += new System.EventHandler(this.TbResult_TextChanged);
			this.TbResult.DragEnter += new System.Windows.Forms.DragEventHandler(this.TbResult_DragEnter);
			this.TbResult.Enter += new System.EventHandler(this.TbResult_Enter);
			this.TbResult.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbResult_KeyDown);
			this.TbResult.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbResult_KeyPress);
			this.TbResult.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbResult_KeyUp);
			this.TbResult.Leave += new System.EventHandler(this.TbResult_Leave);
			this.TbResult.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TbResult_MouseUp);
			// 
			// ToolTip
			// 
			this.ToolTip.AutoPopDelay = 6000;
			this.ToolTip.BackColor = System.Drawing.Color.Ivory;
			this.ToolTip.ForeColor = System.Drawing.Color.Black;
			this.ToolTip.InitialDelay = 500;
			this.ToolTip.ReshowDelay = 100;
			// 
			// ChkTopMost
			// 
			this.ChkTopMost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ChkTopMost.AutoSize = true;
			this.ChkTopMost.BackColor = System.Drawing.Color.DimGray;
			this.ChkTopMost.FlatAppearance.BorderSize = 0;
			this.ChkTopMost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ChkTopMost.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.ChkTopMost.Location = new System.Drawing.Point(608, 5);
			this.ChkTopMost.Margin = new System.Windows.Forms.Padding(0);
			this.ChkTopMost.Name = "ChkTopMost";
			this.ChkTopMost.Size = new System.Drawing.Size(12, 11);
			this.ChkTopMost.TabIndex = 0;
			this.ChkTopMost.TabStop = false;
			this.ToolTip.SetToolTip(this.ChkTopMost, "最前面に表示");
			this.ChkTopMost.UseVisualStyleBackColor = false;
			this.ChkTopMost.Click += new System.EventHandler(this.ChkTopMost_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.DimGray;
			this.ClientSize = new System.Drawing.Size(624, 561);
			this.Controls.Add(this.ChkTopMost);
			this.Controls.Add(this.TbCurDir);
			this.Controls.Add(this.LblCurDir);
			this.Controls.Add(this.LblWait);
			this.Controls.Add(this.BtnCmdExecStream);
			this.Controls.Add(this.TbCmd);
			this.Controls.Add(this.LblCmd);
			this.Controls.Add(this.RtbCmdLog);
			this.Controls.Add(this.LblCmdLog);
			this.Controls.Add(this.CbCmdHistory);
			this.Controls.Add(this.TbDgvSearch);
			this.Controls.Add(this.BtnDgvSearch);
			this.Controls.Add(this.BtnDgvMacro);
			this.Controls.Add(this.DgvMacro);
			this.Controls.Add(this.BtnDgvCmd);
			this.Controls.Add(this.DgvCmd);
			this.Controls.Add(this.BtnCmdExec);
			this.Controls.Add(this.BtnCmdExecUndo);
			this.Controls.Add(this.BtnAllClear);
			this.Controls.Add(this.Lbl_F1);
			this.Controls.Add(this.Lbl_F2);
			this.Controls.Add(this.Lbl_F3);
			this.Controls.Add(this.Lbl_F5);
			this.Controls.Add(this.Lbl_F6);
			this.Controls.Add(this.Lbl_F7);
			this.Controls.Add(this.ScrTbResult);
			this.Controls.Add(this.TbResult);
			this.Controls.Add(this.LblResult);
			this.Controls.Add(this.BtnResult1);
			this.Controls.Add(this.BtnResult2);
			this.Controls.Add(this.BtnResult3);
			this.Controls.Add(this.BtnResult4);
			this.Controls.Add(this.BtnResult5);
			this.Controls.Add(this.CbResultHistory);
			this.Controls.Add(this.Lbl_F8);
			this.Controls.Add(this.TbInfo);
			this.Controls.Add(this.NudFontSize);
			this.Controls.Add(this.LblFontSize);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(520, 400);
			this.Name = "Form1";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Resize += new System.EventHandler(this.Form1_Resize);
			this.CmsCmd.ResumeLayout(false);
			this.CmsCmd2.ResumeLayout(false);
			this.CmsCmdLog.ResumeLayout(false);
			this.CmsResult.ResumeLayout(false);
			this.CmsTbCurDir.ResumeLayout(false);
			this.CmsTbDgvSearch.ResumeLayout(false);
			this.CmsTextSelect.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.DgvCmd)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.DgvMacro)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NudFontSize)).EndInit();
			this.ScrTbResult.Panel1.ResumeLayout(false);
			this.ScrTbResult.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ScrTbResult)).EndInit();
			this.ScrTbResult.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button BtnAllClear;
		private System.Windows.Forms.Button BtnCmdExec;
		private System.Windows.Forms.Button BtnCmdExecStream;
		private System.Windows.Forms.Button BtnCmdExecUndo;
		private System.Windows.Forms.Button BtnDgvCmd;
		private System.Windows.Forms.Button BtnDgvSearch;
		private System.Windows.Forms.Button BtnDgvMacro;
		private System.Windows.Forms.Button BtnPasteFilename;
		private System.Windows.Forms.Button BtnPasteTextfile;
		private System.Windows.Forms.Button BtnResult1;
		private System.Windows.Forms.Button BtnResult2;
		private System.Windows.Forms.Button BtnResult3;
		private System.Windows.Forms.Button BtnResult4;
		private System.Windows.Forms.Button BtnResult5;
		private System.Windows.Forms.ComboBox CbCmdHistory;
		private System.Windows.Forms.ComboBox CbResultHistory;
		private System.Windows.Forms.ContextMenuStrip CmsCmd;
		private System.Windows.Forms.ContextMenuStrip CmsCmd2;
		private System.Windows.Forms.ContextMenuStrip CmsCmdLog;
		private System.Windows.Forms.ContextMenuStrip CmsNull;
		private System.Windows.Forms.ContextMenuStrip CmsResult;
		private System.Windows.Forms.ContextMenuStrip CmsTbCurDir;
		private System.Windows.Forms.ContextMenuStrip CmsTbDgvSearch;
		private System.Windows.Forms.ContextMenuStrip CmsTextSelect;
		private System.Windows.Forms.DataGridView DgvCmd;
		private System.Windows.Forms.DataGridView DgvMacro;
		private System.Windows.Forms.DataGridViewTextBoxColumn DgvTb11;
		private System.Windows.Forms.DataGridViewTextBoxColumn DgvTb12;
		private System.Windows.Forms.DataGridViewTextBoxColumn DgvTb21;
		private System.Windows.Forms.Label Lbl_F1;
		private System.Windows.Forms.Label Lbl_F2;
		private System.Windows.Forms.Label Lbl_F3;
		private System.Windows.Forms.Label Lbl_F5;
		private System.Windows.Forms.Label Lbl_F6;
		private System.Windows.Forms.Label Lbl_F7;
		private System.Windows.Forms.Label Lbl_F8;
		private System.Windows.Forms.Label LblCmd;
		private System.Windows.Forms.Label LblCmdLog;
		private System.Windows.Forms.Label LblCurDir;
		private System.Windows.Forms.Label LblFontSize;
		private System.Windows.Forms.Label LblResult;
		private System.Windows.Forms.Label LblWait;
		private System.Windows.Forms.NumericUpDown NudFontSize;
		private System.Windows.Forms.RichTextBox RtbCmdLog;
		private System.Windows.Forms.SplitContainer ScrTbResult;
		private System.Windows.Forms.TextBox TbCmd;
		private System.Windows.Forms.TextBox TbCurDir;
		private System.Windows.Forms.TextBox TbDgvSearch;
		private System.Windows.Forms.TextBox TbInfo;
		private System.Windows.Forms.TextBox TbResult;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_クリア;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドをグループ化;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドをグループ化_クリア;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドをグループ化_出力;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドをグループ化_追加;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドを読込;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドを読込_再読込;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドを保存;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_ファイル選択;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_フォルダ選択;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_マクロ変数;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_上書き;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_全コピー;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_貼り付け;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_セミコロン;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_タブ;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_ダブルクォーテーション;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_ミリ秒;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_一時変数;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_改行;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_月;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_現時間;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_時;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_時間;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_出力の行データ;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_出力の行番号;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_日;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_日付;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_年;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_秒;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_分;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd2_閉じる;
		private System.Windows.Forms.ToolStripMenuItem CmsCmdLog_クリア;
		private System.Windows.Forms.ToolStripMenuItem CmsCmdLog_拡大;
		private System.Windows.Forms.ToolStripMenuItem CmsCmdLog_元に戻す;
		private System.Windows.Forms.ToolStripMenuItem CmsCmdLog_上書き;
		private System.Windows.Forms.ToolStripMenuItem CmsCmdLog_全コピー;
		private System.Windows.Forms.ToolStripMenuItem CmsCmdLog_貼り付け;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_クリア;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_ファイル名を貼り付け;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_出力へコピー;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_出力へコピー_1;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_出力へコピー_2;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_出力へコピー_3;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_出力へコピー_4;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_出力へコピー_5;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_上書き;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_全コピー;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_全選択;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_貼り付け;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_名前を付けて保存;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_名前を付けて保存_SJIS;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_名前を付けて保存_UTF8N;
		private System.Windows.Forms.ToolStripMenuItem CmsTbCurDir_全コピー;
		private System.Windows.Forms.ToolStripMenuItem CmsTbDgvSearch_クリア;
		private System.Windows.Forms.ToolStripMenuItem CmsTbDgvSearch_貼り付け;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_Cancel;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_DQで囲む;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_DQを消去;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_コピー;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_ネット検索;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_ネット検索_Google;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_ネット検索_Googleマップ;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_ネット検索_Google翻訳;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_ネット検索_URLを開く;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_ネット検索_Wikipedia;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_ネット検索_YouTube;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_関連付けられたアプリケーションで開く;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_切り取り;
		private System.Windows.Forms.ToolStripMenuItem CmsTextSelect_貼り付け;
		private System.Windows.Forms.ToolStripSeparator CmsCmd_tss01;
		private System.Windows.Forms.ToolStripSeparator CmsCmd_tss02;
		private System.Windows.Forms.ToolStripSeparator CmsCmd_tss03;
		private System.Windows.Forms.ToolStripSeparator CmsCmd2_tss01;
		private System.Windows.Forms.ToolStripSeparator CmsCmd2_tss02;
		private System.Windows.Forms.ToolStripSeparator CmsCmd2_tss03;
		private System.Windows.Forms.ToolStripSeparator CmsCmd2_tss04;
		private System.Windows.Forms.ToolStripSeparator CmsCmdLog_tss01;
		private System.Windows.Forms.ToolStripSeparator CmsCmdLog_tss02;
		private System.Windows.Forms.ToolStripSeparator CmsResult_tss01;
		private System.Windows.Forms.ToolStripSeparator CmsResult_tss02;
		private System.Windows.Forms.ToolStripSeparator CmsResult_tss03;
		private System.Windows.Forms.ToolStripSeparator CmsResult_tss04;
		private System.Windows.Forms.ToolStripSeparator CmsTextSelect_tss01;
		private System.Windows.Forms.ToolStripSeparator CmsTextSelect_tss02;
		private System.Windows.Forms.ToolStripSeparator CmsTextSelect_tss03;
		private System.Windows.Forms.ToolStripSeparator CmsTextSelect_tss04;
		private System.Windows.Forms.ToolStripSeparator CmsTextSelect_tss05;
		private System.Windows.Forms.ToolStripSeparator CmsTextSelect_ネット検索_tss01;
		private System.Windows.Forms.ToolTip ToolTip;
		private System.Windows.Forms.CheckBox ChkTopMost;
	}
}

