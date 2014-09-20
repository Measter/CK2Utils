using System.Collections.Generic;
using Parsers.Options;

namespace Parsers.Trait
{
	public class Trait
	{
		public string ID;

		public bool Education;
		public bool Priest;
		public bool IsHealth;
		public bool IsIllness;
		public bool Customizer;
		public bool Congenital;
		public bool Incapacitating;
		public bool IsEpidemic;
		public bool Birth;
		public bool Inbred;
		public bool Lifestyle;
		public bool Personality;
		public bool Leader;
		public bool Cached;
		public bool Pilgrimage;
		public bool Agnatic;

		public int Intrigue;
		public int Stewardship;
		public int Martial;
		public int Diplomacy;
		public int Learning;
		public int AiZeal;
		public int VassalOpinion;
		public int SexAppealOpinion;
		public int SameOpinion;
		public int AiRationality;
		public int Cost;
		public int ChurchOpinion;
		public int SameOpinionIfSameReligion;
		public int TwinOpinion;
		public int SpouseOpinion;
		public int SameReligionOpinion;
		public int DynastyOpinion;
		public int OppositeOpinion;
		public int AiHonour;
		public int AiGreed;
		public int AiAmbition;
		public int LiegeOpinion;
		public int AmbitionOpinion;
		public int InfidelOpinion;
		public int LeadershipTraits;
		public int MuslimOpinion;

		public double Fertility;
		public double Health;
		public double MonthlyCharacterPiety;
		public double GlobalTaxModifier;
		public double MonthlyCharacterPrestige;

		public OptionGroup Potential;
		public OptionGroup CommandModifier;

		/// <summary>
		/// List of opposite trait IDs
		/// </summary>
		public List<string> OppositeIDList;
		/// <summary>
		/// List of opposite Traits.
		/// Is filled when traits are linked.
		/// </summary>
		public List<Trait> OppositeTraits = new List<Trait>();

		/// <summary>
		/// Used for storing data specific to the program.
		/// </summary>
		public Dictionary<string, object> CustomFlags = new Dictionary<string, object>();

		/// <summary>
		/// Contains any option groups that do not have a specified field.
		/// </summary>
		public List<OptionGroup> MiscOptionGroups = new List<OptionGroup>();
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
#if DEBUG
      return string.Format( "ID: {0}, Misc Options: {1}, Misc Option Groups: {2}", ID, MiscOptions.Count,
                            MiscOptionGroups.Count );
#endif
			return ID;
		}
	}
}