using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Image_Converter
{
    class FileManager
    {
        private readonly DataManager _data;
        private readonly ConsoleLogger _logger;
        private readonly Zip _zip;
        private readonly PasswordService _passwordService = new PasswordService();

        int _success, _failure;

        public enum FileType
        {
            Map, Image
        }

        public enum Mode
        {
            Add, Default
        }

        public FileManager(Dictionary<char, string> key, ConsoleLogger logger)
        {
            _data = new DataManager(key);
            _logger = logger;
            _zip = new Zip(logger);
        }

        public FileManager() { }

        public void Find(string route, FileType type, Mode mode = Mode.Default, string zipLocation = null)
        {
            if (mode is Mode.Default)
            {
                if (type == FileType.Image)
                {
                    FindPictures(route, type);
                    ArchiveFiles($"{route}\\{Configurations.ENCODE_FOLDER_NAME}", route);
                }
                else if (type == FileType.Map)
                {
                    string mapFiles = $"{route}\\{Configurations.TEMP_EXTRACT_FOLDER}";
                    FindMaps(route, type, mapFiles);
                    ManageFolders(mapFiles, route);
                }
            }
            else if(mode is Mode.Add && type is FileType.Image)
            {
                FindPictures(route, type);
                AddFilesToArchive($"{route}\\{Configurations.ENCODE_FOLDER_NAME}", zipLocation);
            }

            PrintResult(_success, _failure);
        }

        void FindMaps(string route, FileType type, string mapFiles) //decode
        {
            _zip.Extract(route);

            IEnumerable<string> files = Directory.GetFiles(mapFiles, "*", SearchOption.TopDirectoryOnly)
                .Select(f => Path.GetFileName(f))
                .Where(f => IsMapFile(f));

            if (files.Count() == 0)
                throw new EmptyFolderException($"This folder is empty! - ({mapFiles})");

            foreach (String file in files)
            {
                if (WorkWithFile(mapFiles, file, type))
                    _success++;
                else
                    _failure++;
            }

            Console.WriteLine();
            _logger.Info("The file creation just started!");
            Console.WriteLine();

            _data.SetLogger(_logger);
            _data.CreatePictures(mapFiles);
        }

        void FindPictures(string route, FileType type, Mode mode = Mode.Default) //encode
        {
            String GetFileName(string path)
            {
                return path.Split("\\")[^1];
            }

            IEnumerable<string> files = Directory.GetFiles(route, "*", SearchOption.AllDirectories)
                .Where(f => IsPicture(f));
            List<string> duplicates = CountDuplicates(route);

            if (duplicates.Count == 0)
            {
                if (files.Count() == 0)
                    throw new EmptyFolderException($"This folder is empty! - ({route})");

                List<string> tempNames = new List<string>();

                foreach (String file in files)
                {
                    string fileName = GetFileName(file);

                    if (!tempNames.Contains(fileName))
                    {
                        if (WorkWithFile(route, file, type))
                            _success++;
                        else
                            _failure++;

                        tempNames.Add(fileName);
                    }
                }
            }
            else
                throw new DuplicateException($"We have been found {duplicates.Count}pc of duplicate(s)!");
        }

        void PrintResult(int success, int failure)
        {
            Console.WriteLine();
            _logger.Info($"Total files found: {success + failure}\n" +
                         $"\tSuccess: {success}\n" +
                         $"\tFailure: {failure}");
        }

        bool WorkWithFile(string route, string file, FileType type)
        {
            try
            {
                string result = type == FileType.Image
                    ?
                    _data.Save(route, file) 
                    :
                    _data.Prepare(route, file);

                _logger.Success
                (
                    type == FileType.Image 
                        ?
                        $"{_data.GetFilename(file)} encoded to {result}"
                        :
                        $"{result} just prepared for the file creation!"
                );
                return true;
            }
            catch (Exception e)
            {
                _logger.Failure($"{e.Message}: {file}");
                return false;
            }
        }

        Boolean IsPicture(string file)
        {
            string[] extensions = new string[]
            {
                "png", "jpg", "jpeg", "bmp"
            };

            string[] temp = file.Split(".");
            return extensions.Contains(temp[^1]);
        }

        Boolean IsMapFile(string file)
        {
            string[] extensions = new string[] { "txt" };
            return extensions.Contains(file.Split(".")[1]);
        }

        public List<string> CountDuplicates(string route)
        {
            IEnumerable<string> files = Directory.GetFiles(route, "*", SearchOption.TopDirectoryOnly).Select(f => Path.GetFileName(f));
            List<string> fileNames = new List<string>();
            List<string> duplicates = new List<string>();

            foreach(string file in files)
            {
                string f = _data.GetFilenameWithoutExtension(file);
                if (!fileNames.Contains(f))
                    fileNames.Add(f);
                else
                    duplicates.Add(f);
            }
            return duplicates;
        }

        void ManageFolders(string source, string target)
        {
            Directory.Move(
                $"{source}\\{Configurations.DECODE_FOLDER_NAME}",
                $"{target}\\{Configurations.DECODE_FOLDER_NAME}"
            );
            Directory.Delete(source, true);
        }

        private void ArchiveFiles(string filesLocation, string output)
        {
            Console.WriteLine("\nPlease enter the zip file's password:");
            string password = _passwordService.GetPassword();
            string vPass = "";

            while(vPass != password)
            {
                Console.WriteLine("\nPlease re-enter the password!");
                vPass = _passwordService.GetPassword();

                if (vPass != password)
                {
                    Console.WriteLine();
                    _logger.Error("The re-entered password is incorrect!");
                }
            }

            Console.WriteLine();
            _zip.Archive(filesLocation, output, password);
            Directory.Delete(filesLocation, true);
        }

        private void AddFilesToArchive(string filesLocation, string zipLocation)
        {
            _zip.AddFilesToArchive(filesLocation, zipLocation);
            Directory.Delete(filesLocation, true);
        }
    }
}