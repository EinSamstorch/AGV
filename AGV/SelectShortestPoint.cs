using System;
using System.Collections.Generic;
using System.Text;

namespace AGV
{
    class SelectShortestPoint
    {

        
        private const string filePath = @"D:\Documents\Visual Studio 2019\AGV\AGV\Source\coordinate.txt";
        public static int GetPoint(double x, double y)
        {
            int point = 0;
            Dictionary<Point, double> dic = new Dictionary<Point, double>();
            double minDistance = Double.MaxValue;
            
            
            Path.ReadPathFile(filePath);
            foreach (Point point1 in Path.initialPoints)
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

            Dictionary<Point, double>.Enumerator en = dic.GetEnumerator();
            for (int i = 0; i < dic.Count; i++)
            {
                if (en.MoveNext())
                {
                    double value = en.Current.Value;
                    if (value == minDistance)
                    {
                        point = i;
                        break;
                    }
                }
            }
            return (point + 1);
        }

    }
}
