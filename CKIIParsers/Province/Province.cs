using System.Collections.Generic;
using Parsers.Options;

namespace Parsers.Province
{
	public class Province
	{
		/// <summary>
		/// The title ID of the province.
		/// E.g. c_holland.
		/// </summary>
		public string Title;
		public string Culture;
		public string Religion;
		public string Terrain;
		public bool IsCoastal;

		/// <summary>
		/// The ID of the province.
		/// E.g. 534
		/// </summary>
		public int ID;
		public int MaxSettlements;

		public List<EventOption> History = new List<EventOption>();

		public List<Settlement> Settlements = new List<Settlement>();

		/// <summary>
		/// List of the land provinces adjacent to this provence.
		/// </summary>
		public List<Province> Adjacencies = new List<Province>();

		/// <summary>
		/// Used for storing data specific to the program.
		/// </summary>
		public Dictionary<string, object> CustomFlags = new Dictionary<string, object>();

		public string Filename;

		public override string ToString()
		{
			return string.Format( "Title: {0}, Id: {1}", Title, ID );
		}
	}
}