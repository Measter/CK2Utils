using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using Pdoxcl2Sharp;

namespace Parsers.Options
{
	public enum OptionType
	{
		Group,
		Event,
		ID,
		String,
		StringList,
		Bool,
		Float,
		Integer,
		IntList,
		Colour,
		Date,
	}

	[DebuggerDisplay( "Tag: {GetIDString}, Data: {GetValueString}" )]
	public abstract class Option
	{
		public readonly OptionType Type;

		protected Option( OptionType type )
		{
			Type = type;
		}

		public abstract string GetIDString
		{
			get;
		}
		public abstract string GetValueString
		{
			get;
		}


		private static Stack<Option> m_optionStack = new Stack<Option>();
		private static Option m_lastParsedOptionRoot;
		public static Option GetLastRoot()
		{
			Option opt = m_lastParsedOptionRoot;
			m_lastParsedOptionRoot = null;
			return opt;
		}

		public static string Output( Option opt, char prefixChar, int level, bool newLine = true )
		{
			StringBuilder sb = new StringBuilder();

			sb.Append( String.Empty.PadLeft( level, prefixChar ) );
			sb.Append( opt.GetIDString );
			sb.Append( " = " );

			if( opt.Type == OptionType.Event )
			{
				sb.AppendLine( "{" );
				foreach( Option o in ( (EventOption)opt ).SubOptions )
					sb.AppendLine( Output( o, prefixChar, level + 1, false ) );

				sb.Append( String.Empty.PadLeft( level, prefixChar ) );

				if( newLine )
					sb.AppendLine( "}" );
				else
					sb.Append( "}" );
			}
			if( opt.Type == OptionType.Group )
			{
				sb.AppendLine( "{" );

				foreach( Option o in ( (GroupOption)opt ).SubOptions )
					sb.AppendLine( Output( o, prefixChar, level + 1, false ) );

				sb.Append( String.Empty.PadLeft( level, prefixChar ) );
				if( newLine )
					sb.AppendLine( "}" );
				else
					sb.Append( "}" );
			} else
			{
				if( newLine )
					sb.AppendLine( opt.GetValueString );
				else
					sb.Append( opt.GetValueString );
			}

			return sb.ToString();
		}

		/// <summary>
		/// Parses a single option of the type: tag = data
		/// </summary>
		public static Option ParseSingle( string tag, string data )
		{
			Option opt;

			if( IsNumberType( data ) )
			{
				if( data.Contains( "." ) )
				{
					if( data.IndexOf( ".", StringComparison.Ordinal ) != data.LastIndexOf( ".", StringComparison.Ordinal ) )
						opt = new DateOption( tag, data.ToDateTime() );
					else
						opt = new FloatOption( tag, Double.Parse( data, CultureInfo.InvariantCulture ) );
				} else
					opt = new IntegerOption( tag, Int32.Parse( data ) );
			} else if( data == "yes" || data == "no" )
			{
				opt = new BoolOption( tag, data == "yes" );
			} else
			{
				opt = new StringOption( tag, data, OptionType.String );
			}

			return opt;
		}

		private static bool IsNumberType( string data )
		{
			int index = data.IndexOf( '-' );
			if ( index > 0 )
				return false;

			foreach ( char c in data )
				if ( Char.IsLetter( c ) || c == '_' )
					return false;

			return true;
		}

		/// <summary>
		/// used to parse group options of the type: tag = { tag2 = data ... }
		/// </summary>
		/// <param name="tag">The *last* read tag.</param>
		/// <returns>Parsed option can be retrieved using GetLastRoot()</returns>
		public static void ParseGeneric( ParadoxParser parser, string tag )
		{
			Option opt;

			if( parser.NextIsBracketed() )
			{
				if( tag == "data" )
				{
					opt = new IntListOption( tag, parser.ReadIntList() );
				} else
				{
					if( tag.Contains( "." ) && tag.IndexOf( ".", StringComparison.Ordinal ) != tag.LastIndexOf( ".", StringComparison.Ordinal ) )
						opt = new EventOption( tag );
					else
						opt = new GroupOption( tag );

					m_optionStack.Push( opt );
					parser.Parse( ParseGeneric );
					m_optionStack.Pop();
				}
			} else
			{
				opt = ParseSingle( tag, parser.ReadString() );
			}

			if( m_optionStack.Count == 0 ) // We're in the root.
				m_lastParsedOptionRoot = opt;
			else
			{
				Option parent = m_optionStack.Peek();
				if( parent.Type == OptionType.Group )
					( (GroupOption)parent ).SubOptions.Add( opt );
				else if( parent.Type == OptionType.Event )
					( (EventOption)parent ).SubOptions.Add( opt );
			}
		}
	}

	[DebuggerDisplay( "Tag: {m_id}, SubOptions: {SubOptions.Count}" )]
	public class GroupOption : Option
	{
		public List<Option> SubOptions;
		private readonly string m_id;

		public GroupOption( string id )
			: base( OptionType.Group )
		{
			m_id = id;
			SubOptions = new List<Option>();
		}

		public override string GetIDString
		{
			get
			{
				return m_id;
			}
		}

		public override string GetValueString
		{
			get
			{
				return SubOptions.Count.ToString( CultureInfo.InvariantCulture );
			}
		}

	}

