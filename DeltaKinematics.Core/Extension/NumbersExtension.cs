namespace DeltaKinematics.Core.Extension
{
    public static class NumbersExtension
    {
        public static bool Between(this int numberToCheck, int range)
        {
            return numberToCheck.Between(-range, range);
        }

        public static bool Between(this int numberToCheck, int bottom, int top)
        {
            return (numberToCheck > bottom && numberToCheck < top);
        }
    }
}