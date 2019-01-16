using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace P_median
{
    class Program
    {
        static void Main(string[] args)
        {          
            DateTime dt1 = DateTime.Now;
            string dateStr = "01";
            StreamReader sr = new StreamReader(@"C:\DATA\day" + dateStr + "_2ndJoin.csv", Encoding.Default);
            List<DemandPoint> dpoints = new List<DemandPoint>(); string line;
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

            //double[,] distanceArr = Get2DdistanceArray(dpoints);

            int[] IDs = new int[dpoints.Count];
            for (int i = 0; i < IDs.Length; i++)
                IDs[i] = dpoints[i].objectID;

            int N = dpoints.Count;                      //总共demand point个数
            int P = 5;                                  //总共facility个数
            List<double> SumList = new List<double>();  //比较每一个方案的Sum

            //所有可供选择的facility方案——string数组，每个item对应一个facility方案
            List<string> CnmResult = GetCnmResult(IDs, P);
            int solutionNum = CnmResult.Count;
            int solutionPercent = solutionNum / 100;                //one percent of total
            int process = 0;                                        //percentage
            //遍历所有的可能方案
            for (int i = 0; i < CnmResult.Count; i++)
            {
                double sum = 0;
                string[] facilities = CnmResult[i].Split(',');
                List<string> restPoints = ArrayExclude(IDs, facilities);
                //遍历一个方案的restPoints
                for (int j = 0; j < restPoints.Count; j++)
                {
                    int restPoint = Convert.ToInt32(restPoints[j]);
                    //遍历Facilities，为剩下的每个point找到最近的facility,计算距离
                    int int_facility = Convert.ToInt32(GetNearFaciAndDistan(facilities, restPoint, dpoints).Split(',')[0]);
                    DemandPoint facility = dpoints[int_facility];
                    //restPoint过来facilit的权重*距离
                    double distance = facility.weightedDistanceTo(dpoints[restPoint]);
                    //restPoint过来facility的距离
                    //double distance = facility.noweightDistanceTo(dpoints[restPoint]);
                    sum += distance;
                }
                SumList.Add(sum);
                //Show progress
                if (solutionPercent != 0 && SumList.Count % solutionPercent == 0)
                {
                    process += 1;
                    Console.WriteLine("calculate distance: " + process.ToString() + " percent finished!");
                }
            }

            //得到sum列表中最小
            double min = Double.MaxValue; int minIndex = 0;string minString = "";
            for (int i = 0; i < SumList.Count; i++)
            {
                if (SumList[i] <= min)
                {
                    min = SumList[i];
                    minIndex = i;
                    minString += CnmResult[minIndex] + ";";
                }
            }

            Console.WriteLine("Best IDs: " + CnmResult[minIndex]);

            string[] results = CnmResult[minIndex].Split(',');
            for (int i = 0; i < results.Length; i++)
            {
                int result = Convert.ToInt32(results[i]);
                string resultPosition = dpoints[result].location.ToString();
                Console.WriteLine("Point" + i.ToString() + ": " + resultPosition);
            }

            Console.WriteLine("Best Cost: " + min.ToString());
            DateTime dt2 = DateTime.Now;
            Console.WriteLine("所用时间：" + TimeDiff(dt1, dt2));
            Console.ReadKey();
        }

        static string GetNearFaciAndDistan(string[] facilities, int restPointIndex, List<DemandPoint> demandPoints)
        {
            int neareastFacility = 0; double minDistance = double.MaxValue;
            for (int faciIndex = 0; faciIndex < facilities.Length; faciIndex++)
            {
                int oneFacilityIndex = Convert.ToInt32(facilities[faciIndex]);
                DemandPoint oneFacility = demandPoints[oneFacilityIndex];
                //double oneDistance = distanceArr[restPointIndex, oneFacility];
                DemandPoint restPoint = demandPoints[restPointIndex];
                double oneDistance = restPoint.DistanceTo(oneFacility.location);
                if (oneDistance < minDistance)
                {
                    minDistance = oneDistance;
                    neareastFacility = oneFacilityIndex;
                }
            }
            return neareastFacility.ToString() + ',' + minDistance.ToString();
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
        static List<string> ArrayExclude(int[] sourceArray, string[] toBeExclude)
        {
            List<string> result = new List<string>();
            List<string> exclude = new List<string>(toBeExclude);
            for (int i = 0; i < sourceArray.Length; i++)
            {
                if (exclude.Contains(sourceArray[i].ToString()))
                    continue;
                else
                    result.Add(sourceArray[i].ToString());
            }
            return result;
        }

        static List<string> GetCnmResult(int[] sourceArr, int m)   //m=select number
        {
            List<string> CnmResult = new List<string>();
            int size = (int)Cnm(sourceArr.Length, m);
            Console.WriteLine("Cn m= {0}", size);
            int solutionPercent = size / 100;                      //one percent of total
            int process = 0;                                       //percentage

            for (int i = 0; i < size; i++)
            {
                String data = getValue(sourceArr, m, i);
                CnmResult.Add(data);
                // show progress
                
                if (solutionPercent != 0 && CnmResult.Count % solutionPercent == 0)
                {
                    process += 1;
                    Console.WriteLine("GetCnmResult: " + process.ToString() + " percent finished!");
                }
            }
            return CnmResult;
        }
        static void ShowProgress(int totalNum, int currentNum)
        {

        }
        static int GetSmallerIndex(double num1, double num2)
        {
            int smallerIndex;
            if (num1 > num2)
                smallerIndex = 1;
            else
                smallerIndex = 0;

            return smallerIndex;
        }
        static double GetSmallNum(double num1, double num2)
        {
            return (num2 > num1) ? num1 : num2;
        }
        public static String getValue(int[] source, int m, int x)
        {
            // 数组大小
            int n = source.Length;
            // 存储组合数
            StringBuilder sb = new StringBuilder();
            int start = 0;
            while (m > 0)
            {
                if (m == 1)
                {
                    // m == 1 时为组合数的最后一个字符
                    sb.Append(source[start + x] + ",");
                    break;
                }
                for (int i = 0; i <= n - m; i++)
                {
                    int cnm = (int)Cnm(n - 1 - i, m - 1);
                    if (x <= cnm - 1)
                    {
                        sb.Append(source[start + i] + ",");
                        // 启始下标前移
                        start = start + (i + 1);
                        // 搜索区域减小
                        n = n - (i + 1);
                        // 组合数的剩余字符个数减少
                        m--;
                        break;
                    }
                    else
                    {
                        x = x - cnm;
                    }
                }
            }
            return sb.Remove(sb.Length - 1, 1).ToString();//sb.Substring(0, sb.Length() - 1).toString();
        }
        // 计算组合数
        public static double Cnm(int n, int m)
        {
            if (n < 0 || m < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (n == 0 || m == 0)
            {
                return 1;
            }
            if (m > n)
            {
                return 0;
            }
            if (m > n / 2.0)
            {
                m = n - m;
            }
            double result = 0.0;
            for (int i = n; i >= (n - m + 1); i--)
            {
                result += Math.Log(i);
            }
            for (int i = m; i >= 1; i--)
            {
                result -= Math.Log(i);
            }
            result = Math.Exp(result);
            return Math.Round(result);
        }
        public static double[,] Get2DdistanceArray(List<DemandPoint> demandPoints)
        {
            int size = demandPoints.Count;
            double[,] distance = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == j)
                        distance[i,j] = 0;
                    distance[i, j] = demandPoints[i].DistanceTo(demandPoints[j].location);
                }
            }
            return distance;
        }

    }
}
