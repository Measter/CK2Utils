using System.Collections.Generic;
using Parsers.Options;

namespace Parsers.Event
{
	public class Event
	{
		public string EventType;
		public string PictureID;
		public string SoundID;
		public string Border;

		public string Description;
		public string Title;

		/// <summary>
		/// Dictionary of ID options. E.g. picture = GFX_evt_lovers
		/// Key is the option name. E.g. picture
		/// </summary>
		public Dictionary<string, string> IDOptions = new Dictionary<string, string>();
		/// <summary>
		/// Dictionary of Boolean options. E.g. only_rulers = yes
		/// Key is the option name. E.g. only_rulers
		/// </summary>
		public Dictionary<string, bool> BoolOptions = new Dictionary<string, bool>();
		/// <summary>
		/// Dictionary of String options. E.g. desc = "EVTDESC76000"
		/// Key is the option name. E.g. desc
		/// </summary>
		public Dictionary<string, string> StringOptions = new Dictionary<string, string>();
		/// <summary>
		/// Dictionary of Number options. E.g. min_age = 16
		/// Key is the option name. E.g. min_age
		/// </summary>
		public Dictionary<string, double> NumberOptions = new Dictionary<string, double>();

		/// <summary>
		/// Event ID if using namespaces. E.g. id = measter.2
		/// </summary>
		public string NamespaceID;
		/// <summary>
		/// Event ID. E.g. id = 2
		/// </summary>
		public int NumberID;

		public OptionGroup Triggers;
		public OptionGroup MeanTimeToHappen;
		public OptionGroup Immediate;
		public List<OptionGroup> Options = new List<OptionGroup>();

		/// <summary>
		/// Contains any option groups that do not have a specified field.
		/// </summary>
		public List<OptionGroup> MiscOptionGroups = new List<OptionGroup>();

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
			if( NamespaceID == null )
				return string.Format( "NumberId: {0}, Description: {1}, EventType: {2}", NumberID, Description, EventType );

			return string.Format( "NamespaceId: {0}, Description: {1}, EventType: {2}", NamespaceID, Description, EventType );
		}
	}
}