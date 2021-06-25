﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Image_Converter
{
    abstract class Menu
    {
        protected ConsoleLogger _logger = new ConsoleLogger();

        public void Start()
        {
            while (true)
            {
                ShowMenu();
                try
                {
                    if (!MenuFunctions())
                        break;
                    else
                        Console.Clear();
                }
                catch (MyException e)
                {
                    Console.Clear();
                    _logger.Error($"{e.Message}");
                }
            }
        }

        protected abstract void ShowMenu();
        protected abstract bool MenuFunctions();

        protected void WaitToKey()
        {
            Console.WriteLine("\n--->[Press enter to continue.]");
            Console.ReadKey();
        }
    }
}
