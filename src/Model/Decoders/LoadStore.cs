using System.Collections.Generic;
/// <summary>
/// File: LoadStore.cs
/// The class responsible for decoding load/store instructions
/// </summary>
namespace armsim.Model.Decoders
{
    //The subclass of Instruction that deals with Load and Store instructions
    class LoadStore : Instruction
    {
        Memory RAM; //Load and Store instructions have access to RAM
        int address; //The address to read from or write to
        char Rd, Rn; //Rd: destination register, Rn: the register that hold the adress (if Storing, vice versa)
        bool P, U, B, W, L, IBit; //P: Pre(True) vs Post(false) indexing, U: Unsigned vs Signed, B: Byte vs Word, W: Writeback or not, L: Load vs Store
        string stringValue = ""; //second half of ldr/str command
        int offset; //Value of offset from initial address to calculate effective address

        //Indepentent constructor that initializes all its components itself
        public LoadStore(int code, ref Memory Regs, ref Memory RAM)
        {
            this.code = code;
            this.Regs = Regs;
            this.RAM = RAM;
            type = (char)ExtractBits(code, 25, 27);
            cond = (char)ExtractBits(code, 28, 31);
        }
        //Dependent constructor that relies on outside sources 
        //to fill in the basic components
        public LoadStore(ref Memory RAM)
        {
            this.RAM = RAM;
        }

        //Return string representation of the instruction
        public override string ToString()
        {
            string wor = suffix;
            if (L)
                wor += "ldr";
            else wor += "str";

            if (B) wor += "b";

            wor += " r" + (int)Rd + ", [r" + (int)Rn + (P ? "" : "]") + stringValue + (P ? "]" : "") + (W ? "!" : "");
            return AliasInstructions(wor);
        }

        //Decodes loadstore instruction for 'code' parent variable and returns
        //itself as a fully populated instruction ready for execution
        public override Instruction Decode()
        {
            IBit = TestFlag(code, 25);
            P = TestFlag(code, 24);
            U = TestFlag(code, 23);
            B = TestFlag(code, 22);
            W = TestFlag(code, 21);
            L = TestFlag(code, 20);
            Rn = (char)ExtractBits(code, 16, 19);
            Rd = (char)ExtractBits(code, 12, 15);

            int regContent = Regs.ReadWord(GetReg(Rn));
            if (Rn == 15)
                regContent += 8;

            offset = !IBit ? DecodeImmediateOffset() : DecodeRegisterOffset();
            address = P ? regContent + offset : regContent;

            return this;
        }

        //Extract adress according to immediate offset specification
        int DecodeImmediateOffset()
        {
            int shiftval = (ExtractBits(code, 0, 11) * (U ? 1 : -1));
            stringValue = ", #" + shiftval;

            return shiftval;
        }

        //Extract adress according to register offset specification
        int DecodeRegisterOffset()
        {
            char Rm = (char)ExtractBits(code, 0, 3);
            char Sh = (char)ExtractBits(code, 5, 6);
            int shiftVal = ExtractBits(code, 7, 11);
            shiftVal *= U ? 1 : -1;

            stringValue = ", " + (shiftVal == 0 ? (U ? "" : "-") : "") + "r" + (int)Rm;

            int regval = Regs.ReadWord(GetReg(Rm));
            if (Rm == 15)
                regval += 8;

            if (shiftVal != 0)
                stringValue += ", " + BarrelShifter.CodeToString(Sh) + " #" + shiftVal;
            else if (!U) regval *= -1;

            return BarrelShifter.ShiftByCode(regval, shiftVal, Sh);
        }

        //If L is true, it performs Load
        //if L is false, it performs Store
        public override void Execute()
        {
            if (L)
                Load();
            else
                Store();

            if (W) Regs.WriteWord(GetReg(Rn), address); 
            else if (!P) Regs.WriteWord(GetReg(Rn), address + offset);
        }

        //Loads value pointed to by 'address' into register Rd
        void Load()
        {
            if (B)
                Regs.WriteWord(GetReg(Rd), RAM.ReadByte(address));
            else
                Regs.WriteWord(GetReg(Rd), RAM.ReadWord(address));
        }

        //Stores value from register Rd in 'address'
        void Store()
        {
            if (B)
            {
                int storeVal = Regs.ReadWord(GetReg(Rd));
                if (Rd == 15)
                    storeVal += 8;
                RAM.WriteByte(address, (byte)storeVal);
            }
            else
            {
                int storeVal = Regs.ReadWord(GetReg(Rd));
                if (Rd == 15)
                    storeVal += 8;

                RAM.WriteWord(address, storeVal);
            }
        }
    }
}
