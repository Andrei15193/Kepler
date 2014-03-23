using System;
namespace Andrei15193.Kepler.Language.Lexic
{
	public struct ScannedAtom<TAtomCode>
		: IEquatable<ScannedAtom<TAtomCode>>
		where TAtomCode : struct
	{
		public ScannedAtom(TAtomCode code, int line, int column, string value)
		{
			if (line < 1)
				throw new ArgumentException("Line must be above 0 (zero)!", "line");
			if (column < 1)
				throw new ArgumentException("Line must be above 0 (zero)!", "column");
			if (value == null)
				throw new ArgumentNullException("value");
			if (value.Length == 0)
				throw new ArgumentException("Cannot be empty!", "value");

			_code = code;
			_line = line;
			_column = column;
			_value = value;
		}

		public static bool operator ==(ScannedAtom<TAtomCode> left, ScannedAtom<TAtomCode> right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(ScannedAtom<TAtomCode> left, object right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(ScannedAtom<TAtomCode> left, ValueType right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(ScannedAtom<TAtomCode> left, IEquatable<ScannedAtom<TAtomCode>> right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(object left, ScannedAtom<TAtomCode> right)
		{
			return right.Equals(left);
		}
		public static bool operator ==(ValueType left, ScannedAtom<TAtomCode> right)
		{
			return right.Equals(left);
		}
		public static bool operator ==(IEquatable<ScannedAtom<TAtomCode>> left, ScannedAtom<TAtomCode> right)
		{
			return right.Equals(left);
		}
		public static bool operator !=(ScannedAtom<TAtomCode> left, ScannedAtom<TAtomCode> right)
		{
			return !(left == right);
		}
		public static bool operator !=(ScannedAtom<TAtomCode> left, object right)
		{
			return !(left == right);
		}
		public static bool operator !=(ScannedAtom<TAtomCode> left, ValueType right)
		{
			return !(left == right);
		}
		public static bool operator !=(ScannedAtom<TAtomCode> left, IEquatable<ScannedAtom<TAtomCode>> right)
		{
			return !(left == right);
		}
		public static bool operator !=(object left, ScannedAtom<TAtomCode> right)
		{
			return !(left == right);
		}
		public static bool operator !=(ValueType left, ScannedAtom<TAtomCode> right)
		{
			return !(left == right);
		}
		public static bool operator !=(IEquatable<ScannedAtom<TAtomCode>> left, ScannedAtom<TAtomCode> right)
		{
			return !(left == right);
		}

		#region IEquatable<ScannedAtom<TAtomCode>> Members
		bool IEquatable<ScannedAtom<TAtomCode>>.Equals(ScannedAtom<TAtomCode> other)
		{
			return Equals(other);
		}
		#endregion
		public override bool Equals(object obj)
		{
			return (obj is ScannedAtom<TAtomCode> && Equals((ScannedAtom<TAtomCode>)obj));
		}
		public override int GetHashCode()
		{
			return (_code.GetHashCode() ^ _column.GetHashCode() ^ _line.GetHashCode() ^ _value.GetHashCode());
		}
		public override string ToString()
		{
			return string.Format("{0}: {1} [line: {2}, column: {3}]", _code.ToString(), _value, _line, _column);
		}
		public bool Equals(object obj, bool caseSensitive)
		{
			return (obj is ScannedAtom<TAtomCode> && Equals((ScannedAtom<TAtomCode>)obj, caseSensitive));
		}
		public bool Equals(ScannedAtom<TAtomCode> other, bool caseSensitive = true)
		{
			return (_code.Equals(other._code)
					&& _column == other._column
					&& _line == other._line
					&& string.Equals(_value, other._value, (caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)));
		}
		public int Line
		{
			get
			{
				return _line;
			}
		}
		public int Column
		{
			get
			{
				return _column;
			}
		}
		public TAtomCode Code
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

		private readonly int _line;
		private readonly int _column;
		private readonly TAtomCode _code;
		private readonly string _value;
	}
}