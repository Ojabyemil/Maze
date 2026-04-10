using System.Security.Cryptography.X509Certificates;

internal class Program
{
    public class Cell
    {
        public int X;
        public int Y;

        public Dictionary<string, bool> Walls { get; set; }

        public bool Visited;
        public bool Solution;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;

            Walls = new Dictionary<string, bool>
        {
            { "top", true },
            { "left", true },
            { "bottom", true },
            { "right", true }
        };

            Visited = false;
            Solution = false;
        }
    }
    static void Main(string[] args)
    {
        int score = 0;
        bool debug = false;
        subMain();
        void subMain()
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.Write("Select the size of your maze:\n1. Easy 6x6\n2. Medium 10x10\n3. Hard 14x14\n4. Extreme 20x20\n5. Custom\n\nPress 1-5 to choose.");
            int width = 1;
            int height = 1;
            int scoreToAdd = 0;
            ConsoleKeyInfo size = Console.ReadKey();
            switch (size.Key)
            {
                case ConsoleKey.D1:
                    width = 6; height = 6; break;
                case ConsoleKey.D2:
                    width = 10; height = 10; break;
                case ConsoleKey.D3:
                    width = 14; height = 14; break;
                case ConsoleKey.D4:
                    width = 20; height = 20;
                    Console.Clear();
                    Console.Write("Please enter fullscreen and zoom out at least twice (ctrl + mouse wheel down)\nPress enter to continue");
                    Console.ReadLine();
                    break;
                case ConsoleKey.D5:
                    Console.Clear();
                    Console.Write("Enter width: ");
                    Console.CursorVisible = true;
                    width = int.Parse(Console.ReadLine());
                    Console.Write("Enter height: ");
                    height = int.Parse(Console.ReadLine()); break;
                default:
                    subMain();
                    break;
            }
            Console.Clear();
            Console.Write("Do you want to play with fog?\nPress y for fog, press anything else to play without (normal)");
            bool fog = false;
            ConsoleKeyInfo FoW = Console.ReadKey();
            if (FoW.Key == ConsoleKey.Y)
            {
                fog = true;
            }
            Console.Clear();
            Console.Write("Do you want to skip the creation animation?\nPress y to skip, press anything else to watch it");
            bool watch = true;
            ConsoleKeyInfo Watch = Console.ReadKey();
            if (Watch.Key == ConsoleKey.Y)
            {
                watch = false;
            }
            Console.CursorVisible = false;
            int YPosMin;
            int YPosMax;
            int XPosMin;
            int XPosMax;
            const int cellWidth = 4;
            const int cellHeight = 2;

            Cell[,] grid = new Cell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = new Cell(x, y);
                }
            }
            Cell Player = grid[0, 0];

            Console.Clear();
            Random rng = new Random();
            Cell current = grid[rng.Next(0, width), rng.Next(0, height)];
            bool IsGenerating = true;
            ChooseNext(current);
            void ChooseNext(Cell curr)
            {
                current = curr;
                int x = curr.X;
                int y = curr.Y;
                curr.Visited = true;
                if (watch)
                {
                    Draw(false, curr.X, curr.Y);
                    Thread.Sleep(10);
                }
                while (CheckNeighbours(curr))
                {
                    switch (rng.Next(0, 4))
                    {
                        case 0:
                            if (y > 0 && grid[x, y - 1].Visited == false)
                            {
                                Cell next = grid[x, y - 1];
                                curr.Walls["top"] = false;
                                grid[curr.X, curr.Y].Walls["top"] = false;
                                next.Walls["bottom"] = false;
                                grid[next.X, next.Y].Walls["bottom"] = false;
                                ChooseNext(next);
                            }
                            break;
                        case 1:
                            if (x < width - 1 && grid[x + 1, y].Visited == false)
                            {
                                Cell next = grid[x + 1, y];
                                curr.Walls["right"] = false;
                                grid[curr.X, curr.Y].Walls["right"] = false;
                                next.Walls["left"] = false;
                                grid[next.X, next.Y].Walls["left"] = false;
                                ChooseNext(next);
                            }
                            break;
                        case 2:
                            if (y < height - 1 && grid[x, y + 1].Visited == false)
                            {
                                Cell next = grid[x, y + 1];
                                curr.Walls["bottom"] = false;
                                grid[curr.X, curr.Y].Walls["bottom"] = false;
                                next.Walls["top"] = false;
                                grid[next.X, next.Y].Walls["top"] = false;
                                ChooseNext(next);
                            }
                            break;
                        case 3:
                            if (x > 0 && grid[x - 1, y].Visited == false)
                            {
                                Cell next = grid[x - 1, y];
                                curr.Walls["left"] = false;
                                grid[curr.X, curr.Y].Walls["left"] = false;
                                next.Walls["right"] = false;
                                grid[next.X, next.Y].Walls["right"] = false;
                                ChooseNext(next);
                            }
                            break;
                    }
                }
            }
            IsGenerating = false;

            Console.Clear();
            if (!fog)
            {
                Draw(true, 0, 0);
            }
            else
            {
                Draw(false, 0, 0);
            }
            void Draw(bool full, int XPos, int YPos)
            {
                if (full)
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    // Console.WriteLine(XPos + " " + YPos);
                    // Console.WriteLine();
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (x == current.X && y == current.Y && IsGenerating)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                            }
                            else if (x == Player.X && y == Player.Y && !IsGenerating)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                            else if (x == width - 1 && y == height - 1 && !IsGenerating)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                            }
                            else if (grid[x, y].Visited == true)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            Console.Write("██");
                            if (grid[x, y].Walls["right"] == true)
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            if (y < height)
                            {
                                Console.Write("██");
                            }
                        }
                        Console.WriteLine();
                        for (int x = 0; x < width; x++)
                        {
                            if (grid[x, y].Walls["bottom"] == true)
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            if (x < width)
                            {
                                Console.Write("██  ");
                            }
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    if (fog)
                    {
                        Console.Clear();
                    }
                    if (XPos > 0)
                    {
                        XPosMin = -1;
                    }
                    else
                    {
                        XPosMin = 0;
                    }

                    if (YPos > 0)
                    {
                        YPosMin = -1;
                    }
                    else
                    {
                        YPosMin = 0;
                    }

                    if (XPos < width - 1)
                    {
                        XPosMax = 1;
                    }
                    else
                    {
                        XPosMax = 0;
                    }

                    if (YPos < height - 1)
                    {
                        YPosMax = 1;
                    }
                    else
                    {
                        YPosMax = 0;
                    }

                    for (int y = YPosMin; y <= YPosMax; y++)
                    {
                        Console.SetCursorPosition((XPos + XPosMin) * cellWidth, (YPos + y) * cellHeight);
                        for (int x = XPosMin; x <= XPosMax; x++)
                        {
                            if (x == 0 && y == 0 && IsGenerating)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                            }
                            else if (x == 0 && y == 0 && !IsGenerating)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                            else if (XPos + x == width - 1 && YPos + y == height - 1)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                            }
                            else if (grid[XPos + x, YPos + y].Visited == true)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            Console.Write("██");
                            if (grid[XPos + x, YPos + y].Walls["right"] == true)
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            Console.Write("██");
                        }
                        Console.SetCursorPosition((XPos + XPosMin) * cellWidth, (YPos + y) * cellHeight + 1);
                        for (int x = XPosMin; x <= XPosMax; x++)
                        {
                            if (grid[XPos + x, YPos + y].Walls["bottom"] == true)
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            Console.Write("██");
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write("██");
                        }
                    }
                    Console.SetCursorPosition(0, height * cellHeight);
                }

                Console.ForegroundColor = ConsoleColor.White;
                if (debug)
                {
                    Console.Write($"{score} {fog} {XPos} {YPos} ");
                }
            }
            bool CheckNeighbours(Cell current)
            {
                int x = current.X;
                int y = current.Y;
                int available = 0;
                if (y == 0)
                {
                    available++;
                }
                else if (grid[x, y - 1].Visited)
                {
                    available++;
                }

                if (x == width - 1)
                {
                    available++;
                }
                else if (grid[x + 1, y].Visited)
                {
                    available++;
                }

                if (y == height - 1)
                {
                    available++;
                }
                else if (grid[x, y + 1].Visited)
                {
                    available++;
                }

                if (x == 0)
                {
                    available++;
                }
                else if (grid[x - 1, y].Visited)
                {
                    available++;
                }

                if (available == 4)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            Play();
            void Play()
            {
                while (true)
                {
                    if (Player.X == width - 1 && Player.Y == height - 1)
                    {
                        scoreToAdd += (width * height);
                        if (fog)
                        {
                            scoreToAdd *= 10;
                        }
                        score += scoreToAdd;
                        subMain();
                    }
                    ConsoleKeyInfo key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.W:
                            if (grid[Player.X, Player.Y].Walls["top"] == false)
                            {
                                Player = grid[Player.X, Player.Y - 1];
                            }
                            break;
                        case ConsoleKey.D:
                            if (grid[Player.X, Player.Y].Walls["right"] == false)
                            {
                                Player = grid[Player.X + 1, Player.Y];
                            }
                            break;
                        case ConsoleKey.S:
                            if (grid[Player.X, Player.Y].Walls["bottom"] == false)
                            {
                                Player = grid[Player.X, Player.Y + 1];
                            }
                            break;
                        case ConsoleKey.A:
                            if (grid[Player.X, Player.Y].Walls["left"] == false)
                            {
                                Player = grid[Player.X - 1, Player.Y];
                            }
                            break;
                        case ConsoleKey.K:
                            debug = !debug;
                            break;
                        case ConsoleKey.R:
                            subMain();
                            break;
                    }
                    Draw(false, Player.X, Player.Y);
                }

            }
        }
    }
    
}