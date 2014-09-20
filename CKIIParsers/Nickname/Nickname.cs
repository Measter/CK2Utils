using System.Collections.Generic;
using Parsers.Options;

namespace Parsers.Nickname
{
	public class Nickname
	{
		public string ID;

		public OptionGroup Allows;
		public OptionGroup Chance;

		/// <summary>
		/// Used for storing data specific to the program.
		/// </summary>
		public Dictionary<string, object> CustomFlags = new Dictionary<string, object>();

		/// <summary>
		/// The file that the data was loaded from.
		/// </summary>
		public string Filename;

		public override string ToString()
		{
			return string.Format( "Id: {0}", ID );
		}
	}
}