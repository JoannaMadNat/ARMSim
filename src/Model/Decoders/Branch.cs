/// <summary>
/// File: Branch.cs
/// Class hat deals with branching instructions
/// </summary>
namespace armsim.Model.Decoders
{
    //subclass of Instruction that deals with jump instructions
    class Branch : Instruction
    {
        int offset; //The offset from the current program counter location
        int address;
        bool link; //whether to store the previous pc value or not
        bool isRet = false, //Is this BX
            sign = false; //Jump foreward(false) or backwards(true)
        int Rm; //Register holding link address

        //Constructor
        public Branch(bool isRet)
        {
            this.isRet = isRet;
        }

        //Decode the 'code' variable by extracting the relative address to jump to.
        public override Instruction Decode()
        {
            if (isRet)
                ProcessReturnAddress();
            else ProcessBranchAddress();

            return this;
        }

        //Get the register the register that hold the link address
        void ProcessReturnAddress()
        {
            Rm = ExtractBits(code, 0, 3);
        }

        //Calculate the address to jump to relative to PC
        void ProcessBranchAddress()
        {
            int pc = Regs.ReadWord(Reg.R15);
            link = TestFlag(code, 24);
            sign = TestFlag(code, 23);

            offset = ExtractBits(code, 0, 22);

            if (sign)
            {
                uint mask = 0xFFF00000;
                offset |= (int)mask; //mask to convert to signed FF000000
            }
            offset <<= 2;
            address = pc + 8 + offset;
        }

        //Return string representation of the instruction
        public override string ToString()
        {
            string opstr = "";
            string sub = (sign) ? " <sub>" : "";
            if (!isRet)
            {
                if (link) opstr += "BL" + suffix + " 0x" + address.ToString("X8") + sub;
                else opstr += "B" + suffix + " 0x" + address.ToString("X8") + sub;
            }
            else
                opstr += AliasInstructions("BX" + suffix + " r" + Rm) + sub;

            return opstr;
        }

        //Return string interpretation of decoded branch instruction with the calculated address to jump to according to PC
        public string ToString(int pc)
        {
            string opstr = "";
            string sub = (sign) ? " <sub>" : "";
            int addr = pc + 8 + offset;
            if (!isRet)
            {
                if (link) opstr += "BL" + suffix + " 0x" + addr.ToString("X8") + sub;
                else opstr += "B" + suffix + " 0x" + addr.ToString("X8") + sub;
            }
            else
                opstr += AliasInstructions("BX" + suffix + " r" + Rm) + sub;

            return opstr;
        }

        //Change the value of the program counter,
        //if L is true, store previous PC value in R14
        public override void Execute()
        {
            if (isRet)
            {
                ExecuteReturn();
                return;
            }

            if (link)
                Regs.WriteWord(GetReg(14), Regs.ReadWord(Reg.R15));
            Regs.WriteWord(Reg.R15, address);
        }

        //Execute BX: jump to address stored in link register
        void ExecuteReturn()
        {
            Regs.WriteWord(Reg.R15, Regs.ReadWord(GetReg(Rm)));
        }

    }
}
