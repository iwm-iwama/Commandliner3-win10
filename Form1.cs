using Microsoft.VisualBasic; // プロジェクト～参照の追加

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
		private const string ProgramID = "iwm_Commandliner3.10";
		private const string VERSION = "Ver.20220416 'A-29' (C)2018-2022 iwm-iwama";

		// 最初に読み込まれる設定ファイル
		private const string ConfigFn = "config.iwmcmd";

		// TextBox, RichTextBox 内のテキスト処理(複数行)に使用 ※改行コード長 NL.Length = 2
		private const string NL = "\r\n";

		// "\r\n" "\n" の２パターンに対応
		private const string RgxNL = "\r?\n";

		// 設定ファイルの行末尾
		private const string RgxCmdNL = "(;|\\s)*\n";

		private readonly object[] AryDgvMacro = {
		//  [マクロ]         [説明]                                                                                                              [引数]
			"#932",          "コマンド実行時の出力文字コード Shift_JIS ※初期設定値",                                                                0,
			"#65001",        "コマンド実行時の出力文字コード UTF-8",                                                                                 0,
			"#stream",       "出力行毎に処理          #stream \"dir #{dq}#{}#{dq}\" \"2\" ※\"(コマンド)\" \"(出力)[1..5]\"",                        2,
			"#streamDL",     "出力行毎にダウンロード  #streamDL \"#{}\" \"#{line,3}\" ※\"(出力行)\" \"(ファイル名)拡張子は自動付与\"",              2,
			"#set",          "一時変数                #set \"japan\" \"日本\" => #{%japan} で参照／#set でリスト表示",                               2,
			"#bd",           "開始時のフォルダに戻る",                                                                                               0,
			"#cd",           "フォルダ変更            #cd \"..\" ※フォルダがないときは新規作成します。",                                            1,
			"#cls",          "クリア",                                                                                                               0,
			"#clear",        "全クリア",                                                                                                             0,
			"#echo",         "印字                    #echo \"#{line,4}#{tab}DATA#{nl}\" \"10\" ※\"(出力)\" \"(回数)\"",                            2,
			"#result",       "出力変更                #result \"2\" ※\"(出力)[1..5]\"",                                                             1,
			"#result+",      "出力列結合              #result+ \"1,2\" \"#{tab}\" ※\"(出力)n,..\" \"(結合文字)\"",                                  2,
			"#grep",         "検索                    #grep \"\\d{4}\"   ※\"正規表現\"",                                                            1,
			"#except",       "不一致検索              #except \"\\d{4}\" ※\"正規表現\"",                                                            1,
			"#greps",        "検索（合致数指定）      #greps \"\\\\\" \"1,4\" ※\"正規表現\" \"以上,以下\"",                                         2,
			"#extract",      "抽出                    #extract \"http\\S+?\\.(jpg|png)\" ※\"正規表現\"",                                            1,
			"#replace",      "置換                    #replace \"^(\\d{2})(\\d{2})\" \"$1+$2\" ※\"正規表現\" \"[$1..$9]\"",                         2,
			"#split",        "分割                    #split \"#{tab}\" \"[0]#{tab}[1]\" ※\"正規表現\" \"[0..n]分割列\"",                           2,
			"#sort",         "ソート(昇順)",                                                                                                         0,
			"#sort-r",       "ソート(降順)",                                                                                                         0,
			"#uniq",         "前後行の重複行をクリア  ※データ全体の重複をクリアするときは #sort と併用",                                            0,
			"#trim",         "文頭文末の空白クリア",                                                                                                 0,
			"#rmBlankLn",    "空白行クリア",                                                                                                         0,
			"#rmNL",         "改行をクリア",                                                                                                         0,
			"#toUpper",      "大文字に変換",                                                                                                         0,
			"#toLower",      "小文字に変換",                                                                                                         0,
			"#toWide",       "全角に変換",                                                                                                           0,
			"#toZenNum",     "全角に変換(数字のみ)",                                                                                                 0,
			"#toZenKana",    "全角に変換(カナのみ)",                                                                                                 0,
			"#toNarrow",     "半角に変換",                                                                                                           0,
			"#toHanNum",     "半角に変換(数字のみ)",                                                                                                 0,
			"#toHanKana",    "半角に変換(カナのみ)",                                                                                                 0,
			"#dfList",       "フォルダ・ファイル一覧  #dfList \"フォルダ名\"",                                                                       1,
			"#dList",        "フォルダ一覧            #dList \"フォルダ名\"",                                                                        1,
			"#fList",        "ファイル一覧            #fList \"フォルダ名\"",                                                                        1,
			"#grepFile",     "テキストファイル検索    #grepFile \"(grep|File)\" \"フォルダ名 or ファイル名\" ※\"正規表現\" \"最大250個\"",          2,
			"#wread",        "テキストファイル取得    #wread \"http://.../index.html\" ※UTF-8",                                                     1,
			"#fread",        "テキストファイル読込    #fread \"ファイル名\"",                                                                        1,
			"#fwrite",       "テキストファイル書込    #fwrite \"ファイル名\" \"932 or 65001\" ※932=Shift_JIS／65001=UTF-8",                         2,
			"#rename",       "ファイル名置換          #rename \"(.+)\" \"#{line,4}_$1\" ※\"正規表現\" \"[$1..$9]\"",                                2,
			"#pos",          "フォーム位置            #pos \"50\" \"100\" ※\"横位置(X)\" \"縦位置(Y)\"",                                            2,
			"#size",         "フォームサイズ          #size \"600\" \"600\" ※\"幅(Width)\" \"高さ(Height)\"",                                       2,
			"#sizeMax",      "フォームサイズ（最大化）",                                                                                             0,
			"#sizeMin",      "フォームサイズ（最小化）",                                                                                             0,
			"#sizeNormal",   "フォームサイズ（普通）",                                                                                               0,
			"#macroList",    "マクロ一覧",                                                                                                           0,
			"#help",         "操作説明",                                                                                                             0,
			"#version",      "バージョン",                                                                                                           0,
			"#exit",         "終了",                                                                                                                 0
		};

		//--------------------------------------------------------------------------------
		// 大域変数
		//--------------------------------------------------------------------------------
		// エラーが発生したとき
		private bool ExecStopOn = false;

		// 出力文字コード／初期値 = CP932(Shift_JIS)
		private int OutputEncode = 932;

		// BaseDir
		private string BaseDir = "";

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
			"         ↑引数はダブルクォーテーションで囲む。" + NL +
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
			"(例) #cls; dir; #grep \"^20\"; #replace \"^20(\\d+)\" \"'$1\"" + NL +
			"         ↑セミコロンで区切る。" + NL +
			NL +
			"-------------------------------------" + NL +
			"> マクロ・コマンド入力 特殊キー操作 <" + NL +
			"-------------------------------------" + NL +
			"[↑]                 実行履歴（過去）" + NL +
			"[↓]                 実行履歴（最近）" + NL +
			NL +
			"[Ctrl]+[↑]          コンテキストメニュー表示" + NL +
			"[Ctrl]+[↓]" + NL +
			NL +
			"[Ctrl]+[Space]       クリア" + NL +
			"[Ctrl]+[Backspace]   カーソルより前方をクリア" + NL +
			"[Ctrl]+[Delete]      カーソルより後方をクリア" + NL +
			"[Ctrl]+[Z]           直前に戻す" + NL +
			NL +
			"[PgUp]               カーソルを空白位置へ移動（前へ）" + NL +
			"[PgDn]               カーソルを空白位置へ移動（次へ）" + NL +
			NL +
			"[F1]  マクロ・コマンド入力履歴" + NL +
			"[F2]  マクロ選択" + NL +
			"[F3]  コマンド選択" + NL +
			"[F4]  (割り当てなし)" + NL +
			"[F5]  実行" + NL +
			"[F6]  出力を実行前に戻す" + NL +
			"[F7]  出力をクリア" + NL +
			"[F8]  出力履歴" + NL +
			"[F9]  マクロ・コマンド入力 ～ 出力 間をフォーカス移動" + NL +
			"[F10] (システムメニュー)" + NL +
			"[F11] 出力変更（前へ）" + NL +
			"[F12] 出力変更（次へ）" + NL +
			NL +
			"-------------------------------------" + NL +
			"> マクロ・コマンド検索 特殊キー操作 <" + NL +
			"-------------------------------------" + NL +
			"[Enter]              検索開始" + NL +
			"[Ctrl]+[Space]       クリア" + NL +
			"[Ctrl]+[Backspace]   カーソルより前方をクリア" + NL +
			"[Ctrl]+[Delete]      カーソルより後方をクリア" + NL +
			"[↓]                 マクロ・コマンド選択へ移動" + NL +
			NL +
			"-------------------------------------" + NL +
			"> マクロ・コマンド選択 特殊キー操作 <" + NL +
			"-------------------------------------" + NL +
			"[↑](上端で操作)     マクロ・コマンド検索へ移動" + NL +
			"[Ctrl]+[PgUp]        カーソルを最上部へ移動" + NL +
			"[Ctrl]+[PgDn]        カーソルを最下部へ移動" + NL +
			NL +
			"---------------------" + NL +
			"> 出力 特殊キー操作 <" + NL +
			"---------------------" + NL +
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
			"  #{msec} ミリ秒     (例) 999" + NL +
			"  #{y}    年         (例) 2021" + NL +
			"  #{m}    月         (例) 05" + NL +
			"  #{d}    日         (例) 01" + NL +
			"  #{h}    時         (例) 11" + NL +
			"  #{n}    分         (例) 34" + NL +
			"  #{s}    秒         (例) 00" + NL +
			NL +
			"  #{環境変数}        (例) #{OS} => Windows_NT" + NL +
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
			"◇ファイル記述例" + NL +
			"  // 単一行コメント" + NL +
			"  /*" + NL +
			"     複数行コメント" + NL +
			"  */" + NL +
			"  #cls" + NL +
			"  dir" + NL +
			"  #grep \"^20\"" + NL +
			"  #replace \"^20(\\d+)\" \"'$1\"" + NL
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
			TbCmd.Text = "#help";

			// RtbCmdLog
			GblRtbCmdLogHeightDefault = RtbCmdLog.Height;

			// DgvMacro
			for (int _i1 = 0; _i1 < AryDgvMacro.Length; _i1 += 3)
			{
				_ = DgvMacro.Rows.Add(AryDgvMacro[_i1], AryDgvMacro[_i1 + 1]);
			}

			// DgvCmd
			SubDgvCmdLoad();

			// TbResult
			SubTbResultChange(0, true);

			// ScrTbResult
			ScrTbResult.Visible = false;

			// TbInfo
			TbInfo.Text = "";

			// フォントサイズ
			NudFontSize.Value = (int)Math.Round(TbResult.Font.Size);

			// 設定ファイルが存在するとき
			if (File.Exists(ConfigFn))
			{
				(_, string _data) = RtnTextFread(ConfigFn, false, "");
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
					(string _fn, string _data) = RtnTextFread(_s1, false, "");
					if (_fn.Length > 0)
					{
						TbCmd.Paste(Regex.Replace(_data, RgxCmdNL, ";"));
					}
				}
				BtnCmdExec_Click(sender, e);
				TbCmd.Text = "";
			}

			// 開始時フォーカス
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

			// RtbCmdLog 開かれているときだけ調整
			SubRtbCmdLogResize(RtbCmdLog.Height > GblRtbCmdLogHeightDefault);
		}

		private void SubFormStartPosition()
		{
			Location = new Point(Cursor.Position.X - (Width / 2), Cursor.Position.Y - (SystemInformation.CaptionHeight / 2));
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
			ToolTip.SetToolTip(TbCurDir, TbCurDir.Text.Replace("\\", NL));
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
		// ChkTopMost
		//--------------------------------------------------------------------------------
		private void ChkTopMost_Click(object sender, EventArgs e)
		{
			TopMost = ChkTopMost.Checked;
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
			Lbl_F1.ForeColor = Lbl_F2.ForeColor = Lbl_F3.ForeColor = Lbl_F5.ForeColor = Lbl_F6.ForeColor = Lbl_F7.ForeColor = Lbl_F8.ForeColor = Color.Gray;
			LblCmd.Visible = false;
			GblTbCmdPos = TbCmd.SelectionStart;
		}

		private void TbCmd_TextChanged(object sender, EventArgs e)
		{
			// IME確定 [Enter] で本イベントは発生しない(＝改行されない)ので "\n" の有無で入力モードの判定可能
			if (TbCmd.Text.IndexOf("\n") >= 0)
			{
				TbCmd.Text = Regex.Replace(TbCmd.Text, RgxNL, "");
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

			// [Ctrl]+[S]
			if (e.KeyData == (Keys.Control | Keys.S))
			{
				Cursor.Position = new Point(Left + ((Width - CmsCmd.Width) / 2), Top + SystemInformation.CaptionHeight + TbCmd.Bottom - 20);
				CmsCmd.Show(Cursor.Position);
				CmsCmd_コマンドを保存.Select();
				return;
			}

			// [Ctrl]+[Delete]
			if (e.KeyData == (Keys.Control | Keys.Delete))
			{
				TbCmd.Select(TbCmd.SelectionStart, TbCmd.TextLength);
				TbCmd.Cut();
				// 文字位置を再設定しないと SendMessage で不具合
				TbCmd.SelectionStart = TbCmd.TextLength;
				return;
			}

			// [Ctrl]+[Backspace]
			if (e.KeyData == (Keys.Control | Keys.Back))
			{
				TbCmd.Select(0, TbCmd.SelectionStart);
				TbCmd.Cut();
				// 文字位置を再設定しないと SendMessage で不具合
				TbCmd.SelectionStart = 0;
				return;
			}

			// [Ctrl]+[↑]
			// [Ctrl]+[↓]
			if (e.KeyData == (Keys.Control | Keys.Up) || e.KeyData == (Keys.Control | Keys.Down))
			{
				Cursor.Position = new Point(Left + ((Width - CmsCmd.Width) / 2), Top + SystemInformation.CaptionHeight + TbCmd.Bottom - 20);
				CmsCmd.Show(Cursor.Position);
				CmsCmd_マクロ変数.Select();
				return;
			}

			// [Ctrl]+[Space]
			if (e.KeyData == (Keys.Control | Keys.Space))
			{
				TbCmd.SelectAll();
				TbCmd.Cut();
				// 文字位置を再設定しないと SendMessage で不具合
				TbCmd.SelectionStart = 0;
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
			MatchCollection MC;
			int iPos;

			switch (e.KeyCode)
			{
				case Keys.Escape:
					SubRtbCmdLogResize(false);
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
					_ = TbResult.Focus();
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
					// 実行は KeyPress
					// ここでは何もしない
					break;

				case Keys.PageUp:
					MC = Regex.Matches(TbCmd.Text.Substring(0, TbCmd.SelectionStart), @"\S+\s*$");
					TbCmd.SelectionStart = MC.Count > 0 ? MC[0].Index : 0;
					break;

				case Keys.PageDown:
					iPos = TbCmd.SelectionStart;
					MC = Regex.Matches(TbCmd.Text.Substring(iPos), @"\s\S+");
					TbCmd.SelectionStart = MC.Count > 0 ? iPos + 1 + MC[0].Index : TbCmd.TextLength;
					break;

				case Keys.Up:
					SubTbCmdHistory_Get(true);
					break;

				case Keys.Down:
					SubTbCmdHistory_Get(false);
					break;
			}

			// 補完(*)
			if (TbCmd.TextLength == TbCmd.SelectionStart && e.KeyData != Keys.Delete && e.KeyData != Keys.Back)
			{
				// (例) "#grep "（末尾は半角スペース）
				// "#"以降は最短一致
				Regex rgx = new Regex(".*(?<macro>#\\S+?)\\s+$", RegexOptions.IgnoreCase);
				Match match = rgx.Match(TbCmd.Text);
				if (match.Success)
				{
					string macro = match.Groups["macro"].Value;
					string sSpace = "";
					int iSpace = 0;

					// 引数分の "" 付与
					for (int _i1 = 0; _i1 < AryDgvMacro.Length; _i1 += 3)
					{
						if (AryDgvMacro[_i1].ToString().ToLower() == macro.ToLower() && (int)AryDgvMacro[_i1 + 2] > 0)
						{
							iSpace = (int)AryDgvMacro[_i1 + 2];
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

			GblTbCmdPos = TbCmd.SelectionStart;
		}

		private void TbCmd_MouseEnter(object sender, EventArgs e)
		{
			///ToolTip.SetToolTip(TbCmd, RtnCmdFormat(TbCmd.Text));
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
				s1 += Directory.Exists(_s1) ? $"\"{_s1.TrimEnd('\\')}\\\" " : $"\"{_s1}\" ";
			}
			// 末尾の空白をクリア
			TbCmd.Paste(s1.TrimEnd());
			SubTbCmdDQFormat();
		}

		private void SubTbCmdDQFormat()
		{
			// 余計な " をクリア
			string s1 = TbCmd.Text;
			s1 = Regex.Replace(s1, "\"{2,}(\\S+)", "\"$1");
			s1 = Regex.Replace(s1, "(\\S+)\"{2,}", "$1\"");
			TbCmd.Text = s1.TrimEnd();
			SubTbCmdFocus(-1);
		}

		//--------------------------------------------------------------------------------
		// CmsCmd
		//--------------------------------------------------------------------------------
		private string GblCmsCmdBatch = "";

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

		private void CmsCmd_マクロ変数_Click(object sender, EventArgs e)
		{
			Cursor.Position = new Point(Left + ((Width - CmsCmd.Width) / 2), Top + SystemInformation.CaptionHeight + TbCmd.Bottom - 20);
			CmsCmd2.Show(Cursor.Position);
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
				// 末尾の空白をクリア
				TbCmd.Paste($"\"{fbd.SelectedPath.TrimEnd('\\')}\\\"");
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
					s1 += $"\"{_s1}\" ";
				}
				// 末尾の空白をクリア
				TbCmd.Paste(s1.TrimEnd());
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

		private string GblCmsCmdFn = "";

		private void CmsCmd_コマンドを保存_Click(object sender, EventArgs e)
		{
			string fn = (GblCmsCmdFn.Length == 0 ? "" : Path.GetDirectoryName(GblCmsCmdFn) + "\\") + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".iwmcmd";
			if (RtnTextFileWrite(RtnCmdFormat(TbCmd.Text).Trim().TrimEnd(';') + ";" + NL, 932, fn, true, CMD_FILTER))
			{
				GblCmsCmdFn = fn;
			}
		}

		private void CmsCmd_コマンドを読込_Click(object sender, EventArgs e)
		{
			CmsCmd.Close();

			(string fn, string data) = RtnTextFread(GblCmsCmdFn, true, CMD_FILTER);
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
				(_, string data) = RtnTextFread(GblCmsCmdFn, false, "");
				TbCmd.Text = Regex.Replace(data, RgxCmdNL, "; ");
				SubTbCmdFocus(-1);
			}
			else
			{
				CmsCmd_コマンドを読込_Click(sender, e);
			}
		}

		private void CmsCmd2_Opened(object sender, EventArgs e)
		{
			_ = CmsCmd2.Focus();
			CmsCmd2_一時変数.Enabled = false;

			// #{%[キー]} のメニュー作成
			if (DictHash.Count > 0)
			{
				CmsCmd2_一時変数.DropDownItems.Clear();

				foreach (KeyValuePair<string, string> _kv1 in DictHash)
				{
					ToolStripMenuItem _tsi = new ToolStripMenuItem
					{
						Text = "#{%" + _kv1.Key + "} " + _kv1.Value
					};
					_tsi.Click += CmsCmd2_一時変数_SubMenuClick;
					_ = CmsCmd2_一時変数.DropDownItems.Add(_tsi);
				}

				CmsCmd2_一時変数.Enabled = true;
			}

			CmsCmd2_閉じる.Select();
		}

		private void CmsCmd2_一時変数_SubMenuClick(object sender, EventArgs e)
		{
			ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
			TbCmd.Paste(Regex.Replace(tsmi.Text, @"^(#\{%.+?\}).+", "$1"));
			_ = CmsCmd2.Focus();
			CmsCmd2_一時変数.Select();
		}

		private bool GblCmsCmd2CancelOn = true;

		private void CmsCmd2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			GblCmsCmd2CancelOn = true;
		}

		private void CmsCmd2_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			e.Cancel = GblCmsCmd2CancelOn;
		}

		private void CmsCmd2_閉じる_Click(object sender, EventArgs e)
		{
			GblCmsCmd2CancelOn = false;
			CmsCmd2.Close();
		}

		private void CmsCmd2_タブ_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{tab}");
		}

		private void CmsCmd2_改行_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{nl}");
		}

		private void CmsCmd2_ダブルクォーテーション_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{dq}");
		}

		private void CmsCmd2_セミコロン_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{sc}");
		}

		private void CmsCmd2_現時間_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{now}");
		}

		private void CmsCmd2_日付_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{ymd}");
		}

		private void CmsCmd2_年_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{y}");
		}

		private void CmsCmd2_月_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{m}");
		}

		private void CmsCmd2_日_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{d}");
		}

		private void CmsCmd2_時間_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{hns}");
		}

		private void CmsCmd2_時_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{h}");
		}

		private void CmsCmd2_分_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{n}");
		}

		private void CmsCmd2_秒_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{s}");
		}

		private void CmsCmd2_ミリ秒_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{msec}");
		}

		private void CmsCmd2_出力の行データ_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{}");
		}

		private void CmsCmd2_出力の行番号_Click(object sender, EventArgs e)
		{
			TbCmd.Paste("#{line,,}");
		}

		//--------------------------------------------------------------------------------
		// RtbCmdLog
		//--------------------------------------------------------------------------------
		private void RtbCmdLog_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					SubRtbCmdLogResize(false);
					SubTbCmdFocus(GblTbCmdPos);
					break;

				case Keys.F9:
					_ = TbResult.Focus();
					break;
			}
		}

		private void RtbCmdLog_Enter(object sender, EventArgs e)
		{
			LblCmdLog.Visible = true;
		}

		private void RtbCmdLog_Leave(object sender, EventArgs e)
		{
			LblCmdLog.Visible = false;
			SubRtbCmdLogResize(false);
			RtbCmdLog.SelectionStart = RtbCmdLog.TextLength;
			RtbCmdLog.ScrollToCaret();
		}

		private void RtbCmdLog_MouseUp(object sender, MouseEventArgs e)
		{
			CmsTextSelect_Open(e, RtbCmdLog);
		}

		//--------------------------------------------------------------------------------
		// CmsCmdLog
		//--------------------------------------------------------------------------------
		private void CmsCmdLog_Opened(object sender, EventArgs e)
		{
			_ = RtbCmdLog.Focus();
		}

		private void CmsCmdLog_クリア_Click(object sender, EventArgs e)
		{
			RtbCmdLog.Text = "";
		}

		private void CmsCmdLog_全コピー_Click(object sender, EventArgs e)
		{
			RtbCmdLog.SelectAll();
			RtbCmdLog.Copy();
		}

		private void CmsCmdLog_上書き_Click(object sender, EventArgs e)
		{
			RtbCmdLog.Text = "";
			CmsCmdLog_貼り付け_Click(sender, e);
		}

		private void CmsCmdLog_貼り付け_Click(object sender, EventArgs e)
		{
			RtbCmdLog.Paste();
		}

		private void CmsCmdLog_拡大_Click(object sender, EventArgs e)
		{
			SubRtbCmdLogResize(true);
		}

		private void CmsCmdLog_元に戻す_Click(object sender, EventArgs e)
		{
			SubRtbCmdLogResize(false);
		}

		private int GblRtbCmdLogHeightDefault;

		private void SubRtbCmdLogResize(bool bSizeMax)
		{
			RtbCmdLog.BringToFront();

			if (bSizeMax)
			{
				RtbCmdLog.Height = CbResultHistory.Bottom - RtbCmdLog.Top;
				RtbCmdLog.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
			}
			else
			{
				RtbCmdLog.Height = GblRtbCmdLogHeightDefault;
				RtbCmdLog.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
			}
		}

		//--------------------------------------------------------------------------------
		// DgvMacro
		//--------------------------------------------------------------------------------
		private int GblDgvMacroRow = 0;
		private bool GblDgvMacroOpen = false; // DgvMacro.enabled より速い

		private void BtnDgvMacro_Click(object sender, EventArgs e)
		{
			DgvMacro.Visible = false;

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

				TbDgvSearch.Visible = false;
				BtnDgvSearch.Visible = false;

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

				TbDgvSearch.Left = DgvMacro.Left + 60;
				BtnDgvSearch.Left = TbDgvSearch.Left + 100;

				TbDgvSearch.BringToFront();
				BtnDgvSearch.BringToFront();

				TbDgvSearch.Visible = true;
				BtnDgvSearch.Visible = true;

				_ = TbDgvSearch.Focus();

				if (DgvMacro.RowCount > 0)
				{
					using (DataGridViewCell _o1 = DgvMacro[0, 0])
					{
						_o1.Selected = true;
						_o1.Style.SelectionBackColor = DgvMacro.DefaultCellStyle.BackColor;
						_o1.Style.SelectionForeColor = DgvMacro.DefaultCellStyle.ForeColor;
					}
				}
			}

			DgvMacro.Visible = true;
		}

		private void DgvMacro_Enter(object sender, EventArgs e)
		{
			Lbl_F2.ForeColor = Color.Gold;
		}

		private void DgvMacro_Leave(object sender, EventArgs e)
		{
			Lbl_F2.ForeColor = Color.Gray;
		}

		private void DgvMacro_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			DgvCellColor(sender, e, Color.Empty, Color.Empty);
		}

		private void DgvMacro_CellLeave(object sender, DataGridViewCellEventArgs e)
		{
			DgvCellColor(sender, e, DgvMacro.DefaultCellStyle.BackColor, DgvMacro.DefaultCellStyle.ForeColor);
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

			for (int _i1 = 0; _i1 < AryDgvMacro.Length; _i1 += 3)
			{
				if (s1 == AryDgvMacro[_i1].ToString())
				{
					int _i2 = (int)AryDgvMacro[_i1 + 2];
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
				// TbCmd.SelectionStart 取得が先
				GblTbCmdPos = TbCmd.SelectionStart + s1.Length - iPosForward;
				TbCmd.Paste(s1 + ";");
			}
			else
			{
				GblTbCmdPos = s1.Length - iPosForward;
				TbCmd.Text = s1;
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
			// [Ctrl]+[PgUp]
			if (e.KeyData == (Keys.Control | Keys.PageUp))
			{
				DgvMacro.CurrentCell = DgvMacro[0, 0];
				return;
			}

			// [Ctrl]+[PgDn]
			if (e.KeyData == (Keys.Control | Keys.PageDown))
			{
				DgvMacro.CurrentCell = DgvMacro[0, DgvMacro.RowCount - 1];
				return;
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

				case Keys.Up:
					if (GblDgvMacroRow == 0)
					{
						_ = TbDgvSearch.Focus();
					}
					break;

				case Keys.Right:
					DgvMacro.CurrentCell = DgvMacro[0, DgvMacro.CurrentRow.Index];
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
			DgvCmd.Visible = false;

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

				TbDgvSearch.Visible = false;
				BtnDgvSearch.Visible = false;

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

				TbDgvSearch.Left = DgvCmd.Left + 70;
				BtnDgvSearch.Left = TbDgvSearch.Left + 100;

				TbDgvSearch.BringToFront();
				BtnDgvSearch.BringToFront();

				TbDgvSearch.Visible = true;
				BtnDgvSearch.Visible = true;

				_ = TbDgvSearch.Focus();

				if (DgvCmd.RowCount > 0)
				{
					using (DataGridViewCell _o1 = DgvCmd[0, 0])
					{
						_o1.Selected = true;
						_o1.Style.SelectionBackColor = DgvCmd.DefaultCellStyle.BackColor;
						_o1.Style.SelectionForeColor = DgvCmd.DefaultCellStyle.ForeColor;
					}
				}
			}

			DgvCmd.Visible = true;
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
				// TbCmd.SelectionStart 取得が先
				GblTbCmdPos = TbCmd.SelectionStart + s1.Length + 1;
				TbCmd.Paste(s1 + ";");
			}
			else
			{
				GblTbCmdPos = s1.Length;
				TbCmd.Text = s1;
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
			// [Ctrl]+[PgUp]
			if (e.KeyData == (Keys.Control | Keys.PageUp))
			{
				DgvCmd.CurrentCell = DgvCmd[0, 0];
				return;
			}

			// [Ctrl]+[PgDn]
			if (e.KeyData == (Keys.Control | Keys.PageDown))
			{
				DgvCmd.CurrentCell = DgvCmd[0, DgvCmd.RowCount - 1];
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
					DgvCmd.CurrentCell = DgvCmd[0, DgvCmd.CurrentCell.RowIndex - (DgvCmd.CurrentRow.Index == GblDgvCmdRow ? 0 : 1)];
					GblTbCmdPos = DgvCmd.CurrentCell.Value.ToString().Length;
					DgvCmd_CellMouseClick(sender, null);
					break;

				case Keys.Space:
					GblTbCmdPos = DgvCmd.CurrentCell.Value.ToString().Length;
					DgvCmd_CellMouseClick(sender, null);
					break;

				case Keys.Up:
					if (GblDgvCmdRow == 0)
					{
						_ = TbDgvSearch.Focus();
					}
					break;
			}
		}

		private string[] GblAryDgvCmd;

		private void SubDgvCmdLoad()
		{
			BtnDgvSearch.Visible = false;
			TbDgvSearch.Visible = false;

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
			GblAryDgvCmd = lst1.Distinct().ToArray();
			lst1.Clear();

			foreach (string _s1 in GblAryDgvCmd)
			{
				_ = DgvCmd.Rows.Add(_s1);
			}
		}

		//--------------------------------------------------------------------------------
		// TbDgvSearch
		//--------------------------------------------------------------------------------
		// 0=DgvMacro／1=DgvCmd
		private readonly string[] GblAryTbDgvSearch = { "", "" };

		private void TbDgvSearch_Enter(object sender, EventArgs e)
		{
			TbDgvSearch.BackColor = Color.LightYellow;
			Lbl_F2.ForeColor = Lbl_F3.ForeColor = Color.Gray;
			if (GblDgvMacroOpen)
			{
				Lbl_F2.ForeColor = Color.Gold;
				TbDgvSearch.Text = GblAryTbDgvSearch[0];
			}
			else
			{
				Lbl_F3.ForeColor = Color.Gold;
				TbDgvSearch.Text = GblAryTbDgvSearch[1];
			}
			TbDgvSearch.SelectionStart = TbDgvSearch.TextLength;
		}

		private void TbDgvSearch_Leave(object sender, EventArgs e)
		{
			TbDgvSearch.BackColor = Color.Azure;
			Lbl_F2.ForeColor = Lbl_F3.ForeColor = Color.Gray;
		}

		private void TbDgvSearch_TextChanged(object sender, EventArgs e)
		{
			if (GblDgvMacroOpen)
			{
				GblAryTbDgvSearch[0] = TbDgvSearch.Text;
			}
			else
			{
				GblAryTbDgvSearch[1] = TbDgvSearch.Text;
			}
		}

		private void TbDgvSearch_KeyPress(object sender, KeyPressEventArgs e)
		{
			// ビープ音抑制
			if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)
			{
				e.Handled = true;
			}
		}

		private void TbDgvSearch_KeyUp(object sender, KeyEventArgs e)
		{
			// [Ctrl]+[Space]
			if (e.KeyData == (Keys.Control | Keys.Space))
			{
				TbDgvSearch.Text = "";
				// 文字位置を再設定しないと SendMessage で不具合
				TbDgvSearch.SelectionStart = 0;
				return;
			}

			// [Ctrl]+[Backspace]
			if (e.KeyData == (Keys.Control | Keys.Back))
			{
				TbDgvSearch.Text = TbDgvSearch.Text.Substring(TbDgvSearch.SelectionStart);
				// 文字位置を再設定しないと SendMessage で不具合
				TbDgvSearch.SelectionStart = 0;
				return;
			}

			// [Ctrl]+[Delete]
			if (e.KeyData == (Keys.Control | Keys.Delete))
			{
				TbDgvSearch.Text = TbDgvSearch.Text.Substring(0, TbDgvSearch.SelectionStart);
				// 文字位置を再設定しないと SendMessage で不具合
				TbDgvSearch.SelectionStart = TbDgvSearch.TextLength;
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
					BtnDgvSearch_Click(sender, e);
					break;

				case Keys.PageUp:
					TbDgvSearch.SelectionStart = 0;
					break;

				case Keys.PageDown:
					TbDgvSearch.SelectionStart = TbDgvSearch.TextLength;
					break;

				case Keys.Up:
					break;

				case Keys.Down:
					if (GblDgvMacroOpen)
					{
						_ = DgvMacro.Focus();
					}
					else
					{
						_ = DgvCmd.Focus();
					}
					break;
			}
		}

		private void BtnDgvSearch_Click(object sender, EventArgs e)
		{
			if (GblDgvMacroOpen)
			{
				DgvMacro.Enabled = false;
				DgvMacro.Rows.Clear();
				for (int _i1 = 0; _i1 < AryDgvMacro.Length; _i1 += 3)
				{
					if (AryDgvMacro[_i1].ToString().IndexOf(TbDgvSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0 || AryDgvMacro[_i1 + 1].ToString().IndexOf(TbDgvSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
					{
						_ = DgvMacro.Rows.Add(AryDgvMacro[_i1], AryDgvMacro[_i1 + 1]);
					}
				}
				if (DgvMacro.RowCount > 0)
				{
					using (DataGridViewCell _o1 = DgvMacro[0, 0])
					{
						_o1.Style.SelectionBackColor = DgvMacro.DefaultCellStyle.BackColor;
						_o1.Style.SelectionForeColor = DgvMacro.DefaultCellStyle.ForeColor;
					}
				}
				DgvMacro.Enabled = true;
			}
			else
			{
				DgvCmd.Enabled = false;
				DgvCmd.Rows.Clear();
				foreach (string _s1 in GblAryDgvCmd)
				{
					if (_s1.IndexOf(TbDgvSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
					{
						_ = DgvCmd.Rows.Add(_s1);
					}
				}
				if (DgvCmd.RowCount > 0)
				{
					using (DataGridViewCell _o1 = DgvCmd[0, 0])
					{
						_o1.Style.SelectionBackColor = DgvCmd.DefaultCellStyle.BackColor;
						_o1.Style.SelectionForeColor = DgvCmd.DefaultCellStyle.ForeColor;
					}
				}
				DgvCmd.Enabled = true;
			}
			_ = TbDgvSearch.Focus();
		}

		//--------------------------------------------------------------------------------
		// CmsTbDgvSearch
		//--------------------------------------------------------------------------------
		private void CmsTbDgvSearch_Opened(object sender, EventArgs e)
		{
			_ = TbDgvSearch.Focus();
		}

		private void CmsTbDgvSearch_クリア_Click(object sender, EventArgs e)
		{
			TbDgvSearch.Text = "";
		}

		private void CmsTbDgvSearch_貼り付け_Click(object sender, EventArgs e)
		{
			TbDgvSearch.Paste();
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
		// 設定ファイル中にコメント /* ～ */ があるとき
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
					RtbCmdLog.AppendText(_s1.TrimEnd(';') + NL);
					RtbCmdLog.ScrollToCaret();
					SubTbCmdExec(_s1);
				}
			}

			// マクロ・コマンド履歴に追加
			GblListCmdHistory.Add(TbCmd.Text.Trim());

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

			SubTbCmdHistory_Set(TbCmd.Text);
			TbCmd.Text = "";
			GblTbCmdPos = 0;

			// 計測終了
			sw.Stop();
			SubCmdLogAddRem($"{sw.ElapsedMilliseconds / 1000.0:F3}秒", Color.White);
		}

		// 実行履歴を [↑][↓] で呼び出す
		private readonly List<string> GblLstTbCmdHistory = new List<string>();
		private int GblLstTbCmdHistoryPos = 0;

		private void SubTbCmdHistory_Set(string s1)
		{
			GblLstTbCmdHistory.Add(s1);
			GblLstTbCmdHistoryPos = GblLstTbCmdHistory.Count;
		}

		private void SubTbCmdHistory_Get(bool bKeyUp)
		{
			if (bKeyUp)
			{
				if (GblLstTbCmdHistoryPos > 0)
				{
					--GblLstTbCmdHistoryPos;
					TbCmd.Text = GblLstTbCmdHistory[GblLstTbCmdHistoryPos];
					TbCmd.SelectionStart = TbCmd.TextLength;
				}
				else
				{
					TbCmd.Text = "";
				}
			}
			else
			{
				if (GblLstTbCmdHistoryPos < GblLstTbCmdHistory.Count)
				{
					TbCmd.Text = GblLstTbCmdHistory[GblLstTbCmdHistoryPos];
					++GblLstTbCmdHistoryPos;
					TbCmd.SelectionStart = TbCmd.TextLength;
				}
				else
				{
					TbCmd.Text = "";
				}
			}
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
				string[] aOp = new string[256];

				for (int _i1 = aOp.Length - 1; _i1 >= 0; _i1--)
				{
					aOp[_i1] = "";
				}

				// aOp[0] 取得
				rgx = new Regex("^(?<pattern>\\S+?)\\s");
				match = rgx.Match(cmd);
				aOp[0] = match.Groups["pattern"].Value;

				// aOp[1..n] 取得
				//   , 区切りも容認する
				rgx = new Regex("\"(?<pattern>.*?)\"[\\s,]");
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
					// コマンド実行時の出力文字コード Shift_JIS
					case "#932":
						OutputEncode = 932;
						break;

					// コマンド実行時の出力文字コード UTF-8
					case "#65001":
						OutputEncode = 65001;
						break;

					// クリア
					case "#cls":
						TbResult.Text = "";
						break;

					// 全クリア
					case "#clear":
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
								SubCmdLogAddRem($"新規フォルダ '{_sFullPath}' を作成しました。", Color.Cyan);
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

					// 抽出
					case "#extract":
						TbResult.Text = RtnTextExtract(TbResult.Text, aOp[1]);
						break;

					// 置換
					case "#replace":
						TbResult.Text = RtnTextReplace(TbResult.Text, aOp[1], aOp[2]);
						break;

					// 分割変換
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
						TbResult.Paste(RtnColumnJoin(aOp[1], aOp[2]));
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
								SubCmdLogAddRem(aOp[1], Color.Cyan);
								SubCmdLogAddRem(ex.Message, Color.Red);
								_ = MessageBox.Show(
									"[Err] " + ex.Message,
									ProgramID
								);
							}
						}
						break;

					// テキストファイル読込
					case "#fread":
						(s1, s2) = RtnTextFread(aOp[1], false, "");
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
						TbResult.AppendText(RtnDirFileList(aOp[1], i1, true) + NL);
						TbResult.ScrollToCaret();
						break;

					// テキストファイル検索
					case "#grepfile":
						if (aOp[1].Length == 0)
						{
							break;
						}
						aOp[1] = RtnCnvMacroVar(aOp[1], 0, "");
						if (aOp[2].Length == 0)
						{
							aOp[2] = ".";
						}
						_ = sb.Clear();
						i1 = 0;
						for (int _i1 = 2, _i2 = aOp.Length; _i1 < _i2; _i1++)
						{
							if (aOp[_i1].Length > 0)
							{
								if (Directory.Exists(aOp[_i1]))
								{
									foreach (string _s1 in Regex.Split(RtnDirFileList(aOp[_i1], 2, false), NL))
									{
										if (RtnIsTextFile(_s1))
										{
											string _s2 = File.ReadAllText(_s1, Encoding.GetEncoding(RtnIsFileEncCp65001(_s1) ? 65001 : 932));
											i2 = 0;
											foreach (string _s3 in Regex.Split(_s2, RgxNL))
											{
												++i2;
												// 正規表現文法エラーはないか？
												try
												{
													if (Regex.IsMatch(_s3, aOp[1], RegexOptions.IgnoreCase))
													{
														_ = sb.Append(_s1);
														_ = sb.Append("\tL");
														_ = sb.Append(i2.ToString());
														_ = sb.Append("\t");
														_ = sb.Append(_s3.Trim());
														_ = sb.Append(NL);
														++i1;
													}
												}
												catch
												{
													break;
												}
											}
										}
									}
								}
								else if (RtnIsTextFile(aOp[_i1]))
								{
									string _s2 = File.ReadAllText(aOp[_i1], Encoding.GetEncoding(RtnIsFileEncCp65001(aOp[_i1]) ? 65001 : 932));
									i2 = 0;
									foreach (string _s3 in Regex.Split(_s2, RgxNL))
									{
										++i2;
										// 正規表現文法エラーはないか？
										try
										{
											if (Regex.IsMatch(_s3, aOp[1], RegexOptions.IgnoreCase))
											{
												_ = sb.Append(aOp[_i1]);
												_ = sb.Append("\tL");
												_ = sb.Append(i2.ToString());
												_ = sb.Append("\t");
												_ = sb.Append(_s3.Trim());
												_ = sb.Append(NL);
												++i1;
											}
										}
										catch
										{
											break;
										}
									}
								}
							}
						}
						TbResult.AppendText(sb.ToString());
						TbResult.ScrollToCaret();
						SubCmdLogAddRem($"{i1}行 該当", Color.Cyan);
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

						sb.Clear();

						BtnCmdExecStream.Visible = true;

						int iStreamBgn = RtbCmdLog.TextLength;
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
									// Stdout
									string _s3 = Regex.Replace(PS.StandardOutput.ReadToEnd(), RgxNL, NL).TrimEnd() + NL;

									// ログ
									RtbCmdLog.AppendText(_s3);
									_ = sb.Append(_s3);

									// RtbCmdLog の着色・スクロール
									int iStreamEnd = RtbCmdLog.TextLength;
									RtbCmdLog.SelectionStart = iStreamBgn;
									RtbCmdLog.SelectionLength = iStreamEnd - iStreamBgn;
									RtbCmdLog.SelectionColor = Color.Lime;
									RtbCmdLog.SelectionStart = iStreamEnd;
									RtbCmdLog.ScrollToCaret();
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

						// 出力[n]
						if (i1 >= 0)
						{
							GblAryResultBuf[i1] = sb.ToString();
							GblAryResultStartIndex[i1] = GblAryResultBuf[i1].Length;
							if (GblAryResultStartIndex[i1] > 0)
							{
								Controls[GblAryResultBtn[i1]].BackColor = Color.Maroon;
							}
						}

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
										SubCmdLogAddRem(_s10, Color.Cyan);
										SubCmdLogAddRem(ex.Message, Color.Red);
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
							"※大文字・小文字を区別しない。 (例) #clear と #CLEAR は同じ。" + NL +
							NL +
							string.Format("{0,-12}{1}", "[マクロ]", "[説明]")
						);
						for (int _i1 = 0; _i1 < AryDgvMacro.Length; _i1 += 3)
						{
							_ = sb.Append(NL);
							_ = sb.Append(string.Format(" {0,-15}{1}", AryDgvMacro[_i1], AryDgvMacro[_i1 + 1]));
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
				PS.StartInfo.StandardOutputEncoding = PS.StartInfo.StandardErrorEncoding = Encoding.GetEncoding(OutputEncode);

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

			TbResult.ScrollToCaret();

			SubLblWaitOn(false);
		}

		private void SubCmdLogAddRem(string str, Color color)
		{
			int iBgn = RtbCmdLog.TextLength;
			RtbCmdLog.AppendText(str + NL);
			int iEnd = RtbCmdLog.TextLength;
			RtbCmdLog.SelectionStart = iBgn;
			RtbCmdLog.SelectionLength = iEnd - iBgn;
			RtbCmdLog.SelectionColor = color;
			RtbCmdLog.SelectionStart = iEnd;
			RtbCmdLog.ScrollToCaret();
		}

		//--------------------------------------------------------------------------------
		// BtnCmdExecUndo
		//--------------------------------------------------------------------------------
		private string GblCmdExecOld = "";
		private string GblCmdExecNew = "";

		private void BtnCmdExecUndo_Click(object sender, EventArgs e)
		{
			TbResult.Text = TbResult.Text == GblCmdExecNew ? GblCmdExecOld : GblCmdExecNew;
			TbResult.SelectionStart = TbResult.TextLength;
			TbResult.ScrollToCaret();
			SubTbCmdFocus(GblTbCmdPos);
		}

		//--------------------------------------------------------------------------------
		// 出力クリア
		//--------------------------------------------------------------------------------
		private void BtnClear_Click(object sender, EventArgs e)
		{
			TbResult.Text = "";
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
			SubRtbCmdLogResize(false);
		}

		private void TbResult_Leave(object sender, EventArgs e)
		{
			LblResult.Visible = false;

			if (TbResult.TextLength == 0)
			{
				return;
			}

			bool bDiff = true;

			// 異なるデータのみ追加
			foreach (KeyValuePair<string, string> _dict in GblDictResultHistory)
			{
				if (_dict.Value == TbResult.Text)
				{
					bDiff = false;
					break;
				}
			}

			if (bDiff)
			{
				// Key重複回避のため一応遅延
				Thread.Sleep(10);
				GblDictResultHistory.Add(DateTime.Now.ToString("HH:mm:ss.ff"), TbResult.Text);

				// 履歴は 10 件
				if (GblDictResultHistory.Count > 10)
				{
					_ = GblDictResultHistory.Remove(GblDictResultHistory.First().Key);
				}
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

			// [Ctrl]+[S]
			if (e.KeyData == (Keys.Control | Keys.S))
			{
				Cursor.Position = new Point(Left + ((Width - CmsResult.Width) / 2), Top + SystemInformation.CaptionHeight + TbResult.Top + 20);
				CmsResult.Show(Cursor.Position);
				CmsResult_名前を付けて保存.Select();
				SendKeys.Send("{RIGHT}");
				return;
			}

			// [Ctrl]+[PgUp]
			if (e.KeyData == (Keys.Control | Keys.PageUp))
			{
				TbResult.SelectionStart = 0;
				TbResult.ScrollToCaret();
				return;
			}

			// [Ctrl]+[PgDn]
			if (e.KeyData == (Keys.Control | Keys.PageDown))
			{
				TbResult.SelectionStart = TbResult.TextLength;
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

			ScrTbResult.Visible = true;
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
				if (Directory.Exists(_s1))
				{
					_ = sb.Append(@"\");
				}
				_ = sb.Append(NL);
			}
			TbResult.Paste(sb.ToString());
		}

		private void CmsResult_出力へコピー_1_Click(object sender, EventArgs e)
		{
			SubCmsResultCopyTo(0);
		}

		private void CmsResult_出力へコピー_2_Click(object sender, EventArgs e)
		{
			SubCmsResultCopyTo(1);
		}

		private void CmsResult_出力へコピー_3_Click(object sender, EventArgs e)
		{
			SubCmsResultCopyTo(2);
		}

		private void CmsResult_出力へコピー_4_Click(object sender, EventArgs e)
		{
			SubCmsResultCopyTo(3);
		}

		private void CmsResult_出力へコピー_5_Click(object sender, EventArgs e)
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

		private string GblCmsResultFn = "";

		private void SubCmsResult_名前を付けて保存(int encode)
		{
			string fn = (GblCmsResultFn.Length == 0 ? "" : Path.GetDirectoryName(GblCmsResultFn) + "\\") + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
			if (RtnTextFileWrite(TbResult.Text, encode, fn, true, TEXT_FILTER))
			{
				GblCmsResultFn = fn;
			}
		}

		private void SubTbResultCnt()
		{
			int iSelectRow = 0;
			int iSelectNL = 0;
			int iSelectWord = TbResult.SelectionLength;

			if (iSelectWord > 0)
			{
				// 速度重視
				foreach (string _s1 in TbResult.SelectedText.Split('\n'))
				{
					++iSelectNL;
					// _s1 = str + "\r" なので "\r".Length = 1 だけ考慮すればよい
					if (_s1.Length > 1)
					{
						++iSelectRow;
					}
				}
				iSelectWord -= (iSelectNL - 1) * NL.Length;

				TbInfo.Text = string.Format("(全{0}行／有効{1}行) {2}文字", iSelectNL, iSelectRow, iSelectWord);
				TbInfo.Visible = true;
			}
			else
			{
				TbInfo.Visible = false;
			}
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

		private void ScrTbResult_Panel1_Click(object sender, EventArgs e)
		{
			// 誤操作で表示されたままになったとき使用
			ScrTbResult.Visible = false;
		}

		private void ScrTbResult_Panel1_DragLeave(object sender, EventArgs e)
		{
			ScrTbResult.Visible = false;
		}

		private void ScrTbResult_Panel2_Click(object sender, EventArgs e)
		{
			// 誤操作で表示されたままになったとき使用
			ScrTbResult.Visible = false;
		}

		private void ScrTbResult_Panel2_DragLeave(object sender, EventArgs e)
		{
			ScrTbResult.Visible = false;
		}

		private void BtnPasteFilename_Click(object sender, EventArgs e)
		{
			// 誤操作で表示されたままになったとき使用
			ScrTbResult.Visible = false;
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
				if (Directory.Exists(_s1))
				{
					_ = sb.Append(@"\");
				}
				_ = sb.Append(NL);
			}
			TbResult.Paste(sb.ToString());

			if (sb.Length > 0)
			{
				TbResult_Leave(sender, e);
			}

			ScrTbResult.Visible = false;
		}

		private void BtnPasteTextfile_Click(object sender, EventArgs e)
		{
			// 誤操作で表示されたままになったとき使用
			ScrTbResult.Visible = false;
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
				(string _s2, string _s3) = RtnTextFread(_s1, false, "");
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
			ScrTbResult.Visible = false;

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
		private List<string> GblListCmdHistory = new List<string>() { };

		private void CbCmdHistory_Enter(object sender, EventArgs e)
		{
			Lbl_F1.ForeColor = Color.Gold;

			// GblListCmdHistory から空白と重複を削除
			_ = GblListCmdHistory.RemoveAll(s => s == "");
			GblListCmdHistory.Sort();
			GblListCmdHistory = GblListCmdHistory.Distinct().ToList();

			// CbCmdHistory を再編成
			CbCmdHistory.Items.Clear();
			_ = CbCmdHistory.Items.Add("");

			foreach (string _s1 in GblListCmdHistory)
			{
				_ = CbCmdHistory.Items.Add(_s1);
			}

			CbCmdHistory.SelectedIndex = 0;

			int i1 = 0;

			foreach (string _s1 in GblListCmdHistory)
			{
				++i1;
				if (_s1 == TbCmd.Text)
				{
					CbCmdHistory.SelectedIndex = i1;
					break;
				}
			}
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

		private void CbCmdHistory_KeyUp(object sender, KeyEventArgs e)
		{
			// [Ctrl]+[PgUp]
			if (e.KeyData == (Keys.Control | Keys.PageUp))
			{
				CbCmdHistory.SelectedIndex = 0;
				return;
			}

			// [Ctrl]+[PgDn]
			if (e.KeyData == (Keys.Control | Keys.PageDown))
			{
				CbCmdHistory.SelectedIndex = CbCmdHistory.Items.Count - 1;
				return;
			}

			switch (e.KeyCode)
			{
				case Keys.F1:
				case Keys.Space:
					// 自身を閉じる
					CbCmdHistory.DroppedDown = false;
					break;
			}
		}

		private readonly SortedDictionary<string, string> GblDictResultHistory = new SortedDictionary<string, string>();

		private void CbResultHistory_Enter(object sender, EventArgs e)
		{
			Lbl_F8.ForeColor = Color.Gold;

			// CbResultHistory を再構成
			CbResultHistory.Items.Clear();
			_ = CbResultHistory.Items.Add("");

			foreach (KeyValuePair<string, string> _dict in GblDictResultHistory)
			{
				string _s1 = _dict.Value.Substring(0, _dict.Value.Length < 80 ? _dict.Value.Length : 80).TrimStart();
				_ = CbResultHistory.Items.Add(string.Format("{0}  {1}", _dict.Key, _s1.Replace(NL, "　")));
			}

			CbResultHistory.SelectedIndex = 0;

			int i1 = 0;

			foreach (KeyValuePair<string, string> _dict in GblDictResultHistory)
			{
				++i1;
				if (_dict.Value == TbResult.Text)
				{
					CbResultHistory.SelectedIndex = i1;
					break;
				}
			}
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
				TbResult.Text = GblDictResultHistory[CbResultHistory.Text.Substring(0, 11)];
				// TbResult.SelectionStart = NUM 不可
				TbResult.ScrollToCaret();
			}

			CbResultHistory.Text = "";
			SubTbCmdFocus(GblTbCmdPos);
		}

		private void CbResultHistory_KeyUp(object sender, KeyEventArgs e)
		{
			// [Ctrl]+[PgUp]
			if (e.KeyData == (Keys.Control | Keys.PageUp))
			{
				CbResultHistory.SelectedIndex = 0;
				return;
			}

			// [Ctrl]+[PgDn]
			if (e.KeyData == (Keys.Control | Keys.PageDown))
			{
				CbResultHistory.SelectedIndex = CbResultHistory.Items.Count - 1;
				return;
			}

			switch (e.KeyCode)
			{
				case Keys.F8:
				case Keys.Space:
					// 自身を閉じる
					CbResultHistory.DroppedDown = false;
					break;
			}
		}

		//--------------------------------------------------------------------------------
		// フォントサイズ
		//--------------------------------------------------------------------------------
		private void NudFontSize_ValueChanged(object sender, EventArgs e)
		{
			TbCmd.Font = new Font(TbCmd.Font.Name.ToString(), (float)NudFontSize.Value);
			RtbCmdLog.Font = new Font(RtbCmdLog.Font.Name.ToString(), (float)NudFontSize.Value);
			TbResult.Font = new Font(TbResult.Font.Name.ToString(), (float)NudFontSize.Value);
		}

		private void NudFontSize_KeyUp(object sender, KeyEventArgs e)
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
			LblCurDir.Visible = LblCmdLog.Visible = LblResult.Visible = false;
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

			if (cmd.Contains("#{"))
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
			}

			return cmd;
		}

		//--------------------------------------------------------------------------------
		// 正規表現による検索
		//--------------------------------------------------------------------------------
		private string RtnTextGrep(string str, string sSearch, bool bIgnoreCase)
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
					if (bIgnoreCase == Regex.IsMatch(_s1, sSearch, RegexOptions.IgnoreCase))
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
				SubCmdLogAddRem("文法エラー？", Color.Red);
				return str;
			}
			else
			{
				SubCmdLogAddRem($"{iCnt}行" + (bIgnoreCase ? $" {iWord}個" : "") + $" 該当", Color.Cyan);
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
				SubCmdLogAddRem("文法エラー？", Color.Red);
				return str;
			}
			else
			{
				SubCmdLogAddRem($"{iCnt}行 該当", Color.Cyan);
				return sb.ToString();
			}
		}

		//--------------------------------------------------------------------------------
		// 正規表現による抽出
		//--------------------------------------------------------------------------------
		private string RtnTextExtract(string str, string sSearch)
		{
			// 正規表現文法エラーはないか？
			try
			{
				Regex rgx = new Regex("(?<pattern>" + sSearch.Trim() + ")", RegexOptions.IgnoreCase);
				StringBuilder sb = new StringBuilder();
				foreach (Match _m1 in rgx.Matches(str))
				{
					_ = sb.Append(_m1.Groups["pattern"].Value.Trim());
					_ = sb.Append(NL);
				}
				return sb.ToString();
			}
			catch
			{
				SubCmdLogAddRem("文法エラー？", Color.Red);
				return str;
			}
		}

		//--------------------------------------------------------------------------------
		// 正規表現による置換
		//--------------------------------------------------------------------------------
		private string RtnTextReplace(string str, string sOld, string sNew)
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
				// SNew 本体は変更しない
				string _s2 = RtnCnvMacroVar(sNew, ++iLine, "");

				// 正規表現文法エラーはないか？
				try
				{
					_s2 = Regex.Replace(_s1, sOld, _s2, RegexOptions.IgnoreCase).Replace("\a", "");
				}
				catch
				{
					bErr = true;
					break;
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
				SubCmdLogAddRem("文法エラー？", Color.Red);
				return str;
			}
			else
			{
				SubCmdLogAddRem($"{iCnt}行 該当", Color.Cyan);
				return sb.ToString().TrimEnd() + NL;
			}
		}

		//--------------------------------------------------------------------------------
		// 正規表現によるファイル名置換
		//--------------------------------------------------------------------------------
		private string RtnFnRename(string path, string sOld, string sNew)
		{
			StringBuilder sb = new StringBuilder();
			int iLine = 0;

			foreach (string _s1 in Regex.Split(path, RgxNL))
			{
				// 文頭文末の " を消除
				string _sOldFn = Regex.Replace(_s1, "^\"(.+)\"", "$1");

				if (File.Exists(_sOldFn))
				{
					_sOldFn = Path.GetFullPath(_sOldFn);

					string _dir = Path.GetDirectoryName(_sOldFn) + "\\";
					int _dirLen = _dir.Length;

					string _sNewFn = _dir + Regex.Replace(_sOldFn.Substring(_dirLen), sOld, RtnCnvMacroVar(sNew, ++iLine, ""));

					try
					{
						SubCmdLogAddRem($"{_sOldFn}{NL}=> {_sNewFn}", Color.Cyan);
						File.Move(_sOldFn, _sNewFn);
						_ = sb.Append(_sNewFn);
					}
					catch
					{
						SubCmdLogAddRem("=> [Err] " + (_sOldFn != _sNewFn && File.Exists(_sNewFn) ? "重複するファイル名が存在します。" : "リネームできませんでした。"), Color.Red);
						_ = sb.Append(_sOldFn);
					}

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

			sSplit = RtnCnvMacroVar(sSplit, 0, "");

			StringBuilder sb = new StringBuilder();
			int iLine = 0;
			bool bErr = false;

			// 行末の空白行は対象外
			foreach (string _s1 in Regex.Split(str.TrimEnd(), RgxNL))
			{
				// sReplace 本体は変更しない
				string _sReplace = RtnCnvMacroVar(sReplace, ++iLine, "");

				// 正規表現文法エラーはないか？
				try
				{
					string[] a1 = Regex.Split(_s1, sSplit, RegexOptions.IgnoreCase);

					for (int _i1 = 0; _i1 < a1.Length; _i1++)
					{
						_sReplace = _sReplace.Replace($"[{_i1}]", a1[_i1]);
					}
				}
				catch
				{
					bErr = true;
					break;
				}

				// 該当なしの変換子を削除
				_sReplace = Regex.Replace(_sReplace, @"\[\d+\]", "");

				_ = sb.Append(_sReplace);
				_ = sb.Append(NL);
			}

			if (bErr)
			{
				SubCmdLogAddRem("文法エラー？", Color.Red);
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
					// (要)例外処理
					try
					{
						_ = int.TryParse(aResult[_i2], out int _i3);
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
			string flg = null; // ここは null でよい
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
		// Text File ?
		//--------------------------------------------------------------------------------
		private bool RtnIsTextFile(string fn)
		{
			if (!File.Exists(fn))
			{
				return false;
			}
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

		private (string, string) RtnTextFread(string path, bool bGuiOn, string filter) // return(ファイル名, データ)
		{
			if (bGuiOn || path.Length == 0)
			{
				OpenFileDialog ofd = new OpenFileDialog
				{
					Filter = filter,
					InitialDirectory = path.Length == 0 ? Environment.CurrentDirectory : Path.GetDirectoryName(path)
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

			if (RtnIsTextFile(path))
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

		private string RtnDirFileList(string path, int iDirFile, bool bRecursive)
		{
			// iDirFile
			//   0 = Dir + File
			//   1 = Dir
			//   2 = File
			try
			{
				path = Path.GetFullPath(path + "\\");
			}
			catch
			{
				return "";
			}

			if (!Directory.Exists(path))
			{
				return "";
			}

			GblRtnDirList.Clear();
			GblRtnFileList.Clear();

			// Dir
			GblRtnDirList.Add(path);
			SubDirList(path, bRecursive);

			// File
			if (iDirFile != 1)
			{
				SubFileList();
			}

			string rtn;

			switch (iDirFile)
			{
				// Dir
				case (1):
					GblRtnDirList.Sort();
					rtn = string.Join(NL, GblRtnDirList);
					SubCmdLogAddRem($"{GblRtnDirList.Count()}フォルダ 該当", Color.Cyan);
					break;

				// File
				case (2):
					GblRtnFileList.Sort();
					rtn = string.Join(NL, GblRtnFileList);
					SubCmdLogAddRem($"{GblRtnFileList.Count()}ファイル 該当", Color.Cyan);
					break;

				// Dir + File
				default:
					GblRtnDirList.AddRange(GblRtnFileList);
					GblRtnDirList.Sort();
					rtn = string.Join(NL, GblRtnDirList);
					SubCmdLogAddRem($"{GblRtnDirList.Count()}フォルダ／ファイル 該当", Color.Cyan);
					break;
			}

			GblRtnDirList.Clear();
			GblRtnFileList.Clear();

			return rtn;
		}

		// 再帰
		private void SubDirList(string path, bool bRecursive)
		{
			// Dir 取得
			// SearchOption.AllDirectories はシステムフォルダ・アクセス時にエラーが出るので使用不可
			foreach (string _s1 in Directory.GetDirectories(path, "*"))
			{
				GblRtnDirList.Add(_s1 + "\\");
				try
				{
					if (bRecursive)
					{
						SubDirList(_s1, bRecursive);
					}
					else
					{
						return;
					}
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
