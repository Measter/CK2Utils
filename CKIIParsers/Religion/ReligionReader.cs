using System;
using System.Collections.Generic;
using System.IO;
using Parsers.Options;
using Pdoxcl2Sharp;

namespace Parsers.Religion
{
	public class ReligionReader : ReaderBase
	{
		/// <summary>
		/// Dictionary of the loaded ReligionGroups.
		/// Key is the ID of the religion group. E.g. christian 
		/// </summary>
		public Dictionary<string, ReligionGroup> ReligionGroups;
		/// <summary>
		/// Dictionary of the loaded Religions.
		/// Key is the ID of the religion. E.g. catholic
		/// </summary>
		public Dictionary<string, Religion> Religions;

		private string m_lastFileName;

		public ReligionReader()
		{
			ReligionGroups = new Dictionary<string, ReligionGroup>();
			Religions = new Dictionary<string, Religion>();
			Errors = new List<string>();
		}

		/// <summary>
		/// Loads a single file of religions. Any errors encountered are stored in the Errors list.
		/// </summary>
		/// <param name="filename">Path of the file to load.</param>
		public override void Parse( string filename )
		{
			if( !File.Exists( filename ) )
			{
				Errors.Add( string.Format( "File not found: {0}", filename ) );
				return;
			}

			m_lastFileName = filename;

			using( FileStream fs = new FileStream( filename, FileMode.Open ) )
			{
				try
				{
					ParadoxParser.Parse( fs, ParseReligionGroup );

					LinkReligions();
				} catch( Exception ex )
				{
					Errors.Add( ex.ToString() );
				}
			}
		}

		/// <summary>
		/// Loads all the files in a folder for parsing. Any errors encountered are stored in the Errors list.
		/// </summary>
		/// <param name="folder">Path of the folder to load files from.</param>
		public override void ParseFolder( string folder )
		{
			DirectoryInfo dir = new DirectoryInfo( folder );

			if( !dir.Exists )
			{
				Errors.Add( "Unable to find folder: " + folder );
				return;
			}

			FileInfo[] files = dir.GetFiles( "*.txt" );

			foreach( FileInfo f in files )
			{
				Parse( f.FullName );
			}
		}

		private void LinkReligions()
		{
			foreach( var r in Religions )
			{
				if( r.Value.ParentString != null && Religions.ContainsKey( r.Value.ParentString ) )
					r.Value.Parent = Religions[r.Value.ParentString];
				if( r.Value.ReformedString != null && Religions.ContainsKey( r.Value.ReformedString ) )
					r.Value.Reformed = Religions[r.Value.ReformedString];
			}
		}

		private void ParseReligionGroup( ParadoxParser parser, string tag )
		{
			ReligionGroup rg = new ReligionGroup();
			rg.Name = tag;
			rg.Filename = m_lastFileName;

			if( ReligionGroups.ContainsKey( rg.Name ) )
				ReligionGroups.Remove( rg.Name );
			ReligionGroups[rg.Name] = rg;

			Action<ParadoxParser, string> getOptions = ( p, s ) =>
			{
				IList<string> stringList;

				switch( s )
				{
					case "graphical_culture":
						rg.GraphicalCulture = p.ReadString();
						break;
					case "crusade_cb":
						rg.CrusadeCB = p.ReadString();
						break;

					#region Bool Options
					case "has_coa_on_barony_only":
						rg.CoAOnBaronyOnly = p.ReadBool();
						break;
					case "playable":
						rg.Playable = p.ReadBool();
						break;
					case "pacifist":
						rg.Pacifist = p.ReadBool();
						break;
					case "ai_peaceful":
						rg.AIPeaceful = p.ReadBool();
						break;

					case "hostile_within_group":
						rg.HostileWithinGroup = p.ReadBool();
						break;
					#endregion

					case "male_names":
						stringList = p.ReadStringList();
						foreach( string name in stringList )
							rg.MaleNames.Add( name.Split( '_' )[0] );
						break;
					case "female_names":
						stringList = p.ReadStringList();
						foreach( string name in stringList )
							rg.FemaleNames.Add( name.Split( '_' )[0] );
						break;

					case "ai_convert_same_group":
						rg.AIConvertSameGroup = p.ReadInt32();
						break;
					case "ai_convert_other_group":
						rg.AIConvertOtherGroup = p.ReadInt32();
						break;

					default:
						ParseReligion( rg, p, s );
						break;
				}
			};

			parser.Parse( getOptions );
		}

