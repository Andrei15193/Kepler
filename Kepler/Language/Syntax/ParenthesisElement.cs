namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class ParenthesisElement
        : ExpressionElement
    {
        internal ParenthesisElement(bool isOpenning)
        {
            _isOpenning = isOpenning;
        }

        public override string ToString()
        {
            return base.ToString() + (_isOpenning ? " openning" : " closing");
        }

        public override ExpressionElementType ElementType
        {
            get
            {
                return ExpressionElementType.Parenthesis;
            }
        }

        public bool IsOpenning
        {
            get
            {
                return _isOpenning;
            }
        }

        private readonly bool _isOpenning;
    }
}
