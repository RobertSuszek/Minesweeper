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
            Game game = new Game(Difficulty.hard);
            Random random = new Random();
            do
            {
                Console.Clear();
                game.MakeMove(random.Next(0, game.GetHeight() - 1), random.Next(0, game.GetWidth() - 1));
                DisplayBoard(game);
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
                case CellType.bomb:
                    Console.Write("B ");
                    break;
                case CellType.zero:
                    Console.Write("0 ");
                    break;
                case CellType.one:
                    Console.Write("1 ");
                    break;
                case CellType.two:
                    Console.Write("2 ");
                    break;
                case CellType.three:
                    Console.Write("3 ");
                    break;
                case CellType.four:
                    Console.Write("4 ");
                    break;
                case CellType.five:
                    Console.Write("5 ");
                    break;
                case CellType.six:
                    Console.Write("6 ");
                    break;
                case CellType.seven:
                    Console.Write("7 ");
                    break;
                case CellType.eight:
                    Console.Write("8 ");
                    break;
                case CellType.notVisible:
                    Console.Write("? ");
                    break;
            }
        }
    }
}
