using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TripCube
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime start, end;

            string writeFolder = @"D:\interpolation_data\";
            string CubePath = writeFolder  + "Cube.csv";
            string TravelPath = writeFolder + "0Travel.csv";

            //WriteTravel(readFolder, writeFolder, CubePath, TravelPath);
            // 创建Cube
            CreateCube(TravelPath, CubePath);
            // 从Cube中查询
            start = DateTime.Now;
            double startX = 15,startY = 15; double searchRadius = 5;GISVertex v = new GISVertex(startX, startY);
            Console.WriteLine("X: {0}, Y: {1}, Search radius：{2}", startX, startY,  searchRadius);
            Console.WriteLine(SearchFromCube(TravelPath, CubePath, v, searchRadius));

            end = DateTime.Now;
            Console.WriteLine("Search time : {0}", TimeDiff(start, end));
            Console.ReadKey();
        }

        private static string SearchFromCube(string TravelPath,string CubePath, GISVertex SearchVertex, double searchRadius)
        {
            string travels = "";
            StreamReader sr = new StreamReader(CubePath, Encoding.Default);
            // 从头文件获取查询条件
            string[] header = sr.ReadLine().Split(',');
            double MinStartX = Convert.ToDouble(header[0]), MaxStartX = Convert.ToDouble(header[1]), rangeX =Convert.ToDouble(header[2]);
            double MinStartY = Convert.ToDouble(header[3]), MaxStartY = Convert.ToDouble(header[4]), rangeY = Convert.ToDouble(header[5]);
            int GapNum = Convert.ToInt32(header[6]);

            //把Cube读进来
            List<string> CubeLines = new List<string>();string line;
            while ((line = sr.ReadLine()) != null)
            {
                CubeLines.Add(line);
            }
            // 把Travel读进来
            string[] TravelLines = File.ReadAllLines(TravelPath);
            //int num = Convert.ToInt32(Math.Floor((SearchVertex.x - MinStartX) / rangeX)); //计算x对应Cube行号
            int xStage = Convert.ToInt32(Math.Floor((SearchVertex.x - MinStartX) / rangeX));//每一个x对应了GapNum+1个y
            int yStage = Convert.ToInt32(Math.Floor((SearchVertex.y - MinStartY) / rangeY));
            int num = xStage * (GapNum + 1) + yStage;
            string[] KeyAndValue = CubeLines[num].Split(';');      //这个string由minStartX,lineNum,...,lineNum组成
            string[] SearchedCubeLine = KeyAndValue[1].Split(',');

            // 从x对应Cube的行号获取行程
            for (int i = 0; i < SearchedCubeLine.Length; i++)
            {
                if (SearchedCubeLine[i] == "") continue;
                int LineInTravel = Convert.ToInt32(SearchedCubeLine[i]) + 1;
                string[] travel = TravelLines[LineInTravel].Split(',');
                double StartX = Convert.ToDouble(travel[5]),StartY = Convert.ToDouble(travel[6]);
                if (Math.Abs(StartX - SearchVertex.x) < searchRadius&& Math.Abs(StartY - SearchVertex.y) < searchRadius)
                    travels = travels + TravelLines[LineInTravel] + '\n';
            }

            return travels;
        }
        private static int CreateCube(string travelPath, string cubePath)
        {
            List<string> AllTravels = new List<string>();
            StreamReader sr = new StreamReader(travelPath, Encoding.Default);
            // 内存读取行程数据集，获得起始经纬度最大、最小值
            string line = sr.ReadLine();
            double minX = double.MaxValue, maxX = double.MinValue;
            double minY = double.MaxValue, maxY = double.MinValue;
            Console.WriteLine("Start Reading!");
            int lineNum = 0;
            while ((line = sr.ReadLine()) != null)
            {
                AllTravels.Add(line);
                string[] travel = line.Split(',');
                double startX = Convert.ToDouble(travel[5]),startY = Convert.ToDouble(travel[6]);
                if (startX < minX) minX = startX;
                if (startX > maxX) maxX = startX;
                if (startY < minY) minY = startY;
                if (startY > maxY) maxY = startY; lineNum++;
            }
            Console.WriteLine("Reading finished! Creating Cube now！Record Lin Num: {0}",lineNum);
            // 读取行程数据集，生成Cube
            lineNum = 0; int err = 0;
            Cube cube = new Cube(minX,maxX,minY,maxY);

            for (int i = 0; i < AllTravels.Count; i++)
            {
                string[] oneRecord = AllTravels[i].Split(',');
                string CarID = oneRecord[1];
                DateTime StartTime = DateTime.Parse(oneRecord[2]);
                DateTime EndTime = DateTime.Parse(oneRecord[3]);
                double StartX = Convert.ToDouble(oneRecord[5]), StartY=Convert.ToDouble(oneRecord[6]), EndX= Convert.ToDouble(oneRecord[7]), EndY= Convert.ToDouble(oneRecord[8]);
                double TravelLength = Convert.ToDouble(oneRecord[9]);

                OneTravel oneTravel = new OneTravel(lineNum, CarID, StartTime, EndTime, StartX, StartY, EndX, EndY, TravelLength);

                err = cube.AddTravel(oneTravel);

                lineNum++;
                
            }
            //Console.WriteLine("Creating cube finished，WRITING now!");
            Console.WriteLine("ERROR Num: {0}", err);
            cube.WriteCubeToCSV(cubePath);

            return err;
            //Console.WriteLine("WRITING FINISHED!");
        }
        private static void WriteTravelAndCube(string readPath, string writeFolder, string cubePath, string travelPath)
        {
            //1. 先读取原始文件，生成行程数据集的同时(不写，仅存在内存里)存储行程数据集中起始点X的最大值最小值，据此内存中生成Cube
            string[] names = Directory.GetFiles(readPath);
            FileStream fs_cube = new FileStream(cubePath, FileMode.Create);
            StreamWriter sw_cube = new StreamWriter(fs_cube);
            FileStream fs_travel = new FileStream(travelPath, FileMode.Create);
            StreamWriter sw_travel = new StreamWriter(fs_travel);

            string dateStr = "01"; double StartX_max = Double.MinValue; double StartX_min = Double.MaxValue;
            for (int pathIndex = 0; pathIndex < names.Length; pathIndex++)
            {
                string path = names[pathIndex];
                StreamReader sr = new StreamReader(path, Encoding.Default); 
                string line = sr.ReadLine(); string[] old_record = line.Split(',');string objectID;
                double last_longitude = Convert.ToDouble(old_record[1]); double last_latitude = Convert.ToDouble(old_record[2]); DateTime last_GPStime = DateTime.Parse(old_record[3]);
                string last_state = old_record[4];
                // OneTravel
                double startLng, startLat, endLng, endLat;DateTime startDt, endDt;                 
                //读取第一遍原始数据
                while ((line = sr.ReadLine()) != null)
                {
                    string[] oneRecord = line.Split(','); objectID = oneRecord[0];
                    double current_longitude = Convert.ToDouble(oneRecord[1]); double current_latitude = Convert.ToDouble(oneRecord[2]);
                    DateTime current_GPStime = DateTime.Parse(oneRecord[3]); string current_state = oneRecord[4];
                    
                }
            }
            //清空缓冲区
            sw_cube.Flush();sw_travel.Flush();
            //关闭流
            sw_cube.Close(); fs_cube.Close();sw_travel.Close();fs_travel.Close();

            //2.再次遍历内存里的行程数据集，判断行程数据集每一行属于Cube中的哪一个X坐标
            //3.把该行行程数据集的行号写入Cube中对应。
            //4.最后把Cube和行程数据集写到硬盘里

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
