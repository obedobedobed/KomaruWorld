using System;
using KomaruWorld;

using var game = new Game1();

try
{
    game.Run();
}
catch (Exception ex)
{
    Logger.Log(string.Empty);
    Logger.Error("----------ERROR----------");
    Logger.Error(ex.ToString());
    Logger.Error("Exiting from game...");
    Logger.WriteLogs();
    Environment.Exit(-1);
}