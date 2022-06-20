///Tri Le
///CS441 - Artificial Inteligence
///Program 3 - Option Tictactoe

namespace program3
{
    public class TicTacToeAction
    {
        int hash = int.MinValue;

        /// <summary>
        /// Constructor
        /// </summary>
        public TicTacToeAction() { }

        /// <summary>
        /// Constructor with coordinator
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public TicTacToeAction(int x, int y) {

            X = x;
            Y = y;
            hash = X * TictactoeState.MAX_LENGTH + Y + 1;
        }

        public int X { get ; set; }
        public int Y { get; set; }

        /// <summary>
        /// Return the hash value
        /// </summary>
        /// <returns></returns>
        public int Hash() {
            if (hash != int.MinValue)
                return hash;
            hash = X * TictactoeState.MAX_LENGTH + Y + 1;
            return hash;
        }
    }
}
