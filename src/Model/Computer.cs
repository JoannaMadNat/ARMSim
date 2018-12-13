using System;
using System.Collections.Generic;
using System.IO;
using armsim.Model.Decoders;

/// <summary>
/// File: Computer.cs
/// Class that simulates a computer which has registers, RAM, and CPU
/// </summary>

namespace armsim.Model
{
    //A class that simulates all the functions of a computer. Has CPU, Registers, and RAM
    class Computer
    {
        public CPU cpu; //simulated CPU
        public Memory RAM; //Simulated RAM
        public Memory Registers; //Simulated Registers
        public bool[] Flags; //Boolean representation of the cpsr flags
        public Queue<char> ConsoleKeyQueue; //Queue of all characters to be written to console
        public bool OSInstalled = false; //true if there is an Operating System installed
        public bool IRQPending = false; //true if there is a character read from an interrupt pending to be read by the program
        public bool traceAll = false; //Trace all instructions including those in IRQ and SVC mode

        //mode of execution determined by bits 0 to 4 of the CPSR
        public int ExecMode { get { return Memory.ExtractBits(Registers.ReadWord(Reg.CPSR), 0, 4); } }

        ELFLoader loader;   //loads ELF files to the program
        StreamWriter traceFile; //Writes the trace file for testing and tracing
        public int stepNum = 1; //Counts the steps the program has taken since the entry point
        public string traceFileName; //Name of file in which to dump traceData
        public List<int> breakPoints = new List<int>(); //List of breakpoints in whih to stop execution at
        bool isBreak = false; //Checks if the program's last stop was in a break;

        public bool trace = true; //Checks whether to enable tracing or not
        public string currentFilename; //Name of .exe file that is currently loaded
        string log = "Computer: "; //For easier tracing/printing
        public bool[] SystemFlags; //flags that deal with the execution of the program (running and stepping)

        //constructor
        public Computer(int memSize, string logPath, bool trace)
        {
            Tracer.Log(log + "Initating computer with memory size of " + memSize + "...");

            traceFileName = logPath;
            RAM = new Memory(memSize);
            Registers = new Memory(92);
            Flags = new bool[4];
            SystemFlags = new bool[2];
            ConsoleKeyQueue = new Queue<char>();

            cpu = new CPU(ref RAM, ref Registers, ref Flags, ref SystemFlags, ref ConsoleKeyQueue, OSInstalled);
            this.trace = trace;
            if (trace)
                OpenTraceFile();
            Tracer.Log(log + "Done.");
        }

        //Property to access Program Counter
        public int PC
        {
            get { return Registers.ReadWord(Reg.R15); }
            set { Registers.WriteWord(Reg.R15, value); }
        }

        //Resets RAM and Registers to 0
        //Takes in fileName as a parameter, and loads that file's contents to RAM
        //Sets program counter to initial value (entry point)
        public void LoadFileToRAM(string fileName)
        {
            Tracer.Log(log + "Loading to RAM: " + fileName);
            currentFilename = fileName;
            ResetRAMandRegs();
            loader.ReadELF(ref RAM, fileName);
            ResetPCSPandCPSR();
            ResetSystemFlags();
            cpu.UpdateFlags();
        }

        //Sets RAM and Registers to zeros
        public void ResetRAMandRegs()
        {
            Tracer.Log(log + "Reseting RAM and Registers");
            loader = new ELFLoader();
            for (int i = 0; i < RAM.MemoryArray.Length; ++i)
                RAM.MemoryArray[i] = 0;
            for (int i = 0; i < Registers.MemoryArray.Length; ++i)
                Registers.MemoryArray[i] = 0;
        }

        //Returns checksum algorithm performed on RAM.
        public int CheckSum()
        {
            return RAM.CalculateChecksum();
        }

