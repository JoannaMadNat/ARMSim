using System;
/// <summary>
/// File: Options.cs
/// Uses NDesk Options to parse command line arguments.
/// </summary>
/// 

namespace armsim.Model
{
    //A class that stores the specified oftions of the program through the command line arguments.
    class Options
    {
        public string FileName { get; set; } //Name of the file to read from
        public int Memory_Size { get; private set; } //Maximum memory size for RAM
        public bool Exec { get; set; } //Whether to execute program on start
        public bool TraceAll { get; private set; } //Option to trace execution for all modes (SYS, IRQ, and SVC)

        //Constructor
        public Options()
        {
            Memory_Size = 32768; //Default memory size.
        }

        //Uses the function in OptionSet to parse arguments (retrieved in 'args')
        //sets FileName and Memory_Size to the corresponding options.
        public void ParseArgs(string[] args)
        {
            //Check if memory size provided is valid number, otherwise throw exception
            for (int i = 0; i < args.Length; ++i)
            {

                if (args[i] == "--mem" || args[i] == "--memory-size")
                {
                    try { ExtractMemSize(args[i + 1]); }
                    catch
                    {
                        Tracer.Log("Options.Parse: No memory size, or invalid memory size provided; Memory_Size set to default.");
                        Memory_Size = 32768;
                    }
                }
                else if (args[i] == "--l" || args[i] == "--load")
                    try { ExtractLoad(args[i + 1]); }
                    catch
                    {
                        FileName = null;
                        throw new Exception("Options.Parse: Wrong or no file name provided.");
                    }
                else if (args[i] == "--exec")
                {
                    Exec = true;
                }
                else if (args[i] == "--traceall")
                {
                    TraceAll = true;
                }
            }
        }

        //Parse memory size string 'mem' to an integer for Memory_Size
        void ExtractMemSize(string mem)
        {

            Memory_Size = Convert.ToInt32(mem);
            Tracer.Log("Options: Memory size set to " + Memory_Size);
        }

        //Sets FileName to the designated 'load' file name
        void ExtractLoad(string load)
        {
            if (load == "" || load == "--mem" || load == "--memory-size")
                //Throw exception if empty string is provided
                throw new Exception("Blank FileName, please provide file name.");

            FileName = load;
            Tracer.Log("Options.ParseArgs: FileName set to " + FileName);
        }
    }


}


