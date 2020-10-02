# IntercrossTsvFileReader

intercross-311 DAQ MasterのTSV保存ファイルの読み取りサンプルアプリケーション

# ビルド方法

VisualStudio 2019 を使用してビルドしてください。
https://visualstudio.microsoft.com/ja/downloads/

# 使用方法

## 既存TSVファイルを読み取る場合

1. アプリケーションを起動します。
2. 読み取るTSVファイルパスを入力して、`Enter`を押してください。
3. TSVファイルの内容がコンソールに出力されます。

## 自動保存TSVファイルをリアルタイムに読み取る場合

(0.) intercross-311 DAQMaterで自動保存形式をTSVにして、計測を開始します。
1. アプリケーションを起動します。
2. 読み取るTSVファイルパスに何も入力せず、`Enter`を押してください。
3. 受信データのTSVファイルの内容がリアルタイムにコンソールに出力されます。





