using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Measter;
using Parsers.Options;
using Pdoxcl2Sharp;

namespace Parsers.Title
{
	public class TitleReader : ReaderBase
	{
		/// <summary>
		/// Dictionary of loaded Empires.
		/// Key is the ID of the empire. E.g. e_hre 
		/// </summary>
		public Dictionary<string, Title> Empires;
		/// <summary>
		/// Dictionary of loaded Kingdoms.
		/// Key is the ID of the kingdom. E.g. k_frisia
		/// </summary>
		public Dictionary<string, Title> Kingdoms;
		/// <summary>
		/// Dictionary of loaded Duchies.
		/// Key is the ID of the duchy. E.g. d_holland
		/// </summary>
		public Dictionary<string, Title> Duchies;
		/// <summary>
		/// Dictionary of loaded Counties.
		/// Key is the ID of the county. E.g. c_sticht
		/// </summary>
		public Dictionary<string, Title> Counties;
		/// <summary>
		/// Dictionary of loaded Baronies
		/// Key is the ID of the barony. E.g. b_tholen
		/// </summary>
		public Dictionary<string, Title> Baronies;

		private List<string> m_titlePrefixes = new List<string> { "e_", "k_", "d_", "c_", "b_" };
		private string m_lastFileName;
		private Stack<Title> m_parentStack;

		public TitleReader()
		{
			Empires = new Dictionary<string, Title>();
			Kingdoms = new Dictionary<string, Title>();
			Duchies = new Dictionary<string, Title>();
			Counties = new Dictionary<string, Title>();
			Baronies = new Dictionary<string, Title>();
			Errors = new List<string>();
			m_parentStack = new Stack<Title>();
		}

		/// <summary>
		/// Loads a single file of landed titles.
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
					ParadoxParser.Parse( fs, ParseTitle );

					CheckLandless();
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

		private void CheckLandless()
		{
			foreach( var pair in Counties )
				pair.Value.IsTitular = pair.Value.SubTitles.Count == 0;
			foreach( var pair in Duchies )
				pair.Value.IsTitular = pair.Value.SubTitles.Count == 0;
			foreach( var pair in Kingdoms )
				pair.Value.IsTitular = pair.Value.SubTitles.Count == 0;
			foreach( var pair in Empires )
				pair.Value.IsTitular = pair.Value.SubTitles.Count == 0;
		}

