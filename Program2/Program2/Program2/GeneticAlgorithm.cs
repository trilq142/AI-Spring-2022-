//Tri Le
//CS 441
//Program #2
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Program2
{
    /// <summary>
    /// This the generic class for the algorithm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GeneticAlgorithm<T>
    {
        public GeneticAlgorithm() {
        }

        public const int NumIterations = 1_000_000;
        protected Random random = new Random();

        public double MutationRate { get; set; } = 0.05;
        protected Dictionary<int, List<GAData<T>>> population = new Dictionary<int, List<GAData<T>>>();
        protected Dictionary<string, GAData<T>> distinctPopution = new Dictionary<string, GAData<T>>();

        protected int totalFitness = 0;
        protected int totalIndividual = 0;
        protected int totalGenerationNumber = 0;
        protected List<int> fitnessVals = new List<int>();

        /// <summary>
        /// Generate individuals at the beginning
        /// </summary>
        /// <param name="size"></param>
        public abstract void InitPopulation(int size);

        /// <summary>
        /// Calculate the fitness score
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public abstract int Fitness(GAData<T> individual);

        /// <summary>
        /// Print an individual
        /// </summary>
        /// <param name="data"></param>
        public abstract void Print(GAData<T> data);

        /// <summary>
        /// Select parents by normalization rand method
        /// </summary>
        /// <returns></returns>
        (GAData<T>, GAData<T>) RandomSelectionNormalize() {
            fitnessVals.Sort();
            GAData<T> x = null;
            GAData<T> y = null;
            double lottery = random.NextDouble() * 100;
            double probBegin = 0;
            double probEnd = 0;
            for (int i = 0; i <fitnessVals.Count; i++)
            {
                probEnd += ((double)(fitnessVals[i] * population[fitnessVals[i]].Count)) * 100 / ((double)totalFitness);

                if (lottery >= probBegin && lottery < probEnd)
                {
                    int c = random.Next(0, population[fitnessVals[i]].Count);
                    x = population[fitnessVals[i]][c];
                    break;
                }
                probBegin = probEnd;
            }


            while (y == null)
            {
                lottery = random.NextDouble() * 100;
                probBegin = 0;
                probEnd = 0;

                for (int i = 0; i < fitnessVals.Count ; i++)
                {
                    probEnd += ((double)(fitnessVals[i] * population[fitnessVals[i]].Count)) * 100 / ((double)totalFitness);
                    
                    if (lottery >= probBegin && lottery < probEnd)
                    {
                        int c = random.Next(0, population[fitnessVals[i]].Count);

                        if (population[fitnessVals[i]][c].GetHashCode() != x.GetHashCode()) {
                            y = population[fitnessVals[i]][c];
                        }
                        break;
                    }
                    probBegin = probEnd;
                }
            }

            return (x, y);
        }

         /// <summary>
        /// Repoduct a new child by crossover method
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        GAData<T> Reproduce(GAData<T> x, GAData<T> y) {
            int c = random.Next(0, y.State.Length);
            return new GAData<T>
            {
                State = GAData<T>.Join(GAData<T>.SubArray(x.State, 0, c), GAData<T>.SubArray(y.State, c + 1, y.State.Length - 1)),
                FitnessPoint = 0
            };
        }

        /// <summary>
        /// Mutate the current individual
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public abstract GAData<T> Mutate(GAData<T> child);

        /// <summary>
        /// Make the probability happens or not
        /// </summary>
        /// <param name="probability"></param>
        /// <returns></returns>
        public bool RunProbability(double probability) {
            var rVal = random.NextDouble();
            return rVal <= probability;
        }

       /// <summary>
       /// Add individual to the population
       /// </summary>
       /// <param name="individual"></param>
       /// <returns></returns>
        public bool AddToPopulation(GAData<T> individual) {

            if (distinctPopution.TryAdd(individual.GetHashCode(), individual))
            {
                if (this.population.ContainsKey(individual.FitnessPoint) == false)
                {
                    population.Add(individual.FitnessPoint, new List<GAData<T>>());
                    fitnessVals.Add(individual.FitnessPoint);
                }

                population[individual.FitnessPoint].Add(individual);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove one lowest individual from the population
        /// </summary>
        /// <returns></returns>
        public bool RemoveLowestFitnessFromPopulation()
        {
            fitnessVals.Sort();
            for (int i = 0; i < fitnessVals.Count; i++) {
                if (population[fitnessVals[i]].Count > 0)
                {
                    int delItem = random.Next(0, population[fitnessVals[i]].Count);
                    totalFitness -= population[fitnessVals[i]][delItem].FitnessPoint;
                    population[fitnessVals[i]].RemoveAt(delItem);

                    totalIndividual--;

                    if (population[fitnessVals[i]].Count == 0) {
                        population.Remove(fitnessVals[i]);
                        fitnessVals.RemoveAt(i);
                    }
                    return true;
                }
               
            }

            return false;
        }

        /// <summary>
        /// Calculate the average fitness
        /// </summary>
        /// <returns></returns>
        public double AverageFitness() { 
            return  ((double) totalFitness)/((double) totalIndividual);
        }

        /// <summary>
        /// Get the best fitness score
        /// </summary>
        /// <returns></returns>
        public int GetBestFitnessScore()
        {
            fitnessVals.Sort();
            return fitnessVals[fitnessVals.Count - 1];
        }

        /// <summary>
        /// Get one individual that has the best fitness score
        /// </summary>
        /// <returns></returns>
        public GAData<T> GetOneBestFitnessIndividual()
        {
            return population[GetBestFitnessScore()][0];
        }

        /// <summary>
        /// Get the lowest fitness score
        /// </summary>
        /// <returns></returns>
        public int GetLowestFitnessScore()
        {
            fitnessVals.Sort();
            return fitnessVals[0];
        }

        /// <summary>
        /// Run the algorithm
        /// </summary>
        /// <param name="size"></param>
        /// <param name="resultFile"></param>
        /// <param name="typeOfSelection"></param>
        public void Run(int size, string resultFile) {
            totalFitness = 0;
            InitPopulation(size);

            Console.WriteLine("* At the beginin - One highest fitness individual");
            GAData<T> highest = GetOneBestFitnessIndividual();
            Print(highest);


            totalGenerationNumber = 0;
            int currentFitness = 0;

            StringBuilder csv = null;
            string newLine = null;

            // Insert result to file csv
            if (resultFile != null)
            {
                csv = new StringBuilder();
                newLine = string.Format($"{"Population generation number"},{"The average fitness"}");
                csv.AppendLine(newLine);
            }

            do {
                var (x, y) = RandomSelectionNormalize();

                var newChild = Reproduce(x, y);

                if (RunProbability(MutationRate))
                    newChild = Mutate(newChild);

                currentFitness = Fitness(newChild);

                totalGenerationNumber++;

                if (currentFitness > GetLowestFitnessScore())
                {
                    if (AddToPopulation(newChild))
                    {
                        totalFitness += newChild.FitnessPoint;
                        totalIndividual++;

                        //Remove one lowest individual from the population
                        if (totalIndividual > size)
                        {
                            RemoveLowestFitnessFromPopulation();
                        }
                    }
                }

                // Insert result to file csv
                if (resultFile != null)
                {
                    newLine = string.Format($"{totalGenerationNumber},{AverageFitness()}");
                    csv.AppendLine(newLine);
                }

                if (totalGenerationNumber % (20000) == 0)
                {
                    Console.WriteLine($"* At the iteration {totalGenerationNumber} - One highest fitness individual");
                    highest = GetOneBestFitnessIndividual();
                    Print(highest);
                }

            } while (totalGenerationNumber < NumIterations && population.ContainsKey(GAData<T>.MAX_FITNESS_VAL) == false);

            if (population.ContainsKey(GAData<T>.MAX_FITNESS_VAL)) {
                Console.WriteLine($"* At the end - One highest fitness individual");
                highest = GetOneBestFitnessIndividual();
                Print(highest);
            }

            //Write the result to file
            if (resultFile != null)
                File.WriteAllText(resultFile, csv.ToString());

        }
    }
}
