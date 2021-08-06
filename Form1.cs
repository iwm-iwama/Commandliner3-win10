using Microsoft.VisualBasic; // プロジェクト～参照の追加

using System;
using System.Collections.Generic;
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
using System.Web;
using System.Windows.Forms;

namespace iwm_Commandliner3
{
	public partial class Form1 : Form
	{
		//--------------------------------------------------------------------------------
		// 大域定数
		//--------------------------------------------------------------------------------
		private const string ProgramID = "iwm_Commandliner3.2";
		private const string VERSION = "Ver.20210806 'A-29' (C)2018-2021 iwm-iwama";
		// 履歴
		//  Ver.20210806
		//  Ver.20210801
		//  Ver.20210731
		//  Ver.20210715
		//  Ver.20210613
		//  Ver.20210601
		//  Ver.20210529
		//  Ver.20210521
		//  Ver.20210505
		//  Ver.20210414

		private const string ConfigFn = "config.iwmcmd";

		private const string NL = "\r\n";
		private const string RgxNL = "\r??\n";
		private const string RgxCmdNL = "(;|\\s)*\n";

		private readonly string[] TEXT_CODE = { "Shift_JIS", "UTF-8" };

		private readonly object[,] MACRO = {
			// [マクロ]      [説明]                                                                                        [引数]
			{ "#clear",      "全クリア       #clear",                                                                         0 },
			{ "#cls",        "出力クリア     #cls",                                                                           0 },
			{ "#cd",         "フォルダ変更   #cd \"..\" ※フォルダがないときは新規作成します。",                              1 },
			{ "#code",       "文字コード     #code \"Shift_JIS\" | \"UTF-8\"",                                                1 },
			{ "#print",      "印字           #print \"#{result,2}\" ※\"出力\" 1..5",                                         1 },
			{ "#serial",     "連番生成       #serial \"1\" \"100\" \"4\" ※\"[開始番号]\" \"[終了番号]\" \"[ゼロ埋め桁数]\"", 3 },
			{ "#result",     "出力変更       #result \"2\" ※\"出力\" 1..5",                                                  1 },
			{ "#grep",       "検索           #grep \"\\d{4}\"   ※正規表現",                                                  1 },
			{ "#except",     "不一致検索     #except \"\\d{4}\" ※正規表現",                                                  1 },
			{ "#replace",    "置換           #replace \"(\\d{2})(\\d{2})\" \"$1+$2\" ※正規表現 $1..9",                       2 },
			{ "#split",      "分割           #split \"\\t\" \"[LN],[0],[1]\" ※正規表現 [LN]行番号 [0..]分割列",              2 },
			{ "#trim",       "行前後の空白クリア",                                                                            0 },
			{ "#toUpper",    "大文字に変換",                                                                                  0 },
			{ "#toLower",    "小文字に変換",                                                                                  0 },
			{ "#toWide",     "全角に変換",                                                                                    0 },
			{ "#toZenNum",   "全角に変換(数字のみ)",                                                                          0 },
			{ "#toZenKana",  "全角に変換(カナのみ)",                                                                          0 },
			{ "#toNarrow",   "半角に変換",                                                                                    0 },
			{ "#toHanNum",   "半角に変換(数字のみ)",                                                                          0 },
			{ "#toHanKana",  "半角に変換(カナのみ)",                                                                          0 },
			{ "#erase",      "文字クリア     #erase \"0\" \"5\" ※\"[開始位置]\" \"[文字長]\"",                               2 },
			{ "#sort",       "ソート(昇順)",                                                                                  0 },
			{ "#sort-r",     "ソート(降順)",                                                                                  0 },
			{ "#uniq",       "重複行をクリア ※データ全体の重複をクリアするときは #sort と併用",                              0 },
			{ "#rmBlankLn",  "空白行削除",                                                                                    0 },
			{ "#rmNL",       "改行をクリア",                                                                                  0 },
			{ "#wget",       "ファイル取得   #wget \"http://.../index.html\"",                                                1 },
			{ "#fread",      "ファイル読込   #fread \"ファイル名\"",                                                          1 },
			{ "#fwrite",     "ファイル書込   #fwrite \"ファイル名\"",                                                         1 },
			{ "#dfList",     "フォルダ・ファイル一覧 #dfList \"フォルダ名\"",                                                 1 },
			{ "#dList",      "フォルダ一覧           #dList \"フォルダ名\"",                                                  1 },
			{ "#fList",      "ファイル一覧           #fList \"フォルダ名\"",                                                  1 },
			{ "#rename",     "ファイル名置換 #rename \"(.+)\" \"#{line,4}_$1\" ※正規表現 $1..9",                             2 },
			{ "#stream",     "行毎に処理     #stream \"dir \\\"#{}\\\"\" ※ #{} は出力行データ変数",                          1 },
			{ "#set",        "連想配列       #set \"japan\" \"日本\" => #{%japan} で参照／#set でリスト表示",                 2 },
			{ "#calc",       "計算機         #calc \"pi / 180\"",                                                             1 },
			{ "#pos",        "フォーム位置   #pos \"50\" \"100\" ※\"[横位置(X)]\" \"[縦位置(Y)]\"",                          2 },
			{ "#size",       "フォームサイズ #size \"600\" \"600\" ※\"[幅(Width)]\" \"[高さ(Height)]\"",                     2 },
			{ "#sizeMax",    "フォームサイズ（最大化）",                                                                      0 },
			{ "#sizeMin",    "フォームサイズ（最小化）",                                                                      0 },
			{ "#sizeNormal", "フォームサイズ（普通）",                                                                        0 },
			{ "#focus0",     "出力のフォーカス位置を先頭にする",                                                              0 },
			{ "#focus9",     "出力のフォーカス位置を末尾にする",                                                              0 },
			{ "#macroList",  "マクロ一覧",                                                                                    0 },
			{ "#help",       "操作説明",                                                                                      0 },
			{ "#version",    "バージョン",                                                                                    0 },
			{ "#exit",       "終了",                                                                                          0 }
		};

		private const string CmdFile_FILTER = "Command (*.iwmcmd)|*.iwmcmd|All files (*.*)|*.*";

		//--------------------------------------------------------------------------------
		// 大域変数
		//--------------------------------------------------------------------------------
		// エラーが発生したとき
		private bool ExecStopOn = false;

		// CurDir
		private string CurDir = "";

		// 文字列
		private readonly StringBuilder SB = new StringBuilder();

		// 履歴
		private List<string> ListCmdHistory = new List<string>();
		private readonly SortedDictionary<string, string> DictResultHistory = new SortedDictionary<string, string>();

		// Object
		private Process PS = null;
		private object OBJ = null;

		// 連想配列
		private readonly SortedDictionary<string, string> DictHash = new SortedDictionary<string, string>();

		internal static class NativeMethods
		{
			[DllImport("User32.dll", CharSet = CharSet.Unicode)]
			internal static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
		}

		private const int EM_REPLACESEL = 0x00C2;

		//--------------------------------------------------------------------------------
		// Help
		//--------------------------------------------------------------------------------
		private const string HELP_TbCmd =
			"--------------" + NL +
			"> マクロ入力 <" + NL +
			"--------------" + NL +
			"(例) #cd \"..\"" + NL +
			"          ↑引数はダブルクォーテーションで囲む。" + NL +
			NL +
			"----------------" + NL +
			"> コマンド入力 <" + NL +
			"----------------" + NL +
			"(例１) dir .. /b" + NL +
			"(例２) dir \"C:\\Program Files\"" + NL +
			NL +
			"--------------------------------" + NL +
			"> 複数のマクロ・コマンドを入力 <" + NL +
			"--------------------------------" + NL +
			"(例) #cls; dir; #cd \"#{ymd}_#{hns}\"; dir" + NL +
			"         ↑「;」で区切る。" + NL +
			NL +
			"-------------------------------------" + NL +
			"> マクロ・コマンド入力 特殊キー操作 <" + NL +
			"-------------------------------------" + NL +
			"[PgUp] or [PgDn]     スペース位置へカーソル移動" + NL +
			NL +
			"[Ctrl]+[↑]          履歴表示" + NL +
			"[Ctrl]+[↓]          クリア" + NL +
			"[Ctrl]+[Space]       全クリア" + NL +
			"[Ctrl]+[Delete]      カーソルより後をクリア" + NL +
			"[Ctrl]+[Backspace]   カーソルより前をクリア" + NL +
			NL +
			"[F1]  マクロ・コマンド入力履歴" + NL +
			"[F2]  マクロ選択" + NL +
			"[F3]  コマンド選択" + NL +
			"[F4]  出力文字コード" + NL +
			"[F5]  実行" + NL +
			"[F6]  メモを出力にコピー" + NL +
			"[F7]  出力をクリア" + NL +
			"[F8]  出力履歴" + NL +
			"[F9]  出力を直前に戻す" + NL +
			"[F10] システムメニュー" + NL +
			"[F11] →メモ→出力の順にフォーカス移動" + NL +
			"[F12] 出力変更" + NL +
			NL +
			"---------------------------" + NL +
			"> マクロ選択 特殊キー操作 <" + NL +
			"---------------------------" + NL +
			"[←]                 最上位へ移動" + NL +
			"[→]                 最下位へ移動" + NL +
			"[A]..[Z]             検索" + NL +
			"[Shift]+[A]..[Z]     逆順で検索" + NL +
			NL +
			"-----------------------------" + NL +
			"> コマンド選択 特殊キー操作 <" + NL +
			"-----------------------------" + NL +
			"[←]                 最上位へ移動" + NL +
			"[→]                 最下位へ移動" + NL +
			NL +
			"--------------" + NL +
			"> マクロ変数 <" + NL +
			"--------------" + NL +
			"  #{\\t}   タブ \\t" + NL +
			"  #{\\n}   改行 \\n" + NL +
			"  #{ymd}  年月日     (例) 20210501" + NL +
			"  #{hns}  時分秒     (例) 113400" + NL +
			"  #{msec} マイクロ秒 (例) 999" + NL +
			"  #{y}    年         (例) 2021" + NL +
			"  #{m}    月         (例) 05" + NL +
			"  #{d}    日         (例) 01" + NL +
			"  #{h}    時         (例) 11" + NL +
			"  #{n}    分         (例) 34" + NL +
			"  #{s}    秒         (例) 00" + NL +
			"  #{環境変数}        (例) #{OS} => Windows_NT" + NL +
			"  #{result,[NUM]}    出力[NUM]のデータ" + NL +
			"  #{%[STR]}          #set で登録された連想配列データ" + NL +
			NL +
			"  ★ #replace, #rename, #stream で使用可" + NL +
			"    #{line,[NUM]}    出力の行番号（[NUM]でゼロ埋め桁数指定）" + NL +
			NL +
			"  ★ #stream のみで使用可" + NL +
			"    #{}              出力の行データ" + NL +
			NL +
			"----------------" + NL +
			"> 設定ファイル <" + NL +
			"----------------" + NL +
			"作業フォルダに " + ConfigFn + " ファイルが存在するときは、自動的に読み込みます。" + NL +
			"フォーム位置・サイズを指定するときに使用します。" + NL
		;

		private const string HELP_CmsCmd =
			"--------------------------------" + NL +
			"> マクロ・コマンドをグループ化 <" + NL +
			"--------------------------------" + NL +
			"  // から始まる行はコメント" + NL +
			"  行末に ; を記述" + NL +
			NL +
			"【ファイル記述例】" + NL +
			"  // サンプル;" + NL +
			"  #cls;" + NL +
			"  #code \"Shift_JIS\";" + NL +
			"  dir;" + NL +
			"  #grep \"^20\";" + NL +
			"  #replace \"^20(\\d+)\" \"'$1\";" + NL
		;

		//--------------------------------------------------------------------------------
		// Form
		//--------------------------------------------------------------------------------
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			// タイトル表示
			Text = ProgramID;

			// 表示位置
			StartPosition = FormStartPosition.Manual;
			SubForm1_StartPosition();

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

			// TbResult
			SubTbResultChange(0, true);

			// PanelResult
			PanelResult.Visible = false;

			// TbInfo
			SubTbResultCnt();

			// フォントサイズ
			NudTbResult.Value = (int)Math.Round(TbResult.Font.Size);

			// 初フォーカス
			SubTbCmdFocus(-1);

