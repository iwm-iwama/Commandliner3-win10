using System;
using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
//using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic; // プロジェクト～参照の追加

namespace iwm_commandliner3
{
	public partial class Form1 : Form
	{
		//-----------
		// 大域定数
		//-----------
		private const string VERSION = "Ver.20200315_1710 'A-29' (C)2018-2020 iwm-iwama";

		private const string NL = "\r\n";

		private readonly string[] SPLITS = { NL };

		private readonly string[] TEXT_CODE = { "Shift_JIS", "UTF-8" };

		private readonly object[,] CMD = {
			// [マクロ]     [説明]                                         [引数]
			{ "#clear",     "出力消去      #clear",                           0 },
			{ "#cd",        "フォルダ変更  #cd \"..\"",                       1 },
			{ "#code",      "文字コード    #code \"UTF-8\"",                  1 },
			{ "#grep",      "検索          #grep \"2018\"",                   1 },
			{ "#except",    "不一致検索    #except \"2018\"",                 1 },
			{ "#replace",   "置換          #replace \".*(\\d{4}).*\" \"$1\"", 2 },
			{ "#repNL",     "改行を置換    #repNL \" \" \"\\\"\"",            2 },
			{ "#split",     "分割          #split \"\\t\" \"[0],[1]\"",       2 },
			{ "#toUpper",   "大文字に変換",                                   0 },
			{ "#toLower",   "小文字に変換",                                   0 },
			{ "#toWide",    "全角に変換",                                     0 },
			{ "#toZenNum",  "全角に変換(数字のみ)",                           0 },
			{ "#toZenKana", "全角に変換(カナのみ)",                           0 },
			{ "#toNarrow",  "半角に変換",                                     0 },
			{ "#toHanNum",  "半角に変換(数字のみ)",                           0 },
			{ "#toHanKana", "半角に変換(カナのみ)",                           0 },
			{ "#erase",     "文字消去      #erase \"0\" \"5\"",               2 },
			{ "#sort",      "ソート(昇順)",                                   0 },
			{ "#sort-r",    "ソート(降順)",                                   0 },
			{ "#uniq",      "重複行を消去",                                   0 },
			{ "#getLn",     "指定行取得    #getLn \"2\" \"0\"",               2 },
			{ "#rmBlankLn", "空白行削除",                                     0 },
			{ "#addLnNum",  "行番号付与",                                     0 },
			{ "#wget",      "ファイル取得  #wget \"http://.../index.html\"",  1 },
			{ "#fread",     "ファイル読込  #fread \"ファイル名\"",            1 },
			{ "#fwrite",    "ファイル書込  #fwrite \"ファイル名\"",           1 },
			{ "#stream",    "行毎に処理    #stream \"wget\" \"-q\"",          2 },
			{ "#msgbox",    "MsgBox        #msgbox \"本文\"",                 1 },
			{ "#calc",      "計算機        #calc \"pi / 180\"",               1 },
			{ "#now",       "現在日時",                                       0 },
			{ "#version",   "バージョン",                                     0 }
		};

		private readonly string FILE_FILTER = "All files (*.*)|*.*|Command (*.iwmcmd)|*.iwmcmd|Text (*.txt)|*.txt|Tab-Separated (*.tsv)|*.tsv|Comma-Separated (*.csv)|*.csv|HTML (*.html,*.htm)|*.html,*.htm";

		//-----------
		// 大域変数
		//-----------
		// CurDir
		private string CurDir = "";

		// 文字列
		private readonly StringBuilder Sb = new StringBuilder();

		// 履歴
		private readonly List<string> CmdHistory = new List<string>() { };
		private readonly Dictionary<string, string> ResultHistory = new Dictionary<string, string>();

		private delegate void MyEventHandler(object sender, DataReceivedEventArgs e);
		private event MyEventHandler MyEvent = null;

		private Process Ps1 = null;

		internal static class NativeMethods
		{
			[DllImport("User32.dll", CharSet = CharSet.Unicode)]
			internal static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
		}

		private const int EM_REPLACESEL = 0x00C2;

		//-------
		// Form
		//-------
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			// 入力例
			TbCmd.Text = "dir";

			TbCmd_Enter(sender, e);

			// CurDir表示
			CurDir = TbCurDir.Text = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(CurDir);

			// LstTbCmd に着色
			LstTbCmd.DrawMode = DrawMode.OwnerDrawFixed;

			// DgvMacro／DgvCmd 表示
			for (int _i1 = 0; _i1 < CMD.GetLength(0); _i1++)
			{
				_ = DgvMacro.Rows.Add(CMD[_i1, 0], CMD[_i1, 1]);
			}
			GblDgvMacroOpen = true;
			BtnDgvMacro_Click(sender, e);

			SubDgvCmdLoad();
			GblDgvCmdOpen = true;
			BtnDgvCmd_Click(sender, e);

			// Encode
			foreach (string _s1 in TEXT_CODE)
			{
				_ = CbTextCode.Items.Add(_s1);
			}
			CbTextCode.Text = TEXT_CODE[0];

			// LblResult
			TbResult_MouseUp(sender, null);

			// フォントサイズ
			NumericUpDown1.Value = (int)Math.Round(TbResult.Font.Size);

			// 初フォーカス
			SubTbCmdFocus(-1);
		}

