// <copyright file="FraudRadar.cs" company="Payvision">
// Copyright (c) Payvision. All rights reserved.
// </copyright>

namespace Refactoring.FraudDetection
{
    using System;
    using System.Collections.Generic;    
    using System.Linq;

   

    public class FraudRadar : IFraudRadar
    {
        private readonly IHelper _helper;
        private readonly IOrderService _orderService;

        public FraudRadar(IHelper helper, IOrderService orderService)
        {
            _helper = helper;
            _orderService = orderService;
        }
        
        public IEnumerable<FraudResult> CheckOrdersAndGetFrauds(IEnumerable<Order> ordersRaw)
        {
            ordersRaw = ordersRaw ?? throw new ArgumentNullException(nameof(ordersRaw));

            var orders = ordersRaw.ToList();
            List<FraudResult> fraudResults = new List<FraudResult>();
            for (int i = 0; i < orders.Count ; i++)
            {
                var current = orders.ElementAt(i);
                bool isFraudulent = false;

                for (int j = i + 1; j < orders.Count; j++)
                {
                    isFraudulent = CheckIfOrderIsFraudulent(orders.ElementAt(j), current);

                    if (isFraudulent)
                    {
                        //this way we return each order right afeter being checked ;-)
                        yield return new FraudResult { IsFraudulent = true, OrderId = orders[j].OrderId };
                    }
                }
            }            
        }

        public bool CheckIfOrderIsFraudulent(Order theActualOrder, Order current)
        {
            theActualOrder = theActualOrder ?? throw new ArgumentNullException(nameof(theActualOrder));
            current = current ?? throw new ArgumentNullException(nameof(theActualOrder));

            bool isFraudulent = false;
            if (current.DealId == theActualOrder.DealId
                && current.Email == theActualOrder.Email
                && current.CreditCard != theActualOrder.CreditCard)
            {
                isFraudulent = true;
            }

            if (current.DealId == theActualOrder.DealId
                && current.State == theActualOrder.State
                && current.ZipCode == theActualOrder.ZipCode
                && current.Street == theActualOrder.Street
                && current.City == theActualOrder.City
                && current.CreditCard != theActualOrder.CreditCard)
            {
                isFraudulent = true;
            }

            return isFraudulent;
        }

        public IEnumerable<FraudResult> Check(string filePath)
        {
            _helper.ValidateFile(filePath);                        
            //GET ORDERS // READ FRAUD LINES            
            var orders = _orderService.GetOrders(filePath);
            var result = Check(orders);
            return result;            
        }

        public IEnumerable<FraudResult> Check(IEnumerable<Order> orders)
        {
            orders = orders ?? throw new ArgumentNullException(nameof(orders));

            IEnumerable<FraudResult> fraudResults = Enumerable.Empty<FraudResult>();
            //GET ORDERS // READ FRAUD LINES                        
            if (orders.Any())
            {
                // NORMALIZE
                orders = _orderService.NormalizeOrders(orders).ToList();
                // CHECK FRAUD
                fraudResults = CheckOrdersAndGetFrauds(orders).ToList();
            }
            return fraudResults;
        }

        

        
    }
}