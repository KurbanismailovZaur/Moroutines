using System;

namespace Redcode.Moroutines.Exceptions
{
	public class PlayControlException : ApplicationException
	{
		public PlayControlException(string message) : base(message) { }
	}
}