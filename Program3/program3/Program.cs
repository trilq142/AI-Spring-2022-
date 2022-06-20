using System;
using System.IO;

namespace program3
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "result.csv";

            GameRun tictactoeGame = new GameRun();
            if (File.Exists(fileName))
                File.Delete(fileName);

            AIPlayer aiPlayer1 = new AIPlayer(1);
            AIPlayer aiPlayer2 = new AIPlayer(-1);

            tictactoeGame.SetAI(aiPlayer1, aiPlayer2);

            //Training
            tictactoeGame.IntialBeforeTraining(false);

            for (int epoch = 0; epoch < 500; epoch++)
            {
                //Training
                Console.WriteLine($"Epoch = {epoch}");
                tictactoeGame.TraniningAI(100);

                //Play with random AI
                tictactoeGame.AIPlayerAndRandomPlayerCompetetion(epoch, 10, fileName, false);
            }

            //Save 
            tictactoeGame.SaveAIPolicy();
            //   tictactoeGame.LoadAIPolicy();
            //Play with human
            tictactoeGame.AIPlayerAndHumanCompetetion();
        }
    }
}
