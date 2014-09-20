using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Parsers.Options;

namespace Parsers.Title
{
	public class Title
	{
		public string TitleID = null;
		public string Culture = null;
		public string Religion = null;
		public string FoA = null;
		public string TitlePrefix = null;
		public string CharTitle = null;
		public string CharTitleFemale = null;
		public string ControlsReligion = null;
		public string Modifier = null;

		public int Capital = -1;
		public int Dignity = -1;
		public int CountyID = -1;

		public double StrengthGrowthPerCentury = -1;

		public Color Colour1 = Color.Black;
		public Color Colour2 = Color.Black;

		public bool Landless = false;
		public bool Primary = false;
		public bool TwoColours = false;
		public bool Rebel = false;
		public bool Tribe = false;
		public bool Pirate = false;
		public bool ShortName = false;
		public bool LocationRulerTitle = false;
		public bool Caliphate = false;
		public bool HolyOrder = false;
		public bool Independent = false;
		public bool Mercenary = false;
		public bool DuchyRevokation = false;
		public bool DynastyTitleNames = false;
		public bool IsTitular = false;
		public bool CreationRequiresCapital = true;
		public bool PurpleBornHeirs = false;
		public bool HasTopDeJureCapital = false;
		public bool TopDeJureCapital = false;
		public bool Pentarchy = false;
		public bool UsedForDynastyNames = true;
		public bool Assimilate = true;


		/// <summary>
		/// Links to the parent de jure title.
		/// E.g. k_england is the parent of d_york.
		/// </summary>
		public Title Parent;
		/// <summary>
		/// Filtered history state of the title.
		/// </summary>
		public ReadOnlyCollection<Option> History;

		public GroupOption Allows;
		public GroupOption GainEffect;
		/// <summary>
		/// Contains any options that do not have a specified field.
		/// </summary>
		public List<Option> MiscOptions = new List<Option>();
		/// <summary>
		/// Contains the Coat of Arms definition for pagans.
		/// </summary>
		public GroupOption PaganCoA;

		/// <summary>
		/// List of de jure sub titles.
		/// E.g. d_york is a sub title of k_england.
		/// </summary>
		public Dictionary<string, Title> SubTitles = new Dictionary<string, Title>();

		/// <summary>
		/// List of religions that consider this a holy site.
		/// </summary>
		public List<string> HolySite = new List<string>();

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
			if( CountyID > 0 == false )
				return string.Format( "Title: {0}", TitleID );

			return string.Format( "Title: {0}, Id: {1}", TitleID, CountyID );
		}

		public Title ShallowCopy()
		{
			Title copy = (Title)this.MemberwiseClone();

			return copy;
		}
	}
}