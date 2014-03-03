using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace df_for_windows
{
    class Program
    {
        /// <summary>
        /// ヘッダ
        /// </summary>
        private const string HEADER = "Drive Type     Total    Used    Free    % ValumeName";

        /// <summary>
        /// セパレータ
        /// </summary>
        private const string SEPARATOR = "----- ------ ------- ------- ------- ---- ----------";

        /// <summary>
        /// 準備済ドライブの出力フォーマット
        /// </summary>
        private const string READY_FORMAT = "{0, -5} {1, -6} {2, 7} {3, 7} {4, 7} {5, 4} {6}";

        /// <summary>
        /// 未準備ドライブの出力フォーマット
        /// </summary>
        private const string UNREADY_FORMAT = "{0, -5} {1, -6}       -       -       -    -";

        /// <summary>
        /// メイン
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // 動作タイプ
            var type = args.Length == 0 ? "-c" : args[0];

            // ドライブ情報解析
            var analyzed = Analyze();

            switch (type)
            {
                case "-c":
                    OutputToConsole(analyzed);
                    break;

                case "-t":
                    var path = GetPath(args);
                    OutputToFile(analyzed, path);
                    break;

                default: new ArgumentException("無効な引数です。"); break;
            }

            //Console.Write("ドライブ情報の解析が完了しました。キー入力で終了します。");
            //Console.ReadLine();
        }

        /// <summary>
        /// ドライブを解析します。
        /// </summary>
        /// <returns>出力ドライブ情報</returns>
        private static string[] Analyze()
        {
            var driveInfos = new List<string>();

            foreach (var drive in DriveInfo.GetDrives())
            {
                var letter = drive.Name.Substring(0, 2);
                var type = drive.DriveType.ToString();

                if (drive.IsReady)
                {
                    var usedSize = drive.TotalSize - drive.TotalFreeSpace;

                    var total = drive.TotalSize.BytesToGigabytes() + "GB";
                    var used = usedSize.BytesToGigabytes() + "GB";
                    var free = drive.TotalFreeSpace.BytesToGigabytes() + "GB";
                    var usage = CalcUsage(usedSize, drive.TotalSize) + "%";
                    var volume = drive.VolumeLabel;

                    driveInfos.Add(string.Format(
                        READY_FORMAT, letter, type, total, used, free, usage, volume));
                }
                else
                {
                    driveInfos.Add(string.Format(
                        UNREADY_FORMAT, letter, type));
                }
            }

            return driveInfos.ToArray();
        }

        /// <summary>
        /// 使用割合を算出します。
        /// </summary>
        /// <param name="usedSize">使用容量</param>
        /// <param name="totalSize">全体容量</param>
        /// <returns>使用割合</returns>
        private static int CalcUsage(double usedSize, double totalSize)
        {
            return (int)(usedSize / totalSize * 100);
        }

        /// <summary>
        /// ドライブ情報をコンソールに出力します。
        /// </summary>
        /// <param name="driveInfos">ドライブ情報</param>
        private static void OutputToConsole(string[] driveInfos)
        {
            // Header
            Console.WriteLine(HEADER);
            Console.WriteLine(SEPARATOR);

            // Body
            for (var i = 0; i < driveInfos.Length; i++)
                Console.WriteLine(driveInfos[i]);
        }

        /// <summary>
        /// ドライブ情報をファイルに出力します。
        /// </summary>
        /// <param name="driveInfos">ドライブ情報</param>
        /// <param name="path">出力ファイルパス</param>
        private static void OutputToFile(string[] driveInfos, string path)
        {
            // フォルダの存在チェック＆作成
            var fi = new FileInfo(path);
            if (!fi.Directory.Exists) fi.Directory.Create();

            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                // HEADER
                sw.WriteLine(HEADER);
                sw.WriteLine(SEPARATOR);

                // BODY
                for (var i = 0; i < driveInfos.Length; i++)
                    sw.WriteLine(driveInfos[i]);
            }
        }

        /// <summary>
        /// コマンドライン引数から出力パスを取得します。
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        /// <returns>出力パス</returns>
        private static string GetPath(string[] args)
        {
            return args.Length < 2 ? @"c:\temp\df.txt" : args[1];
        }
    }

    /// <summary>
    /// 拡張メソッド群
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// byte から GB に変換します。
        /// </summary>
        /// <param name="bytes">byte値</param>
        /// <returns>GB値</returns>
        public static double BytesToGigabytes(this long bytes)
        {
            double gb = bytes / (1024 * 1024 * 1024);
            return Math.Round(gb, 2, MidpointRounding.AwayFromZero);
        }
    }
}
