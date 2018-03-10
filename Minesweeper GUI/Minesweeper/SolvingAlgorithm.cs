using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Minesweeper.Solver;

namespace Minesweeper
{
    public struct Move { public int X; public int Y; public MoveType Type; }


    class Solver
    {
        enum SquareType { Bomb = -3, Flag, Hidden, Zero, One, Two, Three, Four, Five, Six, Seven, Eight }

        struct Square
        {
            public SquareType type;
            public int x;
            public int y;
        }

        struct SquareInfo
        {
            public int adjacentFlagCount;
            public int adjacentHiddenCount;
        }

        public Move GetNextMove(CellType[][] inputBoard)
        {
            if (moveQueue.Count > 0)
                return moveQueue.Dequeue();

            InitializeBoard(inputBoard);

            AddMovesToQueue();

            return moveQueue.Dequeue();
        }

        //public Move GetNextMove(CellType[][] inputBoard)
        //{
        //    if (isFirstMove)
        //    {
        //        isFirstMove = false;
        //        isSecondMove = true;
        //        QueueRandomMove();
        //        lastMove = moveQueue.Dequeue();
        //        return lastMove;
        //    }
        //    else if (isSecondMove)
        //    {
        //        isSecondMove = false;
        //        InitializeBoard(inputBoard);
        //    }

        //    UpdateBoard(inputBoard);
        //    if (moveQueue.Count > 0)
        //    {
        //        lastMove = moveQueue.Dequeue();
        //        return lastMove;
        //    }
        //    else
        //    {
        //        AddMovesToQueue();
        //        lastMove = moveQueue.Dequeue();
        //        return lastMove;
        //    }
        //}

        public Solver(Game game)
        {
            moveQueue = new Queue<Move>();
            lastMove = new Move();
            isFirstMove = true;
            isSecondMove = false;
            width = game.GetWidth();
            height = game.GetHeight();
            board = new Square[width][];
            for (int i = 0; i < game.GetWidth(); i++)
                board[i] = new Square[height];

            InitializeBoard(game.GameData.board);
        }

