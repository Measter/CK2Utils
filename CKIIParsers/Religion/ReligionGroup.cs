using System.Collections.Generic;
using Parsers.Options;

namespace Parsers.Religion
{
	public class ReligionGroup
	{
		public bool CoAOnBaronyOnly;
		public bool Playable;
		public bool Pacifist;
		public bool AIPeaceful;
		public bool HostileWithinGroup;

		public int AIConvertSameGroup;
		public int AIConvertOtherGroup;

		public string GraphicalCulture;
		public string CrusadeCB;
		public string Name;

		public List<string> MaleNames = new List<string>();
		public List<string> FemaleNames = new List<string>();

		public Dictionary<string, Religion> Religions = new Dictionary<string, Religion>();

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
			return string.Format( "Religion Group: {0}", Name );
		}
	}
}