using System;

namespace Users.Model.CustomException
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}