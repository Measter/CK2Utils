using System.Collections.Generic;
using System.Drawing;
using Parsers.Options;

namespace Parsers.Religion
{
	public class Religion
	{
		public string Name;
		public string GraphicalCulture;
		public string SecondaryEventPictures;
		public string CrusadeName;
		public string ScriptureName;
		public string PriestTitle;
		public string CanGrantInvasionCB;
		public string PietyName;
		public string ExpelModifier;

		public List<string> GodNames = new List<string>();
		public List<string> EvilGodNames = new List<string>();
		public List<string> InterMarry = new List<string>();

		public Color Colour;

		public int IconID;
		public int MaxWives = 1;
		public int HeresyIcon;
		public int ReligionClothingHead;
		public int ReligionClothingPriest;
		public int MaxConsorts;
		public int ShortReignOpinionYearMult = 2;
		public int IndependenceWarScoreBonus;
		public int AIConvertOtherGroups;

		public double Aggression;
		public double PeacePietyGain;

		public bool CanHaveAntipopes;
		public bool Feminist;
		public bool PreReformed;
		public bool MatrilinealMarriages = true;
		public bool CanHoldTemples;
		public bool CanRetireToMonastery;
		public bool PriestsCanInherit = true;
		public bool Pacifist;
		public bool HasHeirDesignation;
		public bool Investiture;
		public bool CanExcommunicate;
		public bool CanGrantDivorce;
		public bool CanGrantClaim;
		public bool CanCallCrusade;
		public bool PriestsCanMarry;
		public bool PSCMarriage;
		public bool Autocephaly;
		public bool DefensiveAttrition;
		public bool AllowVikingInvasion;
		public bool AllowLooting;
		public bool AllowRiverMovement;
		public bool FemaleTempleHolders;
		public bool PeacePrestigeLoss;
		public bool RaisedVassalOpinionLoss = true;
		public bool ReformerHeadOfReligion;
		public bool DivineBlood;
		public bool PCMarriage;
		public bool BSMarriage;

		/// <summary>
		/// Links to the parent religion of a heresy.
		/// </summary>
		public Religion Parent;	   
		public string ParentString;
  		/// <summary>
  		/// Links to the reformed version of the religion.
  		/// </summary>
		public Religion Reformed;
		public string ReformedString;

		public ReligionGroup Group;

		public GroupOption UnitModifier;
		public GroupOption UnitHomeModifier;
		public GroupOption CharacterModifier;

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
			return string.Format( "Religion: {0}", Name );
		}
	}
}