using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AGV
{
    //定义点的结构 
    public struct Point
    {
        public double xCoordinate;
        public double yCoordinate;
        public double theta;//0 <= θ <= 3.141592
        public double xSpeed;//0.1 <= v <= 0.4 , ▲v = 0.1  转弯时速度为0。
        public double ySpeed;//该列全零
        public int label;//用来区分多个点之间的路径段，拐一次弯加1
    }

    public class Program
    {
        private const double OFFSET = 0.05;
        private const double PI = 3.14159;
        private const int POINTNUMBER = 48;
        public static List<Point> initialPoints = new List<Point>();
        private const string filePath = @"D:\Documents\Visual Studio 2019\AGV\AGV\Source\coordinate.txt";

        public static void Main(string[] args)
        {           
            int[] pointsFromPan = { 4, 5, 6, 2 ,3};
            List<Point> simplifiedPoints = new List<Point>();//简化之后要走的点 

            //GeneratePathFile(pointsFromPan);
            Console.ReadKey();

        }

        public static bool GeneratePathFile(int[] pointsFromPan)
        {
            return GeneratePathFile("path5.txt", pointsFromPan);
        }

        public static bool GeneratePathFile(string fileName, int[] pointsFromPan)
        {
            List<Point> simplifiedPoints = new List<Point>();//简化之后要走的点        
            List<Point> generatePoints = new List<Point>();//最终生成的点

            //如果读取文件失败则返回false
            if (!ReadPathFile(filePath))
            {
                return false;
            }

            ModifyThePositionOfStartingPoint(pointsFromPan);

            SimplifyPoints(pointsFromPan, simplifiedPoints);

            for (int i = 0; i < simplifiedPoints.Count - 1; i++)
            {
                double x1 = simplifiedPoints[i].xCoordinate;
                double y1 = simplifiedPoints[i].yCoordinate;
                double x2 = simplifiedPoints[i + 1].yCoordinate;
                double y2 = simplifiedPoints[i + 1].yCoordinate;
                double k = (y2 - y1) / (x2 - x1);
                double b = y1 - k * x1;
                double radian = Math.Atan(k);

                if ((x2-x1) == 0)
                {
                    for (int j = 0; j < (y2-y1)/OFFSET; j++)
                    {
                        Point point = new Point();

                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 将同一条直线上的几个点合并成两个点，方便后面进行加减速控制。
        /// </summary>
        /// <param name="pointsFromPan"></param>
        /// <param name="simplifiedPoints"></param>
        public static void SimplifyPoints(int[] pointsFromPan, List<Point> simplifiedPoints)
        {
            int deta1 = 0; //用于判断前后两个点序号的差值

            for (int i = 0; i < pointsFromPan.Length - 1; i++)
            {
                int deta2 = pointsFromPan[i + 1] - pointsFromPan[i];

                if (deta2 != deta1)
                {
                    simplifiedPoints.Add(initialPoints[pointsFromPan[i]]);
                    deta1 = deta2;
                }
            }
            simplifiedPoints.Add(initialPoints[pointsFromPan[pointsFromPan.Length - 1]]);
        }


        private static void ModifyThePositionOfStartingPoint(int[] points1)
        {
            //如果起点在最下面一行，则起点的y坐标减去0.3米
            if (points1[0] / 4 == 0)
            {
                Point newPoint = new Point();
                newPoint.xCoordinate = initialPoints[points1[0]].xCoordinate;
                newPoint.yCoordinate = initialPoints[points1[0]].yCoordinate - 0.3;
                initialPoints[points1[0]] = newPoint;
            }
            //如果起点在最上面一行，则起点的y坐标加上0.3米
            if (points1[0] / 4 == 3)
            {
                Point newPoint = new Point();
                newPoint.xCoordinate = initialPoints[points1[0]].xCoordinate;
                newPoint.yCoordinate = initialPoints[points1[0]].yCoordinate + 0.3;
                initialPoints[points1[0]] = newPoint;
            }
        }

        public static bool ReadPathFile(string filePath)
        {
            string[][] data = null;
            try
            {
                data = File.ReadAllLines(filePath).Select(x => x.Split(' ')).ToArray();
                Point[] tempPoints = new Point[POINTNUMBER];
                for (int i = 0; i < data.Length; i++)
                {
                    tempPoints[i].xCoordinate = double.Parse(data[i][0]);
                    tempPoints[i].yCoordinate = double.Parse(data[i][1]);
                    initialPoints.Add(tempPoints[i]);
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
