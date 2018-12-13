using System;
/// <summary>
/// File: MRS.cs
/// Sub class of instruction, does MRS instruction.
/// </summary>

namespace armsim.Model.Decoders
{
    //Instruction that transfers value from CPSR or SPSR of the current mode into a general purpose register
    class MRS : Instruction
    {
        char Rd; //destination register to write to
        bool RBit; //reading from either CPSR(false) or SPSR(true)

        //Returns fully populated class with the necessary elements to execute MRS instruction
        public override Instruction Decode()
        {
            RBit = TestFlag(code, 22);
            Rd = (char)ExtractBits(code, 12, 15);

            return this;
        }

        //Transfers value from CPSR or SPSR of the current mode into a general purpose register
        public override void Execute()
        {
            int value = Regs.ReadWord(Reg.CPSR);
            if (RBit)
            {
                char mode = CheckMode();
                if (mode == 'V') //supervisor
                    value = Regs.ReadWord(Reg.LR_SVC);
                else if (mode == 'R') //irq
                    value = Regs.ReadWord(Reg.LR_IRQ);
            }

            Regs.WriteWord(GetReg(Rd), value);
        }

        //Return string interpretation of decoded MRS instruction
        public override string ToString()
        {
            string str = suffix;
            if (RBit)
            {
                char mode = CheckMode();
                if (mode == 'V') //supervisor
                    str += "mrs r" + (int)Rd + ", SPSR_SVC";
                else if (mode == 'R') //irq
                    str += "mrs r" + (int)Rd + ", SPSR_IRQ";
            }
            else
                str += "mrs r" + (int)Rd + ", CPSR";

            return str;
        }
    }
}
