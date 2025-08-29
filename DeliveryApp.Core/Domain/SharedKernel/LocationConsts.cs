using CSharpFunctionalExtensions;
using System.Runtime.CompilerServices;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    public  partial class Location : ValueObject
    {
        public const int XStartLocation = 1;
        public const int XEndLocation = 10;

        public const int YStartLocation = 1;
        public const int YEndLocation = 10;

        public const int RandomizerCorrectingUpperLimitValue = 1;

        public const string XName = nameof(X);
        public const string YName = nameof(Y);
    }
}
