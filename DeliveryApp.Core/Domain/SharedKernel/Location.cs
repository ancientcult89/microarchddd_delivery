using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    public partial class Location : ValueObject
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        private static readonly Random _random = new Random();

        public static Location MinLocation => new(1, 1);
        public static Location MaxLocation => new(10, 10);
        private Location() { }

        private Location(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public static Result<Location, Error> Create(int x, int y)
        {
            if (x < MinLocation.X || x > MaxLocation.X)
                return GeneralErrors.ValueIsNotInRange(XName, MinLocation.X, MaxLocation.X);
            if (y < MinLocation.Y || y > MaxLocation.Y)
                return GeneralErrors.ValueIsNotInRange(YName, MinLocation.Y, MaxLocation.Y);

            return new Location(x, y);
        }

        public static Result<Location, Error> CreateRandom()
        {

            int x = _random.Next(MinLocation.X, MaxLocation.X + RandomizerCorrectingUpperLimitValue);
            int y = _random.Next(MinLocation.Y, MaxLocation.Y + RandomizerCorrectingUpperLimitValue);

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
