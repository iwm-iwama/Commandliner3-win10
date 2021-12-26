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
		private const string ProgramID = "iwm_Commandliner3.6";
		private const string VERSION = "Ver.20211226 'A-29' (C)2018-2021 iwm-iwama";

		// 最初に読み込まれる設定ファイル
		private const string ConfigFn = "config.iwmcmd";

		// TextBox, RichTextBox 内のテキスト処理(複数行)に使用 ※改行コード長 NL.Length = 2
		private const string NL = "\r\n";
		// 汎用テキスト処理(単一行)に使用
		private const string RgxNL = "\r??\n";
		private const string RgxCmdNL = "(;|\\s)*\n";

		private readonly object[,] MACRO = {
			// [マクロ]      [説明]                                                                                                            [引数]
			{ "#stream",     "出力行毎に処理          #stream \"dir #{dq}#{}#{dq}\" \"2\" ※\"(コマンド)\" \"(出力)[1..5]\"",                     2 },
			{ "#streamDL",   "出力行毎にダウンロード  #streamDL \"#{}\" \"#{line,3}\" ※\"(出力行)\" \"(ファイル名)拡張子は自動付与\"",           2 },
			{ "#set",        "一時変数                #set \"japan\" \"日本\" => #{%japan} で参照／#set でリスト表示",                            2 },
			{ "#bd",         "開始時のフォルダに戻る",                                                                                            0 },
			{ "#cd",         "フォルダ変更            #cd \"..\" ※フォルダがないときは新規作成します。",                                         1 },
			{ "#cls",        "クリア",                                                                                                            0 },
			{ "#clear",      "全クリア",                                                                                                          0 },
			{ "#echo",       "印字                    #echo \"#{line}\" \"10\" ※\"(出力)\" \"(回数)\"",                                          2 },
			{ "#result",     "出力変更                #result \"2\" ※\"(出力)[1..5]\"",                                                          1 },
			{ "#result+",    "出力列結合              #result+ \"1,2\" \"#{tab}\" ※\"(出力)n,..\" \"(結合文字)\"",                               2 },
			{ "#grep",       "検索                    #grep \"\\d{4}\"   ※\"正規表現\"",                                                         1 },
			{ "#except",     "不一致検索              #except \"\\d{4}\" ※\"正規表現\"",                                                         1 },
			{ "#greps",      "検索（合致数指定）      #greps \"\\\\\" \"1,4\" ※\"正規表現\" \"以上,以下\"",                                      2 },
			{ "#replace",    "置換                    #replace \"^(\\d{2})(\\d{2})\" \"$1+$2\" ※\"正規表現\" \"[$1..$9]\"",                      2 },
			{ "#split",      "分割                    #split \"\\t\" \"[0]#{tab}[1]\" ※\"正規表現\" \"[0..n]分割列\"",                           2 },
			{ "#sort",       "ソート(昇順)",                                                                                                      0 },
			{ "#sort-r",     "ソート(降順)",                                                                                                      0 },
			{ "#uniq",       "前後行の重複行をクリア ※データ全体の重複をクリアするときは #sort と併用",                                          0 },
			{ "#trim",       "文頭文末の空白クリア",                                                                                              0 },
			{ "#rmBlankLn",  "空白行クリア",                                                                                                      0 },
			{ "#rmNL",       "改行をクリア",                                                                                                      0 },
			{ "#toUpper",    "大文字に変換",                                                                                                      0 },
			{ "#toLower",    "小文字に変換",                                                                                                      0 },
			{ "#toWide",     "全角に変換",                                                                                                        0 },
			{ "#toZenNum",   "全角に変換(数字のみ)",                                                                                              0 },
			{ "#toZenKana",  "全角に変換(カナのみ)",                                                                                              0 },
			{ "#toNarrow",   "半角に変換",                                                                                                        0 },
			{ "#toHanNum",   "半角に変換(数字のみ)",                                                                                              0 },
			{ "#toHanKana",  "半角に変換(カナのみ)",                                                                                              0 },
			{ "#dfList",     "フォルダ・ファイル一覧  #dfList \"フォルダ名\"",                                                                    1 },
			{ "#dList",      "フォルダ一覧            #dList \"フォルダ名\"",                                                                     1 },
			{ "#fList",      "ファイル一覧            #fList \"フォルダ名\"",                                                                     1 },
			{ "#wread",      "テキストファイル取得    #wread \"http://.../index.html\" ※UTF-8",                                                  1 },
			{ "#fread",      "テキストファイル読込    #fread \"ファイル名\"",                                                                     1 },
			{ "#fwrite",     "テキストファイル書込    #fwrite \"ファイル名\" \"932 or 65001\" ※932=Shift_JIS／65001=UTF-8",                      2 },
			{ "#rename",     "ファイル名置換          #rename \"(.+)\" \"#{line,4}_$1\" ※\"正規表現\" \"[$1..$9]\"",                             2 },
			{ "#pos",        "フォーム位置            #pos \"50\" \"100\" ※\"横位置(X)\" \"縦位置(Y)\"",                                         2 },
			{ "#size",       "フォームサイズ          #size \"600\" \"600\" ※\"幅(Width)\" \"高さ(Height)\"",                                    2 },
			{ "#sizeMax",    "フォームサイズ（最大化）",                                                                                          0 },
			{ "#sizeMin",    "フォームサイズ（最小化）",                                                                                          0 },
			{ "#sizeNormal", "フォームサイズ（普通）",                                                                                            0 },
			{ "#macroList",  "マクロ一覧",                                                                                                        0 },
			{ "#help",       "操作説明",                                                                                                          0 },
			{ "#version",    "バージョン",                                                                                                        0 },
			{ "#exit",       "終了",                                                                                                              0 }
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
		private string[] AryDgvCmd;

		// Object
		private Process PS = null;
		private object OBJ = null;

		// 一時変数
		private readonly SortedDictionary<string, string> DictHash = new SortedDictionary<string, string>();

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
			"[Ctrl]+[↓]          コンテキストメニュー表示" + NL +
			"[Ctrl]+[Space]       クリア" + NL +
			"[Ctrl]+[Backspace]   カーソルより前方をクリア" + NL +
			"[Ctrl]+[Delete]      カーソルより後方をクリア" + NL +
			NL +
			"[PgUp] or [PgDn]     カーソルを先頭／末尾へ移動" + NL +
			"[↑] or [↓]         空白位置へカーソル移動" + NL +
			NL +
			"[F1]  マクロ・コマンド入力履歴" + NL +
			"[F2]  マクロ選択" + NL +
			"[F3]  コマンド選択" + NL +
			"[F4]  (割り当てなし)" + NL +
			"[F5]  実行" + NL +
			"[F6]  出力を実行前に戻す" + NL +
			"[F7]  出力をクリア" + NL +
			"[F8]  出力履歴" + NL +
			"[F9]  →メモ→出力の順にフォーカス移動" + NL +
			"[F10] (システムメニュー)" + NL +
			"[F11] 出力変更（前へ）" + NL +
			"[F12] 出力変更（次へ）" + NL +
			NL +
			"---------------------------" + NL +
			"> マクロ選択 特殊キー操作 <" + NL +
			"---------------------------" + NL +
			"[←]                 最上部へ移動" + NL +
			"[→]                 最下部へ移動" + NL +
			"[A]..[Z]             下位へ先頭１文字を検索" + NL +
			"[Shift]+[A]..[Z]     上位へ先頭１文字を検索" + NL +
			NL +
			"-----------------------------" + NL +
			"> コマンド検索 特殊キー操作 <" + NL +
			"-----------------------------" + NL +
			"[Enter]              検索開始" + NL +
			"[Ctrl]+[Space]       クリア" + NL +
			"[Ctrl]+[Backspace]   カーソルより前方をクリア" + NL +
			"[Ctrl]+[Delete]      カーソルより後方をクリア" + NL +
			"[↓]                 コマンド選択へ移動" + NL +
			NL +
			"-----------------------------" + NL +
			"> コマンド選択 特殊キー操作 <" + NL +
			"-----------------------------" + NL +
			"[↑](上端で操作)     コマンド検索へ移動" + NL +
			"[←]                 最上部へ移動" + NL +
			"[→]                 最下部へ移動" + NL +
			NL +
			"-------------------------" + NL +
			"> 結果出力 特殊キー操作 <" + NL +
			"-------------------------" + NL +
			"[Ctrl]+[PgUp]        カーソルを最上部へ移動" + NL +
			"[Ctrl]+[PgDn]        カーソルを最下部へ移動" + NL +
			"[Ctrl]+[↑]          カーソル位置から上位を選択" + NL +
			"[Ctrl]+[↓]          カーソル位置から下位を選択" + NL +
			NL +
			"--------------" + NL +
			"> マクロ変数 <" + NL +
			"--------------" + NL +
			"  #{tab}  タブ( \\t )" + NL +
			"  #{nl}   改行( \\r\\n )" + NL +
			"  #{dq}   ダブルクォーテーション( \" )" + NL +
			"  #{sc}   セミコロン( ; )" + NL +
			NL +
			"  #{now}  現時間     (例) 20210501_113400_999" + NL +
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
			"◇#echo, #rename, #replace, #split, #stream, #streamDL で使用可" + NL +
			"    #{line,[ゼロ埋め桁数],[加算値]} 出力の行番号" + NL +
			NL +
			"◇#stream, #streamDL で使用可" + NL +
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
				_ = DgvMacro.Rows.Add(MACRO[_i1, 0], MACRO[_i1, 1]);
			}
			SubDgvCmdLoad();

			// TbResult
			SubTbResultChange(0, true);

			// SplitContainerResult
			SplitContainerResult.Visible = false;

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
			if (ARGS.Length > 0)
			{
				TbCmd.Text = "";
				foreach (string _s1 in ARGS)
				{
					(string _fn, string _data) = RtnTextFileRead(_s1, false, "");
					if (_fn.Length > 0)
					{
						TbCmd.Paste(Regex.Replace(_data, RgxCmdNL, ";"));
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
		private void TbCurDir_Click(object sender, EventArgs e)
		{
			LblCurDir.Visible = true;

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

			LblCurDir.Visible = false;
			SubTbCmdFocus(-1);
		}

		private void TbCurDir_MouseEnter(object sender, EventArgs e)
		{
			ToolTip1.SetToolTip(TbCurDir, TbCurDir.Text.Replace("\\", NL));
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
		//   RichTextBox化すると操作のたび警告音が発生し、やかましくてしょうがない!!
		//   正攻法での解決策を見出せなかったので、TextBoxでの実装にとどめることにした。
		//--------------------------------------------------------------------------------
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
			// [Ctrl]+[C]
			if (e.KeyData == (Keys.Control | Keys.C))
			{
				TbCmd.Copy();
				return;
			}

			// [Ctrl]+[S]
			if (e.KeyData == (Keys.Control | Keys.S))
			{
				Cursor.Position = new Point(Left + ((Width - CmsCmd.Width) / 2), Top + SystemInformation.CaptionHeight + RtbCmdMemo.Location.Y);
				CmsCmd.Show(Cursor.Position);
				CmsCmd_コマンドを保存.Select();
				return;
			}

			// [Ctrl]+[V]
			if (e.KeyData == (Keys.Control | Keys.V))
			{
				TbCmd.Paste();
				return;
			}

			// [Ctrl]+[X]
			if (e.KeyData == (Keys.Control | Keys.X))
			{
				TbCmd.Cut();
				return;
			}

			// [Ctrl]+[Z]
			if (e.KeyData == (Keys.Control | Keys.Z))
			{
				TbCmd.Undo();
				return;
			}
		}

		private void TbCmd_KeyPress(object sender, KeyPressEventArgs e)
		{
			// ビープ音抑制
			if ((ModifierKeys & Keys.Control) == Keys.Control)
			{
				e.Handled = true;
				return;
			}

			// IME入力対策
			//   TextChanged と処理を分担しないとIME操作時に不具合が発生する
			if (e.KeyChar == (char)Keys.Enter)
			{
				BtnCmdExec_Click(sender, null);
				return;
			}
		}

		private void TbCmd_KeyUp(object sender, KeyEventArgs e)
		{
			// [Ctrl]+[Delete]
			if (e.KeyData == (Keys.Control | Keys.Delete))
			{
				TbCmd.Text = TbCmd.Text.Substring(0, TbCmd.SelectionStart);
				// 文字位置を再設定しないと SendMessage で不具合
				TbCmd.SelectionStart = TbCmd.TextLength;
				return;
			}

			// [Ctrl]+[Backspace]
			if (e.KeyData == (Keys.Control | Keys.Back))
			{
				TbCmd.Text = TbCmd.Text.Substring(TbCmd.SelectionStart);
				// 文字位置を再設定しないと SendMessage で不具合
				TbCmd.SelectionStart = 0;
				return;
			}

			// [Ctrl]+[↑]
			if (e.KeyData == (Keys.Control | Keys.Up))
			{
				return;
			}

			// [Ctrl]+[↓]
			if (e.KeyData == (Keys.Control | Keys.Down))
			{
				Cursor.Position = new Point(Left + ((Width - CmsCmd.Width) / 2), Top + SystemInformation.CaptionHeight + RtbCmdMemo.Location.Y);
				CmsCmd.Show(Cursor.Position);
				CmsCmd_マクロ変数.Select();
				return;
			}

			// [Ctrl]+[Space]
			if (e.KeyData == (Keys.Control | Keys.Space))
			{
				TbCmd.Text = "";
				// 文字位置を再設定しないと SendMessage で不具合
				TbCmd.SelectionStart = 0;
				return;
			}

			MatchCollection MC;
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
					_ = RtbCmdMemo.Focus();
					break;

				case Keys.F10: // システムメニュー表示
					SendKeys.Send("{UP}");
					break;

				case Keys.F11:
					SubTbResultForward();
					break;

				case Keys.F12:
					SubTbResultNext();
					break;

				case Keys.Enter:
					// 実行は KeyPress で行う
					break;

				case Keys.PageUp:
					TbCmd.SelectionStart = 0;
					break;

				case Keys.PageDown:
					TbCmd.SelectionStart = TbCmd.TextLength;
					break;

				case Keys.Up:
					MC = Regex.Matches(TbCmd.Text.Substring(0, TbCmd.SelectionStart), @"\S+\s*$");
					TbCmd.SelectionStart = MC.Count > 0 ? MC[0].Index : 0;
					break;

				case Keys.Down:
					iPos = TbCmd.SelectionStart;
					MC = Regex.Matches(TbCmd.Text.Substring(iPos), @"\s\S+");
					TbCmd.SelectionStart = MC.Count > 0 ? iPos + 1 + MC[0].Index : TbCmd.TextLength;
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

		private void TbCmd_MouseEnter(object sender, EventArgs e)
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
			string s1 = "";
			foreach (string _s1 in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				// " で囲む
				// Dir のとき \ 付与
				// 末尾に空白付与
				s1 += Directory.Exists(_s1) ? $"\"{_s1.TrimEnd('\\')}\\\" " : $"\"{_s1}\" ";
			}
			TbCmd.Paste(s1);
			SubTbCmdDQFormat();
		}

		private void SubTbCmdDQFormat()
		{
			// 余計な " を除く
			TbCmd.Text = Regex.Replace(TbCmd.Text, "(.*?)\\s*\"?\\s*((\".+?\"\\s*)+)\\s*\"?", "$1 $2 ").TrimEnd();
			SubTbCmdFocus(-1);
		}

		//--------------------------------------------------------------------------------
		// CmsCmd
		//--------------------------------------------------------------------------------
		private string GblCmsCmdBatch = "";

		private void CmsCmd_Opened(object sender, EventArgs e)
		{
			_ = TbCmd.Focus();

			// #{%[キー]} のメニュー作成
			if (DictHash.Count == 0)
			{
				CmsCmd_マクロ変数_一時変数.Enabled = false;
			}
			else
			{
				CmsCmd_マクロ変数_一時変数.DropDownItems.Clear();

				foreach (KeyValuePair<string, string> _kv1 in DictHash)
				{
					ToolStripMenuItem _tsi = new ToolStripMenuItem
					{
						Text = "#{%" + _kv1.Key + "} " + _kv1.Value
					};
					_tsi.Click += CmsCmd_マクロ変数_一時変数_SubMenuClick;
					_ = CmsCmd_マクロ変数_一時変数.DropDownItems.Add(_tsi);
				}

				CmsCmd_マクロ変数_一時変数.Enabled = true;
			}
		}

		private void CmsCmd_マクロ変数_一時変数_SubMenuClick(object sender, EventArgs e)
		{
			ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
			TbCmd.Paste(Regex.Replace(tsmi.Text, @"^(#\{%.+?\}).+", "$1"));
		}

		private void CmsCmd_Closed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			// 一度カーソルを外さないと表示が消えてしまう
			_ = TbCurDir.Focus();
			// ちらつきを防止
			TbCurDir.Select(0, 0);
			// 再フォーカス
			_ = TbCmd.Focus();
		}

		private void CmsCmd_クリア_Click(object sender, EventArgs e)
		{
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
			TbCmd.SelectAll();
			TbCmd.Paste(Regex.Replace(Clipboard.GetText(), RgxNL, " "));
		}

		private void CmsCmd_貼り付け_Click(object sender, EventArgs e)
		{
			TbCmd.Paste(Regex.Replace(Clipboard.GetText(), RgxNL, " "));
		}

		private void CmsCmd_マクロ変数_タブ_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{tab}");
		}

		private void CmsCmd_マクロ変数_改行_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{nl}");
		}

		private void CmsCmd_マクロ変数_ダブルクォーテーション_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{dq}");
		}

		private void CmsCmd_マクロ変数_セミコロン_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{sc}");
		}

		private void CmsCmd_マクロ変数_現時間_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{now}");
		}

		private void CmsCmd_マクロ変数_日付_Click(object sender, EventArgs e)
		{
			CmsCmd.Close();
			TbCmd.Paste("#{ymd}");
		}

		private void CmsCmd_マクロ変数_時間_Click(object sender, EventArgs e)
		{
			CmsCmd.Close();
			TbCmd.Paste("#{hns}");
		}

		private void CmsCmd_マクロ変数_マイクロ秒_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{msec}");
		}

		private void CmsCmd_マクロ変数_年_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{y}");
		}

		private void CmsCmd_マクロ変数_月_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{m}");
		}

		private void CmsCmd_マクロ変数_日_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{d}");
		}

		private void CmsCmd_マクロ変数_時_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{h}");
		}

		private void CmsCmd_マクロ変数_分_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{n}");
		}

		private void CmsCmd_マクロ変数_秒_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{s}");
		}

		private void CmsCmd_マクロ変数_簡易計算_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{calc,}");
		}

		private void CmsCmd_マクロ変数_出力の行データ_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{}");

		}

		private void CmsCmd_マクロ変数_出力の行番号_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{line,,}");
		}

		private void CmsCmd_マクロ変数_出力のデータ_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{result,}");
		}

		private void CmsCmd_フォルダ選択_Click(object sender, EventArgs e)
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
				// " で囲む
				// \ 付与
				// 末尾に空白付与
				TbCmd.Paste($"\"{fbd.SelectedPath.TrimEnd('\\')}\\\" ");
				SubTbCmdDQFormat();
			}
		}

		private void CmsCmd_ファイル選択_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog
			{
				InitialDirectory = Environment.CurrentDirectory,
				Multiselect = true
			};

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				string s1 = "";
				foreach (string _s1 in ofd.FileNames)
				{
					// " で囲む
					// 末尾に空白付与
					s1 += $"\"{_s1}\" ";
				}
				TbCmd.Paste(s1);
				SubTbCmdDQFormat();
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

		private string GblCmsCmdFn = null;

		private void CmsCmd_コマンドを保存_Click(object sender, EventArgs e)
		{
			string fn = (GblCmsCmdFn == null ? "" : Path.GetDirectoryName(GblCmsCmdFn) + "\\") + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".iwmcmd";
			if (RtnTextFileWrite(RtnCmdFormat(TbCmd.Text).Trim().TrimEnd(';') + ";" + NL, 932, fn, true, CMD_FILTER))
			{
				GblCmsCmdFn = fn;
			}
		}

		private void CmsCmd_コマンドを読込_Click(object sender, EventArgs e)
		{
			CmsCmd.Close();

			(string fn, string data) = RtnTextFileRead(GblCmsCmdFn, true, CMD_FILTER);
			if (fn.Length > 0)
			{
				GblCmsCmdFn = fn;

				TbCmd.Text = Regex.Replace(data, RgxCmdNL, "; ");
				SubTbCmdFocus(-1);

				string s1 = "";
				foreach (string _s1 in GblCmsCmdFn.Split('\\'))
				{
					s1 += $"{_s1}{NL}";
				}
				CmsCmd_コマンドを読込_再読込.ToolTipText = s1.Trim();
			}
		}

		private void CmsCmd_コマンドを読込_再読込_Click(object sender, EventArgs e)
		{
			if (GblCmsCmdFn.Length > 0)
			{
				(_, string data) = RtnTextFileRead(GblCmsCmdFn, false, "");
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

				case Keys.F9:
					_ = TbResult.Focus();
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
			SubRtbCmdMemoResize(false);
		}

		private void RtbCmdMemo_MouseUp(object sender, MouseEventArgs e)
		{
			CmsTextSelect_Open(e, RtbCmdMemo);
		}

		//--------------------------------------------------------------------------------
		// CmsCmdMemo
		//--------------------------------------------------------------------------------
		private void CmsCmdMemo_Opened(object sender, EventArgs e)
		{
			_ = RtbCmdMemo.Focus();
		}

		private void CmsCmdMemo_クリア_Click(object sender, EventArgs e)
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
			if (GblDgvCmdOpen)
			{
				BtnDgvCmd_Click(sender, e);
			}

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

				int i1 = DgvTb11.Width + DgvTb12.Width + 20;
				if (DgvMacro.Width > i1)
				{
					DgvMacro.Width = i1;
				}

				DgvMacro.Height = Height - 217;

				BtnDgvCmdSearch.SendToBack();
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
				TbCmd.Paste(s1 + ";");
				GblTbCmdPos = TbCmd.SelectionStart + s1.Length - iPosForward;
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
					// [F3]はDGVのソートイベントになり干渉するため要注意
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
			if (GblDgvMacroOpen)
			{
				BtnDgvMacro_Click(sender, e);
			}

			if (GblDgvCmdOpen)
			{
				GblDgvCmdOpen = false;
				DgvCmd.Enabled = false;
				BtnDgvCmd.BackColor = Color.RoyalBlue;
				DgvCmd.ScrollBars = ScrollBars.None;
				DgvCmd.Width = 68;
				DgvCmd.Height = 23;

				TbDgvCmdSearch.Visible = false;
				BtnDgvCmdSearch.Visible = false;

				using (DataGridViewCell _o1 = DgvCmd[0, 0])
				{
					_o1.Selected = true;
					_o1.Style.SelectionBackColor = Color.Empty;
					_o1.Style.SelectionForeColor = Color.Empty;
				}

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
				BtnDgvCmdSearch.Visible = true;

				BtnDgvCmdSearch.BringToFront();
				TbDgvCmdSearch.BringToFront();

				_ = TbDgvCmdSearch.Focus();

				using (DataGridViewCell _o1 = DgvCmd[0, 0])
				{
					_o1.Selected = true;
					_o1.Style.SelectionBackColor = DgvCmd.DefaultCellStyle.BackColor;
					_o1.Style.SelectionForeColor = DgvCmd.DefaultCellStyle.ForeColor;
				}
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

		private void DgvCmd_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			DgvCellColor(sender, e, Color.Empty, Color.Empty);
		}

		private void DgvCmd_CellLeave(object sender, DataGridViewCellEventArgs e)
		{
			DgvCellColor(sender, e, DgvCmd.DefaultCellStyle.BackColor, DgvCmd.DefaultCellStyle.ForeColor);
		}

		private void DgvCellColor(object sender, DataGridViewCellEventArgs e, Color BackColor, Color ForeColor)
		{
			if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
			{
				using (DataGridViewCell _o1 = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex])
				{
					_o1.Style.SelectionBackColor = BackColor;
					_o1.Style.SelectionForeColor = ForeColor;
				}
			}
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
				TbCmd.Paste(s1 + ";");
				GblTbCmdPos = TbCmd.SelectionStart + s1.Length + 1;
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
			BtnDgvCmdSearch.Visible = false;
			TbDgvCmdSearch.Visible = false;

			List<string> lst1 = new List<string>();

			// PATH 要素追加
			// Dir 取得
			foreach (string _s1 in Environment.GetEnvironmentVariable("Path").Replace("/", "\\").Split(';'))
			{
				if (Directory.Exists(_s1))
				{
					lst1.Add(_s1.TrimEnd('\\'));
				}
			}

			// Dir 重複排除
			lst1.Sort();
			List<string> lst2 = lst1.Distinct().ToList();
			lst1.Clear();

			// File 取得
			foreach (string _s1 in lst2)
			{
				DirectoryInfo DI = new DirectoryInfo(_s1);
				foreach (FileInfo _fi1 in DI.GetFiles("*", SearchOption.TopDirectoryOnly))
				{
					if (Regex.IsMatch(_fi1.FullName, @"\.(exe|bat)$", RegexOptions.IgnoreCase))
					{
						lst1.Add(Path.GetFileName(_fi1.FullName));
					}
				}
			}
			lst2.Clear();

			// File 重複排除
			lst1.Sort();
			AryDgvCmd = lst1.Distinct().ToArray();
			lst1.Clear();

			foreach (string _s1 in AryDgvCmd)
			{
				_ = DgvCmd.Rows.Add(_s1);
			}
		}

		private void TbDgvCmdSearch_Enter(object sender, EventArgs e)
		{
			TbDgvCmdSearch.BackColor = Color.LightYellow;
			Lbl_F3.ForeColor = Color.Gold;
		}

		private void TbDgvCmdSearch_Leave(object sender, EventArgs e)
		{
			TbDgvCmdSearch.BackColor = Color.Azure;
			Lbl_F3.ForeColor = Color.Gray;
		}

		private void TbDgvCmdSearch_KeyPress(object sender, KeyPressEventArgs e)
		{
			// ビープ音抑制
			if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)
			{
				e.Handled = true;
			}
		}

		private void TbDgvCmdSearch_KeyUp(object sender, KeyEventArgs e)
		{
			bool bReload = false;

			// [Ctrl]+[Space]
			if (e.KeyData == (Keys.Control | Keys.Space))
			{
				TbDgvCmdSearch.Text = "";
				// 文字位置を再設定しないと SendMessage で不具合
				TbDgvCmdSearch.SelectionStart = 0;
				bReload = true;
			}

			// [Ctrl]+[Backspace]
			if (e.KeyData == (Keys.Control | Keys.Back))
			{
				TbDgvCmdSearch.Text = TbDgvCmdSearch.Text.Substring(TbDgvCmdSearch.SelectionStart);
				// 文字位置を再設定しないと SendMessage で不具合
				TbDgvCmdSearch.SelectionStart = 0;
				bReload = true;
			}

			// [Ctrl]+[Delete]
			if (e.KeyData == (Keys.Control | Keys.Delete))
			{
				TbDgvCmdSearch.Text = TbDgvCmdSearch.Text.Substring(0, TbDgvCmdSearch.SelectionStart);
				// 文字位置を再設定しないと SendMessage で不具合
				TbDgvCmdSearch.SelectionStart = TbDgvCmdSearch.TextLength;
				bReload = true;
			}

			// Reload & Return
			if (bReload && TbDgvCmdSearch.TextLength == 0)
			{
				Refresh();
				DgvCmd.Enabled = false;
				DgvCmd.Rows.Clear();
				foreach (string _s1 in AryDgvCmd)
				{
					_ = DgvCmd.Rows.Add(_s1);
				}
				DgvCmd.Enabled = true;
				return;
			}

			switch (e.KeyCode)
			{
				case Keys.Escape:
				case Keys.F3:
					BtnDgvCmd_Click(sender, e);
					break;

				case Keys.F2:
					BtnDgvMacro_Click(sender, e);
					break;

				case Keys.Enter:
					BtnDgvCmdSearch_Click(sender, e);
					break;

				case Keys.PageUp:
					TbDgvCmdSearch.SelectionStart = 0;
					break;

				case Keys.PageDown:
					TbDgvCmdSearch.SelectionStart = TbDgvCmdSearch.TextLength;
					break;

				case Keys.Up:
					break;

				case Keys.Down:
					_ = DgvCmd.Focus();
					break;
			}
		}

		private void BtnDgvCmdSearch_Click(object sender, EventArgs e)
		{
			DgvCmd.Enabled = false;
			DgvCmd.Rows.Clear();
			foreach (string _s1 in AryDgvCmd)
			{
				if (Regex.IsMatch(_s1, TbDgvCmdSearch.Text, RegexOptions.IgnoreCase))
				{
					_ = DgvCmd.Rows.Add(_s1);
				}
			}
			using (DataGridViewCell _o1 = DgvCmd[0, 0])
			{
				_o1.Style.SelectionBackColor = DgvCmd.DefaultCellStyle.BackColor;
				_o1.Style.SelectionForeColor = DgvCmd.DefaultCellStyle.ForeColor;
			}
			DgvCmd.Enabled = true;
			_ = TbDgvCmdSearch.Focus();
		}

		//--------------------------------------------------------------------------------
		// CmsTbDgvCmdSearch
		//--------------------------------------------------------------------------------
		private void CmsTbDgvCmdSearch_Opened(object sender, EventArgs e)
		{
			_ = TbDgvCmdSearch.Focus();
		}

		private void CmsTbDgvCmdSearch_クリア_Click(object sender, EventArgs e)
		{
			DgvCmd.Enabled = false;
			DgvCmd.Rows.Clear();
			foreach (string _s1 in AryDgvCmd)
			{
				_ = DgvCmd.Rows.Add(_s1);
			}
			DgvCmd.Enabled = true;
			TbDgvCmdSearch.Text = "";
		}

		private void CmsTbDgvCmdSearch_貼り付け_Click(object sender, EventArgs e)
		{
			TbDgvCmdSearch.Paste();
		}

		private delegate void MyEventHandler(object sender, DataReceivedEventArgs e);
		private event MyEventHandler MyEvent = null;

		private void MyEventDataReceived(object sender, DataReceivedEventArgs e)
		{
			TbResult.Paste(e.Data + NL);
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

			// 計測開始
			Stopwatch sw = new Stopwatch();
			sw.Start();

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

			// 計測終了
			sw.Stop();
			SubCmdMemoAddRem($"{sw.ElapsedMilliseconds / 1000.0:F3}秒", Color.White);
		}

		private void BtnCmdExecStream_Click(object sender, EventArgs e)
		{
			BtnCmdExecStream.Visible = false;
			ExecStopOn = true;
		}

		private string RtnCmdFormat(string str)
		{
			Regex rgx = new Regex("(?<pattern>\"[^\"]*\"?)");
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
				rgx = new Regex("^(?<pattern>\\S+?)\\s");
				match = rgx.Match(cmd);
				aOp[0] = match.Groups["pattern"].Value;

				// aOp[1..n] 取得
				rgx = new Regex("\"(?<pattern>.*?)\"\\s");
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
					// クリア
					case "#cls":
						RtbCmdMemo.Text = "";
						BtnClear_Click(null, null);
						break;

					// 全クリア
					case "#clear":
						RtbCmdMemo.Text = "";
						TbResult.Text = "";
						for (int _i1 = 0; _i1 <= GblAryResultMax; _i1++)
						{
							GblAryResultBuf[_i1] = "";
						}
						SubTbResultChange(0, true);
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
								TbResult.Paste(Regex.Replace(s1, RgxNL, NL));
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
							TbResult.Paste(Regex.Replace(s2, RgxNL, NL));
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
						TbResult.Select(TbResult.TextLength, 0);
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

						// 出力[n]
						_ = int.TryParse(aOp[2], out i1);
						--i1;
						if (i1 == GblAryResultIndex || i1 < 0 || i1 > GblAryResultMax)
						{
							i1 = -1;
						}

						BtnCmdExecStream.Visible = true;

						int iStreamBgn = RtbCmdMemo.TextLength;
						int iRead = 0;
						int iNL = NL.Length;
						int iLine = 0;

						PS = new Process();
						PS.StartInfo.UseShellExecute = false;
						PS.StartInfo.RedirectStandardInput = true;
						PS.StartInfo.RedirectStandardOutput = true;
						PS.StartInfo.RedirectStandardError = true;
						PS.StartInfo.CreateNoWindow = true;
						PS.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);
						PS.StartInfo.FileName = "cmd.exe";

						foreach (string _s1 in Regex.Split(TbResult.Text, NL))
						{
							++iLine;

							string _s2 = _s1.Trim();
							if (_s2.Length > 0)
							{
								MyEvent = new MyEventHandler(MyEventDataReceived);

								// aOp[1] 本体は変更しない
								PS.StartInfo.Arguments = $"/c {RtnCnvMacroVar(aOp[1], iLine, _s2)}";

								try
								{
									_ = PS.Start();
									// Stdin 入力要求を回避
									PS.StandardInput.Close();
									// Stdout + Stderr
									string _s3 = Regex.Replace(PS.StandardOutput.ReadToEnd(), RgxNL, NL) + Regex.Replace(PS.StandardError.ReadToEnd(), RgxNL, NL);

									// メモ
									RtbCmdMemo.AppendText(_s3);

									// 出力[n]
									if (i1 >= 0)
									{
										GblAryResultBuf[i1] += _s3;
										GblAryResultStartIndex[i1] += _s3.Length;
										if (GblAryResultStartIndex[i1] > 0)
										{
											Controls[GblAryResultBtn[i1]].BackColor = Color.Maroon;
										}
									}

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
									PS.Kill();
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

						PS.Close();

						BtnCmdExecStream.Visible = false;
						break;

					// 出力行毎にダウンロード
					// ローカルファイルのコピーにも使用可
					case "#streamdl":
						if (aOp[1].Length == 0)
						{
							break;
						}

						BtnCmdExecStream.Visible = true;

						iRead = 0;
						iNL = NL.Length;
						iLine = 0;

						foreach (string _s1 in Regex.Split(TbResult.Text, NL))
						{
							++iLine;

							string _s10 = _s1.Trim();
							if (_s10.Length > 0)
							{
								// aOp[1] 本体は変更しない
								// 行データ を渡す
								_s10 = RtnCnvMacroVar(aOp[1], 0, _s10);

								using (WebClient wc = new WebClient())
								{
									// 行番号を渡す
									string _s20 = RtnCnvMacroVar(aOp[2], iLine, "");
									if (_s20.Length > 0)
									{
										if (Path.GetFileName(_s20).Length > 0)
										{
											// 拡張子付与
											_s20 += Path.GetExtension(_s10);
										}
										else
										{
											// ファイル名付与
											_s20 += Path.GetFileName(_s10);
										}
									}
									else
									{
										_s20 = Path.GetFileName(_s10);
									}

									try
									{
										// URLはソノママ処理
										// ローカルの同一ファイルは処理しない
										if (Regex.IsMatch(_s10, @"^[A-Za-z]\:") && Path.GetFullPath(_s10) == Path.GetFullPath(_s20))
										{
										}
										else
										{
											wc.DownloadFile(_s10, _s20);
										}
									}
									catch (Exception ex)
									{
										SubCmdMemoAddRem(_s10, Color.Cyan);
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
						TbResult.Paste(sb.ToString() + NL);
						break;

					// 操作説明
					case "#help":
						TbResult.Paste(HELP_TbCmd + NL);
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
				PS.OutputDataReceived += new DataReceivedEventHandler(ProcessDataReceived);
				PS.StartInfo.FileName = "cmd.exe";
				PS.StartInfo.Arguments = $"/c {RtnCnvMacroVar(cmd, 0, "")}";

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
			rtb.AppendText(str.TrimEnd(';') + NL);
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
		// 出力クリア
		//--------------------------------------------------------------------------------
		private void BtnClear_Click(object sender, EventArgs e)
		{
			TbResult.Text = "";
			SubTbResultCnt();
			SubTbCmdFocus(GblTbCmdPos);
			GC.Collect();
		}

		//--------------------------------------------------------------------------------
		// TbResult
		//--------------------------------------------------------------------------------
		private void CmsResult_Closed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			// 一度カーソルを外さないと表示が消えてしまう
			_ = TbCurDir.Focus();
			// ちらつきを防止
			TbCurDir.Select(0, 0);
			// 再フォーカス
			_ = TbResult.Focus();
		}

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
				DictResultHistory.Add(DateTime.Now.ToString("HH:mm:ss.ff"), TbResult.Text);
			}
		}

		private void TbResult_KeyDown(object sender, KeyEventArgs e)
		{
			// [Ctrl]+[C]
			if (e.KeyData == (Keys.Control | Keys.C))
			{
				TbResult.Copy();
				return;
			}

			// [Ctrl]+[S]
			if (e.KeyData == (Keys.Control | Keys.S))
			{
				Cursor.Position = new Point(Left + ((Width - CmsResult.Width) / 2), Top + SystemInformation.CaptionHeight + (TbResult.Location.Y + TbResult.Height - CmsResult.Height) / 2);
				CmsResult.Show(Cursor.Position);
				CmsResult_名前を付けて保存.Select();
				SendKeys.Send("{RIGHT}");
				return;
			}

			// [Ctrl]+[V]
			if (e.KeyData == (Keys.Control | Keys.V))
			{
				TbResult.Paste();
				return;
			}

			// [Ctrl]+[X]
			if (e.KeyData == (Keys.Control | Keys.X))
			{
				TbResult.Cut();
				return;
			}

			// [Ctrl]+[Z]
			if (e.KeyData == (Keys.Control | Keys.Z))
			{
				TbResult.Undo();
				return;
			}
		}

		private void TbResult_KeyPress(object sender, KeyPressEventArgs e)
		{
			// ビープ音抑制
			if ((ModifierKeys & Keys.Control) == Keys.Control)
			{
				e.Handled = true;
			}
		}

		private void TbResult_KeyUp(object sender, KeyEventArgs e)
		{
			// [Ctrl]+[PgUp]
			if (e.KeyData == (Keys.Control | Keys.PageUp))
			{
				TbResult.Select(0, 0);
				TbResult.ScrollToCaret();
				return;
			}

			// [Ctrl]+[PgDn]
			if (e.KeyData == (Keys.Control | Keys.PageDown))
			{
				TbResult.Select(TbResult.TextLength, 0);
				TbResult.ScrollToCaret();
				return;
			}

			// [Ctrl]+[↑]
			if (e.KeyData == (Keys.Control | Keys.Up))
			{
				TbResult.Select(0, TbResult.SelectionStart);
				return;
			}

			// [Ctrl]+[↓]
			if (e.KeyData == (Keys.Control | Keys.Down))
			{
				TbResult.Select(TbResult.SelectionStart, TbResult.TextLength);
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
					BtnDgvCmd_Click(sender, e);
					break;

				case Keys.F9:
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.F11:
					SubTbResultForward();
					_ = TbResult.Focus();
					break;

				case Keys.F12:
					SubTbResultNext();
					_ = TbResult.Focus();
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
			// ドロップ不可
			e.Effect = DragDropEffects.None;

			SplitContainerResult.Visible = true;
		}

		//--------------------------------------------------------------------------------
		// CmsResult
		//--------------------------------------------------------------------------------
		private void CmsResult_Opened(object sender, EventArgs e)
		{
			_ = TbResult.Focus();
		}

		private void CmsResult_全選択_Click(object sender, EventArgs e)
		{
			_ = TbResult.Focus();
			TbResult.SelectAll();
			SubTbResultCnt();
		}

		private void CmsResult_クリア_Click(object sender, EventArgs e)
		{
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
			TbResult.SelectAll();
			TbResult.Paste(Regex.Replace(Clipboard.GetText(), RgxNL, NL));
		}

		private void CmsResult_貼り付け_Click(object sender, EventArgs e)
		{
			TbResult.Paste(Regex.Replace(Clipboard.GetText(), RgxNL, NL));
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
			TbResult.Paste(sb.ToString());
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
			SubCmsResult_名前を付けて保存(932);
		}

		private void CmsResult_名前を付けて保存_UTF8N_Click(object sender, EventArgs e)
		{
			SubCmsResult_名前を付けて保存(65001);
		}

		private string GblCmsResultFn = null;

		private void SubCmsResult_名前を付けて保存(int encode)
		{
			string fn = (GblCmsResultFn == null ? "" : Path.GetDirectoryName(GblCmsResultFn) + "\\") + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
			if (RtnTextFileWrite(TbResult.Text, encode, fn, true, TEXT_FILTER))
			{
				GblCmsResultFn = fn;
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
			_ = TbResult.Focus();
		}

		private void BtnResult2_Click(object sender, EventArgs e)
		{
			SubTbResultChange(1, true);
			_ = TbResult.Focus();
		}

		private void BtnResult3_Click(object sender, EventArgs e)
		{
			SubTbResultChange(2, true);
			_ = TbResult.Focus();
		}

		private void BtnResult4_Click(object sender, EventArgs e)
		{
			SubTbResultChange(3, true);
			_ = TbResult.Focus();
		}

		private void BtnResult5_Click(object sender, EventArgs e)
		{
			SubTbResultChange(4, true);
			_ = TbResult.Focus();
		}

		// [0..4]
		private const int GblAryResultMax = 4;
		private int GblAryResultIndex = 0;
		private readonly string[] GblAryResultBtn = { "BtnResult1", "BtnResult2", "BtnResult3", "BtnResult4", "BtnResult5" };
		private readonly string[] GblAryResultBuf = { "", "", "", "", "" };
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

		private void SubTbResultForward()
		{
			int i1 = GblAryResultIndex - 1;
			SubTbResultChange(RtnAryResultBtnRangeChk(i1) ? i1 : GblAryResultMax, true);
		}

		private void SubTbResultNext()
		{
			int i1 = GblAryResultIndex + 1;
			SubTbResultChange(RtnAryResultBtnRangeChk(i1) ? i1 : 0, true);
		}

		private void SplitContainerResult_Panel1_Click(object sender, EventArgs e)
		{
			// 誤操作で表示されたままになったとき使用
			SplitContainerResult.Visible = false;
		}

		private void SplitContainerResult_Panel1_DragLeave(object sender, EventArgs e)
		{
			SplitContainerResult.Visible = false;
		}

		private void SplitContainerResult_Panel2_Click(object sender, EventArgs e)
		{
			// 誤操作で表示されたままになったとき使用
			SplitContainerResult.Visible = false;
		}

		private void SplitContainerResult_Panel2_DragLeave(object sender, EventArgs e)
		{
			SplitContainerResult.Visible = false;
		}

		private void BtnPasteFilename_Click(object sender, EventArgs e)
		{
			// 誤操作で表示されたままになったとき使用
			SplitContainerResult.Visible = false;
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
			TbResult.Paste(sb.ToString());

			if (sb.Length > 0)
			{
				TbResult_Leave(sender, e);
			}

			SplitContainerResult.Visible = false;
		}

		private void BtnPasteTextfile_Click(object sender, EventArgs e)
		{
			// 誤操作で表示されたままになったとき使用
			SplitContainerResult.Visible = false;
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
					if (File.Exists(_s1))
					{
						s1 += "・" + Path.GetFileName(_s1) + NL;
					}
				}
			}

			TbResult.Paste(Regex.Replace(sb.ToString(), RgxNL, NL));

			if (sb.Length > 0)
			{
				TbResult_Leave(sender, e);
			}

			SubLblWaitOn(false);
			SplitContainerResult.Visible = false;

			if (s1.Length > 0)
			{
				_ = MessageBox.Show(
					"[Err] テキストファイルではありません。" + NL + NL + s1,
					ProgramID
				);
			}
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
					TbCmd.Paste(s1 + ";");
					GblTbCmdPos = TbCmd.SelectionStart + s1.Length + 1;
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
				_ = CbResultHistory.Items.Add(string.Format("{0}  {1}", _dict.Key, _s1.Replace(NL, "␤")));
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
			Lbl_F8.ForeColor = Color.Gray;
		}

		private void CbResultHistory_DropDownClosed(object sender, EventArgs e)
		{
			if (CbResultHistory.Text.Length > 0)
			{
				// "HH:mm:ss.ff".Length => 11
				TbResult.Text = DictResultHistory[CbResultHistory.Text.Substring(0, 11)];
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
			switch (OBJ)
			{
				case TextBox tb:
					tb.Paste(tb.SelectedText.Replace("\"", ""));
					break;

				case RichTextBox rtb:
					Clipboard.SetText(rtb.SelectedText.Replace("\"", ""));
					rtb.Paste();
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
				//   cmd.exe 内部コマンド(dir, echo, etc)を実行する際、意図したとおり処理されない表記は #{xxx} とした。
				cmd = Regex.Replace(cmd, "#{tab}", "\t", RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{nl}", NL, RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{dq}", "\"", RegexOptions.IgnoreCase);
				cmd = Regex.Replace(cmd, "#{sc}", ";", RegexOptions.IgnoreCase);

				// 日時変数
				DateTime dt = DateTime.Now;
				cmd = Regex.Replace(cmd, "#{now}", dt.ToString("yyyyMMdd_HHmmss_fff"), RegexOptions.IgnoreCase);
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
				rgx = new Regex("#{(?<pattern>\\S+?)}");
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
						if (_i1 >= 0 && _i1 <= GblAryResultMax)
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
					if (bMatch == Regex.IsMatch(_s1, sSearch, RegexOptions.IgnoreCase))
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

				foreach (string _s1 in Regex.Split(GblAryResultBuf[_iResult].TrimEnd(), RgxNL))
				{
					l1.Add(_s1);
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
		private string RtnZenNum(string str)
		{
			return Regex.Replace(str, @"\d+", RtnReplacerWide);
		}

		private string RtnHanNum(string str)
		{
			// Unicode 全角０-９
			return Regex.Replace(str, @"[\uff10-\uff19]+", RtnReplacerNarrow);
		}

		private string RtnZenKana(string str)
		{
			// Unicode 半角カナ
			return Regex.Replace(str, @"[\uff61-\uFF9f]+", RtnReplacerWide);
		}

		private string RtnHanKana(string str)
		{
			// Unicode 全角カナ
			return Regex.Replace(str, @"[\u30A1-\u30F6]+", RtnReplacerNarrow);
		}

		private string RtnReplacerWide(Match m)
		{
			return Strings.StrConv(m.Value, VbStrConv.Wide, 0x411);
		}

		private string RtnReplacerNarrow(Match m)
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
			byte[] bs = File.ReadAllBytes(fn);
			int iNull = 0;
			for (int _iCnt = bs.Length, _i1 = 0; _i1 < _iCnt; _i1++)
			{
				if (bs[_i1] == 0x00)
				{
					// UTF-16 の 1byte 文字に 0x00 が含まれるので誤検知対策
					if (++iNull >= 2)
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
		// UTF-16 LE
		// UTF-16 BE
		// UTF-8 BOM
		//   BOM判定
		//--------------------------------------------------------------------------------
		// UTF-8 NoBOM
		//   1byte:  [0]0x00..0x7F
		//   2byte:  [0]0xC2..0xDF  [1]0x80..0xBF
		//   3byte:  [0]0xE0..0xEF  [1]0x80..0xBF  [2]0x80..0xBF
		//   4byte:  [0]0xF0..0xF7  [1]0x80..0xBF  [2]0x80..0xBF  [3]0x80..0xBF
		//--------------------------------------------------------------------------------
		// Shift_JIS
		//   2byte:  [0]0x81..0x9F or [0]0xE0..0xEC
		//--------------------------------------------------------------------------------
		private bool RtnIsFileEncCp65001(string fn)
		{
			byte[] bs = File.ReadAllBytes(fn);

			if (bs.Length == 0)
			{
				return true;
			}

			// UTF-16 LE
			// UTF-16 BE
			if (bs.Length >= 2)
			{
				if ((bs[0] == 0xFF && bs[1] == 0xFE) || (bs[0] == 0xFE && bs[1] == 0xFF))
				{
					return false;
				}
			}

			// UTF-8 BOM
			if (bs.Length >= 3 && bs[0] == 0xEF && bs[1] == 0xBB && bs[2] == 0xBF)
			{
				return true;
			}

			// UTF-8 NoBOM
			for (int _iCnt = bs.Length, _i1 = 0; _i1 < _iCnt; _i1++)
			{
				// 1byte
				if (bs[_i1] >= 0x00 && bs[_i1] <= 0x7F)
				{
				}
				// 2byte
				else if (bs[_i1] >= 0xC2 && bs[_i1] <= 0xDF)
				{
					++_i1;
					if (_i1 >= _iCnt || bs[_i1] < 0x80 || bs[_i1] > 0xBF)
					{
						return false;
					}
				}
				// 3byte
				else if (bs[_i1] >= 0xE0 && bs[_i1] <= 0xEF)
				{
					for (int _i2 = 2; _i2 > 0; _i2--)
					{
						++_i1;
						if (_i1 >= _iCnt || bs[_i1] < 0x80 || bs[_i1] > 0xBF)
						{
							return false;
						}
					}
				}
				// 4byte
				else if (bs[_i1] >= 0xF0 && bs[_i1] <= 0xF7)
				{
					for (int _i2 = 3; _i2 > 0; _i2--)
					{
						++_i1;
						if (_i1 >= _iCnt || bs[_i1] < 0x80 || bs[_i1] > 0xBF)
						{
							return false;
						}
					}
				}
				// Shift_JIS
				else if ((bs[_i1] & 0xE0) == 0x80 || (bs[_i1] & 0xE0) == 0xE0)
				{
					return false;
				}
				// 上記以外
				else
				{
					return false;
				}
			}
			return true;
		}

		//--------------------------------------------------------------------------------
		// File Read/Write
		//--------------------------------------------------------------------------------
		private const string CMD_FILTER = "All files (*.*)|*.*|Command (*.iwmcmd)|*.iwmcmd";
		private const string TEXT_FILTER = "All files (*.*)|*.*|Text (*.txt)|*.txt|TSV (*.tsv)|*.tsv|CSV (*.csv)|*.csv|HTML (*.html,*.htm)|*.html,*.htm";

		private (string, string) RtnTextFileRead(string path, bool bGuiOn, string filter) // return(ファイル名, データ)
		{
			if (bGuiOn || path.Length == 0)
			{
				OpenFileDialog ofd = new OpenFileDialog
				{
					Filter = filter,
					InitialDirectory = path == null ? Environment.CurrentDirectory : Path.GetDirectoryName(path)
				};

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					path = ofd.FileName;
				}
				else
				{
					return ("", "");
				}
			}

			if (File.Exists(path) && RtnIsTextFile(path))
			{
				// UTF-8(CP65001) でないときは Shift_JIS(CP932) で読込
				return (path, File.ReadAllText(path, Encoding.GetEncoding(RtnIsFileEncCp65001(path) ? 65001 : 932)));
			}

			// Err
			return ("", "");
		}

		private bool RtnTextFileWrite(string str, int encode, string path, bool bGuiOn, string filter)
		{
			if (bGuiOn || path.Length == 0)
			{
				SaveFileDialog sfd = new SaveFileDialog
				{
					FileName = Path.GetFileName(path),
					Filter = filter,
					FilterIndex = 1,
					InitialDirectory = path.Length == 0 ? Environment.CurrentDirectory : Path.GetDirectoryName(path)
				};

				if (sfd.ShowDialog() == DialogResult.OK)
				{
					path = sfd.FileName;
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
					File.WriteAllText(path, str, new UTF8Encoding(false));
					break;

				default:
					File.WriteAllText(path, str, Encoding.GetEncoding(932));
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
		// Debug Utility
		//--------------------------------------------------------------------------------
		/*
		private void D(params string[] args)
		{
			foreach (string _s1 in args)
			{
				Debug.WriteLine(_s1);
			}
		}

		private void M(params string[] args)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string _s1 in args)
			{
				_ = sb.Append(_s1);
				_ = sb.Append("\r\n");
			}
			_ = MessageBox.Show(sb.ToString());
		}
		*/

		//--------------------------------------------------------------------------------
		// Main()
		//--------------------------------------------------------------------------------
		public static string[] ARGS;

		private static class Program
		{
			[STAThread]
			private static void Main(string[] args)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				ARGS = args;

				Application.Run(new Form1());
			}
		}
	}
}
