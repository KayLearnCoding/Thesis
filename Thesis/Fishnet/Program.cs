using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fishnet
{
    class Program
    {
        static void Main(string[] args)
        {
            string ReadPath = @"E:\DATA\TAXI\for_canteen_results\Fishnet";
            string WritePath = @"E:\DATA\TAXI\for_canteen_results\Fishnet\FishnetTimeAnalysis\";
            int LineNum = 92;
            DateTime start, end; start = DateTime.Now;

            ReadFishnetTxts(ReadPath, WritePath, LineNum);

            end = DateTime.Now;
            Console.WriteLine("time duration : {0}", TimeDiff(start, end));
            //Console.ReadKey();
        }   

        private static void ReadFishnetTxts(string readPath, string writePath, int lineNum)
        {
            FileStream fs = new FileStream(writePath + "Line" + lineNum.ToString() + ".txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string[] names = Directory.GetFiles(readPath);
            for (int i = 0; i < names.Length; i++)
            {
                string[] oneTxt = File.ReadAllLines(names[i]);      //读入一个文件的所有行
                string line = oneTxt[lineNum];                      //5006,121.41,31.14,11
                string taxiNum = line.Split(',')[3];
                sw.WriteLine(taxiNum);
            }
            sw.Flush(); sw.Close(); fs.Close();
        }

        static string TimeDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                dateDiff = ts.Days.ToString() + "天"
                        + ts.Hours.ToString() + "小时"
                        + ts.Minutes.ToString() + "分钟"
                        + ts.Seconds.ToString() + "秒";
            }
            catch
            {

            }
            return dateDiff;
        }
    }
}
