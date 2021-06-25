using System;
using System.Collections.Generic;
using System.Text;

namespace Image_Converter
{
    class ConsoleLogger
    {
        public void Success(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[SUCCESS]: ");
            Console.ResetColor();
            Console.Write(message);
            Console.WriteLine();
        }

        public void Failure(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[FAIL]: ");
            Console.ResetColor();
            Console.Write(message);
            Console.WriteLine();
        }

        public void Unknown(string message)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[UNKNOWN]: ");
            Console.ResetColor();
            Console.Write(message);
            Console.WriteLine();
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ERROR]: ");
            Console.Write(message + "\n");
            Console.WriteLine();
            Console.ResetColor();
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[INFO]: ");
            Console.Write(message);
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
