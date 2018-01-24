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
            Game game = new Game(Difficulty.nonstandard, 300, 300, 900);
            CellType[][] board = game.GetBoard();

            for (int x = 0; x < game.GetWidth(); x++)
            {
                for (int y = 0; y < game.GetHeight(); y++)
                {
                    DisplayBoardElem(board[x][y]);
                }
                Console.WriteLine();
            }

            return 0;
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
            }
        }
    }
}
