using System;
using System.Collections.Generic;
using System.Text;

namespace Bimbrownia.AI.Shared
{
    public static class Misc
    {
        public static void WriteLineInColor(string msg, ConsoleColor color)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.WriteLine(msg);

            Console.ForegroundColor = prevColor;

        }
    }
}
