using DAW.Models;
using System;


namespace DAW.Utils.Exceptions
{
    class MyException : Exception
    {
        public MyException() : base() {}
        public virtual ErrorModel GetError() { return null;}
    }
}
