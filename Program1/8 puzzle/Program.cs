///Tri Le
///CS 441 - AI - Spring 2022
///Program 1

using System;
using System.Collections.Generic;

namespace SearchingAlgorithms
{
    interface ISearchNode : ICloneable
    {
        List<ISearchNode> GenerateNewChildStates();
        public int PathCost { get; set; }
        public int FCost { get; set; }

        public String HashCode { get; set; }
        public ISearchNode Parent { get; set; }
        int XBlank { get; set; }

        public object GetState();
        public void Print(int type);
        public void BuildHashCode();
    }

    interface ISearch
    {
        public const int MAX_MOVES = 10000;
        public delegate int FHeursitic(ISearchNode currentState, ISearchNode goal);
        public int Run();

        public ISearchNode InitState { get; set; }
        public ISearchNode Goal { get; set; }
        public FHeursitic Heuristic { get; set; }

        public void PrintPath(int type);
        public List<ISearchNode> Path { get; set; }
    }

    class NPuzzle : ISearchNode
    {
        int n = 0;
        int[,] board;

        public ISearchNode Parent { get; set; }
        public String HashCode { get; set; }

        public int XBlank { get; set; } = -1;
        public int YBlank { get; set; } = -1;

        public int PathCost { get; set; }

        public int FCost { get; set; }

        public int[,] Board
        {
            get => board;
            set
            {
                board = value;
                FindBlank();
                BuildHashCode();
            }
        }

        public virtual int N
        {
            get => n;
            set
            {
                n = value;
                board = new int[n, n];
            }
        }

        /// <summary>
        /// Check before changing
        /// </summary>
        /// <param name="direction">
        /// The direction of the moves. 
        /// 0:left, 1:right 2:up 3:down
        /// </param>
        /// <returns>
        /// true: can move by this direction
        /// false: cannot move by this direction
        /// </returns>
        public bool CanChange(int direction)
        {
            while (true)
            {

                if (direction == 0 && XBlank > 0) //Left
                    return true;

                if (direction == 1 && XBlank < N - 1) //Right
                    return true;

                if (direction == 2 && YBlank > 0) //Up
                    return true;

                if (direction == 3 && YBlank < N - 1) //Down
                    return true;

                break;
            }

            return false;
        }

