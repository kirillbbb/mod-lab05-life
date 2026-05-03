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
        public readonly List<Cell> neighbors = new();
        private bool next;

        public void DetermineNextLiveState()
        {
            int n = neighbors.Count(x => x.IsAlive);
            next = IsAlive ? (n == 2 || n == 3) : (n == 3);
        }

        public void Advance() => IsAlive = next;
    }

    public class Board
    {
        public readonly Cell[,] Cells;
        private readonly Random rand = new();

        public int Columns => Cells.GetLength(0);
        public int Rows => Cells.GetLength(1);

        public Board(int w, int h)
        {
            Cells = new Cell[w, h];

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    Cells[x, y] = new Cell();

            Connect();
        }

        void Connect()
        {
            for (int x = 0; x < Columns; x++)
            for (int y = 0; y < Rows; y++)
            {
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = (x + dx + Columns) % Columns;
                    int ny = (y + dy + Rows) % Rows;

                    Cells[x, y].neighbors.Add(Cells[nx, ny]);
                }
            }
        }

        public void Randomize(double d)
        {
            foreach (var c in Cells)
                c.IsAlive = rand.NextDouble() < d;
        }

        public void Advance()
        {
            foreach (var c in Cells) c.DetermineNextLiveState();
            foreach (var c in Cells) c.Advance();
        }

        public int CountAlive() =>
            Cells.Cast<Cell>().Count(c => c.IsAlive);
    }

    class Program
    {
        static Config LoadConfig() =>
            JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));

        static int Run(Board b)
        {
            int prev = -1, stable = 0, steps = 0;

            while (true)
            {
                b.Advance();
                steps++;

                int cur = b.CountAlive();

                if (cur == prev) stable++;
                else stable = 0;

                if (stable >= 5) return steps;

                prev = cur;
            }
        }

        static void Main()
        {
            var cfg = LoadConfig();
            Directory.CreateDirectory("Data");

            using var w = new StreamWriter("Data/data.txt");

            for (double d = 0.1; d <= 0.9; d += 0.1)
            {
                int sum = 0;

                for (int i = 0; i < 5; i++)
                {
                    var b = new Board(cfg.Width, cfg.Height);
                    b.Randomize(d);
                    sum += Run(b);
                }

                int avg = sum / 5;
                w.WriteLine($"{d} {avg}");
            }

            Console.WriteLine("Done");
        }
    }
}