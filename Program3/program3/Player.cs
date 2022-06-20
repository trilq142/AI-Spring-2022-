///Tri Le
///CS441 - Artificial Inteligence
///Program 3 - Option Tictactoe

using System.Collections.Generic;

namespace program3
{
    abstract class Player
    {
        protected int _id;
        public abstract void Reset();
        public abstract void UpdateState(TictactoeState currentState);

        public abstract void Learning();

        public abstract TicTacToeAction DeliverAction(TictactoeState currentState,bool show);

        public int GetID() {
            return _id;
        }

        /// <summary>
        /// Get all possible actions
        /// </summary>
        /// <param name="currentState"></param>
        /// <returns></returns>
        public static List<TicTacToeAction> GetAllPossibleNextAction(TictactoeState currentState)
        {
            List<TicTacToeAction> allPossibleNextActions = new List<TicTacToeAction>();

            for (int i = 0; i < TictactoeState.MAX_LENGTH; i++)
                for (int j = 0; j < TictactoeState.MAX_LENGTH; j++)
                {
                    if (currentState.Board[i, j] == 0)
                        allPossibleNextActions.Add(new TicTacToeAction(i, j));
                }

            return allPossibleNextActions;
        }

        public abstract int GetQTableSize();
    }
}