        /// <summary>
        /// Change the state
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool Change(int direction)
        {
            if (direction >= 0 && direction <= 3)
            {
                int changeX = 0;
                int changeY = 0;

                switch (direction)
                {
                    case 0:
                        changeX = -1;
                        break;
                    case 1:
                        changeX = 1;
                        break;
                    case 2:
                        changeY = -1;
                        break;
                    case 3:
                        changeY = 1;
                        break;
                }

                Board[XBlank, YBlank] = Board[XBlank + changeX, YBlank + changeY];
                XBlank += changeX;
                YBlank += changeY;

                Board[XBlank, YBlank] = 0;
                BuildHashCode();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Find the blank position
        /// </summary>
        void FindBlank()
        {
            for (short i = 0; i < this.N; i++)
                for (short j = 0; j < this.N; j++)
                {
                    if (this.Board[i, j] == 0)
                    {
                        XBlank = i;
                        YBlank = j;
                        break;
                    }
                }
        }

        /// <summary>
        /// Generate child states
        /// </summary>
        /// <returns></returns>
        List<ISearchNode> ISearchNode.GenerateNewChildStates()
        {
            List<ISearchNode> result = new List<ISearchNode>();

            for (int i = 0; i < 4; i++)
            {
                if (this.CanChange(i))
                {
                    NPuzzle newPuzzle = (NPuzzle)Clone();
                    newPuzzle.Change(i);

                    newPuzzle.PathCost++;
                    newPuzzle.Parent = this;
                    result.Add(newPuzzle);
                }
            }

            return result;
        }

        /// <summary>
        /// Clone the current object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            NPuzzle newPuzzle = new NPuzzle();
            newPuzzle.N = this.N;

            for (int i = 0; i < this.N; i++)
                for (int j = 0; j < this.N; j++)
                {
                    newPuzzle.Board[i, j] = this.Board[i, j];
                }

            newPuzzle.XBlank = this.XBlank;
            newPuzzle.YBlank = this.YBlank;
            newPuzzle.Parent = this.Parent;

            newPuzzle.PathCost = this.PathCost;
            newPuzzle.FCost = 0;
            return newPuzzle;
        }

        object ISearchNode.GetState()
        {
            return board;
        }

        /// <summary>
        /// Print the current state
        /// </summary>
        /// <param name="type"></param>
        public void Print(int type)
        {
            if (type == 2)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (board[i, j] != 0)
                            Console.Write(board[i, j] + " ");
                        else
                            Console.Write("_ ");
                    }
                    Console.WriteLine("");
                }
            }
            else {
                Console.Write($"({HashCode})");
            }

        }

        /// <summary>
        /// Generate a hashcode for the array board
        /// </summary>
        public void BuildHashCode()
        {
            HashCode = "";
            foreach (int value in board)
            {
                if (value != 0)
                    HashCode += " " + value.ToString();
                else
                    HashCode += " b";
            }
        }
    }

    class BestFirstSearch : ISearch
    {
        protected List<ISearchNode> openState = null;
        protected Dictionary<String, bool> closeState = null;

        public ISearchNode InitState { get; set; }
        public ISearchNode Goal { get; set; }

        public ISearch.FHeursitic Heuristic { get; set; }
        public List<ISearchNode> Path { get; set; }

        /// <summary>
        /// Add node into queue
        /// </summary>
        /// <param name="state"></param>
        protected void Enqueue(ISearchNode state)
        {
            if (!closeState.ContainsKey(state.HashCode))
            {
                closeState.Add(state.HashCode, true);
                openState.Add(state);
            }
        }

        /// <summary>
        /// Remove node from queue
        /// </summary>
        /// <returns></returns>
        protected ISearchNode Dequeue()
        {
            if (openState.Count > 0)
            {
                var result = openState[0];
                openState.RemoveAt(0);
                return result;
            }

            return null;
        }

        /// <summary>
        /// Sort the queue
        /// </summary>
        protected void SortQueue()
        {
            openState.Sort(
             (left, right) => left.FCost - right.FCost 
            );
        }

        /// <summary>
        /// Print path result
        /// </summary>
        /// <param name="type"></param>
        public void PrintPath(int type)
        {
            if (Path != null && Path.Count > 0)
            {
                bool first = true;

                foreach (ISearchNode path in Path)
                {
                    if (first)
                    {
                        first = false;
                    }
                    
                    else if (type == 1)
                        Console.Write("→");
                    else
                        Console.WriteLine("");

                    path.Print(type);
                }
                Console.WriteLine("");
            }
        }

        /// <summary>
        /// Run searching
        /// </summary>
        /// <returns></returns>
        public virtual int Run()
        {
            if (InitState == null || Goal == null)
                return -1;

            openState = new List<ISearchNode>();
            Path = new List<ISearchNode>();
            closeState = new Dictionary<String, bool>();

            InitState.FCost = Heuristic(InitState, Goal);

            Enqueue(InitState);
            ISearchNode currentState = null;

            int moves = -1;

            while (openState.Count > 0 && moves < ISearch.MAX_MOVES)
            {
                currentState = Dequeue();
                moves++;
                if (currentState.HashCode == Goal.HashCode)
                    break;

                var newStates = currentState.GenerateNewChildStates();

                if (newStates != null && newStates.Count > 0)
                {
                    foreach (var newState in newStates)
                    {
                        newState.FCost = Heuristic(newState, Goal);
                        Enqueue(newState);
                    }
                    SortQueue();
                }

            }

            if (currentState != null && currentState.HashCode == Goal.HashCode)
            {
                Path.Add(currentState);

                while (currentState.Parent != null)
                {
                    Path.Insert(0, currentState.Parent);
                    currentState = currentState.Parent;
                }
            }
            return moves;
        }
    }

    class AStart : BestFirstSearch
    {
        /// <summary>
        /// Calculate the F-score
        /// </summary>
        /// <param name="state"></param>
        /// <param name="heuristicVal"></param>
        /// <returns></returns>
        int CalculateFScore(ISearchNode state, int heuristicVal)
            => state.PathCost + heuristicVal;

        /// <summary>
        /// Run Searching
        /// </summary>
        /// <returns></returns>
        public override int Run()
        {
            if (InitState == null || Goal == null)
                return -1;

            openState = new List<ISearchNode>();
            Path = new List<ISearchNode>();
            closeState = new Dictionary<String, bool>();

            InitState.FCost = CalculateFScore(InitState, Heuristic(InitState, Goal));

            Enqueue(InitState);
            ISearchNode currentState = null;
            int moves = -1;

            while (openState.Count > 0 && moves < ISearch.MAX_MOVES)
            {
                currentState = Dequeue();
                moves++;

                if (currentState.HashCode == Goal.HashCode)
                    break;

                var newStates = currentState.GenerateNewChildStates();

                if (newStates != null && newStates.Count > 0)
                {
                    foreach (var newState in newStates)
                    {
                        newState.FCost = CalculateFScore(newState, Heuristic(newState, Goal));
                        Enqueue(newState);
                    }
                    SortQueue();
                }
            }

            if (currentState != null && currentState.HashCode == Goal.HashCode)
            {
                Path.Add(currentState);

                while (currentState.Parent != null)
                {
                    Path.Insert(0, currentState.Parent);
                    currentState = currentState.Parent;
                }
            }

            return moves;
        }
    }

    class Program
    {
        public struct Experiment
        {
            public NPuzzle IntialState { get; set; }
            public NPuzzle Goal { get; set; }
        }

        /// <summary>
        /// Tiles out-of-place Heuristic
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static int Heuristic1(ISearchNode currentState, ISearchNode goal)
        {
            int cost = 0;
            NPuzzle cs = ((NPuzzle)currentState);
            NPuzzle go = ((NPuzzle)goal);

            for (int i = 0; i < cs.N; i++)
                for (int j = 0; j < cs.N; j++)
                {
                    if (cs.Board[i, j] != 0 && cs.Board[i, j] != go.Board[i, j])
                    {
                        cost++;
                    }
                }

            return cost;
        }

        /// <summary>
        /// Manhattan disance
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static int Heuristic2(ISearchNode currentState, ISearchNode goal)
        {
            int cost = 0;
            NPuzzle cs = ((NPuzzle)currentState);
            NPuzzle go = ((NPuzzle)goal);

            bool continueRun = false;

            for (int i = 0; i < cs.N; i++)
                for (int j = 0; j < cs.N; j++)
                {
                    continueRun = true;

                    for (int k = 0; k < cs.N && continueRun; k++)
                        for (int h = 0; h < cs.N && continueRun; h++)
                        {
                            if (cs.Board[i, j] != 0 && cs.Board[i, j] == go.Board[k, h])
                            {
                                cost += Math.Abs(i - k) + Math.Abs(j - h);
                                continueRun = false;
                            }
                        }
                }

            return cost;
        }

        /// <summary>
        /// My heuristic
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static int Heuristic3(ISearchNode currentState, ISearchNode goal)
        {
            double cost = 0;
            NPuzzle cs = ((NPuzzle)currentState);
            NPuzzle go = ((NPuzzle)goal);

            bool continueRun = false;

            for (int i = 0; i < cs.N; i++)
                for (int j = 0; j < cs.N; j++)
                {
                    continueRun = true;

                    for (int k = 0; k < cs.N && continueRun; k++)
                        for (int h = 0; h < cs.N && continueRun; h++)
                        {
                            if (cs.Board[i, j] == go.Board[k, h])
                            {
                                cost += (Math.Abs(i - k) + Math.Abs(j - h)) / (1.5 + Math.Abs(go.XBlank - i) + Math.Abs(go.YBlank - j));
                                continueRun = false;
                            }
                        }
                }

            return (int) Math.Round(cost,0);
        }

        static void Main(string[] args)
        {
            int chooseType = 0;
            Experiment[] experiments = null;

            while (chooseType != 1 && chooseType != 2 && chooseType != 3)
            {
                Console.WriteLine("Choose Type of Puzzle");
                Console.WriteLine("1) 8 puzzle.");
                Console.WriteLine("2) 15 puzzle");

                Console.WriteLine("4) Exit");
                Console.Write("Press 1, 2,or 3?");

                try
                {
                    chooseType = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    chooseType = 0;
                }
            }

            if (chooseType == 3) {
                Console.WriteLine("Exited!");
                return;
            }

            if (chooseType == 1)
            {
                experiments = new Experiment[] {
                        new Experiment {
                            IntialState = new NPuzzle
                            {
                                N = 3,
                                Board = new int[,] {
                                    {1,2,3},
                                    {4,8,5},
                                    {7,0,6}
                                }
                            },
                            Goal= new NPuzzle
                            {
                                N = 3,
                                Board = new int[,] {
                                    {1,2,3},
                                    {4,5,6},
                                    {7,8,0}
                                }
                            }
                        },
                        //Experiment 2
                        new Experiment {
                            IntialState = new NPuzzle
                            {
                                N = 3,
                                Board = new int[,] {
                                    {1,3,0},
                                    {4,2,6},
                                    {7,5,8}
                                }
                            },
                            Goal= new NPuzzle
                            {
                                N = 3,
                                Board = new int[,] {
                                    {1,2,3},
                                    {4,5,6},
                                    {7,8,0}
                                }
                            }
                        },
                        //Experiment 3
                        new Experiment {
                            IntialState = new NPuzzle
                            {
                                N = 3,
                                Board = new int[,] {
                                    {2,0,3},
                                    {1,6,8},
                                    {4,7,5}
                                }
                            },
                            Goal= new NPuzzle
                            {
                                N = 3,
                                Board = new int[,] {
                                    {1,2,3},
                                    {4,5,6},
                                    {7,8,0}
                                }
                            }
                        },
                        //Experiment 4
                        new Experiment {
                            IntialState = new NPuzzle
                            {
                                N = 3,
                                Board = new int[,] {
                                    {4,1,3},
                                    {7,2,5},
                                    {8,0,6}
                                }
                            },
                            Goal= new NPuzzle
                            {
                                N = 3,
                                Board = new int[,] {
                                    {1,2,3},
                                    {4,5,6},
                                    {7,8,0}
                                }
                            }
                        },
                        //Experiment 5
                        new Experiment {
                            IntialState = new NPuzzle
                            {
                                N = 3,
                                Board = new int[,] {
                                    {8,1,3},
                                    {4,0,2},
                                    {7,6,5}
                                }
                            },
                            Goal= new NPuzzle
                            {
                                N = 3,
                                Board = new int[,] {
                                    {1,2,3},
                                    {4,5,6},
                                    {7,8,0}
                                }
                            }
                        },
                };
            }
            else
            {
                experiments = new Experiment[] {
                    //Experiment 1
                    new Experiment {
                        IntialState = new NPuzzle
                        {
                            N = 4,
                            Board = new int[,] {
                                {1,2,3,4},
                                {5,6,7,0},
                                {10,11,12,8},
                                {9,13,14,15},
                            }
                        },
                        Goal= new NPuzzle
                        {
                            N = 4,
                            Board = new int[,] {
                                {1,2,3,4},
                                {5,6,7,8},
                                {9,10,11,12},
                                {13,14,15,0},
                            }
                        }
                    },
                   //Experiment 2
                    new Experiment {
                        IntialState = new NPuzzle
                        {
                            N = 4,
                            Board = new int[,] {
                                {0,6,2,4},
                                {1,5,3,8},
                                {9,11,7,12},
                                {13,10,14,15},
                            }
                        },
                        Goal= new NPuzzle
                        {
                            N = 4,
                            Board = new int[,] {
                                {1,2,3,4},
                                {5,6,7,8},
                                {9,10,11,12},
                                {13,14,15,0},
                            }
                        }
                    },
                    //Experiment 3
                    new Experiment {
                        IntialState = new NPuzzle
                        {
                            N = 4,
                            Board = new int[,] {
                                {1,2,3,4},
                                {0,10,6,7},
                                {5,9,11,8},
                                {13,14,15,12},
                            }
                        },
                        Goal= new NPuzzle
                        {
                            N = 4,
                            Board = new int[,] {
                                {1,2,3,4},
                                {5,6,7,8},
                                {9,10,11,12},
                                {13,14,15,0},
                            }
                        }
                    },
                    //Experiment 4
                    new Experiment {
                        IntialState = new NPuzzle
                        {
                            N = 4,
                            Board = new int[,] {
                                {1,3,7,4},
                                {5,2,8,0},
                                {9,6,11,12},
                                {13,10,14,15},
                            }
                        },
                        Goal= new NPuzzle
                        {
                            N = 4,
                            Board = new int[,] {
                                {1,2,3,4},
                                {5,6,7,8},
                                {9,10,11,12},
                                {13,14,15,0},
                            }
                        }
                    },
                     //Experiment 5
                    new Experiment {
                        IntialState = new NPuzzle
                        {
                            N = 4,
                            Board = new int[,] {
                                {2,7,3,4},
                                {5,1,10,8},
                                {9,13,6,12},
                                {0,14,15,11},
                            }
                        },
                        Goal= new NPuzzle
                        {
                            N = 4,
                            Board = new int[,] {
                                {1,2,3,4},
                                {5,6,7,8},
                                {9,10,11,12},
                                {13,14,15,0},
                            }
                        }
                    },
            };
            }

            while (true)
            {
                int chooseAlgorithm = 0;

                while (chooseAlgorithm != 1 && chooseAlgorithm != 2 && chooseAlgorithm != 3)
                {
                    Console.WriteLine("Choose Algorithm");
                    Console.WriteLine("1) Best First Search");
                    Console.WriteLine("2) A*");
                    Console.WriteLine("3) Exit");
                    Console.Write("Press 1, 2 or 3? ");
                    try
                    {
                        chooseAlgorithm = Convert.ToInt32(Console.ReadLine());
                    }
                    catch
                    {
                        chooseAlgorithm = 0;
                    }
                }

                if (chooseAlgorithm == 3)
                {
                    Console.WriteLine("Exited!");
                    return;
                }

                int chooseHeuristic = 0;

                while (chooseHeuristic != 1 && chooseHeuristic != 2 && chooseHeuristic != 3)
                {
                    Console.WriteLine("Choose Heuristic");
                    Console.WriteLine("1) Tiles out-of-place.");
                    Console.WriteLine("2) Manhattan distance");

                    Console.WriteLine("3) My heuristic");
                    Console.WriteLine("4) Exit");
                    Console.Write("Press 1, 2, 3 or 4? ");

                    try
                    {
                        chooseHeuristic = Convert.ToInt32(Console.ReadLine());
                    }
                    catch
                    {
                        chooseHeuristic = 0;
                    }
                }

                ISearch search = null;

                switch (chooseAlgorithm)
                {
                    case 1:
                        search = new BestFirstSearch();
                        Console.WriteLine("Algorithm Best First Search");
                        break;

                    case 2:
                        search = new AStart();
                        Console.WriteLine("Algorithm A*");
                        break;
                }

                switch (chooseHeuristic)
                {
                    case 1:
                        search.Heuristic = Heuristic1;
                        Console.WriteLine("Heuristic: Tiles out-of-place");
                        break;

                    case 2:
                        search.Heuristic = Heuristic2;
                        Console.WriteLine("Heuristic: Manhattan distance");
                        break;

                    case 3:
                        search.Heuristic = Heuristic3;
                        Console.WriteLine("Heuristic: My heuristic");
                        break;

                    case 4:
                        Console.WriteLine("Exited!");
                        return;
                }


                int choosePrint = 0;

                while (choosePrint != 1 && choosePrint != 2 && choosePrint != 3)
                {
                    Console.WriteLine("Choose Print Type");
                    Console.WriteLine("1) Short Type");
                    Console.WriteLine("2) Pretty Type");
                    Console.WriteLine("3) Exit");
                    Console.Write("Press 1, 2 or 3? ");
                    try
                    {
                        choosePrint = Convert.ToInt32(Console.ReadLine());
                    }
                    catch
                    {
                        choosePrint = 0;
                    }
                }

                if (choosePrint == 3)
                {
                    Console.WriteLine("Exited!");
                    return;
                }


                int totalStepInPath = 0;
                int totalRun = 0;
                int totalExperiment = 0;
                foreach (var e in experiments)
                {
                    totalExperiment++;
                    Console.WriteLine($"@Experiment #{totalExperiment}");

                    Console.WriteLine("Initial State:");
                    e.IntialState.Print(choosePrint);
                    Console.WriteLine("");

                    Console.WriteLine("Goal State:");
                    e.Goal.Print(choosePrint);
                    Console.WriteLine("");

                    search.InitState = e.IntialState;
                    search.Goal = e.Goal;

                    Console.WriteLine("Searching...");
                    DateTime dt = DateTime.Now;
                    int moves = search.Run();

                    TimeSpan ts = DateTime.Now - dt;
                    String totalMiliseconds = ts.TotalMilliseconds.ToString();

                    Console.WriteLine("Result:");

                    if (search.Path.Count > 0)
                    {
                        search.PrintPath(choosePrint);
                        Console.WriteLine($"Total number of steps in path: {search.Path.Count}");
                        totalRun++;
                    }
                    else
                    {
                        Console.WriteLine("Cannot find a solution!");
                    }

                    Console.WriteLine($"Total visited nodes : {moves}");

                    Console.WriteLine($"Total Miliseconds: {totalMiliseconds}");
                    Console.WriteLine("");
                    totalStepInPath += search.Path.Count;
                }

                Console.WriteLine("");
                if (totalRun != 0)
                {
                    Console.WriteLine($"Average number of steps: {totalStepInPath / totalRun}");
                    Console.WriteLine("");
                }
            }
        }
    }
}