			// 設定ファイルが存在するとき
			if (File.Exists(ConfigFn))
			{
				using (StreamReader sr = new StreamReader(ConfigFn, Encoding.GetEncoding("Shift_JIS")))
				{
					TbCmd.Text = Regex.Replace(sr.ReadToEnd(), RgxCmdNL, ";");
				}
				BtnCmdExec_Click(sender, e);
			}

			// 引数によるバッチ処理
			if (Let.cmd.Length > 0)
			{
				TbCmd.Text = Let.cmd;
				BtnCmdExec_Click(sender, e);
			}
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

		private void SubForm1_StartPosition()
		{
			int WorkingAreaW = Screen.PrimaryScreen.WorkingArea.Width;
			int WorkingAreaH = Screen.PrimaryScreen.WorkingArea.Height;

			int WorkingAreaX = Screen.PrimaryScreen.WorkingArea.X;
			int WorkingAreaY = Screen.PrimaryScreen.WorkingArea.Y;

			int MouseX = Cursor.Position.X;
			int MouseY = Cursor.Position.Y;

			// X = Width
			if (WorkingAreaW < MouseX + Size.Width)
			{
				MouseX -= Size.Width;
				if (MouseX < 0)
				{
					MouseX = WorkingAreaX + 10;
				}
			}

			// Y = Height
			if (WorkingAreaH < MouseY + Size.Height)
			{
				MouseY -= Size.Height;
				if (MouseY < 0)
				{
					MouseY = WorkingAreaY + 10;
				}
			}

			Location = new Point(MouseX, MouseY);
		}

		//--------------------------------------------------------------------------------
		// TbCurDir
		//--------------------------------------------------------------------------------
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
			FolderBrowserDialog fbd = new FolderBrowserDialog
			{
				Description = "フォルダを指定してください。",
				RootFolder = Environment.SpecialFolder.MyComputer,
				SelectedPath = Environment.CurrentDirectory,
				ShowNewFolderButton = true
			};

			if (fbd.ShowDialog(this) == DialogResult.OK)
			{
				CurDir = TbCurDir.Text = fbd.SelectedPath;
				Directory.SetCurrentDirectory(CurDir);
			}

			SubTbCmdFocus(-1);
		}

		private void TbCurDir_MouseHover(object sender, EventArgs e)
		{
			int i1 = 0;
			string s1 = "";
			foreach (string _s1 in TbCurDir.Text.Split('\\'))
			{
				s1 += $"[{i1}]  {_s1}{NL}";
				++i1;
			}
			ToolTip1.SetToolTip(TbCurDir, s1);
		}

		//--------------------------------------------------------------------------------
		// CmsTbCurDir
		//--------------------------------------------------------------------------------
		private void CmsTbCurDir_全コピー_Click(object sender, EventArgs e)
		{
			TbCurDir.SelectAll();
			TbCurDir.Copy();
		}

		//--------------------------------------------------------------------------------
		// TbCmd
		//--------------------------------------------------------------------------------
		// RichTextBox化すると操作のたび警告音が発生し、やかましくてしょうがない!!
		// 正攻法での解決策を見出せなかったので、TextBoxでの実装にとどめることにした。
		//--------------------------------------------------------------------------------
		private string TbCmdUndo = "";
		private string TbCmdRedo = "";

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

		private void TbCmd_TextChanged(object sender, EventArgs e)
		{
			// 日本語入力時の[Enter]で本イベントは発生しない（＝改行されない）ので、"\n" の有無で入力モードの判定可能。
			if (TbCmd.Text.IndexOf("\n") >= 0)
			{
				TbCmd.Text = Regex.Replace(TbCmd.Text, RgxNL, "");
				TbCmd.SelectionStart = GblTbCmdPos;
			}
		}

		private void TbCmd_KeyDown(object sender, KeyEventArgs e)
		{
			GblTbCmdPos = TbCmd.SelectionStart;

			// [Ctrl]+[V]
			if (e.KeyData == (Keys.Control | Keys.V))
			{
				Clipboard.SetText(Regex.Replace(Clipboard.GetText(), RgxNL, " "));
				return;
			}
		}

		private void TbCmd_KeyPress(object sender, KeyPressEventArgs e)
		{
			switch (e.KeyChar)
			{
				// IME入力対策
				case (char)Keys.Enter:
					// TextChanged と処理を分担しないと、日本語入力時のカーソル移動に不具合が発生する
					BtnCmdExec_Click(sender, null);
					break;
			}
		}

		private void TbCmd_KeyUp(object sender, KeyEventArgs e)
		{
			// [Ctrl]+[Y]
			if (e.KeyData == (Keys.Control | Keys.Y))
			{
				TbCmd.Text = TbCmdRedo;
				SubTbCmdFocus(-1);
				return;
			}

			// [Ctrl]+[Z]
			if (e.KeyData == (Keys.Control | Keys.Z) && TbCmdUndo.Length > 0)
			{
				if (TbCmdUndo != TbCmd.Text)
				{
					TbCmdRedo = TbCmd.Text;
				}
				TbCmd.Text = TbCmdUndo;
				SubTbCmdFocus(-1);
				return;
			}

			// [Ctrl]+[↑]
			if (e.KeyData == (Keys.Control | Keys.Up))
			{
				CbCmdHistory.DroppedDown = true;
				_ = CbCmdHistory.Focus();
				return;
			}

			// [Ctrl]+[↓]
			if (e.KeyData == (Keys.Control | Keys.Down))
			{
				// [Ctrl]+[Z] 有効化
				TbCmdUndo = TbCmd.Text;
				TbCmd.Text = "";
				return;
			}

			// [Ctrl]+[Space]
			if (e.KeyData == (Keys.Control | Keys.Space))
			{
				TbCmd.Text = "#clear";
				BtnCmdExec_Click(sender, e);
				return;
			}

			// [Ctrl]+[Backspace]
			if (e.KeyData == (Keys.Control | Keys.Back))
			{
				TbCmd.Text = TbCmd.Text.Substring(TbCmd.SelectionStart);
				return;
			}

			// [Ctrl]+[Delete]
			if (e.KeyData == (Keys.Control | Keys.Delete))
			{
				TbCmd.Text = TbCmd.Text.Substring(0, TbCmd.SelectionStart);
				SubTbCmdFocus(-1);
				return;
			}

			MatchCollection Mc;
			int iPos;

			switch (e.KeyCode)
			{
				case Keys.F1:
					CbCmdHistory.DroppedDown = true;
					_ = CbCmdHistory.Focus();
					break;

				case Keys.F2:
					BtnDgvMacro_Click(sender, e);
					break;

				case Keys.F3:
					BtnDgvCmd_Click(sender, e);
					break;

				case Keys.F4:
					CbTextCode.DroppedDown = true;
					_ = CbTextCode.Focus();
					break;

				case Keys.F5:
					BtnCmdExec_Click(sender, e);
					break;

				case Keys.F6:
					BtnMemoCopy_Click(sender, e);
					break;

				case Keys.F7:
					BtnClear_Click(sender, e);
					break;

				case Keys.F8:
					CbResultHistory.DroppedDown = true;
					_ = CbResultHistory.Focus();
					break;

				case Keys.F9:
					BtnResultMem_Click(sender, e);
					break;

				case Keys.F10: // システムメニュー表示
					SendKeys.Send("{UP}");
					break;

				case Keys.F11:
					_ = RtbCmdMemo.Focus();
					break;

				case Keys.F12:
					SubTbResultNext();
					break;

				case Keys.Enter:
					// 実行は KeyPress で行う
					break;

				case Keys.PageUp:
					Mc = Regex.Matches(TbCmd.Text.Substring(0, TbCmd.SelectionStart), @"\S+\s*$");
					TbCmd.SelectionStart = Mc.Count > 0 ? Mc[0].Index : 0;
					break;

				case Keys.PageDown:
					iPos = TbCmd.SelectionStart;
					Mc = Regex.Matches(TbCmd.Text.Substring(iPos), @"\s\S+");
					TbCmd.SelectionStart = Mc.Count > 0 ? iPos + 1 + Mc[0].Index : TbCmd.TextLength;
					break;

				case Keys.Up:
					TbCmd.SelectionStart = 0;
					break;

				case Keys.Down:
					TbCmd.SelectionStart = TbCmd.TextLength;
					break;
			}

			// 補完(*)
			if (TbCmd.TextLength == TbCmd.SelectionStart && e.KeyData != Keys.Delete && e.KeyData != Keys.Back)
			{
				// (例) "#grep "（末尾は半角スペース）
				// "#"以降は最短一致
				Regex rgx = new Regex(@".*(?<macro>#\S+?)\s+$", RegexOptions.IgnoreCase);
				Match match = rgx.Match(TbCmd.Text);

				if (match.Success)
				{
					string macro = match.Groups["macro"].Value;
					string sSpace = "";
					int iSpace = 0;

					// 引数分の "" 付与
					for (int _i1 = 0; _i1 < MACRO.Length / 3; _i1++)
					{
						if (MACRO[_i1, 0].ToString().ToLower() == macro.ToLower() && (int)MACRO[_i1, 2] > 0)
						{
							iSpace = (int)MACRO[_i1, 2];

							for (int _i2 = 0; _i2 < iSpace; _i2++)
							{
								sSpace += "\"\" ";
							}

							break;
						}
					}

					// 余分なスペース削除
					TbCmd.Text = TbCmd.Text.Trim() + " " + sSpace.TrimEnd();

					// カーソル位置
					TbCmd.SelectionStart = TbCmd.TextLength - (iSpace * 3) + 2;
				}

				TbCmd.ForeColor = Color.Black;
			}
			else
			{
				TbCmd.ForeColor = Color.Red;
			}

			TbCmd.ScrollToCaret();
		}

		private void TbCmd_MouseHover(object sender, EventArgs e)
		{
			string[] a1 = Regex.Split(TbCmd.Text, "\\s+(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
			for (int _i1 = 0; _i1 < a1.Length; _i1++)
			{
				a1[_i1] = Regex.Replace(a1[_i1], @";\s*$", "\a");
			}
			string s1 = string.Join(" ", a1);
			s1 = Regex.Replace(s1, @"\a\s*", ";" + NL);
			ToolTip1.SetToolTip(TbCmd, s1);
		}

		private void TbCmd_MouseUp(object sender, MouseEventArgs e)
		{
			CmsTextSelect_Open(e, TbCmd);
		}

		private void TbCmd_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		private void TbCmd_DragDrop(object sender, DragEventArgs e)
		{
			string s1 = "";

			foreach (string _s1 in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				s1 += $" \"{_s1}\"";
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

		private string TbCmdFormat()
		{
			string rtn = "";
			foreach (string _s1 in TbCmd.Text.Split(';'))
			{
				string _s2 = _s1.Trim();
				if (_s2.Length > 0 && _s2 != ";")
				{
					rtn += $"{_s2};{NL}";
				}
			}
			return rtn;
		}

		//--------------------------------------------------------------------------------
		// CmsCmd
		//--------------------------------------------------------------------------------
		private string GblCmsCmdBatch = "";

		private void CmsCmd_全クリア_Click(object sender, EventArgs e)
		{
			// [Ctrl]+[Z] 有効化
			TbCmdUndo = TbCmd.Text;
			TbCmd.Text = "";
		}

		private void CmsCmd_全コピー_Click(object sender, EventArgs e)
		{
			TbCmd.SelectAll();
			TbCmd.Copy();
		}

		private void CmsCmd_上書き_Click(object sender, EventArgs e)
		{
			// [Ctrl]+[Z] 有効化
			TbCmdUndo = TbCmd.Text;
			TbCmd.Text = "";
			CmsCmd_貼り付け_Click(sender, e);
		}

		private void CmsCmd_貼り付け_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(Regex.Replace(Clipboard.GetText(), RgxNL, " "));
			TbCmd.Paste();
		}

		private void CmsCmd_マクロ変数_タブ_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{\\t}");
		}

		private void CmsCmd_マクロ変数_改行_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{\\n}");
		}

