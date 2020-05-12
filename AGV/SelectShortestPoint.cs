using System;
using System.Collections.Generic;
using System.Text;

namespace AGV
{
    class SelectShortestPoint
    {
        private const string filePath = @"D:\Documents\Visual Studio 2019\AGV\AGV\Source\coordinate.txt";
        public static Point GetPoint(double x, double y)
        {
            Point point = new Point();
            Dictionary<Point, double> dic = new Dictionary<Point, double>();
            double minDistance = Double.MaxValue;
            
            Program.ReadPathFile(filePath);
            foreach (Point point1 in Program.initialPoints)
            {
                double x2 = point1.xCoordinate;
                double y2 = point1.yCoordinate;
                double distance = (x - x2) * (x - x2) + (y - y2) * (y - y2);
                if (distance <= minDistance)
                {
                    minDistance = distance;
                }
                dic.Add(point1, distance);
            }

            foreach (KeyValuePair<Point, double> kvp in dic)
            {
                if (kvp.Value == minDistance)
                {
                    point = kvp.Key;
                }
            }
            Console.WriteLine("x:{0,-8:F4},y:{1,-8:F4}", point.xCoordinate, point.yCoordinate);
            return point;
        }

    }
}
