using Microsoft.VisualStudio.TestTools.UnitTesting;
using armsim.Model.Decoders;
using armsim.Model;
using System;

/// <summary>
/// File: DecoderTests.cs
/// Unit tests for Decoders sub-folder classes
/// </summary>

namespace armsim.Unit_Tests
{
    [TestClass]
    public class DecodersTests
    {
        int regsize = 68;

        [TestMethod]
        public void Instruction_DataProcessing()
        {
            //e3a02030 mov r2, #48
            uint u = 0xe3a02030;
            int r = Convert.ToInt32(~u);
            r = ~r;
            bool[] h = new bool[6];

            Memory mem = new Memory(12), regs = new Memory(64);
            Instruction inst = Instruction.Make(r, ref regs, ref mem, ref h);

            Assert.IsTrue(inst.ToString() == "mov r2, #48");
        }

        [TestMethod]
        public void Instruction_Load()
        {
            //LOAD :  e51b3008   ldr r3, [fp, #-8]
            uint u = 0xe51b3008;
            int r = Convert.ToInt32(~u);
            r = ~r;
            bool[] h = new bool[6];

            Memory mem = new Memory(12), regs = new Memory(regsize);
            Instruction inst = Instruction.Make(r, ref regs, ref mem, ref h);

            Assert.IsTrue(inst.ToString() == "ldr r3, [fp, #-8]");
        }

        [TestMethod]
        public void Instruction_Store()
        {
            Memory mem = new Memory(12), regs = new Memory(regsize);

            //STORE :   e50b300c        str     r3, [fp, #-12]
            uint u = 0xe50b300c;
            int r = Convert.ToInt32(~u);
            r = ~r;
            bool[] h = new bool[6];

            Instruction inst = Instruction.Make(r, ref regs, ref mem, ref h);

            Assert.IsTrue(inst.ToString() == "str r3, [fp, #-12]"); 
        }

        [TestMethod]
        public void Instruction_LSMultiple()
        {
            //LS Multiple e8bd0800        ldmfd   sp!, {fp}
            uint u = 0xe8bd0800;
            int r = Convert.ToInt32(~u);
            r = ~r;
            bool[] h = new bool[6];

            Memory mem = new Memory(12), regs = new Memory(regsize);
            Instruction inst = Instruction.Make(r, ref regs, ref mem, ref h);

            Assert.IsTrue(inst.ToString() == "pop {fp}");
        }

        [TestMethod]
        public void Instruction_Branch() //Not implemented yet
        {
            //     ea000006        b       1074 <mystart+0x3c>
            uint u = 0xea000006;
            int r = Convert.ToInt32(~u);
            r = ~r;
            bool[] h = new bool[6];

            Memory mem = new Memory(12), regs = new Memory(regsize);
            Instruction inst = Instruction.Make(r, ref regs, ref mem, ref h);

            Assert.IsTrue(inst.ToString() == "B 0x00000020");
        }

        [TestMethod]
        public void DecExec_DP_Imm_Mov_Success()
        {
            //e3a02030 mov r2, #48
            Memory regs = new Memory(regsize);
            //using not on original command code because c# won't let me alter the sign bit.
            int r = ~0x1C5FDFCF; //Original command: e3a02030 
            DataProcessing inst = new DataProcessing(r, ref regs);
            inst.Decode();
            inst.Execute();

            Assert.IsTrue(inst.ToString() == "mov r2, #48");
            Assert.IsTrue(regs.ReadWord(Reg.R2) == 48);
        }

        [TestMethod]
        public void DecExec_DP_Imm_Mov2_Success()
        {
            // mov     r0, #724
            uint u = 0xe3a00fb5;
            int r = Convert.ToInt32(~u);
            r = ~r;
            Memory regs = new Memory(regsize);
            DataProcessing inst = new DataProcessing(r, ref regs);
            inst.Decode();
            inst.Execute();

            Assert.IsTrue(regs.ReadWord(Reg.R0) == 724);
            Assert.IsTrue(inst.ToString() == "mov r0, #724");
        }

