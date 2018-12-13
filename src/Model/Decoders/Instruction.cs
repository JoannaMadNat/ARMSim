using System;
using System.Collections.Generic;

/// <summary>
/// File: Instruction.cs
/// The parent class of all instruction classes
/// </summary>

namespace armsim.Model.Decoders
{
    //Main parent class of all decodable and executable instructions
    abstract class Instruction
    {
        protected string suffix = "";
        public bool toExecute = true;
        protected Memory Regs; //Reference to the registers
        protected int code; //The instruction code
        protected char cond, type; //cond: condition code, type: type of instruction (not including special cases)
        public abstract void Execute(); //Execute instruction
        public abstract Instruction Decode(); //Decode instruction and return fully populated instance
        public abstract override string ToString(); // convert instruction to string representation
        //Returns fully populated instructions of type DataProcessing, LoadStore, or Branch
        //takes in 'code' as the instruction code to decode, 'Regs' as refferences to the registers
        //'RAM' as refference to RAM
        public static Instruction Make(int code, ref Memory Regs, ref Memory RAM, ref bool[] SystemFlags, ref Queue<char> KQueue)
        {
            Instruction inst;
            char type = (char)ExtractBits(code, 25, 27);
            inst = ChooseType(code, type, ref RAM, ref SystemFlags);
            inst.code = code;
            inst.Regs = Regs;
            inst.type = Convert.ToChar(type);

            inst.cond = (char)ExtractBits(code, 28, 31);
            if (!inst.TestConditionFlags(inst.cond))
                inst.toExecute = false;

            inst.Decode();
            return inst;
        }

        //Makke() with no IO for testing
        public static Instruction Make(int code, ref Memory Regs, ref Memory RAM, ref bool[] SystemFlags)
        {
            Instruction inst;
            Queue<char> KQueue = new Queue<char>();

            char type = (char)ExtractBits(code, 25, 27);
            inst = ChooseType(code, type, ref RAM, ref SystemFlags);

            inst.code = code;
            inst.Regs = Regs;
            inst.type = Convert.ToChar(type);

            inst.cond = (char)ExtractBits(code, 28, 31);
            if (!inst.TestConditionFlags(inst.cond))
                inst.toExecute = false;

            inst.Decode();
            return inst;
        }

        //Takes in Necessary components for all instruction type constructors and returns an initial instruction of that type
        static Instruction ChooseType(int code, char type, ref Memory RAM, ref bool[] flags)
        {
            if (ExtractBits(code, 23, 27) == 2 && ExtractBits(code, 20, 21) == 0)
                return new MRS();

            else if (((code != 0 && type == 0) || type == 1) && ExtractBits(code, 23, 24) == 2 && ExtractBits(code, 20, 21) == 2 && ExtractBits(code, 4, 7) == 0)
                return new MSR();

            else if (type == 7 && TestFlag(code, 24))
                return new Swi(ref flags);

            else if ((code != 0 && type == 0) && ExtractBits(code, 20, 24) == 18 && ExtractBits(code, 4, 7) == 1)
                return new Branch(true);

            else if ((code != 0 && type == 0) || type == 1)
                return new DataProcessing();

            else if (type == 2 || type == 3)
                return new LoadStore(ref RAM); //only loadStore can access memory

            else if (type == 4)
                return new LSMultiple(ref RAM);

            else if (type == 5)
                return new Branch(false);

            else return new NoOp();
        }


        //Takes in address as 'addr' and a 16 bit offset as 'bit', tests the bit located at that offset, 
        //returns true if that bit is 1, and false if bit is 0
        public static bool TestFlag(int code, int bit)
        {
            if (bit > 31 || bit < 0)
                throw new ArgumentOutOfRangeException("RAM.TestFlag: bit out of range of word size");

            int word = code;
            int mask = 0b1 << bit;

            return (word & mask) != 0;
        }

        //Takes in a 32 but number as 'word', a starting offset as 'startBit' and ending offset as 'endBit'
        //Extracts the bits from the word that are located between the offsets (inclusive)
        protected static int ExtractBits(int code, short startBit, short endBit)
        {
            uint word;
            if (code < 0)
            {
                word = Convert.ToUInt32(~code);
                word = ~word; //Getting past the int/uint problems...
            }
            else word = Convert.ToUInt32(code);

            if (startBit < 0 || startBit > 31 || endBit < 0 || endBit > 31 || startBit > endBit)
                throw new ArgumentException("RAM.ExtractBits: Bit size invalid");

            uint mask = 0x0;
            for (int i = startBit; i <= endBit; ++i)
            {
                mask += 0b1;
                if (i != endBit)
                    mask <<= 1;
            }
            mask <<= startBit;

            word &= mask;
            word >>= startBit;

            return (int)word;
        }

        //Takes in a 16 bit offset as 'bit', sets the bit located at that offset in CPSR to 1 if flag if true, 
        //and 0 if flag is false
        public void SetCPSRFlag(short bit, bool flag)
        {
            if (bit > 31 || bit < 0)
                throw new ArgumentOutOfRangeException("RAM.SetFlag: bit out of range of word size");

            int cpsr = Regs.ReadWord(Reg.CPSR);

            bool isOne = TestFlag(cpsr, bit);
            int word = cpsr;
            int mask = 0b1 << bit;

            if (flag && !isOne)
                word |= mask;
            else if (!flag && isOne)
                word &= ~mask;

            Regs.WriteWord(Reg.CPSR, word);
        }

