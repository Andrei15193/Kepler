namespace Andrei15193.Kepler.Language.Syntax
{
    public abstract class ExpressionElement
    {
        public abstract ExpressionElementType ElementType
        {
            get;
        }

        public override string ToString()
        {
            return ElementType.ToString();
        }
    }
}
