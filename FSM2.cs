using System;

namespace FiniteStateMachine
{
    partial class Console_Program
    {
        public static FiniteStateTable CreateTask()
        {
            //Create finite state table 2
            FiniteStateTable FSM2 = new FiniteStateTable(3, 2, "FSM2", 1);
            //a                                                                                     //b                                                                                     c
            FSM2.SetNextState(0, 0, 1); FSM2.SetActions(0, 0, new string[] { }); FSM2.SetNextState(1, 0, 0); FSM2.SetActions(1, 0, new string[] { }); FSM2.SetNextState(2, 0, 0); FSM2.SetActions(2, 0, new string[] { });                   //SA
            FSM2.SetNextState(0, 1, 0); FSM2.SetActions(0, 1, new string[] { "J", "K", "L" }); FSM2.SetNextState(1, 1, 0); FSM2.SetActions(1, 1, new string[] { "J", "K", "L" }); FSM2.SetNextState(2, 1, 0); FSM2.SetActions(2, 1, new string[] { "J", "K", "L" });     //SB
            PrintToConsole(FSM2);
            Console.WriteLine("");
            return FSM2;
        }
    }
}