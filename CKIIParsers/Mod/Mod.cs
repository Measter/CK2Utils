using System.Collections.Generic;

namespace Parsers.Mod
{
	public class Mod
	{
		public string Name = string.Empty;
		public string ModFile = string.Empty;
		public string Path = string.Empty;
		public string UserDir = string.Empty;
		public ModReader.Folder ModPathType;
		public List<string> Dependencies = new List<string>();
		public List<string> Extends = new List<string>();
		public List<string> Replaces = new List<string>();

		/// <summary>
		/// Used for storing data specific to the program.
		/// </summary>
		public Dictionary<string, object> CustomFlags = new Dictionary<string, object>();

		public override string ToString()
		{
			return Name;
		}
	}
}