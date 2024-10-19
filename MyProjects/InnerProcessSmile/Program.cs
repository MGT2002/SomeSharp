using System;
using System.Collections.Generic;
using System.Threading;

namespace Programm
{
    class Programm
    {
        const char Player = (char)2;
        const int speed = 1;
        
        static (int x, int y) position = (5, 5);
        static int width = Console.LargestWindowWidth / 2;
        static int height = Console.LargestWindowHeight / 2;
        static Processes activeWindow = Processes.LUp;
        static ConsoleKey lastKey = ConsoleKey.NoName;

        static void Main()
        {
            Console.SetWindowSize(width, height);
            Console.CursorVisible = false;

            ProcessSync sync = new ProcessSync();

            Thread.Sleep(-1);
        }
        class ProcessSync
        {
            Dictionary<Processes, Mutex> mutexes = new();
            Processes currentState = Processes.none;
            object block = new object();
            EventWaitHandle keyHandler = new(false, EventResetMode.ManualReset, "MyKeyHandle");
            Dictionary<ConsoleKey, EventWaitHandle> handle = new();

            public ProcessSync()
            {
                for (Processes p = Processes.LUp; p <= Processes.LDown; p++)
                {
                    mutexes[p] = new Mutex(false, p.ToString());
                }

                ThreadPool.QueueUserWorkItem((obj) => Start());
                for (ConsoleKey p = ConsoleKey.LeftArrow; p <= ConsoleKey.DownArrow; p++)
                {
                    ConsoleKey t = p;
                    handle[t] = new EventWaitHandle(false, EventResetMode.ManualReset,
                        "MyHandle" + t);
                    ThreadPool.QueueUserWorkItem((obj) => MainWorker(t));
                }
                SetState();
                keyHandler.Set();
            }

            void MainWorker(ConsoleKey key)
            {
                while (true)
                {
                    handle[key].WaitOne();

                    #region MyCode

                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            position.y--;
                            if (position.y < 0)
                            {
                                if (activeWindow == Processes.LDown)
                                {
                                    activeWindow = Processes.LUp;
                                    position.y = height - 1;
                                }
                                else if (activeWindow == Processes.RDown)
                                {
                                    activeWindow = Processes.RUp;
                                    position.y = height - 1;
                                }
                                else
                                    position.y++;
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            position.y++;
                            if (position.y >= height)
                            {
                                if (activeWindow == Processes.LUp)
                                {
                                    activeWindow = Processes.LDown;
                                    position.y = 0;
                                }
                                else if (activeWindow == Processes.RUp)
                                {
                                    activeWindow = Processes.RDown;
                                    position.y = 0;
                                }
                                else
                                    position.y--;
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            position.x++;
                            if (position.x >= width)
                            {
                                if (activeWindow == Processes.LUp)
                                {
                                    activeWindow = Processes.RUp;
                                    position.x = 0;
                                }
                                else if (activeWindow == Processes.LDown)
                                {
                                    activeWindow = Processes.RDown;
                                    position.x = 0;
                                }
                                else
                                    position.x--;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            position.x--;
                            if (position.x < 0)
                            {
                                if (activeWindow == Processes.RUp)
                                {
                                    activeWindow = Processes.LUp;
                                    position.x = width - 1;
                                }
                                else if (activeWindow == Processes.RDown)
                                {
                                    activeWindow = Processes.LDown;
                                    position.x = width - 1;
                                }
                                else
                                    position.x++;
                            }
                            break;
                    }

                    if (currentState == activeWindow)
                    {
                        Console.SetCursorPosition(position.x, position.y);
                        Console.Write(Player);
                    }
                    #endregion

                    keyHandler.Set();
                    handle[key].Reset();
                }
            }

            void Start()
            {
                while (true)
                {
                    keyHandler.WaitOne();
                    switch (lastKey = Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.UpArrow:
                            handle[lastKey].Set();
                            break;
                        case ConsoleKey.DownArrow:
                            handle[lastKey].Set();
                            break;
                        case ConsoleKey.RightArrow:
                            handle[lastKey].Set();
                            break;
                        case ConsoleKey.LeftArrow:
                            handle[lastKey].Set();
                            break;
                        default:
                            continue;
                    }
                    keyHandler.Reset();
                }
            }
            private void SetState()
            {
                ThreadPool.QueueUserWorkItem((obj) => Task1(Processes.LUp));
                ThreadPool.QueueUserWorkItem((obj) => Task1(Processes.RUp));
                ThreadPool.QueueUserWorkItem((obj) => Task1(Processes.RDown));
                ThreadPool.QueueUserWorkItem((obj) => Task1(Processes.LDown));
            }
            private void Task1(Processes process)
            {
                mutexes[process].WaitOne();
                lock (block)
                {
                    //Console.WriteLine((int)process + "S");
                    if (currentState == Processes.none)
                        currentState = process;
                    //Console.WriteLine((int)process + "E");
                }

                if (process == currentState)
                {
                    Console.WriteLine(process);
                    Thread.Sleep(-1);
                }
                mutexes[process].ReleaseMutex();
            }
        }
    }
    enum Processes
    {
        none,
        LUp,
        RUp,
        RDown,
        LDown,
    }
}