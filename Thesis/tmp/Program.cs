using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tmp
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start, end;

            Console.WriteLine("Ceiling 2: " + Math.Ceiling(2.0).ToString());
            Console.WriteLine("Floor 2: " + Math.Floor(2.0).ToString());
            Console.WriteLine("Ceiling 2.5: " + Math.Ceiling(2.5).ToString());
            Console.WriteLine("Floor 2.5 " + Math.Floor(2.5).ToString());


            start = DateTime.Now;

            Console.WriteLine("Read disc now!");
            //ReadDisc(@"E:\DATA\TAXI\for_canteen_results\08AllCar");
            end = DateTime.Now;
            Console.WriteLine("Read disc time : {0}", TimeDiff(start, end));
            
            //ReadFile(@"G:\testData\0801-07");
            start = DateTime.Now;
            Console.WriteLine("Read memory now!");
            //ReadMemory(@"E:\DATA\TAXI\for_canteen_results\07AllCar");
            end = DateTime.Now;
            Console.WriteLine("Read memory time : {0}", TimeDiff(start, end));


            Console.ReadKey();
            //showData();
            //string date;
            //for (int i = 1; i < 8; i++)
            //{
            //    if (i < 10)
            //        date = "0" + i;
            //    else
            //        date = i.ToString();
            //    MergeDate(date);
            //}

        }

        private static void ReadFile(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default); string line;int lineN = 0;
            //读取第一遍原始数据
            while ((line = sr.ReadLine()) != null)
            {
                string[] oneRecord = line.Split('|');
                if (lineN%10000==0)
                {
                    Console.WriteLine(line);
                }
                lineN++;
            }
        }

        static void ReadMemory(string originalDataFolder)
        {
            string[] names = Directory.GetFiles(originalDataFolder); int TotalLine = 0;
            List<string> allLines = new List<string>();
            //read
            for (int pathIndex = 0; pathIndex < names.Length; pathIndex++)  // 读取进dictionary
            {
                string path = names[pathIndex];
                //Console.WriteLine("将原数据塞入各自id的txt，读取进度： " + path);
                StreamReader sr = new StreamReader(path, Encoding.Default); string line;
                while ((line = sr.ReadLine()) != null)
                {
                    allLines.Add(line);
                }
            }
            //read
            for (int i = 0; i < allLines.Count; i++)
            {
                if (i % 100000 == 0)
                {
                    Console.WriteLine("{0} line is {1}", i, allLines[i]);
                }
            }

        }
        static void ReadDisc(string originalDataFolder)
        {
            string[] names = Directory.GetFiles(originalDataFolder);
            List<string> allLines = new List<string>();
            //read
            for (int pathIndex = 0; pathIndex < names.Length; pathIndex++)  // 读取进dictionary
            {
                string path = names[pathIndex];
                //Console.WriteLine("将原数据塞入各自id的txt，读取进度： " + path);
                StreamReader sr = new StreamReader(path, Encoding.Default); string line;
                while ((line = sr.ReadLine()) != null)
                {
                    allLines.Add(line);
                }
            }
            int TotalLine = 0;
            //read
            for (int pathIndex = 0; pathIndex < names.Length; pathIndex++)  // 读取进dictionary
            {
                string path = names[pathIndex];
                //Console.WriteLine("将原数据塞入各自id的txt，读取进度： " + path);
                StreamReader sr = new StreamReader(path, Encoding.Default); string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (TotalLine % 100000 == 0)
                    {
                        Console.WriteLine("{0} line is {1}", TotalLine, line);
                    }
                    TotalLine++;
                }
            }

        }
        static void showData()
        {
            string ReadPath = @"G:\taxi_data\Day1to5";
            StreamReader sr = new StreamReader(ReadPath, Encoding.Default);
            String line; int lineNum = 0;
            while ((line = sr.ReadLine()) != null)
            {
                if (lineNum % 10000 == 0)
                    Console.WriteLine(line.ToString());
                lineNum++;
            }
        }
        static void testMerge(string dateStr)
        {
            string WritePath = @"G:\taxi_data\Day1toDay5";
            FileStream fs = new FileStream(WritePath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(dateStr);
            sw.Flush();
            sw.Close(); fs.Close();
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
        static void MergeDate(string dateStr)
        {
            string ReadPath = @"G:\taxi_data\SODA\" + dateStr;
            string WritePath = @"G:\taxi_data\Day1-7";
            //string twoHourPath = @"G:\老师的比赛\data\old_index\7-8";
            string[] HourPaths = Directory.GetDirectories(ReadPath);// Directory.GetFiles(ReadPath);
            FileStream fs = new FileStream(WritePath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);

            for (int pathIndex = 0; pathIndex < HourPaths.Length; pathIndex++)
            {
                string oneHourPath = HourPaths[pathIndex];
                Console.WriteLine("正在合并第" + dateStr + "天，第" + oneHourPath.Substring(oneHourPath.Length - 2, 2) + "小时");
                string[] filePath = Directory.GetFiles(oneHourPath);

                //遍历小时文件夹，读每一分钟的txt数据
                //int wrongTimeCount = 0;
                for (int filePerMinuteIndex = 0; filePerMinuteIndex < filePath.Length; filePerMinuteIndex++)
                {
                    StreamReader sr = new StreamReader(filePath[filePerMinuteIndex], Encoding.Default);             //导入一个txt文件
                    string line; int lineNum = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineNum++;
                        string[] oneRecord = line.Split('|');
                        string GPStime = oneRecord[9];

                        if (GPStime.Length < "2016-08-01 11:00:00".Length)
                            continue;
                        //确保是当天的数据
                        if (DateTime.Parse(GPStime) >= DateTime.Parse("2016-08-" + dateStr + " 00:00:00") && DateTime.Parse(GPStime) <= DateTime.Parse("2016-08-" + dateStr + " 23:59:59"))
                        {
                            sw.WriteLine(line);
                            //Console.WriteLine(line);
                        }

                        if (lineNum % 50000 == 0)
                            Console.WriteLine("current line: {0}", filePath[filePerMinuteIndex]);
                    }
                    sw.WriteLine("\r\n");
                }
                //Console.WriteLine("第" + pathIndex.ToString() + "小时，时间错误数据共 " + wrongTimeCount.ToString() + "条");
            }

            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close(); fs.Close();
        }
    }
}
