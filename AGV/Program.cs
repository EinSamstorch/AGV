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
        public double angle;//0 <= θ <= 3.141592
        public double xSpeed;//0.1 <= v <= 0.4 , ▲v = 0.1  转弯时速度为0。
        public double ySpeed;//该列全零
        public int label;//用来区分多个点之间的路径段，拐一次弯加1
    }

    public class Program
    {
        private const double OFFSET = 0.05;
        private const int POINTNUMBER = 60;
        public static List<Point> initialPoints = new List<Point>();
        private const string filePath = @"D:\Documents\Visual Studio 2019\AGV\AGV\Source\coordinate.txt";

        public static void Main(string[] args)
        {
            //int[] pointsFromPan = { 9, 10, 11, 7, 8 };

            //int[] pointsFromPan = { 5, 6, 10, 14, 18, 22, 23, 24 };
            int[] pointsFromPan = { 8, 7, 11, 15, 14, 13 };

            //Array.Reverse(pointsFromPan);

            //将小潘师兄传过来的从1开始的点，变成从0开始计数的点 。
            for (int i = 0; i < pointsFromPan.Length; i++)
            {
                pointsFromPan[i] -= 1;
            }

            GeneratePathFile(pointsFromPan);
            Console.ReadKey();
        }

        public static bool GeneratePathFile(int[] pointsFromPan)
        {
            return GeneratePathFile("path.txt", pointsFromPan);
        }

        public static bool GeneratePathFile(string fileName, int[] pointsFromPan)
        {
            List<Point> simplifiedPoints = new List<Point>();//简化之后要走的点
            List<Point> generatePoints = new List<Point>();//最终生成的点
            int label = 0;
            int vFlag = 1;

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
                double x2 = simplifiedPoints[i + 1].xCoordinate;
                double y2 = simplifiedPoints[i + 1].yCoordinate;
                double k = (y2 - y1) / (x2 - x1);
                double b = y1 - k * x1;
                double radian = Math.Atan(k);
                double yCount = Math.Ceiling((Math.Abs(y2 - y1) / OFFSET));
                double xCount = Math.Ceiling(Math.Abs((x2 - x1) / OFFSET));
                int yFlag = 1;
                int xFlag = 1;

                //判断x和y坐标关系
                if (y2 < y1)
                {
                    yFlag = -1;
                }
                if (x2 < x1)
                {
                    xFlag = -1;
                }

                //弧度在0~2PI之间
                if (radian < 0)
                {
                    radian += 2 * Math.PI;
                }
                //第一个点一定是倒车出来的
                if (i == 0)
                {
                    vFlag = -1;
                    radian = -radian + 2 * Math.PI;
                }
                else
                {
                    vFlag = 1;
                }
                if ((x2 - x1) == 0)
                {
                    for (int j = 0; j < yCount; j++)
                    {
                        Point point = new Point
                        {
                            xCoordinate = x1,
                            yCoordinate = y1 + OFFSET * j * yFlag,
                            angle = radian,
                            xSpeed = 0.2 * vFlag,
                            ySpeed = 0,
                            label = label
                        };
                        //AssignSpeedToXDirection(yCount, j, ref point, vFlag);
                        generatePoints.Add(point);
                    }
                }
                else if ((y2 - y1) == 0)
                {
                    //如果两个坐标的y坐标相同，x2<x1时，角度为PI而不是0
                    if (xFlag == -1)
                    {
                        radian = Math.PI;
                    }
                    for (int j = 0; j < xCount; j++)
                    {
                        Point point = new Point
                        {
                            xCoordinate = x1 + OFFSET * j * xFlag,
                            yCoordinate = y1,
                            angle = radian,
                            xSpeed = 0.2 * vFlag,
                            ySpeed = 0,
                            label = label
                        };
                        //AssignSpeedToXDirection(xCount, j, ref point, vFlag);
                        generatePoints.Add(point);
                    }
                }
                else
                {
                    for (int j = 0; j < yCount; j++)
                    {
                        Point point = new Point
                        {
                            yCoordinate = y1 + OFFSET * j * yFlag,
                            xCoordinate = (1 / k) * ((y1 + OFFSET * j * yFlag) - b),
                            angle = radian,
                            xSpeed = 0.2 * vFlag,
                            ySpeed = 0,
                            label = label
                        };
                        //AssignSpeedToXDirection(yCount, j, ref point, vFlag);
                        generatePoints.Add(point);
                    }
                }

                //给最后一个点赋值
                if (i == simplifiedPoints.Count - 2)
                {
                    Point endPoint = new Point
                    {
                        xCoordinate = x2,
                        yCoordinate = y2,
                        angle = radian,
                        xSpeed = 0,
                        ySpeed = 0,
                        label = label
                    };
                    generatePoints.Add(endPoint);
                }
                label += 1;
            }
            foreach (Point point in generatePoints)
            {
                Console.WriteLine("x:{0,-12:F4} y:{1,-12:F4} radian:{2,-12:F4} vx:{3,-12:F4} vy:{4,-12:F4} label:{5,-12:F4}"
                        , point.xCoordinate, point.yCoordinate, point.angle, point.xSpeed, point.ySpeed, point.label);
            }

            return GeneratePathFile(fileName, generatePoints);
        }

        private static bool GeneratePathFile(string fileName, List<Point> generatePoints)
        {
            try
            {
                //将数组写入到txt文件中
                StreamWriter sw = new StreamWriter(fileName, true);

                foreach (Point point in generatePoints)
                {
                    sw.WriteLine("{0,-12:F4} {1,-12:F4} {2,-12:F4} {3,-12:F4} {4,-12:F4} {5,-12:F4}"
                                , point.xCoordinate, point.yCoordinate, point.angle, point.xSpeed, point.ySpeed, point.label);
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

        /// <summary>
        /// 给x方向速度赋值，第一个点速度为0，然后速度开始增加到0.3之后保持 在接近第二个转折点的时候，速度再慢慢减到0，在转弯处速度为0
        /// </summary>
        /// <param name="yCount"></param>
        /// <param name="j"></param>
        /// <param name="point"></param>
        private static void AssignSpeedToXDirection(double yCount, int j, ref Point point, int vFlag)
        {
            if (j < 3)
            {
                point.xSpeed = vFlag * j * 0.1;
            }
            else if (yCount - j > 3)
            {
                point.xSpeed = vFlag * 0.3;
            }
            else
            {
                point.xSpeed = vFlag * (yCount - j) * 0.1;
            }
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
                //由于中间两行序号相差为1或-1的点，他们并不在同一竖直线上，也需要加进去
                if (deta2 == 1 || deta2 == -1)
                {
                    simplifiedPoints.Add(initialPoints[pointsFromPan[i]]);
                    deta1 = deta2;
                    continue;
                }
                if (deta2 != deta1)
                {
                    simplifiedPoints.Add(initialPoints[pointsFromPan[i]]);
                    deta1 = deta2;
                }
            }
            simplifiedPoints.Add(initialPoints[pointsFromPan[^1]]);
        }


        private static void ModifyThePositionOfStartingPoint(int[] points)
        {
            //如果起点在最下面一行，则起点的y坐标减去0.3米
            if (points[0] % 4 == 0)
            {
                Point newPoint = new Point
                {
                    xCoordinate = initialPoints[points[0]].xCoordinate,
                    yCoordinate = initialPoints[points[0]].yCoordinate - 0.3
                };
                initialPoints[points[0]] = newPoint;
            }
            //如果起点在最上面一行，则起点的y坐标加上0.3米
            if (points[0] % 4 == 3)
            {
                Point newPoint = new Point
                {
                    xCoordinate = initialPoints[points[0]].xCoordinate,
                    yCoordinate = initialPoints[points[0]].yCoordinate + 0.3
                };
                initialPoints[points[0]] = newPoint;
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

    }
}