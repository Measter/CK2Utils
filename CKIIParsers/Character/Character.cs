using System;
using System.Collections.Generic;
using Parsers.Options;

namespace Parsers.Character
{
	public class Character
	{
		public int ID;

		public int Dynasty;
		public int Martial;
		public int Diplomacy;
		public int Intrigue;
		public int Stewardship;
		public int Learning;

		/// <summary>
		/// ID of the character's father.
		/// </summary>
		public int FatherID;
		/// <summary>
		/// ID of the character's Mother.
		/// </summary>
		public int MotherID;
		/// <summary>
		/// ID of the character's employer.
		/// </summary>
		public int EmployerID;
		/// <summary>
		/// ID of the character's spouse.
		/// </summary>
		public int CurrentSpouseID;

		/// <summary>
		/// IDs of the previous spouses.
		/// </summary>
		public List<int> PrevSpousesID = new List<int>();

		public bool IsFemale;

		public string Religion;
		public string Culture;
		public string Nickname;
		public string DNA;
		public string Properties;
		public string Name;

		public List<string> TraitList = new List<string>();
		public List<string> Claims = new List<string>();

		/// <summary>
		/// List of events such as birth and death.
		/// </summary>
		public List<EventOption> Events = new List<EventOption>();

		/// <summary>
		/// Contains any options that do not have a specified field.
		/// </summary>
		public List<Option> MiscOptions = new List<Option>();

		/// <summary>
		/// Links to character's mother.
		/// Only filled if characters are linked.
		/// </summary>
		public Character Mother;
		/// <summary>
		/// Links to character's father.
		/// Only filled if characters are linked.
		/// </summary>
		public Character Father;
		/// <summary>
		/// Links to character's employer.
		/// Only filled if characters are linked.
		/// </summary>
		public Character Employer;
		/// <summary>
		/// Links to the current spouse.
		/// Only filled if characters are linked.
		/// </summary>
		public Character CurrentSpouse;

		/// <summary>
		/// Links to the previous spouses.
		/// Only filled if characters are linked.
		/// </summary>
		public List<Character> PrevSpouses = new List<Character>();
		/// <summary>
		/// Links to the character's children.
		/// Only filled if characters are linked.
		/// </summary>
		public List<Character> Children = new List<Character>();
		/// <summary>
		/// Links to the character's siblings.
		/// Only filled if characters are linked.
		/// </summary>
		public List<Character> Siblings = new List<Character>();

		/// <summary>
		/// Used for storing data specific to the program.
		/// </summary>
		public Dictionary<string, Object> CustomFlags = new Dictionary<string, object>();

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
