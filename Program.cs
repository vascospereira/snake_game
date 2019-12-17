using System;

namespace Snake
{
    internal class Program
    {
        private const int Line = 12;
        private const int Column = 32;
        private const string field = " ";
        private const string wall = "#";
        private const string snakeBody = "+";
        private const int MinBoardLineSize = 6;
        private const int MinBoardColumnSize = 12;
        private const int minRandFoodPosition = 2;
        private static int _posXSnake = Column / 2 ;
        private static int _posYSnake = Line / 2;
        // Snake array -> snake length and x and y positions
        private static readonly int[,] Snake = new int[Line * Column / 2, 2];
        private static int _posXFood, _posYFood;
        private static int _snakeSize = 3, _timeWait = 500, _points;
        private static bool _endGame;
        private enum Direction { Right, Left, Up, Down };

        private static void Main()
        {
            if (BoardSizeOK())
            {
                DesignBoard();
                CreateSnake();
                DrawFood();
                Direction dir = Direction.Right;
                Console.SetWindowSize(Column + 1, Line + 1);

                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo cki = Console.ReadKey();
                        switch (cki.Key)
                        {
                            case ConsoleKey.RightArrow:
                                dir = Direction.Right;
                                break;
                            case ConsoleKey.LeftArrow:
                                dir = Direction.Left;
                                break;
                            case ConsoleKey.UpArrow:
                                dir = Direction.Up;
                                break;
                            case ConsoleKey.DownArrow:
                                dir = Direction.Down;
                                break;
                        }
                    }

                    if (_endGame)
                    {
                        break;
                    }

                    MoveSnake(dir);
                    HitTheWall();
                    FoundFood();

                    System.Threading.Thread.Sleep(_timeWait);
                }
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("board too short [length > #5 && column > #11]");
                Console.ReadLine();
            }
        }

        private static bool BoardSizeOK()
        {
            return (Line < MinBoardLineSize || Column < MinBoardColumnSize) ? false : true;
        }

        private static void FoundFood()
        {
            if (Snake[0, 0] != _posXFood || Snake[0, 1] != _posYFood) 
            {
                return;
            }

            DrawFood();

            _snakeSize += 1;
            _timeWait -= 10;
            _points += 15;
        }

        private static void HitTheWall()
        {
            if (Snake[0, 0] != Column - 1 && Snake[0, 1] != Line - 1 && Snake[0, 0] != 1 && Snake[0, 1] != 1)
            {
                return;
            }
            TheEnd("Wall");
        }

        private static void MoveSnake(Direction d)
        {
            Console.SetCursorPosition(Snake[_snakeSize - 1, 0], Snake[_snakeSize - 1, 1]);
            Console.CursorVisible = false;
            // erase snake's tail
            Console.Write(" ");
            // set new snake's position
            for (int i = _snakeSize - 1; i > 0; i--)
            {
                Snake[i, 0] = Snake[i - 1, 0];
                Snake[i, 1] = Snake[i - 1, 1];
            }

            switch (d)
            {
                case Direction.Right:
                    ++Snake[0,0];
                    break;
                case Direction.Left:
                    --Snake[0,0];
                    break;
                case Direction.Down:
                    ++Snake[0,1];
                    break;
                case Direction.Up:
                    --Snake[0,1];
                    break;
            }

            Console.SetCursorPosition(Snake[0,0], Snake[0,1]);
            Console.WriteLine(snakeBody);

            if (HitTheSnake(Snake[0,0], Snake[0,1]))
            {
                _endGame = true;
                TheEnd("Snake");
            }
        }

        private static bool HitTheSnake(int posX, int posY)
        {
            for (int i = 1; i < _snakeSize; i++)
            {
                if (Snake[i, 0] == posX && Snake[i, 1] == posY)
                {
                    return true;
                }
            }

            return false;
        }

        private static void TheEnd(string msg)
        {
            const string gameOver = "****GAME OVER****";
            string _finalMsg = $"You Hit The {msg}!";
            string _finalPoints = $"{_points} pts.";

            if (gameOver.Length >= Column)
            {
                _endGame = true;
                string _fp = $"{_points} pts.";
                Console.SetCursorPosition((Column / 2) - (_fp.Length / 2), (Line / 2));
                Console.WriteLine(_fp);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition((Column / 2) - (gameOver.Length / 2), (Line / 2) - 1);
            Console.WriteLine(gameOver);
            Console.SetCursorPosition((Column / 2) - (_finalMsg.Length / 2), Line / 2);
            Console.WriteLine(_finalMsg);
            Console.SetCursorPosition((Column / 2) - (_finalPoints.Length / 2), (Line / 2) + 1);
            Console.WriteLine(_finalPoints);
            _endGame = true;
        }

        private static void DrawFood()
        {
            bool foodOverSnake;
            Random rnd = new Random();

            do
            {
                _posXFood = rnd.Next(minRandFoodPosition, Column - 1);
                _posYFood = rnd.Next(minRandFoodPosition, Line - 1);
                foodOverSnake = false;

                for (int i = 0; i < _snakeSize; i++)
                {
                    if (Snake[i, 0] != _posXFood || Snake[i, 1] != _posYFood)
                    {
                        continue;
                    }
                    foodOverSnake = true;
                }
            } while (foodOverSnake);

            Console.SetCursorPosition(_posXFood, _posYFood);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("@");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void CreateSnake()
        {
            for (int i = 0; i < _snakeSize; i++)
            {
                Snake[i, 0] = _posXSnake;
                Snake[i, 1] = _posYSnake;
                _posXSnake--;
            }

            for (int i = 0; i < _snakeSize; i++)
            {
                Console.SetCursorPosition(Snake[i,0], Snake[i,1]);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(snakeBody);
            }
        }

        private static void DesignBoard()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            for (int i = 0; i <= Line; i++)
            {
                for (int j = 0; j <= Column; j++)
                {
                    if (i == 0 && j == 1 || i == 1 && j == 0 || i == 1 && j == Column || 
                        i == 0 && j == Column - 1 || i == Line - 1 && j == 0 || 
                        i == Line && j == 1 || i == Line - 1 && j == Column || i == Line && j == Column - 1)
                    {
                        Console.Write(field);
                    }
                    else if(i == 1 || i == Line - 1 || j == 1 || j == Column - 1)
                    {
                        Console.Write(wall);
                    }
                    else
                    {
                        Console.Write(field);     
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
