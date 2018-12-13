/// <summary>
/// File: Interrupt.cs
/// This class represent the interrupt type of instructions
/// </summary>

namespace armsim.Model.Decoders
{
    //Represents the instructions of type Interrupt 
    class Swi : Instruction
    {
        bool[] Flags; //Boolean representation of the cpsr flags
        int swiType; //the value of swi that determines what kind of swi command it is

        //Constructor
        public Swi(ref bool[] Flags)
        {
            this.Flags = Flags;
        }

        //decode interrupt base on 'code' parent variable
        public override Instruction Decode()
        {
            swiType = ExtractBits(code, 0, 23);
            return this;
        }

        //fire the interrupt
        //Interrupts are special. They get special access to the inner guts of the program...
        public override void Execute()
        {
            if (swiType == 0x11)
            {
                Flags[SysFlag.Running] = false;
                Flags[SysFlag.Unsteppable] = true;
                return;
            }

            Regs.WriteWord(Reg.LR_SVC, Regs.ReadWord(Reg.R15)); 
            Regs.WriteWord(Reg.SPSR_SVC, Regs.ReadWord(Reg.CPSR));

            int cpsr = Regs.ReadWord(Reg.CPSR) & 0b1111111111111111111111111100000;
            Regs.WriteWord(Reg.CPSR, cpsr | Mode.Supervisor); //switch to superviser

            if (swiType == 0x0)
                SetCPSRFlag(7, true);
            else SetCPSRFlag(7, false);

            Regs.WriteWord(Reg.R15, 0x8);
        }

        //Return string interpretation of decoded interrupt instruction
        public override string ToString()
        {
            return "swi #0x" + swiType.ToString("X");
        }
    }
}
