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

        public string NormalizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }
            if (!email.Contains("@"))
            {
                throw new ArgumentException($"The email {email} is not valid");
            }
            string normalizedEmail = string.Empty;
            var aux = email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

            aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

            normalizedEmail = string.Join("@", new string[] { aux[0], aux[1] });
            return normalizedEmail;
        }

        public string NormalizeStreet(string street)
        {
            var normalizedStreet = string.Empty;
            normalizedStreet = street.Replace("st.", "street").Replace("rd.", "road");
            return normalizedStreet;
        }

        public string NormalizeState(string state)
        {
            var normalizedState = string.Empty;
            normalizedState = state.Replace("il", "illinois").Replace("ca", "california").Replace("ny", "new york");
            return normalizedState;
        }

        public IEnumerable<Order> NormalizeOrders(IEnumerable<Order> orders)
        {
            List<Order> normalizedOrders = new List<Order>();
            foreach (var order in orders)
            {
                //Normalize email                
                order.Email = NormalizeEmail(order.Email);

                //Normalize street
                order.Street = NormalizeStreet(order.Street);

                //Normalize state
                order.State = NormalizeState(order.State);
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

        public bool CheckFileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public IEnumerable<FraudResult> Check(string filePath)
        {
            if (!CheckFileExists(filePath))
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