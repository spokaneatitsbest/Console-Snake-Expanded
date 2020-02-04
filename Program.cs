using System;
using System.Threading;
using System.Windows.Input;

using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;



namespace Console_Snake_expanded
{

        
    
    class Program
    {
        public const string Version = "1.3";

        //private Window window = new Window();
        public static class Globals
        {
            public static string InputKey {get;set;} = "DownArrow";
            public static bool Running {get;set;}
            public static int Score {get;set;} = 1;
            public static string Render {get;set;}
            public static bool Pause {get;set;}
            public static int Speed {get;set;} = 100;
            public static int FPS {get;set;}
            public static string Facing {get;set;} = " ";
            public static int FPSmax {get;set;} = 10;
            
        }

        public static class SubThreadding
        {
            //public static Thread Input = new Thread(() => InputThread());
            public static Thread Input {get;set;}
            //public static Thread Manager = new Thread(() => Manager());
            public static bool InputState {get;set;}
            
        }
        /*public static void Manager()
        {
            while (true)
            {
                if (SubThreadding.InputState == false)
                {
                    
                }
            }
        }*/

        public static class Food
        {
            public static int x {get;set;}
            public static int y {get;set;}
            
        }


        //////////////////////////////////////////////////////
        
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
        //get speed
        /*    int op = 0;
            string inp;
            do
            {
                Console.Write("Update Speed (miliseconds):  ");
                inp = Console.ReadLine();
            } while (!int.TryParse(inp, out op));
            Globals.Speed = Int32.Parse(inp);*/
            StartMsg("s");
        //initiate secondary thread for input
            Globals.Running = true;
            //Thread subThread_Input = new Thread(() => InputThread());
            //SubThreadding.Manager.Start();
            //SubThreadding.Input.Start();
            Initialize();
        }
        public static void StartMsg(string mode){
        //Explain
            Thread.Sleep(Globals.Speed + 50);
            if (mode != "p")
            {
                Console.Clear();
                Console.Write("\n");
            }
            Console.Write("\r---------------------------------------------------------\n|                    Console Snake v" + Version + "                 |\n---------------------------------------------------------");
            if (mode == "p")
            {
                Console.Write("\n\n--------------------------PAUSED-------------------------\n--------------------Press h for help---------------------\n--------------------Press any key to resume--------------------");
            } else {
            Console.Write("\n-----------------------How To Play-----------------------\n   * Eat as much as possible, without hitting yourself. \n   * Edges don't kill you, they just loop back to the other side\n   * Use arrow keys to move\n   * Use escape to pause\n   * Press ` to change the speed\n   * Press h to display this message\nPress any key to");
            }
            if (mode == "r")
            {
                Console.Write(" resume!");
            } else if (mode == "s"){
                Console.Write(" start!");
            }
            var ch = Console.ReadKey(true).Key;
            Console.Clear();
            Globals.Pause = false;
        }
        public static void Initialize()
        {
            Console.Clear();
            Globals.Pause = false;
            Globals.Running = true;
            Globals.Score = 1;
        //initiate snake
            Snake snake = new Snake();
            var rand = new Random();
            Food.x = rand.Next(10)+1;
            Food.y = rand.Next(10)+1;
            snake.Length = 1;
            Game(snake);
        }
        public static void Game(Snake snake)
        {
            Console.Clear();
            SubThreadding.Input = new Thread(() => InputThread());
            SubThreadding.Input.Start();
            int millisecondsPast = Convert.ToInt32(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond % 1000);
            while (Globals.Running == true)
            {
                Window window = new Window(); //initialize new window
            //rearrange snake array
            
                for (int s = snake.Length; s > 0; s--) 
                {
                    snake[s,0] = snake[s-1,0];
                    snake[s,1] = snake[s-1,1];
                }
            //move the snake array
                switch(Globals.InputKey) {
                    case "LeftArrow": { //left
                        Globals.Facing = "Left";
                        if (snake[1,1]-1 == 0) //loop
                        {
                            
                            snake[0,0] = snake[1,0];
                            snake[0,1] = 20;
                        } else {
                            snake[0,0] = snake[1,0];
                            snake[0,1] = snake[1,1]-1;
                        }
                        break;
                    }
                    case "RightArrow": { //Right
                        Globals.Facing = "Right";
                        if (snake[1,1]+1 == 21) //loop
                        {
                            snake[0,0] = snake[1,0];
                            snake[0,1] = 1;
                        } else {
                            snake[0,0] = snake[1,0];
                            snake[0,1] = snake[1,1]+1;
                        }
                        break;
                    }
                    case "UpArrow": { //Up
                        Globals.Facing = "Up";
                        if (snake[1,0]-1 == 0) //loop
                        {
                            snake[0,0] = 20;
                            snake[0,1] = snake[1,1];
                        } else {
                            snake[0,0] = snake[1,0]-1;
                            snake[0,1] = snake[1,1];
                        }
                        break;
                    }
                    case "DownArrow": { //Down
                        Globals.Facing = "Down";
                        if (snake[1,0]+1 == 21)
                        {
                            snake[0,0] = 1;
                            snake[0,1] = snake[1,1];
                        } else {
                            snake[0,0] = snake[1,0]+1;
                            snake[0,1] = snake[1,1];
                        }
                        break;
                    }
                }
            
            /*//temp output of snake array
            //preserved for testing
                for (int s = 0; s<snake.Length+1; s++) 
                {
                    Console.WriteLine(snake[s,0] + ", " + snake[s,1]);
                }
                Console.WriteLine("")*/
            //insert snake into window
                for (int s = 0; s <= snake.Length; s++) 
                {
                    if (window[snake[s,0],snake[s,1]] == " ")
                    {
                        window[snake[s,0],snake[s,1]] = "o";
                    } else {
                        Globals.Running = false;
                    }
                    window[snake[0,0],snake[0,1]] = "o";
                    

                }
                
            //check food collision 
                if (snake[0,0] == Food.x && snake[0,1] == Food.y)
                {
                    Globals.Score = Globals.Score + 1;
                    var rand = new Random();
                    Food.x = rand.Next(20)+1;
                    Food.y = rand.Next(20)+1;
                    snake.Length = snake.Length+1;
                    
                }

            //insert food   
                window[Food.x,Food.y] = "*";
            //final render - Depricated
            //efficient array
                Globals.Render = "\r Console Snake		     Version " + Version + "\n";
                Console.SetCursorPosition(0, 0);
                for (int x = 0; x <= 21; x++)
                {
                    for (int y = 0; y <= 21; y++) 
                    {
                        Globals.Render = Globals.Render + window[x,y] + " ";
                    }
                    Globals.Render = Globals.Render + "\n";
                }
                Globals.Render = Globals.Render + $" Score: {Globals.Score}                         FPS: {Globals.FPS}/{Globals.FPSmax} ";
                Console.Write(Globals.Render);
            //pause
                while (Globals.Pause == true)
                {
                   Thread.Sleep(100);
                }
            //wait (this slows snake)
                int millisecondsNow = Convert.ToInt32(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond % 1000);
                Globals.FPS = 1000/Math.Abs((millisecondsNow-millisecondsPast+Convert.ToInt32(0.00001)));
                Thread.Sleep(Globals.Speed);
                millisecondsPast = millisecondsNow;
            
            }
            //end loop
            End();
        }
        public static void End()
        {
            //SubThreadding.Input.Interrupt();
            Console.WriteLine("\nGame Over. Press any key to continue.");
            string FileInput = System.IO.File.ReadAllText(@"Game.dat");
            string FileOutput = FileInput;
            string[] FileData = FileInput.Split('@');
            SubThreadding.Input.Join();
            string UserName;
            do
            {
                Console.WriteLine("Name?");
                UserName = Console.ReadLine();
            } while (UserName == "" || UserName == " ");
                   
            if (Convert.ToInt32(FileData[0]) < Globals.Score)
            {
                
                FileOutput = Globals.Score.ToString() + "@" + UserName + "@" + FileOutput;
                System.IO.File.WriteAllText (@"Game.dat", FileOutput);       
                Console.WriteLine("You have the high score of " + Globals.Score);    
            } else {
                FileOutput = FileOutput + "@" + Globals.Score.ToString() + "@" + UserName;
                System.IO.File.WriteAllText (@"Game.dat", FileOutput);
                Console.WriteLine("Your Score was " + Globals.Score + ". The high score is " + FileData[0] + ", set by " + FileData[1]);
            }
            Console.WriteLine("Play again? (y/n)");
            string YN = Console.ReadLine();
            if (YN == "y")
            {
                Initialize();
            } else {
                System.Environment.Exit(0);
            }
        }
        public static void InputThread()
        {
            while (Globals.Running == true)
            {
                var ch = Console.ReadKey(true).Key;
                switch(ch)
                {
                    case ConsoleKey.UpArrow:
                        if (Globals.Facing != "Down")
                        {
                            Globals.InputKey = "UpArrow";
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (Globals.Facing != "Up")
                            {
                            Globals.InputKey = "DownArrow";
                            }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (Globals.Facing != "Right")
                        {
                            Globals.InputKey = "LeftArrow";
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (Globals.Facing != "Left")
                        {
                            Globals.InputKey = "RightArrow";
                        }
                        break;      
                    case ConsoleKey.Escape:
                        /*Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("\nGame Paused.\nPress Enter to continue.");*/
                        Globals.Pause = true;
                        Thread.Sleep(Globals.Speed+50);
                        StartMsg("p");
                        break;
                    /*case ConsoleKey.Enter:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Globals.Pause = false;
                        break;*/
                    case ConsoleKey.Oem3: //grave
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Globals.Pause = true;
                        Thread.Sleep(Globals.Speed+50);
                        Console.Write($"\n\nGame Paused.\nCurrrent speed is {Globals.Speed}");
                        int op = 0;
                        string inp;
                        do
                        {
                            Console.Write("New update Speed (miliseconds):  ");
                            inp = Console.ReadLine();
                        } while (!int.TryParse(inp, out op));
                        Globals.Speed = Int32.Parse(inp);
                        Console.Clear();
                        Globals.FPSmax = 1000/Globals.Speed;
                        Globals.Pause = false;
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case ConsoleKey.H: //help
                        Globals.Pause = true;
                        Thread.Sleep(Globals.Speed+50);
                        StartMsg("r");
                        Console.Clear();
                        Globals.Pause = false;
                        break;

                }                
            }
        }

      /*  public static void FPScalc()
        {
            while (Globals.Running == true)
            {
                int millisecondsPast = Convert.ToInt32(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond % 1000);
                while (Globals.Pause == false)
                {
                    int millisecondsNow = Convert.ToInt32(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond % 1000);
                    Globals.FPS = Math.Abs((millisecondsNow-millisecondsPast));
                }
            }
        }*/
    }

}
