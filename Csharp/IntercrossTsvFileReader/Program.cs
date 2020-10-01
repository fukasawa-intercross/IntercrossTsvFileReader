using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntercrossTsvFileReader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"IntercrossTsvFileReader start");

            Console.WriteLine("読み取るTSVファイルパスを入力して、`Enter`を押してください。");
            Console.WriteLine("何も入力しなかった場合は、DAQMasterの自動保存場所の最新のTSVファイルを読み取ります。");
            Console.WriteLine("指定されたファイルを監視して、読み取り、内容を出力します。");
            Console.WriteLine("終了するためには、画面を閉じてください。");
            Console.Write("> ");

            //ターゲットファイルパスを標準入力から受け取る
            string inputFilePath = Console.ReadLine();

            //余分な文字を除く
            inputFilePath = inputFilePath.Trim('"').Trim();

            //入力された文字列が有効でなかったら、自動で現在保存中のバックアップファイルにする
            if (String.IsNullOrWhiteSpace(inputFilePath))
                inputFilePath = GetLastBackupTsvFilePath();

            //存在しないファイルだったら終了
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine($"指定されたファイルは存在しません。Path = {inputFilePath}");
                Console.WriteLine("なにかキーを押したら、終了します。");
                Console.ReadKey();
                return;
            }

            //読み取り専用＆他から書き込み可でファイルを開く
            using (var fileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                //無限ループ
                while (true)
                {
                    //ファイルからTSVデータを逐次的に読み取る
                    foreach (TsvElement tsvElement in ReadTsvFile(fileStream))
                        Console.WriteLine(tsvElement.ToString());

                    //ファイルを最後まで読み取ったら、100msec待つ
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }
            }
        }

        /// <summary>
        /// 自動保存場所の最新のTSVファイルパスを返す
        /// </summary>
        private static string GetLastBackupTsvFilePath()
        {
            string targetDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\IntercrossData\SensorMaster\";

            return Directory.GetFiles(targetDirectoryPath, "*.tsv")
                 .Select(x => new FileInfo(x))
                 .OrderBy(x => x.LastWriteTime)
                 .LastOrDefault()?.FullName;
        }

        /// <summary>
        /// Streamから逐次的にTSVクラスを読み取る
        /// </summary>
        private static IEnumerable<TsvElement> ReadTsvFile(Stream stream)
        {
            var encoding = Encoding.GetEncoding("Shift_JIS");

            while (true)
            {
                string lineText = ReadLineEx(stream, encoding);

                var tsvElement = TsvElement.FromText(lineText);

                if (tsvElement == null)
                    break;

                yield return tsvElement;
            }
        }

        /// <summary>
        /// Streamから1行分読み取り、その分Streamを勧める
        /// </summary>
        /// <remarks>StreamReaderのReadLineと異なり、ファイルの最後が改行で終わってなければ、その行を無視して元のPositionに戻す</remarks>
        /// <returns>読み取った文字列 読み取れなかった場合はnull</returns>
        private static string ReadLineEx(Stream stream, Encoding encoding)
        {
            long startPosition = stream.Position;

            var textBytes = new List<byte>();

            while (true)
            {
                int character = stream.ReadByte();

                //ファイルの最後まで読み取った場合は-1になる、その場合はnullを返し、StreamのPositionを元に戻す
                if (character < 0 || character > 255)
                {
                    stream.Position = startPosition;
                    return null;
                }

                byte bCharacter = (byte)character;
                const char endLineChar = '\n';

                if (bCharacter == endLineChar)
                    return encoding.GetString(textBytes.ToArray());

                textBytes.Add(bCharacter);
            }
        }
    }

    /// <summary>
    /// intercross-311 TSV形式ファイルの1行分のデータクラス
    /// </summary>
    internal class TsvElement
    {
        /// <summary>
        /// データ種類
        /// </summary>
        public string TypeSensor { get; set; }
        /// <summary>
        /// 時刻
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Chインデックス(1始まり)
        /// </summary>
        public int Ch { get; set; }

        /// <summary>
        /// 計測値
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 文字列からTsvElementクラスを生成する。
        /// </summary>
        /// <param name="lineText">入力文字列 タブ区切りでセンサー種類、時刻、ch、値が含まれたもの 例）"Analog (TAB) 0.01 (TAB) 1 (TAB) 12.4.."</param>
        /// <returns>変換されたTSVクラス 変換できなかった場合はnull</returns>
        public static TsvElement FromText(string lineText)
        {
            var texts = lineText?.Split('\t');
            if (texts?.Length != 4)
                return null;

            if (String.IsNullOrWhiteSpace(texts[0]))
                return null;

            string typeSensor = texts[0];

            if (!Double.TryParse(texts[1], out double time))
                return null;

            if (!Int32.TryParse(texts[2], out int ch))
                return null;


            if (!Double.TryParse(texts[3], out double value))
                return null;

            return new TsvElement
            {
                TypeSensor = typeSensor,
                Time = time,
                Ch = ch,
                Value = value,
            };
        }

        public override string ToString() =>
            $"{this.TypeSensor} T:{this.Time:F3} Ch:{this.Ch} V:{this.Value:F2}";
    }
}
