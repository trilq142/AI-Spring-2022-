///Tri Le
///CS441 - Artificial Inteligence
///Program 3 - Option Tictactoe
using System;

namespace program3
{
    class Helpers
    {
        private static Random random = new Random();
        
        /// <summary>
        /// Generate double random value  by range
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static double GetRandomDouble(double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
