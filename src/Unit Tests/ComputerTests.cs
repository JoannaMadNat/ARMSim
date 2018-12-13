/// <summary>
/// Unit tests for the Computer class.
/// </summary>

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using armsim.Model;
using System.IO;


namespace armsim.Unit_Tests
{
    [TestClass]
    public class ComputerTests
    {
        [TestMethod]
        public void Computer_Constructor_Success()
        {
            Computer turing = new Computer(12, "Jojo", false);
            Assert.IsTrue(turing.RAM.MemoryArray.Length == 12);
            Assert.IsTrue(turing.Registers.MemoryArray.Length == 92);
            Assert.IsTrue(turing.traceFileName == "Jojo");
            Assert.IsTrue(turing.trace == false);
            Assert.IsTrue(turing.cpu.RAM == turing.RAM);
            Assert.IsTrue(turing.cpu.Registers == turing.Registers);
        }

        [TestMethod]
        public void Computer_LoadFiletoRAM_Test1_Success()
        {
            Computer turing = new Computer(32768, "", false);
            turing.LoadFileToRAM(Directory.GetCurrentDirectory() + "\\Test Files\\test1.exe");

            int sum = turing.CheckSum();
            Assert.IsTrue(sum == 536861081);
        }

        [TestMethod]
        public void Computer_LoadFiletoRAM_Test2_Success()
        {
            Computer turing = new Computer(32768, "", false);

            turing.LoadFileToRAM(Directory.GetCurrentDirectory() + "\\Test Files\\test2.exe");

            int sum = turing.CheckSum();
            Assert.IsTrue(sum == 536864433);
        }

        [TestMethod]
        public void Computer_LoadFiletoRAM_Test3_Success()
        {
            Computer turing = new Computer(32768, "", false);

            turing.LoadFileToRAM(Directory.GetCurrentDirectory() + "\\Test Files\\test3.exe");

            int sum = turing.CheckSum();
            Assert.IsTrue(sum == 536861199);
        }

        [TestMethod]
        public void Computer_ResetRAMandRegs_Success()
        {
            Computer turing = new Computer(12, "", false);

            for (int i = 0; i < turing.RAM.MemoryArray.Length; ++i)
                turing.RAM.WriteByte(i, 8);
            for (int i = 0; i < turing.Registers.MemoryArray.Length; ++i)
                turing.Registers.WriteByte(i, 8);

            turing.ResetRAMandRegs();

            int sum = 0;

            for (int i = 0; i < turing.RAM.MemoryArray.Length; ++i)
                sum+= turing.RAM.ReadByte(i);
            for (int i = 0; i < turing.Registers.MemoryArray.Length; ++i)
                sum += turing.Registers.ReadByte(i);

            Assert.IsTrue(sum == 0);
        }

        [TestMethod]
        public void Computer_ResetPC_Success()
        {
            Computer turing = new Computer(32768, "", false);
            turing.LoadFileToRAM(Directory.GetCurrentDirectory() + "\\Test Files\\test1.exe");
            turing.PC += 90;

            turing.ResetPCSPandCPSR();

            Assert.IsTrue(turing.PC == 312);
        }

        [TestMethod]
        public void Computer_Run_Success()
        {
            Computer turing = new Computer(12, "", false);
            for(int i=0; i<turing.RAM.MemoryArray.Length - 4; i+=4)
                turing.RAM.WriteWord(i, 1964);

            uint r = 0xef000011;
            int halt = (int)~r; halt = ~halt;
            turing.RAM.WriteWord(turing.RAM.MemoryArray.Length - 4, halt);
            turing.PC = 0;

            turing.Run();

            Assert.IsTrue(true);
            Assert.IsTrue(turing.CheckSum() == 638);
            Assert.IsTrue(turing.PC == 12);
            Assert.IsTrue(turing.stepNum == 4);
        }

        [TestMethod]
        public void Computer_Step_Success()
        {
            Computer turing = new Computer(32768, "", false);
            turing.LoadFileToRAM(Directory.GetCurrentDirectory() + "\\Test Files\\test1.exe");

            int PC1 = turing.PC;
            turing.Step();
            turing.Step();
            turing.Step();

            Assert.IsTrue(turing.PC - PC1 == 12);

        }

        [TestMethod]
        public void Computer_TraceFormatCheck_Success()
        {
            string traceFile = Directory.GetCurrentDirectory() + "\\Test Files\\testTrace.log";
            Computer turing = new Computer(12, traceFile, true);
            turing.ResetRAMandRegs();
            turing.Registers.WriteWord(Reg.CPSR, Mode.System);

            //Make sure file exists
            Assert.IsTrue(File.Exists(traceFile));

            //File is already open
            try
            {
                turing.OpenTraceFile();
                Assert.Fail();
            }
            catch(Exception e)
            {
                Assert.IsTrue(e.Message.Contains("used by another process"));
            }

            //Write to file
            turing.TraceStep(0);
            turing.CloseTraceFile();

            string line = File.ReadAllText(traceFile);
            Assert.IsTrue(line == "000001 00000000 00000042 0000 SYS 0=00000000 1=00000000 2=00000000 3=00000000 4=00000000 5=00000000 6=00000000 7=00000000 8=00000000 9=00000000 10=00000000 11=00000000 12=00000000 13=00000000 14=00000000 \r\n");
        }

    }


}
