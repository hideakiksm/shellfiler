using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.Virtual;

namespace ShellFiler.FileTask.ArgumentConverter {

    //=========================================================================================
    // クラス：コマンド引数の変換クラス
    //    $F    対象パスのカーソル上のファイルのファイル名（例　MyFile.txt）
    //    $P    対象パスのカーソル上のファイルのフルパス名（例　C:\data\MyFile.txt）
    //    $M    対象パスのマークファイル名（例　C:\data\MyFile.txt C:\data\sample.jpg）
    //    $D    対象パスのフォルダ名（例　C:）
    //    $OF   反対パスのカーソル上のファイルのファイル名
    //    $OP   反対パスのカーソル上のファイルのフルパス名
    //    $OM   反対パスのマークファイル名
    //    $OD   反対パスのフォルダ名
    //    $$    「$」1文字
    //    $<メッセージ> この位置でキー入力を行います。
    //
    //    "C:\Data\LOG.txt"
    //    :e    拡張子（例：txt）
    //    :r    拡張子をのぞいた部分（例：LOG）
    //    :d    ドライブ名（例：C）ネットワークの場合、空文字列となります。
    //    :h    パス名（例：C:Data）
    //    :t    ファイル名本体（例：LOG.txt）
    //    :n    クォーティングがある場合にはずす（例：C:\DAT\LOG.txt）
    //    :q    クォーティングがない場合に付ける（例：C:\DAT\LOG.txt）
    //    :au   大文字化（例：C:\DATA\LOG.TXT）
    //    :al   小文字化（例：c:\data\log.txt）
    //    :ac   先頭大文字化（例：C:\Data\Log.Sys）
    //=========================================================================================
    public class ShellCommandArgument {
        // プログラムの引数
        private string m_argument;

        // 対象パスのファイル一覧
        private FileListView m_target;

        // 反対パスのファイル一覧
        private FileListView m_opposite;

        // 対象パスの必要ファイル
        private LocalExecuteUseFile m_targetUseFile;

        // 反対パスの必要ファイル
        private LocalExecuteUseFile m_oppositeUseFile;

        // 引数についての解析済みの要素配列
        private List<ArgumentElement> m_argumentList = new List<ArgumentElement>();

        // エラーメッセージ（エラーがないときはnull）
        private string m_errorMessage = null;

        // キー入力した結果
        private string m_keyInputResult = null;

        // 転送元のファイル一覧
        private IFileProviderSrc m_fileProviderSrc = null;

        // 転送先のファイル一覧
        private IFileProviderDest m_fileProviderDest = null;

        // 対象パス FileProvider中のカーソル位置のインデックス（コマンド引数上、不要なときは-1）
        private int m_cursorFileProviderIndexSrc;

        // 反対パス FileProvider中のカーソル位置のインデックス（コマンド引数上、不要なときは-1）
        private int m_cursorFileProviderIndexDest;

        // 対象パス マーク中ファイルのインデックス一覧（コマンド引数上、不要なときはnull）
        private List<int> m_markFileProviderIndexSrc = null;

        // 反対パス マーク中ファイルのインデックス一覧（コマンド引数上、不要なときはnull）
        private List<int> m_markFileProviderIndexDest = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]argument         プログラムの引数
        // 　　　　[in]target           対象パスのファイル一覧
        // 　　　　[in]opposite         反対パスのファイル一覧
        // 　　　　[in]targetUseFile    対象パスで必要なファイル
        // 　　　　[in]oppositeUseFile  反対パスで必要なファイル
        // 戻り値：なし
        //=========================================================================================
        public ShellCommandArgument(string argument, FileListView target, FileListView opposite, LocalExecuteUseFile targetUseFile, LocalExecuteUseFile oppositeUseFile) {
            m_argument = argument;
            m_target = target;
            m_opposite = opposite;
            m_targetUseFile = targetUseFile;
            m_oppositeUseFile = oppositeUseFile;
        }

        //=========================================================================================
        // 機　能：引数の意味解析を行う
        // 引　数：なし
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        public bool ParseArgument() {
            bool success;

            // コマンド引数の意味を解析
            success = ParseArgumentMain(m_argument);
            if (!success) {
                return success;
            }

            // 必要ファイルとの矛盾をチェック
            success = CheckFileCount();
            if (!success) {
                return success;
            }
            success = CheckUseFile();
            if (!success) {
                return success;
            }

            // 転送元と転送先を用意
            SetFileProvider();

            return success;
        }