		//-----------
		// TbCurDir
		//-----------
		private void TbCurDir_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog fbd = new FolderBrowserDialog
			{
				Description = "フォルダを指定してください。",
				SelectedPath = Environment.CurrentDirectory,
				ShowNewFolderButton = true
			}
			)
			{
				if (fbd.ShowDialog(this) == DialogResult.OK)
				{
					CurDir = TbCurDir.Text = fbd.SelectedPath;
					Directory.SetCurrentDirectory(CurDir);
				}
			}
			TbCurDir.SelectAll();
		}

		private void TbCurDir_MouseHover(object sender, EventArgs e)
		{
			ToolTip1.SetToolTip(TbCurDir, TbCurDir.Text);
		}

		//--------
		// TbCmd
		//--------
		private int GblTbCmdPos = 0;

		private void TbCmd_Enter(object sender, EventArgs e)
		{
			LstTbCmd.Visible = false;
			BtnCmdBackColor.BackColor = BtnCmdBackColor.ForeColor = Color.RoyalBlue;
			Lbl_F1.ForeColor = Lbl_F2.ForeColor = Lbl_F3.ForeColor = Lbl_F4.ForeColor = Lbl_F5.ForeColor = Lbl_F6.ForeColor = Lbl_F7.ForeColor = Lbl_F8.ForeColor = Lbl_F9.ForeColor = Color.White;
		}

		private void TbCmd_Leave(object sender, EventArgs e)
		{
			GblTbCmdPos = TbCmd.SelectionStart;
			BtnCmdBackColor.BackColor = BtnCmdBackColor.ForeColor = Color.Crimson;
			Lbl_F1.ForeColor = Lbl_F2.ForeColor = Lbl_F3.ForeColor = Lbl_F4.ForeColor = Lbl_F5.ForeColor = Lbl_F6.ForeColor = Lbl_F7.ForeColor = Lbl_F8.ForeColor = Lbl_F9.ForeColor = Color.Gray;
		}

		private void TbCmd_KeyPress(object sender, KeyPressEventArgs e)
		{
			switch (e.KeyChar)
			{
				// IME入力対策
				case (char)Keys.Enter:
					GblTbCmdPos = TbCmd.SelectionStart;
					BtnCmdExec_Click(sender, null);
					break;
			}
		}

		private void TbCmd_KeyUp(object sender, KeyEventArgs e)
		{
			// Fontを戻す
			TbCmd.Font = new Font(TbCmd.Font.Name, TbCmd.Font.Size);

			// [Ctrl+V] のとき
			if (e.KeyData == (Keys.Control | Keys.V))
			{
				TbCmd.Text = TbCmd.Text.Replace(NL, " ").Trim();
				SubTbCmdFocus(-1);
				return;
			}

			int iLen = TbCmd.TextLength;

			switch (e.KeyCode)
			{
				case Keys.Escape:
					_ = TbResult.Focus();
					break;

				case Keys.F1:
					CbCmdHistory.DroppedDown = true;
					_ = CbCmdHistory.Focus();
					// 選択セルがないときエラー
					try
					{
						CbCmdHistory.SelectedIndex = 0;
					}
					catch
					{
					}
					break;

				case Keys.F2:
					BtnDgvMacro_Click(sender, e);
					_ = DgvMacro.Focus();
					// 選択セルがないときエラー
					try
					{
						DgvMacro.CurrentCell = DgvMacro[0, 0];
						DgvMacro.FirstDisplayedScrollingRowIndex = DgvMacro.CurrentRow.Index;
					}
					catch
					{
					}
					break;

				case Keys.F3:
					BtnDgvCmd_Click(sender, e);
					_ = DgvCmd.Focus();
					// 選択セルがないときエラー
					try
					{
						DgvCmd.CurrentCell = DgvCmd[0, 0];
						DgvCmd.FirstDisplayedScrollingRowIndex = DgvCmd.CurrentRow.Index;
					}
					catch
					{
					}
					break;

				case Keys.F4:
					CbTextCode.DroppedDown = true;
					_ = CbTextCode.Focus();
					break;

				case Keys.F5:
					BtnCmdExec_Click(sender, e);
					break;

				case Keys.F6:
					BtnAllClear_Click(sender, e);
					break;

				case Keys.F7:
					CbResultHistory.DroppedDown = true;
					_ = CbResultHistory.Focus();
					// 選択セルがないときエラー
					try
					{
						CbResultHistory.SelectedIndex = 0;
					}
					catch
					{
					}
					break;

				case Keys.F8:
					BtnResultMem_Click(sender, e);
					break;

				case Keys.F9:
					NumericUpDown1.Focus();
					break;

				case Keys.Enter:
					// KeyPressと連動
					TbCmd.Text = TbCmd.Text.Replace(NL, "");
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.Up:
					if (TbCmd.SelectionStart == 0)
					{
						TbCmd.Select(TbCmd.TextLength, 0);
					}
					else
					{
						char[] ac1 = TbCmd.Text.ToCharArray();
						int iPos = TbCmd.SelectionStart - 1;
						for (; iPos > 0; iPos--)
						{
							if (ac1[iPos] == ' ')
							{
								break;
							}
						}
						TbCmd.Select(iPos, 0);
					}
					break;

				case Keys.Down:
					LstTbCmd.Items.Clear();
					_ = LstTbCmd.Items.Add("[×]");
					_ = LstTbCmd.Items.Add(@"..\");
					foreach (string _s1 in Directory.GetDirectories(TbCurDir.Text, "*"))
					{
						_ = LstTbCmd.Items.Add(Path.GetFileName(_s1) + @"\");
					}
					foreach (string _s1 in Directory.GetFiles(TbCurDir.Text, "*"))
					{
						_ = LstTbCmd.Items.Add(Path.GetFileName(_s1));
					}
					LstTbCmd.Visible = true;
					_ = LstTbCmd.Focus();
					LstTbCmd.SetSelected(0, true);
					break;

				case Keys.Right:
					if (iLen == TbCmd.SelectionStart)
					{
						TbCmd.Text += " ";
						TbCmd.Select(TbCmd.TextLength, 0);
						// 後段で補完(*)
					}
					break;

				case Keys.PageUp:
					TbCmd.Text = TbCmd.Text.Substring(TbCmd.SelectionStart);
					break;

				case Keys.PageDown:
					TbCmd.Text = TbCmd.Text.Substring(0, TbCmd.SelectionStart);
					SubTbCmdFocus(-1);
					break;
			}

			// 補完(*)
			if (e.KeyCode == Keys.Right && TbCmd.TextLength == TbCmd.SelectionStart && Regex.IsMatch(TbCmd.Text, @"^.+\s+"))
			{
				TbCmd.ForeColor = Color.Red;
				TbCmd.Text = Regex.Replace(TbCmd.Text, @"\s$", " \"\"").Trim();
				TbCmd.Select(TbCmd.TextLength - 1, 0);
			}
			else
			{
				TbCmd.ForeColor = Color.Black;
			}

			TbCmd.ScrollToCaret();
		}

		private void TbCmd_MouseHover(object sender, EventArgs e)
		{
			ToolTip1.SetToolTip(TbCmd, Regex.Replace(TbCmd.Text, @"(?<=\G.{80})(?!$)", NL));
		}

		private void TbCmd_DragDrop(object sender, DragEventArgs e)
		{
			string[] a1 = (string[])e.Data.GetData(DataFormats.FileDrop, false);

			int i1 = TbCmd.SelectionStart;
			if (i1 == TbCmd.TextLength)
			{
				TbCmd.Text += " ";
				++i1;
			}

			TbCmd.Text = TbCmd.Text.Substring(0, i1) + string.Join(" ", a1) + TbCmd.Text.Substring(i1);
		}

		private void TbCmd_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		//-----------
		// LstTbCmd
		//-----------
		private void LstTbCmd_Click(object sender, EventArgs e)
		{
			LstTbCmd.Visible = false;

			string s1 = LstTbCmd.SelectedItem.ToString();
			if (s1 != "[×]")
			{
				GblTbCmdPos += s1.Length;
				TbCmd.Text = TbCmd.Text.Substring(0, TbCmd.SelectionStart) + s1 + TbCmd.Text.Substring(TbCmd.SelectionStart);
			}
			SubTbCmdFocus(GblTbCmdPos);

			TbCmd.ScrollToCaret();
		}

		private void LstTbCmd_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.Enter:
				case Keys.Space:
					LstTbCmd_Click(sender, e);
					break;
			}
		}

		private void LstTbCmd_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();

			if (e.Index > -1)
			{
				string s1 = LstTbCmd.Items[e.Index].ToString();
				Brush brsh = (e.State & DrawItemState.Selected) != DrawItemState.Selected
					? s1 == "[×]"
						? new SolidBrush(Color.Red)
						: s1.Substring(s1.Length - 1, 1) == @"\"
							? new SolidBrush(Color.Cyan)
							: new SolidBrush(Color.Lime)
					: new SolidBrush(e.ForeColor);
				e.Graphics.DrawString(s1, e.Font, brsh, e.Bounds);
				brsh.Dispose();
			}

			e.DrawFocusRectangle();
		}

		//---------
		// CmsCmd
		//---------
		private string GblCmsCmdBatch = "";

		private void CmsCmd_左へ_Click(object sender, EventArgs e)
		{
			TbCmd.SelectionStart = 0;
			TbCmd.ScrollToCaret();
		}

		private void CmsCmd_右へ_Click(object sender, EventArgs e)
		{
			TbCmd.SelectionStart = TbCmd.TextLength;
			TbCmd.ScrollToCaret();
		}

		private void CmsCmd_全クリア_Click(object sender, EventArgs e)
		{
			TbCmd.Text = "";
		}

		private void CmsCmd_全コピー_Click(object sender, EventArgs e)
		{
			TbCmd.SelectAll();
			TbCmd.Copy();
		}

		private void CmsCmd_コピー_Click(object sender, EventArgs e)
		{
			TbCmd.Copy();
		}

		private void CmsCmd_切り取り_Click(object sender, EventArgs e)
		{
			TbCmd.Cut();
		}

		private void CmsCmd_貼り付け_Click(object sender, EventArgs e)
		{
			TbCmd.Paste();
			TbCmd.Text = Regex.Replace(TbCmd.Text, "\r*\n", " ");
		}

		private void CmsCmd_DQで囲む_Click(object sender, EventArgs e)
		{
			TbCmd.SelectedText = "\"" + TbCmd.SelectedText.Trim('\"') + "\"";
		}

		private void CmsCmd_コマンドをグループ化_追加_Click(object sender, EventArgs e)
		{
			string s1 = TbCmd.Text.Trim();
			if (s1.Length == 0)
			{
				return;
			}
			GblCmsCmdBatch += s1 + "; ";
			CmsCmd_コマンドをグループ化_追加.ToolTipText = CmsCmd_コマンドをグループ化_出力.ToolTipText = GblCmsCmdBatch;
		}

		private void CmsCmd_コマンドをグループ化_出力_Click(object sender, EventArgs e)
		{
			TbCmd.Text = GblCmsCmdBatch.Trim();
			SubTbCmdFocus(-1);
		}

		private void CmsCmd_コマンドをグループ化_消去_Click(object sender, EventArgs e)
		{
			GblCmsCmdBatch = CmsCmd_コマンドをグループ化_追加.ToolTipText = CmsCmd_コマンドをグループ化_出力.ToolTipText = "";
		}

		private void CmsCmd_コマンドをグループ化_簡単な説明_Click(object sender, EventArgs e)
		{
			_ = NativeMethods.SendMessage(
				TbResult.Handle, EM_REPLACESEL, 1,
				NL +
				"--------------" + NL +
				"> 簡単な説明 <" + NL +
				"--------------" + NL +
				"・\"//\" から始まる行はコメント" + NL +
				"・行末には \";\" を記述" + NL +
				NL +
				"【記述例】" + NL +
				"  // サンプル;" + NL +
				"  dir /s;" + NL +
				"  #grep \"2020\";" + NL +
				"  #replace \"20(20)\" \"$1\";" + NL +
				NL +
				"【コマンド欄には以下のように入力】" + NL +
				"  // サンプル; dir /s; #grep \"2020\"; #replace \"20(20)\" \"$1\";" + NL +
				NL
			);
		}

		private void CmsCmd_コマンドを保存_Click(object sender, EventArgs e)
		{
			SubTextToSaveFile(string.Join(";" + NL, RtnCmdFormat(TbCmd.Text).ToArray()) + NL, TEXT_CODE[0]);
		}

		private void CmsCmd_コマンドを読込_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog1 = new OpenFileDialog()
			{
				InitialDirectory = ".",
				Filter = FILE_FILTER,
				FilterIndex = 1
			}
			)
			{
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					TbCmd.Text = Regex.Replace(
						File.ReadAllText(openFileDialog1.FileName, Encoding.GetEncoding(TEXT_CODE[0])),
						@";*\r*\n", // ";" がなくても改行されていれば有効
						"; "
					);
					SubTbCmdFocus(-1);
				}
			}
		}

		private void CmsCmd_キー操作_Click(object sender, EventArgs e)
		{
			_ = NativeMethods.SendMessage(
				TbResult.Handle, EM_REPLACESEL, 1,
				NL +
				"--------------------" + NL +
				"> キー操作について <" + NL +
				"--------------------" + NL +
				" [Home]     カーソルを「先頭」へ移動" + NL +
				" [End]      カーソルを「末尾」へ移動" + NL +
				" [PageUp]   カーソル位置より「前」の文字列を削除" + NL +
				" [PageDown] カーソル位置から「後」の文字列を削除" + NL +
				" [↑]       カーソル位置より前の空白を検索" + NL +
				" [↓]       カレントのフォルダ／ファイル一覧を表示" + NL +
				NL
			);
		}

		//-----------
		// TbCmdSub
		//-----------
		private void TbCmdSub_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					SubTbCmdFocus(-1);
					break;

				case Keys.Up:
					TbCmdSub.Select(0, 0);
					break;

				case Keys.Down:
					TbCmdSub.Select(TbCmdSub.TextLength, 0);
					break;
			}
		}

		private void SubCmdSubAddText(string str)
		{
			_ = NativeMethods.SendMessage(TbCmdSub.Handle, EM_REPLACESEL, 1, str + NL);
			TbCmdSub.SelectionStart = TbCmdSub.TextLength;
			TbCmdSub.ScrollToCaret();
		}

		//------------
		// CmsCmdSub
		//------------
		private void CmsCmdSub_全クリア_Click(object sender, EventArgs e)
		{
			TbCmdSub.Text = "";
		}

		private void CmsCmdSub_全コピー_Click(object sender, EventArgs e)
		{
			TbCmdSub.SelectAll();
			TbCmdSub.Copy();
		}

		private void CmsCmdSub_コピー_Click(object sender, EventArgs e)
		{
			TbCmdSub.Copy();
		}

		private void CmsCmdSub_切り取り_Click(object sender, EventArgs e)
		{
			TbCmdSub.Cut();
		}

		private void CmsCmdSub_貼り付け_Click(object sender, EventArgs e)
		{
			TbCmdSub.Paste();
		}

		//-----------
		// DgvMacro
		//-----------
		private bool GblDgvMacroOpen = true;

		private void BtnDgvMacro_Click(object sender, EventArgs e)
		{
			if (GblDgvMacroOpen == true)
			{
				GblDgvMacroOpen = false;
				BtnDgvMacro.BackColor = Color.LightYellow;

				DgvMacro.Enabled = false;
				DgvMacro.ScrollBars = ScrollBars.None;
				DgvMacro.Width = 68;
				DgvMacro.Height = 23;
			}
			else
			{
				GblDgvMacroOpen = true;
				BtnDgvMacro.BackColor = Color.Gold;

				DgvMacro.Enabled = true;
				DgvMacro.ScrollBars = ScrollBars.Vertical;
				DgvMacro.Width = 369;
				DgvMacro.Height = 305;
			}
		}

		private void DgvMacro_Enter(object sender, EventArgs e)
		{
			Lbl_F2.ForeColor = Color.Gold;
		}

		private void DgvMacro_Leave(object sender, EventArgs e)
		{
			Lbl_F2.ForeColor = Color.Gray;
		}

		private void DgvMacro_MouseHover(object sender, EventArgs e)
		{
			_ = DgvMacro.Focus();
		}

		private void DgvMacro_Click(object sender, EventArgs e)
		{
			string s1 = DgvMacro[0, DgvMacro.CurrentRow.Index].Value.ToString();
			int iPos = 0;

			for (int _i1 = 0; _i1 < CMD.Length; _i1++)
			{
				if (CMD[_i1, 0].ToString() == s1)
				{
					iPos = (int)CMD[_i1, 2];
					for (int _i3 = 0; _i3 < iPos; _i3++)
					{
						s1 += " \"\"";
					}
					iPos = s1.Length - (iPos > 0 ? (iPos * 3) - 2 : 0);
					break;
				}
			}
			TbCmd.Text = s1;
			SubTbCmdFocus(iPos);

			GblDgvMacroOpen = true;
			BtnDgvMacro_Click(sender, e);
		}

		private int GblDgvMacroRow = 0;

		private void DgvMacro_KeyDown(object sender, KeyEventArgs e)
		{
			GblDgvMacroRow = DgvMacro.CurrentRow.Index;

			switch (e.KeyCode)
			{
				case Keys.Enter:
					SubCmdSubAddText("[Click]または[Space]キーで選択" + NL);
					break;
			}
		}

		private void DgvMacro_KeyUp(object sender, KeyEventArgs e)
		{
			int i1 = DgvMacro.RowCount - 1;

			switch (e.KeyCode)
			{
				case Keys.F2:
					BtnDgvMacro_Click(sender, e);
					break;

				case Keys.Space:
					DgvMacro_Click(sender, e);
					break;

				case Keys.Up:
					if (--GblDgvMacroRow < 0)
					{
						GblDgvMacroRow = i1;
					}
					break;

				case Keys.Down:
					if (++GblDgvMacroRow > i1)
					{
						GblDgvMacroRow = 0;
					}
					break;

				case Keys.Left:
					GblDgvMacroRow = GblDgvMacroRow == 0 ? i1 : 0;
					break;

				case Keys.Right:
					GblDgvMacroRow = GblDgvMacroRow == i1 ? 0 : i1;
					break;

				case Keys.PageUp:
					GblDgvMacroRow = GblDgvMacroRow == 0 ? i1 : DgvMacro.CurrentRow.Index;
					break;

				case Keys.PageDown:
					GblDgvMacroRow = GblDgvMacroRow == i1 ? 0 : DgvMacro.CurrentRow.Index;
					break;
			}

			DgvMacro.CurrentCell = DgvMacro[0, GblDgvMacroRow];
		}

		//---------
		// DgvCmd
		//---------
		private bool GblDgvCmdOpen = true;

		private void BtnDgvCmd_Click(object sender, EventArgs e)
		{
			if (GblDgvCmdOpen == true)
			{
				GblDgvCmdOpen = false;
				BtnDgvCmd.BackColor = Color.LightYellow;

				DgvCmd.Enabled = false;
				DgvCmd.ScrollBars = ScrollBars.None;
				DgvCmd.Width = 68;
				DgvCmd.Height = 23;
				TbDgvCmdSearch.Visible = false;
			}
			else
			{
				GblDgvCmdOpen = true;
				BtnDgvCmd.BackColor = Color.Gold;

				DgvCmd.Enabled = true;
				DgvCmd.ScrollBars = ScrollBars.Both;
				DgvCmd.Width = 286;
				DgvCmd.Height = 305;
				TbDgvCmdSearch.Visible = true;
			}
		}

		private void DgvCmd_Enter(object sender, EventArgs e)
		{
			Lbl_F3.ForeColor = Color.Gold;
		}

		private void DgvCmd_Leave(object sender, EventArgs e)
		{
			Lbl_F3.ForeColor = Color.Gray;
		}

		private void DgvCmd_MouseHover(object sender, EventArgs e)
		{
			_ = DgvCmd.Focus();
		}

		private void DgvCmd_Click(object sender, EventArgs e)
		{
			TbCmd.Text = DgvCmd[0, DgvCmd.CurrentCell.RowIndex].Value.ToString();
			SubTbCmdFocus(TbCmd.TextLength);

			GblDgvCmdOpen = true;
			BtnDgvCmd_Click(sender, e);
		}

		private int GblDgvCmdCurRow = 0;

		private void DgvCmd_KeyDown(object sender, KeyEventArgs e)
		{
			GblDgvCmdCurRow = DgvCmd.CurrentRow.Index;

			switch (e.KeyCode)
			{
				case Keys.Enter:
					SubCmdSubAddText("[Click]または[Space]キーで選択" + NL);
					break;
			}
		}

		private void DgvCmd_KeyUp(object sender, KeyEventArgs e)
		{
			int i1 = DgvCmd.RowCount - 1;

			switch (e.KeyCode)
			{
				case Keys.F3:
					BtnDgvCmd_Click(sender, e);
					break;

				case Keys.Space:
					DgvCmd_Click(sender, e);
					break;

				case Keys.Up:
					if (--GblDgvCmdCurRow < 0)
					{
						GblDgvCmdCurRow = i1;
					}
					break;

				case Keys.Down:
					if (++GblDgvCmdCurRow > i1)
					{
						GblDgvCmdCurRow = 0;
					}
					break;

				case Keys.Left:
					GblDgvCmdCurRow = GblDgvCmdCurRow == 0 ? i1 : 0;
					break;

				case Keys.Right:
					GblDgvCmdCurRow = GblDgvCmdCurRow == i1 ? 0 : i1;
					break;

				case Keys.PageUp:
					GblDgvCmdCurRow = GblDgvCmdCurRow == 0 ? i1 : DgvCmd.CurrentRow.Index;
					break;

				case Keys.PageDown:
					GblDgvCmdCurRow = GblDgvCmdCurRow == i1 ? 0 : DgvCmd.CurrentRow.Index;
					break;

				case Keys.Tab:
					_ = TbDgvCmdSearch.Focus();
					break;
			}
			DgvCmd.CurrentCell = DgvCmd[0, GblDgvCmdCurRow];
		}

		private IEnumerable<string> IeFile = null;

		private void SubDgvCmdLoad()
		{
			List<string> l1 = new List<string>();

			foreach (string _s1 in Environment.GetEnvironmentVariable("Path").ToLower().Replace("/", "\\").Split(';'))
			{
				string _s2 = _s1.TrimEnd('\\');
				if (_s2.Length > 0)
				{
					l1.Add(_s2);
				}
			}

			l1.Sort();
			IEnumerable<string> ie1 = l1.Distinct(StringComparer.InvariantCultureIgnoreCase);

			List<string> l2 = new List<string>();

			foreach (string _s1 in ie1)
			{
				DirectoryInfo DI = new DirectoryInfo(_s1);

				if (DI.Exists)
				{
					foreach (FileInfo _fi1 in DI.GetFiles("*", SearchOption.TopDirectoryOnly))
					{
						if (Regex.IsMatch(_fi1.FullName, @"\.(exe|bat|cmd)$", RegexOptions.IgnoreCase))
						{
							l2.Add(Path.GetFileName(_fi1.FullName));
						}
					}
				}
			}

			l2.Sort();
			IeFile = l2.Distinct(StringComparer.InvariantCultureIgnoreCase);

			foreach (string _s1 in IeFile)
			{
				_ = DgvCmd.Rows.Add(_s1);
			}
		}

		private void TbDgvCmdSearch_MouseHover(object sender, EventArgs e)
		{
			_ = TbDgvCmdSearch.Focus();
		}

		private void TbDgvCmdSearch_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Up:
				case Keys.Down:
					_ = DgvCmd.Focus();
					break;
			}
		}

		private void TbDgvCmdSearch_TextChanged(object sender, EventArgs e)
		{
			DgvCmd.Rows.Clear();

			foreach (string _s1 in IeFile)
			{
				if (Regex.IsMatch(_s1, TbDgvCmdSearch.Text, RegexOptions.IgnoreCase))
				{
					_ = DgvCmd.Rows.Add(_s1);
				}
			}

			Thread.Sleep(250);
		}

		//-------------
		// CbTextCode
		//-------------
		private void CbTextCode_Enter(object sender, EventArgs e)
		{
			Lbl_F4.ForeColor = Color.Gold;
		}

		private void CbTextCode_Leave(object sender, EventArgs e)
		{
			Lbl_F4.ForeColor = Color.Gray;
		}

		private void CbTextCode_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F4:
					CbTextCode.DroppedDown = false;
					SubTbCmdFocus(GblTbCmdPos);
					break;
			}
		}

		//-------
		// 実行
		//-------
		private void SubTbCmdExec(string cmd)
		{
			cmd = cmd.Trim();

			if (cmd.Length == 0)
			{
				return;
			}

			LblWait.Left = (Width - LblWait.Width - 24) / 2;
			LblWait.Top = (Height - LblWait.Height) / 2;
			LblWait.Visible = true;
			Refresh();

			Cursor.Current = Cursors.WaitCursor;

			// 変数
			Regex rgx = null;
			Match match = null;
			string s1 = "";
			int i1 = 0;

			// 変換 コマンド => マクロ
			cmd = Regex.Replace(cmd, @"^#*(cd|chdir)\s+", "#cd ", RegexOptions.IgnoreCase);

			// コメント行
			if (cmd.Substring(0, 2) == "//")
			{
				; // 何もしない
			}
			// マクロ実行
			else if (cmd[0] == '#')
			{
				const int aOpMax = 3;
				string[] aOp = Enumerable.Repeat("", aOpMax).ToArray();

				cmd += " "; // 検索用フラグ " " 付与
 
				// #command 取得
				rgx = new Regex("^(?<pattern>.+?)\\s", RegexOptions.None);
				match = rgx.Match(cmd);
				aOp[0] = match.Groups["pattern"].Value;

				// option[n] 取得
				// "\"" 対応
				i1 = 0;
				rgx = new Regex("\"(?<pattern>.*?)\"\\s+", RegexOptions.None);
				for (match = rgx.Match(cmd); match.Success; match = match.NextMatch())
				{
					++i1;
					if (i1 >= aOpMax)
					{
						break;
					}
					aOp[i1] = match.Groups["pattern"].Value;
				}

				List<string> lTmp = new List<string>();

				// 大小区別しない
				switch (aOp[0].ToLower())
				{
					// 出力消去
					case "#clear":
						BtnAllClear_Click(null, null);
						break;

					// フォルダ変更
					case "#cd":
						if (aOp[1].Length == 0)
						{
							SubCmdSubAddText("(例１) #cd \"..\"" + NL + "(例２) #cd \"NewDir\"");
							break;
						}
						string sCd = Path.GetFullPath(aOp[1]);
						try
						{
							Directory.SetCurrentDirectory(sCd);
						}
						catch
						{
							SubCmdSubAddText("Dir \"" + aOp[1] + "\" は存在しない");
							break;
						}
						TbCurDir.Text = Path.GetFullPath(sCd);
						SubCmdSubAddText("\"" + aOp[1] + "\" へ移動 => \"" + TbCurDir.Text + "\"");
						break;

					// 文字コード（Batchで使用）
					case "#code":
						if(aOp[1].Length == 0)
						{
							SubCmdSubAddText("引数：\"文字コード\"" + NL + "※ \"Shift_JIS\", \"UTF-8\"");
							break;
						}
						CbTextCode.Text = aOp[1];
						break;

					// 検索（一致）
					case "#grep":
						TbResult.Text = RtnTextGrep(TbResult.Text, aOp[1], true);
						break;

					// 検索（不一致）
					case "#except":
						TbResult.Text = RtnTextGrep(TbResult.Text, aOp[1], false);
						break;

					// 置換
					case "#replace":
						TbResult.Text = RtnTextReplace(TbResult.Text, aOp[1], aOp[2]);
						break;

					// 改行を置換
					case "#repnl":
						TbResult.Text = RtnTextReplaceNL(TbResult.Text, aOp[1], aOp[2]);
						break;

					// 分割変換（like AWK）
					case "#split":
						TbResult.Text = RtnTextSplitCnv(TbResult.Text, aOp[1], aOp[2]);
						break;

					// 全角変換
					case "#towide":
						TbResult.Text = Strings.StrConv(TbResult.Text, VbStrConv.Wide, 0x411);
						break;

					// 全角変換(数字のみ)
					case "#tozennum":
						TbResult.Text = RtnZenNum(TbResult.Text);
						break;

					// 全角変換(カナのみ)
					case "#tozenkana":
						TbResult.Text = RtnZenKana(TbResult.Text);
						break;

					// 半角変換
					case "#tonarrow":
						TbResult.Text = Strings.StrConv(TbResult.Text, VbStrConv.Narrow, 0x411);
						break;

					// 半角変換(数字のみ)
					case "#tohannum":
						TbResult.Text = RtnHanNum(TbResult.Text);
						break;

					// 半角変換(カナのみ)
					case "#tohankana":
						TbResult.Text = RtnHanKana(TbResult.Text);
						break;

					// 大文字変換
					case "#toupper":
						TbResult.Text = TbResult.Text.ToUpper();
						break;

					// 小文字変換
					case "#tolower":
						TbResult.Text = TbResult.Text.ToLower();
						break;

					// 消去
					case "#erase":
						TbResult.Text = RtnTextEraseInLine(TbResult.Text, aOp[1], aOp[2]);
						break;

					// ソート(昇順)
					case "#sort":
						TbResult.Text = RtnTextSort(TbResult.Text, true);
						break;

					// ソート(降順)
					case "#sort-r":
						TbResult.Text = RtnTextSort(TbResult.Text, false);
						break;

					// 重複行を消去
					case "#uniq":
						TbResult.Text = RtnTextUniq(TbResult.Text);
						break;

					// 指定行取得
					case "#getln":
						int bgnLn = 1;
						int endLn = 0;
						try
						{
							bgnLn = int.Parse(aOp[1]);
							endLn = int.Parse(aOp[2]);
						}
						catch
						{
							SubCmdSubAddText("引数：\"開始行\" \"終了行\"" + NL + "※終了行 \"0\" のときは最終行まで");
							break;
						}
						int cnt2 = 0;
						_ = Sb.Clear();
						foreach (string _s1 in TbResult.Text.Split(SPLITS, StringSplitOptions.None))
						{
							++cnt2;
							if (cnt2 >= bgnLn)
							{
								_ = Sb.Append(_s1 + NL);
							}
							if (endLn > 0 && cnt2 >= endLn)
							{
								break;
							}
						}
						TbResult.Text = Sb.ToString();
						break;

					// 空白行削除
					case "#rmblankln":
						_ = Sb.Clear();
						foreach (string _s1 in TbResult.Text.Split(SPLITS, StringSplitOptions.None))
						{
							if (_s1.TrimEnd().Length > 0)
							{
								_ = Sb.Append(_s1 + NL);
							}
						}
						TbResult.Text = Sb.ToString();
						break;

					// 行番号付与
					case "#addlnnum":
						int cnt1 = 0;
						_ = Sb.Clear();
						foreach (string _s1 in TbResult.Text.Split(SPLITS, StringSplitOptions.None))
						{
							_ = Sb.Append(string.Format("{0:D8}\t{1}{2}", ++cnt1, _s1, NL));
						}
						TbResult.Text = Sb.ToString();
						break;

					// ファイル取得／ファイル読込
					case "#wget":
					case "#fread":
						if (aOp[1].Length == 0)
						{
							break;
						}
						using (System.Net.WebClient wc = new System.Net.WebClient())
						{
							try
							{
								s1 = Encoding.GetEncoding(CbTextCode.Text).GetString(wc.DownloadData(aOp[1]));
								if (Regex.IsMatch(aOp[1], "^(http|ftp)"))
								{
									if (Regex.IsMatch(s1, "charset.*=.*UTF-8", RegexOptions.IgnoreCase))
									{
										CbTextCode.Text = TEXT_CODE[1];
									}
								}
								_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, Regex.Replace(s1, "\r*\n", NL));
							}
							catch
							{
							}
						}
						break;

					// ファイル書込
					case "#fwrite":
						if (aOp[1].Length == 0)
						{
							break;
						}
						switch (CbTextCode.Text.ToUpper())
						{
							case "UTF-8":
								using (StreamWriter sw = new StreamWriter(aOp[1], false, new UTF8Encoding(false)))
								{
									sw.Write(TbResult.Text);
								}
								break;

							default:
								using (StreamWriter sw = new StreamWriter(aOp[1], false, Encoding.GetEncoding(TEXT_CODE[0])))
								{
									sw.Write(TbResult.Text);
								}
								break;
						}
						break;

					// 行毎に処理
					case "#stream":
						if (aOp[1].Length == 0)
						{
							SubCmdSubAddText("出力結果から行毎に読み込んで処理" + NL + "引数：\"コマンド\" \"オプション\"");
							break;
						}
						MyEvent = new MyEventHandler(EventDataReceived);
						foreach (string _s1 in TbResult.Text.Split(SPLITS, StringSplitOptions.None))
						{
							if (_s1.Trim().Length > 0)
							{
								Ps1 = new Process();
								Ps1.StartInfo.FileName = aOp[1];
								Ps1.StartInfo.Arguments = aOp[2] + " " + _s1;
								Ps1.StartInfo.UseShellExecute = false;
								Ps1.StartInfo.RedirectStandardOutput = true;
								Ps1.StartInfo.RedirectStandardError = true;
								Ps1.StartInfo.CreateNoWindow = true;
								Ps1.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CbTextCode.Text);
								Ps1.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);
								_ = Ps1.Start();
								///TbResult.Text += Ps1.StandardOutput.ReadToEnd() + NL;
								TbCmdSub.Text += Ps1.StandardError.ReadToEnd() + NL;
								Ps1.Close();
							}
						}
						TbCmdSub.SelectionStart = TbCmdSub.TextLength;
						TbCmdSub.ScrollToCaret();
						break;

					// MsgBox
					case "#msgbox":
						DialogResult result = MessageBox.Show(aOp[1].Replace("\\n", NL), "", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
						GblCmdExec = result == DialogResult.Yes || result == DialogResult.OK ? true : false;
						break;

					// 計算機
					case "#calc":
						SubCmdSubAddText(RtnEvalCalc(aOp[1]) + NL);
						break;

					// 現在日時
					case "#now":
						SubCmdSubAddText(DateTime.Now.ToString("yyyy/MM/dd(ddd) HH:mm:ss") + NL);
						break;

					// バージョン
					case "#version":
						SubCmdSubAddText(VERSION + NL);
						break;
				}
			}
			// コマンド実行
			else
			{
				MyEvent = new MyEventHandler(EventDataReceived);
				Ps1 = new Process();
				Ps1.StartInfo.FileName = "cmd.exe";
				Ps1.StartInfo.Arguments = "/C " + cmd;
				Ps1.StartInfo.UseShellExecute = false;
				Ps1.StartInfo.RedirectStandardOutput = true;
				Ps1.StartInfo.RedirectStandardError = true;
				Ps1.StartInfo.CreateNoWindow = true;
				Ps1.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CbTextCode.Text);
				Ps1.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);
				_ = Ps1.Start();
				TbResult.Text += Ps1.StandardOutput.ReadToEnd();
				Ps1.Close();
				TbResult.SelectionStart = TbResult.TextLength;
				TbResult.ScrollToCaret();
			}

			LblWait.Visible = false;
			Cursor.Current = Cursors.Default;
		}

		private void EventDataReceived(object sender, DataReceivedEventArgs e)
		{
			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, e.Data + NL);
		}

		private void ProcessDataReceived(object sender, DataReceivedEventArgs e)
		{
			_ = Invoke(MyEvent, new object[2] { sender, e });
		}

		//-------
		// 実行
		//-------
		private bool GblCmdExec = true;

		private void BtnCmdExec_Click(object sender, EventArgs e)
		{
			TbResult_Enter(sender, e);

			// 履歴に追加
			CmdHistory.Add(TbCmd.Text.Trim());
			SubListHistory(CmdHistory, CbCmdHistory);

			GblCmdExec = true;
			foreach (string _s1 in RtnCmdFormat(TbCmd.Text))
			{
				if (GblCmdExec == false)
				{
					break;
				}
				SubTbCmdExec(_s1);
			}
			SubTbCmdFocus(GblTbCmdPos);
		}

		private List<string> RtnCmdFormat(string str)
		{
			List<string> rtn = new List<string>();

			int flg = 0;
			int iBgnPos = 0;
			int iEndLen = 0;
			string s1;

			for (int _i1 = 0; _i1 < str.Length; _i1++)
			{
				++iEndLen;

				if (str[_i1] == '"' && ++flg == 2)
				{
					if (str[_i1 - 1] != '\\')
					{
						flg = 0;
					}
				}

				if (flg == 0 && str[_i1] == ';')
				{
					s1 = str.Substring(iBgnPos, iEndLen - 1).Trim();
					if (s1.Length > 0)
					{
						rtn.Add(s1);
					}
					iBgnPos += iEndLen;
					iEndLen = 0;
				}
			}

			s1 = Regex.Replace(str.Substring(iBgnPos, iEndLen).Trim(), @"[\s;]*$", "");
			if (s1.Length > 0)
			{
				rtn.Add(s1);
			}

			return rtn;
		}

		//-------------
		// 入出力消去
		//-------------
		private void BtnAllClear_Click(object sender, EventArgs e)
		{
			/// TbCmd.Text = "";
			TbCmdSub.Text = "";
			TbResult.Text = "";
			SubTbCmdFocus(-1);
		}

		//-----------
		// TbResult
		//-----------
		private void TbResult_Enter(object sender, EventArgs e)
		{
			GblDgvMacroOpen = true;
			BtnDgvMacro_Click(sender, e);

			GblDgvCmdOpen = true;
			BtnDgvCmd_Click(sender, e);
		}

		private void TbResult_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.PageUp:
					TbResult.SelectionStart = 0;
					break;

				case Keys.PageDown:
					TbResult.SelectionStart = TbResult.TextLength;
					break;

				default:
					TbResult_MouseUp(sender, null);
					break;
			}
		}

		private void TbResult_MouseUp(object sender, MouseEventArgs e)
		{
			if (TbResult.SelectionLength == 0)
			{
				TbResultInfo.Text = "";
				return;
			}

			int iNL = 0;
			int iRow = 0;
			int iCnt = 0;

			foreach (string _s1 in TbResult.SelectedText.Split(SPLITS, StringSplitOptions.None))
			{
				++iNL;

				if (_s1.Trim().Length > 0)
				{
					++iRow;
				}

				iCnt += _s1.Length;
			}

			TbResultInfo.Text = string.Format("{0}字(有効{1}行／全{2}行)選択", iCnt, iRow, iNL);
		}

		private void TbResult_TextChanged(object sender, EventArgs e)
		{
			TbResultInfo.Text = "";
		}

		private void TbResult_DragDrop(object sender, DragEventArgs e)
		{
			_ = Sb.Clear();

			foreach (string _s1 in (string[])e.Data.GetData(DataFormats.FileDrop, false))
			{
				foreach (string _s2 in File.ReadLines(_s1, Encoding.GetEncoding(CbTextCode.Text)))
				{
					_ = Sb.Append(_s2.TrimEnd() + NL);
				}
			}

			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, Sb.ToString());
		}

		private void TbResult_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		//------------
		// CmsResult
		//------------
		private void CmsResult_上へ_Click(object sender, EventArgs e)
		{
			TbResult.SelectionStart = 0;
			TbResult.ScrollToCaret();
		}

		private void CmsResult_下へ_Click(object sender, EventArgs e)
		{
			TbResult.SelectionStart = TbResult.TextLength;
			TbResult.ScrollToCaret();
		}

		private void CmsResult_全選択_Click(object sender, EventArgs e)
		{
			TbResult.SelectAll();
			TbResult_MouseUp(sender, null);
		}

		private void CmsResult_全クリア_Click(object sender, EventArgs e)
		{
			TbResult.Text = "";
		}

		private void CmsResult_全コピー_Click(object sender, EventArgs e)
		{
			TbResult.SelectAll();
			TbResult.Copy();
		}

		private void CmsResult_コピー_Click(object sender, EventArgs e)
		{
			TbResult.Copy();
		}

		private void CmsResult_切り取り_Click(object sender, EventArgs e)
		{
			TbResult.Cut();
		}

		private void CmsResult_貼り付け_Click(object sender, EventArgs e)
		{
			_ = NativeMethods.SendMessage(
				TbResult.Handle, EM_REPLACESEL, 1,
				Regex.Replace(Clipboard.GetText(), "\r*\n", NL)
			);
		}

		private void CmsResult_ファイル名を貼り付け_Click(object sender, EventArgs e)
		{
			_ = Sb.Clear();

			foreach (string _s1 in Clipboard.GetFileDropList())
			{
				_ = Sb.Append(_s1 + (Directory.Exists(_s1) ? @"\" : "") + NL);
			}

			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, Sb.ToString());
		}

		private void CmsResult_名前を付けて保存_Click(object sender, EventArgs e)
		{
			SubTextToSaveFile(TbResult.Text, CbTextCode.Text);
		}

		//-------
		// 履歴
		//-------
		private void CbCmdHistory_Enter(object sender, EventArgs e)
		{
			Lbl_F1.ForeColor = Color.Gold;
		}

		private void CbCmdHistory_Leave(object sender, EventArgs e)
		{
			Lbl_F1.ForeColor = Color.Gray;
		}

		private void CbCmdHistory_DropDownClosed(object sender, EventArgs e)
		{
			if (CbCmdHistory.Text.Length > 0)
			{
				TbCmd.Text = CbCmdHistory.Text;
			}
			SubTbCmdFocus(GblTbCmdPos);
		}

		private void CbCmdHistory_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F1:
					CbCmdHistory.Text = "";
					CbCmdHistory.DroppedDown = false;
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.Enter:
				case Keys.Space:
					SubTbCmdFocus(GblTbCmdPos + CbCmdHistory.Text.Length);
					break;
			}
		}

		private void CbResultHistory_Enter(object sender, EventArgs e)
		{
			Lbl_F7.ForeColor = Color.Gold;
		}

		private void CbResultHistory_Leave(object sender, EventArgs e)
		{
			Lbl_F7.ForeColor = Color.Gray;
		}

		private void CbResultHistory_DropDownClosed(object sender, EventArgs e)
		{
			if (CbResultHistory.Text.Length > "xx:xx:xx|".Length)
			{
				TbResult.Text = ResultHistory[CbResultHistory.Text.Substring(0, 8)];
			}
			SubTbCmdFocus(GblTbCmdPos);
		}

		private void CbResultHistory_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F7:
					CbResultHistory.DroppedDown = false;
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.Enter:
				case Keys.Space:
					SubTbCmdFocus(GblTbCmdPos);
					break;
			}
		}

		private void SubListHistory(List<string> l, ComboBox cb)
		{
			List<string> LTmp = new List<string>
			{
				""
			};

			string flg = "";

			l.Sort();
			foreach (string _s1 in l)
			{
				if (_s1 != flg)
				{
					LTmp.Add(_s1);
					flg = _s1;
				}
			}

			cb.Items.Clear();

			l = LTmp;
			foreach (string _s1 in l)
			{
				_ = cb.Items.Add(_s1);
			}
		}

		private void SubDictHistory(Dictionary<string, string> h, ComboBox cb)
		{
			cb.Items.Clear();

			_ = cb.Items.Add("xx:xx:xx|");

			foreach (KeyValuePair<string, string> dict in h.OrderByDescending(dict => dict.Key))
			{
				string _s1 = dict.Value.Substring(0, dict.Value.Length > 30 ? 30 : dict.Value.Length);
				_ = cb.Items.Add(string.Format("{0}|{1}", dict.Key, _s1.Replace(NL, @"\n")));
			}
		}

		private void BtnResultMem_Click(object sender, EventArgs e)
		{
			if (TbResult.Text.Trim().Length == 0)
			{
				return;
			}

			// 同時刻のときエラー発生
			try
			{
				ResultHistory.Add(DateTime.Now.ToString("HH:mm:ss"), TbResult.Text);
				SubDictHistory(ResultHistory, CbResultHistory);
			}
			catch
			{
			}
		}

		//-----------------
		// フォントサイズ
		//-----------------
		private void NumericUpDown1_Enter(object sender, EventArgs e)
		{
			NumericUpDown1.ForeColor = Color.White;
			NumericUpDown1.BackColor = Color.RoyalBlue;

			Lbl_F9.ForeColor = Color.Gold;
		}

		private void NumericUpDown1_Leave(object sender, EventArgs e)
		{
			NumericUpDown1.ForeColor = Color.White;
			NumericUpDown1.BackColor = Color.DimGray;

			Lbl_F9.ForeColor = Color.Gray;
		}

		private void NumericUpDown1_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F9:
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.Enter:
				case Keys.Space:
					SubTbCmdFocus(GblTbCmdPos);
					break;
			}
		}

		private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (NumericUpDown1.Value < NumericUpDown1.Minimum)
			{
				NumericUpDown1.Value = NumericUpDown1.Minimum;
			}

			if (NumericUpDown1.Value > NumericUpDown1.Maximum)
			{
				NumericUpDown1.Value = NumericUpDown1.Maximum;
			}

			TbResult.Font = new Font(TbResult.Font.Name.ToString(), (float)NumericUpDown1.Value);
		}

		//-----------------------
		// TextBox へフォーカス
		//-----------------------
		// TbCmd
		private void SubTbCmdFocus(int iPos)
		{
			_ = TbCmd.Focus();
			if (iPos < 0 || iPos > TbCmd.TextLength)
			{
				TbCmd.Text = TbCmd.Text.Trim();
				iPos = TbCmd.TextLength;
			}
			TbCmd.Select(iPos, 0);
		}

		//-----------------------
		// contextMenuStrip操作
		//-----------------------
		// ファイル保存
		private void SubTextToSaveFile(string s, string code)
		{
			using (SaveFileDialog saveFileDialog1 = new SaveFileDialog
			{
				InitialDirectory = ".",
				FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt",
				Filter = FILE_FILTER,
				FilterIndex = 1
			}
			)
			{
				if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					switch (code.ToUpper())
					{
						case "UTF-8":
							File.WriteAllText(saveFileDialog1.FileName, s, new UTF8Encoding(false));
							break;

						default:
							File.WriteAllText(saveFileDialog1.FileName, s, Encoding.GetEncoding(TEXT_CODE[0]));
							break;
					}
				}
			}
		}

		//---------------------
		// 正規表現による検索
		//---------------------
		private string RtnTextGrep(string str, string sRgx, bool bMatch)
		{
			if (sRgx.Length == 0)
			{
				SubCmdSubAddText(@"正規表現による検索：\t \\ \. etc." + NL);
				return str;
			}

			Regex rgx;

			try
			{
				rgx = new Regex("(?i)" + sRgx, RegexOptions.Compiled);
			}
			catch
			{
				return str;
			}

			_ = Sb.Clear();

			foreach (string _s1 in str.Split(SPLITS, StringSplitOptions.None))
			{
				string _s2 = _s1 + NL;
				if (bMatch == rgx.IsMatch(_s2))
				{
					_ = Sb.Append(_s2);
				}
			}

			return Sb.ToString();
		}

		//---------------------
		// 正規表現による置換
		//---------------------
		private string RtnTextReplace(string str, string sOld, string sNew)
		{
			if (sOld.Length == 0)
			{
				SubCmdSubAddText(@"正規表現による置換：\t \\ \. $1,$2... etc." + NL);
				return str;
			}

			// 特殊文字を置換
			sNew = sNew.Replace("\\t", "\t");
			sNew = sNew.Replace("\\n", NL);
			sNew = sNew.Replace("\\\\", "\\");
			sNew = sNew.Replace("\\\"", "\"");
			sNew = sNew.Replace("\\\'", "\'");

			Regex rgx;

			try
			{
				rgx = new Regex("(?i)" + sOld, RegexOptions.Compiled);
			}
			catch
			{
				return str;
			}

			_ = Sb.Clear();

			foreach (string _s1 in str.Split(SPLITS, StringSplitOptions.None))
			{
				_ = Sb.Append(rgx.Replace(_s1, sNew) + NL);
			}

			return Sb.ToString();
		}

		//-------------------------
		// 正規表現による改行置換
		//-------------------------
		private string RtnTextReplaceNL(string str, string sNew, string sQrt)
		{
			if (sNew.Length == 0)
			{
				SubCmdSubAddText(@"正規表現による改行置換：\t \\ \. etc." + NL + "引数：\"区切り文字\" \"囲い文字\"");
				return str;
			}

			// 特殊文字を置換
			sNew = sNew.Replace("\\t", "\t");
			sNew = sNew.Replace("\\n", NL);
			sNew = sNew.Replace("\\\\", "\\");
			sNew = sNew.Replace("\\\"", "\"");
			sNew = sNew.Replace("\\\'", "\'");

			sQrt = sQrt.Replace("\\\"", "\"");
			sQrt = sQrt.Replace("\\\'", "\'");

			_ = Sb.Clear();

			foreach (string _s1 in str.Split(SPLITS, StringSplitOptions.None))
			{
				if (_s1.Trim().Length > 0)
				{
					_ = Sb.Append(sQrt + _s1 + sQrt + sNew);
				}
			}

			return Sb.ToString();
		}

		//---------------------
		// 正規表現による分割
		//---------------------
		private string RtnTextSplitCnv(string str, string sSplit, string sRgx)
		{
			if (sSplit.Length == 0 || sRgx.Length == 0)
			{
				SubCmdSubAddText(@"正規表現による分割：\t \\ \. [1],[2]... etc." + NL);
				return str;
			}

			// 特殊文字を置換
			sSplit = sSplit.Replace("|", "\\|");

			Regex rgx1, rgx2;

			try
			{
				rgx1 = new Regex(@"\[\d+\]", RegexOptions.Compiled);
				rgx2 = new Regex(sSplit, RegexOptions.Compiled);
			}
			catch
			{
				return str;
			}

			_ = Sb.Clear();

			foreach (string _s1 in str.Split(SPLITS, StringSplitOptions.None))
			{
				if (_s1.Length > 0)
				{
					string[] a1 = rgx2.Split(_s1);
					string _s2 = sRgx;

					for (int _i1 = 0; _i1 < a1.Length; _i1++)
					{
						_s2 = _s2.Replace("[" + _i1.ToString() + "]", a1[_i1]);

						// 特殊文字を置換
						_s2 = _s2.Replace("\\t", "\t");
						_s2 = _s2.Replace("\\n", NL);
						_s2 = _s2.Replace("\\\\", "\\");
						_s2 = _s2.Replace("\\\"", "\"");
						_s2 = _s2.Replace("\\\'", "\'");
					}

					// 該当なしの変換子を削除
					_ = Sb.Append(rgx1.Replace(_s2, "") + NL);
				}
			}

			return Sb.ToString();
		}

		//----------------
		// 全角 <=> 半角
		//----------------
		private static string RtnZenNum(string s)
		{
			Regex rgx = new Regex(@"\d+");
			return  rgx.Replace(s, RtnReplacerWide);
		}

		private static string RtnHanNum(string s)
		{
			// 全角０-９ Unicode
			Regex rgx = new Regex(@"[\uff10-\uff19]+");
			return rgx.Replace(s, RtnReplacerNarrow);
		}

		private static string RtnZenKana(string s)
		{
			// 半角カナ Unicode
			Regex rgx = new Regex(@"[\uff61-\uFF9f]+");
			return rgx.Replace(s, RtnReplacerWide);
		}

		private static string RtnHanKana(string s)
		{
			// 全角カナ Unicode
			Regex rgx = new Regex(@"[\u30A1-\u30F6]+");
			return rgx.Replace(s, RtnReplacerNarrow);
		}

		private static string RtnReplacerWide(Match m)
		{
			return Strings.StrConv(m.Value, VbStrConv.Wide, 0x411);
		}

		private static string RtnReplacerNarrow(Match m)
		{
			return Strings.StrConv(m.Value, VbStrConv.Narrow, 0x411);
		}

		//-----------
		// 文字消去
		//-----------
		private string RtnTextEraseInLine(string str, string sBgnPos, string sEndPos)
		{
			int iBgnPos;
			int iEndPos;

			try
			{
				iBgnPos = int.Parse(sBgnPos);
				iEndPos = int.Parse(sEndPos);
			}
			catch
			{
				SubCmdSubAddText("引数：\"開始位置\" \"文字長\"" + NL);
				return str;
			}

			_ = Sb.Clear();

			foreach (string _s1 in str.Split(SPLITS, StringSplitOptions.None))
			{
				_ = Sb.Append(RtnEraseLen(_s1, ' ', iBgnPos, iEndPos) + NL);
			}

			return Sb.ToString();
		}

		private string RtnEraseLen(string str, char cRep, int iBgnPos, int iLen)
		{
			// 範囲チェック
			if (str.Length == 0 || iBgnPos > str.Length || iLen <= 0)
			{
				return str;
			}

			int iEndPos = iBgnPos + iLen;

			// iEndPos の正位置は？
			if (iEndPos <= 0)
			{
				iEndPos += str.Length;

				if (iEndPos < 0)
				{
					return str;
				}
			}
			else if (iEndPos > str.Length)
			{
				iEndPos = str.Length;
			}

			// iBgnPos の正位置は？
			if (iBgnPos < 0)
			{
				iBgnPos += str.Length;

				if (iBgnPos < 0)
				{
					iBgnPos = 0;
				}
			}

			string rtn = "";

			// 前
			rtn += str.Substring(0, iBgnPos);

			// 中
			for (int _i1 = iBgnPos; _i1 < iEndPos; _i1++)
			{
				rtn += cRep;
			}

			// 後
			rtn += str.Substring(iEndPos, str.Length - iEndPos);

			return rtn;
		}

		//---------------
		// Sort／Sort-R
		//---------------
		private string RtnTextSort(string str, bool bAsc)
		{
			List<string> l1 = new List<string>();

			foreach (string _s1 in str.Split(SPLITS, StringSplitOptions.None))
			{
				string _s2 = _s1.TrimEnd();
				if (_s2.Length > 0)
				{
					l1.Add(_s2);
				}
			}

			l1.Sort();

			if (! bAsc)
			{
				l1.Reverse();
			}

			return string.Join(NL, l1) + NL;
		}

		//-------
		// Uniq
		//-------
		private string RtnTextUniq(string str)
		{
			_ = Sb.Clear();

			string flg = "";

			foreach (string _s1 in str.Split(SPLITS, StringSplitOptions.None))
			{
				if (_s1 != flg && _s1.TrimEnd().Length > 0)
				{
					_ = Sb.Append(_s1 + NL);
					flg = _s1;
				}
			}

			return Sb.ToString();
		}

		//-----------
		// Eval計算
		//-----------
		private string RtnEvalCalc(string str)
		{
			string rtn = str.ToLower().Replace(" ", "");

			// Help
			if (rtn.Length == 0)
			{
				return "pi, rad, pow(n,n), sqrt(n), sin(n°), cos(n°), tan(n°)";
			}

			double _degPerSec = Math.PI / 180;

			// 定数
			rtn = rtn.Replace("pi", Math.PI.ToString());
			rtn = rtn.Replace("rad", (180 / Math.PI).ToString());

			// sqrt(n) sin(n°) cos(n°) tan(n°)
			string[] aMath = { "sqrt", "sin", "cos", "tan" };
			foreach (string _s1 in aMath)
			{
				foreach (Match _m1 in Regex.Matches(rtn, _s1 + @"\(\d+\.*\d*\)"))
				{
					string _s2 = _m1.Value;
					_s2 = _s2.Replace(_s1 + "(", "");
					_s2 = _s2.Replace(")", "");

					double _d1 = double.Parse(_s2);

					switch (_s1)
					{
						case "sqrt": _d1 = Math.Sqrt(_d1); break;
						case "sin": _d1 = Math.Sin(_degPerSec * _d1); break;
						case "cos": _d1 = Math.Cos(_degPerSec * _d1); break;
						case "tan": _d1 = Math.Tan(_degPerSec * _d1); break;
						default: _d1 = 0; break;
					}

					rtn = rtn.Replace(_m1.Value, _d1.ToString());
				}
			}

			// pow(n,n)
			foreach (Match _m1 in Regex.Matches(rtn, @"pow\(\d+\.*\d*\,\d+\)"))
			{
				string _s2 = _m1.Value;
				_s2 = _s2.Replace("pow(", "");
				_s2 = _s2.Replace(")", "");

				string[] _a1 = _s2.Split(',');
				rtn = rtn.Replace(_m1.Value, Math.Pow(double.Parse(_a1[0]), int.Parse(_a1[1])).ToString());
			}

			using (DataTable dt = new DataTable())
			{
				try
				{
					rtn = dt.Compute(rtn, null).ToString();
				}
				catch
				{
					rtn = "Err";
				}
			}

			return rtn;
		}
	}
}
