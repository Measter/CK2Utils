using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parsers.Localisations
{
	public class LocalisationReader : ReaderBase
	{
		public Dictionary<string, string> LocalisationStrings;

		public LocalisationReader()
		{
			LocalisationStrings = new Dictionary<string, string>();
			Errors = new List<string>();
		}

		public override void Parse( string filename )
		{
			FileInfo f = new FileInfo( filename );

			if( !f.Exists )
			{
				Errors.Add( "Unable to find file: " + f.Name );
				return;
			}

			string input;
			StreamReader sr = new StreamReader( f.Open( FileMode.Open, FileAccess.Read ), Encoding.Default );

			while ( ( input = sr.ReadLine() ) != null )
				LocalisationStrings[input.Split( ';' )[0]] = input;

			sr.Dispose();
		}

		public override void ParseFolder( string folder )
		{
			DirectoryInfo dir = new DirectoryInfo( folder );

			if ( !dir.Exists )
			{
				Errors.Add( string.Format( "Folder not found: {0}", dir.FullName ) );
				return;
			}

			FileInfo[] files = dir.GetFiles( "*.csv" );

			foreach ( FileInfo f in files )
				Parse( f.FullName );
		}
	}
}