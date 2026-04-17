using System.Security.Cryptography.X509Certificates;

internal class Program
{
    public class Cell  //This class gives every xy coordinate in the maze it's own data that is used to construct the maze
    {
        public int X;
        public int Y;

        public Dictionary<string, bool> Walls { get; set; }  //Makes it so that I can create multiple named variables that behave like booleans under the same varaible

        public bool Visited;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;

            Walls = new Dictionary<string, bool> //This gives every cell a set of walls that get checked when rendering and when moving
        {
            { "top", true },
            { "left", true },
            { "bottom", true },
            { "right", true }
        }; //Only top and right walls are necessary for now but I keep bottom and left in case I were to change the maze creation function

            Visited = false;
        }
    }
    static void Main()
    {
        int score = 0;
        bool debug = false;
        subMain();
        void subMain() //Almost the entirety of Main has to be in this subMain because it needs to remember the score when doing multiple mazes
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.Write("Select the size of your maze:\n1. Easy 6x6\n2. Medium 10x10\n3. Hard 14x14\n4. Extreme 20x20\n5. Custom\n\nPress 1-5 to choose.");
            int width;
            int height;
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

            Cell[,] grid = new Cell[width, height]; //Generates the cells for the maze

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
            Cell selected = grid[rng.Next(0, width), rng.Next(0, height)]; //Chooses a random viable cell to start the maze generation from
            bool IsGenerating = true;
            ChooseNext(selected);
            void ChooseNext(Cell CurrentCell)
            {
                selected = CurrentCell;
                int x = CurrentCell.X;
                int y = CurrentCell.Y;
                CurrentCell.Visited = true;
                if (watch)
                {
                    Draw(false, CurrentCell.X, CurrentCell.Y);
                    Thread.Sleep(10); //This is so that it generates at a rate that human eyes can see, otherwise it would generate way too fast
                }
                while (CheckNeighbours(CurrentCell))
                {
                    switch (rng.Next(0, 4)) //Chooses a random direction to move towards, if the direction isn't available it will reroll (could theorethically get stuck forever if you're unlucky)
                    {
                        case 0:
                            if (y > 0 && grid[x, y - 1].Visited == false)
                            {
                                Cell next = grid[x, y - 1];
                                CurrentCell.Walls["top"] = false;  //I have no idea if this (and next.Walls) is necessary but I'm too scared to get rid of it
                                grid[CurrentCell.X, CurrentCell.Y].Walls["top"] = false;
                                next.Walls["bottom"] = false;
                                grid[next.X, next.Y].Walls["bottom"] = false;
                                ChooseNext(next);
                            }
                            break;
                        case 1:
                            if (x < width - 1 && grid[x + 1, y].Visited == false)
                            {
                                Cell next = grid[x + 1, y];
                                CurrentCell.Walls["right"] = false;
                                grid[CurrentCell.X, CurrentCell.Y].Walls["right"] = false;
                                next.Walls["left"] = false;
                                grid[next.X, next.Y].Walls["left"] = false;
                                ChooseNext(next);
                            }
                            break;
                        case 2:
                            if (y < height - 1 && grid[x, y + 1].Visited == false)
                            {
                                Cell next = grid[x, y + 1];
                                CurrentCell.Walls["bottom"] = false;
                                grid[CurrentCell.X, CurrentCell.Y].Walls["bottom"] = false;
                                next.Walls["top"] = false;
                                grid[next.X, next.Y].Walls["top"] = false;
                                ChooseNext(next);
                            }
                            break;
                        case 3:
                            if (x > 0 && grid[x - 1, y].Visited == false)
                            {
                                Cell next = grid[x - 1, y];
                                CurrentCell.Walls["left"] = false;
                                grid[CurrentCell.X, CurrentCell.Y].Walls["left"] = false;
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
            if (!fog) //This is to prevent the entire maze from being shown if you're playing fog of war
            {
                Draw(true, 0, 0);
            }
            else
            {
                Draw(false, 0, 0);
            }
            void Draw(bool full, int XPos, int YPos)
            {
                if (full) //This draws the entire maze, extremely bad if you're drawing the entire maze for everytime the player moves but good for initially creating the maze
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    // Console.WriteLine(XPos + " " + YPos);  //these two commented lines are for debugging
                    // Console.WriteLine();
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (x == selected.X && y == selected.Y && IsGenerating)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow; //Not used since it never draws the entire maze whilst generating
                            }
                            else if (x == Player.X && y == Player.Y && !IsGenerating)
                            {
                                Console.ForegroundColor = ConsoleColor.Red; //Shows the player
                            }
                            else if (x == width - 1 && y == height - 1 && !IsGenerating)
                            {
                                Console.ForegroundColor = ConsoleColor.Green; //Shows the end
                            }
                            else if (grid[x, y].Visited == true)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue; //Shows the path
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Black; //Shows walls
                            }
                            Console.Write("██");
                            if (grid[x, y].Walls["right"] == true) //This is how the walls are actually made, I draw an extra cell between the actual cells. This is also why the player always moves two cells
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            if (y < height) //makes it so that we don't get weird edges at the bottom of the maze
                            {
                                Console.Write("██");
                            }
                        }
                        Console.WriteLine();
                        for (int x = 0; x < width; x++) //This draws the inbetween walls for the y axis
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
                else //Draws only the 3x3 square around the player, this massively reduces epilepsy
                {
                    if (fog) //Clears the screen so that you can't see where you have previously been
                    {
                        Console.Clear();
                    }
                    if (XPos > 0) //This entire thing is a mess but I couldn't get it working when I tried shortening it, should be straightforward what it does
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

                    for (int y = YPosMin; y <= YPosMax; y++) //The PosMin and PosMax is to prevent the array from going out of bounds and crashing the entire thing
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
                if (debug) //The debug info, I should probably make it so that it always shows the score though
                {
                    Console.Write($"{score} {fog} {XPos} {YPos} ");
                }
            }
            bool CheckNeighbours(Cell current) //Checks if there's any available cells to move towards when generating
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

                if (available == 4) //If available == 4 then we know that no cell is possible to move to so we make it trace back in the algorithm
                {
                    return false;
                }
                else //available is always at least 1 except for the very first cell (if it's not an edge)
                {
                    return true;
                }
            }
            Play();
            void Play()
            {
                while (true)
                {
                    if (Player.X == width - 1 && Player.Y == height - 1) //If the player is on the end, it ends the game and restarts the program
                    {
                        scoreToAdd += (width * height);
                        if (fog)
                        {
                            scoreToAdd *= 10; //Giving 10x points for fog of war is completely arbitrary and does not mean that fog of war is ten times harder than normal
                        }
                        score += scoreToAdd;
                        subMain();
                    }
                    ConsoleKeyInfo key = Console.ReadKey();
                    switch (key.Key) //The simple WASD movement and a couple extra functions
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
                        case ConsoleKey.K: //Enables/Disables the debug info
                            debug = !debug;
                            break;
                        case ConsoleKey.R: //Manually restarts the program
                            subMain();
                            break;
                    }
                    Draw(false, Player.X, Player.Y); //Draws around the player to show that it moved
                }

            }
        }
    }
    
}