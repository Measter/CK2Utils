using System;
using System.Collections.Generic;
using System.Drawing;
using Parsers.Options;

namespace Parsers.Culture
{
	public class Culture
	{
		public string Name;
		public string GraphicalCulture;
		public string SecondGraphicalCulture;
		public string GraphicalUnitCulture;
		public Color Colour;

		public bool IsHorde;
		public bool FounderNamedDynasties;
		public bool DynastyTitleNames;
		public bool DukesCalledKings;
		public bool BaronTitlesHidden;
		public bool CountTitlesHidden;
		public bool DisinheritFromBlinding;
		public bool UsedForRandom = true;

		public string GrammarTransform;

		public string FromDynastyPrefix;
		public string BastardDynastyPrefix;
		
		public string MalePatronym;
		public string FemalePatronym;
		public bool IsPrefix;

		public int PaternalGrandfatherChance;
		public int MaternalGrandfatherChance;
		public int FatherChance;

		public int PaternalGrandmotherChance;
		public int MaternalGrandmotherChance;
		public int MotherChance;

		public string Modifier;

		public List<string> MaleNames = new List<string>();
		public List<string> FemaleNames = new List<string>();
		public CultureGroup Group;
		public Culture Parent;

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
			return Name;
		}

		public string GetRandomName( bool isFemale, Random rand )
		{
			if ( isFemale )
				return FemaleNames.RandomItem( rand ).Split( '_' ).RandomItem( rand );
			return MaleNames.RandomItem( rand ).Split( '_' ).RandomItem( rand );
		}
	}
}