        [TestMethod]
        public void DecExecDP_Imm_Success()
        {
            //test 1 : e3a02030 mov r2, #48
            Memory regs = new Memory(regsize);
            //using not on original command code because c# won't let me alter the sign bit.
            int r = ~0x1C5FDFCF; //Original command: e3a02030 
            Instruction inst = new DataProcessing(r, ref regs);
            inst.Decode();
            inst.Execute();

            Assert.IsTrue(inst.ToString() == "mov r2, #48");
            Assert.IsTrue(regs.ReadWord(Reg.R2) == 48);
        }

        [TestMethod]
        public void DecExecDP_Imm_MOV_Success()
        {
            Memory regs = new Memory(regsize);

            //  mov     r1, #-1593835520
            uint u = 0xe3a014a1;
            int r = Convert.ToInt32(~u);
            r = ~r; DataProcessing inst = new DataProcessing(r, ref regs);
            inst.Decode();
            inst.Execute();

            Assert.IsTrue(regs.ReadWord(Reg.R1) == -1593835520);
            Assert.IsTrue(inst.ToString() == "mov r1, #-1593835520");
        }

        [TestMethod]
        public void DecExecDP_RegImm_Success()
        {
            // eor r1, r2, r3, lsr #2
            uint u = 0b11100000001000100001000100100011;
            int r = Convert.ToInt32(~u);
            r = ~r;

            Memory regs = new Memory(regsize);
            regs.WriteWord(Reg.R2, 4);
            regs.WriteWord(Reg.R3, 3);

            //using not on original command code because c# won't let me alter the sign bit.
            Instruction inst = new DataProcessing(r, ref regs);
            inst.Decode();
            inst.Execute();

            int exp = 3 >> 2;
            int res = regs.ReadWord(Reg.R1);
            Assert.IsTrue(res == (4 ^ exp));
            Assert.IsTrue(inst.ToString() == "eor r1, r2, r3, lsr #2");
        }

        [TestMethod]
        public void DecExecDP_RegImm_MOV_Success()
        {
            Memory regs = new Memory(regsize);

            // mov     r0, #724
            regs.WriteWord(Reg.R2, 4);
            regs.WriteWord(Reg.R0, 3);

            uint u = 0xe1a02000;
            int r = Convert.ToInt32(~u);
            r = ~r; DataProcessing inst = new DataProcessing(r, ref regs);
            inst.Decode();
            inst.Execute();

            Assert.IsTrue(regs.ReadWord(Reg.R2) == 3);
            Assert.IsTrue(inst.ToString() == "mov r2, r0");

        }

        [TestMethod]
        public void DecExecDP_RegImm_ASR_Success()
        {
            Memory regs = new Memory(regsize);

            //  e1a02141        asr r2, r1, #2
            regs.WriteWord(Reg.R2, 0x1014);
            uint uop = 0xA1000000; int op = (int)~uop; op = ~op;
            regs.WriteWord(Reg.R1, op);

            uint u = 0xe1a02141;
            int r = Convert.ToInt32(~u);
            r = ~r; DataProcessing inst = new DataProcessing(r, ref regs);
            inst.Decode();
            inst.Execute();

            uop = 0xE8400000; op = (int)~uop; op = ~op;
            Assert.IsTrue(regs.ReadWord(Reg.R2) == op);
            Assert.IsTrue(inst.ToString() == "mov r2, r1, asr #2");
        }

        [TestMethod]
        public void DecExecDP_RegImm_ORR_Success()
        {
            Memory regs = new Memory(regsize);

            //  e3802012        orr r2, r0, #18
            regs.WriteWord(Reg.R2, 0x1014);
            uint uop = 0x000002D4; int op = (int)~uop; op = ~op;
            regs.WriteWord(Reg.R0, op);

            uint u = 0xe3802012;
            int r = Convert.ToInt32(~u);
            r = ~r;
            DataProcessing inst = new DataProcessing(r, ref regs);
            inst.Decode();
            inst.Execute();

            uop = 0x000002D6; op = (int)~uop; op = ~op;
            Assert.IsTrue(regs.ReadWord(Reg.R2) == op);
            Assert.IsTrue(inst.ToString() == "orr r2, r0, #18");
        }