        //Resets program counter to program entry point stated in loader.elfHeader.e_entry
        //Resets stepNum to 1
        public void ResetPCSPandCPSR()
        {
            Tracer.Log(log + "Reseting Program Counter to program entry point: " + (int)loader.elfHeader.e_entry);
            if (loader == null)
                return;

            if (RAM.ReadWord(0) == 0)
            {
                Registers.WriteWord(Reg.R15, (int)loader.elfHeader.e_entry); //Sets program counter to program entry point
                Reg.Shelf = 0x7000;
                Registers.WriteWord(Reg.R13, Reg.Shelf);
                Registers.WriteWord(Reg.CPSR, Mode.System);
                OSInstalled = false;
            }
            else
            {
                Registers.WriteWord(Reg.R15, 0);
                Registers.WriteWord(Reg.CPSR, Mode.Supervisor);
                OSInstalled = true;
            }

            stepNum = 1;
        }

        //Runs the program making a fetch/decode/execute cycle in a background thread.
        //Stops execution when finished or when breakpoint is hit
        //Returns RunEventsArg e with events that indicate whether program stopped in a break or not.
        //e.StopCode == 0 stopped normally
        //e.StopCode == 1 stopped with breakpoint
        public void Run()
        {
            SystemFlags[SysFlag.Running] = true;
            Tracer.Log(log + "Running program in background thread...");

            while (true)
            {
                if (!SystemFlags[SysFlag.Running])
                    break;

                for (int i = 0; i < breakPoints.Count; ++i)
                {
                    if (PC == breakPoints[i])
                    {
                        if (isBreak)
                        {
                            isBreak = false;
                            continue;
                        }
                        isBreak = true;
                        Tracer.Log(log + "breakpoint" + (i + 1) + " hit; Terminating background thread.");
                        FireExecEvent(1);
                        return;
                    }
                }

                Step();
            }

            Tracer.Log(log + "Program terminated.");
            FireExecEvent(0);
        }

        //Make only one round of fetch/decode/execute on the CPU
        public int Step()
        {
            if (SystemFlags[SysFlag.Unsteppable])
            {
                Tracer.Log(log + "Program terminated.");
                FireExecEvent(0);
                return 0;
            }

            int opc = PC;
            int fetched = cpu.Fetch();

            cpu.Decode(fetched);

            PC += 4;

            cpu.Execute();
            CheckWriteChar();

            if (trace)
                TraceStep(opc);
            ++stepNum;

            if (IRQPending)
                ProcessIRQ();

            return -1;
        }

        //Switch processor execution to IRQ mode when interrupt is encountered
        void ProcessIRQ()
        {
            Registers.WriteWord(Reg.LR_IRQ, Registers.ReadWord(Reg.R15) + 4);
            Registers.WriteWord(Reg.SPSR_IRQ, Registers.ReadWord(Reg.CPSR));

            int cpsr = Registers.ReadWord(Reg.CPSR) & 0b1111111111111111111111111100000;
            Registers.WriteWord(Reg.CPSR, cpsr | Mode.IRQ); //switch to superviser
            Registers.SetFlag(Reg.CPSR, 7, true);
            Registers.WriteWord(Reg.R15, 0x18);
            IRQPending = false;
        }

        //Returns the boolean value of bit 7 of the CPSR (IRQ flag)
        public bool TestIRQFlag()
        {
            return Instruction.TestFlag(Registers.ReadWord(Reg.CPSR), 7);
        }

        //Checks whether to write a character
        //if true, writes the character into the character queue and resets pending status
        //then fires a write char event (stopecode = 2)
        void CheckWriteChar()
        {
            if (!RAM.pendingChar)
                return;
            RAM.pendingChar = false;
            ConsoleKeyQueue.Enqueue(RAM.characterWrite);
            FireExecEvent(2); //some of these might get ignored, so save to enqueue
        }

        //Fires an event to the main thread with agrument 'arg' to identify what the
        //event is about. See RunEventArgs for StopCode meaning
        void FireExecEvent(int arg)
        {
            RunEventArgs e = new RunEventArgs { StopCode = arg };
            OnStopExcecution(e);
        }

