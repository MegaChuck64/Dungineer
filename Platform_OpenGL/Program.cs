//using GameCode;
using Dungineer;
using System;

try
{
    using var game = new MainGame();
    game.Run();
}
catch(Exception e)
{
    Console.WriteLine("An error occurred while running the game:");
    Console.WriteLine(e.Message);
    Console.WriteLine(e.StackTrace);
    Console.WriteLine("Please check the error and try again.");
    Environment.Exit(1);
}