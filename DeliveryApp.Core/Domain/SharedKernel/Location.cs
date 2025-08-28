using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    public class Location : ValueObject
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        private static readonly Random _random = new Random();

        private Location() { }

        public Location(int x, int y) : this()
        {
            try
            {
                SetX(x);
                SetY(y);
            }
            catch(ArgumentException e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public void SetX(int x)
        {
            if (x < 1)
                throw new ArgumentException(LocationErrors.xRangeError);

            if (x > 10) 
                throw new ArgumentException(LocationErrors.xRangeError);

            this.X = x;
        }

        public void SetY(int y)
        {
            if (y < 1)
                throw new ArgumentException(LocationErrors.yRangeError);

            if (y > 10)
                throw new ArgumentException(LocationErrors.yRangeError);

            this.Y = y;
        }

        public static Location CreateRandom()
        {

            int x = _random.Next(1, 11);
            int y = _random.Next(1, 11);

            return new Location(x, y);
        }


        public int DistanceTo(Location destination)
        {
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
