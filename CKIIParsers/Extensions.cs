using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace Parsers
{
	public static class Extensions
	{
		public static bool IsFlagSet<T>( this Enum value, Enum flag )
		{
			if( !typeof( T ).IsEnum )
				throw new ArgumentException();
			if( value.Equals( flag ) )
				return true;
			return ( (int)Enum.Parse( typeof( T ), value.ToString() ) & (int)Enum.Parse( typeof( T ), flag.ToString() ) ) != 0;
		}

		private static readonly CultureInfo English = CultureInfo.GetCultureInfo( "en-GB" );
		
		public static KeyValuePair<T1, T2> RandomItem<T1, T2>( this Dictionary<T1, T2> list, Random rand )
		{
			if( !list.Any() )
				return default( KeyValuePair<T1, T2> );

			return list.ElementAt( rand.Next( list.Count ) );
		}

		public static T RandomItem<T>( this List<T> list, Random rand )
		{
			if( !list.Any() )
				return default( T );

			return list[rand.Next( list.Count )];
		}

		public static T RandomItem<T>( this IEnumerable<T> list, Random rand )
		{
			if( !list.Any() )
				return default( T );

			var enumerable = list as List<T> ?? list.ToList();
			return enumerable.ToList()[rand.Next( enumerable.Count() )];
		}

		public static int Clamp( this int n, int min, int max )
		{
			if( n > max )
				return max;
			if( n < min )
				return min;
			return n;
		}

		public static Color ParseColour( this IList<string> stringList )
		{
			if( stringList.Count < 3 )
				return Color.White;

			int r, g, b;

			bool isFloat = stringList.Any( s => s.Contains( "." ) );

			if( isFloat )
			{
				double rF = Double.Parse( stringList[0], English );
				double gF = Double.Parse( stringList[1], English );
				double bF = Double.Parse( stringList[2], English );

				r = (int)Math.Round( rF * 255 );
				g = (int)Math.Round( gF * 255 );
				b = (int)Math.Round( bF * 255 );
			} else
			{
				r = Int32.Parse( stringList[0] );
				g = Int32.Parse( stringList[1] );
				b = Int32.Parse( stringList[2] );
			}

			r = r.Clamp( 0, 255 );
			g = g.Clamp( 0, 255 );
			b = b.Clamp( 0, 255 );

			return Color.FromArgb( r, g, b );
		}

		public static DateTime ToDateTime( this string str )
		{
			if( !str.Contains( '.' ) || str.IndexOf(".", StringComparison.Ordinal) == str.LastIndexOf(".", StringComparison.Ordinal) )
				return new DateTime( 1, 1, 1 );

			string[] parts = str.Split( '.' );
			return new DateTime( Int32.Parse( parts[0] ), Int32.Parse( parts[1] ), Int32.Parse( parts[1] ) );
		}
	}
}