        //=========================================================================================
        // 機　能：エラー情報を登録する
        // 引　数：[in]index    コマンド引数の中でエラーが発生した位置の周辺となるインデックス
        // 　　　　[in]error    登録するエラーの種類
        // 戻り値：なし
        //=========================================================================================
        private bool RegistError(int index, ParseError error) {
            m_errorMessage = string.Format(error.ErrorMessage, index, m_argument);
            return false;
        }

        //=========================================================================================
        // 機　能：解析を実行する
        // 引　数：[in]argument  ユーザーが指定したマクロ付きの引数
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool ParseArgumentMain(string argument) {
            bool success;
            int index = 0;
            while (index < argument.Length) {
                char ch = argument[index];
                if (ch == '$') {
                    if (index + 1 < argument.Length && argument[index + 1] == '<') {
                        // $<トークン> を解析
                        string message;
                        int nextIndex;
                        success = GetTokenKeyInput(argument, index + 2, out message, out nextIndex);
                        if (!success) {
                            return success;
                        }
                        string orgPart = "$<" + message + ">";
                        m_argumentList.Add(new ArgumentElement(ArgumentElementType.KeyInput, ArgumentElementDecorator.None, message, orgPart));
                        index = nextIndex;
                    } else {
                        // $変数名:修飾子 を解析
                        string variable, decorator;
                        int nextIndex;
                        success = GetTokenVariable(argument, index + 1, out variable, out decorator, out nextIndex);
                        if (!success) {
                            return success;
                        }
                        ArgumentElementType elementType;
                        ArgumentElementDecorator elementDecorator;
                        success = ParseTokenVariable(index, variable, decorator, out elementType, out elementDecorator);
                        if (!success) {
                            return success;
                        }
                        string orgPart;
                        if (decorator == "") {
                            orgPart = "$" + variable;
                        } else {
                            orgPart = "$" + variable + ":" + decorator;
                        }
                        m_argumentList.Add(new ArgumentElement(elementType, elementDecorator, null, orgPart));
                        index = nextIndex;
                    }
                } else {
                    // 通常の文字列部分
                    string strValue;
                    int nextIndex;
                    GetStringValue(argument, index, out strValue, out nextIndex);
                    m_argumentList.Add(new ArgumentElement(ArgumentElementType.StringPart, ArgumentElementDecorator.None, strValue, strValue));
                    index = nextIndex;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：キー入力のトークンを取得する
        // 引　数：[in]argument    コマンド引数
        // 　　　　[in]startIndex  開始位置のインデックス
        // 　　　　[out]message    キー入力時に表示するメッセージを返す変数
        // 　　　　[out]nextIndex  引数の次の解析位置を返す変数
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool GetTokenKeyInput(string argument, int startIndex, out string message, out int nextIndex) {
            message = null;
            nextIndex = startIndex;
            int lastIndex = argument.IndexOf('>', startIndex);
            if (lastIndex == -1) {
                return RegistError(startIndex, ParseError.KeyInputNotClose);
            }
            message = argument.Substring(startIndex, lastIndex - startIndex);
            nextIndex = lastIndex + 1;
            return true;
        }

        //=========================================================================================
        // 機　能：変数のトークンを取得する
        // 引　数：[in]argument    コマンド引数
        // 　　　　[in]startIndex  開始位置のインデックス
        // 　　　　[out]variable   $の次の文字からの変数名を返す変数
        // 　　　　[out]decorator  :の次の文字からの修飾子を返す変数
        // 　　　　[out]nextIndex  引数の次の解析位置を返す変数
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool GetTokenVariable(string argument, int startIndex, out string variable, out string decorator, out int nextIndex) {
            variable = "";
            decorator = "";
            nextIndex = startIndex;

            // 変数名を抽出
            int index = startIndex;
            while (index < argument.Length) {
                char ch = argument[index];
                if (ch == ':' || ch < 'A' ||'Z' < ch) {
                    break;
                }
                index++;
            }
            if (index < argument.Length && argument[index] == '$' && index == startIndex) {
                variable = "$";
                decorator = "";
                nextIndex = index + 1;
                return true;
            }
            variable = argument.Substring(startIndex, index - startIndex);
            if (variable.Length == 0) {
                return RegistError(startIndex, ParseError.VariableNameEmpty);
            }

            // 変数名だけか？
            if (index >= argument.Length || argument[index] != ':') {
                nextIndex = index;
                return true;
            }

            // 修飾子を抽出
            startIndex = index + 1;
            index = startIndex;
            while (index < argument.Length) {
                char ch = argument[index];
                if (ch < 'a' ||'z' < ch) {
                    break;
                }
                index++;
            }
            decorator = argument.Substring(startIndex, index - startIndex);
            if (decorator.Length == 0) {
                return RegistError(startIndex, ParseError.DecoratorEmpty);
            }
            nextIndex = index;
            return true;
        }

        //=========================================================================================
        // 機　能：変数のトークンを解析し、内部シンボルで返す（$OM:auのOMとauの意味解釈）
        // 引　数：[in]startIndex         変数を解析したときの引数内の開始位置のインデックス
        // 　　　　[in]variable           $の次の文字からの変数名
        // 　　　　[in]decorator          :の次の文字からの修飾子
        // 　　　　[out]elementType       解析した変数
        // 　　　　[out]elementDecorator  解析した修飾子
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool ParseTokenVariable(int startIndex, string variable, string decorator, out ArgumentElementType elementType, out ArgumentElementDecorator elementDecorator) {
            elementType = ArgumentElementType.StringPart;
            elementDecorator = ArgumentElementDecorator.None;
            if (variable == "$") {
                elementType = ArgumentElementType.DollarMark;
            } else if (variable == "F") {
                elementType = ArgumentElementType.TargetFile;
            } else if (variable == "P") {
                elementType = ArgumentElementType.TargetPath;
            } else if (variable == "M") {
                elementType = ArgumentElementType.TargetMark;
            } else if (variable == "D") {
                elementType = ArgumentElementType.TargetDirectory;
            } else if (variable == "OF") {
                elementType = ArgumentElementType.OppositeFile;
            } else if (variable == "OP") {
                elementType = ArgumentElementType.OppositePath;
            } else if (variable == "OM") {
                elementType = ArgumentElementType.OppositeMark;
            } else if (variable == "OD") {
                elementType = ArgumentElementType.OppositeDirectory;
            } else {
                return RegistError(startIndex, ParseError.UnknownVariable);
            }

            if (decorator == "") {
                elementDecorator = ArgumentElementDecorator.None;
            } else if (decorator == "e") {
                elementDecorator = ArgumentElementDecorator.Extension;
            } else if (decorator == "r") {
                elementDecorator = ArgumentElementDecorator.RemoveExtension;
            } else if (decorator == "d") {
                elementDecorator = ArgumentElementDecorator.DriveName;
            } else if (decorator == "h") {
                elementDecorator = ArgumentElementDecorator.PathName;
            } else if (decorator == "t") {
                elementDecorator = ArgumentElementDecorator.FileBody;
            } else if (decorator == "n") {
                elementDecorator = ArgumentElementDecorator.NotQuote;
            } else if (decorator == "q") {
                elementDecorator = ArgumentElementDecorator.Quote;
            } else if (decorator == "au") {
                elementDecorator = ArgumentElementDecorator.ToUpper;
            } else if (decorator == "al") {
                elementDecorator = ArgumentElementDecorator.ToLower;
            } else if (decorator == "ac") {
                elementDecorator = ArgumentElementDecorator.ToCapital;
            } else {
                return RegistError(startIndex, ParseError.UnknownDecorator);
            }
            return true;
        }

        //=========================================================================================
        // 機　能：コマンド引数の文字列部分を取得する
        // 引　数：[in]argument    コマンド引数
        // 　　　　[in]startIndex  開始位置のインデックス
        // 　　　　[out]strValue   文字列部分の値を返す変数
        // 　　　　[out]nextIndex  引数の次の解析位置を返す変数
        // 戻り値：なし
        //=========================================================================================
        private void GetStringValue(string argument, int startIndex, out string strValue, out int nextIndex) {
            int lastIndex = argument.IndexOf('$', startIndex);
            if (lastIndex == -1) {
                strValue = argument.Substring(startIndex);
                nextIndex = argument.Length;
            } else {
                strValue = argument.Substring(startIndex, lastIndex - startIndex);
                nextIndex = lastIndex;
            }
        }

        //=========================================================================================
        // 機　能：ファイル数が妥当かどうかを判断する
        // 引　数：なし
        // 戻り値：妥当なときtrue
        //=========================================================================================
        private bool CheckFileCount() {
            if (m_opposite.FileList.Files.Count == 0 && m_oppositeUseFile != LocalExecuteUseFile.None) {
                m_errorMessage = Resources.ArgumentParse_ErrorOppositeNotExistFile;
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：使用するファイルとコマンドラインの組み合わせが妥当かどうかを判断する
        // 引　数：なし
        // 戻り値：妥当なときtrue
        //=========================================================================================
        private bool CheckUseFile() {
            bool cursorParentTarget = (m_target.FileList.Files.Count > 0 && m_target.FileList.Files[m_target.FileListViewComponent.CursorLineNo].FileName == "..");
            bool cursorParentOpposite = (m_opposite.FileList.Files.Count > 0 && m_opposite.FileList.Files[m_opposite.FileListViewComponent.CursorLineNo].FileName == "..");
            bool valid = true;                      // 組み合わせが正しいときtrue
            bool validCursorTarget = true;          // 対象パスの組み合わせが正しいときtrue
            bool validCursorOpposite = true;        // 反対パスの組み合わせが正しいときtrue
            foreach (ArgumentElement element in m_argumentList) {
                switch (element.Type) {
                    case ArgumentElementType.TargetFile:
                        if (!m_targetUseFile.CanUseTarget) {
                            valid = false;
                        }
                        if (cursorParentTarget) {
                            validCursorTarget = false;
                        }
                        break;
                    case ArgumentElementType.TargetPath:
                        if (!m_targetUseFile.CanUseTarget) {
                            valid = false;
                        }
                        if (cursorParentTarget) {
                            validCursorTarget = false;
                        }
                        break;
                    case ArgumentElementType.TargetMark:
                        if (!m_targetUseFile.CanUseMark) {
                            valid = false;
                        }
                        break;
                    case ArgumentElementType.TargetDirectory:
                        if (!m_targetUseFile.CanUseTarget) {
                            valid = false;
                        }
                        break;
                    case ArgumentElementType.OppositeFile:
                        if (!m_oppositeUseFile.CanUseTarget) {
                            valid = false;
                        }
                        if (cursorParentOpposite) {
                            validCursorOpposite = false;
                        }
                        break;
                    case ArgumentElementType.OppositePath:
                        if (!m_oppositeUseFile.CanUseTarget) {
                            valid = false;
                        }
                        if (cursorParentOpposite) {
                            validCursorOpposite = false;
                        }
                        break;
                    case ArgumentElementType.OppositeMark:
                        if (!m_oppositeUseFile.CanUseMark) {
                            valid = false;
                        }
                        break;
                    case ArgumentElementType.OppositeDirectory:
                        if (!m_oppositeUseFile.CanUseTarget) {
                            valid = false;
                        }
                        break;
                 }
            }
            if (!valid) {
                m_errorMessage = Resources.ArgumentParse_ErrorUseFile;
                return false;
            }
            if (!validCursorTarget || m_targetUseFile.CanUseTarget && cursorParentTarget) {
                m_errorMessage = Resources.ArgumentParse_ErrorCursorTarget;
                return false;
            }
            if (!validCursorOpposite || m_oppositeUseFile.CanUseTarget && cursorParentOpposite) {
                m_errorMessage = Resources.ArgumentParse_ErrorCursorOpposite;
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルプロバイダーを作成してメンバーに格納する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void SetFileProvider() {
            FileSystemFactory factory = Program.Document.FileSystemFactory;

            // 転送元ファイル一覧
            IFileSystem srcFileSystem = factory.CreateFileSystemForFileOperation(m_target.FileList.FileSystem.FileSystemId);
            List<UIFile> targetFileList;
            GetUseFileList(m_target, m_targetUseFile, out targetFileList, out m_cursorFileProviderIndexSrc, out m_markFileProviderIndexSrc);
            m_fileProviderSrc = new FileProviderSrcMarked(m_target.FileList, targetFileList, srcFileSystem, null);      // バックグラウンドタスク起動直前にVirtualFolderInfoを設定
 
            // 転送先ファイル一覧
            if (m_oppositeUseFile == LocalExecuteUseFile.None) {
                m_fileProviderDest = new FileProviderDestDummy();
            } else {
                IFileSystem destFileSystem = factory.CreateFileSystemForFileOperation(m_opposite.FileList.FileSystem.FileSystemId);
                List<UIFile> oppositeFileList;
                GetUseFileList(m_opposite, m_oppositeUseFile, out oppositeFileList, out m_cursorFileProviderIndexDest, out m_markFileProviderIndexDest);
                List<SimpleFileDirectoryPath> oppositeSimpleFileList = UIFile.UIFileListToSimpleFileList(m_opposite.FileList.DisplayDirectoryName, oppositeFileList);
                m_fileProviderDest = new FileProviderDestMarked(m_opposite.FileList.DisplayDirectoryName, destFileSystem, oppositeSimpleFileList, null);
            }
        }

        //=========================================================================================
        // 機　能：使用するファイルの一覧を作成する
        // 引　数：[in]fileListView   ファイル一覧ウィンドウ
        // 　　　　[in]fileUse        対象のファイルが必要かどうかの情報
        // 　　　　[out]fileList      使用するファイルの一覧を返す変数
        // 　　　　[out]cursorIndex   fileList中のカーソル位置のインデックスを返す変数
        // 　　　　[out]markFileIndex マークされているファイル一覧のインデックス配列を返す変数
        // 戻り値：なし
        //=========================================================================================
        private void GetUseFileList(FileListView fileListView, LocalExecuteUseFile fileUse, out List<UIFile> fileList, out int cursorIndex, out List<int> markFileIndex) {
            int cursor = fileListView.FileListViewComponent.CursorLineNo;
            cursorIndex = -1;
            markFileIndex = null;
            fileList = new List<UIFile>();
            if (fileUse == LocalExecuteUseFile.Cursor) {
                // カーソル位置のファイル
                fileList.Add(fileListView.FileList.Files[cursor]);
                cursorIndex = 0;
            } else if (fileUse == LocalExecuteUseFile.CursorMark) {
                // カーソル位置とマークファイル
                UIFile cursorUIFile = fileListView.FileList.Files[cursor];
                if (cursorUIFile.Marked) {
                    // カーソル位置がマーク中の場合はマーク一覧に含まれる
                    fileList.AddRange(fileListView.FileList.MarkFiles);
                    for (int i = 0; i < fileList.Count; i++) {
                        if (fileList[i] == cursorUIFile) {
                            cursorIndex = i;
                            break;
                        }
                    }
                    markFileIndex = new List<int>();
                    for (int i = 0; i < fileList.Count; i++) {
                        markFileIndex.Add(i);           // 0…n-1
                    }
                } else {
                    // カーソル位置がマーク中ではない場合は独自に追加
                    fileList.Add(fileListView.FileList.Files[cursor]);
                    fileList.AddRange(fileListView.FileList.MarkFiles);
                    cursorIndex = 0;
                    markFileIndex = new List<int>();
                    for (int i = 1; i < fileList.Count; i++) {
                        markFileIndex.Add(i);           // 1…n-1
                    }
                }
            } else {
                // すべて
                cursorIndex = cursor;
                markFileIndex = new List<int>();
                for (int i = 0; i < fileListView.FileList.Files.Count; i++) {
                    if (fileListView.FileList.Files[i].FileName == "..") {
                        continue;
                    }
                    fileList.Add(fileListView.FileList.Files[i]);
                    if (fileListView.FileList.Files[i].Marked) {
                        markFileIndex.Add(i);
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：表示用に解釈したコマンドライン引数を整形する
        // 引　数：[in]keyValueList  キー入力した内容
        // 戻り値：コマンドライン文字列を表示用に置き換えたもの（"-b <マークファイル>"など）
        //=========================================================================================
        public string GetDisplayCommand(List<string> keyValueList) {
            int keyValueIndex = 0;
            StringBuilder sb = new StringBuilder();
            foreach (ArgumentElement element in m_argumentList) {
                switch (element.Type) {
                    case ArgumentElementType.KeyInput:
                        sb.Append(keyValueList[keyValueIndex]);
                        keyValueIndex++;
                        break;
                    default:
                        sb.Append(element.OriginalPart);
                        break;
                 }
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：コマンド引数の最終文字列を取得する
        // 引　数：[in]srcPathList   対象パス用の、実態または仮想フォルダでのファイル一覧
        // 　　　　[in]destPathList  反対パス用の、実態または仮想フォルダでのファイル一覧
        // 戻り値：コマンド引数の文字列
        //=========================================================================================
        public string GetCommandArgument(List<string> srcPathList, List<string> destPathList) {
            if (m_fileProviderSrc.FileListContext != null) {
                srcPathList = m_fileProviderSrc.FileListContext.GetExecuteLocalPathList(srcPathList);
            }

            StringBuilder sb = new StringBuilder();
            foreach (ArgumentElement element in m_argumentList) {
                switch (element.Type) {
                    case ArgumentElementType.StringPart:
                        sb.Append(element.StringValue);
                        break;
                    case ArgumentElementType.DollarMark:
                        sb.Append("$");
                        break;
                    case ArgumentElementType.TargetFile:
                        sb.Append(DecorateFilePath(GenericFileStringUtils.GetFileName(srcPathList[m_cursorFileProviderIndexSrc]), element));
                        break;
                    case ArgumentElementType.TargetPath:
                        sb.Append(DecorateFilePath(srcPathList[m_cursorFileProviderIndexSrc], element));
                        break;
                    case ArgumentElementType.TargetMark:
                        for (int i = 0; i < m_markFileProviderIndexSrc.Count; i++) {
                            int index = m_markFileProviderIndexSrc[i];
                            string file = DecorateFilePath(srcPathList[index], element);
                            sb.Append(file);
                            if (i != m_markFileProviderIndexSrc.Count - 1 && file.Length > 0) {
                                sb.Append(' ');
                            }
                        }
                        break;
                    case ArgumentElementType.TargetDirectory:
                        sb.Append(DecorateFilePath(GenericFileStringUtils.GetDirectoryName(srcPathList[0]), element));
                        break;
                    case ArgumentElementType.OppositeFile:
                        sb.Append(DecorateFilePath(GenericFileStringUtils.GetFileName(destPathList[m_cursorFileProviderIndexDest]), element));
                        break;
                    case ArgumentElementType.OppositePath:
                        sb.Append(DecorateFilePath(destPathList[m_cursorFileProviderIndexDest], element));
                        break;
                    case ArgumentElementType.OppositeMark:
                        for (int i = 0; i < m_markFileProviderIndexDest.Count; i++) {
                            int index = m_markFileProviderIndexDest[i];
                            string file = DecorateFilePath(destPathList[index], element);
                            sb.Append(file);
                            if (i != m_markFileProviderIndexDest.Count - 1 && file.Length > 0) {
                                sb.Append(' ');
                            }
                        }
                        break;
                    case ArgumentElementType.OppositeDirectory:
                        sb.Append(DecorateFilePath(GenericFileStringUtils.GetDirectoryName(destPathList[0]), element));
                        break;
                    case ArgumentElementType.KeyInput:
                        sb.Append(m_keyInputResult);
                        break;
                 }
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：ファイルパスの文字列を修飾子で整形する
        // 引　数：[in]fileName    元のファイルパス
        // 　　　　[in]element     修飾子を適用する情報が入ったコマンドライン引数の要素
        // 戻り値：修飾したファイルパス
        //=========================================================================================
        private string DecorateFilePath(string fileName, ArgumentElement element) {
            fileName = GenericFileStringUtils.CompleteQuoteFileName(fileName);
            string result = null;
            switch (element.Decorator) {
                case ArgumentElementDecorator.None:
                    result = fileName;
                    break;
                case ArgumentElementDecorator.Extension:
                    result = GenericFileStringUtils.GetExtensionLast(fileName);
                    break;
                case ArgumentElementDecorator.RemoveExtension:
                    result = fileName.Substring(0, fileName.Length - GenericFileStringUtils.GetExtensionLast(fileName).Length);
                    break;
                case ArgumentElementDecorator.DriveName:
                    result = GenericFileStringUtils.GetDriveName(fileName);
                    break;
                case ArgumentElementDecorator.PathName:
                    result = GenericFileStringUtils.RemoveLastDirectorySeparator(GenericFileStringUtils.GetDirectoryName(fileName));
                    break;
                case ArgumentElementDecorator.FileBody:
                    result = GenericFileStringUtils.GetFileName(fileName);
                    break;
                case ArgumentElementDecorator.NotQuote:
                    result = StringUtils.RemoveStringQuote(fileName);
                    break;
                case ArgumentElementDecorator.Quote:
                    result = StringUtils.AddStringQuote(fileName, "\"");
                    break;
                case ArgumentElementDecorator.ToUpper:
                    result = fileName.ToUpper();
                    break;
                case ArgumentElementDecorator.ToLower:
                    result = fileName.ToLower();
                    break;
                case ArgumentElementDecorator.ToCapital:
                    result = StringUtils.CapitalString(fileName);
                    break;
            }
            return result;
        }

        //=========================================================================================
        // プロパティ：解析済みの引数情報が反対パスの引数を含むときtrue
        //=========================================================================================
        public bool HasOpposite {
            get {
                foreach (ArgumentElement element in m_argumentList) {
                    switch (element.Type) {
                        case ArgumentElementType.StringPart:
                            break;
                        case ArgumentElementType.DollarMark:
                            break;
                        case ArgumentElementType.TargetFile:
                            break;
                        case ArgumentElementType.TargetPath:
                            break;
                        case ArgumentElementType.TargetMark:
                            break;
                        case ArgumentElementType.TargetDirectory:
                            break;
                        case ArgumentElementType.OppositeFile:
                            return true;
                        case ArgumentElementType.OppositePath:
                            return true;
                        case ArgumentElementType.OppositeMark:
                            return true;
                        case ArgumentElementType.OppositeDirectory:
                            return true;
                        case ArgumentElementType.KeyInput:
                            break;
                     }
                }
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：プログラムの引数
        //=========================================================================================
        public string Argument {
            get {
                return m_argument;
            }
        }

        //=========================================================================================
        // プロパティ：エラーメッセージ（エラーがないときはnull）
        //=========================================================================================
        public string ErrorMessage {
            get {
                return m_errorMessage;
            }
        }

        //=========================================================================================
        // プロパティ：キー入力が必要なときtrue
        //=========================================================================================
        public bool IsInputKey {
            get {
                foreach (ArgumentElement element in m_argumentList) {
                    if (element.Type == ArgumentElementType.KeyInput) {
                        return true;
                    }
                }
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：キー入力用のメッセージ（キー入力がない場合はnull）
        //=========================================================================================
        public List<string> KeyInputMessage {
            get {
                List<string> result = new List<string>();
                foreach (ArgumentElement element in m_argumentList) {
                    if (element.Type == ArgumentElementType.KeyInput) {
                        result.Add(element.StringValue);
                    }
                }
                return result;
            }
        }

        //=========================================================================================
        // プロパティ：ユーザーがキー入力したコマンドライン引数の一部
        //=========================================================================================
        public string KeyInputResult {
            set {
                m_keyInputResult = value;
            }
        }

        //=========================================================================================
        // プロパティ：対象パスのファイル一覧
        //=========================================================================================
        public FileListView Target {
            get {
                return m_target;
            }
        }

        //=========================================================================================
        // プロパティ：反対パスのファイル一覧
        //=========================================================================================
        public FileListView Opposite {
            get {
                return m_opposite;
            }
        }

        //=========================================================================================
        // プロパティ：対象パスの必要ファイル
        //=========================================================================================
        public LocalExecuteUseFile TargetUseFile {
            get {
                return m_targetUseFile;
            }
        }

        //=========================================================================================
        // プロパティ：反対パスの必要ファイル
        //=========================================================================================
        public LocalExecuteUseFile OppositeUseFile {
            get {
                return m_oppositeUseFile;
            }
        }

        //=========================================================================================
        // プロパティ：転送元のファイル一覧
        //=========================================================================================
        public IFileProviderSrc FileProviderSrc {
            get {
                return m_fileProviderSrc;
            }
        }

        //=========================================================================================
        // プロパティ：転送先のファイル一覧
        //=========================================================================================
        public IFileProviderDest FileProviderDest {
            get {
                return m_fileProviderDest;
            }
        }
    }
}
