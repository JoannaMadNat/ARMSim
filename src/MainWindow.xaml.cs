using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using armsim.Model;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Threading;

namespace armsim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Options options;
        Options options;
        BooleanSwitch dataSwitch = new
        BooleanSwitch("Logging", "Logging Messages");
        string log = "MainWindow: ";
        Computer Ada;
        System.Threading.Thread ExecuteProgram;
        DispatcherTimer checkProgramStopped;

        public MainWindow()
        {
            InitializeComponent();
            if (dataSwitch.Enabled) Trace.WriteLine("Program started...");

            try
            {
                Init();
            }
            catch (Exception e)
            {
                if (dataSwitch.Enabled) Trace.WriteLine("ERROR:: " + e.Message);
                Close();
            }
        }

        void Init()
        {
            EventManager.RegisterClassHandler(typeof(Control), KeyDownEvent, new KeyEventHandler(OnKeyDownHandler), true);
            EventManager.RegisterClassHandler(typeof(Control), KeyUpEvent, new KeyEventHandler(OnKeyUpHandler), true);

            options = new Options();
            options.ParseArgs(Environment.GetCommandLineArgs());

            Ada = new Computer(options.Memory_Size, Directory.GetCurrentDirectory() + "\\Trace.log", true);
            Ada.StopExcecution += OnStopExecution;
            Ada.traceAll = options.TraceAll;

            checkProgramStopped = new DispatcherTimer();
            checkProgramStopped.Interval = new TimeSpan(0, 0, 0, 0, 1);
            checkProgramStopped.Tick += CheckProgStopTick;

            if (options.FileName != null)
            {
                try
                {
                    LoadFile();
                    if (options.Exec)
                        Run();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error loading file, set to manual load.\nError Message: " + e.Message);
                }
            }
            else
            {
                if (dataSwitch.Enabled) Trace.WriteLine(log + "No file name provided; Manual setup enabled.");
                options.Exec = false;
            }
        }

        //THE EVENT HANDLER CANNOT ACCESS THE GUIIIIII, but thank goodness for DispatchTimers
        void OnStopExecution(object sender, RunEventArgs e)
        {
            progStopped = e.StopCode;
        }

        int progStopped = -1;
        void CheckProgStopTick(object sender, EventArgs e)
        {
            if (progStopped == -1)
                return;

            if (options.Exec && progStopped < 2)
                Environment.Exit(0);


            if (progStopped == 2)
            { //write character
                ClearKeyQueue();
                return;
            }

            Thread.Sleep(1); //clearing the buffer
            ClearKeyQueue();
            CanRunAgain();
            if (progStopped == 0)
                CannotGoForward();

            progStopped = -1;
            UpdatePanels();
        }
        void ClearKeyQueue()
        {
            while (Ada.ConsoleKeyQueue.Count > 0)
                Txt_Console.Text += Ada.ConsoleKeyQueue.Dequeue();
        }

        void CannotGoForward()
        {
            Btn_Run.IsEnabled = false;
            Btn_Step.IsEnabled = false;
        }

        bool keyboardListen = false;
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (keyboardListen)
            {
                switch (e.Key)
                {
                    case Key.O:
                        LoadFileDialog();
                        break;
                    case Key.Q:
                        if (Btn_Stop.IsEnabled)
                            Stop();
                        break;
                    case Key.T:
                        if (Chk_Tracing.IsEnabled)
                            Chk_Tracing.IsChecked = !Chk_Tracing.IsChecked;
                        break;
                    case Key.R:
                        if (Btn_Reset.IsEnabled)
                            Reset();
                        break;
                    case Key.B:
                        if (Btn_Breakpoint.IsEnabled)
                            PromptBreakpoint();
                        break;
                }
                keyboardListen = false;
            }

            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
                keyboardListen = true;

            if (e.Key == Key.F5)
                if (Btn_Run.IsEnabled)
                    Run();
            if (e.SystemKey == Key.F10)
                if (Btn_Step.IsEnabled)
                    Step();
        }

        void OnKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
                keyboardListen = false;
        }

        void LoadFileDialog()
        {
            string temp = options.FileName;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    options.FileName = openFileDialog.FileName;
                    LoadFile();
                }
                catch (Exception ex)
                {
                    if (temp != null)
                    {
                        options.FileName = temp;
                        Ada.currentFilename = temp;
                        Ada.ResetComputerState();
                        UpdatePanels();
                    }
                    MessageBox.Show("Error loading file:\n" + ex.Message);
                }
            }

        }

        void LoadFile()
        {
            Ada.LoadFileToRAM(options.FileName);
            UpdatePanels();
            EnableButtons();
            Txt_Console.Text = "";
        }

        void EnableButtons()
        {
            Btn_Breakpoint.IsEnabled = true;
            Btn_Reset.IsEnabled = true;
            Btn_Run.IsEnabled = true;
            Btn_Step.IsEnabled = true;
            Chk_Tracing.IsEnabled = true;
            Btn_navRAM.IsEnabled = true;
        }

        void UpdatePanels()
        {
            SetupStatusBar();
            SetupFlagsPanel();
            SetupRegistersPanel();
            NavigateRAMPanel(0x0);
            SetupStackPanel();
            UpdateAssemblyPanel();
        }

        int DetermineModeandShelf()
        {
            if (!Ada.OSInstalled)
                return 0;

            int mode = Ada.ExecMode;
            string modeText = "Processor Mode: ";
            switch (mode)
            {
                case Mode.IRQ:
                    Reg.Shelf = 0x7ff0;
                    modeText += "IRQ";
                    break;
                case Mode.Supervisor:
                    Reg.Shelf = 0x78f0;
                    modeText += "Supervisor";
                    break;
                default:
                    modeText += "System";
                    Reg.Shelf = 0x7000;
                    break;
            }
            Blk_ProcesorMode.Text = modeText;

            if (mode == Mode.Supervisor)
                return 16;
            if (mode == Mode.IRQ)
                return 24;
            return 0;
        }
        void SetupStackPanel()
        {
            Lst_Stack.Items.Clear();
            int SP = Ada.Registers.ReadWord(52 + DetermineModeandShelf()); //get correct stack register
            if (SP == Reg.Shelf)
            {
                Lst_Stack.Items.Add("Stack is Empty.");
                return;
            }

            for (int i = Reg.Shelf - 4; i >= SP; i -= 4)
            {
                Lst_Stack.Items.Add("0x" + i.ToString("X8") + ": " + "0x" + Ada.RAM.ReadWord(i).ToString("X8"));
            }
        }

        void SetupStatusBar()
        {
            TxtBlk_Checksum.Text = "Checksum: " + Ada.CheckSum();
            string[] split = options.FileName.Split('\\');
            TxtBlk_FileName.Text = split[split.Length - 1];
        }

        void SetupRegistersPanel()
        {
            Lst_Registers.Items.Clear();
            int regCount = 0;
            for (int i = 0; i < Ada.Registers.MemoryArray.Length; i += 4)
            {
                string str = "";
                if (i <= Reg.R15)
                    str = "R" + regCount++;
                else switch (i)
                    {
                        case Reg.CPSR:
                            str = "CPSR: ";
                        break;
                        case Reg.SP_SVC:
                            str = "SP_SVC: ";
                            break;
                        case Reg.LR_SVC:
                            str = "LR_SVC: ";
                            break;
                        case Reg.SP_IRQ:
                            str = "SP_IRQ: ";
                            break;
                        case Reg.LR_IRQ:
                            str = "LR_IRQ: ";
                            break;
                        case Reg.SPSR_SVC:
                            str = "SPSR_SVC: ";
                            break;
                        case Reg.SPSR_IRQ:
                            str = "SPSR_IRQ: ";
                            break;
                    }
                str += "\t0x" + Ada.Registers.ReadWord(i).ToString("X8");
                Lst_Registers.Items.Add(str);
            }
        }

        void SetupFlagsPanel()
        {
            string tab = "  ";
            TxtBlk_Flags.Text = tab + tab + "N: " + Ada.Flags[Flag.CF] + tab + "Z: " + Ada.Flags[Flag.ZF] + tab + "C: " + Ada.Flags[Flag.NF] + tab + "F: " + Ada.Flags[Flag.FF] + tab + "IRQ: " + Ada.TestIRQFlag() ;
        }

        void UpdateAssemblyPanel()
        {
            lstView_Dissassembly.Items.Clear();
            int rows = 5;
            int PC = Ada.PC;

            for (int i = PC - 4 * rows; i < PC + 4 * rows; i += 4)
            {
                if (i < 0 || i > Ada.RAM.MemoryArray.Length)
                {
                    LstStruct empty = new LstStruct
                    {
                        Address = "0x" + i.ToString("X8"),
                        Values = "",
                        Instruction = ""
                    };
                    continue;
                }

                int instCode = Ada.RAM.ReadWord(i);

                LstStruct row = new LstStruct 
                {
                    Address = "0x" + i.ToString("X8"),
                    Values = StringDecoder.Decode(instCode, Ada.Registers, Ada.RAM, Ada.Flags, i),
                    Instruction = instCode.ToString("X8")
                };
                lstView_Dissassembly.Items.Add(row);
            }
            lstView_Dissassembly.SelectedIndex = rows;
        }

        private void Btn_LoadFile_Click(object sender, RoutedEventArgs e)
        {
            LoadFileDialog();
        }

        private void Btn_Run_Click(object sender, RoutedEventArgs e)
        {
            Run();
        }
        void Run()
        {
            checkProgramStopped.Start();
            CannotRun();
            try
            {
                ExecuteProgram = new Thread(new ThreadStart(Ada.Run));
                ExecuteProgram.Start();
            }
            catch(Exception e)
            {
                MessageBox.Show("ERROR: Program stopped working:\n\n" + e.Message);
            }
        }

        private void Btn_Step_Click(object sender, RoutedEventArgs e)
        {
            Step();
        }

        void Step()
        {
            if (Ada.Step() == 0)
                CannotGoForward();
            UpdatePanels();
        }

        private void Btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        void Stop()
        {
            checkProgramStopped.Stop();
            CanRunAgain();
            ExecuteProgram.Abort();
            UpdatePanels();
        }

        void CannotRun()
        {
            Btn_Stop.IsEnabled = true;
            Btn_Run.IsEnabled = false;
            Btn_Step.IsEnabled = false;
            Btn_Reset.IsEnabled = false;
            Btn_Breakpoint.IsEnabled = false;
        }

        void CanRunAgain()
        {
            Btn_Stop.IsEnabled = false;
            Btn_Run.IsEnabled = true;
            Btn_Step.IsEnabled = true;
            Btn_Reset.IsEnabled = true;
            Btn_Breakpoint.IsEnabled = true;
        }

        private void Btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        void Reset()
        {
            Ada.ResetComputerState();
            UpdatePanels();
            EnableButtons();
            Txt_Console.Text = "";
        }

        private void Btn_Breakpoint_Click(object sender, RoutedEventArgs e)
        {
            PromptBreakpoint();
        }

        //Learned this during the summer. Creates a second window.
        void PromptBreakpoint()
        {
            BreakpointDialog edit = new BreakpointDialog(ref Ada.breakPoints)
            {
                ShowInTaskbar = false,
                Owner = Application.Current.MainWindow
            };
            edit.ShowDialog();
        }

        private void Chk_Tracing_Checked(object sender, RoutedEventArgs e)
        {
            if (Ada != null)
            {
                Ada.trace = true;
                Ada.OpenTraceFile();
            }
        }

        private void Chk_Tracing_Unchecked(object sender, RoutedEventArgs e)
        {
            Ada.trace = false;
            Ada.CloseTraceFile();
        }

        private void Btn_navRAM_Click(object sender, RoutedEventArgs e)
        {
            int address;
            if (txt_navRAM.Text.Trim() == "")
                return;

            try
            {
                address = int.Parse(txt_navRAM.Text, System.Globalization.NumberStyles.HexNumber);
                NavigateRAMPanel(address);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error parsing address:\n" + ex.Message + "\nExample input: 1EE759EA4");
            }

        }

        void NavigateRAMPanel(int startAddress)
        {
            LstView_RAM.Items.Clear();
            int rows = 6, bytes = 16;
            if (startAddress % 2 != 0 || startAddress + (bytes * rows) >= Ada.RAM.MemoryArray.Length || startAddress < 0)
                throw new Exception("Invalid memory address.");

            for (int i = startAddress; i < startAddress + (rows * bytes); i += bytes)
            {
                LstStruct row = new LstStruct
                {
                    Address = "0x" + i.ToString("X8"),
                    Values = ""
                };
                for (int j = 0; j < bytes; j++)
                    row.Values += Ada.RAM.MemoryArray[i + j].ToString("X2") + "    ";

                LstView_RAM.Items.Add(row);
            }
        }

        public struct LstStruct
        {
            string address;
            public string Address
            {
                get { return address; }
                set { address = value; }
            }
            string values;
            public string Values
            {
                get { return values; }
                set { values = value; }
            }
            string instruction;
            public string Instruction
            {
                get { return instruction; }
                set { instruction = value; }
            }
        }

        private void Txt_Console_KeyDown(object sender, KeyEventArgs e)
        {
            if (Ada.TestIRQFlag() && Ada.SystemFlags[SysFlag.Running]) //check irq enabled
                return;

            Ada.RAM.characterRead = ToChar(e.Key);
            Ada.IRQPending = true;
        }

        //THIS IS A NIGHTMAAAAREE!!!!
        //https://stackoverflow.com/questions/318777/c-sharp-how-to-translate-virtual-keycode-to-char
        char ToChar(Key key)
        {
            if (key == Key.Enter)
                return (char)13;

            char c;
            if ((key >= Key.A) && (key <= Key.Z))
            {
                c = (char)((int)'a' + (int)(key - Key.A));
            }
            else if ((key >= Key.D0) && (key <= Key.D9))
            {
                c = (char)((int)'0' + (int)(key - Key.D0));
            }
            else return '~';

            return c;
        }
    }
}


