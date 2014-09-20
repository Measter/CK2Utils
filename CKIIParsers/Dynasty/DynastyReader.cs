using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Parsers.Options;
using Pdoxcl2Sharp;

namespace Parsers.Dynasty
{
	public class DynastyReader : ReaderBase
	{
		/// <summary>
		/// Dictionary of loaded Dynasties.
		/// Key is the ID of the dynasty.
		/// </summary>
		public Dictionary<int, Dynasty> Dynasties;

		private string m_lastFileName = String.Empty;

		public DynastyReader()
		{
			Errors = new List<string>();
			Dynasties = new Dictionary<int, Dynasty>();
		}

		/// <summary>
		/// Loads a file of dynasties. Any errors encountered are stored in the Errors list.
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
					ParadoxParser.Parse( fs, ParseDynasties );
				} catch( Exception ex )
				{
					Errors.Add( ex.ToString() );
				}
			}
		}

		/// <summary>
		/// Loads all files in the given folder.
		/// </summary>
		/// <param name="folder">Path of the folder to load from.</param>
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


		private void ParseDynasties( ParadoxParser parser, string tag )
		{
			Dynasty dyn = new Dynasty();
			dyn.ID = Int32.Parse( tag );
			dyn.Filename = m_lastFileName;

			Dynasties[dyn.ID] = dyn;


			Action<ParadoxParser, string> getOptions = ( p, s ) =>
			{
				if( s == "name" )
					dyn.Name = p.ReadString();
				else if( s == "culture" )
					dyn.CultureString = p.ReadString();
				else if( s == "coat_of_arms" )
					ParseCoatOfArms( dyn, p, s );
			};

			parser.Parse( getOptions );
		}

		private void ParseCoatOfArms( Dynasty dyn, ParadoxParser parser, string tag )
		{
			Option.ParseGeneric( parser, tag );
			GroupOption og = (GroupOption)Option.GetLastRoot();
			dyn.CoatOfArms = og;
		} 


		public Dynasty GetDynastyByCulture( Culture.Culture cul, Random rand )
		{
			return GetDynastyByCulture( Dynasties, cul, rand );
		}

		public Dynasty GetDynastyByCulture( Dictionary<int, Dynasty> availDynasties, Culture.Culture cul, Random rand )
		{
			List<Dynasty> dynList = GetDynastyListByCulture( availDynasties, cul );

			Dynasty dyn = dynList.RandomItem( rand );

			if( dyn == null )
			{
				//Find one in the same culture group
				List<Culture.Culture> culList = cul.Group.Cultures.Select( c => c.Value ).ToList();
				culList.Remove( cul );
				while( dyn == null && culList.Count > 0 )
				{
					cul = culList.RandomItem( rand );
					culList.Remove( cul );

					dynList = GetDynastyListByCulture( availDynasties, cul );

					dyn = dynList.RandomItem( rand );
				}
			}
			return dyn;
		}

		private static List<Dynasty> GetDynastyListByCulture( Dictionary<int, Dynasty> availDynasties, Culture.Culture cul )
		{
			List<Dynasty> dynList = new List<Dynasty>();
			foreach( var d in availDynasties )
				if( d.Value.Culture == cul )
					dynList.Add( d.Value );

			return dynList;
		}

		public void LinkCultures( Dictionary<string, Culture.Culture> cultures )
		{
			foreach( var d in Dynasties )
				if( d.Value.CultureString != null && cultures.ContainsKey( d.Value.CultureString ) )
					d.Value.Culture = cultures[d.Value.CultureString];
		}
	}
}
