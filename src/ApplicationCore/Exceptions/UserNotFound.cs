using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;

namespace ApplicationCore.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string val, string key = "Id") : base($"UserNotFound. {key}: {val}")
        {

        }
    }
}
