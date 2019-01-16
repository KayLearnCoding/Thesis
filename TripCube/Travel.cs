using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TripCube
{
    class Cube
    {
        int GapNum = 5;
        //Dictionary<double, List<int>> XwithLineIndex = new Dictionary<double, List<int>>();
        Dictionary<GISVertex, List<int>> VertexWithLineIndex = new Dictionary<GISVertex, List<int>>();
        double MinStartX, MaxStartX; double rangeX;
        double MinStartY, MaxStartY; double rangeY;
        public Cube(double minStartX, double maxStartX, double minStartY, double maxStartY)
        {
            MinStartX = minStartX;
            MaxStartX = maxStartX;
            rangeX = (maxStartX - minStartX) / GapNum;
            double x = MinStartX;
            MinStartY = minStartY;
            MaxStartY = maxStartY;
            rangeY = (maxStartY - minStartY) / GapNum;
            //for (int i = 0; i < GapNum + 1; i++)
            //{
            //    XwithLineIndex.Add(x, new List<int>());
            //    x = x + rangeX;                             //这里每一个x表示区间最小值，临界值：[min,max)
            //}
            for (int i = 0; i < GapNum + 1; i++)
            {
                double y = MinStartY;
                for (int j = 0; j < GapNum + 1; j++)
                {
                    GISVertex vertex = new GISVertex(x, y);
                    y = y + rangeY;
                    VertexWithLineIndex.Add(vertex, new List<int>());
                }
                x = x + rangeX;
            }
        }

        public int AddTravel(OneTravel travel)
        {
            double x = travel.StartX, y = travel.StartY;
            double intervalX = Math.Floor((x - MinStartX) / rangeX) * rangeX + MinStartX;
            double intervalY = Math.Floor((y - MinStartY) / rangeY) * rangeY + MinStartY;
            GISVertex key = new GISVertex(intervalX, intervalY);
            int errorCount = 0;
            // 什么情况下会出现明明存在key却说不存在
            if (VertexWithLineIndex.ContainsKey(key))
            {
                VertexWithLineIndex[key].Add(travel.LineNum);

            }
            else
            {
                errorCount++;
            }

            return errorCount;
;            //double key = Math.Floor((x - MinStartX) / rangeX) * rangeX + MinStartX; //计算出x对应的区间最小值
            //XwithLineIndex[key].Add(travel.LineNum);    
        }

        public void WriteCubeToCSV(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(MinStartX + "," + MaxStartX + "," + rangeX + "," + MinStartY + "," + MaxStartY + "," + rangeY + "," + GapNum);
            //遍历每一个minX对应的item
            foreach (var item in VertexWithLineIndex)
            {
                sw.Write(item.Key + ";");
                for (int i = 0; i < item.Value.Count; i++)
                {
                    sw.Write(item.Value[i] + ",");
                }
                sw.Write("\n");
            }

            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close(); fs.Close();
        }
    }
    class OneTravel
    {
        //起始点X, 起始点Y, 终止点X, 终止点Y, 起始时间, 终止时间, 行程长度, 行程时间, ...(OneTravel) 
        string CarID;
        public double StartX, StartY, EndX, EndY;
        DateTime StartTime, EndTime, TravelTime;
        double TravelLength;
        public int LineNum;
        public OneTravel(int lineNum, string carID, DateTime startTime, DateTime endTime, double startX,double startY,double endX,double endY, double travelLength)
        {
            LineNum = lineNum;
            CarID = carID;
            StartTime = startTime;
            EndTime = endTime;
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
            TravelLength = travelLength;
        }
    }
    class GISVertex
    {
        public double x;
        public double y;
        public GISVertex(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        internal double DistanceTo(GISVertex anotherLocation)
        {
            return Math.Sqrt((x - anotherLocation.x) * (x - anotherLocation.x) + (y - anotherLocation.y) * (y - anotherLocation.y));
        }
        public override string ToString()
        {
            return "x,y: " + x.ToString() + "," + y.ToString();
        }
        public override int GetHashCode()
        {
            return (x + y * 37).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is GISVertex))
            {
                return false;
            }
            var p = (GISVertex)obj;
            bool flag;
            if (Math.Abs(this.x - p.x) < 0.000001 && Math.Abs(this.y - p.y) < 0.000001)
                flag = true;
            else
                flag = false;
            return flag;
        }
    }
    class Travels
    {
        List<OneTravel> travels = new List<OneTravel>();

        public void AddTravel(OneTravel oneTravel)
        {
            travels.Add(oneTravel);
        }
    }
}