        [TestMethod]
        public void DecExecDP_RegReg_ADD_Success()
        {
            //add r1, r2, r3, ror r4
            uint u = 0b11100000100000100001010001110011;
            int r = Convert.ToInt32(~u);
            r = ~r;

            Memory regs = new Memory(regsize);
            regs.WriteWord(Reg.R2, 4);
            regs.WriteWord(Reg.R3, 3);
            regs.WriteWord(Reg.R4, 5);

            //using not on original command code because c# won't let me alter the sign bit.
            Instruction inst = new DataProcessing(r, ref regs);
            inst.Decode();
            inst.Execute();

            int exp = (((3) >> (5)) | ((3) << (32 - (5)))); //ror
            int res = regs.ReadWord(Reg.R1);
            Assert.IsTrue(res == (4 + exp));
            Assert.IsTrue(inst.ToString() == "add r1, r2, r3, ror r4");
        }

        [TestMethod]
        public void DecExecDP_RegReg_MUL_Success()
        {
            // e0050291        mul     r5, r1, r2
            Memory regs = new Memory(regsize);

            regs.WriteWord(Reg.R2, 0x1014);
            uint uop = 0xA1000000; int op = (int)~uop; op = ~op;
            regs.WriteWord(Reg.R1, op);
            regs.WriteWord(Reg.R2, 0x1050);
            uint u = 0xe0050291;
            int r = Convert.ToInt32(~u); r = ~r;

            Instruction inst = new DataProcessing(r, ref regs);

            inst = new DataProcessing(r, ref regs);
            inst.Decode();
            inst.Execute();

            uop = 0x50000000; op = (int)~uop; op = ~op;
            int res = regs.ReadWord(Reg.R5);
            Assert.IsTrue(res == op);
            Assert.IsTrue(inst.ToString() == "mul r5, r1, r2");
        }

        [TestMethod]
        public void DecExecLS_Imm_Store_Success()
        {
            //Test 1 STORE :  e50b3008        str r3, [fp, #-8]
            uint u = 0xe50b3008;
            int r = Convert.ToInt32(~u);
            r = ~r;

            Memory regs = new Memory(regsize), RAM = new Memory(12);
            regs.WriteWord(Reg.R11, 8);
            regs.WriteWord(Reg.R3, 23);

            LoadStore inst = new LoadStore(r, ref regs, ref RAM);
            inst.Decode();
            inst.Execute();

            Assert.IsTrue(RAM.ReadWord(0) == 23);
            Assert.IsTrue(inst.ToString() == "str r3, [fp, #-8]");

        }

        [TestMethod]
        public void DecExecLS_Imm_Load_Success()
        {

            //Test 2 LOAD :   e51b3008        ldr r3, [fp, #-8]
            uint u = 0xe51b3008;
            int r = Convert.ToInt32(~u);
            r = ~r;
            Memory regs = new Memory(regsize), RAM = new Memory(12);

            regs.WriteWord(Reg.R11, 8);
            RAM.WriteWord(0, 23);

            LoadStore inst = new LoadStore(r, ref regs, ref RAM);
            inst.Decode();
            inst.Execute();

            Assert.IsTrue(regs.ReadWord(Reg.R3) == 23);
            Assert.IsTrue(inst.ToString() == "ldr r3, [fp, #-8]");
        }

