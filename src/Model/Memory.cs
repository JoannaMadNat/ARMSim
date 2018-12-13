using System;

/// <summary>
/// File: Memory.cs
/// A class that acts as a simulated RAM.
/// </summary>
/// 
namespace armsim.Model
{
    //This class simulates memory with an array of bytes organized in Little Endian.
    public class Memory
    {
        byte[] memory_array; //The simulated RAM space
        public char characterWrite = '\0'; //stores value of character to write to console
        public char characterRead = '\0'; //stores last character read from keyboard
        public bool pendingChar = false; //There is a character pending to be written

        //Constructor
        public Memory(int size)
        {
            if (size % 4 != 0)
                throw new ArgumentException("RAM: Invalid size.");

            memory_array = new byte[size];
        }

        //Gets memory Array for read-only purposes
        public byte[] MemoryArray
        {
            get { return memory_array; }
        }

        //Takes in address as addr and reads a 32 bits from RAM at that address in little endian
        //returns the value converted to big endian
        public int ReadWord(int addr)
        {
            if (addr + 4 > memory_array.Length)
                throw new ArgumentOutOfRangeException("RAM.ReadWord: Address " + addr + " out of range.");
            if (addr % 4 != 0)
                throw new ArgumentException("RAM.ReadWord: Invalid memory location.");

            int data = 0, most, second, third, least;

            most = memory_array[addr + 3];
            second = memory_array[addr + 2];
            third = memory_array[addr + 1];
            least = memory_array[addr];

            data += least;
            data += third << 8;
            data += second << 16;
            data += most << 24;
            return data;
        }

        //Takes in address as addr and reads a 16 bits from RAM at that address in little endian
        //returns the value converted to big endian        
        public short ReadHalfWord(int addr)
        {
            if (addr + 2 > memory_array.Length)
                throw new ArgumentOutOfRangeException("RAM.ReadHalWord: Address " + addr + " out of range.");
            if (addr % 2 != 0)
                throw new ArgumentException("RAM.ReadHalfWord: Invalid memory location.");

            short data = 0;
            data += memory_array[addr];
            data += (short)(memory_array[addr + 1] << 8);

            return data;
        }

        //Takes in address as addr and reads a 8 bits from RAM at that address in little endian
        //returns the value converted to big endian
        public byte ReadByte(int addr)
        {
            if (addr == 0x100001)
            {
                return (byte)characterRead;
            }

            if (addr >= memory_array.Length)
                throw new ArgumentOutOfRangeException("RAM.ReadByte: Address " + addr + " out of range.");

            return memory_array[addr];
        }

        //Takes in address as 'addr' and a 32 bit number value as 'word' writes it to RAM at that address in little endian
        public void WriteWord(int addr, int word)
        {
            if (addr + 4 > memory_array.Length)
                throw new ArgumentOutOfRangeException("RAM.WriteWord: Address " + addr + " out of range.");
            if (addr % 4 != 0)
                throw new ArgumentException("RAM.WriteWord: Invalid memory location.");

            byte most, second, third, least;
            most = (byte)((word & 0xff000000) >> 24);
            second = (byte)((word & 0x00ff0000) >> 16);
            third = (byte)((word & 0x0000ff00) >> 8);
            least = (byte)(word & 0x000000ff);

            memory_array[addr] = least;
            memory_array[addr + 1] = third;
            memory_array[addr + 2] = second;
            memory_array[addr + 3] = most;
        }

        //Takes in address as 'addr' and a 16 bit number value as 'halfword' writes it to RAM at that address in little endian
        public void WriteHalfWord(int addr, short halfWord)
        {
            if (addr + 2 > memory_array.Length)
                throw new ArgumentOutOfRangeException("RAM.WriteHalfWord: Address " + addr + " out of range.");
            if (addr % 2 != 0)
                throw new ArgumentException("RAM.WriteHalfWord: Invalid memory location.");
            byte most, least;
            most = (byte)((halfWord & 0xff00) >> 8);
            least = (byte)(halfWord & 0x00ff);

            memory_array[addr] = least;
            memory_array[addr + 1] = most;
        }

        //Takes in address as 'addr' and a 8 bit number value as 'my_byte' writes it to RAM at that address in little endian
        public void WriteByte(int addr, byte my_byte)
        {
            if (addr == 0x100000)
            {
                characterWrite = (char)my_byte;
                pendingChar = true;
                return;
            }

            if (addr >= memory_array.Length)
                throw new ArgumentOutOfRangeException("RAM.WriteByte: Address " + addr + " out of range.");
            memory_array[addr] = my_byte;

        }

        //Takes in address as 'addr' and a 16 bit offset as 'bit', tests the bit located at that offset, 
        //returns true if that bit is 1, and false if bit is 0
        public bool TestFlag(int addr, short bit)
        {
            if (addr >= memory_array.Length)
                throw new ArgumentOutOfRangeException("RAM.TestFlag: Address " + addr + " out of range.");
            if (bit > 31 || bit < 0)
                throw new ArgumentOutOfRangeException("RAM.TestFlag: bit out of range of word size");

            int word = ReadWord(addr);
            int mask = 0b1 << bit;

            return (word & mask) != 0;
        }

        //Takes in address as 'addr' and a 16 bit offset as 'bit', sets the bit located at that offset to 1 if flag if true, 
        //and 0 if flag is false
        public void SetFlag(int addr, short bit, bool flag)
        {
            if (addr >= memory_array.Length)
                throw new ArgumentOutOfRangeException("RAM.SetFlag: Address " + addr + " out of range.");
            if (bit > 31 || bit < 0)
                throw new ArgumentOutOfRangeException("RAM.SetFlag: bit out of range of word size");

            bool isOne = TestFlag(addr, bit);
            int word = ReadWord(addr);
            int mask = 0b1 << bit;

            if (flag && !isOne)
                word += mask;
            else if (!flag && isOne)
                word &= ~mask;

            WriteWord(addr, word);
        }

        //Takes in a 32 but number as 'word', a starting offset as 'startBit' and ending offset as 'endBit'
        //Extracts the bits from the word that are located between the offsets (inclusive)
        public static int ExtractBits(int word, short startBit, short endBit)
        {
            if (startBit < 0 || startBit > 31 || endBit < 0 || endBit > 31 || startBit > endBit)
                throw new ArgumentException("RAM.ExtractBits: Bit size invalid");

            int mask = 0x0;
            for (int i = startBit; i <= endBit; ++i)
            {
                mask += 0b1;
                if (i != endBit)
                    mask <<= 1;
            }
            mask <<= startBit;

            word &= mask;
            return word;
        }

        //Runs checksum algorithm on all values in RAM,
        //returns the computed value
        public int CalculateChecksum()
        {
            int cksum = 0;

            for (int address = 0; address < memory_array.Length; ++address)
                cksum += memory_array[address] ^ address;

            return cksum;
        }

        //takes in an array of byts 'newRAM' and a length,
        //sets the local memory array to the values in the parameter array
        //Used for testing.
        public void SetRAM(byte[] newRAM, int length)
        {
            int dist = length > memory_array.Length ? memory_array.Length : length;
            for (int i = 0; i < dist; ++i)
                memory_array[i] = newRAM[i];
        }
    }
}
