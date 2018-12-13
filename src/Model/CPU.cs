using armsim.Model.Decoders;
using System.Collections.Generic;

/// <summary>
/// File: CPU.cs
/// Class that simulates CPU.
/// </summary>

namespace armsim.Model
{
    //This class simulates a CPU. It fetches, decodes, and executes commmands from the RAM refference.
    class CPU
    {
        public Memory RAM; //simulated RAM
        public Memory Registers; //simulated Registers
        Queue<char> KQueue; //Queue that stores all the characters pending to be written to the console
        public Instruction CurrentInstruction; //Current instruction being executed by CPU
        public bool[] Flags; //Boolean representation of the cpsr flags
        bool[] SystemFlags; //flags that deal with the execution of the program


        //The CPU class has instance variables that hold references to the Registers and RAM objects.
        public CPU(ref Memory RAM, ref Memory Registers, ref bool[] Flags, ref bool[] SystemFlags, ref Queue<char> KQueue, bool OSInst)
        {
            this.RAM = RAM;
            this.Registers = Registers;
            this.Flags = Flags;
            this.SystemFlags = SystemFlags;
            this.KQueue = KQueue;
        }

        //Constructor for testing no IO
        public CPU(ref Memory RAM, ref Memory Registers, ref bool[] Flags, ref bool[] SystemFlags)
        {
            this.RAM = RAM;
            this.Registers = Registers;
            this.Flags = Flags;
            this.SystemFlags = SystemFlags;
            KQueue = new Queue<char>();
        }

        //Returns a word from the RAM address specified by the value of the simulated Program Counter register (R15). 
        public int Fetch()
        {
            return RAM.ReadWord(Registers.ReadWord(Reg.R15));
        }

        // Returns fully populated instruction decoded from 'instCode'
        public void Decode(int instCode)
        {
            CurrentInstruction = Instruction.Make(instCode, ref Registers, ref RAM, ref SystemFlags, ref KQueue);
        }

        //Execute the current instruction
        public void Execute()
        {
            if (CurrentInstruction != null && CurrentInstruction.toExecute)
                CurrentInstruction.Execute(); //All instruction sub-classes have an execute method
            UpdateFlags();
        }

        //Reset CPSR flags to all 0
        void ResetFLags()
        {
            int cpsr = Registers.ReadWord(Reg.CPSR);
            int mask = 0b00001111111111111111111111111111;

            cpsr &= mask;
            Registers.WriteWord(Reg.CPSR, cpsr);
        }

        //Update the boolean CPSR flags with their bit counterparts
        public void UpdateFlags()
        {
            Flags[Flag.NF] = Instruction.TestFlag(Registers.ReadWord(Reg.CPSR), 31);
            Flags[Flag.ZF] = Instruction.TestFlag(Registers.ReadWord(Reg.CPSR), 30);
            Flags[Flag.CF] = Instruction.TestFlag(Registers.ReadWord(Reg.CPSR), 29);
            Flags[Flag.FF] = Instruction.TestFlag(Registers.ReadWord(Reg.CPSR), 28);
        }
    }
}
