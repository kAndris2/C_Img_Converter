using System;
using System.Collections.Generic;
using System.IO;

namespace Image_Converter
{
    class DataManager
    {
        ConsoleLogger _logger;

        readonly Encrypt _encrypt;
        readonly List<Picture> pictures = new List<Picture>();

        const int MAX_PATH_LENGTH = 260;

        public DataManager(Dictionary<char, string> key)
        {
            _encrypt = new Encrypt(key);
        }

        //route = external folder | fileRoute = file sub folder path
        public String Save(string route, string fileRoute)
        {
            byte[] fileMap = File.ReadAllBytes(fileRoute);

            String path = $"{route}\\{Configurations.ENCODE_FOLDER_NAME}";
            CreateDirectory(path);

            String filename = _encrypt.EncodeFilename(
                                    GetFilenameWithoutExtension(
                                        GetFilename(fileRoute)
                                    )
                              )
                              + ".txt";
            String destination = $"{path}\\{filename}";

            if (destination.Length <= MAX_PATH_LENGTH)
            {
                using (StreamWriter sw = new StreamWriter(destination))
                {
                    foreach (byte b in fileMap)
                    {
                        sw.Write(
                            _encrypt.GetEncodedCharacter((char)b)
                        );
                    }
                }
            }
            else
                throw new PathLengthException($"This path is too long! - ({destination.Length}/{MAX_PATH_LENGTH})");
            return filename;
        }

        public String Prepare(string route, string file)
        {
            byte[] fileMap = GetByteMap(route, file);

            String path = $"{route}\\{Configurations.DECODE_FOLDER_NAME}";
            CreateDirectory(path);

            String filename = $"{_encrypt.DecodeFilename(GetFilenameWithoutExtension(file))}.jpg";
            String destinationPath = $"{path}\\{GetRawFilename(filename)}";

            CreateDirectory(destinationPath);

            pictures.Add(
                new Picture(
                    fileMap,
                    filename
                )
            );
            return filename;
        }

        public void CreatePictures(string route)
        {
            String GetFolderName(string dir)
            {
                return dir.Split("\\")[^1];
            }

            Boolean BelongsToFolder(string filename, string folder)
            {
                const char HYPHEN = '-';
                filename = String.Join("", filename.Split('.')[0]);

                if(filename.Contains(HYPHEN))
                {
                    foreach(string part in filename.Split(HYPHEN))
                    {
                        if (part.Equals(folder))
                            return true;
                    }
                }
                return filename.Equals(folder);
            }

            string path = $"{route}\\{Configurations.DECODE_FOLDER_NAME}";
            foreach (Picture picture in pictures)
            {
                foreach (String directory in Directory.GetDirectories(path))
                {
                    string folder = GetFolderName(directory);
                    if (BelongsToFolder(picture.filename, folder))
                    {
                        try
                        {
                            string destination = $"{path}\\{folder}\\{picture.filename}";
                            using (FileStream fs = new FileStream(destination, FileMode.Create))
                            {
                                for (int i = 0; i < picture.byteMap.Length; i++)
                                {
                                    fs.WriteByte(picture.byteMap[i]);
                                }
                                fs.Seek(0, SeekOrigin.Begin);
                            }
                            _logger.Success($"{picture.filename} successfully created at {path}\\{folder}");
                        }
                        catch (Exception e)
                        {
                            _logger.Failure($"{e.Message}: {picture.filename}");
                        }
                    }
                }
            }
        }

        public String GetFilenameWithoutExtension(string filename)
        {
            List<String> parts = new List<string>(filename.Split("."));
            parts.RemoveAt(parts.Count - 1);
            return String.Join(".", parts);
        }

        public void SetLogger(ConsoleLogger logger)
        {
            _logger = logger;
        }

        private byte[] GetByteMap(string route, string file)
        {
            List<byte> bytes = new List<byte>();
            String rawData = _encrypt.GetDecodedString(File.ReadAllText($"{route}\\{file}"));

            foreach (char c in rawData)
            {
                bytes.Add((byte) c);
            }
            return bytes.ToArray();
        }

        String GetRawFilename(string filename)
        {
            return filename.Split(".")[0];
        }

        void CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch(ArgumentException)
            {
                throw new InvalidRouteException($"Invalid characters in the route! - ('{path}')");
            }
        }

        public String GetFilename(string file)
        {
            return file.Split("\\")[^1];
        }
    }
}