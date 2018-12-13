/// <summary>
/// File: LSMultiple.cs
/// this class represents the load/store multiple variant of instuction
/// </summary>
namespace armsim.Model.Decoders
{
    //represents the load/store multiple (FD variant) instructions and can word as push and pop for the stack
    class LSMultiple : Instruction
    {
        Memory RAM; //Load and Store instructions have access to RAM
        int address; //The address to read from or write to
        char Rn; //Rn: Base register used in the addressing mode
        bool P, U, S, W, L, IBit; //P: Pre(True) vs Post(false) indexing, U: Unsigned vs Signed, B: Byte vs Word, W: Writeback or not, L: Load vs Store
        bool[] regList; //list of booleans whose locations represent which registers are the ones to use
        string regstr = ""; //used for the second half of string representation of instruction
        public int topofStack; //top of stack, used for bounds checking (if any)

        //Indepentent constructor that initializes all its components itself
        public LSMultiple(int code, ref Memory Regs, ref Memory RAM, int topofStack)
        {
            this.code = code;
            this.Regs = Regs;
            this.RAM = RAM;
            type = (char)ExtractBits(code, 25, 27);
            cond = (char)ExtractBits(code, 28, 31);
            regList = new bool[16];
            this.topofStack = topofStack;
        }
        //Dependent constructor that relies on outside sources 
        //to fill in the basic components
        public LSMultiple(ref Memory RAM)
        {
            this.RAM = RAM;
            regList = new bool[16];
            topofStack = Reg.Shelf; //get the pun?
        }

        //decode instruction based on 'code' parent variable and returns populated version of itself
        public override Instruction Decode()
        {
            IBit = TestFlag(code, 25);
            P = TestFlag(code, 24);
            U = TestFlag(code, 23);
            S = TestFlag(code, 22);
            W = TestFlag(code, 21);
            L = TestFlag(code, 20);
            Rn = (char)ExtractBits(code, 16, 19);
            address = Regs.ReadWord(GetReg(Rn));

            regstr = "";
            bool first = true;
            for (int i = 0; i < regList.Length; i++)
            {
                regList[i] = TestFlag(code, i);
                if (regList[i])
                {
                    if (!first) regstr += ", ";
                    regstr += "r" + i;
                    first = false;
                }
            }
            return this;
        }

        //Executes instruction by either store or load
        public override void Execute()
        {
            if (L)
                Load();
            else Store();

            if (W) Regs.WriteWord(GetReg(Rn), address);
        }

        //loads the values in the address pointed to by Rn into the active registers
        //in reglist[] while incrementing the address in Rn after loading
        void Load() //pop
        {
            for (int i = 0; i < regList.Length; i++)
                if (regList[i])
                {
                    Regs.WriteWord(GetReg(i), RAM.ReadWord(address));
                    address += 4;
                }
        }

        //stores the values in the active registers in reglist[] into the address pinted to by Rn
        //while decrementing the value Rn before the store process
        void Store() //push
        {
            for (int i = regList.Length - 1; i >= 0; i--)
                if (regList[i])
                {
                    address -= 4;
                    RAM.WriteWord(address, Regs.ReadWord(GetReg(i)));
                }
        }

        //returns string representation of the instruction while also using aliases for push and pop
        //in their special cases.
        public override string ToString()
        {
            string wor = suffix + (L ? "ldmfd" : "stmfd") + " r" + (int)Rn + (W ? "!" : "") + ",";

            //aliases for push and pop
            if (wor.Equals("ldmfd r13!,"))
                wor = "pop";
            else if (wor.Equals("stmfd r13!,"))
                wor = "push";

            wor += " {" + regstr + "}";
            return AliasInstructions(wor);
        }
    }
}
