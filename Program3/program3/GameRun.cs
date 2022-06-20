///Tri Le
///CS441 - Artificial Inteligence
///Program 3 - Option Tictactoe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace program3
{
    class GameRun
    {
        Player _aiPlayer1 = null;
        Player _aiPlayer2 = null;

        Player _player1 = null;
        Player _player2 = null;
        Player _currentPlayer = null;
        TicTacToeAction _currentAction = null;
        TictactoeState _currentState = null;

        string _fileNameAIFirst = "01_train.db";
        string _fileNameAISecond = "02_train.db";

        /// <summary>
        /// Learning process
        /// </summary>
        void Learning()
        {
            _player1.Learning();
            _player2.Learning();

        }

        /// <summary>
        /// Update states for players
        /// </summary>
        void UpdateState()
        {
            _player1.UpdateState(_currentState);
            _player2.UpdateState(_currentState);
        }


        /// <summary>
        /// Reset value befor training
        /// </summary>
        void Reset()
        {
            _player1.Reset();
            _player2.Reset();
            _currentState = new TictactoeState();
            _currentState.Board = TictactoeState.CreateEmptyBoard();
            _currentPlayer = null;
            _currentAction = null;
        }

        /// <summary>
        /// Play game
        /// </summary>
        /// <param name="training"></param>
        /// <param name="printOut"></param>
        /// <returns></returns>
        public int Play(bool training = false, bool printOut = false)
        {
            Reset();
            UpdateState();

            while (true)
            {
                if (_currentPlayer == _player1)
                {
                    _currentPlayer = _player2;
                }
                else if (_currentPlayer == _player2)
                {
                    _currentPlayer = _player1;
                }
                else
                {
                    _currentPlayer = _player1;
                }

                if (printOut)
                    _currentState.PrintState();

                _currentAction = _currentPlayer.DeliverAction(_currentState, printOut);

                if (_currentAction != null)
                {
                    var tempState = _currentState.NextStateFromCurrentState(_currentAction, _currentPlayer);
                    tempState.PreviousState = _currentState;
                    tempState.PreviousPlayer = _currentPlayer;
                    tempState.PreviousAction = _currentAction;

                    _currentState = tempState;

                    UpdateState();
                }

                if (_currentState.CheckStop())
                {
                    if (training)
                        Learning();

                    if (printOut)
                        _currentState.PrintState();

                    return _currentState.TheWinner;
                }
            }

        }

        /// <summary>
        /// Initialize before training
        /// </summary>
        /// <param name="reloadPolicy"></param>
        public void IntialBeforeTraining(bool reloadPolicy = false) {
            _player1 = _aiPlayer1;
            _player2 = _aiPlayer2;

            if (reloadPolicy)
            {
                LoadAIPolicy();
            }
            (_player1 as AIPlayer).DynamicEpsilon = (_player1 as AIPlayer).Epsilon;
            (_player1 as AIPlayer).IncreaseExporeRateMode = true;

            (_player2 as AIPlayer).DynamicEpsilon = (_player2 as AIPlayer).Epsilon;
            (_player2 as AIPlayer).IncreaseExporeRateMode = true;
        }


        /// <summary>
        /// Train for AI
        /// </summary>
        /// <param name="epoches"></param>
        /// <param name="reloadPolicy"></param>
        public void TraniningAI(int epoches = 50000)
        {
            _player1 = _aiPlayer1;
            _player2 = _aiPlayer2;

            (_player1 as AIPlayer).TrainningMode = true;
            (_player2 as AIPlayer).TrainningMode = true;

            // Determin the number of epoch that need for trainning
            //int epoch = 0;

            //int lastEpoch = 0;
            //int lastChange1 = 0;
            //int lastChange2 = 0;

            //while (_player1.GetQTableSize() < 16500 && _player2.GetQTableSize() <16500)
            //{
            //    epoch++;
            //    lastChange1 = _player1.GetQTableSize();
            //    lastChange2 = _player2.GetQTableSize();

            //    Console.WriteLine($"Epoch = {epoch}, Last EpochUpdate = {lastEpoch}, Table1's size = {_player1.GetQTableSize()} , Table2's size = { _player2.GetQTableSize()}");
            //    Play(true);
            //    if (lastChange1 != _player1.GetQTableSize() || lastChange2 != _player2.GetQTableSize())
            //    {
            //        lastEpoch = epoch;
            //    }

            //}


            for (int epoch = 1; epoch <= epoches; epoch++)
            {
                Play(true);
            }

        }

        /// <summary>
        /// Save Policy
        /// </summary>
        public void SaveAIPolicy() {

            if (_aiPlayer1 != null && _aiPlayer2 != null) {
                (_aiPlayer1 as AIPlayer).SavePolicy(_fileNameAIFirst);
                (_aiPlayer2 as AIPlayer).SavePolicy(_fileNameAISecond);
            }
        }


        /// <summary>
        /// Load Policy
        /// </summary>
        public void LoadAIPolicy()
        {
            if (File.Exists(_fileNameAIFirst) && File.Exists(_fileNameAISecond))
            {
                (_aiPlayer1 as AIPlayer).LoadPolicy(_fileNameAIFirst);
                (_aiPlayer2 as AIPlayer).LoadPolicy(_fileNameAISecond);
            }
        }

        /// <summary>
        /// AI and Human Competition
        /// </summary>
        /// <param name="round"></param>
        public void AIPlayerAndHumanCompetetion(int round = 10)
        {
            double aiPoint = 0.0;
            double humanPoint = 0.0;
            for (int epoch = 1; epoch <= round; epoch++)
            {
                Console.WriteLine($"Round = {epoch}");
                char ch = ' ';
                while (!(ch == 'Y' || ch == 'y' || ch == 'n' || ch == 'N'))
                {
                    try
                    {
                        Console.Write($"\n Will AI go first ? (y/n)");
                        ch = Console.ReadKey().KeyChar;
                        Console.WriteLine("");

                    }
                    catch
                    {
                        Console.WriteLine($"\nInput Error");
                        ch = ' ';
                    }
                }

                if (ch == 'Y' || ch == 'y')
                {
                    _player1 = _aiPlayer1;
                    _player2 = new HumanPlayer(-1);
                    (_player1 as AIPlayer).TrainningMode = false;
                }
                else
                {
                    _player1 = new HumanPlayer(1);
                    _player2 = _aiPlayer2;
                    (_player2 as AIPlayer).TrainningMode = false;
                }


                int theWinner = Play(false, true);



                if (ch == 'Y' || ch == 'y')
                {
                    if (theWinner == _player1.GetID())
                    {
                        aiPoint += 1;
                        Console.WriteLine($"AI WON!");
                    }
                    else if (theWinner == _player2.GetID())
                    {
                        humanPoint += 1;
                        Console.WriteLine($"HUMAN WON!");
                    }
                    else
                    {
                        Console.WriteLine($"DRAW!");
                    }

                    Console.WriteLine($"AI points: {aiPoint} , Human Points = {humanPoint}");
                }
                else
                {
                    if (theWinner == _player2.GetID())
                    {
                        aiPoint += 1;
                        Console.WriteLine($"AI WON!");
                    }
                    else if (theWinner == _player1.GetID())
                    {
                        humanPoint += 1;
                        Console.WriteLine($"HUMAN WON!");
                    }
                    else
                    {
                        Console.WriteLine($"DRAW!");
                    }

                    Console.WriteLine($"AI points: {aiPoint} , Human Points = {humanPoint}");
                }
            }

        }

        public void SetAI(AIPlayer p1, AIPlayer p2) {

            _aiPlayer1 = p1;
            _aiPlayer2 = p2;

        }

        /// <summary>
        /// AI vs Random AI competition
        /// </summary>
        /// <param name="currentEpoch"></param>
        /// <param name="round"></param>
        /// <param name="fileResult"></param>
        /// <param name="showResult"></param>
        public void AIPlayerAndRandomPlayerCompetetion(int currentEpoch, int round = 10, string fileResult = null, bool showResult = true)
        {
            Random rand = new Random();
            int choose = 0;

            double aiPoint = 0.0;
            double randomAIPoint = 0.0;

            StringBuilder csv = new StringBuilder();

            for (int epoch = 1; epoch <= round; epoch++)
            {
                if (showResult) Console.WriteLine($"Round = {epoch}");
                choose = rand.Next(0, 2);

                if (choose == 1)
                {
                    _player1 = _aiPlayer1;
                    _player2 = new RandomPlayer(-1);

                    (_player1 as AIPlayer).TrainningMode = false;
                    
                }
                else
                {
                    _player1 = new RandomPlayer(1);
                    _player2 = _aiPlayer2;

                    (_player2 as AIPlayer).TrainningMode = false;
                    
                }

                int theWinner = Play(false, showResult);


                if (choose == 1)
                {
                    if (theWinner == _player1.GetID())
                    {
                        aiPoint += 1;
                        if (showResult) Console.WriteLine($"AI WON!");
                    }
                    else if (theWinner == _player2.GetID())
                    {
                        randomAIPoint += 1;
                        if (showResult) Console.WriteLine($"RANDOM AI WON!");
                    }
                    else
                    {
                        if (showResult) Console.WriteLine($"DRAW!");
                        aiPoint += 0.5;
                        randomAIPoint += 0.5;
                    }

                    if (showResult) Console.WriteLine($"AI points: {aiPoint} , Random AI Points = {randomAIPoint}");
                }
                else
                {
                    if (theWinner == _player2.GetID())
                    {
                        aiPoint += 1;
                        if (showResult) Console.WriteLine($"AI WON!");
                    }
                    else if (theWinner == _player1.GetID())
                    {
                        randomAIPoint += 1;
                        if (showResult) Console.WriteLine($"RANDOM AI WON!");
                    }
                    else
                    {
                        if (showResult) Console.WriteLine($"DRAW!");
                        aiPoint += 0.5;
                        randomAIPoint += 0.5;
                    }

                    if (showResult) Console.WriteLine($"AI points: {aiPoint} ,  Random AI Points = {randomAIPoint}");
                }

            }

            string newLine = string.Format($"{currentEpoch},{aiPoint / round }");
            csv.AppendLine(newLine);
            if (fileResult != null)
                File.AppendAllText(fileResult, csv.ToString());

        }
    }
}
