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

            Assert.IsTrue(Path.ReadPathFile(filePath));

        }

    }
}