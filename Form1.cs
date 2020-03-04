using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
		private const string VERSION = "Ver.20200304_1947 'A-29' (C)2018-2020 iwm-iwama";

		private const string NL = "\r\n";

		private readonly string[] SPLITS = { NL };

		private readonly string[] GblASTextCode = { "Shift_JIS", "UTF-8" };

		private readonly object[,] GblAOCmd = {
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
			{ "#sort",      "ソート",                                         0 },
			{ "#uniq",      "ソートし重複行を消去",                           0 },
			{ "#getLn",     "指定行取得    #getLn \"2\" \"0\"",               2 },
			{ "#rmBlankLn", "空白行削除",                                     0 },
			{ "#addLnNum",  "行番号付与",                                     0 },
			{ "#wget",      "ファイル取得  #wget \"http://.../index.html\"",  1 },
			{ "#stream",    "行毎に処理    #stream \"wget\" \"-q\"",          2 },
			{ "#msgbox",    "MsgBox        #msgbox \"本文\"",                 1 },
			{ "#calc",      "計算機        #calc \"45 * pi / 180\"",          1 },
			{ "#now",       "現在日時",                                       0 },
			{ "#version",   "バージョン",                                     0 }
		};

		//-----------
		// 大域変数
		//-----------
		// CurDir
		private string CurDir = null;

		// 文字列
		private readonly StringBuilder SB = new StringBuilder();

		// 履歴
		private readonly List<string> LCmdHistory = new List<string>() { };
		private readonly Dictionary<string, string> HResultHistory = new Dictionary<string, string>();

		private delegate void MyEventHandler(object sender, DataReceivedEventArgs e);
		private event MyEventHandler MyEvent = null;

		private Process P1 = null;

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
			SubDgvCmdLoad();

			// 入力例
			TbCmd.Text = "dir";

			TbCmd_Enter(sender, e);

			// CurDir表示
			CurDir = TbCurDir.Text = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(CurDir);

			// DgvMacro／DgvCmd 表示
			for (int _i1 = 0; _i1 < GblAOCmd.GetLength(0); _i1++)
			{
				_ = DgvMacro.Rows.Add(GblAOCmd[_i1, 0], GblAOCmd[_i1, 1]);
			}

			GblBDgvMacro = true;
			BtnDgvMacro_Click(sender, e);

			GblBDgvCmd = true;
			BtnDgvCmd_Click(sender, e);

			// Encode
			foreach (string _s1 in GblASTextCode)
			{
				_ = CbTextCode.Items.Add(_s1);
			}
			CbTextCode.Text = GblASTextCode[0];

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
				ShowNewFolderButton = false
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
		private void TbCmd_Enter(object sender, EventArgs e)
		{
			BtnCmdBackColor.BackColor = BtnCmdBackColor.ForeColor = Color.RoyalBlue;
			LstTbCmd.Visible = false;
		}

		private void TbCmd_Leave(object sender, EventArgs e)
		{
			BtnCmdBackColor.BackColor = BtnCmdBackColor.ForeColor = Color.Crimson;
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
			}

			int iLen = TbCmd.TextLength;

			switch (e.KeyCode)
			{
				case Keys.Escape:
					TbResult.Focus();
					break;

				case Keys.Enter:
					// KeyPressと連動
					TbCmd.Text = TbCmd.Text.Replace(NL, null);
					SubTbCmdFocus(-1);
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
						TbCmd.SelectionStart = iPos;
					}
					break;

				case Keys.Down:
					LstTbCmd.Items.Clear();
					_ = LstTbCmd.Items.Add("[×]");
					foreach (string fn in Directory.GetDirectories(TbCurDir.Text, "*"))
					{
						_ = LstTbCmd.Items.Add(@".\" + Path.GetFileName(fn));
					}
					foreach (string fn in Directory.GetFiles(TbCurDir.Text, "*"))
					{
						_ = LstTbCmd.Items.Add(Path.GetFileName(fn));
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
			if ((e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.Space) && TbCmd.TextLength == TbCmd.SelectionStart && Regex.IsMatch(TbCmd.Text, @".*#"))
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
			string s1 = LstTbCmd.SelectedItem.ToString();

			if (s1 != "[×]")
			{
				TbCmd.Text = TbCmd.Text.Substring(0, TbCmd.SelectionStart) + s1 + TbCmd.Text.Substring(TbCmd.SelectionStart);
			}

			LstTbCmd.Visible = false;
			SubTbCmdFocus(-1);

			TbCmd.ScrollToCaret();
		}

		private void LstTbCmd_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.Tab:
					LstTbCmd.Visible = false;
					SubTbCmdFocus(-1);
					break;

				case Keys.Enter:
				case Keys.Space:
					LstTbCmd_Click(sender, e);
					break;
			}
		}

		//---------
		// CmsCmd
		//---------
		private string GblCmsCmdBatch = null;

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
			GblCmsCmdBatch = CmsCmd_コマンドをグループ化_追加.ToolTipText = CmsCmd_コマンドをグループ化_出力.ToolTipText = null;
		}

		private void CmsCmd_コマンドをグループ化_簡単な説明_Click(object sender, EventArgs e)
		{
			_ = NativeMethods.SendMessage(
				TbResult.Handle, EM_REPLACESEL, 1,
				NL +
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

		private void CmsCmd_全クリア_Click(object sender, EventArgs e)
		{
			TbCmd.Text = null;
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

		private void CmsCmd_コマンドを保存_Click(object sender, EventArgs e)
		{
			SubTextToSaveFile(string.Join(";" + NL, RtnCmdFormat(TbCmd.Text).ToArray()) + NL, "Shift_JIS");
		}

		private void CmsCmd_コマンドを読込_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog1 = new OpenFileDialog()
			{
				InitialDirectory = ".",
				FilterIndex = 1
			}
			)
			{
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					TbCmd.Text = Regex.Replace(
						File.ReadAllText(openFileDialog1.FileName, Encoding.GetEncoding("Shift_JIS")),
						@";*\r*\n", // ";" がなくても改行されていれば有効
						"; "
					);
					SubTbCmdFocus(-1);
				}
			}
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
			TbCmdSub.Text = null;
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
		private bool GblBDgvMacro = true;

		private void BtnDgvMacro_Click(object sender, EventArgs e)
		{
			if (GblBDgvMacro == true)
			{
				GblBDgvMacro = false;
				BtnDgvMacro.BackColor = Color.LightYellow;

				DgvMacro.Enabled = false;
				DgvMacro.ScrollBars = ScrollBars.None;
				DgvMacro.Width = 68;
				DgvMacro.Height = 23;
			}
			else
			{
				GblBDgvMacro = true;
				BtnDgvMacro.BackColor = Color.Gold;

				DgvMacro.Enabled = true;
				DgvMacro.ScrollBars = ScrollBars.Vertical;
				DgvMacro.Width = 369;
				DgvMacro.Height = 311;
			}
		}

		private void DgvMacro_MouseHover(object sender, EventArgs e)
		{
			_ = DgvMacro.Focus();
		}

		private void DgvMacro_Click(object sender, EventArgs e)
		{
			string s1 = DgvMacro[0, DgvMacro.CurrentRow.Index].Value.ToString();
			int iPos = 0;

			for (int _i1 = 0; _i1 < GblAOCmd.Length; _i1++)
			{
				if (GblAOCmd[_i1, 0].ToString() == s1)
				{
					int _i2 = (int)GblAOCmd[_i1, 2];
					for (int _i3 = 0; _i3 < _i2; _i3++)
					{
						s1 += " \"\"";
					}
					iPos = _i2 - 1;
					break;
				}
			}

			TbCmd.Text = s1;

			SubTbCmdFocus(TbCmd.TextLength - 1 - (iPos * 3));

			GblBDgvMacro = true;
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
		private bool GblBDgvCmd = true;

		private void BtnDgvCmd_Click(object sender, EventArgs e)
		{
			if (GblBDgvCmd == true)
			{
				GblBDgvCmd = false;
				BtnDgvCmd.BackColor = Color.LightYellow;

				DgvCmd.Enabled = false;
				DgvCmd.ScrollBars = ScrollBars.None;
				DgvCmd.Width = 68;
				DgvCmd.Height = 23;
				TbDgvCmdSearch.Visible = false;
			}
			else
			{
				GblBDgvCmd = true;
				BtnDgvCmd.BackColor = Color.Gold;

				DgvCmd.Enabled = true;
				DgvCmd.ScrollBars = ScrollBars.Both;
				DgvCmd.Width = 286;
				DgvCmd.Height = 311;
				TbDgvCmdSearch.Visible = true;
			}
		}

		private void DgvCmd_MouseHover(object sender, EventArgs e)
		{
			_ = DgvCmd.Focus();
		}

		private void DgvCmd_Click(object sender, EventArgs e)
		{
			string s1 = DgvCmd[0, DgvCmd.CurrentCell.RowIndex].Value.ToString();
			TbCmd.Text = s1;

			SubTbCmdFocus(-1);

			GblBDgvCmd = true;
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
			string s1 = null;
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
						TbResult.Text = null;
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

					// ソート
					case "#sort":
						TbResult.Text = RtnTextSort(TbResult.Text, false);
						break;

					// ソート後、重複消除
					case "#uniq":
						TbResult.Text = RtnTextSort(TbResult.Text, true);
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

						_ = SB.Clear();
						foreach (string _s1 in TbResult.Text.Split(SPLITS, StringSplitOptions.None))
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
						foreach (string _s1 in TbResult.Text.Split(SPLITS, StringSplitOptions.None))
						{
							string _s2 = _s1.TrimEnd();
							if (_s2.Length > 0)
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
						foreach (string _s1 in TbResult.Text.Split(SPLITS, StringSplitOptions.None))
						{
							_ = SB.Append(string.Format("{0:D8}\t{1}{2}", ++cnt1, _s1, NL));
						}
						TbResult.Text = SB.ToString();
						break;

					// ファイル取得
					case "#wget":
						System.Net.WebClient wc = new System.Net.WebClient();
						try
						{
							string url = aOp[1];
							byte[] data = wc.DownloadData(url);
							if (Regex.IsMatch(url, "^(http|ftp)"))
							{
								s1 = Encoding.GetEncoding(CbTextCode.Text.ToString()).GetString(data);
								if (Regex.IsMatch(s1, "charset.*=.*UTF-8", RegexOptions.IgnoreCase))
								{
									CbTextCode.Text = GblASTextCode[1];
								}
							}
							_ = NativeMethods.SendMessage(
								TbResult.Handle, EM_REPLACESEL, 1,
								Regex.Replace(Encoding.GetEncoding(CbTextCode.Text.ToString()).GetString(data), "\r*\n", NL)
							);
						}
						catch
						{
						}
						wc.Dispose();
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
								P1 = new Process();
								P1.StartInfo.FileName = aOp[1];
								P1.StartInfo.Arguments = aOp[2] + " " + _s1;
								P1.StartInfo.UseShellExecute = false;
								P1.StartInfo.RedirectStandardOutput = true;
								P1.StartInfo.RedirectStandardError = true;
								P1.StartInfo.CreateNoWindow = true;
								P1.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CbTextCode.Text.ToString());
								P1.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);
								_ = P1.Start();
								///TbResult.Text += P1.StandardOutput.ReadToEnd() + NL;
								TbCmdSub.Text += P1.StandardError.ReadToEnd() + NL;
								P1.Close();
							}
						}
						TbCmdSub.SelectionStart = TbCmdSub.TextLength;
						TbCmdSub.ScrollToCaret();
						break;

					// MsgBox
					case "#msgbox":
						DialogResult result = MessageBox.Show(
							aOp[1].Replace("\\n", NL),
							"",
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Asterisk
						);
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
				P1 = new Process();
				P1.StartInfo.FileName = "cmd.exe";
				P1.StartInfo.Arguments = "/C " + cmd;
				P1.StartInfo.UseShellExecute = false;
				P1.StartInfo.RedirectStandardOutput = true;
				P1.StartInfo.RedirectStandardError = true;
				P1.StartInfo.CreateNoWindow = true;
				P1.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CbTextCode.Text.ToString());
				P1.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);
				_ = P1.Start();
				TbResult.Text += P1.StandardOutput.ReadToEnd();
				P1.Close();
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

			TbCmd.Text = TbCmd.Text.Trim();

			// 履歴に追加
			LCmdHistory.Add(TbCmd.Text);
			SubListHistory(LCmdHistory, CbCmdHistory);

			GblCmdExec = true;
			foreach (string _s1 in RtnCmdFormat(TbCmd.Text))
			{
				if (GblCmdExec == true)
				{
					SubTbCmdExec(_s1);
				}
			}
			SubTbCmdFocus(-1);
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

				// 20200229 要経過
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
			TbCmd.Text = null;
			TbCmdSub.Text = null;
			TbResult.Text = null;
			SubTbCmdFocus(-1);
		}

		//-----------
		// TbResult
		//-----------
		private void TbResult_Enter(object sender, EventArgs e)
		{
			GblBDgvMacro = true;
			BtnDgvMacro_Click(sender, e);

			GblBDgvCmd = true;
			BtnDgvCmd_Click(sender, e);
		}

		private void TbResult_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					SubTbCmdFocus(-1);
					break;

				case Keys.PageUp:
					TbResult.SelectionStart = 0;
					break;

				case Keys.PageDown:
					TbResult.SelectionStart = TbResult.TextLength;
					break;

				case Keys.V:
					if (e.Control == true)
					{
						CmsResult_貼り付け_Click(sender, e);
					}
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
				TbResultInfo.Text = null;
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
			TbResultInfo.Text = null;
		}

		private void TbResult_DragDrop(object sender, DragEventArgs e)
		{
			_ = SB.Clear();

			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

			foreach(string _s1 in files)
			{
				foreach (string _s2 in File.ReadLines(_s1, Encoding.GetEncoding(CbTextCode.Text.ToString())))
				{
					_ = SB.Append(_s2.TrimEnd() + NL);
				}
			}

			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, SB.ToString());
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
			TbResult.Text = null;
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
			_ = SB.Clear();

			StringCollection files = Clipboard.GetFileDropList();
			foreach (string _s1 in files)
			{
				_ = SB.Append(_s1 + (Directory.Exists(_s1) ? @"\" : null) + NL);
			}

			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, SB.ToString());
		}

		private void CmsResult_名前を付けて保存_ShiftJIS_Click(object sender, EventArgs e)
		{
			SubTextToSaveFile(TbResult.Text, CmsResult_名前を付けて保存_ShiftJIS.Text);
		}

		private void CmsResult_名前を付けて保存_UTF8N_Click(object sender, EventArgs e)
		{
			SubTextToSaveFile(TbResult.Text, CmsResult_名前を付けて保存_UTF8N.Text);
		}

		//-------
		// 履歴
		//-------
		private void CbCmdHistory_SelectedIndexChanged(object sender, EventArgs e)
		{
			TbCmd.Text = CbCmdHistory.Text;
			SubTbCmdFocus(-1);
		}

		private void CbResultHistory_SelectedIndexChanged(object sender, EventArgs e)
		{
			TbResult.Text = HResultHistory[CbResultHistory.Text.Substring(0, 8)];
			SubTbCmdFocus(-1);
		}

		private void SubListHistory(List<string> l, ComboBox cb)
		{
			string sOld = null;
			List<string> LTmp = new List<string>();

			l.Sort();
			foreach (string _s1 in l)
			{
				if (_s1 != sOld)
				{
					LTmp.Add(_s1);
					sOld = _s1;
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

			try
			{
				HResultHistory.Add(DateTime.Now.ToString("HH:mm:ss"), TbResult.Text);
			}
			catch
			{
			}

			SubDictHistory(HResultHistory, CbResultHistory);
		}

		//-----------------
		// フォントサイズ
		//-----------------
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
			if (iPos < 0)
			{
				TbCmd.Text = TbCmd.Text.Trim();
				if (TbCmd.TextLength > 0)
				{
					TbCmd.Text += " ";
				}
				iPos = TbCmd.TextLength;
			}
			TbCmd.SelectionStart = iPos;
			_ = TbCmd.Focus();
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
				Filter = "All files (*.*)|*.*",
				FilterIndex = 1
			}
			)
			{
				if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					switch (code.ToUpper())
					{
						case "UTF-8N":
							UTF8Encoding utf8nEnc = new UTF8Encoding(false);
							File.WriteAllText(saveFileDialog1.FileName, s, utf8nEnc);
							break;

						default:
							File.WriteAllText(saveFileDialog1.FileName, s, Encoding.GetEncoding("Shift_JIS"));
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

			_ = SB.Clear();

			foreach (string _s1 in str.Split(SPLITS, StringSplitOptions.None))
			{
				string _s2 = _s1 + NL;
				if (bMatch == rgx.IsMatch(_s2))
				{
					_ = SB.Append(_s2);
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

			_ = SB.Clear();

			foreach (string _s1 in str.Split(SPLITS, StringSplitOptions.None))
			{
				_ = SB.Append(rgx.Replace(_s1, sNew) + NL);
			}

			return SB.ToString();
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

			_ = SB.Clear();

			foreach (string _s1 in str.Split(SPLITS, StringSplitOptions.None))
			{
				if (_s1.Trim().Length > 0)
				{
					_ = SB.Append(sQrt + _s1 + sQrt + sNew);
				}
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

			_ = SB.Clear();

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

			_ = SB.Clear();

			foreach (string _s1 in str.Split(SPLITS, StringSplitOptions.None))
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

			string rtn = null;

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

		//-------------
		// Sort／Uniq
		//-------------
		private string RtnTextSort(string str, bool bUniq)
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

			if (bUniq)
			{
				l1 = RtnListUniqSort(l1);
			}

			_ = SB.Clear();

			foreach (string _s1 in l1)
			{
				_ = SB.Append(_s1 + NL);
			}

			return SB.ToString();
		}

		//-----------------------
		// ソートして重複を除外
		//-----------------------
		private List<string> RtnListUniqSort(List<string> ListStr)
		{
			List<string> rtn = new List<string>();

			ListStr.Sort();
			string s1 = null;

			foreach (string _s1 in ListStr)
			{
				if (_s1 != s1)
				{
					rtn.Add(_s1);
					s1 = _s1;
				}
			}

			return rtn;
		}

		//-----------
		// Eval計算
		//-----------
		private string RtnEvalCalc(string str)
		{
			string rtn = str.ToLower().Replace(" ", null);

			// Help
			if (rtn.Length == 0)
			{
				return "pi, e, sin(n), cos(n), tan(n), sqrt(n), pow(n,n)";
			}

			using (DataTable dt = new DataTable())
			{
				try
				{
					// 定数
					rtn = rtn.Replace("pi", Math.PI.ToString());
					rtn = rtn.Replace("e", Math.E.ToString());

					// sin(n) cos(n) tan(n) sqrt(n)
					string[] aMath = { "sin", "cos", "tan", "sqrt" };
					foreach (string _s1 in aMath)
					{
						foreach (Match _m1 in Regex.Matches(rtn, _s1 + @"\(\d+\.*\d*\)"))
						{
							string _s2 = _m1.Value;
							_s2 = _s2.Replace(_s1 + "(", null);
							_s2 = _s2.Replace(")", null);

							double _d1 = double.Parse(_s2);

							switch (_s1)
							{
								case "sin": _d1 = Math.Sin(_d1 * Math.PI / 180); break;
								case "cos": _d1 = Math.Cos(_d1 * Math.PI / 180); break;
								case "tan": _d1 = Math.Tan(_d1 * Math.PI / 180); break;
								case "sqrt": _d1 = Math.Sqrt(_d1); break;
								default: _d1 = 0; break;
							}

							rtn = rtn.Replace(_m1.Value, _d1.ToString());
						}
					}

					// pow(n,n)
					foreach (Match _m1 in Regex.Matches(rtn, @"pow\(\d+\.*\d*\,\d+\)"))
					{
						string _s2 = _m1.Value;
						_s2 = _s2.Replace("pow(", null);
						_s2 = _s2.Replace(")", null);

						string[] _a1 = _s2.Split(',');
						rtn = rtn.Replace(_m1.Value, Math.Pow(double.Parse(_a1[0]), int.Parse(_a1[1])).ToString());
					}

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
