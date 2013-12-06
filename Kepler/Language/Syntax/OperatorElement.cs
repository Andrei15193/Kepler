namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class OperatorElement
        : ExpressionElement
    {
        internal OperatorElement(OperationName operationName)
        {
            _operationName = operationName;
            switch (_operationName)
            {
                case OperationName.Addition:
                case OperationName.Subtraction:
                case OperationName.Multiplication:
                case OperationName.Division:
                case OperationName.IntegerDivision:
                case OperationName.Modulo:
                case OperationName.LessThanComparison:
                case OperationName.LessThanOrEqualToComparison:
                case OperationName.EqualComparison:
                case OperationName.GreaterThanOrEqualToComparison:
                case OperationName.GreaterThanComparison:
                case OperationName.Disjunction:
                case OperationName.Conjuction:
                    _location = OperatorLocation.Infixed;
                    break;
                case OperationName.Negation:
                case OperationName.IntegerPromotion:
                case OperationName.AdditiveInverse:
                    _location = OperatorLocation.Prefixed;
                    break;
                default:
                    break;
            }
        }

        public override string ToString()
        {
            return (base.ToString() + " " + OperationName.ToString() + " (" + Location.ToString() + ")");
        }

        public override ExpressionElementType ElementType
        {
            get
            {
                return ExpressionElementType.Operator;
            }
        }

        public OperationName OperationName
        {
            get
            {
                return _operationName;
            }
        }

        public OperatorLocation Location
        {
            get
            {
                return _location;
            }
        }

        private readonly OperationName _operationName;
        private readonly OperatorLocation _location;
    }
}