	[DebuggerDisplay( "Date: {m_date}, SubOptions: {SubOptions.Count}" )]
	public class EventOption : Option
	{
		public List<Option> SubOptions;
		private readonly DateTime m_date;

		public EventOption( string date )
			: base( OptionType.Event )
		{
			m_date = date.ToDateTime();
			SubOptions = new List<Option>();
		}

		public EventOption( DateTime date )
			: base( OptionType.Event )
		{
			m_date = date;
			SubOptions = new List<Option>();
		}

		public DateTime Date
		{
			get
			{
				return m_date;
			}
		}

		public override string GetIDString
		{
			get
			{
				return m_date.Year + "." + m_date.Month + "." + m_date.Day;
			}
		}

		public override string GetValueString
		{
			get
			{
				return SubOptions.Count.ToString( CultureInfo.InvariantCulture );
			}
		}
	}

	[DebuggerDisplay( "Tag: {m_id}, Data: {m_value}" )]
	public class StringOption : Option
	{
		private readonly string m_id;
		private readonly string m_value;

		public StringOption( string id, string value, OptionType type )
			: base( type )
		{
			m_id = id;
			m_value = value;

		}

		public string GetValue
		{
			get
			{
				return m_value;
			}
		}

		public override string GetIDString
		{
			get
			{
				return m_id;
			}
		}

		public override string GetValueString
		{
			get
			{
				return Type == OptionType.String ? "\"" + m_value + "\"" : m_value;
			}
		}
	}

	[DebuggerDisplay( "Tag: {m_id}, Data: {m_value}" )]
	public class BoolOption : Option
	{
		private readonly string m_id;
		private readonly bool m_value;

		public BoolOption( string id, bool value )
			: base( OptionType.Bool )
		{
			m_id = id;
			m_value = value;
		}

		public bool Value
		{
			get
			{
				return m_value;
			}
		}

		public override string GetIDString
		{
			get
			{
				return m_id;
			}
		}

		public override string GetValueString
		{
			get
			{
				return
					Value ? "yes" : "no";
			}
		}
	}

	[DebuggerDisplay( "Tag: {m_id}, Data: {m_value}" )]
	public class IntegerOption : Option
	{
		private readonly string m_id;
		private readonly int m_value;

		public IntegerOption( string id, int value )
			: base( OptionType.Integer )
		{
			m_id = id;
			m_value = value;
		}

		public int Value
		{
			get
			{
				return m_value;
			}
		}

		public override string GetIDString
		{
			get
			{
				return m_id;
			}
		}

		public override string GetValueString
		{
			get
			{
				return Value.ToString( CultureInfo.InvariantCulture );
			}
		}
	}

	[DebuggerDisplay( "Tag: {m_id}, Data: {m_value}" )]
	public class FloatOption : Option
	{
		private readonly string m_id;
		private readonly double m_value;

		public FloatOption( string id, double value )
			: base( OptionType.Float )
		{
			m_id = id;
			m_value = Math.Round( value, 3 );
		}

		public double Value
		{
			get
			{
				return m_value;
			}
		}

		public override string GetIDString
		{
			get
			{
				return m_id;
			}
		}

		public override string GetValueString
		{
			get
			{
				return m_value.ToString( CultureInfo.InvariantCulture );
			}
		}
	}

	[DebuggerDisplay( "Tag: {m_id}, Data: {m_value}" )]
	public class ColourOption : Option
	{
		private readonly string m_id;
		private readonly Color m_value;

		public ColourOption( string id, Color value )
			: base( OptionType.Colour )
		{
			m_id = id;
			m_value = value;
		}

		public Color Value
		{
			get
			{
				return m_value;
			}
		}

		public override string GetIDString
		{
			get
			{
				return m_id;
			}
		}

		public override string GetValueString
		{
			get
			{
				return m_value.ToString();
			}
		}
	}

	[DebuggerDisplay( "Tag: {m_id}, Data: {m_value}" )]
	public class DateOption : Option
	{
		private readonly string m_id;
		private readonly DateTime m_value;

		public DateOption( string id, DateTime value )
			: base( OptionType.Date )
		{
			m_id = id;
			m_value = value;
		}

		public DateTime Value
		{
			get
			{
				return m_value;
			}
		}

		public override string GetIDString
		{
			get
			{
				return m_id;
			}
		}

		public override string GetValueString
		{
			get
			{
				return m_value.ToString( CultureInfo.InvariantCulture );
			}
		}
	}

	[DebuggerDisplay( "Tag: {m_id}, Data Count: {Items.Count}" )]
	public class IntListOption : Option
	{
		private readonly string m_id;
		public readonly IList<int> Items;

		public IntListOption( string id, IList<int> value )
			: base( OptionType.IntList )
		{
			m_id = id;
			Items = value;
		}

		public override string GetIDString
		{
			get
			{
				return m_id;
			}
		}

		public override string GetValueString
		{
			get
			{
				return Items.Count.ToString();
			}
		}
	}

	[DebuggerDisplay( "Tag: {m_id}, Data Count: {Items.Count}" )]
	public class StringListOption : Option
	{
		private readonly string m_id;
		public readonly IList<string> Items;

		public StringListOption( string id, IList<string> value )
			: base( OptionType.StringList )
		{
			m_id = id;
			Items = value;
		}

		public override string GetIDString
		{
			get
			{
				return m_id;
			}
		}

		public override string GetValueString
		{
			get
			{
				return Items.Count.ToString();
			}
		}
	}
}