		private void ParseReligion( ReligionGroup rg, ParadoxParser parser, string tag )
		{
			Religion rel = new Religion();
			rel.Name = tag;
			rel.Group = rg;
			rel.Filename = m_lastFileName;

			if( rg.Religions.ContainsKey( tag ) )
				rg.Religions.Remove( tag );
			rg.Religions[tag] = rel;

			if( Religions.ContainsKey( tag ) )
				Religions.Remove( tag );
			Religions[tag] = rel;

			Action<ParadoxParser, string> getOptions = ( p, s ) =>
			{
				IList<string> stringList;

				switch( s )
				{
					case "color":
						stringList = p.ReadStringList();
						rel.Colour = stringList.ParseColour();
						break;
					case "god_names":
						rel.GodNames.AddRange( p.ReadStringList() );
						break;
					case "evil_god_names":
						rel.EvilGodNames.AddRange( p.ReadStringList() );
						break;
					case "unit_modifier":
						Option.ParseGeneric( p, s );
						rel.UnitModifier = (GroupOption)Option.GetLastRoot();
						break;
					case "unit_home_modifier":
						Option.ParseGeneric( p, s );
						rel.UnitHomeModifier = (GroupOption)Option.GetLastRoot();
						break;
					case "character_modifier":
						Option.ParseGeneric( p, s );
						rel.CharacterModifier = (GroupOption)Option.GetLastRoot();
						break;

					#region ID Options
					case "graphical_culture":
						rel.GraphicalCulture = p.ReadString();
						break;
					case "scripture_name":
						rel.ScriptureName = p.ReadString();
						break;
					case "can_grant_invasion_cb":
						rel.CanGrantInvasionCB = p.ReadString();
						break;
					case "parent":
						rel.ParentString = p.ReadString();
						break;

					case "reformed":
						rel.ReformedString = p.ReadString();
						break;
					case "crusade_name":
						rel.CrusadeName = p.ReadString();
						break;
					case "priest_title":
						rel.PriestTitle = p.ReadString();
						break;
					case "intermarry":
						rel.InterMarry.Add( p.ReadString() );
						break;

					case "secondary_event_pictures":
						rel.SecondaryEventPictures = p.ReadString();
						break;
					case "piety_name":
						rel.PietyName = p.ReadString();
						break;
					case "expel_modifier":
						rel.ExpelModifier = p.ReadString();
						break;
					#endregion

					#region Number Options
					case "icon":
						rel.IconID = p.ReadInt32();
						break;
					case "heresy_icon":
						rel.HeresyIcon = p.ReadInt32();
						break;
					case "max_wives":
						rel.MaxWives = p.ReadInt32();
						break;
					case "religious_clothing_head":
						rel.ReligionClothingHead = p.ReadInt32();
						break;

					case "religious_clothing_priest":
						rel.ReligionClothingPriest = p.ReadInt32();
						break;
					case "max_consorts":
						rel.MaxConsorts = p.ReadInt32();
						break;
					case "short_reign_opinion_year_mult":
						rel.ShortReignOpinionYearMult = p.ReadInt32();
						break;
					case "aggression":
						rel.Aggression = p.ReadDouble();
						break;

					case "independence_war_score_bonus":
						rel.IndependenceWarScoreBonus = p.ReadInt32();
						break;
					case "ai_convert_other_group":
						rel.AIConvertOtherGroups = p.ReadInt32();
						break;
					case "peace_piety_gain":
						rel.PeacePietyGain = p.ReadDouble();
						break;
					#endregion

					#region Bool Options
					case "investiture":
						rel.Investiture = p.ReadBool();
						break;
					case "can_excommunicate":
						rel.CanExcommunicate = p.ReadBool();
						break;
					case "can_grant_divorce":
						rel.CanGrantDivorce = p.ReadBool();
						break;
					case "can_grant_claim":
						rel.CanGrantClaim = p.ReadBool();
						break;

					case "can_call_crusade":
						rel.CanCallCrusade = p.ReadBool();
						break;
					case "priests_can_marry":
						rel.PriestsCanMarry = p.ReadBool();
						break;
					case "psc_marriage":
						rel.PSCMarriage = p.ReadBool();
						break;
					case "autocephaly":
						rel.Autocephaly = p.ReadBool();
						break;

					case "defensive_attrition":
						rel.DefensiveAttrition = p.ReadBool();
						break;
					case "allow_viking_invasion":
						rel.AllowVikingInvasion = p.ReadBool();
						break;
					case "allow_looting":
						rel.AllowLooting = p.ReadBool();
						break;
					case "allow_rivermovement":
						rel.AllowRiverMovement = p.ReadBool();
						break;

					case "female_temple_holders":
						rel.FemaleTempleHolders = p.ReadBool();
						break;
					case "peace_prestige_loss":
						rel.PeacePrestigeLoss = p.ReadBool();
						break;
					case "raised_vassal_opinion_loss":
						rel.RaisedVassalOpinionLoss = p.ReadBool();
						break;
					case "reformer_head_of_religion":
						rel.ReformerHeadOfReligion = p.ReadBool();
						break;

					case "divine_blood":
						rel.DivineBlood = p.ReadBool();
						break;
					case "pc_marriage":
						rel.PCMarriage = p.ReadBool();
						break;
					case "priests_can_inherit":
						rel.PriestsCanInherit = p.ReadBool();
						break;

					case "pacifist":
						rel.Pacifist = p.ReadBool();
						break;
					case "feminist":
						rel.Feminist = p.ReadBool();
						break;
					case "bs_marriage":
						rel.BSMarriage = p.ReadBool();
						break;
					case "pre_reformed":
						rel.PreReformed = p.ReadBool();
						break;

					case "matrilineal_marriages":
						rel.MatrilinealMarriages = p.ReadBool();
						break;
					case "can_hold_temples":
						rel.CanHoldTemples = p.ReadBool();
						break;
					case "can_retire_to_monastery":
						rel.CanRetireToMonastery = p.ReadBool();
						break;
					case "can_have_antipopes":
						rel.CanHaveAntipopes = p.ReadBool();
						break;

					case "has_heir_designation":
						rel.HasHeirDesignation = p.ReadBool();
						break;
					#endregion

					default:
						Option.ParseGeneric( p, s );
						rel.MiscOptions.Add( Option.GetLastRoot() );
						break;
				}
			};

			parser.Parse( getOptions );
		}
	}
}
