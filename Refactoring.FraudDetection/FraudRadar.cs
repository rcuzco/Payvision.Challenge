// <copyright file="FraudRadar.cs" company="Payvision">
// Copyright (c) Payvision. All rights reserved.
// </copyright>

namespace Refactoring.FraudDetection
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

   

    public class FraudRadar : IFraudRadar
    {
        private readonly IHelper helper;
                
        public FraudRadar(IHelper helper)
        {
            this.helper = helper;
        }


        public IEnumerable<Order> GetOrders(string filePath)
        {
            var orders = new List<Order>();


            var lines = File.ReadAllLines(filePath);
            if (lines.Any(line=>!line.Contains(",")))
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

        public IEnumerable<FraudResult> CheckOrdersAndGetFrauds(IEnumerable<Order> ordersRaw)
        {
            var orders = ordersRaw.ToList();
            List<FraudResult> fraudResults = new List<FraudResult>();
            for (int i = 0; i < orders.ToList().Count; i++)
            {
                var current = orders[i];
                bool isFraudulent = false;

                for (int j = i + 1; j < orders.Count; j++)
                {
                    isFraudulent = false;

                    if (current.DealId == orders[j].DealId
                        && current.Email == orders[j].Email
                        && current.CreditCard != orders[j].CreditCard)
                    {
                        isFraudulent = true;
                    }

                    if (current.DealId == orders[j].DealId
                        && current.State == orders[j].State
                        && current.ZipCode == orders[j].ZipCode
                        && current.Street == orders[j].Street
                        && current.City == orders[j].City
                        && current.CreditCard != orders[j].CreditCard)
                    {
                        isFraudulent = true;
                    }

                    if (isFraudulent)
                    {
                        fraudResults.Add(new FraudResult { IsFraudulent = true, OrderId = orders[j].OrderId });
                    }
                }
            }
            return fraudResults;
        }
        
        public IEnumerable<FraudResult> Check(string filePath)
        {
            if (!this.helper.CheckFileExists(filePath))
            {
                throw new FileNotFoundException("The file does not exist", filePath);
            }

            List<FraudResult> fraudResults = new List<FraudResult>();
            IEnumerable<Order> orders = Enumerable.Empty<Order>();
            //GET ORDERS // READ FRAUD LINES            
            orders = GetOrders(filePath);
            if (orders.Any())
            {
                // NORMALIZE
                orders = NormalizeOrders(orders).ToList();


                // CHECK FRAUD
                fraudResults = CheckOrdersAndGetFrauds(orders).ToList();
            }            

            return fraudResults;
        }

        public IEnumerable<FraudResult> Check(IEnumerable<Order> orders)
        {
            List<FraudResult> fraudResults = new List<FraudResult>();            
            //GET ORDERS // READ FRAUD LINES                        
            if (orders.Any())
            {
                // NORMALIZE
                orders = NormalizeOrders(orders).ToList();


                // CHECK FRAUD
                fraudResults = CheckOrdersAndGetFrauds(orders).ToList();
            }

            return fraudResults;
        }

        public class FraudResult
        {
            public int OrderId { get; set; }

            public bool IsFraudulent { get; set; }
        }

        public class Order
        {
            public int OrderId { get; set; }

            public int DealId { get; set; }

            public string Email { get; set; }

            public string Street { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string ZipCode { get; set; }

            public string CreditCard { get; set; }
        }
    }
}