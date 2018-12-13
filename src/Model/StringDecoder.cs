using armsim.Model.Decoders;

/// <summary>
/// File: StringDecoder.cs
/// This class is responsible for returning only string representations of programs.
/// </summary>
namespace armsim.Model
{
    //This class is responsible for returning only string representations of programs.
    //It will not alter memory, or any of the registers. It only reads and decodes instructions.
    class StringDecoder
    {
        //decodes instruction and returns string representation of it
        public static string Decode(int code, Memory Regs, Memory RAM, bool[] flags, int pc)
        {
            if (code == 0 || pc < 0)
                return "";

            //int originalpc = Regs.ReadByte(Reg.R15);
            string str = "";
            Instruction inst = Instruction.Make(code, ref Regs, ref RAM, ref flags);

            if (inst is Branch)
                str = (inst as Branch).ToString(pc);
            else str = inst.ToString();

            return str;
        }
    }
}
