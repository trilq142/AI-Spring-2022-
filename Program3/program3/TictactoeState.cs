///Tri Le
///CS441 - Artificial Inteligence
///Program 3 - Option Tictactoe

using System;
using System.Collections.Generic;


namespace program3
{
    class TictactoeState
    {
        public static Dictionary<int, TictactoeState> StateDic = null;

        int[,] board = null;
        public const int MAX_LENGTH = 3;
        int theWinner = 0;
        bool canStop = false;
        int hashNum = -1;


        public TictactoeState PreviousState { get; set; } = null;
        public Player PreviousPlayer { get; set; } = null;
        public TicTacToeAction PreviousAction { get; set; } = null;
        public int TheWinner { get => theWinner; set => theWinner = value; }
        public int[,] Board { get => board;  set { board = value; hashNum = -1; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public TictactoeState() {
            
        }

        /// <summary>
        /// Return hash state
        /// </summary>
        /// <returns></returns>
        public int Hash()
        {
            if (hashNum != -1)
                return hashNum;
            hashNum = 0;

            for (int i = 0; i < MAX_LENGTH; i++)
            {
                int val = 0;
                for (int j = 0; j < MAX_LENGTH; j++)
                {
                    if (Board[i, j] == -1)
                        val += 2;
                    else if (Board[i, j] == 1)
                        val += 1;
                    hashNum += hashNum * MAX_LENGTH + val;
                }

            }

            return hashNum;
        }


        /// <summary>
        /// Check if current state is stop state
        /// Also determine the winner
        /// </summary>
        /// <returns>True : stop , False : continue</returns>
        public bool CheckStop() {

            //Horizontal check
            int sum = 0;
            for (int i = 0; i < MAX_LENGTH; i++)
            {
                sum = 0;
                for (int j = 0; j < MAX_LENGTH; j++)
                    sum += Board[i, j];

                if (sum == MAX_LENGTH) {
                    TheWinner = 1;
                    canStop = true;
                    return canStop;
                }

                if (sum == -MAX_LENGTH)
                {
                    TheWinner = -1;
                    canStop = true;
                    return canStop;
                }

            }
            
            //Check diagonal from left to right
            sum = 0;
            for (int i = 0; i < MAX_LENGTH; i++)
            {
                sum +=Board[i, i];
            }
            if (sum == MAX_LENGTH)
            {
                TheWinner = 1;
                canStop = true;
                return canStop;
            }

            if (sum == -MAX_LENGTH)
            {
                TheWinner = -1;
                canStop = true;
                return canStop;
            }


            //Check diagonal from right to left
            sum = 0;
            for (int i = 0; i < MAX_LENGTH; i++)
            {
                sum += Board[i, MAX_LENGTH - 1 - i];
            }

            if (sum == MAX_LENGTH)
            {
                TheWinner = 1;
                canStop = true;
                return canStop;
            }

            if (sum == -MAX_LENGTH)
            {
                TheWinner = -1;
                canStop = true;
                return canStop;
            }

            //Check column
            for (int j = 0; j < MAX_LENGTH; j++)
            {
                sum = 0;
                for (int i = 0; i < MAX_LENGTH; i++)
                    sum += Board[i, j];

                if (sum == MAX_LENGTH)
                {
                    TheWinner = 1;
                    canStop = true;
                    return canStop;
                }

                if (sum == -MAX_LENGTH)
                {
                    TheWinner = -1;
                    canStop = true;
                    return canStop;
                }
            }

            //Check if the game is over or not
            bool empty = true;
            for (int i = 0; i < MAX_LENGTH; i++)
                for (int j = 0; j < MAX_LENGTH; j++)
                    if (Board[i, j] == 0)
                    {
                        empty = false;
                        break;
                    }

            return empty;

        }

        /// <summary>
        /// Generate the next state from the current state
        /// </summary>
        /// <param name="action"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public TictactoeState NextStateFromCurrentState(TicTacToeAction action, Player player) {
            TictactoeState tictactoeState = new TictactoeState();
            tictactoeState.Board = (int[,]) Board.Clone();
            tictactoeState.Board[action.X, action.Y] = player.GetID();

            return tictactoeState;
        }

        /// <summary>
        /// Print out the state
        /// </summary>
        public void PrintState() {
            for (int i = 0; i < MAX_LENGTH; i++)
            {
                for (int j = 0; j < MAX_LENGTH; j++)
                {
                    if (Board[i, j] == 1)
                        Console.Write($" x ");
                    else if (Board[i, j] == -1)
                        Console.Write($" o ");
                    else Console.Write($" _ ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        /// <summary>
        /// Create an empty Board
        /// </summary>
        /// <returns></returns>
        public static int[,] CreateEmptyBoard() {
            int[,] currentStateArr = new int[MAX_LENGTH, MAX_LENGTH];

            for (int i = 0; i < MAX_LENGTH; i++)
                for (int j = 0; j < MAX_LENGTH; j++)
                    currentStateArr[i, j] = 0;

            return currentStateArr;
        }

        /// <summary>
        /// Clone this curreent state
        /// </summary>
        /// <returns></returns>
        public TictactoeState Clone()
        {
            TictactoeState newState = new TictactoeState();
            newState.Board = (int[,])this.Board.Clone();
            newState.canStop = this.canStop;
            newState.TheWinner = this.TheWinner;

            return newState;
        }

        /// <summary>
        /// Return the reward for player at this state
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double GetReward(Player p)
        {
            if (p != null) {
                if (CheckStop())
                {
                    if (TheWinner == p.GetID())
                    {
                        return AIPlayer.WIN_REWARD;
                    }
                    else if (TheWinner == -p.GetID())
                    {
                        return AIPlayer.LOSE_REWARD;
                    }
                    else
                    {
                        return AIPlayer.DRAW_REWARD;
                    }
                }
            }

            return 0.0;
        }
    }
}
