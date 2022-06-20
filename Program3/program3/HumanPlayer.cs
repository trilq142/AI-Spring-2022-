///Tri Le
///CS441 - Artificial Inteligence
///Program 3 - Option Tictactoe
///
using System;
using System.Collections.Generic;

namespace program3
{
    class HumanPlayer : Player
    {
        
        public HumanPlayer(int id) {
            _id = id;
        }

        Dictionary<int,bool> listAvaiableAction = new Dictionary<int, bool>();
        
        /// <summary>
        /// Deliver an action based on the current state
        /// </summary>
        /// <param name="currentState"></param>
        /// <returns></returns>
        public override TicTacToeAction DeliverAction(TictactoeState currentState, bool show)
        {
            int action = 0;
            
            while (!listAvaiableAction.ContainsKey(action))
            {
                try
                {
                    Console.Write($"\nEnter a number (1-9):");
                    action = Convert.ToInt32(Console.ReadLine());

                }
                catch
                {
                    Console.WriteLine($"\nInput Error");
                    action = 0;
                }
            }
            
            int x = (action -1) / TictactoeState.MAX_LENGTH ;
            int y = (action -1) % TictactoeState.MAX_LENGTH ;

            return new TicTacToeAction(x, y);
        }

        /// <summary>
        /// Not use
        /// </summary>
        public override void Learning()
        {
           
        }

        /// <summary>
        /// Reset values
        /// </summary>
        public override void Reset()
        {
            listAvaiableAction.Clear();
            for (int i = 0; i < 9; i++)
            {
                listAvaiableAction.Add((i + 1),true);
            }
        }

        /// <summary>
        /// Update the current State
        /// </summary>
        /// <param name="currentState"></param>
        public override void UpdateState(TictactoeState currentState)
        {
            var allActions = GetAllPossibleNextAction(currentState);
            listAvaiableAction.Clear();
            foreach (var a in allActions)
            {
                listAvaiableAction.Add((a.X ) * TictactoeState.MAX_LENGTH + (a.Y + 1 ),true);
            }
        }

        /// <summary>
        /// Return the QTable size
        /// </summary>
        /// <returns></returns>
        public override int GetQTableSize()
        {
            throw new NotImplementedException();
        }

    }
}
