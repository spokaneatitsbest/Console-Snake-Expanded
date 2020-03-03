using System;
using System.Threading;
using System.Windows.Input;

using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;




namespace Console_Snake_expanded
{ 
    public static class Globals
    {
        public static string InputKey {get;set;} = "DownArrow";
        public static bool Running {get;set;}
        public static int Score {get;set;} = 1;
        public static string Render {get;set;}
        public static bool Pause {get;set;}
        public static int Speed {get;set;}// = 100;
        public static int SpeedGoal {get;set;}
        public static int FPS {get;set;}
        public static string Facing {get;set;} = " ";
        public static int FPSmax {get;set;}// = 10;
    }
    class Program
    {
        public const string Version = "1.5";

        //private Window window = new Window();
        

        public static class SubThreadding
        {
            public static Thread Input {get;set;}   
            public static bool InputState {get;set;}
            public static Thread InsertStuff {get;set;}
            public static Thread Renderer {get;set;}
        }
        public static class Food
        {
            public static int x {get;set;}
            public static int y {get;set;}
            public static bool FoodRequested {get;set;} = true;
            
        }
        public static class Settings
        {
            public static string silentStart {get;set;} = "false";
            public static ConsoleColor backCol {get;set;} = ConsoleColor.Black;
            public static ConsoleColor textCol {get;set;} = ConsoleColor.Green;
        }

