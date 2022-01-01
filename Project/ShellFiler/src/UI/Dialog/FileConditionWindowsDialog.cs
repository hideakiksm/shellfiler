using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Condition;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：Windows用転送条件入力ダイアログ
    //=========================================================================================
    public partial class FileConditionWindowsDialog : Form {
        // 編集対象の更新条件（元のClone）
        private FileConditionItemWindows m_condition;

        // 編集時にすでに存在する項目のリスト
        private List<FileConditionItem> m_existingList;

        // 編集対象の項目のインデックス（新規作成のとき-1）
        private int m_existingIndex;

        // ドロップダウン 対象
        private LasyComboBoxImpl m_comboBoxImplTarget;

        // ドロップダウン ファイル名
        private LasyComboBoxImpl m_comboBoxImplFileName;

        // ドロップダウン 更新日時
        private LasyComboBoxImpl m_comboBoxImplUpdate;

        // ドロップダウン 作成日時
        private LasyComboBoxImpl m_comboBoxImplCreate;

        // ドロップダウン アクセス日時
        private LasyComboBoxImpl m_comboBoxImplAccess;

        // ドロップダウン ファイルサイズ
        private LasyComboBoxImpl m_comboBoxImplSize;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public FileConditionWindowsDialog(FileConditionItemWindows condition, List<FileConditionItem> existingList, int existingIndex) {
            InitializeComponent();
            m_condition = condition;
            m_existingList = existingList;
            m_existingIndex = existingIndex;

            // UI用に適当な初期値を設定
            m_condition.SetUIDefaultValue();

            // 表示名
            this.textBoxDispName.Text = condition.DisplayName;

            // 対象
            int targetIndex = TypeComboBoxIndexConverter.FileConditionTargetToIndex(condition.FileConditionTarget);
            m_comboBoxImplTarget = new LasyComboBoxImpl(this.comboBoxTarget, TypeComboBoxIndexConverter.TargetItems, targetIndex);

            // ファイル名
            int fileNameIndex = TypeComboBoxIndexConverter.FileNameTypeToIndex(condition.FileNameType);
            m_comboBoxImplFileName = new LasyComboBoxImpl(this.comboBoxFileName, TypeComboBoxIndexConverter.FileNameItems, fileNameIndex);
            this.textBoxFileName.Text = condition.FileName;

            // 更新日時
            int updateIndex = TypeComboBoxIndexConverter.DateTimeTypeToIndex(condition.UpdateTimeCondition.TimeType);
            m_comboBoxImplUpdate = new LasyComboBoxImpl(this.comboBoxDateUpdate, TypeComboBoxIndexConverter.TimeItems, updateIndex);

            // 作成日時
            int createIndex = TypeComboBoxIndexConverter.DateTimeTypeToIndex(condition.CreateTimeCondition.TimeType);
            m_comboBoxImplCreate = new LasyComboBoxImpl(this.comboBoxDateCreate, TypeComboBoxIndexConverter.TimeItems, createIndex);

            // アクセス日時
            int accessIndex = TypeComboBoxIndexConverter.DateTimeTypeToIndex(condition.AccessTimeCondition.TimeType);
            m_comboBoxImplAccess = new LasyComboBoxImpl(this.comboBoxDateAccess, TypeComboBoxIndexConverter.TimeItems, accessIndex);

            // ファイルサイズ
            int sizeIndex = TypeComboBoxIndexConverter.FileSizeTypeToIndex(condition.FileSizeCondition.SizeType);
            m_comboBoxImplSize = new LasyComboBoxImpl(this.comboBoxSize, TypeComboBoxIndexConverter.FileSizeItems, sizeIndex);

            // 属性
            SetAttributeRadio(this.radioReadonlyNone, this.radioReadonlyOn, this.radioReadonlyOff, condition.AttrReadOnly);
            SetAttributeRadio(this.radioHiddenNone, this.radioHiddenOn, this.radioHiddenOff, condition.AttrHidden);
            SetAttributeRadio(this.radioArchiveNone, this.radioArchiveOn, this.radioArchiveOff, condition.AttrArchive);
            SetAttributeRadio(this.radioSystemNone, this.radioSystemOn, this.radioSystemOff, condition.AttrSystem);

            // その他初期化
            EnableUIItem();
            ConditionDialogImpl.UpdateDatePanel(m_comboBoxImplUpdate, this.panelDateUpdate, condition.UpdateTimeCondition);
            ConditionDialogImpl.UpdateDatePanel(m_comboBoxImplCreate, this.panelDateCreate, condition.CreateTimeCondition);
            ConditionDialogImpl.UpdateDatePanel(m_comboBoxImplAccess, this.panelDateAccess, condition.AccessTimeCondition);
            ConditionDialogImpl.UpdateSizePanel(m_comboBoxImplSize, this.panelSize, m_condition.FileSizeCondition);

            // イベントを接続
            this.comboBoxFileName.SelectedIndexChanged += new System.EventHandler(this.comboBoxFileName_SelectedIndexChanged);
            this.comboBoxDateUpdate.SelectedIndexChanged += new System.EventHandler(this.comboBoxDate_SelectedIndexChanged);
            this.comboBoxDateCreate.SelectedIndexChanged += new System.EventHandler(this.comboBoxDate_SelectedIndexChanged);
            this.comboBoxDateAccess.SelectedIndexChanged += new System.EventHandler(this.comboBoxDate_SelectedIndexChanged);
            this.comboBoxSize.SelectedIndexChanged += new System.EventHandler(this.comboBoxSize_SelectedIndexChanged);
        }

        //=========================================================================================
        // 機　能：属性に対する条件をUIに反映する
        // 引　数：[in]radioNone  指定なしのラジオボタン
        // 　　　　[in]radioOn    ON指定のラジオボタン
        // 　　　　[in]radioOff   OFF指定のラジオボタン
        // 　　　　[in]flag       設定するフラグ
        // 戻り値：なし
        //=========================================================================================
        private void SetAttributeRadio(RadioButton radioNone, RadioButton radioOn, RadioButton radioOff, BooleanFlag flag) {
            if (flag == null) {
                radioNone.Checked = true;
            } else if (flag.Value == true) {
                radioOn.Checked = true;
            } else {
                radioOff.Checked = true;
            }
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            this.textBoxFileName.Enabled = (m_comboBoxImplFileName.SelectedIndex != 0);
        }

        //=========================================================================================
        // 機　能：ファイル名条件のコンボボックスの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxFileName_SelectedIndexChanged(object sender, EventArgs evt) {
            if (m_comboBoxImplFileName == null) {
                return;
            }
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：日付条件のコンボボックスの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxDate_SelectedIndexChanged(object sender, EventArgs evt) {
            if (m_comboBoxImplUpdate == null || m_comboBoxImplCreate == null || m_comboBoxImplAccess == null) {
                return;
            }
            if (sender == this.comboBoxDateUpdate) {
                ConditionDialogImpl.UpdateDatePanel(m_comboBoxImplUpdate, this.panelDateUpdate, m_condition.UpdateTimeCondition);
            } else if (sender == this.comboBoxDateCreate) {
                ConditionDialogImpl.UpdateDatePanel(m_comboBoxImplCreate, this.panelDateCreate, m_condition.CreateTimeCondition);
            } else if (sender == this.comboBoxDateAccess) {
                ConditionDialogImpl.UpdateDatePanel(m_comboBoxImplAccess, this.panelDateAccess, m_condition.AccessTimeCondition);
            }
        }

        //=========================================================================================
        // 機　能：サイズ条件のコンボボックスの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxSize_SelectedIndexChanged(object sender, EventArgs evt) {
            if (m_comboBoxImplSize == null) {
                return;
            }
            if (sender == this.comboBoxSize) {
                ConditionDialogImpl.UpdateSizePanel(m_comboBoxImplSize, this.panelSize, m_condition.FileSizeCondition);
            }
        }

        //=========================================================================================
        // 機　能：転送条件ヘルプのリンクがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelTransferHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            HelpMessageDialog dialog = new HelpMessageDialog(Resources.DlgTransferCond_TitleTransferHelp, NativeResources.HtmlTransferConditionTarget, null);
            dialog.Width = dialog.Width + 200;
            dialog.ShowDialog(this);
        }

        //=========================================================================================
        // 機　能：ファイル名ヘルプのリンクがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelFileNameHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            RegexTestDialog dialog = new RegexTestDialog();
            dialog.ShowDialog(this);
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            bool success;

            // 表示名
            string dispName = this.textBoxDispName.Text;
            if (dispName == "") {
                InfoBox.Warning(this, Resources.DlgFileCondition_ErrorConditionName);
                return;
            }
            for (int i = 0; i < m_existingList.Count; i++) {
                if (i != m_existingIndex && m_existingList[i].DisplayName == dispName) {
                    InfoBox.Warning(this, Resources.DlgFileCondition_ErrorDuplicateConditionName);
                    return;
                }
            }
            m_condition.DisplayName = dispName;

            // 対象
            FileConditionTarget target = TypeComboBoxIndexConverter.IndexToFileConditionTarget(m_comboBoxImplTarget.SelectedIndex);
            m_condition.FileConditionTarget = target;

            // ファイル名
            FileNameType fileNameType;
            string fileName;
            success = ConditionDialogImpl.GetFileNameCondition(this, m_comboBoxImplFileName, this.textBoxFileName, out fileNameType, out fileName);
            if (!success) {
                return;
            }
            m_condition.FileNameType = fileNameType;
            m_condition.FileName = fileName;

            // 更新日時
            success = ConditionDialogImpl.GetFileDate(this, m_condition.UpdateTimeCondition, this.panelDateUpdate, Resources.DlgFileCondition_ErrorDateUpdate, Resources.DlgFileCondition_ErrorDateUpdateReverse);
            if (!success) {
                return;
            }

            // 作成日時
            success = ConditionDialogImpl.GetFileDate(this, m_condition.CreateTimeCondition, this.panelDateCreate, Resources.DlgFileCondition_ErrorDateCreate, Resources.DlgFileCondition_ErrorDateCreateReverse);
            if (!success) {
                return;
            }

            // アクセス日時
            success = ConditionDialogImpl.GetFileDate(this, m_condition.AccessTimeCondition, this.panelDateAccess, Resources.DlgFileCondition_ErrorDateAccess, Resources.DlgFileCondition_ErrorDateAccessReverse);
            if (!success) {
                return;
            }

            // ファイルサイズ
            success = ConditionDialogImpl.GetFileSize(this, m_condition.FileSizeCondition, this.panelSize, Resources.DlgFileCondition_ErrorFileSize, Resources.DlgFileCondition_ErrorFileSizeReverse);
            if (!success) {
                return;
            }

            // 属性
            m_condition.AttrReadOnly = GetAttribute(this.radioReadonlyNone, this.radioReadonlyOn, this.radioReadonlyOff);
            m_condition.AttrHidden = GetAttribute(this.radioHiddenNone, this.radioHiddenOn, this.radioHiddenOff);
            m_condition.AttrArchive = GetAttribute(this.radioArchiveNone, this.radioArchiveOn, this.radioArchiveOff);
            m_condition.AttrSystem = GetAttribute(this.radioSystemNone, this.radioSystemOn, this.radioSystemOff);

            // 入力が空かどうかをチェック
            FileConditionItemWindows check = (FileConditionItemWindows)(m_condition.Clone());
            check.CleanupField();
            if (check.IsEmptyCondition()) {
                InfoBox.Warning(this, Resources.DlgFileCondition_ErrorNoCondition);
                return;
            }

            // 重複チェック
            for (int i = 0; i < m_existingList.Count; i++) {
                if (i != m_existingIndex) {
                    if (m_existingList[i] is FileConditionItemWindows && check.EqualsConfigObject((FileConditionItemWindows)(m_existingList[i]))) {
                        InfoBox.Warning(this, Resources.DlgFileCondition_ErrorSameItem, i + 1);
                        return;
                    }
                }
            }

            // 不要フィールドをクリア
            m_condition.CleanupField();

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：ファイル属性の条件をUIから読み込む
        // 引　数：[in]radioNone     指定なしのラジオボタン
        // 　　　　[in]radioOn       属性ONのラジオボタン
        // 　　　　[in]radioOff      属性OFFのラジオボタン
        // 戻り値：条件を読み込んだ結果（指定なしのときnull）
        //=========================================================================================
        private BooleanFlag GetAttribute(RadioButton radioNone, RadioButton radioOn, RadioButton radioOff) {
            if (radioNone.Checked) {
                return null;
            } else if (radioOn.Checked) {
                return new BooleanFlag(true);
            } else if (radioOff.Checked) {
                return new BooleanFlag(false);
            } else {
                Program.Abort("ラジオボタンの状態が想定外です。");
                return null;
            }
        }

        //=========================================================================================
        // クラス：条件設定ダイアログの共通実装
        //=========================================================================================
        public class ConditionDialogImpl {

            //=========================================================================================
            // 機　能：日付条件のパネルの状態を更新する
            // 引　数：[in]comboBox       変更されたコンボボックスの実装
            // 　　　　[in]panel          条件を反映するパネル
            // 　　　　[in]timeCondition  使用する条件の情報（m_conditionのフィールド）
            // 戻り値：なし
            //=========================================================================================
            public static void UpdateDatePanel(LasyComboBoxImpl comboBoxImpl, Panel panel, DateTimeCondition timeCondition) {
                DateTimeType type = TypeComboBoxIndexConverter.IndexToDateTimeType(comboBoxImpl.SelectedIndex);
                timeCondition.TimeType = type;
                if (type == DateTimeType.None) {
                    // 条件なし
                    if (panel.Controls.Count == 0) {
                        panel.Controls.Add(new FileConditionNoneControl());
                    } else if (panel.Controls[0] is FileConditionNoneControl) {
                        ;
                    } else {
                        Control oldControl = panel.Controls[0];
                        oldControl.Dispose();
                        panel.Controls.Add(new FileConditionNoneControl());
                    }
                } else if (type == DateTimeType.DateXxxStart || type == DateTimeType.DateEndXxx || type == DateTimeType.DateStartXxxEnd || type == DateTimeType.DateXxxStartEndXxx || type == DateTimeType.DateXxx) {
                    // 日付を指定
                    FileConditionDateControl dateControl;
                    if (panel.Controls.Count == 0) {
                        dateControl = new FileConditionDateControl(timeCondition);
                        panel.Controls.Add(dateControl);
                    } else if (panel.Controls[0] is FileConditionDateControl) {
                        dateControl = (FileConditionDateControl)(panel.Controls[0]);
                    } else {
                        Control oldControl = panel.Controls[0];
                        oldControl.Dispose();
                        dateControl = new FileConditionDateControl(timeCondition);
                        panel.Controls.Add(dateControl);
                    }
                    dateControl.Initialize(type);
                } else if (type == DateTimeType.RelativeXxxStart || type == DateTimeType.RelativeEndXxx || type == DateTimeType.RelativeStartXxxEnd || type == DateTimeType.RelativeXxxStartEndXxx || type == DateTimeType.RelativeXxx) {
                    // 日付を相対指定
                    FileConditionRelativeDateControl dateControl;
                    if (panel.Controls.Count == 0) {
                        dateControl = new FileConditionRelativeDateControl(timeCondition);
                        panel.Controls.Add(dateControl);
                    } else if (panel.Controls[0] is FileConditionRelativeDateControl) {
                        dateControl = (FileConditionRelativeDateControl)(panel.Controls[0]);
                    } else {
                        Control oldControl = panel.Controls[0];
                        oldControl.Dispose();
                        dateControl = new FileConditionRelativeDateControl(timeCondition);
                        panel.Controls.Add(dateControl);
                    }
                    dateControl.Initialize(type);
                } else {
                    // 時刻を指定
                    FileConditionTimeControl timeControl;
                    if (panel.Controls.Count == 0) {
                        timeControl = new FileConditionTimeControl(timeCondition);
                        panel.Controls.Add(timeControl);
                    } else if (panel.Controls[0] is FileConditionTimeControl) {
                        timeControl = (FileConditionTimeControl)(panel.Controls[0]);
                    } else {
                        Control oldControl = panel.Controls[0];
                        oldControl.Dispose();
                        timeControl = new FileConditionTimeControl(timeCondition);
                        panel.Controls.Add(timeControl);
                    }
                    timeControl.Initialize(type);
                }
            }

            //=========================================================================================
            // 機　能：サイズ条件のパネルの状態を更新する
            // 引　数：[in]comboBox       変更されたコンボボックスの実装
            // 　　　　[in]panel          条件を反映するパネル
            // 　　　　[in]sizeCondition  使用する条件の情報（m_conditionのフィールド）
            // 戻り値：なし
            //=========================================================================================
            public static void UpdateSizePanel(LasyComboBoxImpl comboBoxImpl, Panel panel, FileSizeCondition sizeCondition) {
                FileSizeType type = TypeComboBoxIndexConverter.IndexToFileSizeType(comboBoxImpl.SelectedIndex);
                sizeCondition.SizeType = type;
                if (type == FileSizeType.None) {
                    // 条件なし
                    if (panel.Controls.Count == 0) {
                        panel.Controls.Add(new FileConditionNoneControl());
                    } else if (panel.Controls[0] is FileConditionNoneControl) {
                        ;
                    } else {
                        Control oldControl = panel.Controls[0];
                        oldControl.Dispose();
                        panel.Controls.Add(new FileConditionNoneControl());
                    }
                } else {
                    // サイズを指定
                    FileConditionSizeControl sizeControl;
                    if (panel.Controls.Count == 0) {
                        sizeControl = new FileConditionSizeControl(sizeCondition);
                        panel.Controls.Add(sizeControl);
                    } else if (panel.Controls[0] is FileConditionSizeControl) {
                        sizeControl = (FileConditionSizeControl)(panel.Controls[0]);
                    } else {
                        Control oldControl = panel.Controls[0];
                        oldControl.Dispose();
                        sizeControl = new FileConditionSizeControl(sizeCondition);
                        panel.Controls.Add(sizeControl);
                    }
                    sizeControl.Initialize(type);
                }
            }

            //=========================================================================================
            // 機　能：日付の条件をUIから読み込む
            // 引　数：[in]parent         親ダイアログ
            // 　　　　[in]timeCondition  読み込んだ結果を反映する情報
            // 　　　　[in]statePanel     状態の表示パネル（エラー情報の読み込み元）
            // 　　　　[in]errorInput     入力された文字列にエラーがあったときに表示するメッセージ
            // 　　　　[in]errorReverse   入力結果が逆転していたときに表示するメッセージ
            // 戻り値：なし
            //=========================================================================================
            public static bool GetFileDate(Form parent, DateTimeCondition timeCondition, Panel statePanel, string errorInput, string errorReverse) {
                DateTimeType type = timeCondition.TimeType;

                // 入力データそのもののチェック
                if (statePanel.Controls.Count == 0 || statePanel.Controls[0] is FileConditionNoneControl) {
                    ;
                } else if (statePanel.Controls[0] is FileConditionDateControl) {
                    FileConditionDateControl control = (FileConditionDateControl)(statePanel.Controls[0]);
                    bool success = control.IsValidInput();
                    if (!success) {
                        InfoBox.Warning(parent, errorInput);
                        return false;
                    }
                } else if (statePanel.Controls[0] is FileConditionRelativeDateControl) {
                    FileConditionRelativeDateControl control = (FileConditionRelativeDateControl)(statePanel.Controls[0]);
                    bool success = control.IsValidInput();
                    if (!success) {
                        InfoBox.Warning(parent, errorInput);
                        return false;
                    }
                } else {
                    FileConditionTimeControl control = (FileConditionTimeControl)(statePanel.Controls[0]);
                    bool success = control.IsValidInput();
                    if (!success) {
                        InfoBox.Warning(parent, errorInput);
                        return false;
                    }
                }

                // 逆転のチェック
                bool valid = true;
                if (type == DateTimeType.None) {
                } else if (type == DateTimeType.DateXxxStart) {
                } else if (type == DateTimeType.DateEndXxx) {
                } else if (type == DateTimeType.DateStartXxxEnd) {
                    valid = (timeCondition.DateStart.Ticks < timeCondition.DateEnd.Ticks);
                } else if (type == DateTimeType.DateXxxStartEndXxx) {
                    valid = (timeCondition.DateStart.Ticks < timeCondition.DateEnd.Ticks);
                } else if (type == DateTimeType.DateXxx) {
                } else if (type == DateTimeType.RelativeXxxStart) {
                } else if (type == DateTimeType.RelativeEndXxx) {
                } else if (type == DateTimeType.RelativeStartXxxEnd) {
                    valid = ((timeCondition.RelativeStart > timeCondition.RelativeEnd) ||
                             (timeCondition.RelativeStart == timeCondition.RelativeEnd && timeCondition.TimeStart.ToIntValue() < timeCondition.TimeEnd.ToIntValue()));
                } else if (type == DateTimeType.RelativeXxxStartEndXxx) {
                    valid = ((timeCondition.RelativeStart > timeCondition.RelativeEnd) ||
                             (timeCondition.RelativeStart == timeCondition.RelativeEnd && timeCondition.TimeStart.ToIntValue() < timeCondition.TimeEnd.ToIntValue()));
                } else if (type == DateTimeType.RelativeXxx) {
                } else if (type == DateTimeType.TimeXxxStart) {
                } else if (type == DateTimeType.TimeEndXxx) {
                } else if (type == DateTimeType.TimeStartXxxEnd) {
                    valid = (timeCondition.TimeStart.ToIntValue() < timeCondition.TimeEnd.ToIntValue());
                } else if (type == DateTimeType.TimeXxxStartEndXxx) {
                    valid = (timeCondition.TimeStart.ToIntValue() < timeCondition.TimeEnd.ToIntValue());
                } else if (type == DateTimeType.TimeXxx) {
                } else {
                    Program.Abort("dateTypeの値が想定外です。");
                    return false;
                }
                if (!valid) {
                    InfoBox.Warning(parent, errorReverse);
                    return false;
                }
                return true;
            }

            //=========================================================================================
            // 機　能：サイズの条件をUIから読み込む
            // 引　数：[in]parent         親ダイアログ
            // 　　　　[in]sizeCondition  読み込んだ結果を反映する情報
            // 　　　　[in]statePanel     状態の表示パネル（エラー情報の読み込み元）
            // 　　　　[in]errorInput     入力された文字列にエラーがあったときに表示するメッセージ
            // 　　　　[in]errorReverse   入力結果が逆転していたときに表示するメッセージ
            // 戻り値：なし
            //=========================================================================================
            public static bool GetFileSize(Form parent, FileSizeCondition sizeCondition, Panel statePanel, string errorInput, string errorReverse) {
                FileSizeType type = sizeCondition.SizeType;

                // 入力データそのもののチェック
                if (statePanel.Controls.Count == 0 || statePanel.Controls[0] is FileConditionNoneControl) {
                    ;
                } else if (statePanel.Controls[0] is FileConditionSizeControl) {
                    FileConditionSizeControl control = (FileConditionSizeControl)(statePanel.Controls[0]);
                    bool success = control.IsValidInput();
                    if (!success) {
                        InfoBox.Warning(parent, errorInput);
                        return false;
                    }
                }

                // 逆転のチェック
                bool valid = true;
                if (type == FileSizeType.None) {
                } else if (type == FileSizeType.XxxSize) {
                } else if (type == FileSizeType.SizeXxx) {
                } else if (type == FileSizeType.SizeXxxSize) {
                    valid = (sizeCondition.MinSize < sizeCondition.MaxSize);
                } else if (type == FileSizeType.XxxSizeXxx) {
                    valid = (sizeCondition.MinSize < sizeCondition.MaxSize);
                } else if (type == FileSizeType.Size) {
                } else {
                    Program.Abort("sizeTypeの値が想定外です。");
                    return false;
                }
                if (!valid) {
                    InfoBox.Warning(parent, errorReverse);
                    return false;
                }
                return true;
            }

            //=========================================================================================
            // 機　能：ファイル名の条件を取得する
            // 引　数：[in]parent            親ダイアログ
            // 　　　　[in]comboBoxImplType  ファイル名種別のコンボボックスの実装
            // 　　　　[in]textBoxName       ファイル名入力用のテキストボックス
            // 　　　　[out]fileNameType     ファイル名種別を返す変数
            // 　　　　[out]fileName         ファイル名を返す変数
            // 戻り値：正常に取得できたときtrue
            //=========================================================================================
            public static bool GetFileNameCondition(Form parent, LasyComboBoxImpl comboBoxImplType, TextBox textBoxName, out FileNameType fileNameType, out string fileName) {
                fileNameType = TypeComboBoxIndexConverter.IndexToFileNameType(comboBoxImplType.SelectedIndex);
                if (fileNameType == FileNameType.None) {
                    fileName = null;
                } else {
                    fileName = textBoxName.Text;
                    if (fileName == "") {
                        InfoBox.Warning(parent, Resources.DlgFileCondition_ErrorFileName);
                        return false;
                    }
                    bool valid = TargetConditionComparetor.ValidateFileName(fileNameType, fileName);
                    if (!valid) {
                        InfoBox.Warning(parent, Resources.DlgFileCondition_ErrorFileNameRegex);
                        return false;
                    }
                }
                return true;
            }
        }

        //=========================================================================================
        // クラス：条件の種類の定数とコンボボックスのインデックスの間での変換クラス
        //=========================================================================================
        public class TypeComboBoxIndexConverter {

            //=========================================================================================
            // 機　能：対象の種類をコンボボックスのインデックスに変換する
            // 引　数：[in]type    対象の種類
            // 戻り値：コンボボックスのインデックス
            //=========================================================================================
            public static int FileConditionTargetToIndex(FileConditionTarget type) {
                if (type == FileConditionTarget.FileOnly) {
                    return 0;
                } else if (type == FileConditionTarget.FolderOnly) {
                    return 1;
                } else if (type == FileConditionTarget.FileAndFolder) {
                    return 2;
                } else {
                    Program.Abort("FileConditionTargetの値が不正です。");
                    return 0;
                }
            }

            //=========================================================================================
            // 機　能：コンボボックスのインデックスを対象の種類に変換する
            // 引　数：[in]index   コンボボックスのインデックス
            // 戻り値：対象の種類
            //=========================================================================================
            public static FileConditionTarget IndexToFileConditionTarget(int index) {
                switch (index) {
                    case 0:
                        return FileConditionTarget.FileOnly;
                    case 1:
                        return FileConditionTarget.FolderOnly;
                    case 2:
                        return FileConditionTarget.FileAndFolder;
                    default:
                        Program.Abort("FileConditionTargetインデックスの値が不正です。");
                        return FileConditionTarget.FileOnly;
                }
            }

            //=========================================================================================
            // 機　能：ファイル名の種類をコンボボックスのインデックスに変換する
            // 引　数：[in]type    ファイル名の種類
            // 戻り値：コンボボックスのインデックス
            //=========================================================================================
            public static int FileNameTypeToIndex(FileNameType type) {
                if (type == FileNameType.None) {
                    return 0;
                } else if (type == FileNameType.WildCard) {
                    return 1;
                } else if (type == FileNameType.RegularExpression) {
                    return 2;
                } else {
                    Program.Abort("FileNameTypeの値が不正です。");
                    return 0;
                }
            }

            //=========================================================================================
            // 機　能：コンボボックスのインデックスをファイル名の種類に変換する
            // 引　数：[in]index   コンボボックスのインデックス
            // 戻り値：ファイル名の種類
            //=========================================================================================
            public static FileNameType IndexToFileNameType(int index) {
                switch (index) {
                    case 0:
                        return FileNameType.None;
                    case 1:
                        return FileNameType.WildCard;
                    case 2:
                        return FileNameType.RegularExpression;
                    default:
                        Program.Abort("FileNameTypeインデックスの値が不正です。");
                        return FileNameType.None;
                }
            }

            //=========================================================================================
            // 機　能：ファイル日付の種類をコンボボックスのインデックスに変換する
            // 引　数：[in]type    ファイル日付の種類
            // 戻り値：コンボボックスのインデックス
            //=========================================================================================
            public static int DateTimeTypeToIndex(DateTimeType type) {
                if (type == DateTimeType.None) {
                    return 0;
                } else if (type == DateTimeType.DateXxxStart) {
                    return 1;
                } else if (type == DateTimeType.DateEndXxx) {
                    return 2;
                } else if (type == DateTimeType.DateStartXxxEnd) {
                    return 3;
                } else if (type == DateTimeType.DateXxxStartEndXxx) {
                    return 4;
                } else if (type == DateTimeType.DateXxx) {
                    return 5;
                } else if (type == DateTimeType.RelativeXxxStart) {
                    return 6;
                } else if (type == DateTimeType.RelativeEndXxx) {
                    return 7;
                } else if (type == DateTimeType.RelativeStartXxxEnd) {
                    return 8;
                } else if (type == DateTimeType.RelativeXxxStartEndXxx) {
                    return 9;
                } else if (type == DateTimeType.RelativeXxx) {
                    return 10;
                } else if (type == DateTimeType.TimeXxxStart) {
                    return 11;
                } else if (type == DateTimeType.TimeEndXxx) {
                    return 12;
                } else if (type == DateTimeType.TimeStartXxxEnd) {
                    return 13;
                } else if (type == DateTimeType.TimeXxxStartEndXxx) {
                    return 14;
                } else if (type == DateTimeType.TimeXxx) {
                    return 15;
                } else {
                    Program.Abort("DateTimeTypeの値が不正です。");
                    return 0;
                }
            }

            //=========================================================================================
            // 機　能：コンボボックスのインデックスをファイル日付の種類に変換する
            // 引　数：[in]index   コンボボックスのインデックス
            // 戻り値：ファイル日付の種類
            //=========================================================================================
            public static DateTimeType IndexToDateTimeType(int index) {
                switch (index) {
                    case 0:
                        return DateTimeType.None;
                    case 1:
                        return DateTimeType.DateXxxStart;
                    case 2:
                        return DateTimeType.DateEndXxx;
                    case 3:
                        return DateTimeType.DateStartXxxEnd;
                    case 4:
                        return DateTimeType.DateXxxStartEndXxx;
                    case 5:
                        return DateTimeType.DateXxx;
                    case 6:
                        return DateTimeType.RelativeXxxStart;
                    case 7:
                        return DateTimeType.RelativeEndXxx;
                    case 8:
                        return DateTimeType.RelativeStartXxxEnd;
                    case 9:
                        return DateTimeType.RelativeXxxStartEndXxx;
                    case 10:
                        return DateTimeType.RelativeXxx;
                    case 11:
                        return DateTimeType.TimeXxxStart;
                    case 12:
                        return DateTimeType.TimeEndXxx;
                    case 13:
                        return DateTimeType.TimeStartXxxEnd;
                    case 14:
                        return DateTimeType.TimeXxxStartEndXxx;
                    case 15:
                        return DateTimeType.TimeXxx;
                    default:
                        Program.Abort("DateTimeTypeインデックスの値が不正です。");
                        return DateTimeType.None;
                }
            }

            //=========================================================================================
            // 機　能：ファイルサイズの種類をコンボボックスのインデックスに変換する
            // 引　数：[in]type    ファイルサイズの種類
            // 戻り値：コンボボックスのインデックス
            //=========================================================================================
            public static int FileSizeTypeToIndex(FileSizeType type) {
                if (type == FileSizeType.None) {
                    return 0;
                } else if (type == FileSizeType.XxxSize) {
                    return 1;
                } else if (type == FileSizeType.SizeXxx) {
                    return 2;
                } else if (type == FileSizeType.SizeXxxSize) {
                    return 3;
                } else if (type == FileSizeType.XxxSizeXxx) {
                    return 4;
                } else {
                    Program.Abort("FileSizeTypeの値が不正です。");
                    return 0;
                }
            }

            //=========================================================================================
            // 機　能：コンボボックスのインデックスをファイルサイズの種類に変換する
            // 引　数：[in]index   コンボボックスのインデックス
            // 戻り値：ファイルサイズの種類
            //=========================================================================================
            public static FileSizeType IndexToFileSizeType(int index) {
                switch (index) {
                    case 0:
                        return FileSizeType.None;
                    case 1:
                        return FileSizeType.XxxSize;
                    case 2:
                        return FileSizeType.SizeXxx;
                    case 3:
                        return FileSizeType.SizeXxxSize;
                    case 4:
                        return FileSizeType.XxxSizeXxx;
                    default:
                        Program.Abort("FileSizeTypeインデックスの値が不正です。");
                        return FileSizeType.None;
                }
            }

            //=========================================================================================
            // 機　能：属性のフラグ状態をコンボボックスのインデックスに変換する
            // 引　数：[in]flag  属性のフラグ
            // 戻り値：コンボボックスのインデックス
            //=========================================================================================
            public static int AttributeStateToIndex(BooleanFlag flag) {
                if (flag == null) {
                    return 0;
                } else if (flag.Value == true) {
                    return 1;
                } else {
                    return 2;
                }
            }

            //=========================================================================================
            // 機　能：コンボボックスのインデックスを属性のフラグ状態に変換する
            // 引　数：[in]index   コンボボックスのインデックス
            // 戻り値：ファイルサイズの種類
            //=========================================================================================
            public static BooleanFlag IndexToAttributeState(int index) {
                switch (index) {
                    case 0:
                        return null;
                    case 1:
                        return new BooleanFlag(true);
                    case 2:
                        return new BooleanFlag(false);
                    default:
                        Program.Abort("Attributeインデックスの値が不正です。");
                        return null;
                }
            }

            //=========================================================================================
            // プロパティ：対象種別の項目一覧
            //=========================================================================================
            public static string[] TargetItems {
                get {
                    string[] targetItems = new string[] {
                        Resources.DlgTransferCond_CondTargetFileOnly,
                        Resources.DlgTransferCond_CondTargetFolderOnly,
                        Resources.DlgTransferCond_CondTargetFileAndFolder,
                    };
                    return targetItems;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル名指定の項目一覧
            //=========================================================================================
            public static string[] FileNameItems {
                get {
                    string[] fileNameItems = new string[] {
                        Resources.DlgTransferCond_CondFileNameNone,
                        Resources.DlgTransferCond_CondFileNameWildCard,
                        Resources.DlgTransferCond_CondFileNameRegularExpression,
                    };
                    return fileNameItems;
                }
            }

            //=========================================================================================
            // プロパティ：日付条件の項目一覧
            //=========================================================================================
            public static string[] TimeItems {
                get {
                    string[] timeItems = new string[] {
                        Resources.DlgTransferCond_CondTimeNone,
                        Resources.DlgTransferCond_CondTimeDateXxxStart,
                        Resources.DlgTransferCond_CondTimeDateEndXxx,
                        Resources.DlgTransferCond_CondTimeDateStartXxxEnd,
                        Resources.DlgTransferCond_CondTimeDateXxxStartEndXxx,
                        Resources.DlgTransferCond_CondTimeDateXxx,
                        Resources.DlgTransferCond_CondTimeRelativeXxxStart,
                        Resources.DlgTransferCond_CondTimeRelativeEndXxx,
                        Resources.DlgTransferCond_CondTimeRelativeStartXxxEnd,
                        Resources.DlgTransferCond_CondTimeRelativeXxxStartEndXxx,
                        Resources.DlgTransferCond_CondTimeRelativeXxx,
                        Resources.DlgTransferCond_CondTimeTimeXxxStart,
                        Resources.DlgTransferCond_CondTimeTimeEndXxx,
                        Resources.DlgTransferCond_CondTimeTimeStartXxxEnd,
                        Resources.DlgTransferCond_CondTimeTimeXxxStartEndXxx,
                        Resources.DlgTransferCond_CondTimeTimeXxx,
                    };
                    return timeItems;
                }
            }

            //=========================================================================================
            // プロパティ：ファイルサイズの項目一覧
            //=========================================================================================
            public static string[] FileSizeItems {
                get {
                    string[] sizeItems = new string[] {
                        Resources.DlgTransferCond_CondSizeNone,
                        Resources.DlgTransferCond_CondSizeXxxSize,
                        Resources.DlgTransferCond_CondSizeSizeXxx,
                        Resources.DlgTransferCond_CondSizeSizeXxxSize,
                        Resources.DlgTransferCond_CondSizeXxxSizeXxx,
                    };
                    return sizeItems;
                }
            }

            //=========================================================================================
            // プロパティ：属性の項目一覧
            //=========================================================================================
            public static string[] AttributeItems {
                get {
                    string[] attributeItems = new string[] {
                        Resources.DlgTransferCond_CondAttrNone,
                        Resources.DlgTransferCond_CondAttrOn,
                        Resources.DlgTransferCond_CondAttrOff,
                    };
                    return attributeItems;
                }
            }
        }
    }
}
