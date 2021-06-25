using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Image_Converter
{
    public static class Configurations
    {
        public static readonly char SEPARATOR = 'g';
        public static readonly char BINDER = 'f';

        public static readonly String KEY_FILE_NAME = "key.sec";
        public static readonly String ENCODE_FOLDER_NAME = "encode";
        public static readonly String DECODE_FOLDER_NAME = "decode";
        public static readonly String TEMP_EXTRACT_FOLDER = "extract";

        public static readonly int MAX_ASSISTANT_CHAR = 4;
        public static readonly int START_CHAR_ASCII = 33;
        public static readonly int END_CHAR_ASCII = 126;

        public static List<char> CHARACTERS = new List<char>()
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',

            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',

            'Á', 'É', 'Í', 'Ó', 'Ö', 'Ő', 'Ú', 'Ü', 'Ű',

            '.', '-', '_', ' ', '(', ')'
        };

        public static String GetRandomCharacters(Dictionary<char, string> table)
        {
            Random random = new Random();
            String result = "";
            bool leave = false;

            while (leave == false)
            {
                while (result.Length != MAX_ASSISTANT_CHAR)
                {
                    int temp = random.Next(START_CHAR_ASCII, END_CHAR_ASCII); //ASCII: ! - ~
                    if (IsValidCharacter(temp))
                        result += (char)temp;
                }

                if (IsValidCharacters(table, result))
                    leave = true;
                else
                    result = "";
            }
            return result;
        }

        static Boolean IsValidCharacter(int index)
        {
            int[] invalid = new int[]
            { 42, 47, 92, 58, 63, 34, 60, 62, 124, 46,
              SEPARATOR, BINDER };
            return !invalid.Contains(index);
        }

        static Boolean IsValidCharacters(Dictionary<char, string> table, string chars)
        {
            foreach (KeyValuePair<char, string> item in table)
            {
                if (chars.Contains(item.Value))
                    return false;
            }
            return true;
        }
    }
}
