//Tri Le
//CS 441
//Program #2
using System;
namespace Program2
{
    public class GAData<T>
    {
        public GAData()
        {
        }
        private string hash = "";

        public const int MAX_LENGTH = 8;
        public const int MAX_FITNESS_VAL = 28;

        public T[] State { get; set; }
        public int FitnessPoint { get; set; }

        /// <summary>
        /// Get sub array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static T[] SubArray(T[] array, int begin, int end)
        {
            T[] result = new T[end-begin+1];
            Array.Copy(array, begin, result, 0, end - begin + 1);
            return result;
        }

        /// <summary>
        /// Join two arrays
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static T[] Join(T[] begin, T[] end) {
            T[] result = new T[begin.Length + end.Length];

            Array.Copy(begin, 0, result, 0, begin.Length);
            Array.Copy(end, 0, result, begin.Length, end.Length);

            return result;
        }

        /// <summary>
        /// Generate the hash string for every state
        /// </summary>
        /// <returns></returns>
        public string GetHashCode()
        {
            if(hash ==""){
                for (int i = 0; i < State.Length; i++)
                    hash += State[i].ToString();
            }
            
            return hash;
        }

    }
}
