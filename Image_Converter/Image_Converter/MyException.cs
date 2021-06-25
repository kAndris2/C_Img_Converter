using System;
using System.Collections.Generic;
using System.Text;

namespace Image_Converter
{
    class MyException : Exception
    {
        public MyException(string message) : base(message) { }
    }

    class InvalidInputException : MyException
    {
        public InvalidInputException(string message) : base(message) { }
    }

    class PathLengthException : MyException
    {
        public PathLengthException(string message) : base(message) { }
    }

    class EmptyFolderException : MyException
    {
        public EmptyFolderException(string message) : base(message) { }
    }

    class DuplicateException : MyException
    {
        public DuplicateException(string message) : base(message) { }
    }

    class InvalidKeyException : MyException
    {
        public InvalidKeyException(string message) : base(message) { }
    }

    class FileNotFoundException : MyException
    {
        public FileNotFoundException(string message) : base(message) { }
    }

    class InvalidRouteException : MyException
    {
        public InvalidRouteException(string message) : base(message) { }
    }

    class InvalidDecodeKeyException : MyException
    {
        public InvalidDecodeKeyException(string message) : base(message) { }
    }
}
