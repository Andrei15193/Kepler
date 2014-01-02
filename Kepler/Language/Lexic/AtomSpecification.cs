using System;
using System.Text.RegularExpressions;
namespace Andrei15193.Kepler.Language.Lexic
{
	public abstract class AtomSpecification
	{
		public static AtomSpecification Create(KeplerLanguage.AtomCode code, string literal, bool ignoreAtom = false, bool isSeparator = false, bool ignoreCase = false)
		{
			return new LiteralAtomSpecification(code, literal, ignoreAtom, ignoreCase);
		}
		public static AtomSpecification Create(KeplerLanguage.AtomCode code, Regex pattern, bool ignoreAtom = false, bool isSeparator = false)
		{
			return new PatternAtomSpecfication(code, pattern, ignoreAtom, isSeparator);
		}
		public static AtomSpecification Create(KeplerLanguage.AtomCode code, string beggining, string end, bool ignoreAtom = false, bool isSeparator = false, bool ignoreCase = false)
		{
			return new EnclosedAtomSpecification(code, beggining, end, ignoreAtom, isSeparator, ignoreCase);
		}
		protected AtomSpecification(KeplerLanguage.AtomCode code, bool ignore, bool isSeparator)
		{
			_code = code;
			_ignore = ignore;
			_isSeparator = isSeparator;
		}

		public bool Ignore
		{
			get
			{
				return _ignore;
			}
		}
		public bool IsSeparator
		{
			get
			{
				return _isSeparator;
			}
		}
		public KeplerLanguage.AtomCode Code
		{
			get
			{
				return _code;
			}
		}
		public abstract bool TryIdentifty(string text, out string atom, int startIndex = 0);
		public abstract bool IsSatisfiedBy(string str, int startIndex = 0);

		private readonly bool _ignore;
		private readonly bool _isSeparator;
		private readonly KeplerLanguage.AtomCode _code;

		private class LiteralAtomSpecification
			: AtomSpecification
		{
			public LiteralAtomSpecification(KeplerLanguage.AtomCode code, string literal, bool canIgnore = false, bool isSeparator = false, bool ignoreCase = false)
				: base(code, canIgnore, isSeparator)
			{
				if (literal == null)
					throw new ArgumentNullException("literal");
				if (literal.Length == 0)
					throw new ArgumentException("The literal cannot an empty string!", "literal");
				_literal = literal;
				_ingoreCase = ignoreCase;
			}

			public override bool TryIdentifty(string text, out string atom, int startIndex = 0)
			{
				if (text != null && string.Compare(_literal, 0, text, startIndex, _literal.Length, _ingoreCase) == 0)
					atom = _literal;
				else
					atom = null;
				return (atom != null);
			}
			public override bool IsSatisfiedBy(string str, int startIndex = 0)
			{
				return (str != null && string.Compare(_literal, 0, str, startIndex, str.Length, _ingoreCase) == 0);
			}

			private readonly bool _ingoreCase;
			private readonly string _literal;
		}
		private class PatternAtomSpecfication
			: AtomSpecification
		{
			public PatternAtomSpecfication(KeplerLanguage.AtomCode code, Regex pattern, bool canIgnore, bool isSeparator)
				: base(code, canIgnore, isSeparator)
			{
				if (pattern == null)
					throw new ArgumentNullException("pattern");
				_pattern = pattern;
			}

			public override bool TryIdentifty(string text, out string atom, int startIndex = 0)
			{
				Match patternFirstMatch = _pattern.Match(text, startIndex);
				if (patternFirstMatch.Index == startIndex)
					atom = patternFirstMatch.Value;
				else
					atom = null;
				return (atom != null);
			}
			public override bool IsSatisfiedBy(string str, int startIndex = 0)
			{
				return _pattern.IsMatch(str, startIndex);
			}

			private readonly Regex _pattern;
		}
		private class EnclosedAtomSpecification
			: AtomSpecification
		{
			public EnclosedAtomSpecification(KeplerLanguage.AtomCode code, string begining, string end, bool canIgnore, bool isSeparator, bool ignoreCase)
				: base(code, canIgnore, isSeparator)
			{
				if (begining == null)
					throw new ArgumentNullException("begining");
				if (begining.Length == 0)
					throw new ArgumentException("The begining cannot an empty string!", "begining");
				if (end == null)
					throw new ArgumentNullException("end");
				if (end.Length == 0)
					throw new ArgumentException("The end cannot an empty string!", "end");
				_beginning = begining;
				_end = end;
				_ignoreCase = ignoreCase;
			}

			public override bool TryIdentifty(string text, out string atom, int startIndex = 0)
			{
				atom = null;
				if (text == null || string.Compare(_beginning, 0, text, startIndex, _beginning.Length, _ignoreCase) != 0)
					return false;
				int endIndex = startIndex + _beginning.Length;
				do
					endIndex = text.IndexOf(_end, endIndex);
				while (endIndex != -1 && text[endIndex - 1] == '\\');
				if (endIndex != -1)
					atom = text.Substring(startIndex, endIndex - startIndex);
				return (atom != null);
			}
			public override bool IsSatisfiedBy(string str, int startIndex = 0)
			{
				string atom;
				return (TryIdentifty(str, out atom, startIndex) && atom.Length == str.Length);
			}

			private readonly bool _ignoreCase;
			private readonly string _beginning;
			private readonly string _end;
		}
	}
}