using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Image_Converter
{
    class Encrypt
    {
        readonly Dictionary<char, string> titleChars;

        public Encrypt(Dictionary<char, string> key)
        {
            titleChars = key;
        }

        Dictionary<char, string> bodyChars = new Dictionary<char, string>
        {
            {'R', "...<-.->!"}, {'4', "!<-.->..."}, {'L', "...<-.->..."}, {'g', "(^-^)"},
            {'F', "(0-0)"}, {'C', "(o-^)"}, {'H', "<-.->''"}, {'u', "(.)(.)"}, {'f', "8=D--"},
            {'X', "d(O_O)b"}, {' ', "(o_O)"}, {'n', "(^_^)"}, {'K', "(-.-)zzz"}, {'G', "(=^-^=)"}
        };

        public String EncodeFilename(string filename)
        {
            String GetBindedName(List<string> parts)
            {
                if (parts.Count >= 2)
                {
                    for (int i = 0; i < parts.Count; i++)
                    {
                        parts[i] = $"{Configurations.GetRandomCharacters(titleChars)}" +
                                   $"{parts[i]}" +
                                   $"{Configurations.GetRandomCharacters(titleChars)}";
                    }
                }
                return String.Join(Configurations.SEPARATOR, parts);
            }

            List<string> values = new List<string>();
            foreach(char c in filename)
            {
                if(Char.IsLower(c))
                {
                    values.Add(
                        Reverse(
                            titleChars[Char.ToUpper(c)]
                        )
                    );
                }
                else
                    values.Add(titleChars[c]);
            }
            return GetBindedName(values);
        }

        public String DecodeFilename(string filename)
        {
            List<string> GetParts(string[] data)
            {
                String GetPart(string item)
                {
                    String result = "";

                    for (int i = Configurations.MAX_ASSISTANT_CHAR; i < (item.Length - Configurations.MAX_ASSISTANT_CHAR); i++)
                    {
                        result += item[i];
                    }
                    return result;
                }

                List<string> parts = new List<string>();

                for(int i = 0; i < data.Length; i++)
                {
                    parts.Add(
                        GetPart(data[i])
                    );
                }
                return parts;
            }

            Boolean IsValueExists(string value)
            {
                foreach(KeyValuePair<char, string> item in titleChars)
                {
                    if (value.Equals(item.Value))
                        return true;
                }
                return false;
            }

            Boolean IsValidValues(List<string> values)
            {
                int count = 0;
                foreach(string value in values)
                {
                    foreach(KeyValuePair<char, string> item in titleChars)
                    {
                        if (value.Equals(item.Value) || Reverse(value).Equals(item.Value))
                            count++;
                    }
                }
                return count.Equals(values.Count);
            }

            List<string> values = GetParts(filename.Split(Configurations.SEPARATOR));
            String decode = "";
            String tempValue = "";
            char tempKey;
            bool exists;

            if (IsValidValues(values))
            {
                foreach (string value in values)
                {
                    exists = IsValueExists(value);
                    tempValue = exists ? value : Reverse(value);
                    tempKey = titleChars.FirstOrDefault(x => x.Value == tempValue).Key;
                    decode += exists ? tempKey : Char.ToLower(tempKey);
                }
            }
            else
                throw new InvalidDecodeKeyException("You are using an invalid key to decode");

            return decode;
        }

        public String GetEncodedCharacter(char c)
        {
            if(bodyChars.ContainsKey(c))
                return bodyChars[c];
            return c.ToString();
        }

        public String GetDecodedString(string rawData)
        {
            foreach(KeyValuePair<char, string> body in bodyChars)
            {
                rawData = rawData.Replace(body.Value, body.Key.ToString());
            }
            return rawData;
        }

        private String Reverse(string str)
        {
            char[] charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
