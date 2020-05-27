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

    public class Path
    {
        private const double OFFSET = 0.05;
        private const double SPEED = 0.3;
        private const int POINTNUMBER = 60;
        public static List<Point> initialPoints = new List<Point>();
        private const string filePath = @"D:\Documents\Visual Studio 2019\AGV\AGV\Source\coordinate.txt";

        //public Path()
        //{
            
        //}

        public static void Main(string[] args)
        {
            //ReadPathFile(filePath);
            //int[] pointsFromPan = { 9, 10, 11, 7, 8 };

            //int[] pointsFromPan = { 5, 6, 10, 14, 18, 22, 23, 24 };
            //int[] pointsFromPan = { 9, 10, 14, 18, 17 };
            //int[] pointsFromPan = { 3, 7, 11, 14, 15, 14,13 };
            //int[] pointsFromPan = { 8, 7, 11, 15, 14, 13 };
            int[] pointsFromPan = { 13, 14, 10, 6, 2 };
            //int[] pointsFromPan = { 13, 14, 18, 22, 26, 30, 31, 32 };
            //int[] pointsFromPan = { 3, 7, 11, 15, 19, 18, 17 };
            //int[] pointsFromPan = { 57,58,59,55,51,47,43,39,35,31,27,23,19,15,11,7,3 };
            //int[] pointsFromPan = { 6, 7, 11, 15, 11, 10 };
            //int[] pointsFromPan = { 2, 6, 7, 3 };
            string[] sNums;
            int[] iNums;
            while (true)
            {
               sNums = Console.ReadLine().Split(",");
               iNums = Array.ConvertAll<string, int>(sNums, int.Parse);
               GeneratePathFile(iNums);
            }
            
               


            









            //Array.Reverse(pointsFromPan);

            //将小潘师兄传过来的从1开始的点，变成从0开始计数的点 。
            for (int i = 0; i < pointsFromPan.Length; i++)
            {
                pointsFromPan[i] -= 1;
            }

            GeneratePathFile(pointsFromPan);
            double x = -1.6;
            double y = 0;
            //SelectShortestPoint.GetPoint(x, y);
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
            int label = -1;
            int vFlag = 1;

            //如果读取文件失败则返回false
            if (!ReadPathFile(filePath))
            {
                return false;
            }
            

            // 修改起点位置
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

                int xFlag = (x2 < x1) ? -1 : 1;
                int yFlag = (y2 < y1) ? -1 : 1;

                // 四个象限角度的转化
                if (x2 >= x1 && y2 > y1)
                {
                    radian = radian;
                }
                else if (x2 < x1 && y2 >= y1)
                {
                    radian = radian + Math.PI;
                }
                else if (x2 < x1 && y2 < y1)
                {
                    radian = radian + Math.PI;
                }
                else
                {
                    radian = radian + 2 * Math.PI;
                }

                //第一个点一定是倒车出来的
                if (i == 0)
                {
                    vFlag = -1;
                // 如果初始算出的路径角度在0~pi，则车头的方向是radian + pi，如果是pi~2pi，则车头方向是radian - Pi
                    radian = (radian <= Math.PI) ? (radian + Math.PI) : (radian - Math.PI);
                }
                else
                {
                    vFlag = 1;
                }
                // x坐标相同
                if ((x2 - x1) == 0)
                {
                    for (int j = 0; j < yCount; j++)
                    {
                        Point point = new Point
                        {
                            xCoordinate = x1,
                            yCoordinate = y1 + OFFSET * j * yFlag,
                            angle = radian,
                            xSpeed = SPEED * vFlag,
                            ySpeed = 0,
                            label = label
                        };
                        //AssignSpeedToXDirection(yCount, j, ref point, vFlag);
                        generatePoints.Add(point);
                    }
                }
                // y坐标相同
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
                            xSpeed = SPEED * vFlag,
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
                            xSpeed = SPEED * vFlag,
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
                        xSpeed = SPEED,
                        ySpeed = 0,
                        label = label
                    };
                    generatePoints.Add(endPoint);
                }
                label -= 1;
            }
            foreach (Point point in generatePoints)
            {
                Console.WriteLine("x:{0,-12:F4} y:{1,-12:F4} radian:{2,-12:F4} vx:{3,-12:F4} vy:{4,-12:F4} label:{5,-12:F4}"
                        , point.xCoordinate, point.yCoordinate, point.angle, point.xSpeed, point.ySpeed, point.label);
            }
            // 由于从坐标文件中读取的坐标数据其实已经被改变了，因此需要清空后，方便下一次重新写入。
            initialPoints.Clear();
            return GeneratePathFile(fileName, generatePoints);
        }

        private static bool GeneratePathFile(string fileName, List<Point> generatePoints)
        {
            try
            {
                //将数组写入到txt文件中
                //FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);                 
                StreamWriter sw = new StreamWriter(fileName, false);

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
        /// 将同一条直线上的几个点合并成两个点，方便后面进行加减速控制。
        /// </summary>
        /// <param name="pointsFromPan"></param>
        /// <param name="simplifiedPoints"></param>
        public static void SimplifyPoints(int[] pointsFromPan, List<Point> simplifiedPoints)
        {
            int deta1 = 0; //用于判断前后两个点序号的差值


            for (int i = 0; i < pointsFromPan.Length - 1; i++)
            {
                // 小车在两个出库口和入库口之间运动
                if ((pointsFromPan[0] == 1 && pointsFromPan[^1] == 2 )|| (pointsFromPan[0] == 2 && pointsFromPan[^1] == 1))
                {
                    simplifiedPoints.Add(initialPoints[pointsFromPan[0]]);
                    Point midPoint1 = new Point
                    {
                        xCoordinate = initialPoints[pointsFromPan[1]].xCoordinate,
                        yCoordinate = initialPoints[pointsFromPan[0]].yCoordinate
                    };
                    Point midPoint2 = new Point
                    {
                        xCoordinate = initialPoints[pointsFromPan[1]].xCoordinate,
                        yCoordinate = initialPoints[pointsFromPan[pointsFromPan.Length - 1]].yCoordinate
                    };
                    simplifiedPoints.Add(midPoint1);
                    simplifiedPoints.Add(midPoint2);
                    break;
                }

                // 如果起点是仓库
                if (pointsFromPan[0] == 1 || pointsFromPan[0] == 2)
                {
                    simplifiedPoints.Add(initialPoints[pointsFromPan[0]]);
                    Point midPoint = new Point
                    {
                        xCoordinate = initialPoints[pointsFromPan[^1]].xCoordinate,
                        yCoordinate = initialPoints[pointsFromPan[0]].yCoordinate
                    };
                    simplifiedPoints.Add(midPoint);
                    break;
                }

                // 如果终点是仓库
                if (pointsFromPan[^1] == 1 || pointsFromPan[^1] == 2)
                {
                    simplifiedPoints.Add(initialPoints[pointsFromPan[0]]);
                    Point midPoint = new Point
                    {
                        xCoordinate = initialPoints[pointsFromPan[0]].xCoordinate,
                        yCoordinate = initialPoints[pointsFromPan[^1]].yCoordinate
                    };
                    simplifiedPoints.Add(midPoint);
                    break;
                }

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
            else if (points[0] % 4 == 3)
            {
                Point newPoint = new Point
                {
                    xCoordinate = initialPoints[points[0]].xCoordinate,
                    yCoordinate = initialPoints[points[0]].yCoordinate + 0.3
                };
                initialPoints[points[0]] = newPoint;
            }
            //如果起点在仓库，则x坐标减去0.3m
            else if (points[0] == 1 || points[0] == 2){
                Point newPoint = new Point
                {
                    xCoordinate = initialPoints[points[0]].xCoordinate - 0.3,
                    yCoordinate = initialPoints[points[0]].yCoordinate
                };
                initialPoints[points[0]] = newPoint;
            }
        }

        // 获取初始点位
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