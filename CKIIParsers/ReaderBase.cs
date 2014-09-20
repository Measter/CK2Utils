using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parsers
{
	public abstract class ReaderBase
	{
		/// <summary>
		/// List of errors encountered during parsing.
		/// </summary>
		public List<string> Errors;

		/// <summary>
		/// Loads a file containing cultures. Any errors encountered are stored in the Errors list.
		/// </summary>
		/// <param name="filename">Path of the file to load.</param> 
		abstract public void Parse( string filename );

		/// <summary>
		/// Loads all the files in a folder for parsing. Any errors encountered are stored in the Errors list.
		/// </summary>
		/// <param name="folder">Path of the folder to load files from.</param>
		public abstract void ParseFolder( string folder );
	}
}
