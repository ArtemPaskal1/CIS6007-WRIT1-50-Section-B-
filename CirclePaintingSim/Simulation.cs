using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public class Simulation
{
    const int NUM_CIRCLES = 1200;
    const int PAINT_DELAY_MS = 20;

    static bool[] painted = new bool[NUM_CIRCLES];
    static readonly object paintLock = new object();
    static int paintedCount = 0;
    static readonly object consoleLock = new object(); // Для потокобезопасного вывода

    static void Worker(int workerId, Queue<int> workQueue)
    {
        while (true)
        {
            int id;

            lock (workQueue)
            {
                if (workQueue.Count == 0)
                    return;

                id = workQueue.Dequeue();
            }

            bool paintedNow = false;

            lock (paintLock)
            {
                if (!painted[id])
                {
                    painted[id] = true;
                    Interlocked.Increment(ref paintedCount);
                    paintedNow = true;
                }
            }

            if (paintedNow)
                ShowProgress(paintedCount, NUM_CIRCLES);

            Thread.Sleep(PAINT_DELAY_MS);
        }
    }

    public static double Run(int K)
    {
        Queue<int> queue = new Queue<int>();
        for (int i = 0; i < NUM_CIRCLES; i++)
            queue.Enqueue(i);

        Array.Fill(painted, false);
        paintedCount = 0;

        Stopwatch sw = Stopwatch.StartNew();
        List<Thread> threads = new List<Thread>();

        for (int i = 0; i < K; i++)
        {
            int wid = i;
            Thread t = new Thread(() => Worker(wid, queue));
            t.Start();
            threads.Add(t);
        }

        foreach (var t in threads)
            t.Join();

        sw.Stop();
        return sw.Elapsed.TotalSeconds;
    }

    static void ShowProgress(int paintedCount, int total)
    {
        double ratio = (double)paintedCount / total;
        int width = 30;
        int filled = Math.Clamp((int)(ratio * width), 0, width);
        int empty = width - filled;

        lock (consoleLock)
        {
            Console.CursorLeft = 0;
            Console.Write($"Progress: [{new string('█', filled)}{new string('-', empty)}] {(int)(ratio * 100)}%");
            if (paintedCount == total)
                Console.WriteLine(); // перенос строки при завершении
        }
    }
}