        private void AddMovesToQueue()
        {
            Square analyzedSquare;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    analyzedSquare = board[x][y];
                    QueueFlagMoves(analyzedSquare);
                    QueueExposeMoves(analyzedSquare);
                }
            }
            if (moveQueue.Count == 0)
                QueueRandomMove();
        }

        private void QueueFlagMoves(Square analyzedSquare)
        {
            if ((int)analyzedSquare.type >= 1)
            {
                SquareInfo info = GetAdjacentSquaresInfo(analyzedSquare.x, analyzedSquare.y);
                if ((int)analyzedSquare.type == (info.adjacentHiddenCount + info.adjacentFlagCount))
                {
                    List<Square> adjacentHiddenSquares = GetAdjacentSquares(analyzedSquare.x, analyzedSquare.y, SquareType.Hidden);
                    foreach (Square sq in adjacentHiddenSquares)
                    {
                        if (!moveQueue.Contains(new Move { X = sq.x, Y = sq.y, Type = MoveType.Flag }))
                            moveQueue.Enqueue(new Move { X = sq.x, Y = sq.y, Type = MoveType.Flag });
                    }
                }

                if ((((int)analyzedSquare.type - info.adjacentFlagCount) == 2) && (info.adjacentHiddenCount == 3))
                {
                    List<Square> adjacentCrossSquares = GetAdjacentCrossSquares(analyzedSquare.x, analyzedSquare.y);
                    foreach (Square adjacentSquare in adjacentCrossSquares)
                    {
                        SquareInfo infoAboutAdjacent = GetAdjacentSquaresInfo(adjacentSquare.x, adjacentSquare.y);
                        if ((((int)adjacentSquare.type - infoAboutAdjacent.adjacentFlagCount) == 1) && (infoAboutAdjacent.adjacentHiddenCount == 3))
                        {
                            List<Square> hiddenAroundAnalyzed = GetAdjacentSquares(analyzedSquare.x, analyzedSquare.y, SquareType.Hidden);
                            List<Square> hiddenAroundAdjacent = GetAdjacentSquares(adjacentSquare.x, adjacentSquare.y, SquareType.Hidden);

                            foreach (Square sq in hiddenAroundAnalyzed)
                            {
                                if (!hiddenAroundAdjacent.Contains(sq))
                                {
                                    if (!moveQueue.Contains(new Move { X = sq.x, Y = sq.y, Type = MoveType.Expose }))
                                        moveQueue.Enqueue(new Move { X = sq.x, Y = sq.y, Type = MoveType.Flag });
                                }
                            }
                        }
                    }
                }
            }
        }

        private void QueueExposeMoves(Square analyzedSquare)
        {
            if ((int)analyzedSquare.type >= 1)
            {
                SquareInfo info = GetAdjacentSquaresInfo(analyzedSquare.x, analyzedSquare.y);
                if ((int)analyzedSquare.type == info.adjacentFlagCount)
                {
                    List<Square> adjacentHiddenSquares = GetAdjacentSquares(analyzedSquare.x, analyzedSquare.y, SquareType.Hidden);
                    foreach (Square sq in adjacentHiddenSquares)
                    {
                        if (!moveQueue.Contains(new Move { X = sq.x, Y = sq.y, Type = MoveType.Expose }))
                            moveQueue.Enqueue(new Move { X = sq.x, Y = sq.y, Type = MoveType.Expose });
                    }
                }

                if ((((int)analyzedSquare.type - info.adjacentFlagCount) == 1) && (info.adjacentHiddenCount == 2))
                {
                    List<Square> adjacentSquares = GetAdjacentSquares(analyzedSquare.x, analyzedSquare.y);
                    foreach (Square adjacentSquare in adjacentSquares)
                    {
                        SquareInfo infoAboutAdjacent = GetAdjacentSquaresInfo(adjacentSquare.x, adjacentSquare.y);
                        if ((((int)adjacentSquare.type - infoAboutAdjacent.adjacentFlagCount) == 1) && (infoAboutAdjacent.adjacentHiddenCount == 3))
                        {
                            List<Square> hiddenAroundAnalyzed = GetAdjacentSquares(analyzedSquare.x, analyzedSquare.y, SquareType.Hidden);
                            List<Square> hiddenAroundAdjacent = GetAdjacentSquares(adjacentSquare.x, adjacentSquare.y, SquareType.Hidden);
                            List<Square> result = new List<Square>();
                            foreach (Square sq in hiddenAroundAdjacent)
                            {
                                if (!hiddenAroundAnalyzed.Contains(sq))
                                    result.Add(sq);
                            }
                            if (result.Count == 1)
                            {
                                Square sq = result[0];
                                if (!moveQueue.Contains(new Move { X = sq.x, Y = sq.y, Type = MoveType.Expose }))
                                    moveQueue.Enqueue(new Move { X = sq.x, Y = sq.y, Type = MoveType.Expose });
                            }
                        }
                    }
                }

                if ((((int)analyzedSquare.type - info.adjacentFlagCount) == 2) && (info.adjacentHiddenCount == 3))
                {
                    List<Square> adjacentCrossSquares = GetAdjacentCrossSquares(analyzedSquare.x, analyzedSquare.y);
                    foreach (Square adjacentSquare in adjacentCrossSquares)
                    {
                        SquareInfo infoAboutAdjacent = GetAdjacentSquaresInfo(adjacentSquare.x, adjacentSquare.y);
                        if ((((int)adjacentSquare.type - infoAboutAdjacent.adjacentFlagCount) == 1) && (infoAboutAdjacent.adjacentHiddenCount == 3))
                        {
                            List<Square> hiddenAroundAnalyzed = GetAdjacentSquares(analyzedSquare.x, analyzedSquare.y, SquareType.Hidden);
                            List<Square> hiddenAroundAdjacent = GetAdjacentSquares(adjacentSquare.x, adjacentSquare.y, SquareType.Hidden);

                            foreach (Square sq in hiddenAroundAdjacent)
                            {
                                if (!hiddenAroundAnalyzed.Contains(sq))
                                {
                                    if (!moveQueue.Contains(new Move { X = sq.x, Y = sq.y, Type = MoveType.Expose }))
                                        moveQueue.Enqueue(new Move { X = sq.x, Y = sq.y, Type = MoveType.Expose });
                                }
                            }
                        }
                    }
                }
            }
        }

        private void QueueRandomMove()
        {
            Random random = new Random();
            List<Square> hiddenSquares = GetAllHiddenSquares();
            Square randomHiddenSquare = hiddenSquares[random.Next(0, hiddenSquares.Count)];
            moveQueue.Enqueue(new Move { X = randomHiddenSquare.x, Y = randomHiddenSquare.y, Type = MoveType.Expose });
        }

        private SquareInfo GetAdjacentSquaresInfo(int X, int Y)
        {
            SquareInfo info = new SquareInfo();
            List<Square> adjacentSquares = GetAdjacentSquares(X, Y);
            info.adjacentFlagCount = 0;
            info.adjacentHiddenCount = 0;

            foreach (Square sq in adjacentSquares)
            {
                if (sq.type == SquareType.Flag)
                    info.adjacentFlagCount++;

                else if (sq.type == SquareType.Hidden)
                    info.adjacentHiddenCount++;
            }

            return info;
        }

        private List<Square> GetAdjacentSquares(int X, int Y)
        {
            List<Square> squares = new List<Square>();
            if (X > 0 && Y > 0)
                squares.Add(board[X - 1][Y - 1]);
            if (Y > 0)
                squares.Add(board[X][Y - 1]);
            if (X < width - 1 && Y > 0)
                squares.Add(board[X + 1][Y - 1]);
            if (X > 0)
                squares.Add(board[X - 1][Y]);
            if (X < width - 1)
                squares.Add(board[X + 1][Y]);
            if (X > 0 && Y < height - 1)
                squares.Add(board[X - 1][Y + 1]);
            if (Y < height - 1)
                squares.Add(board[X][Y + 1]);
            if (X < width - 1 && Y < height - 1)
                squares.Add(board[X + 1][Y + 1]);

            return squares;
        }

        private List<Square> GetAdjacentSquares(int X, int Y, SquareType Type)
        {
            List<Square> squares = new List<Square>();
            if (X > 0 && Y > 0)
            {
                if (board[X - 1][Y - 1].type == Type)
                    squares.Add(board[X - 1][Y - 1]);
            }
            if (Y > 0)
            {
                if (board[X][Y - 1].type == Type)
                    squares.Add(board[X][Y - 1]);
            }
            if (X < width - 1 && Y > 0)
            {
                if (board[X + 1][Y - 1].type == Type)
                    squares.Add(board[X + 1][Y - 1]);
            }
            if (X > 0)
            {
                if (board[X - 1][Y].type == Type)
                    squares.Add(board[X - 1][Y]);
            }
            if (X < width - 1)
            {
                if (board[X + 1][Y].type == Type)
                    squares.Add(board[X + 1][Y]);
            }
            if (X > 0 && Y < height - 1)
            {
                if (board[X - 1][Y + 1].type == Type)
                    squares.Add(board[X - 1][Y + 1]);
            }
            if (Y < height - 1)
            {
                if (board[X][Y + 1].type == Type)
                    squares.Add(board[X][Y + 1]);
            }
            if (X < width - 1 && Y < height - 1)
            {
                if (board[X + 1][Y + 1].type == Type)
                    squares.Add(board[X + 1][Y + 1]);
            }

            return squares;
        }

        private List<Square> GetAdjacentCrossSquares(int X, int Y)
        {
            List<Square> squares = new List<Square>();
            if (Y > 0)
                squares.Add(board[X][Y - 1]);
            if (X > 0)
                squares.Add(board[X - 1][Y]);
            if (X < width - 1)
                squares.Add(board[X + 1][Y]);
            if (Y < height - 1)
                squares.Add(board[X][Y + 1]);

            return squares;
        }

        private List<Square> GetAdjacentCrossSquares(int X, int Y, SquareType Type)
        {
            List<Square> squares = new List<Square>();
            if (Y > 0)
            {
                if (board[X][Y - 1].type == Type)
                    squares.Add(board[X][Y - 1]);
            }
            if (X > 0)
            {
                if (board[X - 1][Y].type == Type)
                    squares.Add(board[X - 1][Y]);
            }
            if (X < width - 1)
            {
                if (board[X + 1][Y].type == Type)
                    squares.Add(board[X + 1][Y]);
            }
            if (Y < height - 1)
            {
                if (board[X][Y + 1].type == Type)
                    squares.Add(board[X][Y + 1]);
            }

            return squares;
        }

        private List<Square> GetAllHiddenSquares()
        {
            List<Square> hiddenSquares = new List<Square>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (board[x][y].type == SquareType.Hidden)
                        hiddenSquares.Add(board[x][y]);
                }
            }

            return hiddenSquares;
        }

        private void UpdateBoard(CellType[][] inputBoard)
        {
            int x = lastMove.X;
            int y = lastMove.Y;
            switch (inputBoard[x][y])
            {
                case CellType.NotVisible:
                    board[x][y].type = SquareType.Hidden;
                    break;
                case CellType.Bomb:
                    board[x][y].type = SquareType.Bomb;
                    break;
                case CellType.Flag:
                    board[x][y].type = SquareType.Flag;
                    break;
                case CellType.Zero:
                    board[x][y].type = SquareType.Zero;
                    break;
                case CellType.One:
                    board[x][y].type = SquareType.One;
                    break;
                case CellType.Two:
                    board[x][y].type = SquareType.Two;
                    break;
                case CellType.Three:
                    board[x][y].type = SquareType.Three;
                    break;
                case CellType.Four:
                    board[x][y].type = SquareType.Four;
                    break;
                case CellType.Five:
                    board[x][y].type = SquareType.Five;
                    break;
                case CellType.Six:
                    board[x][y].type = SquareType.Six;
                    break;
                case CellType.Seven:
                    board[x][y].type = SquareType.Seven;
                    break;
                case CellType.Eight:
                    board[x][y].type = SquareType.Eight;
                    break;
            }
        }

        private void InitializeBoard(CellType[][] inputBoard)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    board[x][y].x = x;
                    board[x][y].y = y;
                    switch (inputBoard[x][y])
                    {
                        case CellType.NotVisible:
                            board[x][y].type = SquareType.Hidden;
                            break;
                        case CellType.Bomb:
                            board[x][y].type = SquareType.Bomb;
                            break;
                        case CellType.Flag:
                            board[x][y].type = SquareType.Flag;
                            break;
                        case CellType.Zero:
                            board[x][y].type = SquareType.Zero;
                            break;
                        case CellType.One:
                            board[x][y].type = SquareType.One;
                            break;
                        case CellType.Two:
                            board[x][y].type = SquareType.Two;
                            break;
                        case CellType.Three:
                            board[x][y].type = SquareType.Three;
                            break;
                        case CellType.Four:
                            board[x][y].type = SquareType.Four;
                            break;
                        case CellType.Five:
                            board[x][y].type = SquareType.Five;
                            break;
                        case CellType.Six:
                            board[x][y].type = SquareType.Six;
                            break;
                        case CellType.Seven:
                            board[x][y].type = SquareType.Seven;
                            break;
                        case CellType.Eight:
                            board[x][y].type = SquareType.Eight;
                            break;
                    }
                }
            }
        }

        private int width;
        private int height;
        private Move lastMove;
        private bool isFirstMove;
        private bool isSecondMove;
        private Queue<Move> moveQueue;
        private Square[][] board;
    }
}
