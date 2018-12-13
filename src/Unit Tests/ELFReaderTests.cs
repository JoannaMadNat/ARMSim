/// <summary>
/// Unit tests to test ELFLoader.cs
/// </summary>
/// 
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using armsim.Model;
using System.IO;


namespace armsim.Unit_Tests
{
    [TestClass]
    public class ELFReaderTests
    {
        [TestMethod]
        public void LoadELF1_Success()
        {
            Memory RAM = new Memory(32768);
            ELFLoader elf = new ELFLoader();
            elf.ReadELF(ref RAM, Directory.GetCurrentDirectory() + "\\Test Files\\test1.exe");

            int sum = RAM.CalculateChecksum();
            Assert.IsTrue(sum == 536861081);
        }

        [TestMethod]
        public void LoadELF2_Success()
        {
            Memory RAM = new Memory(32768);
            ELFLoader elf = new ELFLoader();
            elf.ReadELF(ref RAM, Directory.GetCurrentDirectory() + "\\Test Files\\test2.exe");

            int sum = RAM.CalculateChecksum();
            Assert.IsTrue(sum == 536864433);
        }

        [TestMethod]
        public void LoadELF3_Success()
        {
            Memory RAM = new Memory(32768);
            ELFLoader elf = new ELFLoader();
            elf.ReadELF(ref RAM, Directory.GetCurrentDirectory() + "\\Test Files\\test3.exe");

            int sum = RAM.CalculateChecksum();
            Assert.IsTrue(sum == 536861199);
        }

        [TestMethod]
        public void LoadELF_NotELF()
        {
            Memory RAM = new Memory(2084);
            ELFLoader elf = new ELFLoader();

            try
            {
                elf.ReadELF(ref RAM, Directory.GetCurrentDirectory() + "\\Test Files\\iamnotanELF.exe");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("not an ELF file"));
            }

        }

        [TestMethod]
        public void LoadELF_NoFile()
        {
            Memory RAM = new Memory(2084);
            ELFLoader elf = new ELFLoader();

            try
            {
                elf.ReadELF(ref RAM, Directory.GetCurrentDirectory() + "\\Test Files\\IdoNotExist.exe");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("does not exist"));
            }

        }




    }
}
