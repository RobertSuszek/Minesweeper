using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using static Minesweeper.Solver;

namespace Minesweeper
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;
        private GUIBoard board;
        private Difficulty diff;
        private Solver solve;
        private DispatcherTimer dispatcherTimer;
        private int winCount = 0;
        private int loseCount = 0;
        private double winRatio = 0.0;
        private bool stopUntilKeypressed;

        public MainWindow()
        {
            InitializeComponent();

            diff = Difficulty.Easy;

            game = new Game(diff);
            board = new GUIBoard(game);
            this.GameGrid.Children.Add(board.board);
            solve = new Solver(game);

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        private void Reset()
        {
            this.GameGrid.Children.Remove(board.board);
            game = new Game(diff);
            board = new GUIBoard(game);

            this.GameGrid.Children.Add(board.board);
            solve = new Solver(game);
        }

        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            Reset();
            loseCount = 0;
            winCount = 0;
            winRatio = 0.0;
        }
        private void DifficultyButtonClick(object sender, RoutedEventArgs e)
        {
            string BtnName = (sender as Button).Name;
            switch (BtnName)
            {
                case "easyButton":
                    diff = Difficulty.Easy;
                    break;
                case "mediumButton":
                    diff = Difficulty.Medium;
                    break;
                case "hardButton":
                    diff = Difficulty.Hard;
                    break;
            }
            Reset();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (game.GameData.isRunning)
            {
                Move move = new Move();
                move = solve.GetNextMove(game.GameData.board);
                switch (move.Type)
                {
                    case MoveType.Expose:
                        game.MakeMove(move.Y, move.X, MoveType.Expose);
                        break;
                    case MoveType.ExposeAdjacent:
                        game.MakeMove(move.Y, move.X, MoveType.ExposeAdjacent);
                        break;
                    case MoveType.Flag:
                        game.MakeMove(move.Y, move.X, MoveType.Flag);
                        break;
                }
                if (game.GameData.lose)
                    MessageBox.Show("Przegrana, " + move.X + ", " + move.Y);

                board.UpdateBoard();
            }
            else
            {
                if (game.GameData.lose)
                    loseCount++;
                if (game.GameData.win)
                    winCount++;

                winRatio = (double)winCount / ((double)winCount + (double)loseCount) * 100.0;
                winRatio = Math.Round(winRatio, 2, MidpointRounding.AwayFromZero);

                win.Content = string.Format("Win: {0}", winCount);
                lose.Content = string.Format("Lose: {0}", loseCount);
                ratio.Content = string.Format("WR: {0}%", winRatio);
                Reset();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            stopUntilKeypressed = false;
        }
    }
}