/// <summary>
/// File: CPUTests.cs
/// Unit tests for CPU class
/// </summary>
/// 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using armsim.Model;


namespace armsim.Unit_Tests
{
    [TestClass]
    public class CPUTests
    {
        [TestMethod]
        public void CPU_Constructor_Success()
        {

            Memory RAM = new Memory(12), registers = new Memory(4);
            bool[] flags = new bool[4], h = new bool[2];

            CPU ceepee = new CPU(ref RAM, ref registers, ref flags, ref h);

            Assert.IsTrue(ceepee.RAM == RAM);
            Assert.IsTrue(ceepee.Registers == registers);
        }

        [TestMethod]
        public void CPU_Fetch_Success()
        {
            Memory RAM = new Memory(12), registers = new Memory(64);
            registers.WriteWord(Reg.R15, 4);
            RAM.WriteWord(registers.ReadWord(Reg.R15), 400);
            bool[] flags = new bool[4], h = new bool[2];
            CPU cpu = new CPU(ref RAM, ref registers, ref flags, ref h);
            int fetched = cpu.Fetch();
            Assert.IsTrue(fetched == 400);
        }

        ////These tests do nothing for now and will always fail.
        //[TestMethod]
        //public void CPU_Decode_Success()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod]
        //public void CPU_Execute_Success()
        //{
        //    Assert.Fail();
        //}
    }
}
