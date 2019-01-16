using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start, end; start = DateTime.Now;
            InterpolateCarIDs(@"E:\DATA\TAXI\results\03AllCar", "03", @"E:\DATA\TAXI\results\03AllCarInterpolation\");
            end = DateTime.Now;
            Console.WriteLine("time duration : {0}", TimeDiff(start, end));

            Console.ReadKey();
        }
        static void InterpolateCarIDs(string inputFolder, string dateStr, string writeFolder)//Interpolate car's id by one car's coordinate
        {
            string[] names = Directory.GetFiles(inputFolder);
            for (int nameIndex = 0; nameIndex < names.Length; nameIndex++)
            {
                if (nameIndex % 1000 == 1)
                    Console.WriteLine("将塞入各自id的txt数据进行插值，读写进度： " + names[nameIndex]);
                StreamReader sr = new StreamReader(names[nameIndex], Encoding.Default);
                string WritePath = writeFolder + names[nameIndex].Substring(names[nameIndex].Length - 9, 5) + "_Intepolation.txt";
                if (!Directory.Exists(writeFolder))
                    Directory.CreateDirectory(writeFolder);
                List<string> GPStimeList = new List<string>(); List<double> lngList = new List<double>();
                List<double> latList = new List<double>(); List<string> stateList = new List<string>();
                //inserted Lists
                List<double> newLatList = new List<double>(); List<double> newLngList = new List<double>();
                List<string> newGPStimeList = new List<string>(); List<string> newStateList = new List<string>();
                string objectID = null;
                string line;
                while ((line = sr.ReadLine()) != "" && line != null)
                {
                    string[] oneRecord = line.Split(','); objectID = oneRecord[0];
                    double longitude = Convert.ToDouble(oneRecord[1]);
                    double latitude = Convert.ToDouble(oneRecord[2]);
                    string GPStime = oneRecord[3];
                    string state = oneRecord[4];
                    GPStimeList.Add(GPStime); lngList.Add(longitude); latList.Add(latitude); stateList.Add(state);
                }
                //第一个若不是11:00:00则从11:00:00分开始计算
                if (GPStimeList[0] != "2016-08-" + dateStr + " 11:00:00")
                {
                    GPStimeList.Insert(0, "2016-08-" + dateStr + " 11:00:00");
                    lngList.Insert(0, lngList[0]);
                    latList.Insert(0, latList[0]);
                    stateList.Insert(0, stateList[0]);
                }

                //遍历GPSTime数组，每一分钟写入一个点
                for (int i = 0; i < GPStimeList.Count - 1; i++)
                {
                    TimeSpan time = DateTime.Parse(GPStimeList[i + 1]) - DateTime.Parse(GPStimeList[i]);
                    int stepNum = (int)time.TotalSeconds;  //步数等于时间间隔 e.g 11s-14s（3s）插2个空（12s，13s） 3m
                    double lngStepLen = (lngList[i + 1] - lngList[i]) / stepNum;        //步长  1m
                    double latStepLen = (latList[i + 1] - latList[i]) / stepNum;        //步长  1m
                    string state = stateList[i];
                    for (int j = 0; j < stepNum; j++)                        //0,1,2
                    {
                        double insertedLat = latList[i] + latStepLen * j;    //latList[0],插入新点1，新点2
                        double insertedLng = lngList[i] + lngStepLen * j;    //
                        string insertedTime = DateTime.Parse(GPStimeList[i]).AddSeconds(j).ToString();
                        newLatList.Add(insertedLat); newLngList.Add(insertedLng);
                        newGPStimeList.Add(insertedTime); newStateList.Add(state);
                    }
                }
                //把一个文件夹的内容都塞到内存里，最后一起写入文件
                newGPStimeList.Add(DateTime.Parse(GPStimeList[GPStimeList.Count - 1]).ToString());
                newStateList.Add(stateList[stateList.Count - 1]);
                newLatList.Add(latList[latList.Count - 1]); newLngList.Add(newLngList[newLngList.Count - 1]);

                FileStream fs = new FileStream(WritePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                for (int i = 0; i < newGPStimeList.Count; i++)
                {
                    if (i % 60 == 0)
                        sw.WriteLine(objectID + "," + newLngList[i] + "," + newLatList[i] + "," + newGPStimeList[i] + "," + newStateList[i]);
                }
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                fs.Close();
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
