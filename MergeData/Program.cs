using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Thesis;

namespace MergeData
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start, end;
            start = DateTime.Now;

            string writePath = @"G:\research\data\data lion_taxi_data\journey-new.csv";
            //string writePath = @"G:\research\result\journey.csv";
            

            string file1 = @"G:\research\result\week1.csv", file2 = @"G:\research\result\www2.csv";

            MergeFile(file1, file2, writePath);
            //MergeFolder(15, 28, writePath);
            end = DateTime.Now;

            Console.Write("Total time: " + TimeDiff(start, end));
            Console.ReadKey();
        }

        private static void ShowData(string filePath)
        {
            StreamReader sr = new StreamReader(filePath); string line = sr.ReadLine(); int lineNum = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] oneRecord = line.Split(',');
                string time = oneRecord[2];
                try
                {
                    if (DateTime.Parse(time) > DateTime.Parse("2016-08-10 00:00:00"))
                    {
                        lineNum++;
                        Console.WriteLine(line);
                    }
                }
                catch
                {

                }
            }
        }

        private static void MergeFile(string file1Path, string file2Path, string mergedFilePath)
        {
            FileStream fs = new FileStream(mergedFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            Console.WriteLine("Current file: {0}", file1Path);
            StreamReader sr = new StreamReader(file1Path, Encoding.Default); string line;
            List<string> allRecord = new List<string>();
            //读取第一个文件
            while ((line = sr.ReadLine()) != null)
                allRecord.Add(line);
            //read one file, write one file
            for (int i = 0; i < allRecord.Count; i++)
            {
                sw.WriteLine(allRecord[i]);
            }
            allRecord.Clear();
            
            //读取第二个文件
            sr = new StreamReader(file2Path, Encoding.Default); 
            Console.WriteLine("Current file: {0}", file2Path);
            while ((line = sr.ReadLine()) != null)
                allRecord.Add(line);
            //read one file, write one file
            for (int i = 0; i < allRecord.Count; i++)
            {
                sw.WriteLine(allRecord[i]);
            }

            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close(); fs.Close();
        }

        private static void MergeFolder(int StartDate, int EndDate, string writePath)
        {
            FileStream fs = new FileStream(writePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            // 这一层为日期文件夹
            for (int date = StartDate; date < EndDate+1; date++)
            {
                string dateStr = date.ToString(); if (date < 10) dateStr = "0" + dateStr;
                string readPath = @"G:\origin_data\" + dateStr + "\\";
                // 这一层为小时文件夹
                for (int hour = 0; hour < 24; hour++)
                {
                    string hourStr = hour.ToString(); if (hour < 10) hourStr = "0" + hourStr;
                    Console.WriteLine("Current date: {0}, current hour: {1}", dateStr, hourStr);
                    string[] names = Directory.GetFiles(readPath + hourStr); List<string> allRecord = new List<string>();
                    for (int pathIndex = 0; pathIndex < names.Length; pathIndex++)
                    {
                        string path = names[pathIndex];
                        StreamReader sr = new StreamReader(path, Encoding.Default); string line;
                        //读取第一遍原始数据
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] oneRecord = line.Split('|');
                            string GPStime = oneRecord[9];
                            if (GPStime.Length < "2016-08-01 11:00:00".Length)
                                continue;
                            if (DateTime.Parse(GPStime) >= DateTime.Parse("2016-08-" + dateStr + " 00:00:00") && DateTime.Parse(GPStime) < DateTime.Parse("2016-08-" + dateStr + " 23:59:59"))
                            {
                                allRecord.Add(line);
                            }
                        }
                    }
                    //一小时写一次文件
                    for (int i = 0; i < allRecord.Count; i++)
                    {
                        sw.WriteLine(allRecord[i]);
                    }
                    allRecord.Clear();
                }
                //一天写一次数据？
            }
            //清空缓冲区
            sw.Flush();
            //关闭流
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
    }
}
