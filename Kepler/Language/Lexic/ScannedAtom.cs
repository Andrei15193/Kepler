namespace Andrei15193.Kepler.Language.Lexic
{
	public sealed class ScannedAtom
	{
		public ScannedAtom(KeplerLanguage.AtomCode code, uint line, uint column, string value = null)
		{
			_code = code;
			_line = line;
			_column = column;
			_value = value;
		}

		public uint Line
		{
			get
			{
				return _line;
			}
		}
		public uint Column
		{
			get
			{
				return _column;
			}
		}
		public KeplerLanguage.AtomCode Code
		{
			get
			{
				return _code;
			}
		}
		public string Value
		{
			get
			{
				return _value;
			}
		}

		private readonly uint _line;
		private readonly uint _column;
		private readonly KeplerLanguage.AtomCode _code;
		private readonly string _value;
	}
}