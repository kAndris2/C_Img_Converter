using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Image_Converter
{
    class Picture
    {
        public byte[] byteMap { get; }
        public String filename { get; }

        public Picture(byte[] byteMap, string filename)
        {
            this.byteMap = byteMap;
            this.filename = filename;
        }
    }
}