		private void ParseTitle( ParadoxParser parser, string tag )
		{
			Title t = new Title();
			t.Filename = m_lastFileName;
			t.TitleID = tag;

			if( m_parentStack.Count > 0 )
			{
				t.Parent = m_parentStack.Peek();
				t.Parent.SubTitles.Add( tag, t );
			}

			Action<ParadoxParser, string> getOptions = ( p, s ) =>
			{
				IList<string> stringList;

				switch( s )
				{
					case "color":
						stringList = p.ReadStringList();
						t.Colour1 = stringList.ParseColour();
						break;
					case "color2":
						stringList = p.ReadStringList();
						t.Colour2 = stringList.ParseColour();
						t.TwoColours = true;
						break;
					case "male_names":
					case "female_names":
						t.MiscOptions.Add( new StringListOption( s, p.ReadStringList() ) );
						break;

					case "allow":
						Option.ParseGeneric( p, s );
						t.Allows = (GroupOption)Option.GetLastRoot();
						break;
					case "gain_effect":
						Option.ParseGeneric( p, s );
						t.GainEffect = (GroupOption)Option.GetLastRoot();
						break;
					case "pagan_coa":
						Option.ParseGeneric( p, s );
						t.PaganCoA = (GroupOption)Option.GetLastRoot();
						break;

					#region ID
					case "culture":
						t.Culture = p.ReadString();
						break;
					case "controls_religion":
						t.ControlsReligion = p.ReadString();
						break;
					case "religion":
						t.Religion = p.ReadString();
						break;
					case "modifier":
						t.Modifier = p.ReadString();
						break;

					case "holy_site":
						string val = p.ReadString();
						if( !t.HolySite.Contains( val ) )
							t.HolySite.Add( val );
						break;
					#endregion

					#region Bools
					case "rebel":
						t.Rebel = p.ReadBool();
						break;
					case "landless":
						t.Landless = p.ReadBool();
						break;
					case "primary":
						t.Primary = p.ReadBool();
						break;
					case "tribe":
						t.Tribe = p.ReadBool();
						break;

					case "pirate":
						t.Pirate = p.ReadBool();
						break;
					case "short_name":
						t.ShortName = p.ReadBool();
						break;
					case "location_ruler_title":
						t.LocationRulerTitle = p.ReadBool();
						break;
					case "caliphate":
						t.Caliphate = p.ReadBool();
						break;

					case "holy_order":
						t.HolyOrder = p.ReadBool();
						break;
					case "independent":
						t.Independent = p.ReadBool();
						break;
					case "mercenary":
						t.Mercenary = p.ReadBool();
						break;
					case "duchy_revokation":
						t.DuchyRevokation = p.ReadBool();
						break;

					case "dynasty_title_names":
						t.DynastyTitleNames = p.ReadBool();
						break;
					case "creation_requires_capital":
						t.CreationRequiresCapital = p.ReadBool();
						break;
					case "purple_born_heirs":
						t.PurpleBornHeirs = p.ReadBool();
						break;
					case "has_top_de_jure_capital":
						t.HasTopDeJureCapital = p.ReadBool();
						break;

					case "top_de_jure_capital":
						t.TopDeJureCapital = p.ReadBool();
						break;
					case "pentarchy":
						t.Pentarchy = p.ReadBool();
						break;
					case "used_for_dynasty_names":
						t.UsedForDynastyNames = p.ReadBool();
						break;
					case "assimilate":
						t.Assimilate = p.ReadBool();
						break;
					#endregion

					#region Numbers
					case "capital":
						t.Capital = p.ReadInt32();
						break;
					case "strength_growth_per_century":
						t.StrengthGrowthPerCentury = p.ReadDouble();
						break;
					case "dignity":
						t.Dignity = p.ReadInt32();
						break;
					#endregion

					#region String
					case "title":
						t.CharTitle = '"' + p.ReadString() + '"';
						break;
					case "foa":
						t.FoA = '"' + p.ReadString() + '"';
						break;
					case "title_prefix":
						t.TitlePrefix = '"' + p.ReadString() + '"';
						break;
					case "title_female":
						t.CharTitleFemale = '"' + p.ReadString() + '"';
						break;
					#endregion

					default:
						if( m_titlePrefixes.Contains( s.Substring( 0, 2 ) ) )
						{
							// Subtitle
							m_parentStack.Push( t );
							ParseTitle( parser, s );
							m_parentStack.Pop();
						} else
						{
							// Other unhandle data
							Option.ParseGeneric( p, s );
							t.MiscOptions.Add( Option.GetLastRoot() );
						}
						break;
				}
			};

			parser.Parse( getOptions );

			if( t.TitleID.StartsWith( "e_" ) )  //Empire
			{
				InsertTitle( t, Empires );
			} else if( t.TitleID.StartsWith( "k_" ) )  //Kingdom
			{
				InsertTitle( t, Kingdoms, true, Empires );
			} else if( t.TitleID.StartsWith( "d_" ) )  //Duchy
			{
				InsertTitle( t, Duchies, true, Kingdoms );
			} else if( t.TitleID.StartsWith( "c_" ) )  //County
			{
				InsertTitle( t, Counties, true, Duchies );
			} else if( t.TitleID.StartsWith( "b_" ) ) // Barony
			{
				InsertTitle( t, Baronies, true, Counties );
			}
		}

		private void InsertTitle( Title t, Dictionary<string, Title> thisTier, bool searchHigherTier = false, Dictionary<string, Title> higherTier = null )
		{
			if( thisTier.ContainsKey( t.TitleID ) )
				thisTier.Remove( t.TitleID );

			if( searchHigherTier && higherTier != null )
			{
				foreach( KeyValuePair<string, Title> hi in higherTier )
				{
					if( hi.Value.SubTitles.ContainsKey( t.TitleID ) )
						hi.Value.SubTitles.Remove( t.TitleID );
				}
			}

			thisTier.Add( t.TitleID, t );
		}


		public void LinkCounties( Dictionary<int, Province.Province> provinces )
		{
			foreach( KeyValuePair<string, Title> pair in Counties )
			{
				Title c = pair.Value;
				Province.Province prov = provinces.ToList().Find( p => p.Value.Title == c.TitleID ).Value;

				if( prov == null )
					continue;

				c.CountyID = prov.ID;
				c.Capital = c.CountyID;
			}
		}
	}
}
