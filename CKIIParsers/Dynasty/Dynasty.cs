using System.Collections.Generic;
using Parsers.Options;

namespace Parsers.Dynasty
{
	public class Dynasty
	{
		public int ID;

		public Culture.Culture Culture;
		public string CultureString;
		public string Name;

		public Option CoatOfArms;

		/// <summary>
		/// Contains any options that do not have a specified field.
		/// </summary>
		public List<Option> MiscOptions = new List<Option>();

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
			return string.Format( "Id: {0}, Name: {1}", ID, Name );
		}
	}
}