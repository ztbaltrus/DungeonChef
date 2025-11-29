using System;
using Microsoft.Xna.Framework;

namespace DungeonChef
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Src.Core.GameRoot();
            game.Run();
        }
    }
}
