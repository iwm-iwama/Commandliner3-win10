namespace iwm_commandliner3
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
			this.DgvMacro = new System.Windows.Forms.DataGridView();
			this.Dgv_Tbc21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Dgv_Tbc22 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.CmsResult = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CmsResult_上へ = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_下へ = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsResult_全選択 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_全クリア = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_全コピー = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_L3 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsResult_コピー = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_切り取り = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_貼り付け = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_ファイル名を貼り付け = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsResult_L4 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsResult_名前を付けて保存 = new System.Windows.Forms.ToolStripMenuItem();
			this.TbCmd = new System.Windows.Forms.TextBox();
			this.CmsCmd = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CmsCmd_左へ = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_右へ = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd_全クリア = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_全コピー = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd_コピー = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_切り取り = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_貼り付け = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd_DQで囲む = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd_コマンドをグループ化 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_コマンドをグループ化_追加 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_コマンドをグループ化_出力 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_コマンドをグループ化_消去 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd_コマンドをグループ化_簡単な説明 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_コマンドを保存 = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmd_コマンドを読込 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmd_キー操作 = new System.Windows.Forms.ToolStripMenuItem();
			this.TbCurDir = new System.Windows.Forms.TextBox();
			this.CmsNull = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.BtnCmdExec = new System.Windows.Forms.Button();
			this.BtnResultMem = new System.Windows.Forms.Button();
			this.CbTextCode = new System.Windows.Forms.ComboBox();
			this.CbCmdHistory = new System.Windows.Forms.ComboBox();
			this.CbResultHistory = new System.Windows.Forms.ComboBox();
			this.BtnAllClear = new System.Windows.Forms.Button();
			this.NumericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.BtnDgvMacro = new System.Windows.Forms.Button();
			this.BtnDgvCmd = new System.Windows.Forms.Button();
			this.Lbl2 = new System.Windows.Forms.Label();
			this.TbDgvCmdSearch = new System.Windows.Forms.TextBox();
			this.DgvCmd = new System.Windows.Forms.DataGridView();
			this.DgvCmd01 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TbResult = new System.Windows.Forms.TextBox();
			this.TbCmdSub = new System.Windows.Forms.TextBox();
			this.CmsCmdSub = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CmsCmdSub_全クリア = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmdSub_全コピー = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.CmsCmdSub_コピー = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmdSub_切り取り = new System.Windows.Forms.ToolStripMenuItem();
			this.CmsCmdSub_貼り付け = new System.Windows.Forms.ToolStripMenuItem();
			this.LblWait = new System.Windows.Forms.Label();
			this.TbResultInfo = new System.Windows.Forms.TextBox();
			this.BtnCmdBackColor = new System.Windows.Forms.Button();
			this.LstTbCmd = new System.Windows.Forms.ListBox();
			this.Lbl_F1 = new System.Windows.Forms.Label();
			this.Lbl_F2 = new System.Windows.Forms.Label();
			this.Lbl_F3 = new System.Windows.Forms.Label();
			this.Lbl_F4 = new System.Windows.Forms.Label();
			this.Lbl_F5 = new System.Windows.Forms.Label();
			this.Lbl_F6 = new System.Windows.Forms.Label();
			this.Lbl_F7 = new System.Windows.Forms.Label();
			this.Lbl_F8 = new System.Windows.Forms.Label();
			this.Lbl_F9 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.DgvMacro)).BeginInit();
			this.CmsResult.SuspendLayout();
			this.CmsCmd.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.DgvCmd)).BeginInit();
			this.CmsCmdSub.SuspendLayout();
			this.SuspendLayout();
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
            this.Dgv_Tbc21,
            this.Dgv_Tbc22});
			this.DgvMacro.GridColor = System.Drawing.Color.LightGray;
			this.DgvMacro.Location = new System.Drawing.Point(87, 119);
			this.DgvMacro.Margin = new System.Windows.Forms.Padding(0);
			this.DgvMacro.MultiSelect = false;
			this.DgvMacro.Name = "DgvMacro";
			this.DgvMacro.ReadOnly = true;
			this.DgvMacro.RowHeadersVisible = false;
			this.DgvMacro.RowTemplate.Height = 21;
			this.DgvMacro.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.DgvMacro.Size = new System.Drawing.Size(68, 23);
			this.DgvMacro.StandardTab = true;
			this.DgvMacro.TabIndex = 7;
			this.DgvMacro.TabStop = false;
			this.DgvMacro.Click += new System.EventHandler(this.DgvMacro_Click);
			this.DgvMacro.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgvMacro_KeyDown);
			this.DgvMacro.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DgvMacro_KeyUp);
			this.DgvMacro.MouseHover += new System.EventHandler(this.DgvMacro_MouseHover);
			// 
			// Dgv_Tbc21
			// 
			this.Dgv_Tbc21.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Dgv_Tbc21.FillWeight = 150F;
			this.Dgv_Tbc21.HeaderText = "マクロ";
			this.Dgv_Tbc21.MinimumWidth = 80;
			this.Dgv_Tbc21.Name = "Dgv_Tbc21";
			this.Dgv_Tbc21.ReadOnly = true;
			this.Dgv_Tbc21.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Dgv_Tbc21.Width = 80;
			// 
			// Dgv_Tbc22
			// 
			this.Dgv_Tbc22.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Dgv_Tbc22.FillWeight = 150F;
			this.Dgv_Tbc22.HeaderText = "説明";
			this.Dgv_Tbc22.MinimumWidth = 265;
			this.Dgv_Tbc22.Name = "Dgv_Tbc22";
			this.Dgv_Tbc22.ReadOnly = true;
			this.Dgv_Tbc22.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Dgv_Tbc22.Width = 265;
			// 
			// CmsResult
			// 
			this.CmsResult.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsResult_上へ,
            this.CmsResult_下へ,
            this.toolStripMenuItem1,
            this.CmsResult_全選択,
            this.CmsResult_全クリア,
            this.CmsResult_全コピー,
            this.CmsResult_L3,
            this.CmsResult_コピー,
            this.CmsResult_切り取り,
            this.CmsResult_貼り付け,
            this.CmsResult_ファイル名を貼り付け,
            this.CmsResult_L4,
            this.CmsResult_名前を付けて保存});
			this.CmsResult.Name = "CmsResult";
			this.CmsResult.Size = new System.Drawing.Size(181, 264);
			// 
			// CmsResult_上へ
			// 
			this.CmsResult_上へ.ForeColor = System.Drawing.Color.OrangeRed;
			this.CmsResult_上へ.Name = "CmsResult_上へ";
			this.CmsResult_上へ.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_上へ.Text = "▲";
			this.CmsResult_上へ.Click += new System.EventHandler(this.CmsResult_上へ_Click);
			// 
			// CmsResult_下へ
			// 
			this.CmsResult_下へ.ForeColor = System.Drawing.Color.OrangeRed;
			this.CmsResult_下へ.Name = "CmsResult_下へ";
			this.CmsResult_下へ.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_下へ.Text = "▼";
			this.CmsResult_下へ.Click += new System.EventHandler(this.CmsResult_下へ_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(167, 6);
			// 
			// CmsResult_全選択
			// 
			this.CmsResult_全選択.Name = "CmsResult_全選択";
			this.CmsResult_全選択.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_全選択.Text = "全選択";
			this.CmsResult_全選択.Click += new System.EventHandler(this.CmsResult_全選択_Click);
			// 
			// CmsResult_全クリア
			// 
			this.CmsResult_全クリア.Name = "CmsResult_全クリア";
			this.CmsResult_全クリア.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_全クリア.Text = "全クリア";
			this.CmsResult_全クリア.Click += new System.EventHandler(this.CmsResult_全クリア_Click);
			// 
			// CmsResult_全コピー
			// 
			this.CmsResult_全コピー.Name = "CmsResult_全コピー";
			this.CmsResult_全コピー.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_全コピー.Text = "全コピー";
			this.CmsResult_全コピー.Click += new System.EventHandler(this.CmsResult_全コピー_Click);
			// 
			// CmsResult_L3
			// 
			this.CmsResult_L3.Name = "CmsResult_L3";
			this.CmsResult_L3.Size = new System.Drawing.Size(167, 6);
			// 
			// CmsResult_コピー
			// 
			this.CmsResult_コピー.Name = "CmsResult_コピー";
			this.CmsResult_コピー.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_コピー.Text = "コピー";
			this.CmsResult_コピー.Click += new System.EventHandler(this.CmsResult_コピー_Click);
			// 
			// CmsResult_切り取り
			// 
			this.CmsResult_切り取り.Name = "CmsResult_切り取り";
			this.CmsResult_切り取り.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_切り取り.Text = "切り取り";
			this.CmsResult_切り取り.Click += new System.EventHandler(this.CmsResult_切り取り_Click);
			// 
			// CmsResult_貼り付け
			// 
			this.CmsResult_貼り付け.Name = "CmsResult_貼り付け";
			this.CmsResult_貼り付け.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_貼り付け.Text = "貼り付け";
			this.CmsResult_貼り付け.Click += new System.EventHandler(this.CmsResult_貼り付け_Click);
			// 
			// CmsResult_ファイル名を貼り付け
			// 
			this.CmsResult_ファイル名を貼り付け.Name = "CmsResult_ファイル名を貼り付け";
			this.CmsResult_ファイル名を貼り付け.Size = new System.Drawing.Size(170, 22);
			this.CmsResult_ファイル名を貼り付け.Text = "ファイル名を貼り付け";
			this.CmsResult_ファイル名を貼り付け.Click += new System.EventHandler(this.CmsResult_ファイル名を貼り付け_Click);
			// 
			// CmsResult_L4
			// 
			this.CmsResult_L4.Name = "CmsResult_L4";
			this.CmsResult_L4.Size = new System.Drawing.Size(167, 6);
			// 
			// CmsResult_名前を付けて保存
			// 
			this.CmsResult_名前を付けて保存.ForeColor = System.Drawing.SystemColors.ControlText;
			this.CmsResult_名前を付けて保存.Name = "CmsResult_名前を付けて保存";
			this.CmsResult_名前を付けて保存.Size = new System.Drawing.Size(180, 22);
			this.CmsResult_名前を付けて保存.Text = "名前を付けて保存";
			this.CmsResult_名前を付けて保存.Click += new System.EventHandler(this.CmsResult_名前を付けて保存_Click);
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
			this.TbCmd.Location = new System.Drawing.Point(11, 26);
			this.TbCmd.Margin = new System.Windows.Forms.Padding(0);
			this.TbCmd.Multiline = true;
			this.TbCmd.Name = "TbCmd";
			this.TbCmd.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.TbCmd.Size = new System.Drawing.Size(444, 42);
			this.TbCmd.TabIndex = 2;
			this.TbCmd.WordWrap = false;
			this.TbCmd.DragDrop += new System.Windows.Forms.DragEventHandler(this.TbCmd_DragDrop);
			this.TbCmd.DragEnter += new System.Windows.Forms.DragEventHandler(this.TbCmd_DragEnter);
			this.TbCmd.Enter += new System.EventHandler(this.TbCmd_Enter);
			this.TbCmd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbCmd_KeyPress);
			this.TbCmd.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbCmd_KeyUp);
			this.TbCmd.Leave += new System.EventHandler(this.TbCmd_Leave);
			this.TbCmd.MouseHover += new System.EventHandler(this.TbCmd_MouseHover);
			// 
			// CmsCmd
			// 
			this.CmsCmd.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsCmd_左へ,
            this.CmsCmd_右へ,
            this.toolStripSeparator3,
            this.CmsCmd_全クリア,
            this.CmsCmd_全コピー,
            this.toolStripSeparator1,
            this.CmsCmd_コピー,
            this.CmsCmd_切り取り,
            this.CmsCmd_貼り付け,
            this.toolStripSeparator4,
            this.CmsCmd_DQで囲む,
            this.toolStripSeparator7,
            this.CmsCmd_コマンドをグループ化,
            this.CmsCmd_コマンドを保存,
            this.CmsCmd_コマンドを読込,
            this.toolStripSeparator6,
            this.CmsCmd_キー操作});
			this.CmsCmd.Name = "CmsResult";
			this.CmsCmd.Size = new System.Drawing.Size(175, 298);
			// 
			// CmsCmd_左へ
			// 
			this.CmsCmd_左へ.ForeColor = System.Drawing.Color.OrangeRed;
			this.CmsCmd_左へ.Name = "CmsCmd_左へ";
			this.CmsCmd_左へ.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_左へ.Text = "◀";
			this.CmsCmd_左へ.Click += new System.EventHandler(this.CmsCmd_左へ_Click);
			// 
			// CmsCmd_右へ
			// 
			this.CmsCmd_右へ.ForeColor = System.Drawing.Color.OrangeRed;
			this.CmsCmd_右へ.Name = "CmsCmd_右へ";
			this.CmsCmd_右へ.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_右へ.Text = "▶";
			this.CmsCmd_右へ.Click += new System.EventHandler(this.CmsCmd_右へ_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(171, 6);
			// 
			// CmsCmd_全クリア
			// 
			this.CmsCmd_全クリア.Name = "CmsCmd_全クリア";
			this.CmsCmd_全クリア.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_全クリア.Text = "全クリア";
			this.CmsCmd_全クリア.Click += new System.EventHandler(this.CmsCmd_全クリア_Click);
			// 
			// CmsCmd_全コピー
			// 
			this.CmsCmd_全コピー.Name = "CmsCmd_全コピー";
			this.CmsCmd_全コピー.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_全コピー.Text = "全コピー";
			this.CmsCmd_全コピー.Click += new System.EventHandler(this.CmsCmd_全コピー_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
			// 
			// CmsCmd_コピー
			// 
			this.CmsCmd_コピー.Name = "CmsCmd_コピー";
			this.CmsCmd_コピー.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_コピー.Text = "コピー";
			this.CmsCmd_コピー.Click += new System.EventHandler(this.CmsCmd_コピー_Click);
			// 
			// CmsCmd_切り取り
			// 
			this.CmsCmd_切り取り.Name = "CmsCmd_切り取り";
			this.CmsCmd_切り取り.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_切り取り.Text = "切り取り";
			this.CmsCmd_切り取り.Click += new System.EventHandler(this.CmsCmd_切り取り_Click);
			// 
			// CmsCmd_貼り付け
			// 
			this.CmsCmd_貼り付け.Name = "CmsCmd_貼り付け";
			this.CmsCmd_貼り付け.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_貼り付け.Text = "貼り付け";
			this.CmsCmd_貼り付け.Click += new System.EventHandler(this.CmsCmd_貼り付け_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(171, 6);
			// 
			// CmsCmd_DQで囲む
			// 
			this.CmsCmd_DQで囲む.Name = "CmsCmd_DQで囲む";
			this.CmsCmd_DQで囲む.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_DQで囲む.Text = "選択範囲を \" で囲む";
			this.CmsCmd_DQで囲む.Click += new System.EventHandler(this.CmsCmd_DQで囲む_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(171, 6);
			// 
			// CmsCmd_コマンドをグループ化
			// 
			this.CmsCmd_コマンドをグループ化.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsCmd_コマンドをグループ化_追加,
            this.CmsCmd_コマンドをグループ化_出力,
            this.CmsCmd_コマンドをグループ化_消去,
            this.toolStripSeparator5,
            this.CmsCmd_コマンドをグループ化_簡単な説明});
			this.CmsCmd_コマンドをグループ化.Name = "CmsCmd_コマンドをグループ化";
			this.CmsCmd_コマンドをグループ化.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_コマンドをグループ化.Text = "コマンドをグループ化";
			// 
			// CmsCmd_コマンドをグループ化_追加
			// 
			this.CmsCmd_コマンドをグループ化_追加.Name = "CmsCmd_コマンドをグループ化_追加";
			this.CmsCmd_コマンドをグループ化_追加.Size = new System.Drawing.Size(150, 22);
			this.CmsCmd_コマンドをグループ化_追加.Text = "キャッシュに追加";
			this.CmsCmd_コマンドをグループ化_追加.Click += new System.EventHandler(this.CmsCmd_コマンドをグループ化_追加_Click);
			// 
			// CmsCmd_コマンドをグループ化_出力
			// 
			this.CmsCmd_コマンドをグループ化_出力.Name = "CmsCmd_コマンドをグループ化_出力";
			this.CmsCmd_コマンドをグループ化_出力.Size = new System.Drawing.Size(150, 22);
			this.CmsCmd_コマンドをグループ化_出力.Text = "出力";
			this.CmsCmd_コマンドをグループ化_出力.Click += new System.EventHandler(this.CmsCmd_コマンドをグループ化_出力_Click);
			// 
			// CmsCmd_コマンドをグループ化_消去
			// 
			this.CmsCmd_コマンドをグループ化_消去.Name = "CmsCmd_コマンドをグループ化_消去";
			this.CmsCmd_コマンドをグループ化_消去.Size = new System.Drawing.Size(150, 22);
			this.CmsCmd_コマンドをグループ化_消去.Text = "キャッシュを消去";
			this.CmsCmd_コマンドをグループ化_消去.Click += new System.EventHandler(this.CmsCmd_コマンドをグループ化_消去_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(147, 6);
			// 
			// CmsCmd_コマンドをグループ化_簡単な説明
			// 
			this.CmsCmd_コマンドをグループ化_簡単な説明.Name = "CmsCmd_コマンドをグループ化_簡単な説明";
			this.CmsCmd_コマンドをグループ化_簡単な説明.Size = new System.Drawing.Size(150, 22);
			this.CmsCmd_コマンドをグループ化_簡単な説明.Text = "簡単な説明";
			this.CmsCmd_コマンドをグループ化_簡単な説明.Click += new System.EventHandler(this.CmsCmd_コマンドをグループ化_簡単な説明_Click);
			// 
			// CmsCmd_コマンドを保存
			// 
			this.CmsCmd_コマンドを保存.Name = "CmsCmd_コマンドを保存";
			this.CmsCmd_コマンドを保存.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_コマンドを保存.Text = "コマンドを保存";
			this.CmsCmd_コマンドを保存.Click += new System.EventHandler(this.CmsCmd_コマンドを保存_Click);
			// 
			// CmsCmd_コマンドを読込
			// 
			this.CmsCmd_コマンドを読込.Name = "CmsCmd_コマンドを読込";
			this.CmsCmd_コマンドを読込.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_コマンドを読込.Text = "コマンドを読込";
			this.CmsCmd_コマンドを読込.Click += new System.EventHandler(this.CmsCmd_コマンドを読込_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(171, 6);
			// 
			// CmsCmd_キー操作
			// 
			this.CmsCmd_キー操作.Name = "CmsCmd_キー操作";
			this.CmsCmd_キー操作.Size = new System.Drawing.Size(174, 22);
			this.CmsCmd_キー操作.Text = "キー操作について";
			this.CmsCmd_キー操作.Click += new System.EventHandler(this.CmsCmd_キー操作_Click);
			// 
			// TbCurDir
			// 
			this.TbCurDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TbCurDir.BackColor = System.Drawing.Color.DimGray;
			this.TbCurDir.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.TbCurDir.ContextMenuStrip = this.CmsNull;
			this.TbCurDir.Cursor = System.Windows.Forms.Cursors.Default;
			this.TbCurDir.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.TbCurDir.ForeColor = System.Drawing.Color.White;
			this.TbCurDir.Location = new System.Drawing.Point(10, 5);
			this.TbCurDir.Margin = new System.Windows.Forms.Padding(0);
			this.TbCurDir.Name = "TbCurDir";
			this.TbCurDir.Size = new System.Drawing.Size(444, 13);
			this.TbCurDir.TabIndex = 1;
			this.TbCurDir.TabStop = false;
			this.TbCurDir.Text = "TbCurDir";
			this.TbCurDir.WordWrap = false;
			this.TbCurDir.Click += new System.EventHandler(this.TbCurDir_Click);
			this.TbCurDir.MouseHover += new System.EventHandler(this.TbCurDir_MouseHover);
			// 
			// CmsNull
			// 
			this.CmsNull.Name = "contextMenuStrip0";
			this.CmsNull.Size = new System.Drawing.Size(61, 4);
			// 
			// ToolTip1
			// 
			this.ToolTip1.AutoPopDelay = 6000;
			this.ToolTip1.BackColor = System.Drawing.Color.Ivory;
			this.ToolTip1.ForeColor = System.Drawing.Color.Black;
			this.ToolTip1.InitialDelay = 500;
			this.ToolTip1.ReshowDelay = 100;
			// 
			// BtnCmdExec
			// 
			this.BtnCmdExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnCmdExec.BackColor = System.Drawing.Color.RoyalBlue;
			this.BtnCmdExec.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BtnCmdExec.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnCmdExec.Font = new System.Drawing.Font("Yu Gothic UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnCmdExec.ForeColor = System.Drawing.Color.White;
			this.BtnCmdExec.Location = new System.Drawing.Point(404, 119);
			this.BtnCmdExec.Margin = new System.Windows.Forms.Padding(0);
			this.BtnCmdExec.Name = "BtnCmdExec";
			this.BtnCmdExec.Size = new System.Drawing.Size(22, 22);
			this.BtnCmdExec.TabIndex = 12;
			this.BtnCmdExec.TabStop = false;
			this.BtnCmdExec.Text = "▶";
			this.ToolTip1.SetToolTip(this.BtnCmdExec, " [F5] 実行");
			this.BtnCmdExec.UseVisualStyleBackColor = false;
			this.BtnCmdExec.Click += new System.EventHandler(this.BtnCmdExec_Click);
			// 
			// BtnResultMem
			// 
			this.BtnResultMem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.BtnResultMem.BackColor = System.Drawing.Color.OrangeRed;
			this.BtnResultMem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnResultMem.Font = new System.Drawing.Font("Yu Gothic UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnResultMem.ForeColor = System.Drawing.Color.White;
			this.BtnResultMem.Location = new System.Drawing.Point(72, 400);
			this.BtnResultMem.Margin = new System.Windows.Forms.Padding(0);
			this.BtnResultMem.Name = "BtnResultMem";
			this.BtnResultMem.Size = new System.Drawing.Size(22, 22);
			this.BtnResultMem.TabIndex = 16;
			this.BtnResultMem.TabStop = false;
			this.BtnResultMem.Text = "■";
			this.ToolTip1.SetToolTip(this.BtnResultMem, "[F8] 出力を記憶");
			this.BtnResultMem.UseVisualStyleBackColor = false;
			this.BtnResultMem.Click += new System.EventHandler(this.BtnResultMem_Click);
			// 
			// CbTextCode
			// 
			this.CbTextCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CbTextCode.BackColor = System.Drawing.Color.DimGray;
			this.CbTextCode.ContextMenuStrip = this.CmsNull;
			this.CbTextCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CbTextCode.DropDownWidth = 72;
			this.CbTextCode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CbTextCode.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.CbTextCode.ForeColor = System.Drawing.Color.White;
			this.CbTextCode.FormattingEnabled = true;
			this.CbTextCode.Location = new System.Drawing.Point(321, 120);
			this.CbTextCode.Margin = new System.Windows.Forms.Padding(0);
			this.CbTextCode.Name = "CbTextCode";
			this.CbTextCode.Size = new System.Drawing.Size(72, 20);
			this.CbTextCode.TabIndex = 11;
			this.CbTextCode.TabStop = false;
			this.ToolTip1.SetToolTip(this.CbTextCode, "[F4] 出力文字コード");
			this.CbTextCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CbTextCode_KeyUp);
			// 
			// CbCmdHistory
			// 
			this.CbCmdHistory.BackColor = System.Drawing.Color.WhiteSmoke;
			this.CbCmdHistory.ContextMenuStrip = this.CmsNull;
			this.CbCmdHistory.DropDownHeight = 200;
			this.CbCmdHistory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CbCmdHistory.DropDownWidth = 300;
			this.CbCmdHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CbCmdHistory.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.CbCmdHistory.ForeColor = System.Drawing.Color.Black;
			this.CbCmdHistory.FormattingEnabled = true;
			this.CbCmdHistory.IntegralHeight = false;
			this.CbCmdHistory.ItemHeight = 12;
			this.CbCmdHistory.Location = new System.Drawing.Point(11, 120);
			this.CbCmdHistory.Margin = new System.Windows.Forms.Padding(0);
			this.CbCmdHistory.MaxDropDownItems = 20;
			this.CbCmdHistory.Name = "CbCmdHistory";
			this.CbCmdHistory.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.CbCmdHistory.Size = new System.Drawing.Size(55, 20);
			this.CbCmdHistory.TabIndex = 5;
			this.CbCmdHistory.TabStop = false;
			this.ToolTip1.SetToolTip(this.CbCmdHistory, "[F1] マクロ・コマンド履歴");
			this.CbCmdHistory.DropDownClosed += new System.EventHandler(this.CbCmdHistory_DropDownClosed);
			this.CbCmdHistory.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CbCmdHistory_KeyUp);
			// 
			// CbResultHistory
			// 
			this.CbResultHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.CbResultHistory.BackColor = System.Drawing.Color.WhiteSmoke;
			this.CbResultHistory.ContextMenuStrip = this.CmsNull;
			this.CbResultHistory.DropDownHeight = 200;
			this.CbResultHistory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CbResultHistory.DropDownWidth = 300;
			this.CbResultHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CbResultHistory.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.CbResultHistory.ForeColor = System.Drawing.Color.Black;
			this.CbResultHistory.FormattingEnabled = true;
			this.CbResultHistory.IntegralHeight = false;
			this.CbResultHistory.ItemHeight = 12;
			this.CbResultHistory.Location = new System.Drawing.Point(11, 401);
			this.CbResultHistory.Margin = new System.Windows.Forms.Padding(0);
			this.CbResultHistory.MaxDropDownItems = 20;
			this.CbResultHistory.Name = "CbResultHistory";
			this.CbResultHistory.Size = new System.Drawing.Size(55, 20);
			this.CbResultHistory.TabIndex = 15;
			this.CbResultHistory.TabStop = false;
			this.ToolTip1.SetToolTip(this.CbResultHistory, "[F7] 出力履歴");
			this.CbResultHistory.DropDownClosed += new System.EventHandler(this.CbResultHistory_DropDownClosed);
			this.CbResultHistory.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CbResultHistory_KeyUp);
			// 
			// BtnAllClear
			// 
			this.BtnAllClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnAllClear.BackColor = System.Drawing.Color.Crimson;
			this.BtnAllClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.BtnAllClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnAllClear.Font = new System.Drawing.Font("Yu Gothic UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnAllClear.ForeColor = System.Drawing.Color.White;
			this.BtnAllClear.Location = new System.Drawing.Point(432, 119);
			this.BtnAllClear.Margin = new System.Windows.Forms.Padding(0);
			this.BtnAllClear.Name = "BtnAllClear";
			this.BtnAllClear.Size = new System.Drawing.Size(22, 22);
			this.BtnAllClear.TabIndex = 13;
			this.BtnAllClear.TabStop = false;
			this.BtnAllClear.Text = "✖";
			this.ToolTip1.SetToolTip(this.BtnAllClear, "[F6] 全表示消去");
			this.BtnAllClear.UseVisualStyleBackColor = false;
			this.BtnAllClear.Click += new System.EventHandler(this.BtnAllClear_Click);
			// 
			// NumericUpDown1
			// 
			this.NumericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.NumericUpDown1.BackColor = System.Drawing.Color.DimGray;
			this.NumericUpDown1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.NumericUpDown1.ContextMenuStrip = this.CmsNull;
			this.NumericUpDown1.Cursor = System.Windows.Forms.Cursors.Default;
			this.NumericUpDown1.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.NumericUpDown1.ForeColor = System.Drawing.Color.White;
			this.NumericUpDown1.Location = new System.Drawing.Point(396, 401);
			this.NumericUpDown1.Margin = new System.Windows.Forms.Padding(0);
			this.NumericUpDown1.Maximum = new decimal(new int[] {
            288,
            0,
            0,
            0});
			this.NumericUpDown1.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.NumericUpDown1.Name = "NumericUpDown1";
			this.NumericUpDown1.Size = new System.Drawing.Size(45, 20);
			this.NumericUpDown1.TabIndex = 18;
			this.NumericUpDown1.TabStop = false;
			this.ToolTip1.SetToolTip(this.NumericUpDown1, "[F9] 出力文字サイズ");
			this.NumericUpDown1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.NumericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
			this.NumericUpDown1.Enter += new System.EventHandler(this.NumericUpDown1_Enter);
			this.NumericUpDown1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumericUpDown1_KeyUp);
			this.NumericUpDown1.Leave += new System.EventHandler(this.NumericUpDown1_Leave);
			// 
			// BtnDgvMacro
			// 
			this.BtnDgvMacro.BackColor = System.Drawing.Color.LightYellow;
			this.BtnDgvMacro.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.BtnDgvMacro.Font = new System.Drawing.Font("ＭＳ ゴシック", 3.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnDgvMacro.ForeColor = System.Drawing.Color.Gold;
			this.BtnDgvMacro.Location = new System.Drawing.Point(75, 124);
			this.BtnDgvMacro.Margin = new System.Windows.Forms.Padding(0);
			this.BtnDgvMacro.Name = "BtnDgvMacro";
			this.BtnDgvMacro.Size = new System.Drawing.Size(13, 13);
			this.BtnDgvMacro.TabIndex = 6;
			this.BtnDgvMacro.TabStop = false;
			this.ToolTip1.SetToolTip(this.BtnDgvMacro, "[F2] マクロ");
			this.BtnDgvMacro.UseVisualStyleBackColor = false;
			this.BtnDgvMacro.Click += new System.EventHandler(this.BtnDgvMacro_Click);
			// 
			// BtnDgvCmd
			// 
			this.BtnDgvCmd.BackColor = System.Drawing.Color.LightYellow;
			this.BtnDgvCmd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.BtnDgvCmd.Font = new System.Drawing.Font("ＭＳ ゴシック", 3.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnDgvCmd.ForeColor = System.Drawing.Color.Gold;
			this.BtnDgvCmd.Location = new System.Drawing.Point(158, 124);
			this.BtnDgvCmd.Margin = new System.Windows.Forms.Padding(0);
			this.BtnDgvCmd.Name = "BtnDgvCmd";
			this.BtnDgvCmd.Size = new System.Drawing.Size(13, 13);
			this.BtnDgvCmd.TabIndex = 8;
			this.BtnDgvCmd.TabStop = false;
			this.ToolTip1.SetToolTip(this.BtnDgvCmd, "[F3] コマンド");
			this.BtnDgvCmd.UseVisualStyleBackColor = false;
			this.BtnDgvCmd.Click += new System.EventHandler(this.BtnDgvCmd_Click);
			// 
			// Lbl2
			// 
			this.Lbl2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Lbl2.AutoSize = true;
			this.Lbl2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Lbl2.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl2.ForeColor = System.Drawing.Color.White;
			this.Lbl2.Location = new System.Drawing.Point(441, 402);
			this.Lbl2.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl2.Name = "Lbl2";
			this.Lbl2.Size = new System.Drawing.Size(20, 15);
			this.Lbl2.TabIndex = 19;
			this.Lbl2.Text = "pt";
			this.Lbl2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// TbDgvCmdSearch
			// 
			this.TbDgvCmdSearch.BackColor = System.Drawing.Color.LightYellow;
			this.TbDgvCmdSearch.ContextMenuStrip = this.CmsNull;
			this.TbDgvCmdSearch.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.TbDgvCmdSearch.ForeColor = System.Drawing.Color.Black;
			this.TbDgvCmdSearch.Location = new System.Drawing.Point(235, 120);
			this.TbDgvCmdSearch.Margin = new System.Windows.Forms.Padding(0);
			this.TbDgvCmdSearch.Name = "TbDgvCmdSearch";
			this.TbDgvCmdSearch.Size = new System.Drawing.Size(80, 20);
			this.TbDgvCmdSearch.TabIndex = 10;
			this.TbDgvCmdSearch.TabStop = false;
			this.TbDgvCmdSearch.WordWrap = false;
			this.TbDgvCmdSearch.TextChanged += new System.EventHandler(this.TbDgvCmdSearch_TextChanged);
			this.TbDgvCmdSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbDgvCmdSearch_KeyUp);
			this.TbDgvCmdSearch.MouseHover += new System.EventHandler(this.TbDgvCmdSearch_MouseHover);
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
            this.DgvCmd01});
			this.DgvCmd.GridColor = System.Drawing.Color.LightGray;
			this.DgvCmd.Location = new System.Drawing.Point(170, 119);
			this.DgvCmd.Margin = new System.Windows.Forms.Padding(0);
			this.DgvCmd.MultiSelect = false;
			this.DgvCmd.Name = "DgvCmd";
			this.DgvCmd.ReadOnly = true;
			this.DgvCmd.RowHeadersVisible = false;
			this.DgvCmd.RowTemplate.Height = 21;
			this.DgvCmd.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.DgvCmd.Size = new System.Drawing.Size(68, 23);
			this.DgvCmd.TabIndex = 9;
			this.DgvCmd.TabStop = false;
			this.DgvCmd.Click += new System.EventHandler(this.DgvCmd_Click);
			this.DgvCmd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgvCmd_KeyDown);
			this.DgvCmd.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DgvCmd_KeyUp);
			this.DgvCmd.MouseHover += new System.EventHandler(this.DgvCmd_MouseHover);
			// 
			// DgvCmd01
			// 
			this.DgvCmd01.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.DgvCmd01.HeaderText = "コマンド";
			this.DgvCmd01.MinimumWidth = 50;
			this.DgvCmd01.Name = "DgvCmd01";
			this.DgvCmd01.ReadOnly = true;
			this.DgvCmd01.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.DgvCmd01.Width = 260;
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
			this.TbResult.Location = new System.Drawing.Point(10, 145);
			this.TbResult.Margin = new System.Windows.Forms.Padding(0);
			this.TbResult.MaxLength = 2147483647;
			this.TbResult.Multiline = true;
			this.TbResult.Name = "TbResult";
			this.TbResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TbResult.Size = new System.Drawing.Size(445, 245);
			this.TbResult.TabIndex = 14;
			this.TbResult.TabStop = false;
			this.TbResult.WordWrap = false;
			this.TbResult.TextChanged += new System.EventHandler(this.TbResult_TextChanged);
			this.TbResult.DragDrop += new System.Windows.Forms.DragEventHandler(this.TbResult_DragDrop);
			this.TbResult.DragEnter += new System.Windows.Forms.DragEventHandler(this.TbResult_DragEnter);
			this.TbResult.Enter += new System.EventHandler(this.TbResult_Enter);
			this.TbResult.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbResult_KeyUp);
			this.TbResult.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TbResult_MouseUp);
			// 
			// TbCmdSub
			// 
			this.TbCmdSub.AllowDrop = true;
			this.TbCmdSub.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TbCmdSub.BackColor = System.Drawing.Color.Black;
			this.TbCmdSub.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.TbCmdSub.ContextMenuStrip = this.CmsCmdSub;
			this.TbCmdSub.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.TbCmdSub.ForeColor = System.Drawing.Color.Yellow;
			this.TbCmdSub.Location = new System.Drawing.Point(10, 70);
			this.TbCmdSub.Margin = new System.Windows.Forms.Padding(0);
			this.TbCmdSub.Multiline = true;
			this.TbCmdSub.Name = "TbCmdSub";
			this.TbCmdSub.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.TbCmdSub.Size = new System.Drawing.Size(445, 39);
			this.TbCmdSub.TabIndex = 4;
			this.TbCmdSub.TabStop = false;
			this.TbCmdSub.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbCmdSub_KeyUp);
			// 
			// CmsCmdSub
			// 
			this.CmsCmdSub.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmsCmdSub_全クリア,
            this.CmsCmdSub_全コピー,
            this.toolStripSeparator2,
            this.CmsCmdSub_コピー,
            this.CmsCmdSub_切り取り,
            this.CmsCmdSub_貼り付け});
			this.CmsCmdSub.Name = "CmsResult";
			this.CmsCmdSub.Size = new System.Drawing.Size(116, 120);
			// 
			// CmsCmdSub_全クリア
			// 
			this.CmsCmdSub_全クリア.Name = "CmsCmdSub_全クリア";
			this.CmsCmdSub_全クリア.Size = new System.Drawing.Size(115, 22);
			this.CmsCmdSub_全クリア.Text = "全クリア";
			this.CmsCmdSub_全クリア.Click += new System.EventHandler(this.CmsCmdSub_全クリア_Click);
			// 
			// CmsCmdSub_全コピー
			// 
			this.CmsCmdSub_全コピー.Name = "CmsCmdSub_全コピー";
			this.CmsCmdSub_全コピー.Size = new System.Drawing.Size(115, 22);
			this.CmsCmdSub_全コピー.Text = "全コピー";
			this.CmsCmdSub_全コピー.Click += new System.EventHandler(this.CmsCmdSub_全コピー_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(112, 6);
			// 
			// CmsCmdSub_コピー
			// 
			this.CmsCmdSub_コピー.Name = "CmsCmdSub_コピー";
			this.CmsCmdSub_コピー.Size = new System.Drawing.Size(115, 22);
			this.CmsCmdSub_コピー.Text = "コピー";
			this.CmsCmdSub_コピー.Click += new System.EventHandler(this.CmsCmdSub_コピー_Click);
			// 
			// CmsCmdSub_切り取り
			// 
			this.CmsCmdSub_切り取り.Name = "CmsCmdSub_切り取り";
			this.CmsCmdSub_切り取り.Size = new System.Drawing.Size(115, 22);
			this.CmsCmdSub_切り取り.Text = "切り取り";
			this.CmsCmdSub_切り取り.Click += new System.EventHandler(this.CmsCmdSub_切り取り_Click);
			// 
			// CmsCmdSub_貼り付け
			// 
			this.CmsCmdSub_貼り付け.Name = "CmsCmdSub_貼り付け";
			this.CmsCmdSub_貼り付け.Size = new System.Drawing.Size(115, 22);
			this.CmsCmdSub_貼り付け.Text = "貼り付け";
			this.CmsCmdSub_貼り付け.Click += new System.EventHandler(this.CmsCmdSub_貼り付け_Click);
			// 
			// LblWait
			// 
			this.LblWait.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.LblWait.AutoSize = true;
			this.LblWait.BackColor = System.Drawing.Color.Navy;
			this.LblWait.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.LblWait.Font = new System.Drawing.Font("ＭＳ ゴシック", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.LblWait.ForeColor = System.Drawing.Color.Yellow;
			this.LblWait.Location = new System.Drawing.Point(167, 204);
			this.LblWait.Margin = new System.Windows.Forms.Padding(0);
			this.LblWait.Name = "LblWait";
			this.LblWait.Padding = new System.Windows.Forms.Padding(16, 3, 16, 3);
			this.LblWait.Size = new System.Drawing.Size(130, 23);
			this.LblWait.TabIndex = 20;
			this.LblWait.Text = "Waiting...";
			this.LblWait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.LblWait.Visible = false;
			// 
			// TbResultInfo
			// 
			this.TbResultInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TbResultInfo.BackColor = System.Drawing.Color.DimGray;
			this.TbResultInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.TbResultInfo.ForeColor = System.Drawing.Color.White;
			this.TbResultInfo.Location = new System.Drawing.Point(100, 401);
			this.TbResultInfo.Margin = new System.Windows.Forms.Padding(0);
			this.TbResultInfo.Name = "TbResultInfo";
			this.TbResultInfo.ReadOnly = true;
			this.TbResultInfo.Size = new System.Drawing.Size(285, 12);
			this.TbResultInfo.TabIndex = 17;
			this.TbResultInfo.TabStop = false;
			this.TbResultInfo.Text = "TbResultInfo";
			this.TbResultInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TbResultInfo.WordWrap = false;
			// 
			// BtnCmdBackColor
			// 
			this.BtnCmdBackColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnCmdBackColor.BackColor = System.Drawing.Color.RoyalBlue;
			this.BtnCmdBackColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.BtnCmdBackColor.ForeColor = System.Drawing.Color.RoyalBlue;
			this.BtnCmdBackColor.Location = new System.Drawing.Point(9, 24);
			this.BtnCmdBackColor.Margin = new System.Windows.Forms.Padding(0);
			this.BtnCmdBackColor.Name = "BtnCmdBackColor";
			this.BtnCmdBackColor.Size = new System.Drawing.Size(448, 46);
			this.BtnCmdBackColor.TabIndex = 3;
			this.BtnCmdBackColor.TabStop = false;
			this.BtnCmdBackColor.UseVisualStyleBackColor = false;
			// 
			// LstTbCmd
			// 
			this.LstTbCmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.LstTbCmd.BackColor = System.Drawing.Color.Black;
			this.LstTbCmd.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.LstTbCmd.ForeColor = System.Drawing.Color.White;
			this.LstTbCmd.FormattingEnabled = true;
			this.LstTbCmd.HorizontalScrollbar = true;
			this.LstTbCmd.ItemHeight = 12;
			this.LstTbCmd.Location = new System.Drawing.Point(12, 44);
			this.LstTbCmd.Margin = new System.Windows.Forms.Padding(0);
			this.LstTbCmd.Name = "LstTbCmd";
			this.LstTbCmd.Size = new System.Drawing.Size(442, 64);
			this.LstTbCmd.TabIndex = 21;
			this.LstTbCmd.TabStop = false;
			this.LstTbCmd.Visible = false;
			this.LstTbCmd.Click += new System.EventHandler(this.LstTbCmd_Click);
			this.LstTbCmd.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.LstTbCmd_DrawItem);
			this.LstTbCmd.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LstTbCmd_KeyUp);
			// 
			// Lbl_F1
			// 
			this.Lbl_F1.AutoSize = true;
			this.Lbl_F1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F1.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F1.ForeColor = System.Drawing.Color.White;
			this.Lbl_F1.Location = new System.Drawing.Point(10, 110);
			this.Lbl_F1.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F1.Name = "Lbl_F1";
			this.Lbl_F1.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F1.TabIndex = 22;
			this.Lbl_F1.Text = "F1";
			// 
			// Lbl_F2
			// 
			this.Lbl_F2.AutoSize = true;
			this.Lbl_F2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F2.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F2.ForeColor = System.Drawing.Color.White;
			this.Lbl_F2.Location = new System.Drawing.Point(87, 110);
			this.Lbl_F2.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F2.Name = "Lbl_F2";
			this.Lbl_F2.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F2.TabIndex = 23;
			this.Lbl_F2.Text = "F2";
			// 
			// Lbl_F3
			// 
			this.Lbl_F3.AutoSize = true;
			this.Lbl_F3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F3.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F3.ForeColor = System.Drawing.Color.White;
			this.Lbl_F3.Location = new System.Drawing.Point(170, 110);
			this.Lbl_F3.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F3.Name = "Lbl_F3";
			this.Lbl_F3.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F3.TabIndex = 24;
			this.Lbl_F3.Text = "F3";
			// 
			// Lbl_F4
			// 
			this.Lbl_F4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Lbl_F4.AutoSize = true;
			this.Lbl_F4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F4.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F4.ForeColor = System.Drawing.Color.White;
			this.Lbl_F4.Location = new System.Drawing.Point(319, 110);
			this.Lbl_F4.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F4.Name = "Lbl_F4";
			this.Lbl_F4.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F4.TabIndex = 25;
			this.Lbl_F4.Text = "F4";
			// 
			// Lbl_F5
			// 
			this.Lbl_F5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Lbl_F5.AutoSize = true;
			this.Lbl_F5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F5.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F5.ForeColor = System.Drawing.Color.White;
			this.Lbl_F5.Location = new System.Drawing.Point(404, 110);
			this.Lbl_F5.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F5.Name = "Lbl_F5";
			this.Lbl_F5.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F5.TabIndex = 26;
			this.Lbl_F5.Text = "F5";
			// 
			// Lbl_F6
			// 
			this.Lbl_F6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Lbl_F6.AutoSize = true;
			this.Lbl_F6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F6.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F6.ForeColor = System.Drawing.Color.White;
			this.Lbl_F6.Location = new System.Drawing.Point(432, 110);
			this.Lbl_F6.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F6.Name = "Lbl_F6";
			this.Lbl_F6.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F6.TabIndex = 27;
			this.Lbl_F6.Text = "F6";
			// 
			// Lbl_F7
			// 
			this.Lbl_F7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.Lbl_F7.AutoSize = true;
			this.Lbl_F7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F7.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F7.ForeColor = System.Drawing.Color.White;
			this.Lbl_F7.Location = new System.Drawing.Point(9, 391);
			this.Lbl_F7.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F7.Name = "Lbl_F7";
			this.Lbl_F7.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F7.TabIndex = 28;
			this.Lbl_F7.Text = "F7";
			// 
			// Lbl_F8
			// 
			this.Lbl_F8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.Lbl_F8.AutoSize = true;
			this.Lbl_F8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F8.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F8.ForeColor = System.Drawing.Color.White;
			this.Lbl_F8.Location = new System.Drawing.Point(72, 391);
			this.Lbl_F8.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F8.Name = "Lbl_F8";
			this.Lbl_F8.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F8.TabIndex = 29;
			this.Lbl_F8.Text = "F8";
			// 
			// Lbl_F9
			// 
			this.Lbl_F9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Lbl_F9.AutoSize = true;
			this.Lbl_F9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Lbl_F9.Font = new System.Drawing.Font("ＭＳ ゴシック", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Lbl_F9.ForeColor = System.Drawing.Color.White;
			this.Lbl_F9.Location = new System.Drawing.Point(395, 391);
			this.Lbl_F9.Margin = new System.Windows.Forms.Padding(0);
			this.Lbl_F9.Name = "Lbl_F9";
			this.Lbl_F9.Size = new System.Drawing.Size(17, 11);
			this.Lbl_F9.TabIndex = 30;
			this.Lbl_F9.Text = "F9";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.DimGray;
			this.ClientSize = new System.Drawing.Size(464, 431);
			this.Controls.Add(this.LstTbCmd);
			this.Controls.Add(this.LblWait);
			this.Controls.Add(this.CbResultHistory);
			this.Controls.Add(this.CbCmdHistory);
			this.Controls.Add(this.DgvMacro);
			this.Controls.Add(this.TbDgvCmdSearch);
			this.Controls.Add(this.DgvCmd);
			this.Controls.Add(this.BtnResultMem);
			this.Controls.Add(this.BtnCmdExec);
			this.Controls.Add(this.NumericUpDown1);
			this.Controls.Add(this.Lbl2);
			this.Controls.Add(this.TbResult);
			this.Controls.Add(this.TbCurDir);
			this.Controls.Add(this.CbTextCode);
			this.Controls.Add(this.TbResultInfo);
			this.Controls.Add(this.BtnAllClear);
			this.Controls.Add(this.BtnDgvCmd);
			this.Controls.Add(this.BtnDgvMacro);
			this.Controls.Add(this.TbCmd);
			this.Controls.Add(this.BtnCmdBackColor);
			this.Controls.Add(this.TbCmdSub);
			this.Controls.Add(this.Lbl_F1);
			this.Controls.Add(this.Lbl_F2);
			this.Controls.Add(this.Lbl_F6);
			this.Controls.Add(this.Lbl_F5);
			this.Controls.Add(this.Lbl_F4);
			this.Controls.Add(this.Lbl_F3);
			this.Controls.Add(this.Lbl_F7);
			this.Controls.Add(this.Lbl_F8);
			this.Controls.Add(this.Lbl_F9);
			this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(480, 470);
			this.Name = "Form1";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "iwm_commandliner3";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.DgvMacro)).EndInit();
			this.CmsResult.ResumeLayout(false);
			this.CmsCmd.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.DgvCmd)).EndInit();
			this.CmsCmdSub.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.DataGridView DgvMacro;
		private System.Windows.Forms.ContextMenuStrip CmsResult;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_切り取り;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_コピー;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_貼り付け;
		private System.Windows.Forms.TextBox TbCmd;
		private System.Windows.Forms.ToolStripSeparator CmsResult_L3;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_全コピー;
		private System.Windows.Forms.TextBox TbCurDir;
		private System.Windows.Forms.ToolStripSeparator CmsResult_L4;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_名前を付けて保存;
		private System.Windows.Forms.ContextMenuStrip CmsCmd;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_貼り付け;
		private System.Windows.Forms.ToolTip ToolTip1;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_上へ;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_下へ;
		private System.Windows.Forms.ContextMenuStrip CmsNull;
		private System.Windows.Forms.NumericUpDown NumericUpDown1;
		private System.Windows.Forms.Label Lbl2;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_全コピー;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_切り取り;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コピー;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_全クリア;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_全クリア;
		private System.Windows.Forms.TextBox TbDgvCmdSearch;
		private System.Windows.Forms.DataGridView DgvCmd;
		private System.Windows.Forms.TextBox TbResult;
		private System.Windows.Forms.Button BtnCmdExec;
		private System.Windows.Forms.Button BtnResultMem;
		private System.Windows.Forms.TextBox TbCmdSub;
		private System.Windows.Forms.ContextMenuStrip CmsCmdSub;
		private System.Windows.Forms.ToolStripMenuItem CmsCmdSub_全クリア;
		private System.Windows.Forms.ToolStripMenuItem CmsCmdSub_全コピー;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem CmsCmdSub_切り取り;
		private System.Windows.Forms.ToolStripMenuItem CmsCmdSub_コピー;
		private System.Windows.Forms.ToolStripMenuItem CmsCmdSub_貼り付け;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ComboBox CbTextCode;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_ファイル名を貼り付け;
		private System.Windows.Forms.ToolStripMenuItem CmsResult_全選択;
		private System.Windows.Forms.ComboBox CbCmdHistory;
		private System.Windows.Forms.ComboBox CbResultHistory;
		private System.Windows.Forms.Label LblWait;
		private System.Windows.Forms.TextBox TbResultInfo;
		private System.Windows.Forms.Button BtnAllClear;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.DataGridViewTextBoxColumn Dgv_Tbc21;
		private System.Windows.Forms.DataGridViewTextBoxColumn Dgv_Tbc22;
		private System.Windows.Forms.DataGridViewTextBoxColumn DgvCmd01;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドをグループ化;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドをグループ化_追加;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドをグループ化_出力;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドを保存;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドを読込;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドをグループ化_消去;
		private System.Windows.Forms.Button BtnDgvMacro;
		private System.Windows.Forms.Button BtnDgvCmd;
		private System.Windows.Forms.Button BtnCmdBackColor;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_コマンドをグループ化_簡単な説明;
		private System.Windows.Forms.ListBox LstTbCmd;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_左へ;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_右へ;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_DQで囲む;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.Label Lbl_F1;
		private System.Windows.Forms.Label Lbl_F2;
		private System.Windows.Forms.Label Lbl_F3;
		private System.Windows.Forms.Label Lbl_F4;
		private System.Windows.Forms.Label Lbl_F5;
		private System.Windows.Forms.Label Lbl_F6;
		private System.Windows.Forms.Label Lbl_F7;
		private System.Windows.Forms.Label Lbl_F8;
		private System.Windows.Forms.Label Lbl_F9;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem CmsCmd_キー操作;
	}
}

