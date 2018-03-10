using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    enum Difficulty { easy, medium, hard, nonstandard };
    enum CellType { bomb = -1, zero, one, two, three, four, five, six, seven, eight, notVisible, undefined }

    struct Cell
    {
        public int x;
        public int y;
        public bool isBomb;
        public bool isVisible;
        public int adjacentBombCount;
    };

    struct GameData
    {
        public bool win;
        public bool lose;
        public bool isRunning;
        public CellType[][] board;
    }

    class Game
    {
        public GameData GameData;
        public int GetWidth() { return width; }
        public int GetHeight() { return height; }

        public void MakeMove(int Y, int X)
        {
            HandleMove(X, Y);

            SetCellTypeBoard();

            if (board[X][Y].isBomb)
            {
                GameData.lose = true;
                GameData.isRunning = false;
            }
            if (CheckWinConditions() && !GameData.lose)
            {
                GameData.win = true;
                GameData.isRunning = false;
            }
        }

        public Game(Difficulty diff, int Width = 8, int Height = 8, int BombCount = 10)
        {
            SetDifficulty(diff, Width, Height, BombCount);
            InitializeBoard();
            SetupBombs();
            SetupAdjacentBombCounts();
        }

        private bool CheckWinConditions()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!board[x][y].isVisible && !board[x][y].isBomb)
                        return false;
                }
            }
            return true;
        }

        private void HandleMove(int X, int Y)
        {
            if (!board[X][Y].isVisible)
            {
                board[X][Y].isVisible = true;
                if (board[X][Y].adjacentBombCount == 0)
                {
                    if (X > 0 && Y > 0)
                        HandleMove(X - 1, Y - 1);
                    if (Y > 0)
                        HandleMove(X, Y - 1);
                    if (X < width - 1 && Y > 0)
                        HandleMove(X + 1, Y - 1);
                    if (X > 0)
                        HandleMove(X - 1, Y);
                    if (X < width - 1)
                        HandleMove(X + 1, Y);
                    if (X > 0 && Y < height - 1)
                        HandleMove(X - 1, Y + 1);
                    if (Y < height - 1)
                        HandleMove(X, Y + 1);
                    if (X < width - 1 && Y < height - 1)
                        HandleMove(X + 1, Y + 1);
                } 
            }
        }

        private void SetCellTypeBoard()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                    GameData.board[x][y] = GetCellType(board[x][y]);
            }
        }

        private CellType GetCellType(Cell cell)
        {
            CellType celltype = CellType.undefined;

            if (!cell.isVisible)
                return CellType.notVisible;

            switch (cell.adjacentBombCount)
            {
                case -1:
                    celltype = CellType.bomb;
                    break;
                case 0:
                    celltype = CellType.zero;
                    break;
                case 1:
                    celltype = CellType.one;
                    break;
                case 2:
                    celltype = CellType.two;
                    break;
                case 3:
                    celltype = CellType.three;
                    break;
                case 4:
                    celltype = CellType.four;
                    break;
                case 5:
                    celltype = CellType.five;
                    break;
                case 6:
                    celltype = CellType.six;
                    break;
                case 7:
                    celltype = CellType.seven;
                    break;
                case 8:
                    celltype = CellType.eight;
                    break;
            }
            return celltype;
        }

        private int CalculateAdjacentBombCount(Cell cell)
        {
            int bombCount = 0;

            if (cell.isBomb)
                return -1;

            if (cell.x > 0 && cell.y > 0)
            {
                if (board[cell.x - 1][cell.y - 1].isBomb)
                    bombCount++;
            }
            if (cell.y > 0)
            {
                if (board[cell.x][cell.y - 1].isBomb)
                    bombCount++;
            }
            if (cell.x < width-1 && cell.y > 0)
            {
                if (board[cell.x + 1][cell.y - 1].isBomb)
                    bombCount++;
            }
            if (cell.x > 0)
            {
                if (board[cell.x - 1][cell.y].isBomb)
                    bombCount++;
            }
            if (cell.x < width - 1)
            {
                if (board[cell.x + 1][cell.y].isBomb)
                    bombCount++;
            }
            if (cell.x > 0 && cell.y < height - 1)
            {
                if (board[cell.x - 1][cell.y + 1].isBomb)
                    bombCount++;
            }
            if (cell.y < height - 1)
            {
                if (board[cell.x][cell.y + 1].isBomb)
                    bombCount++;
            }
            if (cell.x < width - 1 && cell.y < height - 1)
            {
                if (board[cell.x + 1][cell.y + 1].isBomb)
                    bombCount++;
            }


            return bombCount;
        }

        private void SetupAdjacentBombCounts()
        {
            for (int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                    board[x][y].adjacentBombCount = CalculateAdjacentBombCount(board[x][y]);
            }
        }

        private void SetupBombs()
        {
            Random random = new Random();
            int randomX;
            int randomY;
            int i = 0;

            while (i < bombCount)
            {
                randomX = random.Next(0, width);
                randomY = random.Next(0, height);

                if (!board[randomX][randomY].isBomb)
                {
                    board[randomX][randomY].isBomb = true;
                    board[randomX][randomY].adjacentBombCount = -1;
                    i++;
                }
            }

        }

        private void InitializeBoard()
        {
            board = new Cell[width][];
            for (int x = 0; x < width; x++)
            {
                board[x] = new Cell[height];
                for (int y = 0; y < height; y++)
                {
                    board[x][y].x = x;
                    board[x][y].y = y;
                    board[x][y].isBomb = false;
                    board[x][y].isVisible = false;
                    board[x][y].adjacentBombCount = 0;
                }
            }
            GameData.lose = false;
            GameData.win = false;
            GameData.isRunning = true;
            GameData.board = new CellType[width][];
            for (int i = 0; i < width; i++)
                GameData.board[i] = new CellType[height];

            SetCellTypeBoard();
        }

        private void SetDifficulty(Difficulty diff, int Width, int Height, int BombCount)
        {
            switch (diff)
            {
                case Difficulty.easy:
                    width = 8;
                    height = 8;
                    bombCount = 10;
                    break;
                case Difficulty.medium:
                    width = 16;
                    height = 16;
                    bombCount = 40;
                    break;
                case Difficulty.hard:
                    width = 16;
                    height = 30;
                    bombCount = 99;
                    break;
                case Difficulty.nonstandard:
                    width = Width;
                    height = Height;
                    bombCount = BombCount;
                    break;
            }
        }

        private int width;
        private int height;
        private int bombCount;
        private Cell[][] board;
    }
}