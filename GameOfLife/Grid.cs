using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Grid
    {
        private int SizeX;
        private int SizeY;
        private Cell[,] cells;
        private Cell[,] nextGenerationCells;
        private static Random rnd;
        private Canvas drawCanvas;
        private Ellipse[,] cellsVisuals;


        public Grid(Canvas c)
        {
            drawCanvas = c;
            rnd = new Random();
            SizeX = (int)(c.Width / 5);
            SizeY = (int)(c.Height / 5);
            cells = new Cell[SizeX, SizeY];
            cellsVisuals = new Ellipse[SizeX, SizeY];

            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j] = new Cell(i, j, 0, false);
                    cells[i, j].IsAlive = GetRandomBoolean();

                }

            InitCellsVisuals();
        }


        public void Clear()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j] = new Cell(i, j, 0, false);
                    nextGenerationCells[i, j] = new Cell(i, j, 0, false);
                    cellsVisuals[i, j].Fill = Brushes.Gray;
                }
        }


        void MouseMove(object sender, MouseEventArgs e)
        {
            var cellVisual = sender as Ellipse;

            int i = (int)cellVisual.Margin.Left / 5;
            int j = (int)cellVisual.Margin.Top / 5;


            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!cells[i, j].IsAlive)
                {
                    cells[i, j].IsAlive = true;
                    cells[i, j].Age = 0;
                    cellVisual.Fill = Brushes.White;
                }
            }
        }

        public void UpdateGraphics()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                    cellsVisuals[i, j].Fill = cells[i, j].IsAlive
                                                  ? (cells[i, j].Age < 2 ? Brushes.White : Brushes.DarkGray)
                                                  : Brushes.Gray;
        }

        public void InitCellsVisuals()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    cellsVisuals[i, j] = new Ellipse();
                    cellsVisuals[i, j].Width = cellsVisuals[i, j].Height = 5;
                    double left = cells[i, j].PositionX;
                    double top = cells[i, j].PositionY;
                    cellsVisuals[i, j].Margin = new Thickness(left, top, 0, 0);
                    cellsVisuals[i, j].Fill = Brushes.Gray;
                    drawCanvas.Children.Add(cellsVisuals[i, j]);

                    cellsVisuals[i, j].MouseMove += MouseMove;
                    cellsVisuals[i, j].MouseLeftButtonDown += MouseMove;
                }

            UpdateGraphics();
        }


        public static bool GetRandomBoolean()
        {
            return rnd.NextDouble() > 0.8;
        }

        public void UpdateToNextGeneration()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j].IsAlive = nextGenerationCells[i, j].IsAlive;
                    cells[i, j].Age = nextGenerationCells[i, j].Age;
                }

            cells = nextGenerationCells;
            nextGenerationCells = null;

            UpdateGraphics();
        }


        public void Update()
        {
            nextGenerationCells = new Cell[SizeX, SizeY];

            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    nextGenerationCells[i, j] = CalculateNextGeneration(i, j);   
                }
            }

            UpdateToNextGeneration();
        }

        public Cell CalculateNextGeneration(int row, int column)
        {
            bool alive;
            int count, age;

            alive = cells[row, column].IsAlive;
            age = cells[row, column].Age;
            count = CountNeighbors(row, column);

            if (alive && (count == 2 || count == 3))
            {
                cells[row, column].Age++;
                return new Cell(row, column, cells[row, column].Age, true);
            }
            else if (!alive && count == 3)
            {
                return new Cell(row, column, 0, true);
            }

            return new Cell(row, column, 0, false);
        }

        public void CalculateNextGeneration(int row, int column, ref bool isAlive, ref int age)     // OPTIMIZED AND CLEAN NOW
        {
            isAlive = cells[row, column].IsAlive;
            age = cells[row, column].Age;

            int count = CountNeighbors(row, column);

            if (isAlive)
            {
                if (count == 2 || count == 3)
                {
                    isAlive = true;
                    cells[row, column].Age++;
                    age = cells[row, column].Age;
                }
                else
                {
                    isAlive = false;
                    age = 0;
                }
            }
            else if (count == 3)
            {

                isAlive = true;
                age = 0;
            }
        }

        public int CountNeighbors(int row, int column)
        {
            int count = 0;

            for (int i1 = row - 1; i1 <= row + 1; i1++)
            {
                for (int j1 = column - 1; j1 <= column + 1; j1++)
                {
                    if (i1 == row && j1 == column) continue;

                    if (i1 >= 0 && i1 < SizeX && j1 >= 0 && j1 < SizeY)
                    {
                        if (cells[i1, j1].IsAlive) count++;
                    }
                }
            }

            return count;
        }
    }
}