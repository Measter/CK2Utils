using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pdoxcl2Sharp;

namespace Parsers.Map
{
	public class Map
	{
		public int MaxProvinces = 0;
		public string Definitions = String.Empty;
		public string Provinces = String.Empty;
		public string Positions = String.Empty;
		public string Terrain = String.Empty;
		public string Rivers = String.Empty;
		public string TerrainDefinition = String.Empty;
		public string Heightmap = String.Empty;
		public string TreeDefinition = String.Empty;
		public string Continent = String.Empty;
		public string Adjacencies = String.Empty;
		public string Climate = String.Empty;
		public string Region = String.Empty;
		public string Static = String.Empty;

		public List<List<int>> OceanRegions = new List<List<int>>();
		public List<SeaZone> SeaZones = new List<SeaZone>();

		public List<int> Tree = new List<int>();
		public List<int> MajorRivers = new List<int>();

		public List<string> Errors = new List<string>();

		public static Map Parse( string filename )
		{
			Map m = new Map();

			if( !File.Exists( filename ) )
			{
				m.Errors.Add( string.Format( "File not found: {0}", filename ) );
				return m;
			}

			using( FileStream fs = new FileStream( filename, FileMode.Open ) )
			{
				try
				{
					ParadoxParser.Parse( fs, m.ParseMap );
				} catch( Exception ex )
				{
					m.Errors.Add( ex.ToString() );
				}
			}

			return m;
		}

		private void ParseMap( ParadoxParser parser, string s )
		{
			IList<int> intList;

			switch( s )
			{
				case "max_provinces":
					MaxProvinces = parser.ReadInt32();
					break;

				#region File List
				case "definitions":
					Definitions = parser.ReadString();
					break;
				case "provinces":
					Provinces = parser.ReadString();
					break;
				case "positions":
					Positions = parser.ReadString();
					break;
				case "terrain":
					Terrain = parser.ReadString();
					break;

				case "rivers":
					Rivers = parser.ReadString();
					break;
				case "terrain_definition":
					TerrainDefinition = parser.ReadString();
					break;
				case "heightmap":
					Heightmap = parser.ReadString();
					break;
				case "tree_definition":
					TreeDefinition = parser.ReadString();
					break;

				case "continent":
					Continent = parser.ReadString();
					break;
				case "adjacencies":
					Adjacencies = parser.ReadString();
					break;
				case "climate":
					Climate = parser.ReadString();
					break;
				case "region":
					Region = parser.ReadString();
					break;

				case "static":
					Static = parser.ReadString();
					break;
				#endregion

				case "sea_zones":
					intList = parser.ReadIntList();
					SeaZones.Add( new SeaZone( intList[0], intList[1] ) );
					break;
				case "ocean_region":
					parser.ReadString();
					OceanRegions.Add( parser.ReadIntList().ToList() );
					break;

				case "tree":
					Tree.AddRange( parser.ReadIntList().ToList() );
					break;
				case "major_rivers":
					MajorRivers.AddRange( parser.ReadIntList() );
					break;
			}
		}


		public bool IsSeaProvince( int provinceID )
		{
			return SeaZones.Any( zone => provinceID >= zone.Start && provinceID <= zone.End );
		}
	}

	public class SeaZone : IEquatable<SeaZone>
	{
		public int Start;
		public int End;

		public SeaZone( int start, int end )
		{
			Start = start;
			End = end;
		}

		#region Equality members

		public bool Equals( SeaZone other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			return Start == other.Start && End == other.End;
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != this.GetType() )
				return false;
			return Equals( (SeaZone)obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ( Start * 397 ) ^ End;
			}
		}

		public static bool operator ==( SeaZone left, SeaZone right )
		{
			return Equals( left, right );
		}

		public static bool operator !=( SeaZone left, SeaZone right )
		{
			return !Equals( left, right );
		}

		#endregion
	}
}
