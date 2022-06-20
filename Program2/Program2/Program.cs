//Tri Le
//CS 441
//Program #2

using System;

namespace Program2
{
    class Program
    {
        /// <summary>
        /// Run experiment
        /// </summary>
        /// <param name="resultFile"></param>
        /// <param name="initPopulation"></param>
        /// <param name="typeOfSelection"></param>
        static void experiment(string resultFile, int initPopulation)
        {
            Console.WriteLine($"### EXPERIMENT WITH POPULATION -{initPopulation}- ###");

            EightQueenProblem eightQueenProblem = new EightQueenProblem();
            eightQueenProblem.Run(initPopulation, resultFile);

            Console.WriteLine($"###           END              ###");
        }
        static void Main(string[] args)
        {
            experiment("experiment10-type1.csv", 10);
            experiment("experiment100-type1.csv", 100);
            experiment("experiment500-type1.csv", 500);
            experiment("experiment1000-type1.csv", 1000);

        }
    }
}
