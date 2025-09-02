using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    public partial class Location : ValueObject
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        private static readonly Random _random = new Random();

        private Location() { }

        private Location(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public static Result<Location, Error> Create(int x, int y)
        {
            if (x < XStartLocation || x > XEndLocation)
                return GeneralErrors.ValueIsNotInRange(XName, XStartLocation, XEndLocation);
            if (y < YStartLocation || y > YEndLocation)
                return GeneralErrors.ValueIsNotInRange(YName, XStartLocation, XEndLocation);

            return new Location(x, y);
        }

        public static Result<Location, Error> CreateRandom()
        {

            int x = _random.Next(XStartLocation, XEndLocation + RandomizerCorrectingUpperLimitValue);
            int y = _random.Next(YStartLocation, YEndLocation + RandomizerCorrectingUpperLimitValue);

            return Location.Create(x, y);
        }


        public Result<int, Error> DistanceTo(Location destination)
        {
            if(destination == null)
                return GeneralErrors.ValueIsRequired(nameof(destination));

            int distanceX = Math.Abs(this.X - destination.X);
            int distanceY = Math.Abs(this.Y - destination.Y);

            return distanceX + distanceY;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }
    }
}
