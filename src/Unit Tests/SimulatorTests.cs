using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using armsim.Model;
using System.IO;

namespace armsim.Unit_Tests
{
    [TestClass]
    public class SimulatorTests
    {
        [TestMethod]
        public void Simulator_CLevel()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\ctestTest.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\ctest.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\ctest.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_BLevel()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\btestTest.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\btest.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\btest.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        //Test files by Zac Hayes
        [TestMethod]
        public void Simulator_pcTest()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\pctestTest.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\pctest.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\pctest.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Trim().Equals(exp)); 
            }
        }

        //test files by Jonathon Pridgeon
        [TestMethod]
        public void Simulator_stackTest()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\stacktestTest.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\stacktest.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\stacktest.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Trim().Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_Branch()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\branchTest.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\branch.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\branch.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_Cmp()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\cmpTest.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\cmp.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\cmp.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_Mersenne_no_io()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\mersenne_no_ioTest.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\mersenne_no_io.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\mersenne_no_io.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_locals_no_io()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\locals_no_ioTest.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\locals_no_io.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\locals_no_io.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_quicksort_no_io()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\quicksort_no_ioTest.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\quicksort_no_io.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\quicksort_no_io.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_btestldr()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\btestldr_Test.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\btestldr.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\btestldr.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_bteststr()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\bteststr_Test.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\bteststr.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\bteststr.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_ctest()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\ctest_Test.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\ctest.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\ctest.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_ctestmine()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\ctestmine_Test.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\ctestmine.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\ctestmine.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_ldmstm()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\ldmstm_Test.log";
            string expectedlogFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\ldmstm.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\ldmstm.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
            string[] expected = File.ReadAllLines(expectedlogFile);

            Assert.IsTrue(result.Length == expected.Length);
            for (int i = 0; i < expected.Length; ++i)
            {
                string res = result[i], exp = expected[i];
                Assert.IsTrue(res.Equals(exp));
            }
        }

        [TestMethod]
        public void Simulator_quicksort()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\io\\quicksort_test.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\io\\quicksort.exe";

                Computer euler = new Computer(32768, logfile, true);
                euler.LoadFileToRAM(testFile);
                euler.Run();
                euler.CloseTraceFile();

                string[] result = File.ReadAllLines(logfile);
}

        [TestMethod]
        public void Simulator_sieve()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\io\\sieve_test.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\simTests\\sim2\\io\\sieve.exe";

                Computer euler = new Computer(32768, logfile, true);
                euler.LoadFileToRAM(testFile);
                euler.Run();
                euler.CloseTraceFile();

                string[] result = File.ReadAllLines(logfile);
        }

        //by Jonathon Pridgeon
        [TestMethod]
        public void Simulator_strTest()
        {
            string logfile = Directory.GetCurrentDirectory() + "\\Test Files\\strtest\\strtest.log";
            string testFile = Directory.GetCurrentDirectory() + "\\Test Files\\strtest\\strtest.exe";

            Computer euler = new Computer(32768, logfile, true);
            euler.LoadFileToRAM(testFile);
            euler.Run();
            euler.CloseTraceFile();

            string[] result = File.ReadAllLines(logfile);
        }
    }
}
