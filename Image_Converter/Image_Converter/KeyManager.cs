using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Image_Converter
{
    class KeyManager
    {
        String keyRoute;

        public KeyManager(string keyRoute)
        {
            if (IsValidKey(keyRoute))
                this.keyRoute = keyRoute;
            else
                throw new InvalidKeyException($"Invalid key! - ('{keyRoute}')");
        }

        public KeyManager() { }

        public Dictionary<char, string> GetKey()
        {
            Dictionary<char, string> key = new Dictionary<char, string>();
            string[] keyParts = ReadKey(keyRoute).Split(Configurations.SEPARATOR);

            foreach(string part in keyParts)
            {
                string[] item = part.Split(Configurations.BINDER);
                key.Add(Char.Parse(item[1]), item[0]);
            }
            return key;
        }

        public void GenerateKeyToFile(string route)
        {
            String GetValuePairs(Dictionary<char, string> table)
            {
                List<string> valuePairs = new List<string>();

                foreach(KeyValuePair<char, string> item in table)
                {
                    valuePairs.Add(
                        $"{item.Value}" +
                        $"{Configurations.BINDER}" +
                        $"{item.Key}"
                    );
                }
                return String.Join(Configurations.SEPARATOR, valuePairs);
            }

            Dictionary<char, string> keyTable = new Dictionary<char, string>();

            foreach (char key in GetShuffledCharacters())
            {
                keyTable.Add(
                    key,
                    Configurations.GetRandomCharacters(keyTable)
                );
            }

            File.WriteAllText($"{route}\\{Configurations.KEY_FILE_NAME}", GetValuePairs(keyTable));
        }

        Boolean IsValidKey(string route)
        {
            string key = ReadKey(route);
            string[] parts = key.Split(Configurations.SEPARATOR);

            if (parts.Length < Configurations.CHARACTERS.Count) //Basic case is 4! 'AA.0'
                return false;

            foreach(string item in parts)
            {
                string[] temp = item.Split(Configurations.BINDER);
                if (temp.Length != 2)
                    return false;
            }

            return true;
        }

        String ReadKey(string route)
        {
            if (File.Exists(route))
                return File.ReadAllText(route);
            else
                throw new FileNotFoundException($"File doesn't exist! - ('{route}')");
        }

        List<char> GetShuffledCharacters()
        {
            Random random = new Random();
            List<char> temp = new List<char>(Configurations.CHARACTERS);
            List<char> result = new List<char>();

            while (result.Count != Configurations.CHARACTERS.Count)
            {
                int index = random.Next(temp.Count);
                result.Add(temp[index]);
                temp.RemoveAt(index);
            }
            return result;
        }
    }
}
