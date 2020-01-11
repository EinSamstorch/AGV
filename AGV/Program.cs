using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AGV
{
    struct Point
    {
        public string xCoordinate;
        public string yCoordinate;
        public string theta;//0 <= θ <= 3.141592
        public string xSpeed;
        public string ySpeed;//该列全零
        public string label;//用来区分多个点之间的路径段，拐一次弯加1
    }

    public class Program
    {
        //插补间距5cm
        private const double OFFSET = 0.05;
        private const int COUNT = 20;
        private const double PI = 3.14159;
        static double[] x = new double[48];
        static double[] y = new double[48];
        private const string filePath = @"D:\Documents\Visual Studio 2019\AGV\AGV\Source\coordinate.txt";

        public static void Main(double offset)
        {
            
            //List<Point> points = new List<Point>();
            int[] points1 = { 4, 5, 6, 2 ,3};

            GeneratePathFile(points1);
            Console.ReadKey();

        }

        public static bool GeneratePathFile(int[] points)
        {
            return GeneratePathFile("path5.txt", points);
        }

        public static bool GeneratePathFile(string fileName, int[] points)
        {
            //读取数据失败则返回
            if (!ReadPathFile(filePath))
            {
                return false;
            }

            //根据点的数量确定二维数组长度
            int size = (points.Length - 1) * 20 + 1;
            Point[] points1 = new Point[(points.Length - 1) * 20 + 1];
            string[,] generateCoordinates = new string[(points.Length - 1) * 20 + 1, 6];
            double flag1 = 0;
            double vFlag = 1, oFlag = 1;

            for (int i = 0; i < points.Length - 1; i++)
            {
                
                double x1 = x[points[i]];
                double y1 = y[points[i]];
                double x2 = x[points[i + 1]];
                double y2 = y[points[i + 1]];
                double k = (y2 - y1) / (x2 - x1);
                double b = y1 - k * x1;
                double radian = Math.Atan(k);
                //小车在第一个点都是倒车出来的
                if (i == 0)
                    vFlag = -1;
                else
                    vFlag = 1;
                if (y2 > y1)
                    oFlag = 1;
                else
                    oFlag = -1;
                if (radian < 0)
                {
                    radian += 2 * PI;
                }
                for (int j = 0; j < COUNT; j++)
                {
                    //x1和x2在一列
                    if (x1 == x2)
                    { 
                        generateCoordinates[i * COUNT + j, 0] = x1.ToString("0.000000");//x坐标值不变
                        generateCoordinates[i * COUNT + j, 1] = (y1 + j * oFlag * (OFFSET)).ToString("0.000000");
                        generateCoordinates[i * COUNT + j, 2] = radian.ToString("0.000000");//弧度制 = arctan（k)
                        generateCoordinates[i * COUNT + j, 3] = (0.2 * vFlag).ToString("0.000000");//x方向速度
                        generateCoordinates[i * COUNT + j, 4] = 0.ToString("0.000000");//y方向速度
                        generateCoordinates[i * COUNT + j, 5] = flag1.ToString("0.000000");//转向标志位
                    }
                    else
                    //x1和x2不在同一列
                    {
                        generateCoordinates[i * COUNT + j, 0] = ((x1 + j * OFFSET).ToString("0.000000"));//x坐标每次加偏移值
                        generateCoordinates[i * COUNT + j, 1] = (k * double.Parse(generateCoordinates[i * COUNT + j, 0]) + b).ToString("0.000000");//y=kx+b
                        generateCoordinates[i * COUNT + j, 2] = radian.ToString("0.000000");//弧度制 = arctan（k)
                        generateCoordinates[i * COUNT + j, 3] = (0.2 * vFlag).ToString("0.000000");
                        generateCoordinates[i * COUNT + j, 4] = 0.ToString("0.000000");
                        generateCoordinates[i * COUNT + j, 5] = flag1.ToString("0.000000");

                    }
                }
                //给最后一个点赋值
                generateCoordinates[(points.Length - 1) * COUNT, 0] = x2.ToString("0.000000");
                generateCoordinates[(points.Length - 1) * COUNT, 1] = y2.ToString("0.000000");
                generateCoordinates[(points.Length - 1) * COUNT, 2] = radian.ToString("0.000000");//弧度制 = arctan（k)
                generateCoordinates[(points.Length - 1) * COUNT, 3] = 0.2.ToString("0.000000");
                generateCoordinates[(points.Length - 1) * COUNT, 4] = 0.ToString("0.000000");
                generateCoordinates[(points.Length - 1) * COUNT, 5] = flag1.ToString("0.000000");
                flag1++;

            }

            for (int i = 0; i < generateCoordinates.GetLength(0); i++)
            {
                Console.WriteLine("x:{0}\t y:{1}\t radian:{2}\t vx:{3}\t vy:{4}\t flag:{5}",
                    generateCoordinates[i, 0], generateCoordinates[i, 1], generateCoordinates[i, 2], generateCoordinates[i, 3], generateCoordinates[i, 4], generateCoordinates[i, 5]);
            }

            return GeneratePathFile(fileName, generateCoordinates);
        }


        public static bool ReadPathFile(string filePath)
        {
            string[][] data = null;
            try
            {
                data = File.ReadAllLines(filePath).Select(x => x.Split(' ')).ToArray();
                for (int i = 0; i < data.Length; i++)
                {
                    x[i] = double.Parse(data[i][0]);
                    y[i] = double.Parse(data[i][1]);
                }    
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private static bool GeneratePathFile(string fileName, string[,] generateCoordinates)
        {
            try
            {
                IsTheSameFileExists(fileName);
                //将数组写入到txt文件中
                string sign = "\t";//元素之间分隔符号，此处设置为tab
                StreamWriter sw = new StreamWriter(fileName, true);

                for (int i = 0; i < generateCoordinates.GetLength(0); i++)
                {
                    for (int j = 0; j < generateCoordinates.GetLength(1); j++)
                    {
                        sw.Write(generateCoordinates[i, j] + sign);
                    }
                    sw.WriteLine();
                }
                sw.Flush();
                sw.Close();
                sw.Dispose();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        private static void IsTheSameFileExists(string fileName)
        {
            //监测是否有同名文件，有的话则删除
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}
