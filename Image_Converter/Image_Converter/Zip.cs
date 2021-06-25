using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Image_Converter
{
    class Zip
    {
        readonly String ZIP_FILE_NAME = Configurations.ENCODE_FOLDER_NAME + ".zip";

        ConsoleLogger _logger;

        public Zip(ConsoleLogger logger)
        {
            _logger = logger;
        }

        public void AddFilesToArchive(string filesLocation, string zipLocation)
        {
            ZipFile _zip = ZipFile.Read($"{zipLocation}\\{ZIP_FILE_NAME}");
            _zip.AddFiles
            (
                Directory.EnumerateFiles(filesLocation),
                false,
                ""
            );
            _zip.Save();
        }

        public void Archive(string filesLocation, string output, string password)
        {
            Console.WriteLine();
            _logger.Info("Please be patient, the files are being archived...");
            using (ZipFile zip = new ZipFile())
            {
                zip.Password = password;
                zip.AddDirectory(filesLocation);
                zip.Save(output + $"\\{ZIP_FILE_NAME}");
            }
        }

        public void Extract(string zipLocation)
        {
            PasswordService _passwordService = new PasswordService();
            ZipFile zip = ZipFile.Read($"{zipLocation}\\{ZIP_FILE_NAME}");
            bool check = false;

            while (!check)
            {
                Console.WriteLine("Please enter the password:");
                string password = _passwordService.GetPassword();
                int count = 0;

                try
                {
                    foreach (ZipEntry item in zip)
                    {
                        item.ExtractWithPassword(
                            $"{zipLocation}\\{Configurations.TEMP_EXTRACT_FOLDER}", 
                            ExtractExistingFileAction.OverwriteSilently, 
                            password
                        );

                        count++;
                        Console.Clear();
                        _logger.Success($"File extracted: {count}");

                        if (!check) 
                            check = true;
                    }
                    Console.Clear();
                }
                catch (BadPasswordException e)
                {
                    Console.WriteLine();
                    _logger.Error(e.Message);
                }
            }
        }
    }
}