        //Tests condition flag at the end of instruction,
        //determines the suffix to place in front or behind the instruction name
        // and returns whether the instruction should execute of not
        bool TestConditionFlags(char cond)
        {
            switch (cond)
            {
                case (char)0:
                    suffix = "eq";
                    if (TestFlag(Regs.ReadWord(Reg.CPSR), 30))
                        return true;
                    break;
                case (char)1:
                    suffix = "ne";
                    if (!TestFlag(Regs.ReadWord(Reg.CPSR), 30))
                        return true;
                    break;
                case (char)2:
                    suffix = "cshs";
                    if (TestFlag(Regs.ReadWord(Reg.CPSR), 29))
                        return true;
                    break;
                case (char)3:
                    suffix = "cclo";
                    if (!TestFlag(Regs.ReadWord(Reg.CPSR), 29))
                        return true;
                    break;
                case (char)4:
                    suffix = "mi";
                    if (TestFlag(Regs.ReadWord(Reg.CPSR), 31))
                        return true;
                    break;
                case (char)5:
                    suffix = "pl";
                    if (!TestFlag(Regs.ReadWord(Reg.CPSR), 31))
                        return true;
                    break;
                case (char)6:
                    suffix = "vs";
                    if (TestFlag(Regs.ReadWord(Reg.CPSR), 28))
                        return true;
                    break;
                case (char)7:
                    suffix = "vc";
                    if (!TestFlag(Regs.ReadWord(Reg.CPSR), 28))
                        return true;
                    break;
                case (char)8:
                    suffix = "hi";
                    if (!TestFlag(Regs.ReadWord(Reg.CPSR), 30) && TestFlag(Regs.ReadWord(Reg.CPSR), 29))
                        return true;
                    break;
                case (char)9:
                    suffix = "ls";
                    if (!TestFlag(Regs.ReadWord(Reg.CPSR), 29) || TestFlag(Regs.ReadWord(Reg.CPSR), 30))
                        return true;
                    break;
                case (char)10:
                    suffix = "ge";
                    if (TestFlag(Regs.ReadWord(Reg.CPSR), 31) == TestFlag(Regs.ReadWord(Reg.CPSR), 28))
                        return true;
                    break;
                case (char)11:
                    suffix = "lt";
                    if (TestFlag(Regs.ReadWord(Reg.CPSR), 31) != TestFlag(Regs.ReadWord(Reg.CPSR), 28))
                        return true;
                    break;
                case (char)12:
                    suffix = "gt";
                    if (!TestFlag(Regs.ReadWord(Reg.CPSR), 30) && (TestFlag(Regs.ReadWord(Reg.CPSR), 31) == TestFlag(Regs.ReadWord(Reg.CPSR), 28)))
                        return true;
                    break;
                case (char)13:
                    suffix = "le";
                    if (TestFlag(Regs.ReadWord(Reg.CPSR), 30) || (TestFlag(Regs.ReadWord(Reg.CPSR), 31) != TestFlag(Regs.ReadWord(Reg.CPSR), 28)))
                        return true;
                    break;
                case (char)14:
                    suffix = ""; //no need to mention the always execution
                    return true;
            }
            return false;
        }

        //Replaces instruction names with their given aliases
        protected string AliasInstructions(string opString)
        {
            if (opString != "")
            {
                opString = opString.Replace("r11", "fp");
                opString = opString.Replace("r13", "sp");
                opString = opString.Replace("r14", "lr");
                opString = opString.Replace("r15", "pc");
            }
            return opString;
        }

        //Checks and returns character representation of current execution mode
        //Y = System
        //V = Supervisor
        //R = IRQ
        protected char CheckMode()
        {
            int mode = ExtractBits(Regs.ReadWord(Reg.CPSR), 0, 4);
            if (mode == Mode.Supervisor) //svc
                return 'V';
            if (mode == Mode.IRQ) //irq
                return 'R';
            else //sys
                return 'Y';
        }

        //Writes to CPSR what is stored in SPSR of the mode execution is returning from
        protected void SwitchBackMode()
        {
            char mode = CheckMode();
            if (mode == 'V')
                Regs.WriteWord(Reg.CPSR, Regs.ReadWord(Reg.SPSR_SVC)); //switch to system
            else if (mode == 'R')
            {
                Regs.WriteWord(Reg.CPSR, Regs.ReadWord(Reg.SPSR_IRQ)); //switch to system

            }

            SetCPSRFlag(7, false);
        }

        //Determines which of the banked registers shuld be used at a given mode in the program.
        protected int GetReg(int regNum)
        {
            if (regNum == 14 || regNum == 13) //sp 
            {
                char mode = CheckMode();
                if (mode == 'V')
                    return regNum * 4 + 16;
                if (mode == 'R')
                    return regNum * 4 + 24;
            }

            return regNum * 4; //sys
        }
    }
}
