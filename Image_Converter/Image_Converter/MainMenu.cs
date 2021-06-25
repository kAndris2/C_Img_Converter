using System;
using System.Collections.Generic;
using System.Text;

namespace Image_Converter
{
    class MainMenu : Menu
    {
        protected override void ShowMenu()
        {
            Console.WriteLine("[MAIN MENU]:\n");
            List<string> options = new List<string>
            {
                "Encode pictures",
                "Decode pictures",
                "Add items to the archive",
                "Check duplicates",
                "Generate key"
            };

            for (int i = 0; i < options.Count; i++)
                Console.WriteLine($"({i + 1}). - {options[i]}");
            Console.WriteLine("\n(0). - Exit.");
        }

        protected override bool MenuFunctions()
        {
            Console.WriteLine("\nPlease enter an index to choose a function:");
            string input = Console.ReadLine();

            if (input == "0")
            {
                Environment.Exit(-1);
                return false;
            }
            else if (input == "1" || input == "2" || input == "3")
            {
                Console.Clear();

                Console.WriteLine("Please give me a path:");

                string path = GetPath(),
                       keyLocation = $"{path}\\{Configurations.KEY_FILE_NAME}";

                FileManager _fManager = new FileManager(
                    new KeyManager(keyLocation).GetKey(),
                    _logger
                );
                Console.Clear();

                switch(input)
                {
                    case "1":
                        _fManager.Find(path, FileManager.FileType.Image);
                        break;
                    case "2":
                        _fManager.Find(path, FileManager.FileType.Map);
                        break;
                    case "3":
                        Console.WriteLine("Please give me the zip file's location:");
                        string zipLocation = GetPath();

                        _fManager.Find(path, FileManager.FileType.Image, FileManager.Mode.Add, zipLocation);
                        break;
                }

                WaitToKey();
                return true;
            }
            else if (input == "4")
            {
                Console.Clear();
                Console.WriteLine("Please give me a path:");

                string path = GetPath();
                Console.Clear();

                List<string> duplicates = new FileManager().CountDuplicates(path);
                _logger.Info($"Found {duplicates.Count}pc of duplicate(s)!");
                Console.WriteLine();

                for(int i = 0; i < duplicates.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. - {duplicates[i]}");
                }

                WaitToKey();
                return true;
            }
            else if (input == "5")
            {
                Console.Clear();
                Console.WriteLine("Please give me a path where I will generate the key:");

                string path = GetPath();
                Console.Clear();

                new KeyManager().GenerateKeyToFile(path);

                return true;
            }
            else
                throw new InvalidInputException($"There is no such option! - ('{input}')");
        }

        private String GetPath()
        {
            string path = Console.ReadLine();

            if (IsValidPath(path))
                return path;

            throw new InvalidInputException($"The entered path is invalid! - ('{path}')");
        }

        private bool IsValidPath(string path)
        {
            return path.Contains(@":\");
        }
    }
}