        //Reloads current file to memory
        public void ResetComputerState()
        {
            Tracer.Log(log + "Reloading " + currentFilename);
            LoadFileToRAM(currentFilename);
        }

        //Reset the system flags that control the programs execution (running or stepping)
        void ResetSystemFlags()
        {
            for (int i = 0; i < SystemFlags.Length; i++)
                SystemFlags[i] = false;
        }

        //Open trace file for writing
        public void OpenTraceFile()
        {
            Tracer.Log(log + "Opening Trace file for writing: " + traceFileName);
            traceFile = new StreamWriter(traceFileName);
        }

        //close tracefile for reading
        public void CloseTraceFile()
        {
            Tracer.Log(log + "Closing Trace file for reading: " + traceFileName);

            if (traceFile != null)
                traceFile.Close();
        }

        //Write data to trace file
        public void TraceStep(int opc)
        {
            int mode = ExecMode;
            if (mode != Mode.System && !traceAll)
                return;

            if (traceAll) //ignore reset handler
                if ((opc >= 0x88 && opc <= 0xb8) || opc == 0)
                    return;

            //step_number program_counter checksum nzcf mode r0 r1 r2 r3 r4 r5 r6 r7 r8 r9 r10 r11 r12 r13 r14
            string line = stepNum.ToString("D6") + " " + opc.ToString("X8") + " " + CheckSum().ToString("X8") + " ";
            string modestr;
            for (int i = 0; i < Flags.Length; ++i)
                line += Convert.ToInt32(Flags[i]);

            modestr = mode == Mode.IRQ ? "IRQ" : mode == Mode.Supervisor ? "SVC" : "SYS";
            line += " " + modestr + " ";

            int count = 0;
            for (int i = 0; i < 60; i += 4) //all system regs
            {
                line += count + "=" + Registers.ReadWord(i).ToString("X8") + " ";
                ++count;
            }

            traceFile.WriteLine(line);

            Tracer.Log(line);
        }

        //Event that fires when Run() finishes executing
        public event EventHandler<RunEventArgs> StopExcecution;

        //Fires event when program stops
        void OnStopExcecution(RunEventArgs e)
        {
            StopExcecution?.Invoke(this, e);
        }
    }

    //Arguments for my custom event: StopCode to determine the nature of the program's termination,
    //stopcode:
    //-1: to be ignored
    //0 : program ended normally
    //1 : program ended in a breakpoint
    //2 : program wants to write a character
    public class RunEventArgs : EventArgs
    {
        public int StopCode { get; set; }
        public char WriteChar { get; set; }
    }

    //All the reister names for easier access
    static class Reg
    {
        public const int R0 = 0;
        public const int R1 = 4;
        public const int R2 = 8;
        public const int R3 = 12;
        public const int R4 = 16;
        public const int R5 = 20;
        public const int R6 = 24;
        public const int R7 = 28;
        public const int R8 = 32;
        public const int R9 = 36;
        public const int R10 = 40;
        public const int R11 = 44;
        public const int R12 = 48;
        public const int R13 = 52;
        public const int R14 = 56;
        public const int R15 = 60;
        public const int CPSR = 64;
        public const int SP_SVC = 68;
        public const int LR_SVC = 72;
        public const int SP_IRQ = 76;
        public const int LR_IRQ = 80;
        public const int SPSR_SVC = 84;
        public const int SPSR_IRQ = 88;
        public static int Shelf = 0x7000; //Special register to keep track of where the stack starts... pun intended.
    }

    //All the flag names for easier access
    static class Flag
    {
        public const int NF = 0;
        public const int ZF = 1;
        public const int CF = 2;
        public const int FF = 3;
    }

    //All the system flags names for easier access
    static class SysFlag
    {
        public const int Running = 0;
        public const int Unsteppable = 1;
    }

    //All the different execution modes
    static class Mode
    {
        public const int Supervisor = 0b10011;
        public const int IRQ = 0b10010;
        public const int System = 0b11111;
    }
}