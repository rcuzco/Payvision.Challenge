using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Refactoring.FraudDetection.FraudRadar;

namespace Refactoring.FraudDetection
{
    public class OrderService : IOrderService
    {
        private readonly IHelper helper;

        public OrderService(IHelper helper)
        {
            this.helper = helper;
        }

        public IEnumerable<Order> GetOrders(string filePath)
        {
            var orders = new List<Order>();


            var lines = File.ReadAllLines(filePath);
            if (lines.Any(line => !line.Contains(",")))
            {
                throw new FormatException("File is not format compliant. Not all lines are well formatted");
            }

            foreach (var line in lines)
            {
                var items = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var order = new Order
                {
                    OrderId = int.Parse(items[0]),
                    DealId = int.Parse(items[1]),
                    Email = items[2].ToLower(),
                    Street = items[3].ToLower(),
                    City = items[4].ToLower(),
                    State = items[5].ToLower(),
                    ZipCode = items[6],
                    CreditCard = items[7]
                };

                orders.Add(order);
            }
            return orders;
        }

        public IEnumerable<Order> NormalizeOrders(IEnumerable<Order> orders)
        {
            List<Order> normalizedOrders = new List<Order>();
            foreach (var order in orders)
            {
                //Normalize email                
                order.Email = this.helper.NormalizeEmail(order.Email);

                //Normalize street
                order.Street = this.helper.NormalizeStreet(order.Street);

                //Normalize state
                order.State = this.helper.NormalizeState(order.State);
            }
            normalizedOrders.AddRange(orders);
            return normalizedOrders;
        }
    }
}
