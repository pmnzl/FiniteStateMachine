using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FiniteStateMachine
{
	class FiniteStateTable
	{
		public FiniteStateTable(int ix, int jx, String Name, int InitialState)
		{
			//Initialise new FST.
			this.FST = new cell_FST[ix, jx];
			this.Name = Name;
			this.State = InitialState;

			//Initialise each cell of the FST.
			for (int i = 0; i < ix; i++)
			{
				for (int j = 0; j < jx; j++)
				{
					this.FST[i, j].Actions = new List<String>();
				}
			}
		}

		protected struct cell_FST
		{
			//Finite State Table cell 'variables'.
			public int NextState;
			public List<String> Actions;
		}

		protected cell_FST[,] FST; //Finite State Table (as an array).
		public String Name { get; }
		public int State { get; set; }

		public void InvokeMethods(int ix, int jx, bool Threading)
		{
			//Invoke each action stored in a FST cell. Converts strings to delegates.
			foreach (string Method in this.FST[ix, jx].Actions)
			{
				MethodInfo ThisMethod = this.GetType().GetMethod(Method);

				if (Threading == true) { Task.Run(() => ThisMethod.Invoke(this, null)); }	//Calls each method on a new thread. () = Lambda function
				else { ThisMethod.Invoke(this, null); }     //Calls each method on the same thread.

			}
		}

		//Getters & Setters
		public void SetNextState(int ix, int jx, int State) { this.FST[ix, jx].NextState = State; }
		public void SetActions(int ix, int jx, String[] AddActions) { this.FST[ix, jx].Actions.AddRange(AddActions); }
		public int GetNextState(int ix, int jx) { return this.FST[ix, jx].NextState; }
		public String[] GetActions(int ix, int jx) { return this.FST[ix, jx].Actions.ToArray(); }

		//Actions (Methods)
		public void J() { Console.WriteLine("Action J On Thread {0}", Thread.CurrentThread.ManagedThreadId); }
		public void K() { Console.WriteLine("Action K On Thread {0}", Thread.CurrentThread.ManagedThreadId); }
		public void L() { Console.WriteLine("Action L On Thread {0}", Thread.CurrentThread.ManagedThreadId); }
		public void W() { Console.WriteLine("Action W On Thread {0}", Thread.CurrentThread.ManagedThreadId); }
		public void X() { Console.WriteLine("Action X On Thread {0}", Thread.CurrentThread.ManagedThreadId); }
		public void Y() { Console.WriteLine("Action Y On Thread {0}", Thread.CurrentThread.ManagedThreadId); }
		public void Z() { Console.WriteLine("Action Z On Thread {0}", Thread.CurrentThread.ManagedThreadId); }
	}
}