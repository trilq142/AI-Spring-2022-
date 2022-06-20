//Tri Le
//CS 441
//Program #2

using System;

namespace Program2
{
    public class EightQueenProblem : GeneticAlgorithm<int>
    {
        /// <summary>
        /// Calculate the fitness score
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public override int Fitness(GAData<int> individual)
        {
            int maxPoint = GAData<int>.MAX_FITNESS_VAL;
            
            for (int i = 0; i < individual.State.Length - 1 ; i++)
            {
                for (int j = i + 1; j < individual.State.Length; j++) {
                    if (individual.State[i] == individual.State[j]) {
                        maxPoint--;
                    }
                }
            }

            for (int i = 0; i < individual.State.Length - 1; i++)
            {
                for (int j =i+1; j < individual.State.Length && individual.State[i] + (j - i) < individual.State.Length; j++)
                {
                    if (individual.State[i] + (j - i) == individual.State[j])
                    {
                        maxPoint--;
                    }
                }
            }

            for (int i = 0; i < individual.State.Length - 1; i++)
            {
                for (int j = i + 1; j < individual.State.Length && individual.State[i] - (j - i) < individual.State.Length; j++)
                {
                    if (individual.State[i] - (j - i) == individual.State[j])
                    {
                        maxPoint--;
                    }
                }
            }


            individual.FitnessPoint = maxPoint;

            return maxPoint;
        }

        /// <summary>
        /// Generate the initial population
        /// </summary>
        /// <param name="size"></param>
        public override void InitPopulation(int size)
        {
            for (int num = 0; num < size; num++) {
                GAData<int> newChild = null;
                newChild = new GAData<int>
                {
                    State = new int[GAData<int>.MAX_LENGTH],
                    FitnessPoint = 0
                };

                for (var i = 0; i < newChild.State.Length; i++)
                {
                    newChild.State[i] = this.random.Next(0, GAData<int>.MAX_LENGTH);
                }

                int currentFitness = Fitness(newChild);

                if (currentFitness >= 28) {
                    num--;
                    continue;
                }

                if (AddToPopulation(newChild) )
                {
                    totalFitness += newChild.FitnessPoint;
                    totalIndividual++;
                }
                else {
                    num--;
                }
            }
        }

        /// <summary>
        /// Create a mutation
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public override GAData<int> Mutate(GAData<int> child)
        {
            int c = this.random.Next(0, GAData<int>.MAX_LENGTH);
            int i = this.random.Next(0, GAData<int>.MAX_LENGTH);
            child.State[i] = c;

            return child;
        }

        /// <summary>
        /// Print an individual
        /// </summary>
        /// <param name="data"></param>
        public override void Print(GAData<int> data)
        {
            Console.Write($"   ");
            for (int i = 0; i < data.State.Length; i++)
            {
                Console.Write($"{i}\t");
            }
            Console.Write($"\n");

            for (int i = 0; i < data.State.Length; i++)
            {
                Console.Write($"{i} |");
                for (int j = 0; j < data.State[i]; j++)
                {
                    Console.Write("\t");
                }
                Console.Write("Q\n");
            }
        }
    }
}
