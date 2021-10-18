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
		private const string VERSION = "Ver.20211017 'A-29' (C)2018-2021 iwm-iwama";
		// Ver.3.2
		//   20211017
		//   20210912
		//   20210907
		//   20210830
		//   20210822
		//   20210807
		//   20210801
		//   20210731
		//   20210715
		// Ver.3.0
		//   20210613
		//   etc.

		private const string ConfigFn = "config.iwmcmd";

		// TextBox, RichTextBox 内のテキスト処理(複数行)に使用 ※改行コード長 NL.Length = 2
		private const string NL = "\r\n";
		// 汎用テキスト処理(単一行)に使用
		private const string RgxNL = "\r??\n";
		private const string RgxCmdNL = "(;|\\s)*\n";

		private readonly object[,] MACRO = {
			// [マクロ]      [説明]                                                                                            [引数]
			{ "#stream",     "出力行毎に処理          #stream \"dir \\\"#{}\\\"\" ※ #{} は出力行データ変数",                     1 },
			{ "#streamDL",   "出力行毎にダウンロード  #streamDL \"#{line,3}\" ※ 拡張子は自動付与／\"ファイル名\"は省略可",       1 },
			{ "#set",        "一時変数                #set \"japan\" \"日本\" => #{%japan} で参照／#set でリスト表示",            2 },
			{ "#bd",         "最初のフォルダに戻る",                                                                              0 },
			{ "#cd",         "フォルダ変更            #cd \"..\" ※フォルダがないときは新規作成します。",                         1 },
			{ "#clear",      "全クリア",                                                                                          0 },
			{ "#cls",        "出力クリア",                                                                                        0 },
			{ "#echo",       "印字                    #echo \"#{line}\" \"10\" ※\"出力\" \"回数\"",                              2 },
			{ "#result",     "出力変更                #result \"2\" ※\"出力1..5\"",                                              1 },
			{ "#result+",    "出力列結合              #result+ \"1,2\" \"#{tab}\" ※\"出力,..\" \"結合文字\"",                    2 },
			{ "#grep",       "検索                    #grep \"\\d{4}\"   ※\"正規表現\"",                                         1 },
			{ "#except",     "不一致検索              #except \"\\d{4}\" ※\"正規表現\"",                                         1 },
			{ "#greps",      "検索（合致数指定）      #greps \"\\\\\" \"1,4\" ※\"正規表現\" \"以上,以下\"",                      2 },
			{ "#replace",    "置換                    #replace \"^(\\d{2})(\\d{2})\" \"$1+$2\" ※\"正規表現\" \"$1..9\"",         2 },
			{ "#split",      "分割                    #split \"\\t\" \"[0]#{tab}[1]\" ※\"正規表現\" \"[0..]分割列\"",            2 },
			{ "#sort",       "ソート(昇順)",                                                                                      0 },
			{ "#sort-r",     "ソート(降順)",                                                                                      0 },
			{ "#uniq",       "前後行の重複行をクリア ※データ全体の重複をクリアするときは #sort と併用",                          0 },
			{ "#trim",       "文頭文末の空白クリア",                                                                              0 },
			{ "#rmBlankLn",  "空白行クリア",                                                                                      0 },
			{ "#rmNL",       "改行をクリア",                                                                                      0 },
			{ "#toUpper",    "大文字に変換",                                                                                      0 },
			{ "#toLower",    "小文字に変換",                                                                                      0 },
			{ "#toWide",     "全角に変換",                                                                                        0 },
			{ "#toZenNum",   "全角に変換(数字のみ)",                                                                              0 },
			{ "#toZenKana",  "全角に変換(カナのみ)",                                                                              0 },
			{ "#toNarrow",   "半角に変換",                                                                                        0 },
			{ "#toHanNum",   "半角に変換(数字のみ)",                                                                              0 },
			{ "#toHanKana",  "半角に変換(カナのみ)",                                                                              0 },
			{ "#dfList",     "フォルダ・ファイル一覧  #dfList \"フォルダ名\"",                                                    1 },
			{ "#dList",      "フォルダ一覧            #dList \"フォルダ名\"",                                                     1 },
			{ "#fList",      "ファイル一覧            #fList \"フォルダ名\"",                                                     1 },
			{ "#wread",      "テキストファイル取得    #wread \"http://.../index.html\" ※ UTF-8",                                 1 },
			{ "#fread",      "テキストファイル読込    #fread \"ファイル名\"",                                                     1 },
			{ "#fwrite",     "テキストファイル書込    #fwrite \"ファイル名\" \"932 or 65001\" ※ 932=Shift_JIS／65001=UTF-8",     2 },
			{ "#rename",     "ファイル名置換          #rename \"(.+)\" \"#{line,4}_$1\" ※\"正規表現\" \"$1..9\"",                2 },
			{ "#pos",        "フォーム位置            #pos \"50\" \"100\" ※\"横位置(X)\" \"縦位置(Y)\"",                         2 },
			{ "#size",       "フォームサイズ          #size \"600\" \"600\" ※\"幅(Width)\" \"高さ(Height)\"",                    2 },
			{ "#sizeMax",    "フォームサイズ（最大化）",                                                                          0 },
			{ "#sizeMin",    "フォームサイズ（最小化）",                                                                          0 },
			{ "#sizeNormal", "フォームサイズ（普通）",                                                                            0 },
			{ "#focus0",     "出力のフォーカス位置を先頭にする",                                                                  0 },
			{ "#focus9",     "出力のフォーカス位置を末尾にする",                                                                  0 },
			{ "#macroList",  "マクロ一覧",                                                                                        0 },
			{ "#help",       "操作説明",                                                                                          0 },
			{ "#version",    "バージョン",                                                                                        0 },
			{ "#exit",       "終了",                                                                                              0 }
		};

		//--------------------------------------------------------------------------------
		// 大域変数
		//--------------------------------------------------------------------------------
		// エラーが発生したとき
		private bool ExecStopOn = false;

		// BaseDir
		private string BaseDir = "";

		// 履歴
		private List<string> ListCmdHistory = new List<string>();
		private readonly SortedDictionary<string, string> DictResultHistory = new SortedDictionary<string, string>();
		private List<string> ListDgvCmd = new List<string>();

		// Object
		private Process PS = null;
		private object OBJ = null;

		// 一時変数
		private readonly SortedDictionary<string, string> DictHash = new SortedDictionary<string, string>();

		internal static class NativeMethods
		{
			[DllImport("User32.dll", CharSet = CharSet.Unicode)]
			internal static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
		}

		public void SubSendMessage(IntPtr hWnd, string lParam)
		{
			_ = NativeMethods.SendMessage(hWnd, 0x00C2, 1, lParam);
		}

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
			"         ↑セミコロンで区切る。" + NL +
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
			"[F4]  （割り当てなし）" + NL +
			"[F5]  実行" + NL +
			"[F6]  出力を実行前に戻す" + NL +
			"[F7]  出力をクリア" + NL +
			"[F8]  出力履歴" + NL +
			"[F9]  （割り当てなし）" + NL +
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
			"  #{tab}  タブ( \\t )" + NL +
			"  #{nl}   改行( \\r\\n )" + NL +
			"  #{dq}   ダブルクォーテーション( \" )" + NL +
			"  #{sc}   セミコロン( ; )" + NL +

			NL +
			"  #{ymd}  年月日     (例) 20210501" + NL +
			"  #{hns}  時分秒     (例) 113400" + NL +
			"  #{msec} マイクロ秒 (例) 999" + NL +
			"  #{y}    年         (例) 2021" + NL +
			"  #{m}    月         (例) 05" + NL +
			"  #{d}    日         (例) 01" + NL +
			"  #{h}    時         (例) 11" + NL +
			"  #{n}    分         (例) 34" + NL +
			"  #{s}    秒         (例) 00" + NL +
			NL +
			"  #{環境変数}        (例) #{OS} => Windows_NT" + NL +
			NL +
			"  #{calc,[式]}       簡易計算" + NL +
			"                     (例) #{calc,180 / pi}" + NL +
			"                       +  -  *  /  %" + NL +
			"                       pi  sin([NUM°])  cos([NUM°])  tan([NUM°])" + NL +
			"                       pow([NUM],[NUM])  sqrt([NUM])" + NL +
			NL +
			"  #{result,[番号]}   出力[番号]データ" + NL +
			NL +
			"  #{%[キー]}         #set で登録された一時変数データ" + NL +
			NL +
			"◇#replace, #split, #rename, #stream で使用可" + NL +
			"    #{line,[ゼロ埋め桁数],[加算値]} 出力の行番号" + NL +
			NL +
			"◇#split, #stream で使用可" + NL +
			"    #{}              出力の行データ" + NL +
			NL +
			"----------------" + NL +
			"> 設定ファイル <" + NL +
			"----------------" + NL +
			"作業フォルダに " + ConfigFn + " ファイルが存在するときは、自動的に読み込みます。" + NL +
			"（フォーム位置・サイズを指定するときに使用）" + NL +
			NL +
			"◇以下はコメント行" + NL +
			"  ・文頭が // の行（単一行コメント）" + NL +
			"  ・文頭 /* から 文頭 */ で囲まれた行（複数行コメント）" + NL +
			NL +
			"◇行末に ; を記述" + NL +
			NL +
			"◇ファイル記述例" + NL +
			"  // 単一行コメント" + NL +
			"  /*" + NL +
			"     複数行コメント" + NL +
			"  */" + NL +
			"  #cls;" + NL +
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
			SubFormStartPosition();

			// TbCurDir
			BaseDir = TbCurDir.Text = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(BaseDir);

			// TbCmd 入力例
			TbCmd.Text = "dir";
			TbCmd_Enter(sender, e);

			// RtbCmdMemo
			GblRtbCmdMemoHeightDefault = RtbCmdMemo.Height;

			// DgvMacro／DgvCmd 表示
			for (int _i1 = 0; _i1 < MACRO.GetLength(0); _i1++)
			{
				string _s1 = "  " + (_i1 + 1).ToString();
				_ = DgvMacro.Rows.Add(MACRO[_i1, 0], MACRO[_i1, 1], _s1.Substring(_s1.Length - 3));
			}
			SubDgvCmdLoad();

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
				(_, string _data) = RtnTextFileRead(ConfigFn, false, "");
				TbCmd.Text = Regex.Replace(_data, RgxCmdNL, ";");
				BtnCmdExec_Click(sender, e);
				TbCmd.Text = "";
			}

			// コマンドライン引数によるバッチ処理
			if (Let.args.Length > 0)
			{
				TbCmd.Text = "";
				foreach (string _s1 in Let.args)
				{
					(string _fn, string _data) = RtnTextFileRead(_s1, false, "");
					if (_fn.Length > 0)
					{
						SubSendMessage(TbCmd.Handle, Regex.Replace(_data, RgxCmdNL, ";"));
					}
				}
				BtnCmdExec_Click(sender, e);
				TbCmd.Text = "";
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

			// RtbCmdMemo　開かれているときだけ調整
			SubRtbCmdMemoResize(RtbCmdMemo.Height > GblRtbCmdMemoHeightDefault);
		}

		private void SubFormStartPosition()
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
				TbCurDir.Text = fbd.SelectedPath;
				Directory.SetCurrentDirectory(TbCurDir.Text);
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

		private void TbCurDir_MouseLeave(object sender, EventArgs e)
		{
			TbCurDir.BackColor = Color.DimGray;
		}

		private void TbCurDir_DragEnter(object sender, DragEventArgs e)
		{
			TbCurDir.BackColor = Color.Black;
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		private void TbCurDir_DragDrop(object sender, DragEventArgs e)
		{
			string s1 = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
			if (Directory.Exists(s1))
			{
				Directory.SetCurrentDirectory(s1);
				TbCurDir.Text = s1;
			}
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
			Lbl_F1.ForeColor = Lbl_F2.ForeColor = Lbl_F3.ForeColor = Lbl_F5.ForeColor = Lbl_F6.ForeColor = Lbl_F7.ForeColor = Lbl_F8.ForeColor = Color.White;
			LblCmd.Visible = true;
		}

		private void TbCmd_Leave(object sender, EventArgs e)
		{
			GblTbCmdPos = TbCmd.SelectionStart;
			Lbl_F1.ForeColor = Lbl_F2.ForeColor = Lbl_F3.ForeColor = Lbl_F5.ForeColor = Lbl_F6.ForeColor = Lbl_F7.ForeColor = Lbl_F8.ForeColor = Color.Gray;
			LblCmd.Visible = false;
		}

		private void TbCmd_TextChanged(object sender, EventArgs e)
		{
			// IME確定 [Enter] で本イベントは発生しない(＝改行されない)ので "\n" の有無で入力モードの判定可能
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
					// TextChanged と処理を分担しないとIME操作時に不具合が発生する
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
				case Keys.Escape:
					SubRtbCmdMemoResize(false);
					break;

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
					break;

				case Keys.F5:
					BtnCmdExec_Click(sender, e);
					break;

				case Keys.F6:
					BtnCmdExecUndo_Click(sender, e);
					break;

				case Keys.F7:
					BtnClear_Click(sender, e);
					break;

				case Keys.F8:
					CbResultHistory.DroppedDown = true;
					_ = CbResultHistory.Focus();
					break;

				case Keys.F9:
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
			ToolTip1.SetToolTip(TbCmd, RtnCmdFormat(TbCmd.Text));
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
			int iPos = TbCmd.SelectionStart;

			// 前後の \ を削除
			try
			{
				if (TbCmd.Text.Substring(iPos, 1) == "\"")
				{
					TbCmd.Text = TbCmd.Text.Remove(iPos, 1);
				}
			}
			catch
			{
			}

			try
			{
				if (TbCmd.Text.Substring(iPos - 1, 1) == "\"")
				{
					--iPos;
					TbCmd.Text = TbCmd.Text.Remove(iPos, 1);
				}
			}
			catch
			{
			}

			string s1 = "";

			foreach (string _s1 in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				s1 += $" \"{_s1}\"";
			}

			s1 = s1.Trim();

			int i1 = iPos + s1.Length;
			TbCmd.Text = TbCmd.Text.Substring(0, iPos) + s1 + TbCmd.Text.Substring(iPos);
			TbCmd.SelectionStart = i1;
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
			SubSendMessage(TbCmd.Handle, "#{tab}");
		}

		private void CmsCmd_マクロ変数_改行_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{nl}");
		}

		private void CmsCmd_マクロ変数_ダブルクォーテーション_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{dq}");
		}

		private void CmsCmd_マクロ変数_セミコロン_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{sc}");
		}

		private void CmsCmd_マクロ変数_日付_Click(object sender, EventArgs e)
		{
			CmsCmd.Close();
			SubSendMessage(TbCmd.Handle, "#{ymd}");
		}

		private void CmsCmd_マクロ変数_時間_Click(object sender, EventArgs e)
		{
			CmsCmd.Close();
			SubSendMessage(TbCmd.Handle, "#{hns}");
		}

		private void CmsCmd_マクロ変数_マイクロ秒_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{msec}");
		}

		private void CmsCmd_マクロ変数_年_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{y}");
		}

		private void CmsCmd_マクロ変数_月_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{m}");
		}

		private void CmsCmd_マクロ変数_日_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{d}");
		}

		private void CmsCmd_マクロ変数_時_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{h}");
		}

		private void CmsCmd_マクロ変数_分_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{n}");
		}

		private void CmsCmd_マクロ変数_秒_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{s}");
		}

		private void CmsCmd_マクロ変数_簡易計算_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{calc,}");
		}

		private void CmsCmd_マクロ変数_出力の行データ_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{}");

		}

		private void CmsCmd_マクロ変数_出力の行番号_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{line,,}");
		}

		private void CmsCmd_マクロ変数_出力のデータ_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{result,}");
		}

		private void CmsCmd_マクロ変数_一時変数_Click(object sender, EventArgs e)
		{
			SubSendMessage(TbCmd.Handle, "#{%}");
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
				// 前後の \ を削除
				try
				{
					if (TbCmd.Text.Substring(iPos, 1) == "\"")
					{
						TbCmd.Text = TbCmd.Text.Remove(iPos, 1);
					}
				}
				catch
				{
				}

				try
				{
					if (TbCmd.Text.Substring(iPos - 1, 1) == "\"")
					{
						--iPos;
						TbCmd.Text = TbCmd.Text.Remove(iPos, 1);
					}
				}
				catch
				{
				}

				int i1 = iPos + fbd.SelectedPath.Length + 2;
				TbCmd.Text = TbCmd.Text.Substring(0, iPos) + "\"" + fbd.SelectedPath + "\"" + TbCmd.Text.Substring(iPos);
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
				// 前後の \ を削除
				try
				{
					if (TbCmd.Text.Substring(iPos, 1) == "\"")
					{
						TbCmd.Text = TbCmd.Text.Remove(iPos, 1);
					}
				}
				catch
				{
				}

				try
				{
					if (TbCmd.Text.Substring(iPos - 1, 1) == "\"")
					{
						--iPos;
						TbCmd.Text = TbCmd.Text.Remove(iPos, 1);
					}
				}
				catch
				{
				}

				string s1 = "";

				foreach (string _s1 in ofd.FileNames)
				{
					s1 += $" \"{_s1}\"";
				}

				s1 = s1.Trim();

				int i1 = iPos + s1.Length + 2;
				TbCmd.Text = TbCmd.Text.Substring(0, iPos) + s1 + TbCmd.Text.Substring(iPos);
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

		private void CmsCmd_コマンドを保存_Click(object sender, EventArgs e)
		{
			_ = RtnTextFileWrite(RtnCmdFormat(TbCmd.Text).Trim().TrimEnd(';') + ";" + NL, 932, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".iwmcmd", true, CMD_FILTER);
		}

		private string GblCmsCmdInputFn = "";

		private void CmsCmd_コマンドを読込_Click(object sender, EventArgs e)
		{
			CmsCmd.Close();

			(string fn, string data) = RtnTextFileRead("", true, CMD_FILTER);
			if (fn.Length > 0)
			{
				GblCmsCmdInputFn = fn;
				TbCmd.Text = Regex.Replace(data, RgxCmdNL, "; ");
				SubTbCmdFocus(-1);

				string s1 = "";
				foreach (string _s1 in GblCmsCmdInputFn.Split('\\'))
				{
					s1 += $"{_s1}{NL}";
				}
				CmsCmd_コマンドを読込_再読込.ToolTipText = s1.Trim();
			}
		}

		private void CmsCmd_コマンドを読込_再読込_Click(object sender, EventArgs e)
		{
			if (GblCmsCmdInputFn.Length > 0)
			{
				(_, string data) = RtnTextFileRead(GblCmsCmdInputFn, false, "");
				TbCmd.Text = Regex.Replace(data, RgxCmdNL, "; ");
				SubTbCmdFocus(-1);
			}
			else
			{
				CmsCmd_コマンドを読込_Click(sender, e);
			}
		}

		//--------------------------------------------------------------------------------
		// RtbCmdMemo
		//--------------------------------------------------------------------------------
		private void RtbCmdMemo_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					SubRtbCmdMemoResize(false);
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.F11:
					_ = TbResult.Focus();
					TbResult.SelectionStart = TbResult.TextLength;
					TbResult.ScrollToCaret();
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

		private void CmsCmdMemo_拡大_Click(object sender, EventArgs e)
		{
			SubRtbCmdMemoResize(true);
		}

		private void CmsCmdMemo_元に戻す_Click(object sender, EventArgs e)
		{
			SubRtbCmdMemoResize(false);
		}

		private int GblRtbCmdMemoHeightDefault;

		private void SubRtbCmdMemoResize(bool bSizeMax)
		{
			RtbCmdMemo.BringToFront();
			RtbCmdMemo.Height = bSizeMax ? Height - 111 : GblRtbCmdMemoHeightDefault;
		}

		//--------------------------------------------------------------------------------
		// DgvMacro
		//--------------------------------------------------------------------------------
		private int GblDgvMacroRow = 0;
		private bool GblDgvMacroOpen = false; // DgvMacro.enabled より速い

		private void BtnDgvMacro_Click(object sender, EventArgs e)
		{
			if (GblDgvMacroOpen)
			{
				GblDgvMacroOpen = false;
				DgvMacro.Enabled = false;
				BtnDgvMacro.BackColor = Color.RoyalBlue;
				DgvMacro.ScrollBars = ScrollBars.None;
				DgvMacro.Width = 68;
				DgvMacro.Height = 23;

				SubTbCmdFocus(GblTbCmdPos);
			}
			else
			{
				GblDgvMacroOpen = true;
				DgvMacro.Enabled = true;
				BtnDgvMacro.BackColor = Color.Crimson;
				DgvMacro.ScrollBars = ScrollBars.Both;
				DgvMacro.Width = Width - 107;

				int i1 = DgvTb11.Width + DgvTb12.Width + ID.Width + 20;
				if (DgvMacro.Width > i1)
				{
					DgvMacro.Width = i1;
				}

				DgvMacro.Height = Height - 217;

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
			// 外部から操作されたとき e は発火しない
			if ((e != null && e.RowIndex == -1) || DgvMacro.CurrentCellAddress.X > 0)
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

				TbCmd.Text = s2 + s1 + " " + s3;
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
		private bool GblDgvCmdOpen = false; // DgvCmd.enabled より速い

		private void BtnDgvCmd_Click(object sender, EventArgs e)
		{
			if (GblDgvCmdOpen)
			{
				GblDgvCmdOpen = false;
				DgvCmd.Enabled = false;
				BtnDgvCmd.BackColor = Color.RoyalBlue;
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
				BtnDgvCmd.BackColor = Color.Crimson;
				DgvCmd.ScrollBars = ScrollBars.Both;
				DgvCmd.Width = Width - 194;

				int i1 = DgvTb21.Width + 20;
				if (DgvCmd.Width > i1)
				{
					DgvCmd.Width = i1;
				}

				DgvCmd.Height = Height - 217;
				TbDgvCmdSearch.Visible = true;

				TbDgvCmdSearch.BringToFront();
				_ = TbDgvCmdSearch.Focus();
			}
		}

		private void TbDgvCmdSearch_MouseHover(object sender, EventArgs e)
		{
			TbDgvCmdSearch.Focus();
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
			// 外部から操作されたとき e は発火しない
			if (e != null && e.RowIndex == -1)
			{
				return;
			}

			string s1 = DgvCmd[0, DgvCmd.CurrentCell.RowIndex].Value.ToString();

			// [Ctrl]+ のときは挿入モード／それ以外は上書き
			if ((ModifierKeys & Keys.Control) == Keys.Control)
			{
				int iPos = TbCmd.SelectionStart;
				string s2 = TbCmd.Text.Substring(0, iPos);
				string s3 = TbCmd.Text.Substring(iPos);

				TbCmd.Text = s2 + s1 + " " + s3;
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

		private void SubDgvCmdLoad()
		{
			TbDgvCmdSearch.Visible = false;

			List<string> l1 = new List<string>();

			// PATH 要素追加
			foreach (string _s1 in Environment.GetEnvironmentVariable("Path").Replace("/", "\\").Split(';'))
			{
				l1.Add(_s1);
			}

			// 重複排除
			ListDgvCmd.Clear();
			l1.Sort();
			l1 = l1.Distinct().ToList();
			// foreach ( ... List.Distinct().ToList()) は NG
			foreach (string _s1 in l1)
			{
				// PATH 以下の実行ファイルを取得
				DirectoryInfo DI = new DirectoryInfo(_s1);
				if (DI.Exists)
				{
					foreach (FileInfo _fi1 in DI.GetFiles("*", SearchOption.TopDirectoryOnly))
					{
						if (Regex.IsMatch(_fi1.FullName, @"\.(exe|bat)$", RegexOptions.IgnoreCase))
						{
							ListDgvCmd.Add(Path.GetFileName(_fi1.FullName));
						}
					}
				}
			}
			l1.Clear();

			// 重複排除
			ListDgvCmd.Sort();
			ListDgvCmd = ListDgvCmd.Distinct().ToList();
			// foreach ( ... List.Distinct().ToList()) は NG
			foreach (string _s1 in ListDgvCmd)
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
				foreach (string _s1 in ListDgvCmd)
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

			foreach (string _s1 in ListDgvCmd)
			{
				_ = DgvCmd.Rows.Add(_s1);
			}
		}

		private void CmsTbDgvCmdSearch_貼り付け_Click(object sender, EventArgs e)
		{
			TbDgvCmdSearch.Paste();
		}

		private delegate void MyEventHandler(object sender, DataReceivedEventArgs e);
		private event MyEventHandler MyEvent = null;

		private void MyEventDataReceived(object sender, DataReceivedEventArgs e)
		{
			SubSendMessage(TbResult.Handle, e.Data + NL);
		}

		private void ProcessDataReceived(object sender, DataReceivedEventArgs e)
		{
			_ = Invoke(MyEvent, new object[2] { sender, e });
		}

		//--------------------------------------------------------------------------------
		// 実行
		//--------------------------------------------------------------------------------
		// コメント /* ～ */
		private bool GblRemOn = false;

		private void BtnCmdExec_Click(object sender, EventArgs e)
		{
			GblRemOn = false;

			// 出力を記憶（実行前）
			GblCmdExecOld = TbResult.Text;

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
			foreach (string _s1 in RtnCmdList(TbCmd.Text))
			{
				if (_s1.Length > 0)
				{
					SubCmdMemoAddText(_s1);
					SubTbCmdExec(_s1);
				}
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

			// 出力を記憶（実行後）
			GblCmdExecNew = TbResult.Text;

			Cursor.Current = Cursors.Default;

			SubTbCmdFocus(GblTbCmdPos);
		}

		private void BtnCmdExecStream_Click(object sender, EventArgs e)
		{
			BtnCmdExecStream.Visible = false;
			ExecStopOn = true;
		}

		private string RtnCmdFormat(string str)
		{
			Regex rgx = new Regex("(?<pattern>\"[^\"]*\"?)", RegexOptions.None);
			foreach (Match _m1 in rgx.Matches(str))
			{
				string _sOld = _m1.Groups["pattern"].Value;

				// " に囲まれた ; を 一時的に \a に変換
				str = str.Replace(_sOld, _sOld.Replace(";", "\a"));
			}

			// ; で改行
			str = Regex.Replace(str, "\\s*;\\s*", ";" + NL);

			// \a を ; に戻す
			return str.Replace("\a", ";");
		}

		private List<string> RtnCmdList(string str)
		{
			return Regex.Split(RtnCmdFormat(str), RgxNL).ToList();
		}

		private void SubTbCmdExec(string cmd)
		{
			// エラーが発生しているとき
			if (ExecStopOn)
			{
				return;
			}

			// 文頭文末の空白と、末尾の ; を消除
			cmd = cmd.Trim().TrimEnd(';');

			// コメント行
			if (Regex.IsMatch(cmd, @"^//"))
			{
				return;
			}

			// コメント /* ～ */
			if (Regex.IsMatch(cmd, @"^/\*"))
			{
				GblRemOn = true;
				return;
			}
			else if (Regex.IsMatch(cmd, @"^\*/"))
			{
				GblRemOn = false;
				cmd = Regex.Replace(cmd, @"^\*/", "").TrimStart();
				if (cmd.Length == 0)
				{
					return;
				}
			}
			else if (GblRemOn)
			{
				return;
			}

			// タイトルに表示
			Text = $"> {cmd}";

			// 変数
			Regex rgx;
			Match match;
			int i1 = 0;
			int i2 = 0;
			string s1 = "";
			string s2 = "";
			StringBuilder sb = new StringBuilder();

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

				// 空の aOp[n] を作成
				// 引数 n が増減したときは見直し
				string[] aOp = { "", "", "", "" };

				// aOp[0] 取得
				rgx = new Regex("^(?<pattern>\\S+?)\\s", RegexOptions.None);
				match = rgx.Match(cmd);
				aOp[0] = match.Groups["pattern"].Value;

				// aOp[1..n] 取得
				rgx = new Regex("\"(?<pattern>.*?)\"\\s", RegexOptions.None);
				i1 = 1;
				foreach (Match _m1 in rgx.Matches(cmd))
				{
					aOp[i1] = _m1.Groups["pattern"].Value;
					if (++i1 >= aOp.Length)
					{
						break;
					}
				}

				// 大小区別しない
				switch (aOp[0].ToLower())
				{
					// 全クリア
					case "#clear":
						TbCmd.Text = "";
						RtbCmdMemo.Text = "";
						TbResult.Text = "";

						for (int _i1 = 0; _i1 < GblAryResultBuf.Length; _i1++)
						{
							GblAryResultBuf[_i1] = "";
						}
						SubTbResultChange(0, true);

						GC.Collect();
						break;

					// 出力クリア
					case "#cls":
						BtnClear_Click(null, null);
						break;

					// 最初のフォルダに戻る
					case "#bd":
						Directory.SetCurrentDirectory(BaseDir);
						TbCurDir.Text = BaseDir;
						break;

					// フォルダ変更
					case "#cd":
						if (aOp[1].Length == 0)
						{
							TbCurDir_Click(null, null);
							break;
						}

						aOp[1] = RtnCnvMacroVar(aOp[1], 0, "");
						string _sFullPath = Path.GetFullPath(aOp[1]);

						try
						{
							// フォルダが存在しないときは新規作成
							if (!Directory.Exists(_sFullPath))
							{
								_ = Directory.CreateDirectory(_sFullPath);
								SubCmdMemoAddRem($"新規フォルダ '{_sFullPath}' を作成しました。", Color.Cyan);
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

					// 印字
					case "#echo":
						if (!int.TryParse(aOp[2], out i1))
						{
							i1 = 1;
						}
						_ = sb.Clear();
						for (int _i1 = 1; _i1 <= i1; _i1++)
						{
							_ = sb.Append(RtnCnvMacroVar(aOp[1], _i1, ""));
						}
						TbResult.AppendText(sb.ToString());
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

					// 検索
					case "#grep":
						TbResult.Text = RtnTextGrep(TbResult.Text, aOp[1], true);
						break;

					// 不一致検索
					case "#except":
						TbResult.Text = RtnTextGrep(TbResult.Text, aOp[1], false);
						break;

					// 検索（合致数指定）
					case "#greps":
						TbResult.Text = RtnTextGreps(TbResult.Text, aOp[1], aOp[2]);
						break;

					// 置換
					case "#replace":
						TbResult.Text = RtnTextReplace(TbResult.Text, aOp[1], aOp[2], true);
						break;

					// 分割変換（like AWK）
					case "#split":
						TbResult.Text = RtnTextSplit(TbResult.Text, aOp[1], aOp[2]);
						break;

					// 文頭文末の空白クリア
					case "#trim":
						TbResult.Text = RtnTextTrim(TbResult.Text);
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

					// 出力列結合
					case "#result+":
						TbResult.Text = RtnColumnJoin(aOp[1], aOp[2]);
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

					// 空白行クリア
					case "#rmblankln":
						TbResult.Text = Regex.Replace(TbResult.Text, $"({NL})+", NL);
						break;

					// 改行をクリア
					case "#rmnl":
						TbResult.Text = Regex.Replace(TbResult.Text, $"({NL})+", "");
						break;

					// テキストファイル取得(UTF-8)
					case "#wread":
					{
						using (WebClient wc = new WebClient())
						{
							try
							{
								// UTF-8(CP65001) で読込
								s1 = Encoding.GetEncoding(65001).GetString(wc.DownloadData(aOp[1]));
								SubSendMessage(TbResult.Handle, Regex.Replace(s1, RgxNL, NL));
							}
							catch (Exception ex)
							{
								SubCmdMemoAddRem(aOp[1], Color.Cyan);
								SubCmdMemoAddRem(ex.Message, Color.Red);
							}
						}
					}
					break;

					// テキストファイル読込
					case "#fread":
						(s1, s2) = RtnTextFileRead(aOp[1], false, "");
						if (s1.Length > 0)
						{
							SubSendMessage(TbResult.Handle, Regex.Replace(s2, RgxNL, NL));
						}
						break;

					// テキストファイル書込
					case "#fwrite":
						_ = int.TryParse(aOp[2], out i1);
						_ = RtnTextFileWrite(TbResult.Text, i1, aOp[1], false, "");
						break;

					// フォルダ・ファイル一覧
					case "#dflist":
					case "#dlist":
					case "#flist":
						aOp[1] = aOp[1].Length > 0 ? RtnCnvMacroVar(aOp[1], 0, "") : ".";
						switch (aOp[0].ToLower())
						{
							case "#dflist":
								i1 = 0;
								break;

							case "#dlist":
								i1 = 1;
								break;

							case "#flist":
								i1 = 2;
								break;
						}
						TbResult.AppendText(RtnDirFileList(aOp[1], i1));
						TbResult.SelectionStart = TbResult.TextLength;
						TbResult.ScrollToCaret();
						break;

					// ファイル名置換
					case "#rename":
						TbResult.Text = RtnFnRename(TbResult.Text, aOp[1], aOp[2]);
						break;

					// 出力行毎に処理
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
								// aOp[1] 本体は変更しない
								// 行番号, 行データ を渡す
								s1 = RtnCnvMacroVar(aOp[1], iLine, _s2);

								MyEvent = new MyEventHandler(MyEventDataReceived);

								PS = new Process();
								PS.StartInfo.UseShellExecute = false;
								PS.StartInfo.RedirectStandardInput = true;
								PS.StartInfo.RedirectStandardOutput = true;
								PS.StartInfo.RedirectStandardError = true;
								PS.StartInfo.CreateNoWindow = true;
								PS.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);

								PS.StartInfo.FileName = "cmd.exe";
								PS.StartInfo.Arguments = $"/c {s1}";

								try
								{
									_ = PS.Start();
									// Stdin 入力要求を回避
									PS.StandardInput.Close();
									// Stdout
									RtbCmdMemo.AppendText(Regex.Replace(PS.StandardOutput.ReadToEnd(), RgxNL, NL));
									// Stderr
									RtbCmdMemo.AppendText(Regex.Replace(PS.StandardError.ReadToEnd(), RgxNL, NL));
									PS.Close();

									// RtbCmdMemo の着色・スクロール
									int iStreamEnd = RtbCmdMemo.TextLength;
									RtbCmdMemo.SelectionStart = iStreamBgn;
									RtbCmdMemo.SelectionLength = iStreamEnd - iStreamBgn;
									RtbCmdMemo.SelectionColor = Color.Lime;
									RtbCmdMemo.SelectionStart = iStreamEnd;
									RtbCmdMemo.ScrollToCaret();
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

							// TbResult の進捗状況
							TbResult.Select(iRead, _s1.Length);
							_ = TbResult.Focus();
							iRead += _s1.Length + iNL;
						}
						BtnCmdExecStream.Visible = false;
						break;

					// 出力行毎にダウンロード
					case "#streamdl":
						BtnCmdExecStream.Visible = true;

						iRead = 0;
						iNL = NL.Length;
						iLine = 0;

						foreach (string _s1 in Regex.Split(TbResult.Text, NL))
						{
							++iLine;

							string _s2 = _s1.Trim();
							if (_s2.Length > 0)
							{
								using (WebClient wc = new WebClient())
								{
									try
									{
										// aOp[1] 本体は変更しない
										if (aOp[1].Trim().Length > 0)
										{
											// 行番号を渡す
											wc.DownloadFile(_s2, RtnCnvMacroVar(aOp[1], iLine, "") + Path.GetExtension(_s2));
										}
										else
										{
											wc.DownloadFile(_s2, Path.GetFileName(_s2));
										}
									}
									catch (Exception ex)
									{
										SubCmdMemoAddRem(_s2, Color.Cyan);
										SubCmdMemoAddRem(ex.Message, Color.Red);
									}
								}

								// 処理中断
								Thread.Sleep(100);
								Application.DoEvents();
								if (ExecStopOn)
								{
									break;
								}
							}

							// TbResult の進捗状況
							TbResult.Select(iRead, _s1.Length);
							_ = TbResult.Focus();
							iRead += _s1.Length + iNL;
						}
						BtnCmdExecStream.Visible = false;
						break;

					// 一時変数
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

					// フォーム位置
					case "#pos":
						// X
						s1 = RtnCnvMacroVar(aOp[1], 0, "");
						if (Regex.IsMatch(s1, @"\d+%"))
						{
							_ = int.TryParse(s1.Replace("%", ""), out i1);
							i1 = (int)(Screen.GetWorkingArea(this).Width / 100.0 * i1);
						}
						else
						{
							_ = int.TryParse(s1, out i1);
						}
						// Y
						s1 = RtnCnvMacroVar(aOp[2], 0, "");
						if (Regex.IsMatch(s1, @"\d+%"))
						{
							_ = int.TryParse(s1.Replace("%", ""), out i2);
							i2 = (int)(Screen.GetWorkingArea(this).Height / 100.0 * i2);
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
						s1 = RtnCnvMacroVar(aOp[1], 0, "");
						if (Regex.IsMatch(s1, @"\d+%"))
						{
							_ = int.TryParse(s1.Replace("%", ""), out i1);
							i1 = (int)(Screen.GetWorkingArea(this).Width / 100.0 * Math.Abs(i1));
						}
						else
						{
							_ = int.TryParse(s1, out i1);
						}
						// Height
						s1 = RtnCnvMacroVar(aOp[2], 0, "");
						if (Regex.IsMatch(s1, @"\d+%"))
						{
							_ = int.TryParse(s1.Replace("%", ""), out i2);
							i2 = (int)(Screen.GetWorkingArea(this).Height / 100.0 * Math.Abs(i2));
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
						_ = sb.Clear();
						_ = sb.Append(
							"--------------" + NL +
							"> マクロ一覧 <" + NL +
							"--------------" + NL +
							"※大文字・小文字を区別しない。(例) #clear と #CLEAR は同じ。" + NL +
							NL +
							"[マクロ]     [説明]" + NL
						);
						for (int _i1 = 0; _i1 < MACRO.Length / 3; _i1++)
						{
							_ = sb.Append(string.Format("{0,-13}{1}", MACRO[_i1, 0], MACRO[_i1, 1]));
							_ = sb.Append(NL);
						}
						SubSendMessage(TbResult.Handle, sb.ToString() + NL);
						break;

					// 操作説明
					case "#help":
						SubSendMessage(TbResult.Handle, HELP_TbCmd + NL);
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
				cmd = RtnCnvMacroVar(cmd, 0, "");

				MyEvent = new MyEventHandler(MyEventDataReceived);

				PS = new Process();
				PS.StartInfo.UseShellExecute = false;
				PS.StartInfo.RedirectStandardInput = true;
				PS.StartInfo.RedirectStandardOutput = true;
				PS.StartInfo.RedirectStandardError = true;
				PS.StartInfo.CreateNoWindow = true;
				PS.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);

				PS.StartInfo.FileName = "cmd.exe";
				PS.StartInfo.Arguments = $"/c {cmd}";

				_ = PS.Start();
				// Stdin 入力要求を回避
				PS.StandardInput.Close();
				// Stdout
				s1 = PS.StandardOutput.ReadToEnd();
				PS.Close();

				// "\n" を \r\n に変換
				s1 = Regex.Replace(s1, RgxNL, NL);

				// ESC は除外
				// 事前に string.IndexOf するより直接 Regex した方が速い
				s1 = Regex.Replace(s1, @"\033\[(\S+?)m", "", RegexOptions.IgnoreCase);

				TbResult.AppendText(s1);
			}

			// TbResult.SelectionStart = NUM 不可
			TbResult.ScrollToCaret();

			SubLblWaitOn(false);
		}

		private void SubCmdMemoAddText(string str)
		{
			RichTextBox rtb = RtbCmdMemo;
			rtb.SelectionStart = rtb.TextLength;
			SubSendMessage(rtb.Handle, str.TrimEnd(';') + NL);
			rtb.SelectionStart = rtb.TextLength;
			rtb.ScrollToCaret();
		}

		private void SubCmdMemoAddRem(string str, Color color)
		{
			int iBgn = RtbCmdMemo.TextLength;
			RtbCmdMemo.AppendText(str + NL);
			int iEnd = RtbCmdMemo.TextLength;
			RtbCmdMemo.SelectionStart = iBgn;
			RtbCmdMemo.SelectionLength = iEnd - iBgn;
			RtbCmdMemo.SelectionColor = color;
			RtbCmdMemo.SelectionStart = iEnd;
			RtbCmdMemo.ScrollToCaret();
		}

		//--------------------------------------------------------------------------------
		// BtnCmdExecUndo
		//--------------------------------------------------------------------------------
		private string GblCmdExecOld = "";
		private string GblCmdExecNew = "";

		private void BtnCmdExecUndo_Click(object sender, EventArgs e)
		{
			TbResult.Text = TbResult.Text == GblCmdExecNew ? GblCmdExecOld : GblCmdExecNew;
			SubTbCmdFocus(GblTbCmdPos);
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
			SubRtbCmdMemoResize(false);
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
			SubSendMessage(TbResult.Handle, Regex.Replace(Clipboard.GetText(), RgxNL, NL));
		}

		private void CmsResult_ファイル名を貼り付け_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string _s1 in Clipboard.GetFileDropList())
			{
				_ = sb.Append(_s1);
				_ = sb.Append((Directory.Exists(_s1) ? @"\" : ""));
				_ = sb.Append(NL);
			}
			SubSendMessage(TbResult.Handle, sb.ToString());
		}

		private void CmsResult_出力画面へコピー_1_Click(object sender, EventArgs e)
		{
			SubCmsResultCopyTo(0);
		}

		private void CmsResult_出力画面へコピー_2_Click(object sender, EventArgs e)
		{
			SubCmsResultCopyTo(1);
		}

		private void CmsResult_出力画面へコピー_3_Click(object sender, EventArgs e)
		{
			SubCmsResultCopyTo(2);
		}

		private void CmsResult_出力画面へコピー_4_Click(object sender, EventArgs e)
		{
			SubCmsResultCopyTo(3);
		}

		private void CmsResult_出力画面へコピー_5_Click(object sender, EventArgs e)
		{
			SubCmsResultCopyTo(4);
		}

		private void SubCmsResultCopyTo(int iIndex)
		{
			if (GblAryResultIndex == iIndex)
			{
				return;
			}

			GblAryResultBuf[iIndex] = TbResult.Text;

			// 非選択タブのうちデータのあるタブは色を変える
			Controls[GblAryResultBtn[iIndex]].BackColor = GblAryResultBuf[iIndex].Length > 0 ? Color.Maroon : Color.DimGray;
		}

		private void CmsResult_名前を付けて保存_SJIS_Click(object sender, EventArgs e)
		{
			_ = RtnTextFileWrite(TbResult.Text, 932, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt", true, TEXT_FILTER);
		}

		private void CmsResult_名前を付けて保存_UTF8N_Click(object sender, EventArgs e)
		{
			_ = RtnTextFileWrite(TbResult.Text, 65001, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt", true, TEXT_FILTER);
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

		private int GblAryResultIndex = 0;
		private readonly string[] GblAryResultBtn = { "BtnResult1", "BtnResult2", "BtnResult3", "BtnResult4", "BtnResult5" };
		private readonly string[] GblAryResultBuf = { "", "", "", "", "", "" };
		private readonly int[] GblAryResultStartIndex = { 0, 0, 0, 0, 0 };

		private bool RtnAryResultBtnRangeChk(int index)
		{
			return index >= 0 && index < GblAryResultBtn.Length;
		}

		private void SubTbResultChange(int index, bool bMoveOn)
		{
			if (index == -1)
			{
				index = GblAryResultIndex;
			}

			if (!RtnAryResultBtnRangeChk(index))
			{
				return;
			}

			// 選択されたタブへ移動
			foreach (string _s1 in GblAryResultBtn)
			{
				if (_s1 == GblAryResultBtn[index])
				{
					Controls[_s1].BackColor = Color.Crimson;

					// 旧タブのデータ保存
					GblAryResultBuf[GblAryResultIndex] = TbResult.Text;
					GblAryResultStartIndex[GblAryResultIndex] = TbResult.SelectionStart;

					TbResult_Leave(null, null);

					// 新タブのデータ読込
					TbResult.Text = GblAryResultBuf[index];
					TbResult.Select(GblAryResultStartIndex[index], 0);

					if (bMoveOn)
					{
						_ = TbResult.Focus();
						TbResult.ScrollToCaret();
					}
					// 新タブ位置を記憶
					GblAryResultIndex = index;
				}
			}

			// 非選択タブのうちデータのあるタブは色を変える
			for (int _i1 = 0; _i1 < GblAryResultBtn.Length; _i1++)
			{
				if (_i1 != GblAryResultIndex)
				{
					Controls[GblAryResultBtn[_i1]].BackColor = GblAryResultBuf[_i1].Length > 0 ? Color.Maroon : Color.DimGray;
				}
			}

			_ = TbCmd.Focus();
		}

		private void SubTbResultNext()
		{
			int i1 = GblAryResultIndex + 1;
			SubTbResultChange(RtnAryResultBtnRangeChk(i1) ? i1 : 0, true);
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

			StringBuilder sb = new StringBuilder();
			string s1 = "";

			foreach (string _s1 in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				(string _s2, string _s3) = RtnTextFileRead(_s1, false, "");
				if (_s2.Length > 0)
				{
					_ = sb.Append(_s3);
				}
				else
				{
					s1 += "・" + Path.GetFileName(_s1) + NL;
				}
			}

			SubSendMessage(TbResult.Handle, Regex.Replace(sb.ToString(), RgxNL, NL));

			if (sb.Length > 0)
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
			StringBuilder sb = new StringBuilder();

			foreach (string _s1 in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				_ = sb.Append(_s1);
				_ = sb.Append(Directory.Exists(_s1) ? @"\" : "");
				_ = sb.Append(NL);
			}
			SubSendMessage(TbResult.Handle, sb.ToString());

			if (sb.Length > 0)
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

			// ListCmdHistory から空白と重複を削除
			_ = ListCmdHistory.RemoveAll(s => s == "");
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
			foreach (string _s1 in ListCmdHistory)
			{
				if (_s1 == TbCmd.Text)
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
				string _s1 = _dict.Value.Substring(0, _dict.Value.Length < 80 ? _dict.Value.Length : 80).TrimStart();
				_ = CbResultHistory.Items.Add(string.Format("{0}  {1}", _dict.Key, _s1.Replace(NL, "■")));
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

		//--------------------------------------------------------------------------------
		// フォントサイズ
		//--------------------------------------------------------------------------------
		private const decimal NudTbResult_BaseSize = 10;
		private decimal NudTbResult_CurSize = NudTbResult_BaseSize;

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

		private void CmsTextSelect_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			// 選択キー [Enter] [↑] [↓]
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
			{
				return;
			}

			CmsTextSelect.Close();

			bool bCapsLock = Control.IsKeyLocked(Keys.CapsLock);

			// [A..Z] [a..z]
			if (e.KeyValue >= 65 && e.KeyValue <= 90)
			{
				switch (OBJ)
				{
					case TextBox tb:
						tb.SelectedText = bCapsLock ? e.KeyCode.ToString().ToUpper() : e.KeyCode.ToString().ToLower();
						break;

					case RichTextBox rtb:
						rtb.SelectedText = bCapsLock ? e.KeyCode.ToString().ToUpper() : e.KeyCode.ToString().ToLower();
						break;
				}
			}

			switch (e.KeyCode)
			{
				// 削除
				case Keys.Delete:
				case Keys.Back:
					switch (OBJ)
					{
						case TextBox tb:
							tb.SelectedText = "";
							break;

						case RichTextBox rtb:
							rtb.SelectedText = "";
							break;
					}
					break;
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
			int iPos1;
			int iPos2;
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

		private void CmsTextSelect_ネット検索_URLを開く_Click(object sender, EventArgs e)
		{
			CmsTextSelect_関連付けられたアプリケーションで開く_Click(sender, e);
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
		private void SubLblWaitOn(bool bOn)
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
		// マクロ変数の置換
		//--------------------------------------------------------------------------------
		private string RtnCnvMacroVar(string cmd, int iLine, string sLine)
		{
			Regex rgx;
			Match match;

			// 行番号を取得
			if (iLine > 0)
			{
				rgx = new Regex("#{(?<pattern>line.*?)}", RegexOptions.IgnoreCase);
				foreach (Match _m1 in rgx.Matches(cmd))
				{
					string _s1 = _m1.Groups["pattern"].ToString();
					string[] _a1 = _s1.Split(',');
					int _iZero = 0;

					if (_a1.Length > 1)
					{
						_ = int.TryParse(_a1[1], out _iZero);
					}

					if (_a1.Length > 2)
					{
						_ = int.TryParse(_a1[2], out int _i1);
						iLine += _i1;
					}

					cmd = cmd.Replace("#{" + _s1 + "}", string.Format("{0:D" + _iZero + "}", iLine));
				}
			}

			// 行データを取得
			if (sLine.Length > 0)
			{
				cmd = cmd.Replace("#{}", sLine);
			}

			if (Regex.IsMatch(cmd, @"#\{.+\}"))
			{
				// 拡張表記
				cmd = Regex.Replace(cmd, "#{tab}", "\t", RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{nl}", NL, RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{dq}", "\"", RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{sc}", ";", RegexOptions.IgnoreCase);

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
				GblAryResultBuf[GblAryResultIndex] = TbResult.Text;
				rgx = new Regex("#{(?<pattern>result.*?)}", RegexOptions.IgnoreCase);
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
							cmd = cmd.Replace("#{" + _s1 + "}", GblAryResultBuf[_i1]);
						}
					}
				}

				// 一時変数
				rgx = new Regex("#{%(?<pattern>.+?)}");
				foreach (Match _m1 in rgx.Matches(cmd))
				{
					string _s1 = _m1.Groups["pattern"].Value.Trim();
					if (DictHash.ContainsKey(_s1))
					{
						cmd = cmd.Replace("#{%" + _s1 + "}", DictHash[_s1]);
					}
				}

				// 簡易計算 <= すべて変換後
				rgx = new Regex("#{(?<pattern>calc.*?)}", RegexOptions.IgnoreCase);
				foreach (Match _m1 in rgx.Matches(cmd))
				{
					string _s1 = _m1.Groups["pattern"].ToString();
					string[] _a1 = _s1.Split(',');
					cmd = cmd.Replace("#{" + _s1 + "}", RtnEvalCalc(_a1.Length > 1 ? _a1[1] : ""));
				}
			}

			return cmd;
		}

		//--------------------------------------------------------------------------------
		// 正規表現による検索
		//--------------------------------------------------------------------------------
		private string RtnTextGrep(string str, string sSearch, bool bMatch)
		{
			if (sSearch.Length == 0)
			{
				return str;
			}

			sSearch = RtnCnvMacroVar(sSearch, 0, "");

			StringBuilder sb = new StringBuilder();
			int iCnt = 0;
			int iWord = 0;
			bool bErr = false;

			foreach (string _s1 in Regex.Split(str, RgxNL))
			{
				// 正規表現文法エラーはないか？
				try
				{
					if (_s1.Length > 0 && bMatch == Regex.IsMatch(_s1, sSearch, RegexOptions.IgnoreCase))
					{
						_ = sb.Append(_s1);
						_ = sb.Append(NL);
						++iCnt;

						Regex rgx = new Regex($"(?<pattern>{sSearch}?)", RegexOptions.IgnoreCase);
						foreach (Match _m1 in rgx.Matches(_s1))
						{
							if (_m1.Groups["pattern"].Value.Length > 0)
							{
								++iWord;
							}
						}
					}
				}
				catch
				{
					bErr = true;
					break;
				}
			}

			if (bErr)
			{
				SubCmdMemoAddRem("文法エラー？", Color.Red);
				return str;
			}
			else
			{
				SubCmdMemoAddRem($"{iCnt}行" + (bMatch ? $" {iWord}個" : "") + $" 該当", Color.Cyan);
				return sb.ToString();
			}
		}

		//--------------------------------------------------------------------------------
		// 正規表現による検索（出現回数指定）
		//--------------------------------------------------------------------------------
		private string RtnTextGreps(string str, string sSearch, string sTimes)
		{
			if (sSearch.Length == 0)
			{
				return str;
			}

			sSearch = RtnCnvMacroVar(sSearch, 0, "");

			StringBuilder sb = new StringBuilder();
			int iCnt = 0;
			bool bErr = false;

			string[] aTimes = sTimes.Split(',');
			int iTimesBgn;
			int iTimesEnd;

			// iTimesBgn 以上、iTimesEnd 以下
			if (aTimes.Length > 1)
			{
				_ = int.TryParse(aTimes[0], out iTimesBgn);
				_ = int.TryParse(aTimes[1], out iTimesEnd);

				// iTimesBgn 以上（Max以下）
				if (iTimesEnd == 0)
				{
					iTimesEnd = str.Length;
				}
			}
			// 一致
			else
			{
				_ = int.TryParse(aTimes[0], out iTimesBgn);
				iTimesEnd = iTimesBgn;
			}

			foreach (string _s1 in Regex.Split(str, RgxNL))
			{
				// 正規表現文法エラーはないか？
				try
				{
					int _i1 = Regex.Split(_s1, sSearch, RegexOptions.IgnoreCase).Count() - 1;
					if (_s1.Trim().Length > 0 && _i1 >= iTimesBgn && _i1 <= iTimesEnd)
					{
						_ = sb.Append(_s1);
						_ = sb.Append(NL);
						++iCnt;
					}
				}
				catch
				{
					bErr = true;
					break;
				}
			}

			if (bErr)
			{
				SubCmdMemoAddRem("文法エラー？", Color.Red);
				return str;
			}
			else
			{
				SubCmdMemoAddRem($"{iCnt}行 該当", Color.Cyan);
				return sb.ToString();
			}
		}

		//--------------------------------------------------------------------------------
		// 正規表現による置換
		//--------------------------------------------------------------------------------
		private string RtnTextReplace(string str, string sOld, string sNew, bool bCnvMacroVar)
		{
			if (str.Length == 0 || sOld.Length == 0)
			{
				return str;
			}

			// Regex.Replace("12345", "(123)(45)", "$1999$2") のとき
			//   => OK "12399945"
			//   => NG "$199945"
			sNew = Regex.Replace(sNew, "(\\$[1-9])([0-9])", "$1\a$2");

			StringBuilder sb = new StringBuilder();
			int iLine = 0;
			int iCnt = 0;
			bool bErr = false;

			foreach (string _s1 in Regex.Split(str, RgxNL))
			{
				string _s2;

				// 正規表現文法エラーはないか？
				try
				{
					_s2 = Regex.Replace(_s1, sOld, sNew, RegexOptions.IgnoreCase).Replace("\a", "");
				}
				catch
				{
					bErr = true;
					break;
				}

				if (bCnvMacroVar)
				{
					++iLine;
					_s2 = RtnCnvMacroVar(_s2, iLine, "");
				}

				if (_s1 != _s2)
				{
					++iCnt;
				}
				_ = sb.Append(_s2);
				_ = sb.Append(NL);
			}

			if (bErr)
			{
				SubCmdMemoAddRem("文法エラー？", Color.Red);
				return str;
			}
			else
			{
				SubCmdMemoAddRem($"{iCnt}行 該当", Color.Cyan);
				return sb.ToString().TrimEnd() + NL;
			}
		}

		//--------------------------------------------------------------------------------
		// 正規表現によるファイル名置換
		//--------------------------------------------------------------------------------
		private string RtnFnRename(string str, string sOld, string sNew)
		{
			string sPartDir = "";
			string sPartFileOld = "";

			foreach (string _s1 in Regex.Split(str, RgxNL))
			{
				string _s2 = _s1.Trim();

				// 文頭文末の " を消除
				_s2 = Regex.Replace(_s2, "^\"(.+)\"", "$1");

				if (File.Exists(_s2))
				{
					string _s3 = Path.GetDirectoryName(_s2);
					if (_s3.Length == 0)
					{
						_s3 = Environment.CurrentDirectory;
					}
					sPartDir += _s3 + NL;
					sPartFileOld += Path.GetFileName(_s2) + NL;
				}
				// 空行追加
				else
				{
					sPartDir += NL;
					sPartFileOld += NL;
				}
			}

			StringBuilder sb = new StringBuilder();
			string[] aPartDir = Regex.Split(sPartDir.TrimEnd(), RgxNL);
			string[] aPartFileOld = Regex.Split(sPartFileOld.TrimEnd(), RgxNL);
			int iLine = 0;
			int i1 = 0;

			foreach (string _s1 in Regex.Split(RtnTextReplace(sPartFileOld, sOld, sNew, false).TrimEnd(), RgxNL))
			{
				++iLine;
				string _sNewFn = RtnCnvMacroVar(_s1.Trim(), iLine, "");
				string _sOld = $"{aPartDir[i1]}\\{aPartFileOld[i1]}";
				string _sNew = $"{aPartDir[i1]}\\{_sNewFn}";
				++i1;

				if (File.Exists(_sOld))
				{
					SubCmdMemoAddRem($"{_sOld}{NL}=> {_sNewFn}", Color.Cyan);

					try
					{
						File.Move(_sOld, _sNew);
						_ = sb.Append(_sNew);
						_ = sb.Append(NL);
					}
					catch
					{
						_ = sb.Append(_sOld);
						_ = sb.Append(NL);
						SubCmdMemoAddRem("=> [Err] " + (_sOld != _sNew && File.Exists(_sNew) ? "重複するファイル名が存在します。" : "リネームできませんでした。"), Color.Red);
					}
				}
				else
				{
					_ = sb.Append(NL);
				}
			}

			return sb.ToString();
		}

		//--------------------------------------------------------------------------------
		// 正規表現による分割
		//--------------------------------------------------------------------------------
		private string RtnTextSplit(string str, string sSplit, string sReplace)
		{
			if (str.Length == 0 || sSplit.Length == 0 || sReplace.Length == 0)
			{
				return str;
			}

			StringBuilder sb = new StringBuilder();
			int iLine = 0;
			bool bErr = false;

			// 行末の空白行(＝無データ)は対象外
			foreach (string _s1 in Regex.Split(str.TrimEnd(), RgxNL))
			{
				// sReplace 本体は変更しない
				string _s2 = sReplace;

				// 正規表現文法エラーはないか？
				try
				{
					string[] a1 = Regex.Split(_s1, sSplit, RegexOptions.IgnoreCase);

					for (int _i1 = 0; _i1 < a1.Length; _i1++)
					{
						_s2 = _s2.Replace($"[{_i1}]", a1[_i1]);
					}
				}
				catch
				{
					bErr = true;
					break;
				}

				// 該当なしの変換子を削除
				_s2 = Regex.Replace(_s2, @"\[\d+\]", "");

				// #{calc,} 使用可
				++iLine;
				_s2 = RtnCnvMacroVar(_s2, iLine, _s1);

				_ = sb.Append(_s2);
				_ = sb.Append(NL);
			}

			if (bErr)
			{
				SubCmdMemoAddRem("文法エラー？", Color.Red);
				return str;
			}
			else
			{
				return sb.ToString();
			}
		}

		//--------------------------------------------------------------------------------
		// TbResult を列結合
		//--------------------------------------------------------------------------------
		private string RtnColumnJoin(string str, string sSeparater)
		{
			sSeparater = RtnCnvMacroVar(sSeparater, 0, "");

			// lResult[行 0..n][出力 0..4]
			List<string[]> lResult = new List<string[]>();
			List<string> l1;

			// 出力[0..4]を行毎に配列化
			for (int _iResult = 0; _iResult < 5; _iResult++)
			{
				l1 = new List<string>();
				int _iLine = 0;

				foreach (string _s1 in GblAryResultBuf[_iResult].TrimEnd().Split('\n'))
				{
					l1.Add(_s1.Trim());
					++_iLine;
				}

				lResult.Add(l1.ToArray());
			}

			// 出力[0..4]のうち最も長い行長を取得
			string[] aResult = str.Split(',');
			int lSize = 0;

			for (int _i1 = 0; _i1 < aResult.Length; _i1++)
			{
				if (lSize < lResult[_i1].Length)
				{
					lSize = lResult[_i1].Length;
				}
			}

			StringBuilder sb = new StringBuilder();

			for (int _i1 = 0; _i1 < lSize; _i1++)
			{
				for (int _i2 = 0; _i2 < aResult.Length; _i2++)
				{
					try
					{
						int.TryParse(aResult[_i2], out int _i3);
						_ = sb.Append(lResult[_i3 - 1][_i1]);
					}
					catch
					{
					}

					if (_i2 < aResult.Length - 1)
					{
						_ = sb.Append(sSeparater);
					}
				}
				_ = sb.Append(NL);
			}

			return sb.ToString();
		}

		//--------------------------------------------------------------------------------
		// Trim
		//--------------------------------------------------------------------------------
		private string RtnTextTrim(string str)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string _s1 in Regex.Split(str, RgxNL))
			{
				_ = sb.Append(_s1.Trim());
				_ = sb.Append(NL);
			}
			return sb.ToString().TrimEnd() + NL;
		}

		//--------------------------------------------------------------------------------
		// 全角 <=> 半角
		//--------------------------------------------------------------------------------
		private static string RtnZenNum(string str)
		{
			return Regex.Replace(str, @"\d+", RtnReplacerWide);
		}

		private static string RtnHanNum(string str)
		{
			// Unicode 全角０-９
			return Regex.Replace(str, @"[\uff10-\uff19]+", RtnReplacerNarrow);
		}

		private static string RtnZenKana(string str)
		{
			// Unicode 半角カナ
			return Regex.Replace(str, @"[\uff61-\uFF9f]+", RtnReplacerWide);
		}

		private static string RtnHanKana(string str)
		{
			// Unicode 全角カナ
			return Regex.Replace(str, @"[\u30A1-\u30F6]+", RtnReplacerNarrow);
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
		// Sort／Sort-R
		//--------------------------------------------------------------------------------
		private string RtnTextSort(string str, bool bAsc)
		{
			List<string> l1 = new List<string>();
			foreach (string _s1 in Regex.Split(str, RgxNL))
			{
				l1.Add(_s1);
			}
			l1.Sort();
			_ = l1.RemoveAll(s => s.Length == 0);
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
			StringBuilder sb = new StringBuilder();
			string flg = null;
			foreach (string _s1 in Regex.Split(str, RgxNL))
			{
				if (_s1 != flg && _s1.Length > 0)
				{
					flg = _s1;
					_ = sb.Append(_s1);
					_ = sb.Append(NL);
				}
			}
			return sb.ToString();
		}

		//--------------------------------------------------------------------------------
		// Eval計算
		//--------------------------------------------------------------------------------
		private string RtnEvalCalc(string str)
		{
			//【説明】
			//   +  -  *  /  %
			//   pi  sin([NUM°])  cos([NUM°])  tan([NUM°])
			//   pow([NUM],[NUM])  sqrt([NUM])

			string rtn = Regex.Replace(str.ToLower(), @"(\s+|math\.)", "");

			if (rtn.Length == 0)
			{
				return "[Err]";
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
					rtn = "[Err]";
				}
			}

			return rtn;
		}

		//--------------------------------------------------------------------------------
		// Text File ?
		//--------------------------------------------------------------------------------
		private bool RtnIsTextFile(string fn)
		{
			FileStream fs = File.OpenRead(fn);
			long len = fs.Length;
			byte[] ac = new byte[len];
			int size = fs.Read(ac, 0, (int)len);

			int iNull = 0;

			for (int _i1 = 0; _i1 < size; _i1++)
			{
				if (ac[_i1] == 0)
				{
					if (++iNull > 2)
					{
						return false;
					}
				}
				else
				{
					iNull = 0;
				}
			}
			return true;
		}

		//--------------------------------------------------------------------------------
		// UTF-8 判定
		//   1byte: [0]0x00..0x7F
		//   2byte: [0]0xC2..0xDF  [1]0x80..0xBF
		//   3byte: [0]0xE0..0xEF  [1]0x80..0xBF  [2]0x80..0xBF
		//   4byte: [0]0xF0..0xF7  [1]0x80..0xBF  [2]0x80..0xBF	[3]0x80..0xBF
		//--------------------------------------------------------------------------------
		private bool RtnIsFileEncCp65001(string fn)
		{
			byte[] bs = File.ReadAllBytes(fn);

			for (int _i1 = 1; _i1 < bs.Length; _i1++)
			{
				// 1byte
				if (bs[_i1] >= 0x00 && bs[_i1] <= 0x7F)
				{
				}
				// 2byte
				else if (bs[_i1] >= 0xC2 && bs[_i1] <= 0xDF)
				{
					++_i1;
					if (bs[_i1] < 0x80 || bs[_i1] > 0xBF)
					{
						return false;
					}
				}
				// 3byte
				else if (bs[_i1] >= 0xE0 && bs[_i1] <= 0xEF)
				{
					++_i1;
					for (int _i2 = 0; _i2 < 2 && _i1 < bs.Length; _i2++)
					{
						if (bs[_i1] < 0x80 || bs[_i1] > 0xBF)
						{
							return false;
						}
						++_i1;
					}
				}
				// 4byte
				else if (bs[_i1] >= 0xF0 && bs[_i1] <= 0xF7)
				{
					++_i1;
					for (int _i2 = 0; _i2 < 3 && _i1 < bs.Length; _i2++)
					{
						if (bs[_i1] < 0x80 || bs[_i1] > 0xBF)
						{
							return false;
						}
						++_i1;
					}
				}
			}
			return true;
		}

		//--------------------------------------------------------------------------------
		// File Read/Write
		//--------------------------------------------------------------------------------
		private const string CMD_FILTER = "Command (*.iwmcmd)|*.iwmcmd|All files (*.*)|*.*";
		private const string TEXT_FILTER = "Text (*.txt)|*.txt|TSV (*.tsv)|*.tsv|CSV (*.csv)|*.csv|HTML (*.html,*.htm)|*.html,*.htm|All files (*.*)|*.*";

		private (string, string) RtnTextFileRead(string fn, bool bGuiOn, string filter) // return(ファイル名, データ)
		{
			if (bGuiOn || fn.Length == 0)
			{
				OpenFileDialog ofd = new OpenFileDialog
				{
					Filter = filter,
					InitialDirectory = Environment.CurrentDirectory
				};

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					fn = ofd.FileName;
				}
				else
				{
					return ("", "");
				}
			}

			if (File.Exists(fn) && RtnIsTextFile(fn))
			{
				// UTF-8(CP65001) でないときは Shift_JIS(CP932) で読込
				return (fn, File.ReadAllText(fn, Encoding.GetEncoding(RtnIsFileEncCp65001(fn) ? 65001 : 932)));
			}

			// Err
			return ("", "");
		}

		private bool RtnTextFileWrite(string str, int encode, string fn, bool bGuiOn, string filter)
		{
			if (bGuiOn || fn.Length == 0)
			{
				SaveFileDialog sfd = new SaveFileDialog
				{
					FileName = fn,
					Filter = filter,
					FilterIndex = 1,
					InitialDirectory = Environment.CurrentDirectory
				};

				if (sfd.ShowDialog() == DialogResult.OK)
				{
					fn = sfd.FileName;
				}
				else
				{
					return false;
				}
			}

			// UTF-8(CP65001) でないときは Shift_JIS(CP932) で書込
			switch (encode)
			{
				case 65001:
					// UTF-8N(BOMなし)
					File.WriteAllText(fn, str, new UTF8Encoding(false));
					break;

				default:
					File.WriteAllText(fn, str, Encoding.GetEncoding(932));
					break;
			}

			return true;
		}

		//--------------------------------------------------------------------------------
		// Dir / File 取得
		//--------------------------------------------------------------------------------
		private readonly List<string> GblRtnDirList = new List<string>();
		private readonly List<string> GblRtnFileList = new List<string>();

		private string RtnDirFileList(string path, int iDirFile)
		{
			// iDirFile
			//   0 = Dir + File
			//   1 = Dir
			//   2 = File

			string s1 = Path.GetFullPath(path + "\\");

			if (!Directory.Exists(s1))
			{
				return "";
			}

			GblRtnDirList.Clear();
			GblRtnFileList.Clear();

			// Dir
			GblRtnDirList.Add(s1);
			SubDirList(s1);

			// File
			if (iDirFile != 1)
			{
				SubFileList();
			}

			string rtn;
			int iCnt;

			switch (iDirFile)
			{
				// Dir
				case (1):
					GblRtnDirList.Sort();
					rtn = string.Join(NL, GblRtnDirList);
					iCnt = GblRtnDirList.Count();
					break;

				// File
				case (2):
					GblRtnFileList.Sort();
					rtn = string.Join(NL, GblRtnFileList);
					iCnt = GblRtnFileList.Count();
					break;

				// Dir + File
				default:
					GblRtnDirList.AddRange(GblRtnFileList);
					GblRtnDirList.Sort();
					rtn = string.Join(NL, GblRtnDirList);
					iCnt = GblRtnDirList.Count();
					break;
			}

			GblRtnDirList.Clear();
			GblRtnFileList.Clear();
			GC.Collect();

			SubCmdMemoAddRem($"{iCnt}行 該当", Color.Cyan);

			return rtn + NL;
		}

		// 再帰
		private void SubDirList(string path)
		{
			// Dir 取得
			// SearchOption.AllDirectories はシステムフォルダ・アクセス時にエラーが出るので使用不可
			foreach (string _s1 in Directory.GetDirectories(path, "*"))
			{
				GblRtnDirList.Add(_s1 + "\\");
				try
				{
					SubDirList(_s1);
				}
				catch
				{
					// エラー・キーは削除
					_ = GblRtnDirList.Remove(_s1 + "\\");
				}
			}
		}

		private void SubFileList()
		{
			// File 取得
			foreach (string _s1 in GblRtnDirList)
			{
				foreach (string _s2 in Directory.GetFiles(_s1, "*"))
				{
					GblRtnFileList.Add(_s2);
				}
			}
		}

		//--------------------------------------------------------------------------------
		// Main()
		//--------------------------------------------------------------------------------
		public class Let
		{
			public static string[] args;
		}

		private static class Program
		{
			[STAThread]
			private static void Main(string[] ARGS)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				Let.args = ARGS;

				Application.Run(new Form1());
			}
		}
	}
}
