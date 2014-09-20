using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Parsers.Options;
using Pdoxcl2Sharp;

namespace Parsers.Culture
{
	public class CultureReader : ReaderBase
	{
		/// <summary>
		/// Dictionary of loaded CultureGroups.
		/// Key is the ID of the culture group. E.g. north_germanic
		/// </summary>
		public Dictionary<string, CultureGroup> CultureGroups;
		/// <summary>
		/// Dictionary of loaded Cultures.
		/// Key is the ID of the culture. E.g. swedish
		/// </summary>
		public Dictionary<string, Culture> Cultures;

		private string m_lastFileName;
		
		public CultureReader()
		{
			CultureGroups = new Dictionary<string, CultureGroup>();
			Cultures = new Dictionary<string, Culture>();
			Errors = new List<string>();
			m_lastFileName = m_lastFileName = string.Empty;
		}

		/// <summary>
		/// Loads a file containing cultures. Any errors encountered are stored in the Errors list.
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

			using ( FileStream fs = new FileStream( filename, FileMode.Open ) )
			{
				try
				{
					ParadoxParser.Parse( fs, ParseCultureGroups );
				} catch ( Exception ex)
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

		private void ParseCultureGroups( ParadoxParser parser, string tag )
		{
			CultureGroup cg = new CultureGroup();
			cg.Name = tag;
			cg.Filename = m_lastFileName;

			if( CultureGroups.ContainsKey( tag ) )
				CultureGroups.Remove( tag );
			CultureGroups[tag] = cg;

			Action<ParadoxParser, string> getOptions = ( p, s ) =>
			{
				if( s == "graphical_culture" )
					cg.GraphicalCulture = p.ReadString();
				else if ( s == "second_graphical_culture" )
					p.ReadString();
				else
					ParseCultures( cg, p, s );
			};

			parser.Parse( getOptions );
		}

		private void ParseCultures( CultureGroup cg, ParadoxParser parser, string tag )
		{
			Culture cul = new Culture();
			cul.Name = tag;
			cul.Group = cg;
			cul.Filename = m_lastFileName;

			if( cg.Cultures.ContainsKey( tag ) )
				cg.Cultures.Remove( tag );
			cg.Cultures[tag] = cul;

			if( Cultures.ContainsKey( tag ) )
				Cultures.Remove( tag );
			Cultures[tag] = cul;

			Action<ParadoxParser, string> getOptions = ( p, s ) =>
			{
				IList<string> stringList;

				switch( s )
				{
					case "color":
						stringList = p.ReadStringList();
						cul.Colour = stringList.ParseColour();
						break;
					case "male_names":
						stringList = p.ReadStringList();
						foreach( string name in stringList )
							cul.MaleNames.Add( name.Split( '_' )[0] );
						break;
					case "female_names":
						stringList = p.ReadStringList();
						foreach( string name in stringList )
							cul.FemaleNames.Add( name.Split( '_' )[0] );
						break;

					#region Id Options
					case "graphical_culture":
						cul.GraphicalCulture = p.ReadString();
						break;
					case "modifier":
						cul.Modifier = p.ReadString();
						break;
					case "graphical_unit_culture":
						cul.GraphicalUnitCulture = p.ReadString();
						break;
					case "grammar_transform":
						cul.GrammarTransform = p.ReadString();
						break;

					case "parent":
						Cultures.TryGetValue( p.ReadString(), out cul.Parent );
						break;
					case "second_graphical_culture":
						cul.SecondGraphicalCulture = p.ReadString();
						break;
					#endregion

					#region Bool Options
					case "prefix":
						cul.IsPrefix = p.ReadBool();
						break;
					case "horde":
						cul.IsHorde = p.ReadBool();
						break;
					case "dynasty_title_names":
						cul.DynastyTitleNames = p.ReadBool();
						break;
					case "founder_named_dynasties":
						cul.FounderNamedDynasties = p.ReadBool();
						break;

					case "dukes_called_kings":
						cul.DukesCalledKings = p.ReadBool();
						break;
					case "baron_titles_hidden":
						cul.BaronTitlesHidden = p.ReadBool();
						break;
					case "count_titles_hidden":
						cul.CountTitlesHidden = p.ReadBool();
						break;
					case "disinherit_from_blinding":
						cul.DisinheritFromBlinding = p.ReadBool();
						break;

					case "used_for_random":
						cul.UsedForRandom = p.ReadBool();
						break;
					#endregion

					#region String Options
					case "from_dynasty_prefix":
						cul.FromDynastyPrefix = p.ReadString();
						break;
					case "male_patronym":
						cul.MalePatronym = p.ReadString();
						break;
					case "female_patronym":
						cul.FemalePatronym = p.ReadString();
						break;
					case "bastard_dynasty_prefix":
						cul.BastardDynastyPrefix = p.ReadString();
						break;
					#endregion

					#region Number Options
					case "pat_grf_name_chance":
						cul.PaternalGrandfatherChance = p.ReadInt32();
						break;
					case "mat_grf_name_chance":
						cul.MaternalGrandfatherChance = p.ReadInt32();
						break;
					case "father_name_chance":
						cul.FatherChance = p.ReadInt32();
						break;

					case "pat_grm_name_chance":
						cul.PaternalGrandmotherChance = p.ReadInt32();
						break;
					case "mat_grm_name_chance":
						cul.MaternalGrandmotherChance = p.ReadInt32();
						break;
					case "mother_name_chance":
						cul.MotherChance = p.ReadInt32();
						break;
					#endregion

					default:
						Option.ParseGeneric( p, s );
						cul.MiscOptions.Add( Option.GetLastRoot() );
						break;
				}
			};

			parser.Parse( getOptions );
		}
	}
}
