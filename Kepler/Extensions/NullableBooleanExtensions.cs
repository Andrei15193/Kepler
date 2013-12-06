namespace Andrei15193.Kepler.Extensions
{
    static public class NullableBooleanExtensions
    {
        static public bool? And(this bool? left, bool? right)
        {
            if (left.HasValue && right.HasValue)
                return (left.Value && right.Value);
            else
                return null;
        }

        static public bool? Or(this bool? left, bool? right)
        {
            if (left.HasValue)
                if (right.HasValue)
                    return (left.Value || right.Value);
                else
                    return left;
            else
                return right;
        }

        static public bool? And(this bool? left, bool right)
        {
            if (left.HasValue)
                return (left.Value && right);
            else
                return null;
        }

        static public bool? Or(this bool? left, bool right)
        {
            if (left.HasValue)
                return (left.Value || right);
            else
                return right;
        }

        static public bool? And(this bool left, bool? right)
        {
            return right.And(left);
        }

        static public bool? Or(this bool left, bool? right)
        {
            return right.Or(left);
        }
    }
}
