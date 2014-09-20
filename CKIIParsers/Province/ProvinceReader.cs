using System;
using System.Collections.Generic;
using System.IO;
using Measter;
using Parsers.Options;
using Pdoxcl2Sharp;

namespace Parsers.Province
{
	public class ProvinceReader : ReaderBase
	{
		/// <summary>
		/// Dictionary of loaded Provinces.
		/// Key is the ID of the province.
		/// </summary>
		public Dictionary<int, Province> Provinces;

		private Province m_curProv;


		public ProvinceReader()
		{
			Provinces = new Dictionary<int, Province>();
			Errors = new List<string>();
		}

		/// <summary>
		/// Loads a file containing a province.
		/// </summary>
		/// <param name="filename">Path of the file to load.</param>
		public override void Parse( string filename )
		{
			if( !File.Exists( filename ) )
			{
				Errors.Add( "Unable to find file: " + filename );
				return;
			}

			using( FileStream fs = new FileStream( filename, FileMode.Open ) )
			{
				try
				{
					m_curProv = new Province();
					m_curProv.Filename = filename;

					m_curProv.ID = Int32.Parse( Path.GetFileName( filename ).Split( '-' )[0].Trim() );

					ParadoxParser.Parse( fs, ParseProvinceData );

					if( !String.IsNullOrWhiteSpace( m_curProv.Title ) )
					{
						if( Provinces.ContainsKey( m_curProv.ID ) )
							Provinces.Remove( m_curProv.ID );
						Provinces[m_curProv.ID] = m_curProv; 
					}

					m_curProv = null;
				} catch( Exception ex )
				{
					Errors.Add( ex.ToString() );
				}
			}
		}


		/// <summary>
		/// Loads all the files in a folder.
		/// </summary>
		/// <param name="folder">Path to the folder to load from.</param>
		public override void ParseFolder( string folder )
		{
			DirectoryInfo dir = new DirectoryInfo( folder );

			if( !dir.Exists )
			{
				Errors.Add( "Unable to find folder: " + folder );
				return;
			}

			FileInfo[] provs = dir.GetFiles();

			foreach( FileInfo prov in provs )
			{
				Parse( prov.FullName );
			}
		}


		private void ParseProvinceData( ParadoxParser parser, string tag )
		{
			if( tag.StartsWith( "b_" ) )
			{
				m_curProv.Settlements.Add( new Settlement( tag, parser.ReadString() ) );
				return;
			}
			if( Char.IsDigit( tag[0] ) )
			{
				// Should be an event date.
				Option.ParseGeneric( parser, tag );
				m_curProv.History.Add( (EventOption)Option.GetLastRoot() );
				return;
			}

			switch( tag )
			{
				case "max_settlements":
					m_curProv.MaxSettlements = parser.ReadInt32();
					break;

				case "title":
					m_curProv.Title = parser.ReadString();
					break;
				case "culture":
					m_curProv.Culture = parser.ReadString();
					break;
				case "religion":
					m_curProv.Religion = parser.ReadString();
					break;
				case "terrain":
					m_curProv.Terrain = parser.ReadString();
					break;
			}
		}


		/// <summary>
		/// Reads the default.map, adjacencies.csv, and setup.log to find which provinces are connected.
		/// </summary>
		/// <param name="setupLogFile">Path to the setup.log file.</param>
		/// <param name="map"> </param>
		/// <param name="adjFile">Path to the adjacencies file.</param>
		public void ParseAdjacencies( string setupLogFile, Map.Map map, string adjFile )
		{
			if( !File.Exists( setupLogFile ) )
				throw new FileNotFoundException( "Unable to find the log file.", setupLogFile );
			if( !File.Exists( adjFile ) )
				throw new FileNotFoundException( "Unable to find the adjacencies file.", adjFile );

			if( Provinces.Count == 0 )
				throw new Exception( "No provinces have been loaded." );

			string line;
			FileStream fs;
			TextReader tw;
			string[] lineSides;
			int provinceID = 0;
			Province currProv;
			Province adjProv;
			FileInfo readFile;

			#region Adjacencies File
			readFile = new FileInfo( adjFile );
			using( fs = readFile.Open( FileMode.Open, FileAccess.Read ) )
			using( tw = new StreamReader( fs ) )
			{
				while( ( line = tw.ReadLine() ) != null )
				{
					if( line.Trim().ToLower().StartsWith( "from" ) )
						continue;
					if( line.Trim().ToLower().StartsWith( "-1" ) )
						continue;

					lineSides = line.Split( ';' );

					if( !Int32.TryParse( lineSides[0].Trim(), out provinceID ) )
						continue;

					if( !Provinces.ContainsKey( provinceID ) )
					{
						Errors.Add( "Adjacencies: Unable to find province ID: " + provinceID );
						continue;
					}
					currProv = Provinces[provinceID];

					if( !Int32.TryParse( lineSides[1].Trim(), out provinceID ) )
						continue;

					if( !Provinces.ContainsKey( provinceID ) )
					{
						Errors.Add( "Adjacencies: Unable to find province ID: " + provinceID );
						continue;
					}
					adjProv = Provinces[provinceID];

					if( !currProv.Adjacencies.Contains( adjProv ) )
						currProv.Adjacencies.Add( adjProv );
					if( !adjProv.Adjacencies.Contains( currProv ) )
						adjProv.Adjacencies.Add( currProv );
				}
			}
			#endregion

			#region setup.log
			readFile = new FileInfo( setupLogFile );
			using( fs = readFile.Open( FileMode.Open, FileAccess.Read ) )
			using( tw = new StreamReader( fs ) )
			{
				while( ( line = tw.ReadLine() ) != null )
				{
					if( !line.Trim().StartsWith( "[map.cpp:" ) )
						continue;
					if( !line.Contains( "Adjacencies for" ) )
						continue;

					lineSides = line.Split( ':' );
					lineSides = lineSides[2].Split( new[] { "==>" }, StringSplitOptions.RemoveEmptyEntries );

					//No adjacencies
					if( lineSides[1].Trim() == "NONE" )
						continue;

					//Check for valid province ID.
					if( !Int32.TryParse( lineSides[0].Replace( "Adjacencies for", "" ).Trim(), out provinceID ) )
						continue;

					//We don't want sea provinces.
					if( map.IsSeaProvince( provinceID ) )
						continue;

					if( !Provinces.ContainsKey( provinceID ) )
					{
						Errors.Add( "Adjacencies: Unable to find province ID: " + provinceID );
						continue;
					}

					currProv = Provinces[provinceID];

					//now to find each adjacent province.
					lineSides = lineSides[1].Split( new[] { " " }, StringSplitOptions.RemoveEmptyEntries );

					foreach( string t in lineSides )
					{
						if( !Int32.TryParse( t.Trim(), out provinceID ) )
							continue;

						if( map.MajorRivers.Contains( provinceID ) )
							continue;

						if( map.IsSeaProvince( provinceID ) )
						{
							currProv.IsCoastal = true;
							continue;
						}

						if( !Provinces.ContainsKey( provinceID ) )
						{
							Errors.Add( "Adjacencies: Unable to find province ID: " + provinceID );
							continue;
						}

						adjProv = Provinces[provinceID];

						if( !currProv.Adjacencies.Contains( adjProv ) )
							currProv.Adjacencies.Add( adjProv );
					}
				}
			}
			#endregion
		}
	}
}
