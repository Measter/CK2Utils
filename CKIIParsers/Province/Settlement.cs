using System.Collections.Generic;

namespace Parsers.Province
{
  public class Settlement
  {
    public string Type;
    public string Title;

    /// <summary>
    /// Used for storing data specific to the program.
    /// </summary>
    public Dictionary<string, object> CustomFlags = new Dictionary<string, object>();

    public Settlement( string title, string type )
    {
      Type = type;
      Title = title;
    }

    public override string ToString()
    {
      return string.Format( "Title: {0}, Type: {1}", Title, Type );
    }
  }
}