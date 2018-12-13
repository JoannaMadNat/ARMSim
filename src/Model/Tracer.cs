using System.Diagnostics;

/// <summary>
/// File: Tracer.cs
/// Custom class for Tracing the program. 
/// </summary>

namespace armsim.Model
{
    //This class uses Trace from System.Diagnostics but autamatically detects if trace switch is on.
    //Switch Can be changes in App.Config file
    public sealed class Tracer
    {
        private static Tracer instance = null; //Instance to use for singleton
        private static readonly object padlock = new object(); //lock to keep safe during multithreading
        static BooleanSwitch dataSwitch = new BooleanSwitch("Logging", "Logging Messages"); //Switch to determine whether to trace or not

        //Main and only instance of Tracer
        public static Tracer Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Tracer();
                    }
                    return instance;
                }
            }
        }

        //static function for access anywhere in the cde to drop a log line. thread-safe
        public static void Log(string line)
        {
            lock (padlock)
            {
                if (dataSwitch.Enabled) Trace.WriteLine(line);
            }
        }
    }
}
