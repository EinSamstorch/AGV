using Microsoft.VisualStudio.TestTools.UnitTesting;
using AGV;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGV.Tests
{
    [TestClass()]
    public class ProgramTests
    {

        [TestMethod()]
        public void ReadPathFileTest()
        {
            string filePath = @"D:\Documents\Visual Studio 2019\AGV\AGV\Source\test.txt";

            Assert.IsTrue(Program.ReadPathFile(filePath));

        }

        [TestMethod()]
        public void SimplifyPointsTest()
        {
            //int[] pointsFromPan = { 4, 5, 6, 2, 3 };
            //int[] pointsFromPan = { 4, 5, 9, 13, 17, 18, 19 };
            int[] pointsFromPan = { 8, 9, 10, 14, 18, 19};
            List<int> simplifiedPoints = new List<int>();

            Program.SimplifyPoints(pointsFromPan, simplifiedPoints);

            foreach (var item in simplifiedPoints)
            {
                Console.WriteLine(item);
            }
        }
    }
}