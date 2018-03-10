using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    enum Difficulty { Easy, Medium, Hard, Nonstandard }
    enum CellType { Bomb = -1, Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Flag, NotVisible, Undefined }
    public enum MoveType { Expose, Flag, ExposeAdjacent, Undefined }

    struct Cell
    {
        public int x;
        public int y;
        public bool isBomb;
        public bool isVisible;
        public bool isFlag;
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

        public void MakeMove(int Y, int X, MoveType type)
        {
            switch (type)
            {
                case MoveType.Expose:
                    if (firstMove)
                    {
                        SetupBombs(X, Y);
                        SetupAdjacentBombCounts();
                        firstMove = false;
                    }
                    if (!board[X][Y].isFlag)
                        RevealCell(X, Y);
                    if (board[X][Y].isBomb)
                        Lose();
                    break;
                case MoveType.ExposeAdjacent:
                    if (firstMove)
                    {
                        SetupBombs(X, Y);
                        SetupAdjacentBombCounts();
                        firstMove = false;
                    }
                    List<Cell> cells = GetAdjacentCells(X, Y);
                    if (GetAdjacentFlagCount(cells) == board[X][Y].adjacentBombCount)
                    {
                        foreach (Cell cell in cells)
                            RevealCell(cell.x, cell.y);
                    }
                    break;
                case MoveType.Flag:
                    ToggleFlag(X, Y);
                    break;
                default:
                    break;
            }
            if (CheckWinConditions() && !GameData.lose)
                Win();

            SetCellTypeBoard();
        }

        public Game(Difficulty diff, int Width = 8, int Height = 8, int BombCount = 10)
        {
            SetDifficulty(diff, Width, Height, BombCount);
            InitializeBoard();
            
        }

        private void Lose()
        {
            GameData.lose = true;
            GameData.isRunning = false;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                    RevealCell(x, y);
            }
        }

        private void Win()
        {
            GameData.win = true;
            GameData.isRunning = false;
        }

        private void ToggleFlag(int X, int Y)
        {
            if (!board[X][Y].isVisible)
            { 
                if (board[X][Y].isFlag)
                {
                    board[X][Y].isFlag = false;
                }
                else if (!board[X][Y].isFlag)
                {
                    board[X][Y].isFlag = true;
                }
            }
        }

        private bool CheckWinConditions()
        {
            int flagsOnBombsCount = 0;
            int unrevealedBombs = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!board[x][y].isBomb && !board[x][y].isVisible)
                        return false;

                    if (board[x][y].isBomb && board[x][y].isFlag)
                        flagsOnBombsCount++;

                    if (board[x][y].isBomb && !board[x][y].isVisible && !board[x][y].isFlag)
                        unrevealedBombs++;
                }
            }
            if ((flagsOnBombsCount + unrevealedBombs )== bombCount)
                return true;
            else
                return false;
        }

        private void RevealCell(int X, int Y)
        {
            List<Cell> cells = GetAdjacentCells(X, Y);

            if (!board[X][Y].isVisible)
            {
                board[X][Y].isVisible = true;
                if (board[X][Y].adjacentBombCount == 0)
                {
                    foreach (Cell cell in cells)
                        RevealCell(cell.x, cell.y);
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
            if (cell.isFlag)
                return CellType.Flag;

            if (!cell.isVisible)
                return CellType.NotVisible;

            switch (cell.adjacentBombCount)
            {
                case -1:
                    return CellType.Bomb;
                case 0:
                    return CellType.Zero;
                case 1:
                    return CellType.One;
                case 2:
                    return CellType.Two;
                case 3:
                    return CellType.Three;
                case 4:
                    return CellType.Four;
                case 5:
                    return CellType.Five;
                case 6:
                    return CellType.Six;
                case 7:
                    return CellType.Seven;
                case 8:
                    return CellType.Eight;
                default:
                    return CellType.Undefined;
            }
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
            if (cell.x < width - 1 && cell.y > 0)
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
                for (int y = 0; y < height; y++)
                    board[x][y].adjacentBombCount = CalculateAdjacentBombCount(board[x][y]);
            }
        }

        private void SetupBombs(int X, int Y)
        {
            bool canPlace = false;
            List<Cell> cells = GetAdjacentCells(X, Y);
            cells.Add(board[X][Y]);
            Random random = new Random();
            int randomX;
            int randomY;
            int i = 0;

            while (i < bombCount)
            {
                randomX = random.Next(0, width);
                randomY = random.Next(0, height);

                foreach (Cell cell in cells)
                {
                    if (cell.x == randomX && cell.y == randomY)
                    {
                        canPlace = false;
                        break;
                    }
                    canPlace = true;
                }

                if (!board[randomX][randomY].isBomb && canPlace)
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
                    board[x][y].isFlag = false;
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
                case Difficulty.Easy:
                    width = 8;
                    height = 8;
                    bombCount = 10;
                    break;
                case Difficulty.Medium:
                    width = 16;
                    height = 16;
                    bombCount = 40;
                    break;
                case Difficulty.Hard:
                    width = 16;
                    height = 30;
                    bombCount = 99;
                    break;
                case Difficulty.Nonstandard:
                    width = Width;
                    height = Height;
                    bombCount = BombCount;
                    break;
            }
            firstMove = true;
        }

        private List<Cell> GetAdjacentCells(int X, int Y)
        {
            List<Cell> cells = new List<Cell>();
            if (X > 0 && Y > 0)
                cells.Add(board[X - 1][Y - 1]);
            if (Y > 0)
                cells.Add(board[X][Y - 1]);
            if (X < width - 1 && Y > 0)
                cells.Add(board[X + 1][Y - 1]);
            if (X > 0)
                cells.Add(board[X - 1][Y]);
            if (X < width - 1)
                cells.Add(board[X + 1][Y]);
            if (X > 0 && Y < height - 1)
                cells.Add(board[X - 1][Y + 1]);
            if (Y < height - 1)
                cells.Add(board[X][Y + 1]);
            if (X < width - 1 && Y < height - 1)
                cells.Add(board[X + 1][Y + 1]);

            return cells;
        }

        private int GetAdjacentFlagCount(List<Cell> cells)
        {
            int flagCount = 0;

            foreach (Cell cell in cells)
            {
                if (cell.isFlag)
                    flagCount++;
            }
            return flagCount;
        }

        private int width;
        private int height;
        private int bombCount;
        private bool firstMove;
        private Cell[][] board;
    }
}