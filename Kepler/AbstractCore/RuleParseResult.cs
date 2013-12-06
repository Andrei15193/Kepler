namespace Andrei15193.Kepler.AbstractCore
{
	public struct RuleParseResult<TCode>
		where TCode : struct
	{
		public RuleParseResult(string errorMessage)
		{
			_errorMessage = errorMessage;
			_parsedNode = null;
		}
		public RuleParseResult(ParsedNode<TCode> parsedNode)
		{
			_errorMessage = null;
			_parsedNode = parsedNode;
		}

		public string ErrorMessage
		{
			get
			{
				return _errorMessage;
			}
		}
		public ParsedNode<TCode> ParsedNode
		{
			get
			{
				return _parsedNode;
			}
		}

		private readonly string _errorMessage;
		private readonly ParsedNode<TCode> _parsedNode;
	}
}
