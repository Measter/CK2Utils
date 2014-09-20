using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parsers.Options
{
	public class EventOptionDateComparer  : IComparer<EventOption>
	{
		#region Implementation of IComparer<in EventOption>

		public int Compare( EventOption x, EventOption y )
		{
			return x.Date.CompareTo( y.Date );
		}

		#endregion
	}
}
