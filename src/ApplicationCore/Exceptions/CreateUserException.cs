using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Exceptions
{
	public class CreateUserException : Exception
	{
		public CreateUserException(string message) : base(message)
		{

		}
	}
}
