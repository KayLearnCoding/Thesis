using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sort
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start, end;
            start = DateTime.Parse("2018/8/8 15:35:00");

            string ReadFile = @"G:\research\data\journey.csv";
            string WriteFile = @"G:\research\data\journey_sorted.csv";
            //ReadAndSort(ReadFile, WriteFile);
            
            end = DateTime.Parse("2018/8/8 19:20:00");
            Console.WriteLine("Total time: {0}", TimeDiff(start, end));
            Console.ReadKey();
        }

        private static void ReadAndSort(string readFile, string writeFile)
        {
            //读文件
            List<double> arr = new List<double>();
            StreamReader sr = new StreamReader(readFile, Encoding.Default); string line = sr.ReadLine();
            //读取第一遍原始数据
            while ((line = sr.ReadLine()) != null)
            {
                string[] oneRecord = line.Split(',');
                double longitute = Convert.ToDouble(oneRecord[5]);
                arr.Add(longitute);
            }

            //排序
            int len = arr.Count;
            int preIndex;
            double current;
            for (int i = 1; i < len; i++)
            {
                preIndex = i - 1;
                current = arr[i];
                while (preIndex >= 0 && arr[preIndex] > current)
                {
                    arr[preIndex + 1] = arr[preIndex];
                    preIndex--;
                }
                arr[preIndex + 1] = current;
            }

           
            foreach (var item in arr)
            {
               // Console.WriteLine(item.ToString());
            }
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
