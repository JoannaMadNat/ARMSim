/// <summary>
/// Unit tests to test RAM.cs
/// </summary>
/// 
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using armsim.Model;

namespace armsim.Unit_Tests
{
    [TestClass]
    public class RAMTests
    {
        byte[] testValues;
        public RAMTests()
        {
            testValues = new byte[12];
            for (int i = 0; i < 12; ++i)
                testValues[i] = (byte)i;
        }

        [TestMethod]
        public void ReadWord_Success()
        {
            Memory RAM = new Memory(12);
            RAM.SetRAM(testValues, 12);

            int res = RAM.ReadWord(4);
            Assert.IsTrue(res == 0x07060504);
        }

        [TestMethod]
        public void ReadWord_WrongAddress()
        {
            Memory RAM = new Memory(12);
            RAM.SetRAM(testValues, 12);

            try
            {
                int res = RAM.ReadWord(3);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("Invalid memory"));
            }
        }
        [TestMethod]
        public void ReadWord_OutOfRange()
        {
            Memory RAM = new Memory(12);
            RAM.SetRAM(testValues, 12);


            try
            {
                int res = RAM.ReadWord(15);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("out of range"));
            }
        }

        [TestMethod]
        public void ReadHalfWord_Success()
        {
            Memory RAM = new Memory(12);
            RAM.SetRAM(testValues, 12);

            int res = RAM.ReadHalfWord(2);
            Assert.IsTrue(res == 0x0302);
        }
        [TestMethod]
        public void ReadHalfWord_OutOfRange()
        {
            Memory RAM = new Memory(12);
            RAM.SetRAM(testValues, 12);


            try
            {
                int res = RAM.ReadHalfWord(15);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("out of range"));
            }
        }
        [TestMethod]
        public void ReadHalfWord_WrongAddress()
        {
            Memory RAM = new Memory(12);
            RAM.SetRAM(testValues, 12);


            try
            {
                int res = RAM.ReadHalfWord(3);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("Invalid memory"));
            }
        }

        [TestMethod]
        public void ReadByte_Success()
        {
            Memory RAM = new Memory(12);
            RAM.SetRAM(testValues, 12);

            int res = RAM.ReadByte(3);
            Assert.IsTrue(res == 0x3);
        }
        [TestMethod]
        public void ReadByte_OutOfRange()
        {
            Memory RAM = new Memory(12);
            RAM.SetRAM(testValues, 12);

            try
            {
                int res = RAM.ReadByte(15);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("out of range"));
            }
        }

        //***********************************************Write tests
        [TestMethod]
        public void WriteWord_Success()
        {
            Memory RAM = new Memory(12);

            RAM.WriteWord(4, 0x19961977);
            Assert.IsTrue(RAM.MemoryArray[4] == 0x77);
            Assert.IsTrue(RAM.MemoryArray[5] == 0x19);
            Assert.IsTrue(RAM.MemoryArray[6] == 0x96);
            Assert.IsTrue(RAM.MemoryArray[7] == 0x19);


        }
        [TestMethod]
        public void WriteWord_WrongAddress()
        {
            Memory RAM = new Memory(12);

            try
            {
                RAM.WriteWord(3, 0x7654);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("Invalid memory"));
            }
        }
        [TestMethod]
        public void WriteWord_OutOfRange()
        {
            Memory RAM = new Memory(12);

            try
            {
                RAM.WriteWord(15, 0x7654);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("out of range"));
            }
        }

        [TestMethod]
        public void WriteHalfWord_Success()
        {
            Memory RAM = new Memory(12);

            RAM.WriteHalfWord(2, 0x3223);
            Assert.IsTrue(RAM.MemoryArray[2] == 0x23);
            Assert.IsTrue(RAM.MemoryArray[3] == 0x32);
        }
        [TestMethod]
        public void WriteHalfWord_OutOfRange()
        {
            Memory RAM = new Memory(12);

            try
            {
                RAM.WriteHalfWord(15, 0x32);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("out of range"));
            }
        }
        [TestMethod]
        public void WriteHalfWord_WrongAddress()
        {
            Memory RAM = new Memory(12);

            try
            {
                RAM.WriteHalfWord(3, 0x32);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("Invalid memory"));
            }
        }

        [TestMethod]
        public void WriteByte_Success()
        {
            Memory RAM = new Memory(12);

            RAM.WriteByte(2, 0x2);
            Assert.IsTrue(RAM.MemoryArray[2] == 0x02);
        }
        [TestMethod]
        public void WriteByte_OutOfRange()
        {
            Memory RAM = new Memory(12);

            try
            {
                RAM.WriteByte(15, 0x2);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("out of range"));
            }
        }

        [TestMethod]
        public void TestFlag_Success()
        {
            Memory RAM = new Memory(12);
            RAM.WriteWord(4, 0x55555555);
            Assert.IsTrue(RAM.TestFlag(4, 4));
            Assert.IsFalse(RAM.TestFlag(4, 3));
        }
        [TestMethod]
        public void TestFlag_OutOfRange()
        {
            Memory RAM = new Memory(12);
            try
            {
                RAM.TestFlag(100, 2);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("out of range"));
            }
        }
        [TestMethod]
        public void TestFlag_WrongBitSize()
        {
            Memory RAM = new Memory(12);
            try
            {
                RAM.TestFlag(2, 100);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("out of range of word size"));
            }
        }
        [TestMethod]
        public void SetFlag_Success()
        {
            Memory RAM = new Memory(12);
            RAM.WriteWord(4, 0x55555555);
            RAM.SetFlag(4, 0, false);
            Assert.IsFalse(RAM.TestFlag(4, 0));

            RAM.SetFlag(4, 1, true);
            Assert.IsTrue(RAM.TestFlag(4, 1));

            Assert.IsTrue(RAM.ReadWord(4) == 0x55555556);
        }
        [TestMethod]
        public void SetFlag_OutOfRange()
        {
            Memory RAM = new Memory(12);
            try
            {
                RAM.SetFlag(100, 2, false);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("out of range"));
            }
        }
        [TestMethod]
        public void SetFlag_WrongBitSize()
        {
            Memory RAM = new Memory(12);
            try
            {
                RAM.SetFlag(2, 100, true);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.IsTrue(e.Message.Contains("out of range of word size"));
            }
        }

        [TestMethod]
        public void ExtractBits_Success()
        {
            int res = Memory.ExtractBits(0xb5, 1, 3);
            Assert.IsTrue(res == 0x04);
        }
        public void ExtractBits_WrongBitSize()
        {
            Memory.ExtractBits(0x3456, 77, 60);
        }

        [TestMethod]
        public void ConsistentData_Word()
        {
            Memory RAM = new Memory(12);

            int first = 0x19641234, second = 0x19962345, third = 0x20080006;
            RAM.WriteWord(0, first);
            RAM.WriteWord(4, second);
            RAM.WriteWord(8, third);

            int ret1 = RAM.ReadWord(0);
            int ret2 = RAM.ReadWord(4);
            int ret3 = RAM.ReadWord(8);

            Assert.IsTrue(ret1 == first);
            Assert.IsTrue(ret2 == second);
            Assert.IsTrue(ret3 == third);

        }
        [TestMethod]
        public void ConsistentData_HalfWord()
        {
            Memory RAM = new Memory(12);


            short first = 0x1964, second = 0x1996;
            RAM.WriteHalfWord(0, first);
            RAM.WriteHalfWord(2, second);

            short ret1 = RAM.ReadHalfWord(0);
            short ret2 = RAM.ReadHalfWord(2);

            Assert.IsTrue(ret1 == first);
            Assert.IsTrue(ret2 == second);
        }
    }
}
