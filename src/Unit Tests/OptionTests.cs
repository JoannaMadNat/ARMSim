/// <summary>
/// Unit tests to test Options.cs
/// </summary>
/// 
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using armsim.Model;


namespace armsim.Unit_Tests
{
    [TestClass]
    public class OptionTests
    {
        [TestMethod]
        public void Options_Success()
        {
            Options options = new Options();
            options.ParseArgs(new string[] { "--mem", "700", "--l", "jojo" });

            Assert.IsTrue(options.FileName == "jojo");
            Assert.IsTrue(options.Memory_Size == 700);
        }

        [TestMethod]
        public void Options_NoArg_Size_Success()
        {
            Options options = new Options();
                options.ParseArgs(new string[] { "--mem" });
            Assert.IsTrue(options.Memory_Size == 32768);

        }
        [TestMethod]
        public void Options_NoArg_Load()
        {
            Options options = new Options();
            try
            {
                options.ParseArgs(new string[] { "--l" });
                Assert.Fail();
            }
            catch(Exception e)
            {
                Assert.IsTrue(e.Message.Contains("no file name"));
            }
        }

        [TestMethod]
        public void Options_JustMem_Success()
        {
            Options options = new Options();
            options.ParseArgs(new string[] { "--mem", "700" });
            Assert.IsTrue(options.Memory_Size == 700);
        }

        [TestMethod]
        public void Options_JustLoad_Success()
        {
            Options options = new Options();
            options.ParseArgs(new string[] { "--l", "jojo" });
            Assert.IsTrue(options.FileName == "jojo");
        }

        [TestMethod]
        public void Options_BlankName()
        {
            Options options = new Options();
            try
            {
                options.ParseArgs(new string[] { "--l", "" });
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("no file name"));
            }
        }

        [TestMethod]
        public void Options_NoMemandLoad()
        {
            Options options = new Options();
           options.ParseArgs(new string[] { "--mem", "--l", "jojo" });
            Assert.IsTrue(options.Memory_Size == 32768);
            Assert.IsTrue(options.FileName == "jojo");


        }

        [TestMethod]
        public void Options_NoLoadandMem()
        {
            Options options = new Options();
            try
            {
                options.ParseArgs(new string[] { "--l", "--mem", "12" });
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("no file name"));
                Assert.IsTrue(options.FileName == null);
            }
        }
    }
}
