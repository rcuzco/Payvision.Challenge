using System.Collections.Generic;

namespace Refactoring.FraudDetection
{
    public interface IOrderService
    {
        IEnumerable<FraudRadar.Order> GetOrders(string filePath);
        IEnumerable<FraudRadar.Order> NormalizeOrders(IEnumerable<FraudRadar.Order> orders);
    }
}