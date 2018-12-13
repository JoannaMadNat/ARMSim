/// <summary>
/// File: BarrelShifter.cs
/// This class represents the barrel shifter and performs
/// lsl, lsr, asr, and ror based on a given code
/// </summary>
namespace armsim.Model.Decoders
{
    //This class takes an operand and performs a shift/rotate operation by a given number of bits based on a code
    class BarrelShifter
    {
        //performs logical left shift on op1 by 'shift' bits
        static int LSL(int op1, int shift)
        {
            return op1 << shift;
        }

        //performs logical right shift on op1 by 'shift' bits
        static int LSR(int op1, int shift)
        {
            return (int)((uint)op1 >> shift);
        }

        //performs arithmetic right shift on op1 by 'shift' bits
        static int ASR(int op1, int shift)
        {
            return op1 >> shift;
        }

        //performs right totation on op1 by 'shift' bits
        static int ROR(int op1, int shift)
        {
            return (op1 >> shift) | (op1 << (32 - shift));
        }

        //takes op1 as the number to perform 'shift' bit shift/rotation on,
        //determines operation to perform based on 'code' where code is:
        //0b00 lsl, 0b10 asr, 0b01 lsr, 0b11 ror
        public static int ShiftByCode(int op, int shift, char code)
        {
            int res = 0;
            switch (code)
            {
                case (char)0:
                    res = LSL(op, shift);
                    break;
                case (char)2:
                    res = ASR(op, shift);
                    break;
                case (char)1:
                    res = LSR(op, shift);
                    break;
                case (char)3:
                    res = ROR(op, shift);
                    break;
            }
            return res;
        }

        //Returns string representation of operation based on 'code'
        //0b00 lsl, 0b10 asr, 0b01 lsr, 0b11 ror
        public static string CodeToString(char code)
        {
            switch (code)
            {
                case (char)0:
                    return "lsl";
                case (char)2:
                    return "asr";
                case (char)1:
                    return "lsr";
                case (char)3:
                    return "ror";
            }
            return "";
        }
    }
}
