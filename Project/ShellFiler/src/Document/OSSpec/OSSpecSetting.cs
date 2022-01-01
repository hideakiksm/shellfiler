using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.UI;
using ShellFiler.Command;

namespace ShellFiler.Document.OSSpec {

    //=========================================================================================
    // クラス：サーバーの仕様設定
    //=========================================================================================
    public class OSSpecSetting {
        // OS特性の一覧
        private List<ShellCommandDictionary> m_commandDictionaryList = new List<ShellCommandDictionary>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public OSSpecSetting() {
            {
                ShellCommandDictionary dic = new ShellCommandDictionary();
                m_commandDictionaryList.Add(dic);

                dic.CommandChangePrompt = "export PS1=\"{0}\"";

                dic.CommandGetFileList = "ls -a -l -Q --time-style=full-iso {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "ls: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.Comment,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "total"),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.UInt)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.Comment,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "合計"),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.UInt)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.LsAttr),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsLink),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsOwner),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsGroup),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsSize),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsUpdTimeFull),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsFileName)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.Repeat | OSSpecLineType.OrPrev,
                            new OSSpecColumnExpect(OSSpecTokenType.LsAttr),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsLink),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsOwner),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsGroup),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsSize),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsUpdTimeFull),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.LsFileName),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "->"),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsLnPath)));
                    dic.ExpectGetFileList = expect;
                }

                dic.CommandGetVolumeInfo = "df -k -P {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "df: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.Comment | OSSpecLineType.OrPrev,
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.None,
                            new OSSpecColumnExpect(OSSpecTokenType.Str),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.ULong),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.DfUsed),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.DfFree),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.DfUsedP),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectGetVolumeInfo = expect;
                }

                dic.CommandGetFileHead = "head -c {1} {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "head: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectGetFileHead = expect;
                }

                dic.CommandGetFileInfo = "stat --format=\"%A %h %U %G %s %y %x %N\" {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.None,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "stat: `"),
                            new OSSpecColumnExpect(OSSpecTokenType.StatNotFound)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "stat: cannot stat `"),
                            new OSSpecColumnExpect(OSSpecTokenType.StatNotFound)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine | OSSpecLineType.OrPrev,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "stat: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev,
                            new OSSpecColumnExpect(OSSpecTokenType.LsAttr),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsLink),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsOwner),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsGroup),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsSize),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsUpdTimeFull),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsAccTimeFull),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsFileName)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev,
                            new OSSpecColumnExpect(OSSpecTokenType.LsAttr),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsLink),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsOwner),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsGroup),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsSize),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsUpdTimeFull),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsAccTimeFull),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsFileName),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "->"),
                            new OSSpecColumnExpect(OSSpecTokenType.Space),
                            new OSSpecColumnExpect(OSSpecTokenType.LsLnPath)));
                    dic.ExpectGetFileInfo = expect;
                }

                dic.CommandSetFileTime = "touch -t {0} {1}";
                dic.CommandSetModifiedTime = "touch -mt {0} {1}";
                dic.CommandSetAccessedTime = "touch -at {0} {1}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "touch: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectSetFileTime = expect;
                }
                dic.ValueTouchTimeFormat = "CCYYMMDDhhmm.ss";

                dic.CommandSetPermissions = "chmod {0} {1}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "chmod: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectSetPermissions = expect;
                }

                dic.CommandMakeDirectory = "mkdir -p {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "mkdir: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectMakeDirectory = expect;
                }

                dic.CommandDeleteDirectoryRecursive = "rm -rf {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine | OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "rm: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectDeleteDirectoryRecursive = expect;
                }

                dic.CommandDeleteDirectory = "rmdir {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine | OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "rmdir: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectDeleteDirectory = expect;
                }

                dic.CommandDeleteFile = "rm {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine | OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "rm: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectDeleteFile = expect;
                }
                dic.ValueDeleteDirectoryRecursivePrompt = "rm: *(yes/no)";
                dic.ValueDeleteDirectoryRecursiveAnswer = "n";
                dic.ValueDeleteDirectoryPrompt = "rmdir: *(yes/no)";
                dic.ValueDeleteDirectoryAnswer = "n";
                dic.ValueDeleteFilePrompt = @"rm\:.*(yes/no)";
                dic.ValueDeleteFileAnswer = "n";

                dic.CommandUpload = "cat << {0} | xxd -r -p > {1}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine | OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "cat: `"),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine | OSSpecLineType.Repeat | OSSpecLineType.OrPrev,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "xxd: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.Comment | OSSpecLineType.Repeat | OSSpecLineType.OrPrev,
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectUpload = expect;
                }
                dic.ValueUploadHearDocument = "> ";
                dic.ValueUploadEncoding = ShellUploadEncoding.HexStream;
                dic.ValueUploadChunkSize = 1024;

                dic.CommandDownload = "cat {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine | OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "cat: `"),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectDownload = expect;
                }

                dic.CommandComputeChecksum = "cksum {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "cksum: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev,
                            new OSSpecColumnExpect(OSSpecTokenType.CksumCRC),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.CksumSize),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectComputeChecksum = expect;
                }
                dic.ValueUploadDownloadRetryCount = 3;
                dic.ValueUploadDownloadUseCheckCksum = true;

                dic.CommandRename = "mv {0} {1}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "mv: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectRename = expect;
                }

                dic.CommandChangeOwnerUser = "chown {1} {0}";
                dic.CommandChangeOwnerGroup = "chown :{1} {0}";
                dic.CommandChangeOwnerUserGroup = "chown {1}:{2} {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "chown: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectChangeOwner = expect;
                }

                dic.CommandCopyFile = "cp {0} {1}";
                dic.CommandCopyFileAndAttr = "cp -p {0} {1}";
                dic.CommandCopyDirectory = "cp -r {0} {1}";
                dic.CommandCopyDirectoryAndAttr = "cp -r -p {0} {1}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "cp: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectCopy = expect;
                }

                dic.CommandMove = "mv -i {0} {1}";
                dic.CommandMoveAndAttr = "mv -i {0} {1}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "mv: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectMove = expect;
                }

                dic.CommandSymboricLink = "ln -s -f {0} {1}";
                dic.CommandHardLink = "ln -f {0} {1}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "ln: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectMove = expect;
                }

                dic.CommandChangeLoginUser = "su {0}";
                dic.CommandChangeLoginUserShell = "su - {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "su: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "不明な ID です: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectChangeLoginUser = expect;
                }

                dic.CommandExit = "exit";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "exit: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectExit = expect;
                }
                dic.ValueRootUserName = "root";
                dic.ValueChangeUserPrompt = new string[] {"パスワード: ", "Password: "};
                dic.ValueChangeUserLoginShell = true;
                dic.ValuePromptUserServer = @"\\u@\\h";

                dic.CommandCheckSymbolicLink = "ls -l {0} && test -e {0} && echo \"{1}\" && test -d {0} && echo \"{2}\"";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "cd: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "ls: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "test: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectCheckSymbolicLink = expect;
                }
                dic.ValueLinkTargetSameTimeCount = 10;

                dic.CommandRetrieveFolderSize = "du -b {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "du: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.Repeat,
                            new OSSpecColumnExpect(OSSpecTokenType.DuDirSize),
                            new OSSpecColumnExpect(OSSpecTokenType.SpaceRepeat),
                            new OSSpecColumnExpect(OSSpecTokenType.DuDirPath)));
                    dic.ExpectRetrieveFolderSize = expect;
                }

                dic.CommandShellExecutePrev = "cd {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "cd: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectShellExecute = expect;
                }

                dic.CommandGetProcessList = "id -u; ps auxw";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "id: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev | OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "ps: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectGetProcessList = expect;
                }

                dic.CommandNetStat = "netstat -a -t -u";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "netstat: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectNetStat = expect;
                }

                dic.CommandKillProcess = "kill -9 {0}";
                dic.CommandKillProcessForce = "kill {0}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "kill: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectKillProcess = expect;
                }

                dic.CommandArchiveZipTime = "zip -r -o";
                dic.CommandArchiveZip = "zip -r";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "zip: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectArchiveZip = expect;
                }
                dic.ValueArchiveZipCompressionOption = "-{0}";
                dic.ValueArchiveZipComplessionDefault = 6;

                dic.CommandArchiveTarGz = "tar czf";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "tar: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectArchiveTarGz = expect;
                }

                dic.CommandArchiveTarBz2 = "tar cjf";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "tar: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectArchiveTarBz2 = expect;
                }

                dic.CommandArchiveTar = "tar cf";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "tar: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectArchiveTar = expect;
                }

                dic.CommandArchiveExecute = "cd {0} && rm -f {1} &&";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "cd: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine | OSSpecLineType.OrPrev,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "rm: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectArchiveExecute = expect;
                }

                dic.CommandGetCurrentDirectory = "pwd";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "pwd: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.OrPrev,
                            new OSSpecColumnExpect(OSSpecTokenType.PwdCurrent)));
                    dic.ExpectGetCurrentDirectory = expect;
                }

                dic.CommandAppendFileFirst = "cat {0} > {1}";
                dic.CommandAppendFileNext = "cat {0} >> {1}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "cat: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "-bash: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectAppendFile = expect;
                }

                dic.CommandSplitFile = "split -b {0} -d {1} {2}";
                {
                    List<OSSpecLineExpect> expect = new List<OSSpecLineExpect>();
                    expect.Add(new OSSpecLineExpect(
                            OSSpecLineType.ErrorLine,
                            new OSSpecColumnExpect(OSSpecTokenType.Specify, "split: "),
                            new OSSpecColumnExpect(OSSpecTokenType.StrAll)));
                    dic.ExpectSplitFile = expect;
                }
            }
        }
        

        public ShellCommandDictionary GetShellCommandDictionary(SSHUserAuthenticateSettingItem.OSType osType) {
            return m_commandDictionaryList[0];
        }

        //=========================================================================================
        // プロパティ：サーバー設定の一覧
        //=========================================================================================
        public List<ShellCommandDictionary> CommandDictionaryList {
            get {
                return m_commandDictionaryList;
            }
        }
    }
}
