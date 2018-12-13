# Joanna Al Madanat Jaar
CpS: 310 | October 12, 2018 | 35:52 hrs

## Overview
armsim is a fully functional ARM Simulator that allows the user to load a file into a simulated ARM processor. The user may run the program through the command line or through the GUI. The GUI shows the state of the program and allows stepping and running.

## Features
All A level features implemented, including swi for input and output as well as handling and IRQ.

## Prerequisites
To run armsim, user must have Visual Studio 2017 and Windows 7 or later installed
Use Visual Studio to run the unit tests, using Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll

## Build and Test
 A ready-made Release build is stored in \install at the project directory.

#### To build the solution manualy:
* Open the solution file (armsim.sln) using Visual Studio 2017
* Go to the dropdown that says 'Debug' next to 'Any CPU' and change debug to 'Release'
* Go to the Build menu and select Build Solution
* The build files are located in the solution directory folder at bin > Release (Right-Click armsim in the project view and select 'Open Folder in File Explorer' to open the directory)

#### To run the unit tests:
* Open the solution file (armsim.sln) using Visual Studio 2017
* Copy the Test Files folder into the bin > Debug folder. (If there is no Debug folder, build the solution using the Debug option in the dropdown from 'To build the solution manualy')
* Make sure the 'Debug' dropdown in set to 'Debug' and not 'Release,' otherwise the tests cannot locate the test files needed for testing.
* In the Test Explorer, click 'Run All' to run all the unit tests and see their results, or right-click certain tests and 'Run Selected Tests' to run them seperately. (If Test Explorer is not open, go to Test > Windows > Test Explorer)

## Configuration
Go to the App.config file;
- To toggle logging on or off, switch the value of "Logging" to either 0 (to disable logging) or any non-zero number (to enable logging)
- To change the file in which the logging data is sent, change the value of initializeData to the name of the designated file.
<br>-- To enable tracing, in the GUI application, make sure the 'Enable Tracing' checkbox is ticked while the program is running. Uncheck the checkbox or close the application before opening the file to read it.

## User Guide
#### CMD startup
* open the command line.
* navigate to the loation of the .exe, and Type in the command line:<br> 
<i>> ARMSIM.exe --l|--load elf-file_name [--mem|--memory-size] memory_size_number [--exec] [--traceall]</i><br>
Where --l enters the location and name of the elf file 
<br> --m (optional) enters the amount of memory in RAM (Must be multiple of 4). default memory size is (32768).
<br> --exec (optional) loads the program and runs it with trscing enabled, and promptly closes the GUI. Trace lines dumped in Trace.Log. (No GUI interaction available when --exec is enabled)
<br> --traceall (optional) enables tracing steps during all modes including SVC and IRQ
<br><b>Note:</b> If --exec is enabled, and --l is not followed by a value or the value is invalid, the program will not execute and GUI manual interface will start for manual operation as normal.


#### GUI interface
* Double click armsim.exe, or start the program using the command line without the --exec option. 
* If no file has been loaded, click 'Load File' and select the .exe file to load.
* Press ![Play](\src\Images\play.png) to run the whole program
* Press ![Step](\src\Images\step.png) to step through the program one instruction at a time
* Press ![Stop](\src\Images\stop.png) to stop the program at the current instruction
* Press ![Reset](\src\Images\reset.png) to reset the Program Counter and go back to the start of the program
* Press ![BreakPoint](\src\Images\pause.png) to set and manage breakpoints to stop the program at (Write addresses where the PC should stop; must be a multiple of 4)
* Use the 'Go To' and RAM panel to view the contents of RAM by typing in the desgnated address (must be a multiple of 2)
* View the contents of the registers and stack with the two right panels respectively
* The dissassembly panel shows the current address pointed to by the PC, its value, and the decoded instruction in assembly language representation.
* Use the console to interat with the program when prompted
* The flags show the NZCF flags from the previous operation as well as the IRQ status.
* Toggle tracing on and off with the 'Enable Tracing' checkbox, the Trace data will be dumped in a file named 'Trace.Log' (untick the checkbox or close the application to open the file)
* To enable tracing using the GUI application, make sure the 'Enable Tracing' checkbox is ticked while the program is running. Uncheck the checkbox or close the application before opening the file to read it.

## Instruction implementation
* Data processing: <br>
	MOV, MVN, ADD, SUB, RSB, MUL, AND, ORR, EOR, BIC, CMP
	<br> Operand2 can be: Immediate, Register with Immediate Shift, Register with Register Shift
* Barrel shifter: <br> LSL, LSR, ASR, ROR
* Load/Store: <br> LDR/STR (word, unsigned byte) with preindexed addressing only (with and without writeback; no postindexed addressing)
* STM/LDM:<br> FD variant, with and without writeback. (also known as PUSH and POP)
* Branching:<br> B, BX, BL
* Interrups and Exceptions:<br> SWI and IRQ
* Misc: <br> MRS, MSR

## Bug Report
* The program sometimes skips breakpoints.

## Project Journal: 
[Project journal link](https://bju-my.sharepoint.com/:w:/g/personal/jalma146_students_bju_edu/EQ779K1bs8xHjkNbO4wUn6kBgZhhfJbhufpv1kXzC5tscQ?e=NlIj4F
)

## Academic Integrity Statement: 
By affixing my signature below, I certify that the accompanying work represents my own intellectual effort. Furthermore, I have received no outside help other than what is documented below.

| Date | URL | Nature of Help | Time Spent |
|:-:	|:-:	|:-:	|:-:	|	
|9/12/2018|https://www.wpf-tutorial.com/dialogs/the-openfiledialog/  |File opening dialog	|3 mins	|
|9/12/2018|https://stackoverflow.com/questions/4999988/to-clear-the-contents-of-a-file|Quick way to clear contents of a file. 	|less that 1 min	|
|9/15/2018|https://social.msdn.microsoft.com/Forums/vstudio/en-US/cf884a91-c135-447d-b16b-214d2d9e9972/capture-all-keyboard-input-regardless-of-what-control-has-focus?forum=wpf|How to retrieve input from the whole window| 4 mins
|9/17/2018|https://docs.microsoft.com/en-us/dotnet/standard/events/how-to-raise-and-consume-event|How to use event handlers in C#|3 mins
|9/18/2018|https://icons8.com/icon/new-icons/all|The icons for the GUI
|9/19/2018|https://stackoverflow.com/questions/20792054/when-resizing-window-elements-move|How to scale objects in the window| 1:44 hrs
|10/6/2018|https://stackoverflow.com/questions/3220579/does-msil-have-rol-and-ror-instructions|For the ROR instruction | 2 mins
|10/7/2018|https://stackoverflow.com/questions/8125127/what-is-the-c-sharp-equivalent-of-java-unsigned-right-shift-operator |shortened version of lsr | 2 mins
|11/6/2018|https://stackoverflow.com/questions/318777/c-sharp-how-to-translate-virtual-keycode-to-char| Converting keycodes to chars | 2 mins