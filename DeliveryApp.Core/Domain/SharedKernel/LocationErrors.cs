using Primitives;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    public static class LocationErrors
    {
        public static Error LocationNotSpecified()
        {
            return new Error("location.not.specified", $"Location not specified");
        }
    }
}
