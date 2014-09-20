using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parsers.Mod
{
	public class ModReader : ReaderBase
	{
		/// <summary>
		/// List of loaded Mods
		/// </summary>
		public List<Mod> Mods;

		public ModReader()
		{
			Mods = new List<Mod>();
			Errors = new List<string>();
		}

		public override void Parse( string filename )
		{
			Parse( filename, Folder.MyDocs );
		}

		/// <summary>
		/// Loads all files in the given folder.
		/// </summary>
		/// <param name="filename">Path to the mod file to load.</param>
		/// <param name="dir">Specifies whether path is CKII install or My Docs.</param>
		public void Parse( string filename, Folder dir )
		{
			if( !File.Exists( filename ) )
			{
				Errors.Add( string.Format( "File not found: {0}", filename ) );
				return;
			}

			Mod m;
			string line;
			FileInfo f = new FileInfo( filename );
			m = new Mod();
			m.ModFile = f.Name;
			m.ModPathType = dir;
			int lineNum = 1;

			using( StreamReader sr = new StreamReader( filename, Encoding.GetEncoding( 1252 ) ) )
			{
				while( ( line = sr.ReadLine() ) != null )
				{
					line = line.Trim();
					if( line.StartsWith( "#" ) )
						continue;

					try
					{
						if( line.StartsWith( "name" ) )
							m.Name = line.Split( '=' )[1].Split( '#' )[0].Replace( "\"", "" ).Trim();
						if( line.StartsWith( "path" ) || line.StartsWith( "archive" ) )
							m.Path = line.Split( '=' )[1].Split( '#' )[0].Replace( "\"", "" ).Trim();
						if( line.StartsWith( "user_dir" ) || line.StartsWith( "archive" ) )
							m.UserDir = line.Split( '=' )[1].Split( '#' )[0].Replace( "\"", "" ).Trim();

						if( line.StartsWith( "extend" ) )
							m.Extends.Add( line.Split( '=' )[1].Split( '#' )[0].Replace( "\"", "" ).Trim() );
						if( line.StartsWith( "replace" ) )
							m.Replaces.Add( line.Split( '=' )[1].Split( '#' )[0].Replace( "\"", "" ).Trim().Replace( '/', '\\' ) );

						if( line.StartsWith( "dependencies" ) )
						{
							string[] tstring = line.Split( '=' )[1].Split( '#' )[0].Replace( "{", "" ).Replace( "}", "" ).Trim().Split( '"' );
							foreach( string s in tstring )
							{
								if( s.Trim() != string.Empty )
								{
									m.Dependencies.Add( s );
								}
							}
						}
					} catch( System.Exception )
					{
						Errors.Add( string.Format( "Error parsing file {0} on line {1}", f.Name, lineNum ) );
					}
					lineNum++;
				}
			}
			Mods.Add( m );
		}

		public override void ParseFolder( string folder )
		{
			ParseFolder( folder, Folder.MyDocs );
		}

		/// <summary>
		/// Loads all files in the given folder.
		/// </summary>
		/// <param name="folder">Path to the folder containing the files to load.</param>
		/// <param name="dirType">Specifies whether path is CKII install or My Docs.</param>
		public void ParseFolder( string folder, Folder dirType )
		{
			DirectoryInfo dir = new DirectoryInfo( folder );

			if ( !dir.Exists )
				return;

			FileInfo[] mods = dir.GetFiles( "*.mod" );

			foreach ( FileInfo f in mods )
				Parse( f.FullName, dirType );
		}

		public enum Folder
		{
			CKDir,
			MyDocs,
			DLC
		}
	}
}
