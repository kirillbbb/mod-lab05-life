using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace cli_life
{
    public class Config
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }

    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;

        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Count(x => x.IsAlive);

            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }

        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }

    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;

        public int Columns => Cells.GetLength(0);
        public int Rows => Cells.GetLength(1);

        private readonly Random rand = new Random();

        public Board(int width, int height, int cellSize)
        {
            CellSize = cellSize;
            Cells = new Cell[width, height];

            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
        }

        public void Randomize(double density)
        {
            foreach (var c in Cells)
                c.IsAlive = rand.NextDouble() < density;
        }

        public void Advance()
        {
            foreach (var c in Cells)
                c.DetermineNextLiveState();

            foreach (var c in Cells)
                c.Advance();
        }

        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;

                            int nx = (x + dx + Columns) % Columns;
                            int ny = (y + dy + Rows) % Rows;

                            Cells[x, y].neighbors.Add(Cells[nx, ny]);
                        }
                    }
                }
            }
        }

        public int CountAlive()
        {
            return Cells.Cast<Cell>().Count(c => c.IsAlive);
        }

        public void Save(string path)
        {
            using var writer = new StreamWriter(path);

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                    writer.Write(Cells[x, y].IsAlive ? '1' : '0');

                writer.WriteLine();
            }
        }

        public void Load(string path)
        {
            var lines = File.ReadAllLines(path);

            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Columns; x++)
                    Cells[x, y].IsAlive = lines[y][x] == '1';
        }

        // DFS для поиска компонент
        public int CountClusters()
        {
            bool[,] visited = new bool[Columns, Rows];
            int clusters = 0;

            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if (Cells[x, y].IsAlive && !visited[x, y])
                    {
                        DFS(x, y, visited);
                        clusters++;
                    }
                }
            }

            return clusters;
        }

        private void DFS(int x, int y, bool[,] visited)
        {
            if (x < 0 || y < 0 || x >= Columns || y >= Rows) return;
            if (visited[x, y] || !Cells[x, y].IsAlive) return;

            visited[x, y] = true;

            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    if (!(dx == 0 && dy == 0))
                        DFS((x + dx + Columns) % Columns,
                            (y + dy + Rows) % Rows,
                            visited);
        }
    }

    class Program
    {
        static Config LoadConfig(string path)
        {
            return JsonSerializer.Deserialize<Config>(File.ReadAllText(path));
        }

        static void Render(Board board)
        {
            Console.Clear();

            for (int y = 0; y < board.Rows; y++)
            {
                for (int x = 0; x < board.Columns; x++)
                    Console.Write(board.Cells[x, y].IsAlive ? '*' : ' ');

                Console.WriteLine();
            }
        }

        static int RunSimulation(Board board)
        {
            int prev = -1;
            int stable = 0;
            int steps = 0;

            while (true)
            {
                board.Advance();
                steps++;

                int current = board.CountAlive();

                if (current == prev)
                    stable++;
                else
                    stable = 0;

                if (stable >= 5)
                    return steps;

                prev = current;
            }
        }

        static void Main(string[] args)
        {
            var config = LoadConfig("config.json");

            var board = new Board(config.Width, config.Height, config.CellSize);
            board.Randomize(config.LiveDensity);

            // визуализация (можно убрать)
            for (int i = 0; i < 50; i++)
            {
                Render(board);
                board.Advance();
                System.Threading.Thread.Sleep(200);
            }

            // исследование
            Directory.CreateDirectory("Data");

            using var writer = new StreamWriter("Data/data.txt");

            for (double d = 0.1; d <= 0.9; d += 0.1)
            {
                int total = 0;

                for (int i = 0; i < 5; i++)
                {
                    var b = new Board(config.Width, config.Height, config.CellSize);
                    b.Randomize(d);

                    total += RunSimulation(b);
                }

                int avg = total / 5;
                writer.WriteLine($"{d} {avg}");
                Console.WriteLine($"Density {d}: {avg}");
            }

            Console.WriteLine("Done.");
        }
    }
}