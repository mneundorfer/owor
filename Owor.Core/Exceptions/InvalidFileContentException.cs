using System;

namespace Owor.Core.Exceptions
{

    public class InvalidFileContentException : Exception
    {

        public InvalidFileContentException(string message): base(message)
        {
            
        }

    }

}