using Microsoft.VisualBasic; // プロジェクト～参照の追加

using System;
using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace iwm_commandliner3
{
	public partial class Form1 : Form
	{
		//-----------
		// 大域定数
		//-----------
		private const string VERSION = "Ver.20201114 'A-29' (C)2018-2020 iwm-iwama";

		private const string NL = "\r\n";
		private readonly string RgxNL = "\r*\n";
		private readonly string[] AryNL = { NL };

		private readonly string[] TEXT_CODE = { "Shift_JIS", "UTF-8" };

		private readonly object[,] MACRO = {
			// [マクロ]     [説明]                                         [引数]
			{ "#clear",     "出力消去     #clear",                           0 },
			{ "#cd",        "フォルダ変更 #cd \"..\"",                       1 },
			{ "#code",      "文字コード   #code \"UTF-8\"",                  1 },
			{ "#grep",      "検索         #grep \"2018\"",                   1 },
			{ "#except",    "不一致検索   #except \"2018\"",                 1 },
			{ "#replace",   "置換         #replace \".*(\\d{4}).*\" \"$1\"", 2 },
			{ "#split",     "分割         #split \"\\t\" \"[0],[1]\"",       2 },
			{ "#toUpper",   "大文字に変換",                                  0 },
			{ "#toLower",   "小文字に変換",                                  0 },
			{ "#toWide",    "全角に変換",                                    0 },
			{ "#toZenNum",  "全角に変換(数字のみ)",                          0 },
			{ "#toZenKana", "全角に変換(カナのみ)",                          0 },
			{ "#toNarrow",  "半角に変換",                                    0 },
			{ "#toHanNum",  "半角に変換(数字のみ)",                          0 },
			{ "#toHanKana", "半角に変換(カナのみ)",                          0 },
			{ "#erase",     "文字消去     #erase \"0\" \"5\"",               2 },
			{ "#sort",      "ソート(昇順)",                                  0 },
			{ "#sort-r",    "ソート(降順)",                                  0 },
			{ "#uniq",      "重複行を消去",                                  0 },
			{ "#getLn",     "指定行取得   #getLn \"2\" \"0\"",               2 },
			{ "#rmBlankLn", "空白行削除",                                    0 },
			{ "#addLnNum",  "行番号付与",                                    0 },
			{ "#rmNL",      "改行を消去",                                    0 },
			{ "#wget",      "ファイル取得 #wget \"http://.../index.html\"",  1 },
			{ "#fread",     "ファイル読込 #fread \"ファイル名\"",            1 },
			{ "#fwrite",    "ファイル書込 #fwrite \"ファイル名\"",           1 },
			{ "#stream",    "行毎に処理   #stream \"wget\" \"-q\"",          2 },
			{ "#calc",      "計算機       #calc \"pi / 180\"",               1 },
			{ "#now",       "現在日時",                                      0 },
			{ "#version",   "バージョン",                                    0 },
			{ "#exit",      "終了",                                          0 }
		};

		private readonly string FILE_FILTER = "All files (*.*)|*.*|Command (*.iwmcmd)|*.iwmcmd|Text (*.txt)|*.txt|Tab-Separated (*.tsv)|*.tsv|Comma-Separated (*.csv)|*.csv|HTML (*.html,*.htm)|*.html,*.htm";

		//-----------
		// 大域変数
		//-----------
		// CurDir
		private string CurDir = "";

		// 文字列
		private readonly StringBuilder SB = new StringBuilder();

		// 履歴
		private readonly List<string> CmdHistory = new List<string>() { };
		private readonly Dictionary<string, string> ResultHistory = new Dictionary<string, string>();

		private delegate void MyEventHandler(object sender, DataReceivedEventArgs e);
		private event MyEventHandler MyEvent = null;

		// Object
		private Process PS = null;
		private TextBox TB = null;

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

			// DgvMacro／DgvCmd 表示
			for (int _i1 = 0; _i1 < MACRO.GetLength(0); _i1++)
			{
				_ = DgvMacro.Rows.Add(MACRO[_i1, 0], MACRO[_i1, 1]);
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

			// LblResultInfo
			TbResult_MouseUp(sender, null);

			// フォントサイズ
			NudTbResult.Value = (int)Math.Round(TbResult.Font.Size);

			// 初フォーカス
			SubTbCmdFocus(-1);
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
			if (GblDgvMacroOpen)
			{
				GblDgvMacroOpen = false;
				BtnDgvMacro_Click(sender, e);
			}

			if (GblDgvCmdOpen)
			{
				GblDgvCmdOpen = false;
				BtnDgvCmd_Click(sender, e);
			}
		}

		//-----------
		// TbCurDir
		//-----------
		private void TbCurDir_Enter(object sender, EventArgs e)
		{
			LblCurDir.Visible = true;
		}

		private void TbCurDir_Leave(object sender, EventArgs e)
		{
			LblCurDir.Visible = false;
		}

		private void TbCurDir_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = folderBrowserDialog1;

			fbd.SelectedPath = Environment.CurrentDirectory;

			if (fbd.ShowDialog(this) == DialogResult.OK)
			{
				CurDir = TbCurDir.Text = fbd.SelectedPath;
				Directory.SetCurrentDirectory(CurDir);
			}

			TbCurDir.SelectAll();
		}

		private void TbCurDir_MouseHover(object sender, EventArgs e)
		{
			ToolTip1.SetToolTip(TbCurDir, TbCurDir.Text);
		}

		private void TbCurDir_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F11:
					_ = TbCmd.Focus();
					break;

				case Keys.F12:
					_ = TbResult.Focus();
					break;

				case Keys.PageUp:
					TbCurDir.SelectionStart = TbCurDir.SelectionStart == 0 ? TbCurDir.TextLength : 0;
					break;

				case Keys.PageDown:
					TbCurDir.SelectionStart = TbCurDir.SelectionStart == TbCurDir.TextLength ? 0 : TbCurDir.TextLength;
					break;

				case Keys.Escape:
					SubTbCmdFocus(-1);
					break;
			}
		}

		//--------------
		// CmsTbCurDir
		//--------------
		private void CmsTbCurDir_全コピー_Click(object sender, EventArgs e)
		{
			TbCurDir.SelectAll();
			TbCurDir.Copy();
		}

		//--------
		// TbCmd
		//--------
		private int GblTbCmdPos = 0;

		private void TbCmd_Enter(object sender, EventArgs e)
		{
			Lbl_F1.ForeColor = Lbl_F2.ForeColor = Lbl_F3.ForeColor = Lbl_F4.ForeColor = Lbl_F5.ForeColor = Lbl_F6.ForeColor = Lbl_F7.ForeColor = Lbl_F8.ForeColor = Lbl_F9.ForeColor = Color.White;
			LblCmd.Visible = true;
		}

		private void TbCmd_Leave(object sender, EventArgs e)
		{
			GblTbCmdPos = TbCmd.SelectionStart;
			Lbl_F1.ForeColor = Lbl_F2.ForeColor = Lbl_F3.ForeColor = Lbl_F4.ForeColor = Lbl_F5.ForeColor = Lbl_F6.ForeColor = Lbl_F7.ForeColor = Lbl_F8.ForeColor = Lbl_F9.ForeColor = Color.Gray;
			LblCmd.Visible = false;
		}

		private void TbCmd_KeyDown(object sender, KeyEventArgs e)
		{
			GblTbCmdPos = TbCmd.SelectionStart;
		}

		private void TbCmd_KeyPress(object sender, KeyPressEventArgs e)
		{
			switch (e.KeyChar)
			{
				// IME入力対策
				case (char)Keys.Enter:
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
						CbCmdHistory.SelectedIndex = -1;
					}
					break;

				case Keys.F2:
					BtnDgvMacro_Click(sender, e);

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
					BtnResultCopy_Click(sender, e);
					break;

				case Keys.F7:
					BtnClear_Click(sender, e);
					break;

				case Keys.F8:
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

				case Keys.F9:
					BtnResultMem_Click(sender, e);
					break;

				case Keys.F10: // ControlBoxが使用
					SendKeys.Send("{UP}");
					break;

				case Keys.F11: // 仮実装
					_ = TbCmdMemo.Focus();
					break;

				case Keys.F12: // 仮実装
					_ = TbResult.Focus();
					break;

				case Keys.Enter:
					// KeyPressと連動
					TbCmd.Text = TbCmd.Text.Replace(NL, "");
					SubTbCmdFocus(-1);
					break;

				case Keys.PageUp:
					if (TbCmd.SelectionStart == 0)
					{
						TbCmd.SelectionStart = TbCmd.TextLength;
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
						TbCmd.SelectionStart = iPos;
					}
					break;

				case Keys.PageDown:
					if (TbCmd.SelectionStart == TbCmd.TextLength)
					{
						TbCmd.SelectionStart = 0;
					}
					else
					{
						char[] ac1 = TbCmd.Text.ToCharArray();
						int iPos = TbCmd.SelectionStart + 1;
						for (; iPos < TbCmd.TextLength; iPos++)
						{
							if (ac1[iPos] == ' ')
							{
								break;
							}
						}
						TbCmd.SelectionStart = iPos;
					}
					break;

				case Keys.Up:
					TbCmd.SelectionStart = 0;
					break;

				case Keys.Down:
					TbCmd.SelectionStart = TbCmd.TextLength;
					break;

				case Keys.Right:
					if (iLen == TbCmd.SelectionStart)
					{
						TbCmd.Text += " ";
						TbCmd.SelectionStart = TbCmd.TextLength;
						// 後段で補完(*)
					}
					break;
			}

			// 補完(*)
			if (e.KeyCode == Keys.Right && TbCmd.TextLength == TbCmd.SelectionStart && Regex.IsMatch(TbCmd.Text, @"^.*#.+\s+"))
			{
				int iWQ = Regex.Matches(TbCmd.Text, "\"(.*?)\"").Count;
				for (int _i1 = 0; _i1 < MACRO.Length / 3; _i1++)
				{
					if (Regex.IsMatch(TbCmd.Text, @"^.*" + MACRO[_i1, 0].ToString()) && (int)MACRO[_i1, 2] > iWQ)
					{
						TbCmd.Text = Regex.Replace(TbCmd.Text, @"\s+$", " \"\"");
					}
				}
				TbCmd.SelectionStart = TbCmd.TextLength - 1;
				TbCmd.ForeColor = Color.Red;
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

		private void TbCmd_MouseUp(object sender, MouseEventArgs e)
		{
			CmsTextSelect_Open(e, TbCmd);
		}

		private void TbCmd_DragDrop(object sender, DragEventArgs e)
		{
			string[] a1 = (string[])e.Data.GetData(DataFormats.FileDrop, false);

			string s1 = "";

			if (TbCmd.Text.Substring(TbCmd.SelectionStart - 1, 1) != " ")
			{
				s1 = " ";
			}

			for (int _i1 = 0; _i1 < a1.Length; _i1++)
			{
				s1 += "\"" + a1[_i1] + "\" ";
			}

			if (TbCmd.SelectionStart < TbCmd.TextLength && TbCmd.Text.Substring(TbCmd.SelectionStart, 1) != " ")
			{
			}
			else
			{
				s1 = s1.TrimEnd();
			}

			int i1 = TbCmd.SelectionStart + s1.Length;

			TbCmd.Text = TbCmd.Text.Substring(0, TbCmd.SelectionStart) + s1 + TbCmd.Text.Substring(TbCmd.SelectionStart);

			TbCmd.SelectionStart = i1;
		}

		private void TbCmd_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		//---------
		// CmsCmd
		//---------
		private string GblCmsCmdBatch = "";

		private void CmsCmd_クリア_Click(object sender, EventArgs e)
		{
			// [Ctrl+Z] 有効化
			TbCmd.SelectAll();
			TbCmd.Cut();
		}

		private void CmsCmd_全コピー_Click(object sender, EventArgs e)
		{
			TbCmd.SelectAll();
			TbCmd.Copy();
		}

		private void CmsCmd_上書き_Click(object sender, EventArgs e)
		{
			// [Ctrl+Z] 有効化
			TbCmd.SelectAll();
			CmsCmd_貼り付け_Click(sender, e);
		}

		private void CmsCmd_貼り付け_Click(object sender, EventArgs e)
		{
			// [Ctrl+Z] 有効化
			Clipboard.SetText(Regex.Replace(Clipboard.GetText(), RgxNL, " "));
			TbCmd.Paste();
		}

		private void CmsCmd_フォルダ選択_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = folderBrowserDialog1;

			fbd.SelectedPath = Environment.CurrentDirectory;

			if (fbd.ShowDialog(this) == DialogResult.OK)
			{
				string s1 = "", s2 = "";

				if (TbCmd.Text.Substring(TbCmd.SelectionStart - 1, 1) != " ")
				{
					s1 = " ";
				}

				if (TbCmd.SelectionStart < TbCmd.TextLength && TbCmd.Text.Substring(TbCmd.SelectionStart, 1) != " ")
				{
					s2 = " ";
				}

				int i1 = TbCmd.SelectionStart + fbd.SelectedPath.Length + 3;
				TbCmd.Text = TbCmd.Text.Substring(0, TbCmd.SelectionStart) + s1 + "\"" + fbd.SelectedPath + "\"" + s2 + TbCmd.Text.Substring(TbCmd.SelectionStart);
				TbCmd.SelectionStart = i1;
			}
		}

		private void CmsCmd_ファイル選択_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = openFileDialog1;

			ofd.InitialDirectory = Environment.CurrentDirectory;
			ofd.Multiselect = true;

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				string sFn = "";

				foreach (string _s1 in ofd.FileNames)
				{
					sFn += "\"" + _s1 + "\" ";
				}

				sFn = sFn.TrimEnd();

				string s1 = "", s2 = "";

				if (TbCmd.Text.Substring(TbCmd.SelectionStart - 1, 1) != " ")
				{
					s1 = " ";
				}

				if (TbCmd.SelectionStart < TbCmd.TextLength && TbCmd.Text.Substring(TbCmd.SelectionStart, 1) != " ")
				{
					s2 = " ";
				}

				int i1 = TbCmd.SelectionStart + sFn.Length + 3;
				TbCmd.Text = TbCmd.Text.Substring(0, TbCmd.SelectionStart) + s1 + sFn + s2 + TbCmd.Text.Substring(TbCmd.SelectionStart);
				TbCmd.SelectionStart = i1;
			}
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
			OpenFileDialog ofd = openFileDialog1;

			ofd.InitialDirectory = Environment.CurrentDirectory;

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				TbCmd.Text = Regex.Replace(
					File.ReadAllText(ofd.FileName, Encoding.GetEncoding(TEXT_CODE[0])),
					";*" + RgxNL, // ";" がなくても改行されていれば有効
					"; "
				);
				SubTbCmdFocus(-1);
			}
		}

		//------------
		// TbCmdMemo
		//------------
		private void TbCmdMemo_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.F11: // 仮実装
					_ = TbResult.Focus();
					break;

				case Keys.F12: // 仮実装
					_ = TbCmd.Focus();
					break;
			}
		}

		private void TbCmdMemo_Enter(object sender, EventArgs e)
		{
			LblCmdMemo.Visible = true;
		}

		private void TbCmdMemo_Leave(object sender, EventArgs e)
		{
			LblCmdMemo.Visible = false;
		}

		private void TbCmdMemo_MouseUp(object sender, MouseEventArgs e)
		{
			CmsTextSelect_Open(e, TbCmdMemo);
		}

		//-------------
		// CmsCmdMemo
		//-------------
		private void CmsCmdMemo_クリア_Click(object sender, EventArgs e)
		{
			// [Ctrl+Z]を有効化
			TbCmdMemo.SelectAll();
			TbCmdMemo.Cut();
		}

		private void CmsCmdMemo_全コピー_Click(object sender, EventArgs e)
		{
			TbCmdMemo.SelectAll();
			TbCmdMemo.Copy();
		}

		private void CmsCmdMemo_上書き_Click(object sender, EventArgs e)
		{
			// [Ctrl+Z] 有効化
			TbCmdMemo.SelectAll();
			CmsCmdMemo_貼り付け_Click(sender, e);
		}

		private void CmsCmdMemo_コピー_Click(object sender, EventArgs e)
		{
			TbCmdMemo.Copy();
		}

		private void CmsCmdMemo_切り取り_Click(object sender, EventArgs e)
		{
			TbCmdMemo.Cut();
		}

		private void CmsCmdMemo_貼り付け_Click(object sender, EventArgs e)
		{
			TbCmdMemo.Paste();
		}

		private void SubCmdMemoAddText(string str)
		{
			_ = NativeMethods.SendMessage(TbCmdMemo.Handle, EM_REPLACESEL, 1, str + NL);
			TbCmdMemo.SelectionStart = TbCmdMemo.TextLength;
			TbCmdMemo.ScrollToCaret();
		}

		//-----------
		// DgvMacro
		//-----------
		private int GblDgvMacroRow = 0;
		private bool GblDgvMacroOpen = true; // DgvMacro.enabled より速い

		private void BtnDgvMacro_Click(object sender, EventArgs e)
		{
			if (GblDgvMacroOpen)
			{
				GblDgvMacroOpen = false;
				DgvMacro.Enabled = false;
				BtnDgvMacro.BackColor = Color.LightYellow;
				DgvMacro.ScrollBars = ScrollBars.None;
				DgvMacro.Width = 68;
				DgvMacro.Height = 23;

				SubTbCmdFocus(GblTbCmdPos);
			}
			else
			{
				GblDgvMacroOpen = true;
				DgvMacro.Enabled = true;
				BtnDgvMacro.BackColor = Color.Gold;
				DgvMacro.ScrollBars = ScrollBars.Both;
				DgvMacro.Width = 408;
				DgvMacro.Height = Height - 228;
				/// DgvMacro.MaximumSize = new Size(DgvMacro.Width, ((DgvMacro.RowCount + 1) * (DgvMacro.ColumnHeadersHeight + 1)) + 1);

				DgvMacro.Focus();
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

		private void DgvMacro_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			// セル[1, n]をクリップボードにコピー
			Clipboard.SetText(DgvMacro[1, DgvMacro.CurrentRow.Index].Value.ToString());

			string s1 = DgvMacro[0, DgvMacro.CurrentRow.Index].Value.ToString();
			int iPos = 0;

			for (int _i1 = 0; _i1 < MACRO.Length / 3; _i1++)
			{
				if (s1 == MACRO[_i1, 0].ToString())
				{
					iPos = (int)MACRO[_i1, 2];
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
			GblTbCmdPos = DgvMacro.CurrentCell.Value.ToString().Length + 2;
			BtnDgvMacro_Click(sender, e);
		}

		private void DgvMacro_KeyDown(object sender, KeyEventArgs e)
		{
			GblDgvMacroRow = DgvMacro.CurrentRow.Index;
		}

		private void DgvMacro_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.F2:
					BtnDgvMacro_Click(sender, e);
					break;

				case Keys.Enter:
					DgvMacro.CurrentCell = DgvMacro[0, DgvMacro.CurrentCell.RowIndex - (DgvMacro.CurrentRow.Index == GblDgvMacroRow ? 0 : 1)];
					GblTbCmdPos = DgvMacro.CurrentCell.Value.ToString().Length + 2;
					DgvMacro_CellMouseClick(sender, null);
					break;

				case Keys.Space:
					GblTbCmdPos = DgvMacro.CurrentCell.Value.ToString().Length + 2;
					DgvMacro_CellMouseClick(sender, null);
					break;

				case Keys.PageUp:
				case Keys.Up:
					if (GblDgvMacroRow == 0)
					{
						DgvMacro.CurrentCell = DgvMacro[0, DgvMacro.RowCount - 1];
					}
					break;

				case Keys.PageDown:
				case Keys.Down:
					if (GblDgvMacroRow == DgvMacro.RowCount - 1)
					{
						DgvMacro.CurrentCell = DgvMacro[0, 0];
					}
					break;

				case Keys.Left:
					DgvMacro.CurrentCell = DgvMacro[0, GblDgvMacroRow == 0 ? DgvMacro.RowCount - 1 : 0];
					break;

				case Keys.Right:
					DgvMacro.CurrentCell = DgvMacro[0, GblDgvMacroRow == DgvMacro.RowCount - 1 ? 0 : DgvMacro.RowCount - 1];
					break;
			}
		}

		//---------
		// DgvCmd
		//---------
		private int GblDgvCmdRow = 0;
		private bool GblDgvCmdOpen = true; // DgvCmd.enabled より速い

		private void BtnDgvCmd_Click(object sender, EventArgs e)
		{
			if (GblDgvCmdOpen)
			{
				GblDgvCmdOpen = false;
				DgvCmd.Enabled = false;
				BtnDgvCmd.BackColor = Color.LightYellow;
				DgvCmd.ScrollBars = ScrollBars.None;
				DgvCmd.Width = 68;
				DgvCmd.Height = 23;
				TbDgvCmdSearch.Visible = false;

				SubTbCmdFocus(GblTbCmdPos);
			}
			else
			{
				GblDgvCmdOpen = true;
				DgvCmd.Enabled = true;
				BtnDgvCmd.BackColor = Color.Gold;
				DgvCmd.ScrollBars = ScrollBars.Both;
				DgvCmd.Width = 323;
				DgvCmd.Height = Height - 228;
				/// DgvCmd.MaximumSize = new Size(DgvCmd.Width, ((DgvCmd.RowCount + 1) * (DgvCmd.ColumnHeadersHeight + 1)) + 1);

				TbDgvCmdSearch.Visible = true;
				TbDgvCmdSearch.BringToFront();
				TbDgvCmdSearch.Focus();
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

		private void DgvCmd_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			TbCmd.Text = DgvCmd[0, DgvCmd.CurrentCell.RowIndex].Value.ToString();
			SubTbCmdFocus(TbCmd.TextLength);

			GblDgvCmdOpen = true;
			GblTbCmdPos = DgvCmd.CurrentCell.Value.ToString().Length;
			BtnDgvCmd_Click(sender, e);
		}

		private void DgvCmd_KeyDown(object sender, KeyEventArgs e)
		{
			GblDgvCmdRow = DgvCmd.CurrentRow.Index;
		}

		private void DgvCmd_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.F3:
					BtnDgvCmd_Click(sender, e);
					break;

				case Keys.Enter:
					DgvCmd.CurrentCell = DgvCmd[0, DgvCmd.CurrentCell.RowIndex - (DgvCmd.CurrentRow.Index == GblDgvCmdRow ? 0 : 1)];
					GblTbCmdPos = DgvCmd.CurrentCell.Value.ToString().Length;
					DgvCmd_CellMouseClick(sender, null);
					break;

				case Keys.Space:
					GblTbCmdPos = DgvCmd.CurrentCell.Value.ToString().Length;
					DgvCmd_CellMouseClick(sender, null);
					break;

				case Keys.PageUp:
					if (GblDgvCmdRow == 0)
					{
						DgvCmd.CurrentCell = DgvCmd[0, DgvCmd.RowCount - 1];
					}
					break;

				case Keys.Up:
					if (GblDgvCmdRow == 0)
					{
						_ = TbDgvCmdSearch.Focus();
					}
					break;

				case Keys.PageDown:
				case Keys.Down:
					if (GblDgvCmdRow == DgvCmd.RowCount - 1)
					{
						DgvCmd.CurrentCell = DgvCmd[0, 0];
					}
					break;

				case Keys.Left:
					DgvCmd.CurrentCell = DgvCmd[0, GblDgvCmdRow == 0 ? DgvCmd.RowCount - 1 : 0];
					break;

				case Keys.Right:
					DgvCmd.CurrentCell = DgvCmd[0, GblDgvCmdRow == DgvCmd.RowCount - 1 ? 0 : DgvCmd.RowCount - 1];
					break;
			}
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

		private void TbDgvCmdSearch_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.F3:
					BtnDgvCmd_Click(sender, e);
					break;

				case Keys.Up:
				case Keys.PageUp:
					TbDgvCmdSearch.SelectionStart = 0;
					break;

				case Keys.PageDown:
					TbDgvCmdSearch.SelectionStart = TbDgvCmdSearch.TextLength;
					break;

				case Keys.Down:
					_ = DgvCmd.Focus();
					break;
			}
		}

		private void TbDgvCmdSearch_TextChanged(object sender, EventArgs e)
		{
			DgvCmd.Rows.Clear();

			try
			{
				foreach (string _s1 in IeFile)
				{
					if (Regex.IsMatch(_s1, TbDgvCmdSearch.Text, RegexOptions.IgnoreCase))
					{
						_ = DgvCmd.Rows.Add(_s1);
					}
				}
			}
			catch
			{
			}

			Thread.Sleep(250);
		}

		//--------------------
		// CmsTbDgvCmdSearch
		//--------------------
		private void CmsTbDgvCmdSearch_クリア_Click(object sender, EventArgs e)
		{
			// [Ctrl+Z]を有効化
			TbDgvCmdSearch.SelectAll();
			TbDgvCmdSearch.Cut();

			DgvCmd.Rows.Clear();

			foreach (string _s1 in IeFile)
			{
				_ = DgvCmd.Rows.Add(_s1);
			}
		}

		private void CmsTbDgvCmdSearch_貼り付け_Click(object sender, EventArgs e)
		{
			TbDgvCmdSearch.Paste();
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
				case Keys.Escape:
				case Keys.F4:
				case Keys.Enter:
				case Keys.Space:
					CbTextCode.DroppedDown = false;
					SubTbCmdFocus(GblTbCmdPos);
					break;
			}
		}

		private void EventDataReceived(object sender, DataReceivedEventArgs e)
		{
			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, e.Data + NL);
		}

		private void ProcessDataReceived(object sender, DataReceivedEventArgs e)
		{
			_ = Invoke(MyEvent, new object[2] { sender, e });
		}

		//----------------
		// CmsCbTextCode
		//----------------
		private void CmsCbTextCode_全コピー_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(CbTextCode.Text);
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

		private void SubTbCmdExec(string cmd)
		{
			cmd = cmd.Trim();

			if (cmd.Length == 0)
			{
				return;
			}

			SubLblWaitOn();

			// 変数
			Regex rgx = null;
			Match match = null;
			string s1 = "";
			int i1 = 0;

			// 変換 コマンド => マクロ
			cmd = Regex.Replace(cmd, @"^(cd|chdir)\s+", "#cd ", RegexOptions.IgnoreCase);
			cmd = Regex.Replace(cmd, @"^(cls|clear)", "#clear", RegexOptions.IgnoreCase);

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
						TbCmdMemo.Text = "";
						TbResult.Text = "";
						TbInfo.Text = "";

						GblDgvCmdOpen = true;
						BtnDgvCmd_Click(null, null);

						break;

					// フォルダ変更
					case "#cd":
						aOp[1] = aOp[1].Trim();
						if (aOp[1].Length == 0)
						{
							SubCmdMemoAddText("(例１) #cd \"..\"" + NL + "(例２) #cd \"NewDir\"");
							break;
						}
						string sCd = Path.GetFullPath(aOp[1]);
						try
						{
							Directory.SetCurrentDirectory(sCd);
						}
						catch
						{
							SubCmdMemoAddText("Dir \"" + aOp[1] + "\" は存在しない");
							break;
						}
						TbCurDir.Text = Path.GetFullPath(sCd);
						break;

					// 文字コード（Batchで使用）
					case "#code":
						if (aOp[1].Length == 0)
						{
							SubCmdMemoAddText("引数：\"文字コード\"" + NL + "※ \"Shift_JIS\", \"UTF-8\"");
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
							SubCmdMemoAddText("引数：\"開始行\" \"終了行\"" + NL + "※終了行 \"0\" のときは最終行まで");
							break;
						}
						int cnt2 = 0;
						_ = SB.Clear();
						foreach (string _s1 in TbResult.Text.Split(AryNL, StringSplitOptions.None))
						{
							++cnt2;
							if (cnt2 >= bgnLn)
							{
								_ = SB.Append(_s1 + NL);
							}
							if (endLn > 0 && cnt2 >= endLn)
							{
								break;
							}
						}
						TbResult.Text = SB.ToString();
						break;

					// 空白行削除
					case "#rmblankln":
						_ = SB.Clear();
						foreach (string _s1 in TbResult.Text.Split(AryNL, StringSplitOptions.None))
						{
							if (_s1.TrimEnd().Length > 0)
							{
								_ = SB.Append(_s1 + NL);
							}
						}
						TbResult.Text = SB.ToString();
						break;

					// 行番号付与
					case "#addlnnum":
						int cnt1 = 0;
						_ = SB.Clear();
						foreach (string _s1 in TbResult.Text.Split(AryNL, StringSplitOptions.None))
						{
							_ = SB.Append(string.Format("{0:D8}\t{1}{2}", ++cnt1, _s1, NL));
						}
						TbResult.Text = SB.ToString();
						break;

					// 改行を消去
					case "#rmnl":
						TbResult.Text = Regex.Replace(TbResult.Text, RgxNL, "");
						break;

					// ファイル取得／ファイル読込
					case "#wget":
					case "#fread":
						if (aOp[1].Length == 0)
						{
							break;
						}
						using (WebClient wc = new WebClient())
						{
							try
							{
								s1 = Encoding.GetEncoding(CbTextCode.Text).GetString(wc.DownloadData(aOp[1]));
								_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, Regex.Replace(s1, RgxNL, NL));
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
							SubCmdMemoAddText("出力結果から行毎に読み込んで処理" + NL + "引数：\"コマンド\" \"オプション\"");
							break;
						}
						MyEvent = new MyEventHandler(EventDataReceived);
						foreach (string _s1 in TbResult.Text.Split(AryNL, StringSplitOptions.None))
						{
							if (_s1.Trim().Length > 0)
							{
								PS = new Process();
								PS.StartInfo.FileName = aOp[1];
								PS.StartInfo.Arguments = aOp[2] + " " + _s1;
								PS.StartInfo.UseShellExecute = false;
								PS.StartInfo.RedirectStandardInput = true;
								PS.StartInfo.RedirectStandardOutput = true;
								PS.StartInfo.RedirectStandardError = true;
								PS.StartInfo.CreateNoWindow = true;
								PS.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CbTextCode.Text);
								PS.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);
								try
								{
									// Stdout, Stderr を出力
									_ = PS.Start();
									TbCmdMemo.AppendText(Regex.Replace(PS.StandardOutput.ReadToEnd(), RgxNL, NL));
									TbCmdMemo.AppendText(Regex.Replace(PS.StandardError.ReadToEnd(), RgxNL, NL));
									PS.Close();
								}
								catch
								{
									_ = MessageBox.Show("#stream の引数を再確認してください!!");
									break;
								}
							}
						}
						TbCmdMemo.SelectionStart = TbCmdMemo.TextLength;
						TbCmdMemo.ScrollToCaret();
						break;

					// 計算機
					case "#calc":
						TbResult.Text += RtnEvalCalc(aOp[1]) + NL;
						break;

					// 現在日時
					case "#now":
						TbResult.Text += DateTime.Now.ToString("yyyy/MM/dd(ddd) HH:mm:ss") + NL;
						break;

					// バージョン
					case "#version":
						TbResult.Text += VERSION + NL;
						break;

					// 終了
					case "#exit":
						Application.Exit();
						break;
				}
			}
			// コマンド実行
			else
			{
				MyEvent = new MyEventHandler(EventDataReceived);
				PS = new Process();

				PS.StartInfo.UseShellExecute = false;
				PS.StartInfo.RedirectStandardInput = true;
				PS.StartInfo.RedirectStandardOutput = true;
				PS.StartInfo.RedirectStandardError = true;
				PS.StartInfo.CreateNoWindow = true;
				PS.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CbTextCode.Text);
				PS.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);

				string _cmd1 = "";
				string _cmd2 = "";
				string[] _aryCmd = cmd.Split(' ');

				_cmd1 = _aryCmd[0];
				_aryCmd[0] = "";
				_cmd2 = string.Join(" ", _aryCmd).TrimStart();

				// 直接コマンド実行（例：date）
				// 失敗したら、cmd.exe経由で再実行（例：dir）
				try
				{
					PS.StartInfo.FileName = _cmd1;
					PS.StartInfo.Arguments = _cmd2;
					_ = PS.Start();
				}
				catch
				{
					PS.StartInfo.FileName = "cmd.exe";
					PS.StartInfo.Arguments = $"/c {_cmd1} {_cmd2}";
					_ = PS.Start();
				}

				s1 = PS.StandardOutput.ReadToEnd();
				if (Regex.IsMatch(s1, NL) == false)
				{
					s1 = s1.Replace("\n", NL);
				}
				TbResult.AppendText(s1);

				PS.Close();
			}

			TbResult.SelectionStart = TbResult.TextLength;
			TbResult.ScrollToCaret();

			SubLblWaitOff();
		}

		//----------------
		// BtnResultCopy
		//----------------
		private void BtnResultCopy_Click(object sender, EventArgs e)
		{
			if (TbResult.Text.Trim().Length > 0)
			{
				SubCmdMemoAddText(TbResult.Text);
			}
			SubTbCmdFocus(GblTbCmdPos);
		}

		//-------------
		// 入出力消去
		//-------------
		private void BtnClear_Click(object sender, EventArgs e)
		{
			// [Ctrl+Z]を有効化
			TbResult.SelectAll();
			TbResult.Cut();

			TbInfo.Text = "";

			GC.Collect();

			SubTbCmdFocus(GblTbCmdPos);
		}

		//-----------
		// TbResult
		//-----------
		private void TbResult_Enter(object sender, EventArgs e)
		{
			LblResult.Visible = true;
		}

		private void TbResult_Leave(object sender, EventArgs e)
		{
			LblResult.Visible = false;
		}

		private void TbResult_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.F11: // 仮実装
					_ = TbCmd.Focus();
					break;

				case Keys.F12: // 仮実装
					_ = TbCmdMemo.Focus();
					break;
			}
		}

		private void TbResult_MouseUp(object sender, MouseEventArgs e)
		{
			if (TbResult.SelectionLength == 0)
			{
				TbInfo.Text = "";
				return;
			}

			int iNL = 0;
			int iRow = 0;
			int iCnt = 0;

			foreach (string _s1 in TbResult.SelectedText.Split(AryNL, StringSplitOptions.None))
			{
				++iNL;

				if (_s1.Trim().Length > 0)
				{
					++iRow;
				}

				iCnt += _s1.Length;
			}

			TbInfo.Text = string.Format("{0}字(有効{1}行／全{2}行)選択", iCnt, iRow, iNL);

			CmsTextSelect_Open(e, TbResult);
		}

		private void TbResult_TextChanged(object sender, EventArgs e)
		{
			TbInfo.Text = "";
		}

		private void TbResult_DragDrop(object sender, DragEventArgs e)
		{
			SubLblWaitOn();

			_ = SB.Clear();

			string s1 = "";

			foreach (string _s1 in (string[])e.Data.GetData(DataFormats.FileDrop, false))
			{
				if (RtnIsBinaryFile(_s1))
				{
					s1 += "・" + _s1 + NL;
				}
				else
				{
					foreach (string _s2 in File.ReadLines(_s1, Encoding.GetEncoding(CbTextCode.Text)))
					{
						_ = SB.Append(_s2.TrimEnd() + NL);
					}
				}
			}

			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, SB.ToString());

			if (s1.Length > 0)
			{
				_ = MessageBox.Show(s1, "読込エラー");
			}

			SubLblWaitOff();
		}

		private void TbResult_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		//------------
		// CmsResult
		//------------
		private void CmsResult_クリア_Click(object sender, EventArgs e)
		{
			// [Ctrl+Z]を有効化
			TbResult.SelectAll();
			TbResult.Cut();
		}

		private void CmsResult_全コピー_Click(object sender, EventArgs e)
		{
			TbResult.SelectAll();
			TbResult.Copy();
		}

		private void CmsResult_上書き_Click(object sender, EventArgs e)
		{
			// [Ctrl+Z] 有効化
			TbResult.SelectAll();
			CmsResult_貼り付け_Click(sender, e);
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
				Regex.Replace(Clipboard.GetText(), RgxNL, NL)
			);
		}

		private void CmsResult_ファイル名を貼り付け_Click(object sender, EventArgs e)
		{
			_ = SB.Clear();

			foreach (string _s1 in Clipboard.GetFileDropList())
			{
				_ = SB.Append(_s1 + (Directory.Exists(_s1) ? @"\" : "") + NL);
			}

			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, SB.ToString());
		}

		private void CmsResult_名前を付けて保存_Click(object sender, EventArgs e)
		{
			SubTextToSaveFile(TbResult.Text, CbTextCode.Text);
		}

		//-------
		// 履歴
		//-------
		private int GblCbCmdHistoryRow = 0;
		private int GblCbResultHistoryRow = 0;

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
			if (CbCmdHistory.Text.Length == 0)
			{
				CbCmdHistory.SelectedIndex = -1;
				SubTbCmdFocus(GblTbCmdPos);
			}
			else
			{
				TbCmd.Text = CbCmdHistory.Text;
				SubTbCmdFocus(TbCmd.TextLength);
			}
		}

		private void CbCmdHistory_KeyDown(object sender, KeyEventArgs e)
		{
			GblCbCmdHistoryRow = CbCmdHistory.SelectedIndex;
		}

		private void CbCmdHistory_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F1:
				case Keys.Space:
					CbCmdHistory.DroppedDown = false;
					break;

				case Keys.PageUp:
				case Keys.Up:
					if (GblCbCmdHistoryRow == 0)
					{
						CbCmdHistory.SelectedIndex = CbCmdHistory.Items.Count - 1;
					}
					break;

				case Keys.PageDown:
				case Keys.Down:
					if (GblCbCmdHistoryRow == CbCmdHistory.Items.Count - 1)
					{
						CbCmdHistory.SelectedIndex = 0;
					}
					break;

				case Keys.Left:
					CbCmdHistory.SelectedIndex = CbCmdHistory.SelectedIndex == 0 ? CbCmdHistory.Items.Count - 1 : 0;
					break;

				case Keys.Right:
					CbCmdHistory.SelectedIndex = CbCmdHistory.SelectedIndex == CbCmdHistory.Items.Count - 1 ? 0 : CbCmdHistory.Items.Count - 1;
					break;
			}
		}

		private void CbResultHistory_Enter(object sender, EventArgs e)
		{
			Lbl_F8.ForeColor = Color.Gold;
		}

		private void CbResultHistory_Leave(object sender, EventArgs e)
		{
			Lbl_F8.ForeColor = Color.Gray;
		}

		private void CbResultHistory_DropDownClosed(object sender, EventArgs e)
		{
			if (CbResultHistory.Text.Length == 0)
			{
				CbResultHistory.SelectedIndex = -1;
			}
			else
			{
				TbResult.Text = ResultHistory[CbResultHistory.Text.Substring(0, 8)];
			}
			SubTbCmdFocus(GblTbCmdPos);
		}

		private void CbResultHistory_KeyDown(object sender, KeyEventArgs e)
		{
			GblCbResultHistoryRow = CbResultHistory.SelectedIndex;
		}

		private void CbResultHistory_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.F8:
				case Keys.Space:
					CbResultHistory.DroppedDown = false;
					break;

				case Keys.PageUp:
				case Keys.Up:
					if (GblCbResultHistoryRow == 0)
					{
						CbResultHistory.SelectedIndex = CbResultHistory.Items.Count - 1;
					}
					break;

				case Keys.PageDown:
				case Keys.Down:
					if (GblCbResultHistoryRow == CbResultHistory.Items.Count - 1)
					{
						CbResultHistory.SelectedIndex = 0;
					}
					break;

				case Keys.Left:
					CbResultHistory.SelectedIndex = CbResultHistory.SelectedIndex == 0 ? CbResultHistory.Items.Count - 1 : 0;
					break;

				case Keys.Right:
					CbResultHistory.SelectedIndex = CbResultHistory.SelectedIndex == CbResultHistory.Items.Count - 1 ? 0 : CbResultHistory.Items.Count - 1;
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

		private void BtnResultMem_Click(object sender, EventArgs e)
		{
			if (TbResult.Text.Length > 0)
			{
				// 同時刻のとき +1秒
				Cursor.Current = Cursors.WaitCursor;
				while (true)
				{
					try
					{
						ResultHistory.Add(DateTime.Now.ToString("HH:mm:ss"), TbResult.Text);
						break;
					}
					catch
					{
						Thread.Sleep(1000);
					}
				}
				Cursor.Current = Cursors.Default;

				// 最後の10件のみ表示
				CbResultHistory.Items.Clear();
				_ = CbResultHistory.Items.Add("");

				int cntTail = ResultHistory.Count() - 10;
				int cnt = 0;
				foreach (KeyValuePair<string, string> dict in ResultHistory)
				{
					if (++cnt > cntTail)
					{
						string _s1 = dict.Value.Substring(0, dict.Value.Length > 30 ? 30 : dict.Value.Length);
						_ = CbResultHistory.Items.Add(string.Format("{0}| {1}", dict.Key, _s1.Replace(NL, @"\n")));
					}
				}
			}

			SubTbCmdFocus(GblTbCmdPos);
		}

		//-----------------
		// フォントサイズ
		//-----------------
		private void NudTbResult_Enter(object sender, EventArgs e)
		{
			NudTbResult.ForeColor = Color.White;
			NudTbResult.BackColor = Color.RoyalBlue;
		}

		private void NudTbResult_Leave(object sender, EventArgs e)
		{
			NudTbResult.ForeColor = Color.White;
			NudTbResult.BackColor = Color.DimGray;
		}

		private void NudTbResult_ValueChanged(object sender, EventArgs e)
		{
			if (NudTbResult.Value < NudTbResult.Minimum)
			{
				NudTbResult.Value = NudTbResult.Minimum;
			}
			else if (NudTbResult.Value > NudTbResult.Maximum)
			{
				NudTbResult.Value = NudTbResult.Maximum;
			}

			TbResult.Font = new Font(TbResult.Font.Name.ToString(), (float)NudTbResult.Value);
		}

		private void NudTbResult_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case (Keys.Escape):
				case (Keys.Enter):
				case (Keys.Space):
					SubTbCmdFocus(GblTbCmdPos);
					break;
			}
		}

		//----------------
		// CmsTextSelect
		//----------------
		private void CmsTextSelect_Open(MouseEventArgs e, TextBox Tb)
		{
			if (Tb.SelectionLength > 0 && e.Button == MouseButtons.Left)
			{
				TB = Tb;
				CmsTextSelect.Show(Cursor.Position);
			}
		}

		private void CmsTextSelect_コピー_Click(object sender, EventArgs e)
		{
			TB.Copy();
		}

		private void CmsTextSelect_切り取り_Click(object sender, EventArgs e)
		{
			TB.Cut();
		}

		private void CmsTextSelect_削除_Click(object sender, EventArgs e)
		{
			TB.SelectedText = "";
		}

		private void CmsTextSelect_貼り付け_Click(object sender, EventArgs e)
		{
			TB.Paste();
		}

		private void CmsTextSelect_ネット検索_Google_Click(object sender, EventArgs e)
		{
			SubNetSearch("https://www.google.co.jp/search?q=");
		}

		private void CmsTextSelect_ネット検索_YouTube_Click(object sender, EventArgs e)
		{
			SubNetSearch("https://www.youtube.com/results?search_query=");
		}

		private void CmsTextSelect_ネット検索_Wikipedia_Click(object sender, EventArgs e)
		{
			SubNetSearch("https://ja.wikipedia.org/wiki/");
		}
	
		private void SubNetSearch(string url)
		{
			_ = Process.Start(url + HttpUtility.UrlEncode(TB.SelectedText));
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
				iPos = TbCmd.TextLength;
			}
			TbCmd.Select(iPos, 0);

			LblCmd.Visible = true;
			LblCurDir.Visible = LblCmdMemo.Visible = LblResult.Visible = false;
		}

		//-----------------------
		// contextMenuStrip操作
		//-----------------------
		// ファイル保存
		private void SubTextToSaveFile(string s, string code)
		{
			SaveFileDialog sfd = saveFileDialog1;

			sfd.InitialDirectory = Environment.CurrentDirectory;
			sfd.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
			sfd.Filter = FILE_FILTER;
			sfd.FilterIndex = 1;

			if (sfd.ShowDialog() == DialogResult.OK)
			{
				switch (code.ToUpper())
				{
					case "UTF-8":
						File.WriteAllText(sfd.FileName, s, new UTF8Encoding(false));
						break;

					default:
						File.WriteAllText(sfd.FileName, s, Encoding.GetEncoding(TEXT_CODE[0]));
						break;
				}
			}
		}

		//-----------
		// LblWait
		//-----------
		private void SubLblWaitOn()
		{
			Cursor.Current = Cursors.WaitCursor;

			LblWait.Left = (Width - LblWait.Width - 24) / 2;
			LblWait.Top = (Height - LblWait.Height) / 2;
			LblWait.Visible = true;
			Refresh();
		}

		private void SubLblWaitOff()
		{
			LblWait.Visible = false;
			Cursor.Current = Cursors.Default;
		}

		//---------------------
		// 正規表現による検索
		//---------------------
		private string RtnTextGrep(string str, string sRgx, bool bMatch)
		{
			if (sRgx.Length == 0)
			{
				SubCmdMemoAddText(@"正規表現による検索：\t \\ \. etc.");
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

			_ = SB.Clear();

			foreach (string _s1 in str.Split(AryNL, StringSplitOptions.None))
			{
				if (bMatch == rgx.IsMatch(_s1))
				{
					_ = SB.Append(_s1 + NL);
				}
			}

			return SB.ToString();
		}

		//---------------------
		// 正規表現による置換
		//---------------------
		private string RtnTextReplace(string str, string sOld, string sNew)
		{
			if (sOld.Length == 0)
			{
				SubCmdMemoAddText(@"正規表現による置換：\t \\ \. $1,$2... etc.");
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

			_ = SB.Clear();

			foreach (string _s1 in str.Split(AryNL, StringSplitOptions.None))
			{
				_ = SB.Append(rgx.Replace(_s1, sNew) + NL);
			}

			return SB.ToString();
		}

		//---------------------
		// 正規表現による分割
		//---------------------
		private string RtnTextSplitCnv(string str, string sSplit, string sRgx)
		{
			if (sSplit.Length == 0 || sRgx.Length == 0)
			{
				SubCmdMemoAddText(@"正規表現による分割：\t \\ \. [1],[2]... etc.");
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

			_ = SB.Clear();

			foreach (string _s1 in str.Split(AryNL, StringSplitOptions.None))
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
					_ = SB.Append(rgx1.Replace(_s2, "") + NL);
				}
			}

			return SB.ToString();
		}

		//----------------
		// 全角 <=> 半角
		//----------------
		private static string RtnZenNum(string s)
		{
			Regex rgx = new Regex(@"\d+");
			return rgx.Replace(s, RtnReplacerWide);
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
				SubCmdMemoAddText("引数：\"開始位置\" \"文字長\"");
				return str;
			}

			_ = SB.Clear();

			foreach (string _s1 in str.Split(AryNL, StringSplitOptions.None))
			{
				_ = SB.Append(RtnEraseLen(_s1, ' ', iBgnPos, iEndPos) + NL);
			}

			return SB.ToString();
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

			foreach (string _s1 in str.Split(AryNL, StringSplitOptions.None))
			{
				string _s2 = _s1.TrimEnd();
				if (_s2.Length > 0)
				{
					l1.Add(_s2);
				}
			}

			l1.Sort();

			if (!bAsc)
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
			_ = SB.Clear();

			string flg = "";

			foreach (string _s1 in str.Split(AryNL, StringSplitOptions.None))
			{
				if (_s1 != flg && _s1.TrimEnd().Length > 0)
				{
					_ = SB.Append(_s1 + NL);
					flg = _s1;
				}
			}

			return SB.ToString();
		}

		//-----------
		// Eval計算
		//-----------
		private string RtnEvalCalc(string str)
		{
			string rtn = Regex.Replace(str.ToLower(), @"(\s+|math\.)", "");

			// Help
			if (rtn.Length == 0)
			{
				return "pi, pow(n,n), sqrt(n), sin(n°), cos(n°), tan(n°)";
			}

			// 定数
			rtn = rtn.Replace("pi", Math.PI.ToString());

			double _PiPerDeg = Math.PI / 180;

			// sqrt(n) sin(n°) cos(n°) tan(n°)
			string[] aMath = { "sqrt", "sin", "cos", "tan" };
			foreach (string _s1 in aMath)
			{
				foreach (Match _m1 in Regex.Matches(rtn, $@"{_s1}\(\d+\.*\d*\)"))
				{
					string _s2 = _m1.Value;
					_s2 = _s2.Replace(_s1 + "(", "");
					_s2 = _s2.Replace(")", "");

					double _d1 = double.Parse(_s2);

					switch (_s1)
					{
						case "sqrt":
							_d1 = Math.Sqrt(_d1);
							break;
						case "sin":
							_d1 = Math.Sin(_d1 * _PiPerDeg);
							break;
						case "cos":
							_d1 = Math.Cos(_d1 * _PiPerDeg);
							break;
						case "tan":
							_d1 = Math.Tan(_d1 * _PiPerDeg);
							break;
						default:
							_d1 = 0;
							break;
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

		//----------------
		// Binary File ?
		//----------------
		private bool RtnIsBinaryFile(string path)
		{
			FileStream fs = File.OpenRead(path);
			int len = (int)fs.Length;
			byte[] ac = new byte[len];
			int size = fs.Read(ac, 0, len);

			int cnt = 0;

			for (int _i1 = 0; _i1 < size; _i1++)
			{
				if (ac[_i1] == 0)
				{
					if (++cnt > 2)
					{
						return true;
					}
				}
				else
				{
					cnt = 0;
				}
			}

			return false;
		}
	}
}