        //////////////////////////////////////////////////////
        static void getSettings()
        {
        //create settings file if nonexistant
            if (!File.Exists("Settings.dat"))
            {
                StreamWriter sw = new StreamWriter(File.Open("Settings.dat", System.IO.FileMode.Append));
                sw.Write("silentStart=false\nbackColor=Black\ntextCol=Green\nDefaultSpeed=100");
                sw.Write("\n\nList of colors:\nBlack\nDarkBlue\nDarkGreen\nDarkCyan\nDarkRed\nDarkMagenta\nDarkYellow\nGray\nDarkGray\nBlue\nGreen\nCyan\nRed\nMagenta\nYellow\nWhite\nBlack\nDarkBlue\nDarkGreen\nDarkCyan\nDarkRed\nDarkMagenta\nDarkYellow\nGray\nDarkGray\nBlue\nGreen\nCyan\nRed\nMagenta\nYellow\nWhite");
                sw.Close();
            }
            string[] lines = System.IO.File.ReadAllLines("Settings.dat");
            Settings.silentStart = lines[0].Split("=")[1];
            Console.BackgroundColor =(ConsoleColor) Enum.Parse(typeof(ConsoleColor), lines[1].Split("=")[1],true);
            Console.ForegroundColor =(ConsoleColor) Enum.Parse(typeof(ConsoleColor), lines[2].Split("=")[1],true);
            Globals.Speed = Int32.Parse(lines[3].Split("=")[1]);
            Globals.SpeedGoal = Globals.Speed;
            //Console.WriteLine(lines[3]);
            Globals.FPSmax = 1000/Globals.Speed;
        }
        static void Main(string[] args)
        {
            getSettings();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            if (Settings.silentStart == "false")
            {
                StartMsg("s");
            }
            
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
            Console.Write("\n-----------------------How To Play-----------------------\n   * Eat as much as possible, without hitting yourself. \n   * Edges don't kill you, they just loop back to the other side\n   * Use arrow keys to move\n   * Use escape to pause\n   * Press ` to change the speed\n   * Press h to display this message\n   * Settings are stored in Settings.dat\nPress any key to");
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
        public static Snake snake;
        public static Window window;
        public static void Initialize()
        {
            Console.Clear();
            Globals.Pause = false;
            Globals.Running = true;
            Globals.Score = 1;
        //initiate snake
            snake = new Snake();
            var rand = new Random();
            Food.x = rand.Next(10)+1;
            Food.y = rand.Next(10)+1;
            snake.Length = 1;
            Game();
        }
        public static void Game()
        {
            Console.Clear();
            SubThreadding.Input = new Thread(() => InputThread());
            SubThreadding.Input.Start();
            SubThreadding.Renderer = new Thread(() => Console.Write(""));
            SubThreadding.Renderer.Start();
            int millisecondsPast = Convert.ToInt32(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond % 1000);
            var rand = new Random();
            while (Globals.Running == true) //LOOP START////////////////////////////////////////////////////////////
            {
                window = new Window(); //initialize new window
                if (Food.FoodRequested == true) { //make food
                    do {
                    Food.x = rand.Next(20)+1;
                    Food.y = rand.Next(20)+1;
                    } while (FoodCollision() == false);
                    Food.FoodRequested = false;
                }

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
            
            /////////////////////////////////////////////////////////////////////////////
            //insert stuff into window
                SubThreadding.InsertStuff = new Thread(() => insertStuff());
                SubThreadding.InsertStuff.Start();
                //insertStuff();
                SubThreadding.Renderer = new Thread(() => RenderWindow());
                SubThreadding.Renderer.Start();
            /////////////////////////////////////////////////////////////////////////////
            //check food collision 
                if (snake[0,0] == Food.x && snake[0,1] == Food.y)
                {
                    Globals.Score = Globals.Score + 1;
                    Food.FoodRequested = true;
                    //Food.x = rand.Next(20)+1;
                    //Food.y = rand.Next(20)+1;
                    snake.Length = snake.Length+1;
                }

            


                                
                //RenderWindow();
            //pause
                while (Globals.Pause == true)
                {
                   Thread.Sleep(100);
                }
            //wait (this slows snake)
                
                int millisecondsNow = Convert.ToInt32(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond % 1000);
                try {
                    Globals.FPS = 1000/Math.Abs((millisecondsNow-millisecondsPast+Convert.ToInt32(0.00001)));
                }
                catch (DivideByZeroException) {
                    //Globals.FPS = 1;
                    continue;
                }
                //Console.Write($"{millisecondsNow-millisecondsPast}, {Globals.SpeedGoal - (millisecondsNow-millisecondsPast)}, {Globals.SpeedGoal}");
                //Console.Write(Math.Abs(Globals.Speed - (millisecondsNow-millisecondsPast)));
                Thread.Sleep(Globals.Speed);
                millisecondsPast = millisecondsNow;
            //confirm multithread termination
                SubThreadding.InsertStuff.Join();
                SubThreadding.Renderer.Join();
                
                /*if (Globals.FPS < .8*Globals.FPSmax)
                {
                    Globals.Speed = Globals.Speed-1;
                    Console.Write($"Speed Adjusted to {Globals.Speed}. {Globals.FPS} {Globals.FPSmax}");
                }*/
                
            
            }
            //end loop
            End();
        }
        public static void RenderWindow()
        {
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
            
        }
        public static void insertStuff()
        {
        //insert food
            window[Food.x,Food.y] = "*";
        //snake
            for (int s = 0; s <= snake.Length; s++) 
            {
                if (window[snake[s,0],snake[s,1]] == " ")
                {
                    window[snake[s,0],snake[s,1]] = "o";
                } else if (window[snake[s,0],snake[s,1]] == "*") {
                    window[snake[s,0],snake[s,1]] = "o";
                    Food.FoodRequested = true;
                } else {
                    Globals.Running = false;
                }
                window[snake[0,0],snake[0,1]] = "o";
            }
        }
        
        public static void End()
        {
            //SubThreadding.Input.Interrupt();
            Console.WriteLine("\nGame Over. Press any key to continue.");
        //create file
            if (!File.Exists("Game.dat"))
            {
                StreamWriter sw = new StreamWriter(File.Open("Game.dat", System.IO.FileMode.Append));
                sw.Write("1@default");
                sw.Close();
            }

            string[] FileInput = System.IO.File.ReadAllLines("Game.dat");
            
            string FileOutput = "";
            //line format
            //score@name@gamespeed

            SubThreadding.Input.Join();
            string UserName;
            do
            {
                Console.WriteLine("Name?");
                UserName = Console.ReadLine();
            } while (UserName == "" || UserName == " ");
                   
            if (Convert.ToInt32(FileInput[0].Split("@")[0]) < Globals.Score)
            {
                FileOutput = $"{Globals.Score.ToString()}@{UserName}@{Globals.Speed.ToString()}\n";
                for (int i=0;i<FileInput.Length;i++)
                {
                    FileOutput = FileOutput + FileInput[i] + "\n";
                }
                System.IO.File.WriteAllText (@"Game.dat", FileOutput);
                Console.WriteLine($"{UserName}, you have the high score of " + Globals.Score);    
            } else {
                for (int i=0;i<FileInput.Length;i++)
                {
                    FileOutput = FileOutput + FileInput[i] + "\n";
                    //Console.Write(FileOutput);
                }
                FileOutput = FileOutput + $"{Globals.Score.ToString()}@{UserName}@{Globals.Speed.ToString()}\n";
                System.IO.File.WriteAllText (@"Game.dat", FileOutput);
                Console.WriteLine("Your Score was " + Globals.Score + ". The high score is " + FileInput[0].Split("@")[0] + ", set by " + FileInput[0].Split("@")[1]);
            }
            Console.WriteLine("Play again? (y/n)");
            string YN = Console.ReadLine();
            if (YN == "y")
            {
                Initialize();
            } else if (YN == "Y") {
                Initialize();
            } else {
                System.Environment.Exit(0);
            }
        }
        public static bool FoodCollision()
        {
            //bool foodState = false;
            for (int i=0; i<= snake.Length; i++)
            {
                if (snake[i,0] == Food.x)
                {
                    if (snake[i,1] == Food.y)
                    {
                        return false;
                    }
                }
            }
            return true;
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
                        Globals.SpeedGoal = Globals.Speed;
                        Console.Clear();
                        Globals.FPSmax = 1000/Globals.Speed;
                        Globals.Pause = false;
                        
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

    }

}
