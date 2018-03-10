using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    class Program
    {
        static int Main()
        {
            Game game = new Game(Difficulty.Easy);
            Random random = new Random();
            do
            {
                Console.Clear();
                //game.MakeMove(random.Next(0, game.GetHeight() - 1), random.Next(0, game.GetWidth() - 1), MoveType.Exposure);
                game.MakeMove(0, 0, MoveType.Flag);
                DisplayBoard(game);
                Console.ReadKey();
            } while (game.GameData.isRunning);

            if (game.GameData.win)
                Console.WriteLine("Wygrana!");
            else if (game.GameData.lose)
                Console.WriteLine("Przegrana...");

            return 0;
        }

        static void DisplayBoard(Game game)
        {
            for (int x = 0; x < game.GetWidth(); x++)
            {
                for (int y = 0; y < game.GetHeight(); y++)
                {
                    DisplayBoardElem(game.GameData.board[x][y]);
                }
                Console.WriteLine();
            }
        }

        static void DisplayBoardElem(CellType cell)
        {
            switch (cell)
            {
                case CellType.Flag:
                    Console.Write("F ");
                    break;
                case CellType.Bomb:
                    Console.Write("B ");
                    break;
                case CellType.Zero:
                    Console.Write("0 ");
                    break;
                case CellType.One:
                    Console.Write("1 ");
                    break;
                case CellType.Two:
                    Console.Write("2 ");
                    break;
                case CellType.Three:
                    Console.Write("3 ");
                    break;
                case CellType.Four:
                    Console.Write("4 ");
                    break;
                case CellType.Five:
                    Console.Write("5 ");
                    break;
                case CellType.Six:
                    Console.Write("6 ");
                    break;
                case CellType.Seven:
                    Console.Write("7 ");
                    break;
                case CellType.Eight:
                    Console.Write("8 ");
                    break;
                case CellType.NotVisible:
                    Console.Write("? ");
                    break;
            }
        }
    }
}
