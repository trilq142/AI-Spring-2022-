///Tri Le
///CS441 - Artificial Inteligence
///Program 3 - Option Tictactoe

using System;
using System.Collections.Generic;


namespace program3
{
    class RandomPlayer : Player
    {
        Random random = new Random();
        List<int> listAvaiableAction = new List<int>();

        public RandomPlayer(int id) {
            _id = id;
        }

        /// <summary>
        /// Deliver an action base on the current state
        /// </summary>
        /// <param name="currentState"></param>
        /// <returns></returns>
        public override TicTacToeAction DeliverAction(TictactoeState currentState, bool show)
        {
            TicTacToeAction selectedAction = null;
            List<TicTacToeAction> allPossibleNextActions = GetAllPossibleNextAction(currentState);

            if (allPossibleNextActions.Count > 0)
            {
                selectedAction = allPossibleNextActions[random.Next(0, allPossibleNextActions.Count)];

                return selectedAction;
            }

            return selectedAction;
        }

        public override int GetQTableSize()
        {
            throw new NotImplementedException();
        }

        //Not use 
        public override void Learning()
        {
           
        }

        //Not use 
        public override void Reset()
        {

        }

        //Not use 
        public override void UpdateState(TictactoeState currentState)
        {

        }
    }
}
