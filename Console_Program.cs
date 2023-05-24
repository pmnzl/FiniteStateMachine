using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FiniteStateMachine
{
	partial class Console_Program
	{
		static void Main()
		{
			//Initialise finite state tables.
			//Create finite state table 1
			FiniteStateTable FSM1 = new FiniteStateTable(3, 3, "FSM1", 0);
			//a																					b																					c
			FSM1.SetNextState(0, 0, 1); FSM1.SetActions(0, 0, new string[] { "X", "Y" }); FSM1.SetNextState(1, 0, 0); FSM1.SetActions(1, 0, new string[] { }); FSM1.SetNextState(2, 0, 0); FSM1.SetActions(2, 0, new string[] { });              //S0
			FSM1.SetNextState(0, 1, 0); FSM1.SetActions(0, 1, new string[] { "W" }); FSM1.SetNextState(1, 1, 2); FSM1.SetActions(1, 1, new string[] { "X", "Z" }); FSM1.SetNextState(2, 1, 1); FSM1.SetActions(2, 1, new string[] { });              //S1
			FSM1.SetNextState(0, 2, 0); FSM1.SetActions(0, 2, new string[] { "W" }); FSM1.SetNextState(1, 2, 2); FSM1.SetActions(1, 2, new string[] { }); FSM1.SetNextState(2, 2, 1); FSM1.SetActions(2, 2, new string[] { "X", "Y" });     //S2
			PrintToConsole(FSM1);

			FiniteStateTable FSM2 = CreateTask();

			//Initialise logger
			Logger Log = new Logger();

			//Initialise program variables.
			int CurrentEvent;
			ConsoleKey KeyIn; //Initialise keystroke identifier.
			bool Exit = false;
			bool Trigger = false;

			//Console program to run the finite state machines and display the appropriate information.
			do
			{
				//Match the keystroke up with an event trigger.
				KeyIn = Console.ReadKey(true).Key; //Read keystroke.
				Console.WriteLine("Key {0} Pressed.", KeyIn);
				if (KeyIn == ConsoleKey.A) { CurrentEvent = 0; }
				else if (KeyIn == ConsoleKey.B) { CurrentEvent = 1; }
				else if (KeyIn == ConsoleKey.C) { CurrentEvent = 2; }
				else if (KeyIn == ConsoleKey.Q) { Exit = true; CurrentEvent = -1; }
				else CurrentEvent = -1;

				//Execute each finite state table.
				//Check if finite state machines require triggering.
				if (CurrentEvent != -1) //Check for valid keystroke
				{
					if ((FSM2.State != FSM2.GetNextState(CurrentEvent, FSM2.State) && FSM2.State != 1) || (FSM2.State == 1 && FSM1.State == 1)) //Check if any change is necessary or in S1 condition.
					{
						Trigger = TriggerEvent(FSM2, CurrentEvent, ref Log, KeyIn, true); //Trigger FSM2 with the respective event, state and actions.
					}
					if (FSM1.State != FSM1.GetNextState(CurrentEvent, FSM1.State)) //Check if any change is necessary
					{
						Trigger = TriggerEvent(FSM1, CurrentEvent, ref Log, KeyIn, false); //Trigger FSM2 with the respective event, state and actions.
					}

					if (Trigger == true) //Check if any finite state machine has been triggered.
					{
						Trigger = false;
						PrintToConsole(FSM1);
						PrintToConsole(FSM2);
						Console.WriteLine("");
					}
				}
				else //Log keypresses that dont trigger either machine.
				{ 
					Log.AddLog(FSM1, CurrentEvent, ConvertState(FSM1), KeyIn);
				}
			} while (Exit == false);
			Log.SaveLog(); //Save log.
		}

		static void PrintToConsole(FiniteStateTable FSM)
		{
			//Console display.
			Thread.Sleep(10); //Used for console display so the following lines always show ater the actions.
			Console.WriteLine("[{0}] Now in State {1}", FSM.Name, ConvertState(FSM)); //Convert FSM states to letters and print to console.
		}

		static bool TriggerEvent(FiniteStateTable FSM, int CurrentEvent, ref Logger Log, ConsoleKey KeyIn, bool Threading)
		{
			//Add logs, invoke methods and change the current state of finite state table.
			FSM.InvokeMethods(CurrentEvent, FSM.State, Threading);
			Log.AddLog(FSM, CurrentEvent, ConvertState(FSM), KeyIn);
			FSM.State = FSM.GetNextState(CurrentEvent, FSM.State);
			return true;
		}

		static String ConvertState(FiniteStateTable FSM)
		{
			//Converts the index of a FSM state to its state name.
			String State = "NO_STATE";

			//FSM1 Conditions.
			if (FSM.Name == "FSM1")
			{
				if (FSM.State == 0) { State = "S0"; }
				else if (FSM.State == 1) { State = "S1"; }
				else if (FSM.State == 2) { State = "S2"; }
			}
			//FSM2 Conditions.
			else if (FSM.Name == "FSM2")
			{
				if (FSM.State == 0) { State = "SA"; }
				else if (FSM.State == 1) { State = "SB"; }
			}
			else Console.WriteLine("State conversion for {0} does not exist.", FSM.Name);
			return State;
		}
	}

	class Logger
	{
		//Class used to log time, events and actions of each finite state machine.
		public Logger() { Log = new List<String>(); }
		private readonly List<String> Log;

		public void AddLog(FiniteStateTable FSM, int CurrentEvent, String CurrentState, ConsoleKey KeyIn)
		{
			//Adds a log line of the current time the event triggered and the action to a bigger log list.
			String Timestamp = String.Format("{0:F}", DateTime.Now);

			//If event does not trigger a FSM...
			if (CurrentEvent == -1) { this.Log.Add(Timestamp + "\t\t" + KeyIn); return; }

			//Placeholder variables.
			String Actions = String.Join(", ", FSM.GetActions(CurrentEvent, FSM.State));
			String Event;

			//Convert numbered events to their state names.
			if (CurrentEvent == 0) { Event = "a"; }
			else if (CurrentEvent == 1) { Event = "b"; }
			else if (CurrentEvent == 2) { Event = "c"; }
			else { Event = " "; }
			this.Log.Add(Timestamp + "\t\t" + KeyIn + "\t\t" + FSM.Name + "\t\t" + Event + "\t\t" + CurrentState + "\t\t" + Actions);
		}

		public void SaveLog()
		{
			//Converts the log list to a file to save at a specified directory.
			bool Valid = false;
			String Path = " ";

			//Check if user inputted path is valid. If not get them to try again.
			while (Valid == false)
			{
				//Get user inputs.
				Console.Write("Save Log File: ");
				Path = Console.ReadLine();

				//Exception handler, in case user input is invalid.
				try
				{
					using FileStream CreateFile = File.Create(Path);
					Valid = true;
					CreateFile.Close();
				}
				catch (UnauthorizedAccessException) { Console.WriteLine("Error: Unauthorized Access."); }
				catch (ArgumentException) { Console.WriteLine("Error: Arguments Invalid."); }
				catch (PathTooLongException) { Console.WriteLine("Error: The Path Your Provided Is Too Long."); }
				catch (DirectoryNotFoundException) { Console.WriteLine("Error: Directory Not Found."); }
				catch (IOException) { Console.WriteLine("Error: IO Exception."); }
				catch (NotSupportedException) { Console.WriteLine("Error: Filename Invalid."); }
			}

			//Create File and StreamWriter to that file.
			using FileStream FS = File.OpenWrite(Path);
			using TextWriter SR = new StreamWriter(FS);

			//Writes log to a file and saves it
			SR.WriteLine("TIMESTAMP\t\t\t\tKEYPRESS\tMACHINE\t\tEVENT\t\tSTATE\t\tACTION(S)");
			foreach (String LogEvent in this.Log) { SR.WriteLine(LogEvent); }

			//Close StreamWriter and File
			SR.Close();
			FS.Close();
		}
	}
}