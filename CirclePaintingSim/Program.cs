﻿using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Simulating circle painting...");

        Console.WriteLine($"K = 5   -> {Simulation.Run(5)} sec");
        Console.WriteLine($"K = 20  -> {Simulation.Run(20)} sec");
        Console.WriteLine($"K = 100 -> {Simulation.Run(100)} sec");

        Console.WriteLine("All circles painted.");
    }
}