        [TestMethod]
        public void DecExecLS_RegImm_Store_Success()
        {
            // STORE :       e78210c4        str     r1, [r2, r4, asr #1]
            uint u = 0xe78210c4;
            int r = Convert.ToInt32(~u);
            r = ~r;

            Memory regs = new Memory(regsize), RAM = new Memory(12);
            regs.WriteWord(Reg.R2, 4);
            regs.WriteWord(Reg.R4, 8); //8
            regs.WriteWord(Reg.R1, 23);

            LoadStore inst = new LoadStore(r, ref regs, ref RAM);
            inst.Decode();
            inst.Execute();

            Assert.IsTrue(RAM.ReadWord(8) == 23);
            Assert.IsTrue(inst.ToString() == "str r1, [r2, r4, asr #1]");
        }

        [TestMethod]
        public void DecExecLS_RegImm_Store_Load_Success()
        {
            //Test 2 LOAD :         e79290c4        ldr     r9, [r2, r4, asr #1]
            uint u = 0xe79290c4;
            int r = Convert.ToInt32(~u);
            r = ~r;
            Memory regs = new Memory(regsize), RAM = new Memory(12);

            regs.WriteWord(Reg.R2, 4);
            regs.WriteWord(Reg.R4, 8); //8

            RAM.WriteWord(8, 23);

            LoadStore inst = new LoadStore(r, ref regs, ref RAM);
            inst.Decode();
            inst.Execute();

            Assert.IsTrue(regs.ReadWord(Reg.R9) == 23);
            Assert.IsTrue(inst.ToString() == "ldr r9, [r2, r4, asr #1]");
        }

        [TestMethod]
        public void Barrel_LSL()
        {
            Assert.IsTrue(BarrelShifter.ShiftByCode(2, 3, (char)0) == 2 << 3);
        }

        [TestMethod]
        public void Barrel_LSR()
        {
            Assert.IsTrue(BarrelShifter.ShiftByCode(2, 3, (char)2) == 2 >> 3);
        }

        [TestMethod]
        public void Barrel_ASR()
        {
            uint x = 0xe0000000;
            int y = (int)~x;
            y = ~y;
            Assert.IsTrue(BarrelShifter.ShiftByCode(y, 3, (char)1) == x >> 3);
        }

        [TestMethod]
        public void Barrel_ROR()
        {
            int x = 0x0e000010;
            Assert.IsTrue(BarrelShifter.ShiftByCode(x, 12, (char)3) == 0x0100e000);
        }

        [TestMethod]
        public void LSMultiple_Push()
        {
            //  e92d000a        push {r1, r3} = strdb r13, {r1, r3}
            uint u = 0xe92d000a;
            int r = Convert.ToInt32(~u);
            r = ~r;

            Memory regs = new Memory(regsize), RAM = new Memory(16);
            regs.WriteWord(Reg.R1, 34);
            regs.WriteWord(Reg.R3, -23);
            regs.WriteWord(Reg.R13, 12);

            LSMultiple inst = new LSMultiple(r, ref regs, ref RAM, 12);
            inst.Decode();
            inst.Execute();

            Assert.IsTrue(RAM.ReadWord(4) ==   34);
            Assert.IsTrue(RAM.ReadWord(8) == -23);
            Assert.IsTrue(regs.ReadWord(Reg.R13) == 4);

            Assert.IsTrue(inst.ToString() == "push {r1, r3}");
        }

        [TestMethod]
        public void LSMultiple_Pop_EdgeCases()
        {
            //    e8bd000a        pop     {r1, r3}
            uint u = 0xe8bd000a;
            int r = Convert.ToInt32(~u);
            r = ~r;

            Memory regs = new Memory(regsize), RAM = new Memory(12);
            RAM.WriteWord(4, 34);
            RAM.WriteWord(0, -23);
            regs.WriteWord(Reg.R13, 0);

            LSMultiple inst = new LSMultiple(r, ref regs, ref RAM, 12);
            inst.Decode();
            inst.Execute();

            Assert.IsTrue(regs.ReadWord(Reg.R1) == -23);
            Assert.IsTrue(regs.ReadWord(Reg.R3) == 34);
            Assert.IsTrue(regs.ReadWord(Reg.R13) == 8);
            Assert.IsTrue(inst.ToString() == "pop {r1, r3}");
        }
    }
}
