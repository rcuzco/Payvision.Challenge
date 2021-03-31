using System.Collections.Generic;

namespace Refactoring.FraudDetection
{
    public interface IOrderService
    {
        IEnumerable<Order> GetOrders(string filePath);
        IEnumerable<Order> NormalizeOrders(IEnumerable<Order> orders);
    }
}