# ShellFiler

## 概要

ShellFilerはDOS時代の2画面ファイラーの流れを引き継いだWindows用のファイル管理ソフトです。

左右2画面でファイルを表示してコピーなどのファイル操作を実行できるほか、内蔵ファイルビューアやグラフィックビューアを使って軽快なファイル操作を行えます。

ShellFilerはWindows PC上のファイルを軽快に操作できるだけでなく、SSHクライアントとしても利用できます。LinuxなどのSSHサーバーにあるファイルも、PC上のファイルと同様に操作できます。

[機能の詳細](Doc/README.md)

## ダウンロード

Windows 10以降の64bit環境に対応しています。

安定版：[shellfiler3.0.2.msi](https://github.com/hideakiksm/shellfiler-release/raw/main/release/shellfiler3.0.2.msi)

旧バージョン（[更新履歴](Doc/history.md)）：

* [shellfiler3.0.1.msi](https://github.com/hideakiksm/shellfiler-release/raw/main/release/shellfiler3.0.1.msi)
* [shellfiler3.0.0.msi](https://github.com/hideakiksm/shellfiler-release/raw/main/release/shellfiler3.0.0.msi)

オープンソース版では、それ以前のキー設定とメニュー設定は引き継ぐことができません。

Ver.3.0.0以前のバージョンは一旦アンインストールすることをおすすめします。

## 連携ソフトウェア

ShellFiler単体でも使用できますが、ダウンロード後は連携ソフトウェアをインストールすることをおすすめします。

* [SharpSSH](https://ja.osdn.net/projects/sfnet_sharpssh/)

    SharpSSHをインストールすると、ShellFilerからSSH/SFTPプロトコルを使って、LinuxなどのSSHサーバーにあるファイルを操作することができます。

* [7-Zip](https://sevenzip.osdn.jp/)

    7-Zipをインストールすると、ShellFilerから圧縮ファイルの作成と展開ができるようになります。

* [WinMerge](https://winmerge.org/?lang=ja)

    お好みの差分表示ツールをインストールすると、ファイルやフォルダの差分を簡単に確認できます。
    WinMerge以外にもコマンドラインに比較対象のファイルを指定すると差分が比較できるツール全般に対応しています。
    メニューの[オプション-オプション]の[インストール情報＞差分表示ツール]で設定します。

## アンインストールと設定のクリア

ShellFilerの設定情報は以下に格納されています。

アンインストーラでは、インストール時に転送したファイル以外は削除されません。
再インストールする場合は手順3のファイルを削除すると設定をクリアできます。

1. すべてのShellFilerを終了します。

2. Windowsの設定（コントロールパネル）からアプリ一覧のShellFilerをアンインストールしてます。

3. インストーラ外で書き込まれる以下の３つのフォルダを削除します。

   C:\Program Files\ShellFiler
   SharpSSHなどを手動でコピーすると、フォルダが残ることがあります。

   C:\Users\○○\AppData\Local\ShellFiler
    内部処理でテンポラリとして使用します。

   C:\Users\○○\AppData\Roaming\ShellFiler
   設定ファイルが残ります。削除すると設定がリセットされます。

## ビルド方法

Visual Studio 2022でのビルドに対応しています。

cloneしたあと、`Project`フォルダの.slnファイルを開きます。

* ShellFiler\Project\bin\Debug\（出力フォルダ）にSharpSSHのDLLをコピーしておきます。または、ShellFilerのプロジェクトの右クリック後に[追加][参照]から、別途インストールしたTamir.SharpSSH.dllを追加します。
* ビルドするとReleaseフォルダに実行ファイルができます。
* 本体が`Project\ShellFiler`、C#で実現できない機能を提供するDLLを作成するC++プロジェクトが`Project\SfHelper`です。
* `Util_CommandXmlConverter`は、キーやメニューに割り当てる機能の説明をソースコードから自動生成するツールです。`Project\ShellFiler\src\Command`フォルダにあるC#ソースのコメントから`Project\release\CommandList.dat`を作成します。コマンドを更新した場合に実行します。
* `Util_ImageCombine`は`Project\ShellFiler\Resources`ツールバー用のイメージリストを作成するツールです。ツールバーのイメージを更新した場合に実行します。

## オープンソースについて

苦情対応に追われるのは本意ではないため、当面、Issueは無効化しておきます。

PRは歓迎します。スタイル等は既存のものに合わせてください（C#の標準と乖離していますが、乖離こそがShellFiler）。ご希望される場合はタイトル等でContributorとしてご紹介します。

## 関連ソフトウェア

ShellFilerは以下のライブラリを使用しています。
エラー処理や64bit化のため、一部改変して使用しています。
改変したソースもリポジトリに含まれています。

利用条件の詳細については、インストール後のCredit.txtをご覧ください。

- SpHeader

  Copyright (c) 2004 by Sergei Pavlovsky

  http://www.codeproject.com/KB/miscctrl/CS_Header_Control.aspx

- C# (.net) interface for 7-Zip archive dlls

  Copyright 2008 by Eugene Sichkar

  http://www.codeproject.com/KB/DLL/cs_interface_7zip.aspx

---

Copyright 2011-2022 Hideaki Kishimoto
mail@kissy-software.com
![https://twitter.com/hideaki_ksm](Doc/twitter.png)[@hideaki_ksm](https://twitter.com/hideaki_ksm)
