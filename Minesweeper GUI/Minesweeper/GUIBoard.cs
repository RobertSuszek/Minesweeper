using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Media;

namespace Minesweeper
{    class GUIBoard
    {
        private Button[][] cells;
        private Game game;
        public StackPanel board;
        private StackPanel[] rows;

        private void InitializeBoard(Game Game)
        {
            game = Game;

            board = new StackPanel()
            {
                Orientation = Orientation.Vertical,
            };

            rows = new StackPanel[game.GetWidth()];
            cells = new Button[game.GetWidth()][];
            for (int x = 0; x < game.GetWidth(); x++)
            {
                rows[x] = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };

                cells[x] = new Button[game.GetHeight()];
                for (int y = 0; y < game.GetHeight(); y++)
                {
                    Button button = new Button()
                    {
                        Width = 32,
                        Height = 32,
                        Tag = string.Format("{0},{1}", x, y),
                    };
                    button.BorderThickness = new Thickness(1.5);
                    button.FontSize = 20;
                    button.FontWeight = FontWeights.Bold;
                    button.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(BoardClick);
                    button.MouseUp += new MouseButtonEventHandler(BoardClick);

                    cells[x][y] = button;
                    rows[x].Children.Add(button);
                }
                board.Children.Add(rows[x]);
            }
        }

        public GUIBoard(Game Game)
        {
            InitializeBoard(Game);
        }

        public void UpdateBoard()
        {
            for (int x = 0; x < game.GetWidth(); x++)
            {
                for (int y = 0; y < game.GetHeight(); y++)
                {
                    switch (game.GameData.board[x][y])
                    {
                        case CellType.NotVisible:
                            cells[x][y].Content = "";
                            break;
                        case CellType.Bomb:
                            cells[x][y].Content = "B";
                            cells[x][y].Background = new SolidColorBrush(Color.FromRgb(255, 92, 92));
                            break;
                        case CellType.Flag:
                            cells[x][y].Content = "F";
                            cells[x][y].Background = new SolidColorBrush(Color.FromRgb(163, 206, 255));
                            break;
                        case CellType.Zero:
                            cells[x][y].Content = "";
                            cells[x][y].Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                            break;
                        case CellType.One:
                            cells[x][y].Content = "1";
                            cells[x][y].Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                            break;
                        case CellType.Two:
                            cells[x][y].Content = "2";
                            cells[x][y].Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                            break;
                        case CellType.Three:
                            cells[x][y].Content = "3";
                            cells[x][y].Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                            break;
                        case CellType.Four:
                            cells[x][y].Content = "4";
                            cells[x][y].Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                            break;
                        case CellType.Five:
                            cells[x][y].Content = "5";
                            cells[x][y].Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                            break;
                        case CellType.Six:
                            cells[x][y].Content = "6";
                            cells[x][y].Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                            break;
                        case CellType.Seven:
                            cells[x][y].Content = "7";
                            cells[x][y].Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                            break;
                        case CellType.Eight:
                            cells[x][y].Content = "8";
                            cells[x][y].Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                            break;
                    }
                }
            }
        }

        void BoardClick(object sender, MouseButtonEventArgs e)
        {
            if (game.GameData.isRunning)
            {
                Control src = e.Source as Control;
                string[] s = string.Format("{0}", src.Tag).Split(',');
                int x = Int32.Parse(s[0]);
                int y = Int32.Parse(s[1]);
                MoveType type = new MoveType();
                switch (e.ChangedButton)
                {
                    case MouseButton.Left:
                        type = MoveType.Expose;
                        break;
                    case MouseButton.Right:
                        type = MoveType.Flag;
                        break;
                    case MouseButton.XButton2:
                        type = MoveType.ExposeAdjacent;
                        break;
                    default:
                        break;
                }
                game.MakeMove(y, x, type);
                UpdateBoard();
                Trace.WriteLine(string.Format("Win: {0}", game.GameData.win));
                Trace.WriteLine(string.Format("Lose: {0}", game.GameData.lose));

                if (game.GameData.lose)
                {
                    MessageBox.Show(string.Format("Przegrales"));
                }
                if (game.GameData.win)
                {
                    MessageBox.Show(string.Format("Wygrales"));
                }
            }
        }
    }
}
