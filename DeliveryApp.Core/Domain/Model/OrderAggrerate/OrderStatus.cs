using CSharpFunctionalExtensions;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.OrderAggrerate
{
    [ExcludeFromCodeCoverage]
    public class OrderStatus : ValueObject
    {
        public static OrderStatus Created => new(nameof(Created).ToLowerInvariant());
        public static OrderStatus Assigned => new(nameof(Assigned).ToLowerInvariant());
        public static OrderStatus Completed => new(nameof(Completed).ToLowerInvariant());

        public string Name { get; private set; }
        private OrderStatus() { }
        private OrderStatus(string name) : this()
        {
            Name = name;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}
