using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Thesis
{
    public partial class Form1 : Form
    {
        //默认2016-8-4 
        string date;
        
        OnePoint oneCar;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ReadPath = @"E:\DATA\TAXI\data\" + date + @"\12\1608" + date + @"1200.txt";
            string WritePath = @"E:\DATA\TAXI\results\" + date + @"\specificTime.txt";

            string startTime = "2016-08-01  " + textBox1.Text;
            string endTime = "2016-08-01  " + textBox2.Text;
            string[] startTimeList = textBox1.Text.Split(':');
            string[] endTimeList = textBox2.Text.Split(':');
            if (startTimeList[0] != endTimeList[0])
            {
                MessageBox.Show("选择时间范围过长！请重新输入！");
                return;
            }
            string hour;List<string> minuteList = new List<string>();
            int endTimeMinute = Convert.ToInt32(endTimeList[1]);
            int startTimeMinute = Convert.ToInt32(startTimeList[1]);
            hour = startTimeList[0];
            for (int i = startTimeMinute; i <= endTimeMinute; i++)
            {
                minuteList.Add(i.ToString());
            }
            List<string> ReadPaths = new List<string>();
            for (int i = 0; i < minuteList.Count; i++)
            {

            }
            StreamReader sr = new StreamReader(ReadPath, Encoding.Default);
            FileStream fs = new FileStream(WritePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("carID,longitude,latitude,time");
            String line; int LineNum=0;
            while ((line = sr.ReadLine()) != null)
            {
                //28362|A|0|0|0|0|0|0|2016-08-11 11:03:00|2016-08-11 11:04:17|121.454437|31.225832|30.4|268.0|8|030
                CarState carState;
                string[] oneRecord = line.Split('|');
                string carID = oneRecord[0];
                double longitude = Convert.ToDouble(oneRecord[10]);
                double latitude = Convert.ToDouble(oneRecord[11]);
                string GPStime = oneRecord[9];
                int state = Convert.ToInt32(oneRecord[3]);

                if (state==1)
                {
                    carState = CarState.Vacant;
                }
                else
                {
                    carState = CarState.InUse;
                }
                //carIDList.Add(carID);
                //开始写入
                //sw.Write(carID + "," + longitude + "," + latitude + "," + time + " \r\n");
                if ((DateTime.Parse(startTime)< DateTime.Parse(GPStime))&&(DateTime.Parse(GPStime) < DateTime.Parse(endTime)))
                {
                    if (LineNum%1000==0)
                    {
                        richTextBox1.AppendText(carID + "," + carState.ToString() + "," + longitude + "," + latitude + "," + GPStime + " \r\n");
                    }

                    LineNum++;
                }
                
            }
            richTextBox1.AppendText("读取结束，共" + LineNum + "条");
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
            //richTextBox1.AppendText("hello "+"\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //2016-08-01 11:30:30
            //string ReadPath = @"E:\DATA\TAXI\data\" + day + "\" + hour + "\1608" + day + hour + minute + ".txt";
            //string ReadPath = @"E:\DATA\TAXI\data\" + day + @"\12\1608" + day + @"1200.txt";
            //设置一些参数，输出为day+日期

            string selectedTime = textBox3.Text;            
            string[] selectedTimeList = textBox3.Text.Split(':');
            string hour = selectedTimeList[0];
            string minute = selectedTimeList[1];
            String line; int LineNum = 0;

            try
            {                
                for (int dayInt = 1; dayInt < 7; dayInt++)          //循环每一天的数据
                {
                    if (dayInt < 10)
                    {
                        date = "0" + dayInt;
                    }                    
                    int minuteInt = Convert.ToInt16(selectedTimeList[1]) - 2;

                    //day01.txt 提取的8月1号
                    string WritePath = @"E:\DATA\TAXI\results\day" + date + ".txt";
                    FileStream fs = new FileStream(WritePath, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    //sw.WriteLine("carID,longitude,latitude,time,state");
                    List<string> carIDList = new List<string>();
                    for (int i = 0; i < 4; i++)                     //搜索附近txt
                    {
                        minute = Convert.ToString(minuteInt);
                        if (Convert.ToInt32(minute) < 10)
                        {
                            minute = "0" + minute;
                        }
                        string ReadPath = @"E:\DATA\TAXI\data\" + date + @"\" + hour + @"\1608" + date + hour + minute + ".txt";
                        minuteInt++;
                        StreamReader sr = new StreamReader(ReadPath, Encoding.Default);
    
                        while ((line = sr.ReadLine()) != null)
                        {
                            CarState carState;
                            string[] oneRecord = line.Split('|');
                            string carID = oneRecord[0];
                            double longitude = Convert.ToDouble(oneRecord[10]);
                            double latitude = Convert.ToDouble(oneRecord[11]);
                            string GPStime = oneRecord[9]; 
                            int state = Convert.ToInt32(oneRecord[3]);          //是否空车

                            if (state == 1)
                            {
                                carState = CarState.Vacant;
                            }
                            else
                            {
                                carState = CarState.InUse;
                            }
                            if (GPStime.Length < 12)
                                continue;

                            if (GPStime.Substring(11) == selectedTime)
                            {
                                if (carState == CarState.Vacant)
                                {
                                    //输出看数据
                                    //if (LineNum % 500 == 0)
                                    //{
                                    richTextBox1.AppendText(LineNum + "," + carID + "," + carState.ToString() + "," + longitude + "," + latitude + "," + GPStime + " \r\n");

                                    //}
                                    //写文件
                                    sw.Write(carID + "," + longitude + "," + latitude + "," + GPStime + "," + carState.ToString() + " \r\n");
                                    carIDList.Add(carID);
                                    LineNum++;
                                }
                            }

                        }

                    }
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();

                    //richTextBox1.AppendText("第" + dayInt +"天读取结束，共" + LineNum + "条数据" + " \r\n");
                    //LineNum = 0;
                    //string[] carIDs = carIDList.Distinct().ToArray();
                    //richTextBox1.AppendText("第" + dayInt +"天读取结束，共" + carIDs.Length + "辆车" + " \r\n");
                }
                richTextBox1.AppendText("读取结束，共" + LineNum + "条");
            }

            catch(Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}