		private void CmsCmd_マクロ変数_日付_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{ymd}");
		}

		private void CmsCmd_マクロ変数_時間_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{hns}");
		}

		private void CmsCmd_マクロ変数_マイクロ秒_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{msec}");
		}

		private void CmsCmd_マクロ変数_年_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{y}");
		}

		private void CmsCmd_マクロ変数_月_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{m}");
		}

		private void CmsCmd_マクロ変数_日_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{d}");
		}

		private void CmsCmd_マクロ変数_時_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{h}");
		}

		private void CmsCmd_マクロ変数_分_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{n}");
		}

		private void CmsCmd_マクロ変数_秒_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{s}");
		}

		private void CmsCmd_マクロ変数_出力のデータ_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{result,}");
		}

		private void CmsCmd_マクロ変数_出力の行番号_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{line,}");
		}

		private void CmsCmd_マクロ変数_出力の行データ_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{}");
		}

		private void CmsCmd_マクロ変数_連想配列_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("#{%}");
		}

		private void CmsCmd_文字コード_SJIS_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("Shift_JIS");
		}

		private void CmsCmd_文字コード_UTF8_Click(object sender, EventArgs e)
		{
			CmsCmd_InsertText("UTF-8");
		}

		private void CmsCmd_フォルダ選択_Click(object sender, EventArgs e)
		{
			int iPos = TbCmd.SelectionStart;

			FolderBrowserDialog fbd = new FolderBrowserDialog
			{
				Description = "フォルダを指定してください。",
				RootFolder = Environment.SpecialFolder.MyComputer,
				SelectedPath = Environment.CurrentDirectory,
				ShowNewFolderButton = true
			};

			if (fbd.ShowDialog(this) == DialogResult.OK)
			{
				string s1 = "", s2 = "";

				if (TbCmd.Text.Substring(iPos - 1, 1) != " ")
				{
					s1 = " ";
				}

				if (iPos < TbCmd.TextLength && TbCmd.Text.Substring(iPos, 1) != " ")
				{
					s2 = " ";
				}

				int i1 = iPos + fbd.SelectedPath.Length + 3;
				TbCmd.Text = TbCmd.Text.Substring(0, iPos) + s1 + "\"" + fbd.SelectedPath + "\"" + s2 + TbCmd.Text.Substring(iPos);
				TbCmd.SelectionStart = i1;
			}
		}

		private void CmsCmd_ファイル選択_Click(object sender, EventArgs e)
		{
			int iPos = TbCmd.SelectionStart;

			OpenFileDialog ofd = new OpenFileDialog
			{
				InitialDirectory = Environment.CurrentDirectory,
				Multiselect = true
			};

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				string sFn = "";

				foreach (string _s1 in ofd.FileNames)
				{
					sFn += $"\"{_s1}\" ";
				}

				sFn = sFn.TrimEnd();

				string s1 = "", s2 = "";

				if (TbCmd.Text.Substring(iPos - 1, 1) != " ")
				{
					s1 = " ";
				}

				if (iPos < TbCmd.TextLength && TbCmd.Text.Substring(iPos, 1) != " ")
				{
					s2 = " ";
				}

				int i1 = iPos + sFn.Length + 3;
				TbCmd.Text = TbCmd.Text.Substring(0, iPos) + s1 + sFn + s2 + TbCmd.Text.Substring(iPos);
				TbCmd.SelectionStart = i1;
			}
		}

		private void CmsCmd_コマンドをグループ化_追加_Click(object sender, EventArgs e)
		{
			string s1 = TbCmd.Text.Trim();
			if (s1.Length == 0)
			{
				return;
			}
			GblCmsCmdBatch += $"{s1}; ";
			CmsCmd_コマンドをグループ化_追加.ToolTipText = CmsCmd_コマンドをグループ化_出力.ToolTipText = GblCmsCmdBatch;
		}

		private void CmsCmd_コマンドをグループ化_出力_Click(object sender, EventArgs e)
		{
			TbCmd.Text = GblCmsCmdBatch.Trim();
			SubTbCmdFocus(-1);
		}

		private void CmsCmd_コマンドをグループ化_クリア_Click(object sender, EventArgs e)
		{
			GblCmsCmdBatch = CmsCmd_コマンドをグループ化_追加.ToolTipText = CmsCmd_コマンドをグループ化_出力.ToolTipText = "";
		}

		private void CmsCmd_コマンドをグループ化_簡単な説明_Click(object sender, EventArgs e)
		{
			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, NL + HELP_CmsCmd + NL);
		}

		private void CmsCmd_コマンドを保存_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog
			{
				FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".iwmcmd",
				Filter = CmdFile_FILTER,
				FilterIndex = 1,
				InitialDirectory = Environment.CurrentDirectory
			};

			if (sfd.ShowDialog() == DialogResult.OK)
			{
				File.WriteAllText(sfd.FileName, TbCmdFormat(), Encoding.GetEncoding(TEXT_CODE[0]));
			}
		}

		private void CmsCmd_コマンドを読込_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog
			{
				Filter = CmdFile_FILTER,
				InitialDirectory = Environment.CurrentDirectory
			};

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				TbCmd.Text = Regex.Replace(
					File.ReadAllText(ofd.FileName, Encoding.GetEncoding(TEXT_CODE[0])),
					RgxCmdNL,
					"; "
				);
				SubTbCmdFocus(-1);
			}
		}

		private void CmsCmd_InsertText(string s)
		{
			int iPos = TbCmd.SelectionStart;
			TbCmd.Text = TbCmd.Text.Substring(0, iPos) + s + TbCmd.Text.Substring(iPos);
			TbCmd.SelectionStart = iPos + s.Length;
		}

		//--------------------------------------------------------------------------------
		// RtbCmdMemo
		//--------------------------------------------------------------------------------
		private void RtbCmdMemo_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.F11:
					_ = TbResult.Focus();
					break;

				case Keys.F12:
					SubTbResultNext();
					break;
			}
		}

		private void RtbCmdMemo_Enter(object sender, EventArgs e)
		{
			LblCmdMemo.Visible = true;
		}

		private void RtbCmdMemo_Leave(object sender, EventArgs e)
		{
			LblCmdMemo.Visible = false;
		}

		private void RtbCmdMemo_MouseUp(object sender, MouseEventArgs e)
		{
			CmsTextSelect_Open(e, RtbCmdMemo);
		}

		//--------------------------------------------------------------------------------
		// CmsCmdMemo
		//--------------------------------------------------------------------------------
		private void CmsCmdMemo_全クリア_Click(object sender, EventArgs e)
		{
			RtbCmdMemo.Text = "";
		}

		private void CmsCmdMemo_全コピー_Click(object sender, EventArgs e)
		{
			RtbCmdMemo.SelectAll();
			RtbCmdMemo.Copy();
		}

		private void CmsCmdMemo_上書き_Click(object sender, EventArgs e)
		{
			RtbCmdMemo.Text = "";
			CmsCmdMemo_貼り付け_Click(sender, e);
		}

		private void CmsCmdMemo_貼り付け_Click(object sender, EventArgs e)
		{
			RtbCmdMemo.Paste();
		}

		//--------------------------------------------------------------------------------
		// DgvMacro
		//--------------------------------------------------------------------------------
		private int GblDgvMacroRow = 0;
		private bool GblDgvMacroOpen = true; // DgvMacro.enabled より速い

		private void BtnDgvMacro_Click(object sender, EventArgs e)
		{
			if (GblDgvMacroOpen)
			{
				GblDgvMacroOpen = false;
				DgvMacro.Enabled = false;
				BtnDgvMacro.BackColor = Color.LightYellow;
				BtnDgvMacro.ForeColor = Color.LightYellow;
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
				BtnDgvMacro.ForeColor = Color.Black;
				DgvMacro.ScrollBars = ScrollBars.Both;
				DgvMacro.Width = Width - 112;

				int i1 = DgvTb11.Width + DgvTb12.Width + 20;
				if (DgvMacro.Width > i1)
				{
					DgvMacro.Width = i1;
				}

				DgvMacro.Height = Height - 220;

				TbDgvCmdSearch.SendToBack();
				_ = DgvMacro.Focus();
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

		private void DgvMacro_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			DgvMacro.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "[Ctrl]を押しながら選択すると挿入モード";
		}

		private void DgvMacro_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			// [説明]
			if (DgvMacro.CurrentCellAddress.X == 1)
			{
				return;
			}

			string s1 = DgvMacro[0, DgvMacro.CurrentRow.Index].Value.ToString();
			int iPosForward = 0;

			for (int _i1 = 0; _i1 < MACRO.Length / 3; _i1++)
			{
				if (s1 == MACRO[_i1, 0].ToString())
				{
					int _i2 = (int)MACRO[_i1, 2];
					if (_i2 > 0)
					{
						for (int _i3 = 0; _i3 < _i2; _i3++)
						{
							s1 += " \"\"";
						}
						iPosForward = ((_i2 - 1) * 3) + 1;
					}
					break;
				}
			}

			// [Ctrl]+ のときは挿入モード／それ以外は上書き
			if ((ModifierKeys & Keys.Control) == Keys.Control)
			{
				int iPos = TbCmd.SelectionStart;
				string s2 = TbCmd.Text.Substring(0, iPos);
				string s3 = TbCmd.Text.Substring(iPos);

				TbCmd.Text = s2 + s1 + ";" + s3;
				GblTbCmdPos = s2.Length + s1.Length - iPosForward;
			}
			else
			{
				TbCmd.Text = s1;
				GblTbCmdPos = s1.Length - iPosForward;
			}

			GblDgvMacroOpen = true;
			BtnDgvMacro_Click(sender, e);
		}

		private void DgvMacro_KeyDown(object sender, KeyEventArgs e)
		{
			GblDgvMacroRow = DgvMacro.CurrentRow.Index;
		}

		private void DgvMacro_KeyUp(object sender, KeyEventArgs e)
		{
			// [A..Z] のときは、#マクロ名を検索
			if (e.KeyData >= Keys.A && e.KeyData <= Keys.Z)
			{
				// 現在位置の次のセルから検索
				int iY = DgvMacro.CurrentCellAddress.Y + 1;
				int iLoopCnt = 0;

				while (true)
				{
					// Loop
					if (iY >= DgvMacro.RowCount)
					{
						iY = 0;
					}
					// 一周して見つからないときは break
					if (iLoopCnt > DgvMacro.RowCount)
					{
						break;
					}
					// 見つかったらフォーカス移動
					if (Regex.IsMatch(DgvMacro[0, iY].Value.ToString(), "^#" + e.KeyCode.ToString(), RegexOptions.IgnoreCase))
					{
						DgvMacro.CurrentCell = DgvMacro[0, iY];
						break;
					}
					++iY;
					++iLoopCnt;
				}
			}

			// [Shift]+[A..Z] のときは、#マクロ名を逆順で検索
			if (e.KeyData >= (Keys.Shift | Keys.A) && e.KeyData <= (Keys.Shift | Keys.Z))
			{
				// 現在位置の次のセルから検索
				int iY = DgvMacro.CurrentCellAddress.Y - 1;
				int iLoopCnt = 0;

				while (true)
				{
					// Loop
					if (iY < 0)
					{
						iY = DgvMacro.RowCount - 1;
					}
					// 一周して見つからないときは break
					if (iLoopCnt > DgvMacro.RowCount)
					{
						break;
					}
					// 見つかったらフォーカス移動
					if (Regex.IsMatch(DgvMacro[0, iY].Value.ToString(), "^#" + e.KeyCode.ToString(), RegexOptions.IgnoreCase))
					{
						DgvMacro.CurrentCell = DgvMacro[0, iY];
						break;
					}
					--iY;
					++iLoopCnt;
				}
			}

			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.F2:
					BtnDgvMacro_Click(sender, e);
					break;

				case Keys.F3:
					BtnDgvMacro_Click(sender, e);
					BtnDgvCmd_Click(sender, e);
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

		//--------------------------------------------------------------------------------
		// DgvCmd
		//--------------------------------------------------------------------------------
		private int GblDgvCmdRow = 0;
		private bool GblDgvCmdOpen = true; // DgvCmd.enabled より速い

		private void BtnDgvCmd_Click(object sender, EventArgs e)
		{
			if (GblDgvCmdOpen)
			{
				GblDgvCmdOpen = false;
				DgvCmd.Enabled = false;
				BtnDgvCmd.BackColor = Color.LightYellow;
				BtnDgvCmd.ForeColor = Color.LightYellow;
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
				BtnDgvCmd.ForeColor = Color.Black;
				DgvCmd.ScrollBars = ScrollBars.Both;
				DgvCmd.Width = Width - 197;

				int i1 = DgvTb21.Width + 20;
				if (DgvCmd.Width > i1)
				{
					DgvCmd.Width = i1;
				}

				DgvCmd.Height = Height - 220;
				TbDgvCmdSearch.Visible = true;

				TbDgvCmdSearch.BringToFront();
				_ = TbDgvCmdSearch.Focus();
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

		private void DgvCmd_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			DgvCmd.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "[Ctrl]を押しながら選択すると挿入モード";
		}

		private void DgvCmd_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			string s1 = DgvCmd[0, DgvCmd.CurrentCell.RowIndex].Value.ToString();

			// [Ctrl]+ のときは挿入モード／それ以外は上書き
			if ((ModifierKeys & Keys.Control) == Keys.Control)
			{
				int iPos = TbCmd.SelectionStart;
				string s2 = TbCmd.Text.Substring(0, iPos);
				string s3 = TbCmd.Text.Substring(iPos);

				TbCmd.Text = s2 + s1 + ";" + s3;
				GblTbCmdPos = s2.Length + s1.Length + 1;
			}
			else
			{
				TbCmd.Text = s1;
				GblTbCmdPos = s1.Length;
			}

			GblDgvCmdOpen = true;
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

				case Keys.F2:
					BtnDgvCmd_Click(sender, e);
					BtnDgvMacro_Click(sender, e);
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

		private IEnumerable<string> IeFile;

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

		private void TbDgvCmdSearch_Enter(object sender, EventArgs e)
		{
			Lbl_F3.ForeColor = Color.Gold;
		}

		private void TbDgvCmdSearch_Leave(object sender, EventArgs e)
		{
			Lbl_F3.ForeColor = Color.Gray;
		}

		private void TbDgvCmdSearch_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.F3:
					BtnDgvCmd_Click(sender, e);
					break;

				case Keys.F2:
					BtnDgvCmd_Click(sender, e);
					BtnDgvMacro_Click(sender, e);
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

			Thread.Sleep(100);
		}

		//--------------------------------------------------------------------------------
		// CmsTbDgvCmdSearch
		//--------------------------------------------------------------------------------
		private void CmsTbDgvCmdSearch_全クリア_Click(object sender, EventArgs e)
		{
			TbDgvCmdSearch.Text = "";
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

		//--------------------------------------------------------------------------------
		// CbTextCode
		//--------------------------------------------------------------------------------
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

		private void CbTextCode_SelectedIndexChanged(object sender, EventArgs e)
		{
			SubCmdMemoAddText($"#code \"{CbTextCode.Text}\";");
		}

		private delegate void MyEventHandler(object sender, DataReceivedEventArgs e);
		private event MyEventHandler MyEvent = null;

		private void MyEventDataReceived(object sender, DataReceivedEventArgs e)
		{
			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, e.Data + NL);
		}

		private void ProcessDataReceived(object sender, DataReceivedEventArgs e)
		{
			_ = Invoke(MyEvent, new object[2] { sender, e });
		}

		//--------------------------------------------------------------------------------
		// CmsCbTextCode
		//--------------------------------------------------------------------------------
		private void CmsCbTextCode_全コピー_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(CbTextCode.Text);
		}

		//--------------------------------------------------------------------------------
		// 実行
		//--------------------------------------------------------------------------------
		private void BtnCmdExec_Click(object sender, EventArgs e)
		{
			// Trim() で置換すると GblTbCmdPos が変わるので不可
			if (TbCmd.Text.Trim().Length == 0)
			{
				return;
			}

			ExecStopOn = false;

			Cursor.Current = Cursors.WaitCursor;

			// 出力バッファを確実に反映させるため
			SubTbResultChange(-1, false);

			// 実行
			foreach (string _s1 in RtnCmdFormat(TbCmd.Text))
			{
				// メモに追加
				SubCmdMemoAddText(_s1);
				SubTbCmdExec(_s1);
			}

			// マクロ・コマンド履歴に追加
			ListCmdHistory.Add(TbCmd.Text.Trim());

			// 出力履歴に追加
			if (TbResult.Text.Length > 0)
			{
				TbResult_Leave(sender, e);
			}

			// タイトル表示を戻す
			Text = ProgramID;

			Cursor.Current = Cursors.Default;

			SubTbCmdFocus(GblTbCmdPos);
		}

		private void BtnCmdExecStream_Click(object sender, EventArgs e)
		{
			BtnCmdExecStream.Visible = false;
			ExecStopOn = true;
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
			// エラーが発生しているとき
			if (ExecStopOn)
			{
				return;
			}

			cmd = cmd.Trim();

			// コメント行
			if (Regex.IsMatch(cmd, @"^//"))
			{
				return;
			}

			// タイトルに表示
			Text = $"> {cmd}";

			// 変数
			Regex rgx;
			Match match;
			int i1 = 0, i2 = 0;
			string s1 = "";

			// 変換
			cmd = Regex.Replace(cmd, @"^(cd|chdir)", "#cd", RegexOptions.IgnoreCase);
			cmd = Regex.Replace(cmd, @"^clear", "#clear", RegexOptions.IgnoreCase);
			cmd = Regex.Replace(cmd, @"^cls", "#cls", RegexOptions.IgnoreCase);

			if (cmd.Length == 0)
			{
				return;
			}

			SubLblWaitOn(true);

			// マクロ実行
			if (cmd[0] == '#')
			{
				//【重要】検索用フラグ " " 付与
				cmd += " ";

				//【重要】"abc\"123\"def" のように、" に囲まれた引数を取得するため、一時的に \" を \a に変換しておく。
				// 後で \a を " に変換。
				//  (NG) \\\"
				//  (OK) \\\\\"
				cmd = Regex.Replace(cmd, "\\\\\"", "\a");

				const int aOpMax = 4;
				string[] aOp = Enumerable.Repeat("", aOpMax).ToArray();

				// マクロ取得
				rgx = new Regex("^(?<pattern>\\S+?)\\s", RegexOptions.None);
				match = rgx.Match(cmd);
				aOp[0] = match.Groups["pattern"].Value;

				// Option[n] 取得
				i1 = 0;
				rgx = new Regex("\"(?<pattern>.+?)\"\\s", RegexOptions.None);
				for (match = rgx.Match(cmd.Substring(aOp[0].Length)); match.Success; match = match.NextMatch())
				{
					++i1;
					if (i1 >= aOpMax)
					{
						break;
					}
					// \a を " に変換
					// 空白も有効／Trim()使用不可
					aOp[i1] = Regex.Replace(match.Groups["pattern"].Value, "\a", "\"");
				}

				// 大小区別しない
				switch (aOp[0].ToLower())
				{
					// 全クリア
					case "#clear":
						TbCmd.Text = "";
						RtbCmdMemo.Text = "";
						TbResult.Text = "";

						for (int _i1 = 0; _i1 < AryResultBuf.Length; _i1++)
						{
							AryResultBuf[_i1] = "";
						}
						SubTbResultChange(0, true);

						GC.Collect();
						break;

					// 出力クリア
					case "#cls":
						BtnClear_Click(null, null);
						break;

					// フォルダ変更
					case "#cd":
						if (aOp[1].Length == 0)
						{
							TbCurDir_Click(null, null);
							break;
						}

						// マクロ変数に変換
						aOp[1] = RtnCnvMacroVar(aOp[1]);
						string _sFullPath = Path.GetFullPath(aOp[1]);

						try
						{
							// フォルダが存在しないときは新規作成
							if (!Directory.Exists(_sFullPath))
							{
								_ = Directory.CreateDirectory(_sFullPath);

								int iBgn = RtbCmdMemo.TextLength;
								RtbCmdMemo.AppendText($"新規フォルダ '{_sFullPath}' を作成しました。{NL}");
								int iEnd = RtbCmdMemo.TextLength;

								RtbCmdMemo.SelectionStart = iBgn;
								RtbCmdMemo.SelectionLength = iEnd - iBgn;
								RtbCmdMemo.SelectionColor = Color.Magenta;
								RtbCmdMemo.SelectionStart = iEnd;
								RtbCmdMemo.ScrollToCaret();
							}
							Directory.SetCurrentDirectory(_sFullPath);
							TbCurDir.Text = _sFullPath;
						}
						catch
						{
							_ = MessageBox.Show(
								"[Err] アクセス権限のないフォルダです。" + NL + NL + "・" + _sFullPath + NL + NL + "プログラムの実行を停止します。",
								ProgramID
							);
							ExecStopOn = true;
						}
						break;

					// 文字コード
					case "#code":
						if (aOp[1].Length > 0)
						{
							CbTextCode.Text = aOp[1];
						}
						break;

					// 印字
					case "#print":
						// マクロ変数に変換
						TbResult.AppendText(RtnCnvMacroVar(aOp[1]));
						break;

					// 連番生成
					case "#serial":
						_ = int.TryParse(aOp[1], out i1);
						_ = int.TryParse(aOp[2], out i2);
						_ = int.TryParse(aOp[3], out int _frmt_zeros);
						_ = SB.Clear();
						for (; i1 <= i2; i1++)
						{
							_ = SB.Append(string.Format("{0:D" + _frmt_zeros + "}", i1));
							_ = SB.Append(NL);
						}
						TbResult.AppendText(SB.ToString());
						TbResult.SelectionStart = TbResult.TextLength;
						TbResult.ScrollToCaret();
						break;

					// 出力変更
					case "#result":
						_ = int.TryParse(aOp[1], out i1);
						--i1;
						if (RtnAryResultBtnRangeChk(i1))
						{
							SubTbResultChange(i1, true);
						}
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
						// マクロ変数に変換
						aOp[1] = RtnCnvMacroVar(aOp[1]);
						aOp[2] = RtnCnvMacroVar(aOp[2]);
						TbResult.Text = RtnTextReplace(TbResult.Text, aOp[1], aOp[2], true);
						break;

					// 分割変換（like AWK）
					case "#split":
						TbResult.Text = RtnTextSplitCnv(TbResult.Text, aOp[1], aOp[2]);
						break;

					// 行前後の空白クリア
					case "#trim":
						TbResult.Text = RtnTextTrim(TbResult.Text);
						break;

					// 全角変換
					case "#towide":
						TbResult.Text = Strings.StrConv(TbResult.Text.TrimEnd(), VbStrConv.Wide, 0x411);
						break;

					// 全角変換(数字のみ)
					case "#tozennum":
						TbResult.Text = RtnZenNum(TbResult.Text.TrimEnd());
						break;

					// 全角変換(カナのみ)
					case "#tozenkana":
						TbResult.Text = RtnZenKana(TbResult.Text.TrimEnd());
						break;

					// 半角変換
					case "#tonarrow":
						TbResult.Text = Strings.StrConv(TbResult.Text.TrimEnd(), VbStrConv.Narrow, 0x411);
						break;

					// 半角変換(数字のみ)
					case "#tohannum":
						TbResult.Text = RtnHanNum(TbResult.Text.TrimEnd());
						break;

					// 半角変換(カナのみ)
					case "#tohankana":
						TbResult.Text = RtnHanKana(TbResult.Text.TrimEnd());
						break;

					// 大文字変換
					case "#toupper":
						TbResult.Text = TbResult.Text.TrimEnd().ToUpper();
						break;

					// 小文字変換
					case "#tolower":
						TbResult.Text = TbResult.Text.TrimEnd().ToLower();
						break;

					// クリア
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

					// 重複行をクリア
					case "#uniq":
						TbResult.Text = RtnTextUniq(TbResult.Text);
						break;

					// 空白行削除
					case "#rmblankln":
						_ = SB.Clear();
						foreach (string _s1 in Regex.Split(TbResult.Text, NL))
						{
							if (_s1.TrimEnd().Length > 0)
							{
								_ = SB.Append(_s1);
								_ = SB.Append(NL);
							}
						}
						TbResult.Text = SB.ToString();
						break;

					// 改行をクリア
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

						// マクロ変数に変換
						aOp[1] = RtnCnvMacroVar(aOp[1]);

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

						// マクロ変数に変換
						aOp[1] = RtnCnvMacroVar(aOp[1]);

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

					// フォルダ・ファイル一覧
					case "#dflist":
					case "#dlist":
					case "#flist":
						if (aOp[1].Length == 0)
						{
							aOp[1] = ".";
						}

						// マクロ変数に変換
						aOp[1] = RtnCnvMacroVar(aOp[1]);

						MyEvent = new MyEventHandler(MyEventDataReceived);

						PS = new Process();
						PS.StartInfo.UseShellExecute = false;
						PS.StartInfo.RedirectStandardInput = true;
						PS.StartInfo.RedirectStandardOutput = true;
						PS.StartInfo.RedirectStandardError = true;
						PS.StartInfo.CreateNoWindow = true;
						PS.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CbTextCode.Text);
						PS.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);

						PS.StartInfo.FileName = "cmd.exe";
						PS.StartInfo.Arguments = $"/c dir /s /b {aOp[1]}\\";

						_ = PS.Start();
						s1 = PS.StandardOutput.ReadToEnd().Trim();
						PS.Close();

						List<string> _l1 = new List<string>();
						_l1 = Regex.Split(s1, RgxNL).ToList();
						_l1.Sort();

						_ = SB.Clear();

						switch (aOp[0].ToLower())
						{
							case "#dflist":
								foreach (string _s1 in _l1)
								{
									_ = SB.Append(_s1);
									_ = SB.Append(Directory.Exists(_s1) ? @"\" : "");
									_ = SB.Append(NL);
								}
								break;

							case "#dlist":
								foreach (string _s1 in _l1)
								{
									if (Directory.Exists(_s1))
									{
										_ = SB.Append(_s1);
										_ = SB.Append(@"\");
										_ = SB.Append(NL);
									}
								}
								break;

							case "#flist":
								foreach (string _s1 in _l1)
								{
									if (!Directory.Exists(_s1))
									{
										_ = SB.Append(_s1);
										_ = SB.Append(NL);
									}
								}
								break;
						}

						TbResult.AppendText(SB.ToString());
						TbResult.SelectionStart = TbResult.TextLength;
						TbResult.ScrollToCaret();

						break;

					// ファイル名置換
					case "#rename":
						// マクロ変数に変換
						aOp[1] = RtnCnvMacroVar(aOp[1]);
						aOp[2] = RtnCnvMacroVar(aOp[2]);

						TbResult.Text = RtnFnRename(TbResult.Text, aOp[1], aOp[2]);
						break;

					// 行毎に処理
					case "#stream":
						if (aOp[1].Length == 0)
						{
							break;
						}

						BtnCmdExecStream.Visible = true;

						int iStreamBgn = RtbCmdMemo.TextLength;
						int iRead = 0;
						int iNL = NL.Length;
						int iLine = 0;

						foreach (string _s1 in Regex.Split(TbResult.Text, NL))
						{
							++iLine;

							string _s2 = _s1.Trim();

							if (_s2.Length > 0)
							{
								// 【重要】aOp[1] 本体は変更しない
								s1 = aOp[1];

								// マクロ変数に変換
								s1 = Regex.Replace(RtnCnvMacroVar(aOp[1], iLine), @"#\{\}", _s2);

								MyEvent = new MyEventHandler(MyEventDataReceived);

								PS = new Process();
								PS.StartInfo.UseShellExecute = false;
								PS.StartInfo.RedirectStandardInput = true;
								PS.StartInfo.RedirectStandardOutput = true;
								PS.StartInfo.RedirectStandardError = true;
								PS.StartInfo.CreateNoWindow = true;
								PS.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CbTextCode.Text);
								PS.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);

								PS.StartInfo.FileName = "cmd.exe";
								PS.StartInfo.Arguments = $"/c {s1}";

								try
								{
									_ = PS.Start();
									// Stdout
									RtbCmdMemo.AppendText(Regex.Replace(PS.StandardOutput.ReadToEnd(), RgxNL, NL));
									// Stderr
									RtbCmdMemo.AppendText(PS.StandardError.ReadToEnd());
									PS.Close();

									// RtbCmdMemo の着色・スクロール
									int iStreamEnd = RtbCmdMemo.TextLength;
									RtbCmdMemo.SelectionStart = iStreamBgn;
									RtbCmdMemo.SelectionLength = iStreamEnd - iStreamBgn;
									RtbCmdMemo.SelectionColor = Color.Red;
									RtbCmdMemo.SelectionStart = iStreamEnd;
									RtbCmdMemo.ScrollToCaret();

									// TbResult の進捗状況
									int _i1 = _s1.Length;
									TbResult.Select(iRead, _i1);
									_ = TbResult.Focus();
									iRead += _i1;
								}
								catch
								{
								}

								// 処理中断
								Thread.Sleep(100);
								Application.DoEvents();
								if (ExecStopOn)
								{
									break;
								}
							}
							iRead += iNL;
						}
						BtnCmdExecStream.Visible = false;
						break;

					// 連想配列
					case "#set":
						// 【リスト】#set
						// 【削除】  #set "Key" "" | #set "Key"
						// 【登録・変更】#set "Key" "Data" => #{%key} = "Data"
						if (aOp[1].Length == 0 && aOp[2].Length == 0)
						{
							// リスト
							foreach (KeyValuePair<string, string> _kv1 in DictHash)
							{
								TbResult.AppendText("#{%" + _kv1.Key + "}\t" + _kv1.Value + NL);
							}
						}
						else
						{
							if (aOp[2].Length == 0)
							{
								// 削除
								_ = DictHash.Remove(aOp[1]);
							}
							else
							{
								// 登録・変更
								DictHash[aOp[1]] = aOp[2];
							}
						}
						break;

					// 計算機
					case "#calc":
						// マクロ変数に変換
						aOp[1] = RtnCnvMacroVar(aOp[1]);
						TbResult.AppendText(NL + RtnEvalCalc(aOp[1]) + NL);
						break;

					// フォーム位置
					case "#pos":
						// マクロ変数に変換
						s1 = RtnCnvMacroVar(aOp[1]);
						if (Regex.IsMatch(s1, @"\d+%"))
						{
							_ = int.TryParse(s1.Replace("%", ""), out i1);
							i1 = (int)(Screen.GetWorkingArea(this).Width / 100.0 * i1);
						}
						else if (Regex.IsMatch(s1, @"[-\+]\d+"))
						{
							_ = int.TryParse(s1, out i1);
							i1 += Bounds.X;
						}
						else
						{
							_ = int.TryParse(s1, out i1);
						}
						// Y
						// マクロ変数に変換
						s1 = RtnCnvMacroVar(aOp[2]);
						if (Regex.IsMatch(s1, @"\d+%"))
						{
							_ = int.TryParse(s1.Replace("%", ""), out i2);
							i2 = (int)(Screen.GetWorkingArea(this).Height / 100.0 * i2);
						}
						else if (Regex.IsMatch(s1, @"[-\+]\d+"))
						{
							_ = int.TryParse(s1, out i2);
							i2 += Bounds.Y;
						}
						else
						{
							_ = int.TryParse(s1, out i2);
						}
						SetDesktopLocation(i1, i2);
						break;

					// フォームサイズ
					case "#size":
						// Width
						// マクロ変数に変換
						s1 = RtnCnvMacroVar(aOp[1]);
						if (Regex.IsMatch(s1, @"\d+%"))
						{
							_ = int.TryParse(s1.Replace("%", ""), out i1);
							i1 = (int)(Screen.GetWorkingArea(this).Width / 100.0 * Math.Abs(i1));
						}
						else if (Regex.IsMatch(s1, @"[-\+]\d+"))
						{
							_ = int.TryParse(s1, out i1);
							i1 += Bounds.Width;
						}
						else
						{
							_ = int.TryParse(s1, out i1);
						}
						// Height
						// マクロ変数に変換
						s1 = RtnCnvMacroVar(aOp[2]);
						if (Regex.IsMatch(s1, @"\d+%"))
						{
							_ = int.TryParse(s1.Replace("%", ""), out i2);
							i2 = (int)(Screen.GetWorkingArea(this).Height / 100.0 * Math.Abs(i2));
						}
						else if (Regex.IsMatch(s1, @"[-\+]\d+"))
						{
							_ = int.TryParse(s1, out i2);
							i2 += Bounds.Height;
						}
						else
						{
							_ = int.TryParse(s1, out i2);
						}
						Width = i1;
						Height = i2;
						break;

					// フォームサイズ（最大化）
					case "#sizemax":
						WindowState = FormWindowState.Maximized;
						break;

					// フォームサイズ（最小化）
					case "#sizemin":
						WindowState = FormWindowState.Minimized;
						break;

					// フォームサイズ（普通）
					case "#sizenormal":
						WindowState = FormWindowState.Normal;
						break;

					// フォーカス位置を先頭にする
					case "#focus0":
						TbResult.SelectionStart = 0;
						TbResult.ScrollToCaret();
						break;

					// フォーカス位置を末尾にする
					case "#focus9":
						TbResult.SelectionStart = TbResult.TextLength;
						TbResult.ScrollToCaret();
						break;

					// マクロ一覧
					case "#macrolist":
						s1 =
							"--------------" + NL +
							"> マクロ一覧 <" + NL +
							"--------------" + NL +
							"※大文字・小文字を区別しない。(例) #clear と #CLEAR は同じ。" + NL +
							NL +
							"[マクロ]     [説明]" + NL
						;

						for (int _i1 = 0; _i1 < MACRO.Length / 3; _i1++)
						{
							s1 += string.Format("{0,-13}{1}", MACRO[_i1, 0], MACRO[_i1, 1]) + NL;
						}

						_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, NL + s1 + NL);
						break;

					// 操作説明
					case "#help":
						_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, NL + HELP_TbCmd + NL);
						break;

					// バージョン
					case "#version":
						TbResult.AppendText(ProgramID + NL + VERSION + NL);
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
				MyEvent = new MyEventHandler(MyEventDataReceived);

				PS = new Process();
				PS.StartInfo.UseShellExecute = false;
				PS.StartInfo.RedirectStandardInput = true;
				PS.StartInfo.RedirectStandardOutput = true;
				PS.StartInfo.RedirectStandardError = true;
				PS.StartInfo.CreateNoWindow = true;
				PS.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CbTextCode.Text);
				PS.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);

				PS.StartInfo.FileName = "cmd.exe";
				// マクロ変数に変換
				PS.StartInfo.Arguments = "/c " + RtnCnvMacroVar(cmd);

				_ = PS.Start();
				s1 = PS.StandardOutput.ReadToEnd();
				PS.Close();

				// 改行を \r\n に変換／該当するのはUNIX系移植コマンドのみ
				if (s1.IndexOf("\r") == -1)
				{
					s1 = Regex.Replace(s1, RgxNL, NL);
				}

				// ESCをスルー／事前に string.IndexOf するより、直接 Regex した方が速い。
				s1 = Regex.Replace(s1, @"\033\[(\S+?)m", "", RegexOptions.IgnoreCase);

				TbResult.AppendText(s1);
			}

			// TbResult.SelectionStart = NUM 不可
			TbResult.ScrollToCaret();

			SubLblWaitOn(false);
		}

		private string RtnCnvMacroVar(string cmd, int iLine = 0)
		{
			if (Regex.IsMatch(cmd, @"#\{\S+\}"))
			{
				// 拡張表記
				cmd = Regex.Replace(cmd, "#{\\\\t}", "\t", RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{\\\\n}", NL, RegexOptions.IgnoreCase);

				// 日時変数
				DateTime dt = DateTime.Now;
				cmd = Regex.Replace(cmd, "#{ymd}", dt.ToString("yyyyMMdd"), RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{hns}", dt.ToString("HHmmss"), RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{msec}", dt.ToString("fff"), RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{y}", dt.ToString("yyyy"), RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{m}", dt.ToString("MM"), RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{d}", dt.ToString("dd"), RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{h}", dt.ToString("HH"), RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{n}", dt.ToString("mm"), RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{s}", dt.ToString("ss"), RegexOptions.IgnoreCase);

				Regex rgx;
				Match match;

				// 環境変数
				rgx = new Regex("#{(?<pattern>\\S+?)}", RegexOptions.None);
				match = rgx.Match(cmd);
				string s1 = match.Groups["pattern"].Value;
				string s2 = Environment.GetEnvironmentVariable(s1);
				if (s2 != null)
				{
					cmd = cmd.Replace("#{" + s1 + "}", s2);
				}

				// 出力データに変換
				AryResultBuf[AryResultIndex] = TbResult.Text;
				rgx = new Regex("#{(?<pattern>result\\S*?)}", RegexOptions.IgnoreCase);
				foreach (Match _m1 in rgx.Matches(cmd))
				{
					string _s1 = _m1.Groups["pattern"].ToString();
					string[] _a1 = _s1.Split(',');
					int _i1 = 0;
					if (_a1.Length >= 2 && Regex.IsMatch(_a1[1], @"\d+"))
					{
						_ = int.TryParse(_a1[1], out _i1);
						--_i1;
						if (_i1 >= 0 && _i1 <= 4)
						{
							cmd = Regex.Replace(cmd, "#{" + _s1 + "}", AryResultBuf[_i1], RegexOptions.IgnoreCase);
						}
					}
				}

				// 連想配列
				rgx = new Regex("#{%(?<pattern>.+?)}");
				foreach (Match _m1 in rgx.Matches(cmd))
				{
					string _s1 = _m1.Groups["pattern"].Value.Trim();
					if (DictHash.ContainsKey(_s1))
					{
						cmd = Regex.Replace(cmd, "#{%" + _s1 + "}", DictHash[_s1]);
					}
				}

				// 行番号に変換
				rgx = new Regex("#{(?<pattern>line\\S*?)}", RegexOptions.IgnoreCase);
				foreach (Match _m1 in rgx.Matches(cmd))
				{
					string _s1 = _m1.Groups["pattern"].ToString();
					string[] _a1 = _s1.Split(',');
					int _frmt_zeros = 0;
					if (_a1.Length >= 2 && Regex.IsMatch(_a1[1], @"\d+"))
					{
						_ = int.TryParse(_a1[1], out _frmt_zeros);
					}
					cmd = Regex.Replace(cmd, "#{" + _s1 + "}", string.Format("{0:D" + _frmt_zeros + "}", iLine), RegexOptions.IgnoreCase);
				}
			}

			return cmd;
		}

		private void SubCmdMemoAddText(string str)
		{
			RichTextBox To = RtbCmdMemo;
			To.SelectionStart = To.TextLength;
			_ = NativeMethods.SendMessage(To.Handle, EM_REPLACESEL, 1, str + NL);
			To.SelectionStart = To.TextLength;
			To.ScrollToCaret();
		}

		//--------------------------------------------------------------------------------
		// BtnMemoCopy
		//--------------------------------------------------------------------------------
		private void BtnMemoCopy_Click(object sender, EventArgs e)
		{
			RichTextBox From = RtbCmdMemo;
			TextBox To = TbResult;

			if (From.Text.Trim().Length > 0)
			{
				To.SelectionStart = To.TextLength;
				_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, Regex.Replace(From.Text, RgxNL, NL) + NL);
			}

			To.Select(To.TextLength, 0);
			_ = To.Focus();
			TbResult.ScrollToCaret();
		}

		//--------------------------------------------------------------------------------
		// 入出力クリア
		//--------------------------------------------------------------------------------
		private void BtnClear_Click(object sender, EventArgs e)
		{
			TbResult.Text = "";
			GC.Collect();
			SubTbResultCnt();
			SubTbCmdFocus(GblTbCmdPos);
		}

		//--------------------------------------------------------------------------------
		// TbResult
		//--------------------------------------------------------------------------------
		private string TbResultUndo = "";
		private string TbResultRedo = "";

		private void TbResult_Enter(object sender, EventArgs e)
		{
			LblResult.Visible = true;
		}

		private void TbResult_Leave(object sender, EventArgs e)
		{
			LblResult.Visible = false;

			if (TbResult.TextLength == 0)
			{
				return;
			}

			// 異なるデータのみ追加
			bool bExist = false;

			foreach (KeyValuePair<string, string> _dict in DictResultHistory)
			{
				if (_dict.Value == TbResult.Text)
				{
					bExist = true;
					break;
				}
			}

			if (!bExist)
			{
				// Key重複回避のため一応遅延
				Thread.Sleep(10);
				DictResultHistory.Add(DateTime.Now.ToString("HH:mm:ss_fff"), TbResult.Text);
			}
		}

		private void TbResult_KeyDown(object sender, KeyEventArgs e)
		{
			// [Ctrl]+[C]
			if (e.KeyData == (Keys.Control | Keys.C))
			{
				BtnCmdExecStream_Click(sender, e);
				return;
			}

			// [Ctrl]+[V]
			if (e.KeyData == (Keys.Control | Keys.V))
			{
				Clipboard.SetText(Regex.Replace(Clipboard.GetText(), RgxNL, NL));
				return;
			}
		}

		private void TbResult_KeyUp(object sender, KeyEventArgs e)
		{
			// [Ctrl]+[Y]
			if (e.KeyData == (Keys.Control | Keys.Y))
			{
				TbResult.Text = TbResultRedo;
				return;
			}

			// [Ctrl]+[Z]
			if (e.KeyData == (Keys.Control | Keys.Z) && TbResultUndo.Length > 0)
			{
				if (TbResultUndo != TbResult.Text)
				{
					TbResultRedo = TbResult.Text;
				}
				TbResult.Text = TbResultUndo;
				return;
			}

			switch (e.KeyCode)
			{
				case Keys.Escape:
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.F2:
					BtnDgvMacro_Click(sender, e);
					break;

				case Keys.F3:
					BtnDgvMacro_Click(sender, e);
					BtnDgvCmd_Click(sender, e);
					break;

				case Keys.F11:
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.F12:
					SubTbResultNext();
					break;

				default:
					SubTbResultCnt();
					break;
			}
		}

		private void TbResult_MouseUp(object sender, MouseEventArgs e)
		{
			SubTbResultCnt();
			CmsTextSelect_Open(e, TbResult);
		}

		private void TbResult_TextChanged(object sender, EventArgs e)
		{
			SubTbResultCnt();
		}

		private void TbResult_DragEnter(object sender, DragEventArgs e)
		{
			PanelResult.Visible = true;
		}

		//--------------------------------------------------------------------------------
		// CmsResult
		//--------------------------------------------------------------------------------
		private void CmsResult_全選択_Click(object sender, EventArgs e)
		{
			TbResult.SelectAll();
			_ = TbResult.Focus();
			SubTbResultCnt();
		}

		private void CmsResult_全クリア_Click(object sender, EventArgs e)
		{
			// [Ctrl]+[Z] 有効化
			TbResultUndo = TbResult.Text;
			TbResult.Text = "";
		}

		private void CmsResult_全コピー_Click(object sender, EventArgs e)
		{
			TbResult.SelectAll();
			TbResult.Copy();
		}

		private void CmsResult_上書き_Click(object sender, EventArgs e)
		{
			// [Ctrl]+[Z] 有効化
			TbResultUndo = TbResult.Text;
			TbResult.Text = "";
			CmsResult_貼り付け_Click(sender, e);
		}

		private void CmsResult_貼り付け_Click(object sender, EventArgs e)
		{
			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, Regex.Replace(Clipboard.GetText(), RgxNL, NL));
		}

		private void CmsResult_ファイル名を貼り付け_Click(object sender, EventArgs e)
		{
			_ = SB.Clear();

			foreach (string _s1 in Clipboard.GetFileDropList())
			{
				_ = SB.Append(_s1);
				_ = SB.Append((Directory.Exists(_s1) ? @"\" : ""));
				_ = SB.Append(NL);
			}

			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, SB.ToString());
		}

		private void CmsResult_名前を付けて保存_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog
			{
				FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt",
				Filter = "Text (*.txt)|*.txt|TSV (*.tsv)|*.tsv|CSV (*.csv)|*.csv|HTML (*.html,*.htm)|*.html,*.htm|All files (*.*)|*.*",
				FilterIndex = 1,
				InitialDirectory = Environment.CurrentDirectory
			};

			if (sfd.ShowDialog() == DialogResult.OK)
			{
				switch (CbTextCode.Text.ToUpper())
				{
					case "UTF-8":
						File.WriteAllText(sfd.FileName, TbResult.Text, new UTF8Encoding(false));
						break;

					default:
						File.WriteAllText(sfd.FileName, TbResult.Text, Encoding.GetEncoding(TEXT_CODE[0]));
						break;
				}
			}
		}

		private void SubTbResultCnt()
		{
			int iWord = 0;
			int iLine = 1;

			int i1 = TbResult.SelectionStart;
			char[] ca1 = TbResult.Text.Substring(0, i1).ToCharArray();

			for (int _i1 = 0; _i1 < i1; _i1++)
			{
				++iWord;
				if (ca1[_i1] == '\n')
				{
					++iLine;
					iWord = 0;
				}
			}

			int iNL = 0;
			int iRow = 0;
			int iCnt = TbResult.SelectedText.Length;

			if (iCnt > 0)
			{
				foreach (string _s1 in Regex.Split(TbResult.SelectedText, NL))
				{
					++iNL;
					if (_s1.Length > 0)
					{
						++iRow;
					}
				}
				iCnt -= (iNL - 1) * NL.Length;
			}

			TbInfo.Text = string.Format("({0}行){1}字  (有効{2}行／全{3}行){4}字 選択", iLine, iWord, iRow, iNL, iCnt);
		}

		private void BtnResult1_Click(object sender, EventArgs e)
		{
			SubTbResultChange(0, true);
		}

		private void BtnResult2_Click(object sender, EventArgs e)
		{
			SubTbResultChange(1, true);
		}

		private void BtnResult3_Click(object sender, EventArgs e)
		{
			SubTbResultChange(2, true);
		}

		private void BtnResult4_Click(object sender, EventArgs e)
		{
			SubTbResultChange(3, true);
		}

		private void BtnResult5_Click(object sender, EventArgs e)
		{
			SubTbResultChange(4, true);
		}

		private int AryResultIndex = 0;
		private readonly string[] AryResultBtn = { "BtnResult1", "BtnResult2", "BtnResult3", "BtnResult4", "BtnResult5" };
		private readonly string[] AryResultBuf = { "", "", "", "", "", "" };
		private readonly int[] AryResultStartIndex = { 0, 0, 0, 0, 0 };

		private bool RtnAryResultBtnRangeChk(int index)
		{
			return index >= 0 && index < AryResultBtn.Length;
		}

		private void SubTbResultChange(int index, bool bMoveOn = true)
		{
			if (index == -1)
			{
				index = AryResultIndex;
			}

			if (!RtnAryResultBtnRangeChk(index))
			{
				return;
			}

			// 選択されたタブへ移動
			foreach (string _s1 in AryResultBtn)
			{
				if (_s1 == AryResultBtn[index])
				{
					Controls[_s1].BackColor = Color.Crimson;

					// 旧タブのデータ保存
					AryResultBuf[AryResultIndex] = TbResult.Text;
					AryResultStartIndex[AryResultIndex] = TbResult.SelectionStart;
					TbResult_Leave(null, null);

					// 新タブのデータ読込
					TbResult.Text = AryResultBuf[index];
					TbResult.Select(AryResultStartIndex[index], 0);
					if (bMoveOn)
					{
						_ = TbResult.Focus();
						TbResult.ScrollToCaret();
					}
					// 新タブ位置を記憶
					AryResultIndex = index;
				}
			}

			// 非選択タブのうちデータのあるタブは色を変える
			for (int _i1 = 0; _i1 < AryResultBtn.Length; _i1++)
			{
				if (_i1 != AryResultIndex)
				{
					Controls[AryResultBtn[_i1]].BackColor = AryResultBuf[_i1].Length > 0 ? Color.Maroon : Color.DimGray;
				}
			}
		}

		private void SubTbResultNext()
		{
			int i1 = AryResultIndex + 1;
			SubTbResultChange(RtnAryResultBtnRangeChk(i1) ? i1 : 0, false);
		}

		private void PanelResult_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Copy;
		}

		private void PanelResult_DragDrop(object sender, DragEventArgs e)
		{
			PanelResult.Visible = false;
		}

		private void PanelResult_DragLeave(object sender, EventArgs e)
		{
			PanelResult.Visible = false;
		}

		private void BtnPasteTextfile_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		private void BtnPasteTextfile_DragDrop(object sender, DragEventArgs e)
		{
			SubLblWaitOn(true);

			_ = SB.Clear();
			string s1 = "";

			foreach (string _s1 in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				if (RtnIsBinaryFile(_s1))
				{
					s1 += "・" + Path.GetFileName(_s1) + NL;
				}
				else
				{
					foreach (string _s2 in File.ReadLines(_s1, Encoding.GetEncoding(CbTextCode.Text)))
					{
						_ = SB.Append(_s2.TrimEnd());
						_ = SB.Append(NL);
					}
				}
			}

			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, SB.ToString());

			if (SB.Length > 0)
			{
				TbResult_Leave(sender, e);
			}

			if (s1.Length > 0)
			{
				_ = MessageBox.Show(
					"[Err] テキストファイルではありません。" + NL + NL + s1,
					ProgramID
				);
			}

			SubLblWaitOn(false);

			PanelResult.Visible = false;
		}

		private void BtnPasteTextfile_Click(object sender, EventArgs e)
		{
			// 誤操作で表示されたままになったとき使用
			PanelResult.Visible = false;
		}

		private void BtnPasteFilename_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		private void BtnPasteFilename_DragDrop(object sender, DragEventArgs e)
		{
			_ = SB.Clear();
			foreach (string _s1 in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				_ = SB.Append(_s1);
				_ = SB.Append(Directory.Exists(_s1) ? @"\" : "");
				_ = SB.Append(NL);
			}
			_ = NativeMethods.SendMessage(TbResult.Handle, EM_REPLACESEL, 1, SB.ToString());

			if (SB.Length > 0)
			{
				TbResult_Leave(sender, e);
			}

			PanelResult.Visible = false;
		}

		private void BtnPasteFilename_Click(object sender, EventArgs e)
		{
			// 誤操作で表示されたままになったとき使用
			PanelResult.Visible = false;
		}

		private void BtnPasteCancel_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Copy;
		}

		private void BtnPasteCancel_DragDrop(object sender, DragEventArgs e)
		{
			PanelResult.Visible = false;
		}

		private void BtnPasteCancel_Click(object sender, EventArgs e)
		{
			// 誤操作で表示されたままになったとき使用
			PanelResult.Visible = false;
		}

		//--------------------------------------------------------------------------------
		// 履歴
		//--------------------------------------------------------------------------------
		private int GblCbCmdHistoryRow = 0;
		private int GblCbResultHistoryRow = 0;

		private void CbCmdHistory_Enter(object sender, EventArgs e)
		{
			Lbl_F1.ForeColor = Color.Gold;

			// ListCmdHistory から重複排除
			ListCmdHistory.Sort();
			ListCmdHistory = ListCmdHistory.Distinct().ToList();

			// CbCmdHistory を再編成
			CbCmdHistory.Items.Clear();
			_ = CbCmdHistory.Items.Add("");

			foreach (string _s1 in ListCmdHistory)
			{
				_ = CbCmdHistory.Items.Add(_s1);
			}

			int i1 = 1;

			foreach (string s1 in ListCmdHistory)
			{
				if (s1 == TbCmd.Text)
				{
					break;
				}
				++i1;
			}

			CbCmdHistory.SelectedIndex = ListCmdHistory.Count >= i1 ? i1 : 0;
		}

		private void CbCmdHistory_Leave(object sender, EventArgs e)
		{
			GC.Collect();
			Lbl_F1.ForeColor = Color.Gray;
		}

		private void CbCmdHistory_DropDownClosed(object sender, EventArgs e)
		{
			if (CbCmdHistory.Text.Length > 0)
			{
				string s1 = CbCmdHistory.Text;

				// [Ctrl]+ のときは挿入モード／それ以外は上書き
				if ((ModifierKeys & Keys.Control) == Keys.Control)
				{
					int iPos = TbCmd.SelectionStart;
					string s2 = TbCmd.Text.Substring(0, iPos);
					string s3 = TbCmd.Text.Substring(iPos);

					TbCmd.Text = s2 + s1 + ";" + s3;
					GblTbCmdPos = s2.Length + s1.Length + 1;
				}
				else
				{
					TbCmd.Text = s1;
					GblTbCmdPos = s1.Length;
				}
			}

			CbCmdHistory.Text = "";
			SubTbCmdFocus(GblTbCmdPos);
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
					if (CbCmdHistory.Items.Count > 0 && GblCbCmdHistoryRow == CbCmdHistory.Items.Count - 1)
					{
						CbCmdHistory.SelectedIndex = 0;
					}
					break;

				case Keys.Left:
					if (CbCmdHistory.Items.Count > 0)
					{
						CbCmdHistory.SelectedIndex = CbCmdHistory.SelectedIndex == 0 ? CbCmdHistory.Items.Count - 1 : 0;
					}
					break;

				case Keys.Right:
					if (CbCmdHistory.Items.Count > 0)
					{
						CbCmdHistory.SelectedIndex = CbCmdHistory.SelectedIndex == CbCmdHistory.Items.Count - 1 ? 0 : CbCmdHistory.Items.Count - 1;
					}
					break;
			}
		}

		private void CbResultHistory_Enter(object sender, EventArgs e)
		{
			Lbl_F8.ForeColor = Color.Gold;

			// 最近の10件のみ表示
			int cntTail = DictResultHistory.Count() - 10;
			int cnt = 0;
			List<string> lRmKey = new List<string>();

			foreach (KeyValuePair<string, string> _dict in DictResultHistory)
			{
				if (cnt < cntTail)
				{
					lRmKey.Add(_dict.Key);
				}
				++cnt;
			}

			// DictResultHistory から11件目以降のデータを削除
			foreach (string _s1 in lRmKey)
			{
				_ = DictResultHistory.Remove(_s1);
			}

			// CbResultHistory を再編成
			CbResultHistory.Items.Clear();
			_ = CbResultHistory.Items.Add("");

			foreach (KeyValuePair<string, string> _dict in DictResultHistory)
			{
				string _s1 = _dict.Value.Substring(0, _dict.Value.Length < 80 ? _dict.Value.Length : 70).TrimStart();
				_ = CbResultHistory.Items.Add(string.Format("{0}  {1}", _dict.Key, _s1.Replace(NL, @"\n")));
			}

			int i1 = 0;

			foreach (KeyValuePair<string, string> _dict in DictResultHistory)
			{
				if (_dict.Value == TbResult.Text)
				{
					break;
				}
				++i1;
			}

			CbResultHistory.SelectedIndex = i1 < DictResultHistory.Count ? i1 + 1 : 0;
		}

		private void CbResultHistory_Leave(object sender, EventArgs e)
		{
			GC.Collect();
			Lbl_F8.ForeColor = Color.Gray;
		}

		private void CbResultHistory_DropDownClosed(object sender, EventArgs e)
		{
			if (CbResultHistory.Text.Length > 0)
			{
				TbResult.Text = DictResultHistory[CbResultHistory.Text.Substring(0, 12)];
				// TbResult.SelectionStart = NUM 不可
				TbResult.ScrollToCaret();
			}

			CbResultHistory.Text = "";
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

		private void BtnResultMem_Click(object sender, EventArgs e)
		{
			int i1 = CbResultHistory.Items.Count - 2;

			if (i1 >= 0)
			{
				CbResultHistory.SelectedIndex = i1;
				CbResultHistory_DropDownClosed(sender, e);
			}

			SubTbCmdFocus(GblTbCmdPos);
		}

		//--------------------------------------------------------------------------------
		// フォントサイズ
		//--------------------------------------------------------------------------------
		private const decimal NudTbResult_BaseSize = 10;
		private decimal NudTbResult_CurSize = 0;

		private void NudTbResult_DoubleClick(object sender, EventArgs e)
		{
			if (NudTbResult.Value == NudTbResult_BaseSize)
			{
				NudTbResult.Value = NudTbResult_CurSize;
			}
			else
			{
				NudTbResult_CurSize = NudTbResult.Value;
				NudTbResult.Value = NudTbResult_BaseSize;
			}
		}

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
			TbCmd.Font = new Font(TbCmd.Font.Name.ToString(), (float)NudTbResult.Value);
			RtbCmdMemo.Font = new Font(RtbCmdMemo.Font.Name.ToString(), (float)NudTbResult.Value);
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

		//--------------------------------------------------------------------------------
		// CmsTextSelect
		//--------------------------------------------------------------------------------
		private void CmsTextSelect_Open(MouseEventArgs e, object Obj)
		{
			if (e.Button == MouseButtons.Left)
			{
				switch (Obj)
				{
					case TextBox tb when tb.SelectionLength > 0:
					case RichTextBox rtb when rtb.SelectionLength > 0:
						OBJ = Obj;
						CmsTextSelect.Show(Cursor.Position);
						break;

					default:
						OBJ = null;
						break;
				}
			}
		}

		private void CmsTextSelect_Cancel_Click(object sender, EventArgs e)
		{
			CmsTextSelect.Close();
		}

		private void CmsTextSelect_コピー_Click(object sender, EventArgs e)
		{
			switch (OBJ)
			{
				case TextBox tb:
					tb.Copy();
					break;

				case RichTextBox rtb:
					rtb.Copy();
					break;
			}
		}

		private void CmsTextSelect_切り取り_Click(object sender, EventArgs e)
		{
			switch (OBJ)
			{
				case TextBox tb:
					tb.Cut();
					break;

				case RichTextBox rtb:
					rtb.Cut();
					break;
			}
		}

		private void CmsTextSelect_貼り付け_Click(object sender, EventArgs e)
		{
			switch (OBJ)
			{
				case TextBox tb:
					tb.Paste();
					break;

				case RichTextBox rtb:
					rtb.Paste();
					break;
			}
		}

		private void CmsTextSelect_DQで囲む_Click(object sender, EventArgs e)
		{
			switch (OBJ)
			{
				case TextBox tb:
					tb.SelectedText = $"\"{tb.SelectedText.Trim('\"')}\"";
					break;

				case RichTextBox rtb:
					rtb.SelectedText = $"\"{rtb.SelectedText.Trim('\"')}\"";
					break;
			}
		}

		private void CmsTextSelect_DQを消去_Click(object sender, EventArgs e)
		{
			int iPos1, iPos2;
			string str;

			switch (OBJ)
			{
				case TextBox tb:
					iPos1 = tb.SelectionStart;
					iPos2 = tb.SelectionStart + tb.SelectionLength;
					str = tb.SelectedText;
					tb.Text = tb.Text.Substring(0, iPos1) + str.Replace("\"", "") + tb.Text.Substring(iPos2);
					tb.SelectionStart = iPos1;
					break;

				case RichTextBox rtb:
					iPos1 = rtb.SelectionStart;
					iPos2 = rtb.SelectionStart + rtb.SelectionLength;
					str = rtb.SelectedText;
					rtb.Text = rtb.Text.Substring(0, iPos1) + str.Replace("\"", "") + rtb.Text.Substring(iPos2);
					rtb.SelectionStart = iPos1;
					break;
			}
		}

		private void CmsTextSelect_ネット検索_Google_Click(object sender, EventArgs e)
		{
			SubNetSearch("https://www.google.co.jp/search?q=");
		}

		private void CmsTextSelect_ネット検索_Google翻訳_Click(object sender, EventArgs e)
		{
			SubNetSearch("https://translate.google.com/?hl=ja&sl=auto&tl=ja&op=translate&text=");
		}

		private void CmsTextSelect_ネット検索_Googleマップ_Click(object sender, EventArgs e)
		{
			SubNetSearch("https://www.google.co.jp/maps/place/");
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
			string s1 = "";

			switch (OBJ)
			{
				case TextBox tb:
					s1 = tb.SelectedText;
					break;

				case RichTextBox rtb:
					s1 = rtb.SelectedText;
					break;
			}

			_ = Process.Start(url + HttpUtility.UrlEncode(s1.Replace("\n", " ")));
		}

		private void CmsTextSelect_関連付けられたアプリケーションで開く_Click(object sender, EventArgs e)
		{
			string s1 = "";

			switch (OBJ)
			{
				case TextBox tb:
					s1 = tb.SelectedText;
					break;

				case RichTextBox rtb:
					s1 = rtb.SelectedText;
					break;
			}

			foreach (string _s1 in s1.Split('\n'))
			{
				try
				{
					_ = Process.Start(_s1.Trim());
				}
				catch
				{
				}
			}
		}

		//--------------------------------------------------------------------------------
		// TbCmd へフォーカス
		//--------------------------------------------------------------------------------
		private void SubTbCmdFocus(int iPos)
		{
			if (iPos < 0 || iPos > TbCmd.TextLength)
			{
				iPos = TbCmd.TextLength;
			}
			TbCmd.Select(iPos, 0);
			_ = TbCmd.Focus();

			LblCmd.Visible = true;
			LblCurDir.Visible = LblCmdMemo.Visible = LblResult.Visible = false;
		}

		//--------------------------------------------------------------------------------
		// LblWait
		//--------------------------------------------------------------------------------
		private void SubLblWaitOn(bool bOn = true)
		{
			if (bOn)
			{
				Cursor.Current = Cursors.WaitCursor;
				LblWait.Visible = true;
				Refresh();
			}
			else
			{
				LblWait.Visible = false;
				Cursor.Current = Cursors.Default;
			}
		}

		//--------------------------------------------------------------------------------
		// 正規表現による検索
		//--------------------------------------------------------------------------------
		private string RtnTextGrep(string str, string sRgx, bool bMatch = true)
		{
			if (sRgx.Length == 0)
			{
				return str;
			}

			// マクロ変数に変換
			sRgx = RtnCnvMacroVar(sRgx);

			Regex rgx;

			try
			{
				rgx = new Regex("(?i)" + sRgx, RegexOptions.Compiled);
			}
			catch
			{
				return str;
			}

			int iCnt = 0;
			_ = SB.Clear();

			foreach (string _s1 in Regex.Split(str, NL))
			{
				if (_s1.Length > 0 && bMatch == rgx.IsMatch(_s1))
				{
					_ = SB.Append(_s1);
					_ = SB.Append(NL);
					++iCnt;
				}
			}

			int iBgn = RtbCmdMemo.TextLength;
			RtbCmdMemo.AppendText($"{iCnt}行 該当{NL}");
			int iEnd = RtbCmdMemo.TextLength;
			RtbCmdMemo.SelectionStart = iBgn;
			RtbCmdMemo.SelectionLength = iEnd - iBgn;
			RtbCmdMemo.SelectionColor = Color.Cyan;
			RtbCmdMemo.SelectionStart = iEnd;
			RtbCmdMemo.ScrollToCaret();

			return SB.ToString();
		}

		//--------------------------------------------------------------------------------
		// 正規表現による置換
		//--------------------------------------------------------------------------------
		private string RtnTextReplace(string str, string sOld, string sNew, bool bCnvMacroVar = true)
		{
			if (sOld.Length == 0)
			{
				return str;
			}

			// マクロ変数(拡張表記)を置換
			sNew = Regex.Replace(sNew, @"#\{(\\\S)\}", "$1", RegexOptions.IgnoreCase);

			// 特殊文字を置換
			sNew = sNew.Replace("\\t", "\t");
			sNew = sNew.Replace("\\n", NL);
			sNew = sNew.Replace("\\\\", "\\");
			sNew = sNew.Replace("\\\"", "\"");
			sNew = sNew.Replace("\\\'", "\'");

			// Regex.Replace("12345", "(123)(45)", "$19$2")
			// => NG "$1945"
			// => OK "123945"
			sNew = Regex.Replace(sNew, "(\\$[1-9])([0-9])", "$1\a$2");

			Regex rgx;

			try
			{
				rgx = new Regex("(?i)" + sOld, RegexOptions.Compiled);
			}
			catch
			{
				return str;
			}

			int iLine = 0;
			int iCnt = 0;
			_ = SB.Clear();

			foreach (string _s1 in Regex.Split(str.TrimEnd(), NL))
			{
				string _s2 = rgx.Replace(_s1, sNew).Replace("\a", "");

				// マクロ変数に変換
				if (bCnvMacroVar)
				{
					++iLine;
					_s2 = RtnCnvMacroVar(_s2, iLine);
				}

				if (_s1 != _s2)
				{
					++iCnt;
				}
				_ = SB.Append(_s2);
				_ = SB.Append(NL);
			}

			int iBgn = RtbCmdMemo.TextLength;
			RtbCmdMemo.AppendText($"{iCnt}行 該当{NL}");
			int iEnd = RtbCmdMemo.TextLength;
			RtbCmdMemo.SelectionStart = iBgn;
			RtbCmdMemo.SelectionLength = iEnd - iBgn;
			RtbCmdMemo.SelectionColor = Color.Cyan;
			RtbCmdMemo.SelectionStart = iEnd;
			RtbCmdMemo.ScrollToCaret();

			return SB.ToString();
		}

		//--------------------------------------------------------------------------------
		// 正規表現によるファイル名置換
		//--------------------------------------------------------------------------------
		private string RtnFnRename(string str, string sOld, string sNew)
		{
			// 置換後のファイル名から改行を消除
			sNew = Regex.Replace(sNew, @"(\\n|#\{\\n\})", "");

			// 前後の " を消除
			str = Regex.Replace(str, "^\"+(.+)\"+", "$1");

			string sPartDir = "";
			string sPartFileOld = "";

			foreach (string _s1 in Regex.Split(str.TrimEnd(), NL))
			{
				if (File.Exists(_s1))
				{
					string _s2 = Path.GetDirectoryName(_s1);
					if (_s2.Length == 0)
					{
						_s2 = Environment.CurrentDirectory;
					}
					sPartDir += _s2 + NL;
					sPartFileOld += Path.GetFileName(_s1) + NL;
				}
				// 空行追加
				else
				{
					sPartDir += NL;
					sPartFileOld += NL;
				}
			}

			string rtn = "";
			string memo = "";
			string[] aPartDir = Regex.Split(sPartDir.TrimEnd(), NL);
			string[] aPartFileOld = Regex.Split(sPartFileOld.TrimEnd(), NL);
			int iLine = 0;
			int i1 = 0;

			foreach (string _s1 in Regex.Split(RtnTextReplace(sPartFileOld, sOld, sNew, false).TrimEnd(), NL))
			{
				// マクロ変数に変換
				++iLine;
				string _sNewFn = RtnCnvMacroVar(_s1, iLine);

				string _sOld = $"{aPartDir[i1]}\\{aPartFileOld[i1]}";
				string _sNew = $"{aPartDir[i1]}\\{_sNewFn}";

				++i1;

				if (File.Exists(_sOld))
				{
					memo += $"{_sOld}{NL}=> {_sNewFn}{NL}";

					try
					{
						File.Move(_sOld, _sNew);
						rtn += _sNew + NL;
					}
					catch
					{
						rtn += _sOld + NL;
						memo += "=> [Err] " + (_sOld != _sNew && File.Exists(_sNew) ? "重複するファイル名が存在します。" : "リネームできませんでした。") + NL;
					}
				}
				// 空行追加
				else
				{
					rtn += NL;
				}
			}

			int iBgn = RtbCmdMemo.TextLength;
			RtbCmdMemo.AppendText(memo);
			int iEnd = RtbCmdMemo.TextLength;
			RtbCmdMemo.SelectionStart = iBgn;
			RtbCmdMemo.SelectionLength = iEnd - iBgn;
			RtbCmdMemo.SelectionColor = Color.Cyan;
			RtbCmdMemo.SelectionStart = iEnd;
			RtbCmdMemo.ScrollToCaret();

			return rtn;
		}

		//--------------------------------------------------------------------------------
		// 正規表現による分割
		//--------------------------------------------------------------------------------
		private string RtnTextSplitCnv(string str, string sSplit, string sRgx)
		{
			if (sSplit.Length == 0 || sRgx.Length == 0)
			{
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

			// 行番号付与
			const string sLineNumber = @"\[LN\]";
			int iLineNumber = 0;

			foreach (string _s1 in Regex.Split(str.TrimEnd(), NL))
			{
				string[] a1 = rgx2.Split(_s1);
				string _s2 = sRgx;

				_s2 = Regex.Replace(_s2, sLineNumber, (++iLineNumber).ToString(), RegexOptions.IgnoreCase);

				for (int _i1 = 0; _i1 < a1.Length; _i1++)
				{
					_s2 = _s2.Replace($"[{_i1}]", a1[_i1]);
					_s2 = _s2.Replace("\\t", "\t");
					_s2 = _s2.Replace("\\n", NL);
					_s2 = _s2.Replace("\\\\", "\\");
					_s2 = _s2.Replace("\\\"", "\"");
					_s2 = _s2.Replace("\\\'", "\'");
				}

				// 該当なしの変換子を削除
				_ = SB.Append(rgx1.Replace(_s2, ""));
				_ = SB.Append(NL);
			}

			return SB.ToString();
		}

		//--------------------------------------------------------------------------------
		// Trim
		//--------------------------------------------------------------------------------
		private string RtnTextTrim(string str)
		{
			_ = SB.Clear();
			foreach (string _s1 in Regex.Split(str.TrimEnd(), NL))
			{
				_ = SB.Append(_s1.Trim());
				_ = SB.Append(NL);
			}
			return SB.ToString();
		}

		//--------------------------------------------------------------------------------
		// 全角 <=> 半角
		//--------------------------------------------------------------------------------
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

		//--------------------------------------------------------------------------------
		// 文字クリア
		//--------------------------------------------------------------------------------
		private string RtnTextEraseInLine(string str, string sBgnPos, string sEndPos)
		{
			_ = int.TryParse(sBgnPos, out int iBgnPos);
			_ = int.TryParse(sEndPos, out int iEndPos);

			_ = SB.Clear();

			foreach (string _s1 in Regex.Split(str.TrimEnd(), NL))
			{
				_ = SB.Append(RtnEraseLen(_s1, ' ', iBgnPos, iEndPos));
				_ = SB.Append(NL);
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

		//--------------------------------------------------------------------------------
		// Sort／Sort-R
		//--------------------------------------------------------------------------------
		private string RtnTextSort(string str, bool bAsc)
		{
			List<string> l1 = new List<string>();

			foreach (string _s1 in Regex.Split(str.TrimEnd(), NL))
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

		//--------------------------------------------------------------------------------
		// Uniq
		//--------------------------------------------------------------------------------
		private string RtnTextUniq(string str)
		{
			_ = SB.Clear();
			string flg = "";
			foreach (string _s1 in Regex.Split(str, NL))
			{
				if (_s1 != flg && _s1.TrimEnd().Length > 0)
				{
					_ = SB.Append(_s1);
					_ = SB.Append(NL);
					flg = _s1;
				}
			}
			return SB.ToString();
		}

		//--------------------------------------------------------------------------------
		// Eval計算
		//--------------------------------------------------------------------------------
		private string RtnEvalCalc(string str)
		{
			string rtn = Regex.Replace(str.ToLower(), @"(\s+|math\.)", "");

			// Help
			if (rtn.Length == 0)
			{
				return "pi, pow([NUM],[NUM]), sqrt([NUM]), sin([NUM°]), cos([NUM°]), tan([NUM°])";
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
					_s2 = _s2.Replace($"{_s1}(", "");
					_s2 = _s2.Replace(")", "");

					_ = double.TryParse(_s2, out double _d1);

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

				_ = double.TryParse(_a1[0], out double _d1);
				_ = int.TryParse(_a1[1], out int _i1);

				rtn = rtn.Replace(_m1.Value, Math.Pow(_d1, _i1).ToString());
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

		//--------------------------------------------------------------------------------
		// Binary File ?
		//--------------------------------------------------------------------------------
		private bool RtnIsBinaryFile(string path)
		{
			FileStream fs = File.OpenRead(path);
			long len = fs.Length;
			byte[] ac = new byte[len];
			int size = fs.Read(ac, 0, (int)len);

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

		//--------------------------------------------------------------------------------
		// Main()
		//--------------------------------------------------------------------------------
		internal class Let
		{
			public static string cmd = "";
		}

		static class Program
		{
			[STAThread]
			static void Main(string[] ARGS)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				if (ARGS.Length > 0 && File.Exists(ARGS[0]))
				{
					const string Ext = ".iwmcmd";

					if (Path.GetExtension(ARGS[0]) != Ext)
					{
						_ = MessageBox.Show(
							$"バッチ処理用テキストファイルの拡張子は「{Ext}」にしてください。",
							ProgramID
						);
						return;
					}

					using (StreamReader sr = new StreamReader(ARGS[0], Encoding.GetEncoding("Shift_JIS")))
					{
						Let.cmd = Regex.Replace(sr.ReadToEnd(), RgxCmdNL, ";");
					}
				}
				Application.Run(new Form1());
			}
		}
	}
}
