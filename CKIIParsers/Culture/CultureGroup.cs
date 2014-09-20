using System.Collections.Generic;
using Parsers.Options;

namespace Parsers.Culture
{
	public class CultureGroup
	{
		public string Name;
		public string GraphicalCulture;
		public Dictionary<string, Culture> Cultures = new Dictionary<string, Culture>();

		/// <summary>
		/// Used for storing data specific to the program.
		/// </summary>
		public Dictionary<string, object> CustomFlags = new Dictionary<string, object>();

		/// <summary>
		/// Contains any options that do not have a specified field.
		/// </summary>
		public List<Option> MiscOptions = new List<Option>();

		/// <summary>
		/// The file that the data was loaded from.
		/// </summary>
		public string Filename;

		public override string ToString()
		{
			return Name;
		}
	}
}