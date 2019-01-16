using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {

        //static string ReadPath = @"E:\DATA\TAXI\data\" + date + @"\12\1608" + date + @"1200.txt";
        //static string WritePath = @"E:\DATA\TAXI\results\" + date + @"\IDwithXY.txt";

        static double degreeInterval = 0.05;       //5km对应经纬度
        static List<string> carIDList = new List<string>();
        static String line;

        static void Main(string[] args)
        {
            //int lineNum = 0;
            //int a = (int) Math.Ceiling(2.2);
            //Console.Write(a);
            //StreamReader sr = new StreamReader(ReadPath, Encoding.Default);
            //FileStream fs = new FileStream(WritePath, FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.WriteLine("carID,longitude,latitude,time");

            //while ((line = sr.ReadLine()) != null)
            //{
            //    lineNum++;
            //    string[] oneRecord = line.Split('|');
            //    string carID = oneRecord[0];
            //    double longitude = Convert.ToDouble(oneRecord[10]);
            //    double latitude = Convert.ToDouble(oneRecord[11]);
            //    string time = oneRecord[9];
            //    carIDList.Add(carID);
            //    //开始写入
            //    sw.Write(carID + "," + longitude + "," + latitude + "," + time + " \r\n");
            //}

            ////清空缓冲区
            //sw.Flush();
            ////关闭流
            //sw.Close();
            //fs.Close();

            ////Console.WriteLine("有数据："+lineNum);
            //string[] carIDs = carIDList.Distinct().ToArray();
            ////Console.WriteLine("有车：" + carIDs.Length);
            DateTime dt1 = DateTime.Now;

            for (int date = 1; date < 2; date++)
            {
                string dateStr = date.ToString(); if (date < 10) dateStr = "0" + dateStr;
                StreamReader sr = new StreamReader(@"E:\DATA\TAXI\day" + dateStr + "_2ndJoin.txt", Encoding.Default);
                List<DemandPoint> dpoints = new List<DemandPoint>();
                while ((line = sr.ReadLine()) != null)                //0,120.856620029,30.6984000665,0
                {
                    string[] oneRecord = line.Split(',');
                    int objectID = Convert.ToInt32(oneRecord[0]);
                    double x = Convert.ToDouble(oneRecord[1]);
                    double y = Convert.ToDouble(oneRecord[2]);
                    int number = Convert.ToInt32(oneRecord[3]);
                    dpoints.Add(new DemandPoint(new GISVertex(x, y), number, objectID));
                }
                //List<DemandPoint> dpoints = new List<DemandPoint>();  // id == index
                //DemandPoint dp0 = new DemandPoint(new GISVertex(0, 0), 10, 0);
                //DemandPoint dp1 = new DemandPoint(new GISVertex(10, 0), 1, 1);
                //DemandPoint dp2 = new DemandPoint(new GISVertex(20, 0), 1, 2);
                //dpoints.Add(dp0); dpoints.Add(dp1); dpoints.Add(dp2);
                List<double> sumList = GetSumByPointsList(dpoints, dpoints);
                //得到sum列表中最小
                double min = Double.MaxValue; int minIndex = 0;
                for (int i = 0; i < sumList.Count; i++)
                {
                    if (sumList[i] < min)
                    {
                        min = sumList[i];
                        minIndex = i;
                    }
                }
                string writePath = @"E:\DATA\TAXI\1.txt";
                FileStream fs = new FileStream(writePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                for (int i = 0; i < dpoints.Count; i++) { sw.WriteLine(dpoints[i].CarNumber); }
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close(); fs.Close();

                Console.WriteLine("日期:" + dateStr + " 最佳食堂位置——经度：" + dpoints[minIndex].location.x + " ,纬度：：" + dpoints[minIndex].location.y + ";位置ID： " + dpoints[minIndex].objectID);
                Console.WriteLine("最小值： " + min);
            }
            DateTime dt2 = DateTime.Now;


            Console.WriteLine("所用时间：" + TimeDiff(dt1,dt2));
            Console.ReadKey();
        }

        private static List<double> GetSumByPointsList(List<DemandPoint> dpoints, List<DemandPoint> dpoints2)
        {
            List<double> sumList = new List<double>();int rotationTime=0;
            //计算所有点对象的距离列表
            for (int i = 0; i < dpoints.Count; i++)     //i=0 计算第0个点到其余所有点的距离*权重的sum
            {
                double sum = 0;
                for (int j = 0; j < dpoints2.Count; j++)
                {
                    rotationTime++;
                    if (i == j)
                    {
                        continue;
                    }
                    //sum = sum + dpoints[i].weightedDistanceTo(dpoints2[j]);
                    sum = sum + dpoints[i].noweightDistanceTo(dpoints2[j]);
                }
                sumList.Add(sum);
            }
            return sumList;
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
