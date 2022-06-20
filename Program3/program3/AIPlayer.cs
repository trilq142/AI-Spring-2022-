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
    class AIPlayer : Player
    {
        Random random = new Random();

        public const double WIN_REWARD = 1.0;
        public const double LOSE_REWARD = 0.0;
        public const double DRAW_REWARD = 0.5;

        TictactoeState _currentState = null;

        Dictionary<(int,int), double> _qTable = new Dictionary<(int, int), double>();
        public AIPlayer(int id) {
            _id = id;
        }


        public double LearningRate { get ; set ; } = 1.0;
        public double Gamma { get; set; } = 0.9;
        public double Epsilon { get; set; } = 0.1;

        public double DynamicEpsilon { get; set; } = 0.1;
        public double Delta { get; set; } = 0.001;

        public double M { get; set; } =1000;

        public int CurrentEpoch { get; set; } = 0;

        public bool TrainningMode { get; set; } = true;

        public bool IncreaseExporeRateMode { get; set; } = true;


        /// <summary>
        /// Deliver an action based on the current state
        /// </summary>
        /// <param name="currentState"></param>
        /// <returns></returns>
        public override TicTacToeAction DeliverAction(TictactoeState currentState, bool show)
        {
            TicTacToeAction selectedAction = null;
            List<TicTacToeAction> allPossibleNextActions = GetAllPossibleNextAction(currentState);

            if (TrainningMode) {
                if (IncreaseExporeRateMode && DynamicEpsilon <= 1 && CurrentEpoch % M == 0 ) {
                    DynamicEpsilon += Delta;
                }

                if (Helpers.GetRandomDouble(0, 1) < (IncreaseExporeRateMode? DynamicEpsilon :Epsilon))
                {
                    selectedAction = allPossibleNextActions[random.Next(0, allPossibleNextActions.Count)];

                    return selectedAction;
                }
            }

            //Find action with max Q value
            double max = double.MinValue;
            List<TicTacToeAction> listSelectedAction = new List<TicTacToeAction>();

            foreach (var action in allPossibleNextActions)
            {
                var s = currentState.Hash();
                var a = action.Hash() * _id;

                double x = 0;
                if (currentState.PreviousPlayer != null)
                {
                    x = currentState.NextStateFromCurrentState(action, currentState.PreviousPlayer).GetReward(currentState.PreviousPlayer);
                    if (x >= max)
                    {
                        if (max == x)
                        {
                            listSelectedAction.Add(action);
                        }
                        else {
                            listSelectedAction.Clear();
                            listSelectedAction.Add(action);
                        }
                        max = x;
                    }
                }

                if (_qTable.ContainsKey((s,a)) && _qTable[(s, a)] >= max)
                {
                    if (_qTable[(s, a)] == max )
                    {
                        
                        listSelectedAction.Add(action);
                    }
                    else
                    {
                        listSelectedAction.Clear();
                        listSelectedAction.Add(action);
                        max = _qTable[(s, a)];
                    }

                }
            }

            if (listSelectedAction.Count > 0)
            {
                if (show) {
                    Console.Write($"Actions with QValue = {Math.Round(max,3)} is (");
                   // listSelectedAction.Sort((x1, x2) => x2.Item2.CompareTo(x1.Item2));
                    foreach (var action in listSelectedAction)
                    {
                        Console.Write($"{action.Hash()},");
                    }
                    Console.WriteLine(")");
                }


                selectedAction =  listSelectedAction[random.Next(0, listSelectedAction.Count)];
            }
            else {
                if (show) 
                    Console.Write($"Not know this case! Random choose the next!\n");
                selectedAction = allPossibleNextActions[random.Next(0, allPossibleNextActions.Count)];
            }

            if (show) {
                Console.Write($"Chose action = {selectedAction.Hash()} !\n");
            }
                
            return selectedAction;
        }

        /// <summary>
        /// Find the max Q value of the current state
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        double GetMaxQ(TictactoeState currentState, double c) {
            if (currentState != null && currentState.PreviousState != null) {
                var listAction = GetAllPossibleNextAction(currentState.PreviousState);
                double max = c;
                    //currentState.PreviousState.GetReward(currentState.PreviousState.PreviousPlayer);

                foreach (var action in listAction) {
                    var s = currentState.Hash();
                    var a = action.Hash() * _id;

                    if (_qTable.ContainsKey((s, a)) && _qTable[(s, a)] > max) {
                        max = _qTable[(s, a)];
                    }
                }
              
                return max;
                
            }
            return 0.0;
        }

        /// <summary>
        /// Learning process
        /// </summary>
        public override void Learning()
        {
              if (_currentState != null && _currentState.PreviousState != null)
            {
                double r = 0;
                double maxQNextState = 0;

                var currentState = _currentState;
                do
                {
                    
                    var s = currentState.PreviousState!= null? currentState.PreviousState.Hash(): 0;
                    var a = currentState.PreviousAction != null? currentState.PreviousAction.Hash():0;
                    var p = currentState.PreviousPlayer != null? currentState.PreviousPlayer.GetID():0;
                    a = a * p;

                    r = currentState.GetReward(this);

                    if (_qTable.ContainsKey((s, a)) == false)
                        _qTable.Add((s, a), 0.0);
                    
                    _qTable[(s, a)] += LearningRate * (r + Gamma * maxQNextState - _qTable[(s, a)]);
                    
                    maxQNextState = GetMaxQ(currentState,0);

                    currentState = currentState.PreviousState;
                } while (currentState != null);
            }
        }

        /// <summary>
        /// Reset the state
        /// </summary>
        public override void Reset()
        {
            _currentState = null;
        }

        /// <summary>
        /// Update the current state
        /// </summary>
        /// <param name="currentState"></param>
        public override void UpdateState(TictactoeState currentState)
        {
            _currentState = currentState;
        }

        /// <summary>
        /// Store policy to file
        /// </summary>
        /// <param name="fileName"></param>
        public void SavePolicy(string fileName) {
            ObjectStore.WriteToBinaryFile(fileName, _qTable);
        }

        /// <summary>
        /// Load Policy from file
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadPolicy(string fileName) {
            if (File.Exists(fileName)) {
                _qTable = ObjectStore.ReadFromBinaryFile<Dictionary<(int,int), double>>(fileName);
            }
        }

        /// <summary>
        /// Return the QTable size
        /// </summary>
        /// <returns></returns>
        public override int GetQTableSize()
        {
            return _qTable.Keys.Count;
        }
    }
}
