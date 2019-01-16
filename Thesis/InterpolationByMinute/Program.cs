using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Thesis;

namespace getCarID
{
    class Program
    {
        static Dictionary<string, TrackPoints> carIDwithInfo = new Dictionary<string, TrackPoints>();
        //第一遍读取时就放到内存里，现在不用了
        //static List<string> originIDList = new List<string>();
        //static List<string> originTimeList = new List<string>();
        //static List<double> originLngList = new List<double>();
        //static List<double> originLatList = new List<double>();
        //static List<CarState> originStateList = new List<CarState>();
        static void Main(string[] args)
        {
            DateTime start, end;

            for (int date = 2; date < 3; date++)
            {
                string dateStr = date.ToString(); if (date < 10) dateStr = "0" + dateStr;
                string originPath = @"G:\origin_data\" + dateStr;
                string idFilePath = @"G:\interpolation_data\fullCarIDs_day" + dateStr + ".txt";
                string CarPath = @"G:\interpolation_data\" + dateStr + @"AllCar\";
                string InterpolateCarPath = @"G:\interpolation_data\" + dateStr + @"AllCarInterpolation\";
                //根据日期，第一步得到包含所有车辆id的txt
                WriteCarIDsInFolder(originPath, dateStr, idFilePath);
                //根据这个txt创建10000个txt，文件名为其id
                CreateCarIDTxtsByIDs(idFilePath, dateStr, CarPath);
                //把原来的数据写入
                updatedRecordsIntoCarIDTxts(originPath, CarPath, dateStr, idFilePath);
                //在把上一步的数据进行插值____这一步不确定，但是好像没什么问题。。
                InterpolateCarIDs(CarPath, InterpolateCarPath, dateStr);
                //string time = "11:05:00";//Console.ReadLine();                 //11:01:01
                //FindCarRecordBy(dateStr, time, @"G:\taxi_data\interpolation_data\" + dateStr + "AllCarInterpolation", @"G:\taxi_data\interpolation_data\day" + dateStr + ".txt");
            }

            Console.ReadKey();
        }
        private static void WriteCarIDsInFolder(string readPath, string dateStr, string writePath)
        //Write one txt to record each day's car IDs return "get fullCarIDs_day02_11o'clock.txt"
        {
            List<string> carIDList = new List<string>();//List<string> uniqueIDList = new List<string>();
            string[] names = Directory.GetFiles(readPath);
            FileStream fs = new FileStream(writePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            for (int pathIndex = 0; pathIndex < names.Length; pathIndex++)
            {
                string path = names[pathIndex];
                Console.WriteLine("得到fullCarIDs_day" + dateStr + ".txt正在处理： " + path);
                StreamReader sr = new StreamReader(path, Encoding.Default); string line;
                //读取第一遍原始数据
                while ((line = sr.ReadLine()) != null)      
                {
                    string[] oneRecord = line.Split('|');
                    string objectID = oneRecord[0];
                    string GPStime = oneRecord[9];
                    if (GPStime.Length < "2016-08-01 11:00:00".Length)
                        continue;
                    if (DateTime.Parse(GPStime) >= DateTime.Parse("2016-08-" + dateStr + " 00:00:00") && DateTime.Parse(GPStime) < DateTime.Parse("2016-08-" + dateStr + " 23:59:59"))
                    {
                        carIDList.Add(objectID);
                    }
                }
                carIDList = carIDList.Distinct().ToList();
            }
            //写CarIDs，6h的数据用时6min5s
            for (int i = 0; i < carIDList.Count; i++) { sw.WriteLine(carIDList[i]); }
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();fs.Close();
        }
        private static void CreateCarIDTxtsByIDs(string readPath, string dateStr, string writePath)
        {
            Console.WriteLine("根据fullCarIDs_day" + dateStr + "生成txt中...");
            StreamReader sr = new StreamReader(readPath, Encoding.Default); string line;int lineNum = 0;
            //读取CarID，转换为各个ID的txt
            while ((line = sr.ReadLine()) != null)
            {
                string[] oneRecord = line.Split(',');
                string objectID = oneRecord[0];
                string writeFolder = writePath;
                string WritePath = writeFolder + objectID + ".txt";
                if (!Directory.Exists(writeFolder))
                    Directory.CreateDirectory(writeFolder);
                FileStream fs = new FileStream(WritePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Flush();sw.Close();fs.Close();
                if (lineNum%100==0)
                {
                    Console.WriteLine("current file num:{0}", lineNum);
                }
                lineNum++;
            }
        }
        //create dictionary of carIDwithInfo based on idFilePath
        static void CreateDictionary(string idFilePath, string dateStr)     
        {
            StreamReader sr = new StreamReader(idFilePath, Encoding.Default);string line;
            if (carIDwithInfo.Keys.Count != 0)
                carIDwithInfo.Clear();               //第二次用这个字典时先清空一下ID与TrackPoints的对应
            //读取CarID，新建字典
            while ((line = sr.ReadLine()) != null)
            {
                carIDwithInfo.Add(line, new TrackPoints());
            }
        }
        static void CreateDictionaryFrom(string idFilePath,List<string> idList,List<string> timeList, List<double> lngList,List<double> latList)
        {
			StreamReader sr = new StreamReader(idFilePath, Encoding.Default);string line;
            if (carIDwithInfo.Keys.Count != 0)
                carIDwithInfo.Clear();               //第二次用这个字典时先清空一下ID与TrackPoints的对应
            //读取CarID，新建字典
            while ((line = sr.ReadLine()) != null)
            {
                carIDwithInfo.Add(line, new TrackPoints());
            }
        }
        static void updatedRecordsIntoCarIDTxts(string originalDataFolder, string writeFolder, string dateStr,string fullCarIDsTxtPath)
        {
            CreateDictionary(fullCarIDsTxtPath, dateStr);
            if (!Directory.Exists(writeFolder))                             // G:\interpolation_data\01AllCar\
                Directory.CreateDirectory(writeFolder);
            string[] names = Directory.GetFiles(originalDataFolder);
            for (int pathIndex = 0; pathIndex < names.Length; pathIndex++)  // 读取第二遍，写第一遍
            {
                string path = names[pathIndex];
                //Console.WriteLine("将原数据塞入各自id的txt，读取进度： " + path);
                StreamReader sr = new StreamReader(path, Encoding.Default);string line;
                //读取第二遍原始数据
                while ((line = sr.ReadLine()) != null)
                {
                    CarState carState;
                    string[] oneRecord = line.Split('|');
                    string objectID = oneRecord[0];
                    string GPStime = oneRecord[9];
                    double longitude = Convert.ToDouble(oneRecord[10]);
                    double latitude = Convert.ToDouble(oneRecord[11]);
                    int state = Convert.ToInt32(oneRecord[3]);          //是否空车
                    if (state == 1) carState = CarState.Vacant;
                    else carState = CarState.InUse;
                    if (GPStime.Length < "2016-08-01 11:00:00".Length)
                        continue;
                    if (DateTime.Parse(GPStime) >= DateTime.Parse("2016-08-" + dateStr + " 00:00:00") && DateTime.Parse(GPStime) < DateTime.Parse("2016-08-" + dateStr + " 23:59:59"))
                    {
                        OnePoint oneCarInfo = new OnePoint(new GISVertex(longitude, latitude), carState, objectID, GPStime);
                        TrackPoints oneCarTrack = carIDwithInfo[objectID]; //key:carID -> ListofCarID
                        oneCarTrack.addTrackPoint(oneCarInfo);             //调用ListOfCarID的方法，将文件中一行的carInfo加到Dictionaty的ListOfCarID中
                    }
                }
            }

            //将Dictionary中的数据导出
            foreach (var item in carIDwithInfo)
            {
                if (item.Value.ShowCarTrackNum() != 0)
                {
                    string writePath = writeFolder + item.Key + ".txt";
                    FileStream fs = new FileStream(writePath, FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs);
                    List<OnePoint> thisCarTrack = item.Value.GetThisCarTrack();
                    for (int i = 0; i < thisCarTrack.Count; i++)
                    {
                        //写第一遍原始数据
                        sw.WriteLine(thisCarTrack[i].CarID + "," + thisCarTrack[i].CarLocation.x + "," + thisCarTrack[i].CarLocation.y + "," + thisCarTrack[i].GPSTime + "," + thisCarTrack[i].State);
                    }
                    sw.Flush();sw.Close();fs.Close();
                }
                //Console.WriteLine("将原数据塞入各自id的txt，写入进度： " + item.Key);
            }
            //Console.WriteLine("time duration: {0}", TimeDiff(start, end));
        }
        static void updateAndInterpolateCarIDTxts(string originalDataFolder, string writeFolder, string dateStr, string fullCarIDsTxtPath)   // 这个不用了
        {
            CreateDictionary(fullCarIDsTxtPath, dateStr);
            if (!Directory.Exists(writeFolder))                             // G:\interpolation_data\01AllCar\
                Directory.CreateDirectory(writeFolder);
            string[] names = Directory.GetFiles(originalDataFolder);
            for (int pathIndex = 0; pathIndex < names.Length; pathIndex++)  // 读取第二遍，写第一遍
            {
                string path = names[pathIndex];
                //Console.WriteLine("将原数据塞入carIDwithInfo，读取进度：{0}/{1} ", pathIndex, names.Length);
                StreamReader sr = new StreamReader(path, Encoding.Default); string line;
                //读取第二遍原始数据
                while ((line = sr.ReadLine()) != null)
                {
                    CarState carState;
                    string[] oneRecord = line.Split('|');
                    string objectID = oneRecord[0];
                    string GPStime = oneRecord[9];
                    double longitude = Convert.ToDouble(oneRecord[10]);
                    double latitude = Convert.ToDouble(oneRecord[11]);
                    int state = Convert.ToInt32(oneRecord[3]);          //是否空车
                    if (state == 1) carState = CarState.Vacant;
                    else carState = CarState.InUse;
                    if (GPStime.Length < "2016-08-01 11:00:00".Length)
                        continue;
                    if (DateTime.Parse(GPStime) >= DateTime.Parse("2016-08-" + dateStr + " 00:00:00") && DateTime.Parse(GPStime) < DateTime.Parse("2016-08-" + dateStr + " 23:59:59"))
                    {
                        OnePoint oneCarInfo = new OnePoint(new GISVertex(longitude, latitude), carState, objectID, GPStime);
                        TrackPoints oneCarTrack = carIDwithInfo[objectID]; //key:carID -> ListofCarID
                        oneCarTrack.addTrackPoint(oneCarInfo);             //调用ListOfCarID的方法，将文件中一行的carInfo加到Dictionaty的ListOfCarID中
                    }
                }
            }
            //int itemCount = 0;
            foreach (var item in carIDwithInfo)
            {
//                itemCount++;

                //遍历每一ID对应的item
                if (item.Value.ShowCarTrackNum() != 0)
                {
                    List<OnePoint> thisCarTrack = item.Value.GetThisCarTrack();
                    string objectID = null;
                    List<double> newLatList = new List<double>(); List<double> newLngList = new List<double>();
                    List<string> newGPStimeList = new List<string>(); List<string> newStateList = new List<string>();
                    double last_longitude = thisCarTrack[0].CarLocation.x;
                    double last_latitude = thisCarTrack[0].CarLocation.y;
                    DateTime last_GPStime = DateTime.Parse(thisCarTrack[0].GPSTime);
                    string last_state = thisCarTrack[0].State.ToString(); //OneJourney oneJourney = null;

                    for (int i = 1; i < thisCarTrack.Count; i++)
                    {
                        objectID = thisCarTrack[i].CarID;
                        double current_longitude = thisCarTrack[i].CarLocation.x;
                        double current_latitude = thisCarTrack[i].CarLocation.y;
                        DateTime current_GPStime = DateTime.Parse(thisCarTrack[i].GPSTime);
                        string current_state = thisCarTrack[i].State.ToString();

                        if (current_GPStime.Minute - last_GPStime.Minute > 1)
                        {
                            int step_num = current_GPStime.Minute - last_GPStime.Minute;
                            for (int j = 0; j < step_num; j++)
                            {
                                //插值，3s-6s 插入4s 5s 6s
                                DateTime interpolate_time = last_GPStime.AddMinutes(j + 1);
                                string time = interpolate_time.ToShortDateString() + " " + interpolate_time.Hour.ToString() + ":"
                                + interpolate_time.Minute + ":" + "00";//time格式化为：2016/8/3 11:2:18
                                double latitude = current_latitude + (DateTime.Parse(time).TimeOfDay.TotalSeconds - current_GPStime.TimeOfDay.TotalSeconds) * (current_latitude - last_latitude) / (current_GPStime - last_GPStime).TotalSeconds;//线性插值公式进行插值
                                double longitude = current_longitude + (DateTime.Parse(time).TimeOfDay.TotalSeconds - current_GPStime.TimeOfDay.TotalSeconds) * (current_longitude - last_longitude) / (current_GPStime - last_GPStime).TotalSeconds;//线性插值公式进行插值

                                newLatList.Add(latitude); newLngList.Add(longitude); newStateList.Add(current_state); newGPStimeList.Add(DateTime.Parse(time).ToString()); //time格式化为：2016/8/3 11:02:18
                            }
                        }
                        if (current_GPStime.Minute - last_GPStime.Minute == 1 || current_GPStime.Minute - last_GPStime.Minute == -59)
                        {
                            string time = current_GPStime.ToShortDateString() + " " + current_GPStime.Hour.ToString() + ":"
                                + current_GPStime.Minute + ":" + "00";//time格式化为：2016/8/3 11:2:18
                            double latitude = current_latitude + (DateTime.Parse(time).TimeOfDay.TotalSeconds - current_GPStime.TimeOfDay.TotalSeconds) * (current_latitude - last_latitude) / (current_GPStime - last_GPStime).TotalSeconds;//线性插值公式进行插值
                            double longitude = current_longitude + (DateTime.Parse(time).TimeOfDay.TotalSeconds - current_GPStime.TimeOfDay.TotalSeconds) * (current_longitude - last_longitude) / (current_GPStime - last_GPStime).TotalSeconds;//线性插值公式进行插值

                            newLatList.Add(latitude); newLngList.Add(longitude); newStateList.Add(current_state);
                            newGPStimeList.Add(DateTime.Parse(time).ToString()); //time格式化为：2016/8/3 11:02:18
                        }
                        last_GPStime = current_GPStime; last_latitude = current_latitude; last_longitude = current_longitude; last_state = current_state;
                        //写第一遍原始数据
                        //sw.WriteLine(thisCarTrack[i].CarID + "," + thisCarTrack[i].CarLocation.x + "," + thisCarTrack[i].CarLocation.y + "," + thisCarTrack[i].GPSTime + "," + thisCarTrack[i].State);
                    }

                    //Console.WriteLine("将原数据塞入各自id的txt，写入进度：{0}/{1} ",itemCount,carIDwithInfo.Count);

                    string WritePath = writeFolder + item.Key + "_Intepolation.txt";
                    FileStream fs = new FileStream(WritePath, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    for (int i = 0; i < newGPStimeList.Count; i++)
                    {
                        //写第二遍插值后的原数据
                        sw.WriteLine(objectID + "," + newLngList[i] + "," + newLatList[i] + "," + newGPStimeList[i] + "," + newStateList[i]);
                    }
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                }
            }
        }
        //Interpolate car's id by one car's coordinate 
        static void InterpolateCarIDs(string inputFolder, string writeFolder, string dateStr)
        {
            string[] names = Directory.GetFiles(inputFolder);
            for (int nameIndex = 0; nameIndex < names.Length; nameIndex++)
            {
                if (nameIndex % 500 == 0)
                    //Console.WriteLine("将塞入各自id的txt数据进行插值，读写进度： " + names[nameIndex]);
                if (!Directory.Exists(writeFolder))
                    Directory.CreateDirectory(writeFolder);

                //List<string> GPStimeList = new List<string>(); List<double> lngList = new List<double>();
                //List<double> latList = new List<double>(); List<string> stateList = new List<string>();
                //inserted Lists
                List<double> newLatList = new List<double>(); List<double> newLngList = new List<double>();
                List<string> newGPStimeList = new List<string>(); List<string> newStateList = new List<string>();

                string objectID = null;
                StreamReader sr = new StreamReader(names[nameIndex], Encoding.Default);
                string line = sr.ReadLine(); string[] old_record = line.Split(',');
                double last_longitude = Convert.ToDouble(old_record[1]); double last_latitude = Convert.ToDouble(old_record[2]); DateTime last_GPStime = DateTime.Parse(old_record[3]);
                string last_state = old_record[4]; 
                //OneJourney oneJourney = null;
                //读取第三遍原始数据
                while ((line = sr.ReadLine()) != "" && line != null)
                {
                    string[] oneRecord = line.Split(','); objectID = oneRecord[0];
                    double current_longitude = Convert.ToDouble(oneRecord[1]);double current_latitude = Convert.ToDouble(oneRecord[2]);
                    DateTime current_GPStime = DateTime.Parse(oneRecord[3]);string current_state = oneRecord[4];

                    if (current_GPStime.Minute - last_GPStime.Minute > 1)
                    {
                        int step_num = current_GPStime.Minute - last_GPStime.Minute;
                        for (int i = 0; i < step_num; i++)
                        {
                            //插值，3s-6s 插入4s 5s 6s
                            DateTime interpolate_time = last_GPStime.AddMinutes(i + 1);
                            string time = interpolate_time.ToShortDateString() + " " + interpolate_time.Hour.ToString() + ":"
                            + interpolate_time.Minute + ":" + "00";//time格式化为：2016/8/3 11:2:18
                            double latitude = current_latitude + (DateTime.Parse(time).TimeOfDay.TotalSeconds - current_GPStime.TimeOfDay.TotalSeconds) * (current_latitude - last_latitude) / (current_GPStime - last_GPStime).TotalSeconds;//线性插值公式进行插值
                            double longitude = current_longitude + (DateTime.Parse(time).TimeOfDay.TotalSeconds - current_GPStime.TimeOfDay.TotalSeconds) * (current_longitude - last_longitude) / (current_GPStime - last_GPStime).TotalSeconds;//线性插值公式进行插值

                            newLatList.Add(latitude); newLngList.Add(longitude); newStateList.Add(current_state);newGPStimeList.Add(DateTime.Parse(time).ToString()); //time格式化为：2016/8/3 11:02:18
                        }
                    }
                    if (current_GPStime.Minute - last_GPStime.Minute == 1 || current_GPStime.Minute - last_GPStime.Minute == -59)
                    {
                        string time = current_GPStime.ToShortDateString() + " " + current_GPStime.Hour.ToString() + ":"
                            + current_GPStime.Minute + ":" + "00";//time格式化为：2016/8/3 11:2:18
                        double latitude = current_latitude + (DateTime.Parse(time).TimeOfDay.TotalSeconds - current_GPStime.TimeOfDay.TotalSeconds) * (current_latitude - last_latitude) / (current_GPStime - last_GPStime).TotalSeconds;//线性插值公式进行插值
                        double longitude = current_longitude + (DateTime.Parse(time).TimeOfDay.TotalSeconds - current_GPStime.TimeOfDay.TotalSeconds) * (current_longitude - last_longitude) / (current_GPStime - last_GPStime).TotalSeconds;//线性插值公式进行插值

                        newLatList.Add(latitude); newLngList.Add(longitude); newStateList.Add(current_state);
                        newGPStimeList.Add(DateTime.Parse(time).ToString()); //time格式化为：2016/8/3 11:02:18
                    }
                    last_GPStime = current_GPStime; last_latitude = current_latitude; last_longitude = current_longitude; last_state = current_state;
                }
                //数据所有行的时长若不到一分钟，则所有记录同最后一行记录
                if (newGPStimeList.Count == 0)
                {
                    DateTime dtLastTime = DateTime.Parse("2016-08-" + dateStr + " " + last_GPStime.Hour + ":" + last_GPStime.Minute + ":00");
                    newGPStimeList.Add(dtLastTime.ToString()); newLngList.Add(last_longitude); newLatList.Add(last_latitude); newStateList.Add(last_state);
                }
                //这地方可能有问题
                //第一个若不是11:00:00则从11:00:00分开始计算
                if (newGPStimeList[0] != "2016-08-" + dateStr + " 00:00:00")
                {
                    TimeSpan time = DateTime.Parse(newGPStimeList[0]) - DateTime.Parse("2016-08-" + dateStr + " 00:00:00");
                    int insertNum = (int)time.TotalMinutes;
                    for (int i = 0; i < insertNum; i++)
                    {
                        DateTime insertedTime = DateTime.Parse("2016-08-" + dateStr + " 00:00:00").AddMinutes(i);
                        //newGPStimeList.Insert(i, insertedTime.ToString()); newLngList.Insert(i, newLngList[0]); newLatList.Insert(i, newLatList[0]); newStateList.Insert(i, newStateList[0]);
                    }
                }
                //最后一个若不是11:59:00则复制最后一个值至11:59:00
                //——没必要插值到最后一个值，当检索时间大于目标时间的行数，直接给最后一行的值
                string lastOne = newGPStimeList[newGPStimeList.Count - 1];
                if (lastOne != "2016-08-" + dateStr + " 23:59:00")
                {
                    TimeSpan time = DateTime.Parse("2016-08-" + dateStr + " 23:59:00") - DateTime.Parse(lastOne);
                    int insertNum = (int)time.TotalMinutes;
                    for (int i = insertNum; i > 0; i--)
                    {
                        DateTime insertedTime = DateTime.Parse("2016-08-" + dateStr + " 23:59:00").AddMinutes(-i + 1);
                        //newGPStimeList.Add(insertedTime.ToString());
                        //其余List添加原来的最后一个元素
                        //newLatList.Add(newLatList[newLatList.Count - 1]); newLngList.Add(newLngList[newLngList.Count - 1]); newStateList.Add(newStateList[newStateList.Count - 1]);
                    }
                }
                string WritePath = writeFolder + names[nameIndex].Substring(names[nameIndex].Length - 9, 5) + "_Intepolation.txt";
                FileStream fs = new FileStream(WritePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                for (int i = 0; i < newGPStimeList.Count; i++)
                {
                    //写第二遍插值后的原数据
                    sw.WriteLine(objectID + "," + newLngList[i] + "," + newLatList[i] + "," + newGPStimeList[i] + "," + newStateList[i]);
                }
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                fs.Close();
            }
        }
        private static void FindCarRecordBy(string dateStr, string time, string readPath, string writePath)
        {
            FileStream fs = new FileStream(writePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string[] names = Directory.GetFiles(readPath);
            int LineNum = GetLineNumBy(DateTime.Parse("2016-08-" + dateStr + " " + time), dateStr);
            for (int i = 0; i < names.Length; i++)
            {
                string[] oneTxt = File.ReadAllLines(names[i]);      //读入一个文件的所有行
                sw.WriteLine(oneTxt[LineNum]);
            }
            sw.Flush(); sw.Close(); fs.Close();
        }
        static int GetLineNumBy(DateTime time,string dateStr)
        {
            return time.Minute;//11:00:00 为第0行 lineNum = 0
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
        private static void RecordsIntoCarIDTxts(string readPath, string dateStr, string writePath)
        {
            string line;
            string[] names = Directory.GetFiles(readPath);
            for (int pathIndex = 0; pathIndex < names.Length; pathIndex++)
            {
                string path = names[pathIndex];
                if (pathIndex % 100 == 0)
                {
                    Console.WriteLine("当前写文件进度：{0}个 ", pathIndex);
                }
                StreamReader sr = new StreamReader(path, Encoding.Default);
                while ((line = sr.ReadLine()) != null)
                {
                    CarState carState;
                    string[] oneRecord = line.Split('|');
                    string objectID = oneRecord[0];
                    string GPStime = oneRecord[9];
                    double longitude = Convert.ToDouble(oneRecord[10]);
                    double latitude = Convert.ToDouble(oneRecord[11]);
                    int state = Convert.ToInt32(oneRecord[3]);          //是否空车
                    if (state == 1) carState = CarState.Vacant;
                    else carState = CarState.InUse;
                    if (GPStime.Length < "2016-08-01 11:00:00".Length) continue;
                    if (DateTime.Parse(GPStime) >= DateTime.Parse("2016-08-" + dateStr + " 00:00:00") && DateTime.Parse(GPStime) < DateTime.Parse("2016-08-" + dateStr + " 23:59:59"))
                    {
                        //往文件名为车号的txt里写文件
                        string WritePath = writePath + objectID + ".txt";
                        FileStream fs = new FileStream(WritePath, FileMode.Append);
                        StreamWriter sw = new StreamWriter(fs);
                        sw.WriteLine(objectID + "," + longitude + "," + latitude + "," + GPStime + "," + carState);
                        sw.Flush();
                        sw.Close();
                        fs.Close();
                    }
                }
            }
        }

    }
}
