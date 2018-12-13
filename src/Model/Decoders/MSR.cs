using System;
/// <summary>
/// File: MSR.cs
/// Sub class of instruction, does MSR instruction.
/// /// </summary>

namespace armsim.Model.Decoders
{
    //MSR is an instruction that moves either a valuefrom register 
    //or immediate into CPSR or SPSR of the current mode.
    class MSR : Instruction
    {
        bool IBit; //Is operand register oor immediate
        bool RBit; //Transfer to CPSR(false) or SPSR(true)
        string strVal; //string representation of operand
        int value; //value to transfer to CPSR or SPSR

        //Decode MSR to find value to place in either CPSR or SPSR
        public override Instruction Decode()
        {
            IBit = TestFlag(code, 25);
            RBit = TestFlag(code, 22);

            value = IBit ? CalulateImmediateValue() : CalulateRegValue();
            return this;
        }

        //Calcultes and returns the immediate shift operand
        int CalulateImmediateValue()
        {
            short shift = Convert.ToInt16(ExtractBits(code, 8, 11));
            shift *= 2;
            int immediate = ExtractBits(code, 0, 7);
            immediate = BarrelShifter.ShiftByCode(immediate, shift, (char)3);

            strVal = "#" + immediate;

            return immediate;
        }

        //caculated and returns the reister operand
        int CalulateRegValue()
        {
            char Rm = (char)ExtractBits(code, 0, 3);

            int val = Regs.ReadWord(GetReg(Rm));
            if (Rm == 15) val += 8;

            strVal = "r" + (int)Rm;
            return val;
        }

        //transfers the value of a general-purpose register or an 
        //immediate constant to the CPSR or the SPSR of the current mode. 
        public override void Execute()
        {
            if (RBit)
            {
                char mode = CheckMode();
                if (mode == 'V')
                { //supervisor 
                    Regs.WriteWord(Reg.SPSR_SVC, value);
                }
                else if (mode == 'R')
                { //irq
                    Regs.WriteWord(Reg.SPSR_IRQ, value);
                }
            }
            else
                Regs.WriteWord(Reg.CPSR, value);
        }

        //Return string interpretation of decoded MSR instruction
        public override string ToString()
        {
            string str = suffix;
            if (RBit)
            {
                char mode = CheckMode();
                if (mode == 'V') //supervisor
                    str += "msr SPSR_SVC, ";
                else if (mode == 'R') //irq
                    str += "msr SPSR_IRQ, ";
            }
            else
                str += "msr CPSR, ";

            return str + strVal;
        }

    }
}
