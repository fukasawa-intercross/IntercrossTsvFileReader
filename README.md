# IntercrossTsvFileReader

intercross-311 DAQ MasterのTSV保存ファイルを読み取るサンプルアプリケーション

# ビルド方法

VisualStudio 2019 を使用してビルドしてください。
https://visualstudio.microsoft.com/ja/downloads/

# 使用方法
## 既存TSVファイルを読み取る場合

1. アプリケーションを起動します。
1. 読み取るTSVファイルパスを入力して、`Enter`を押してください。
1. TSVファイルの内容がコンソールに出力されます。

## 自動保存TSVファイルをリアルタイムに読み取る場合

0. (intercross-311 DAQMaterで自動保存形式をTSVにして、計測を開始します。)
1. アプリケーションを起動します。
2. 読み取るTSVファイルパスに何も入力せず、`Enter`を押してください。
3. 受信データのTSVファイルの内容がリアルタイムにコンソールに出力されます。

# 出力方法の変更

サンプルアプリケーションでは読み取った内容をコンソールに出力しているだけです。
読みとったデータを元に加工や保存方法を変更したい場合は、以下のコンソールへ出力している部分を書き換えてください。

https://github.com/intercross-corp/IntercrossTsvFileReader/blob/92092167892e3fc2c2859219e56defa47a5c8a7f/Csharp/IntercrossTsvFileReader/Program.cs#L51



