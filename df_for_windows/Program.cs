using System;
using System.Collections.Generic;
using System.IO;

namespace df_for_windows
{
    class Program
    {
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
            Output(Analyze());

            Console.ReadLine();
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
        /// 出力します。
        /// </summary>
        /// <param name="driveInfos">出力ドライブ情報</param>
        private static void Output(string[] driveInfos)
        {
            // Header
            Console.WriteLine("Drive Type     Total    Used    Free    % ValumeName");
            Console.WriteLine("----- ------ ------- ------- ------- ---- ----------");

            // Body
            for (var i = 0; i < driveInfos.Length; i++)
                Console.WriteLine(driveInfos[i]);
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
