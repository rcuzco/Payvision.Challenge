// <copyright file="FraudRadar.cs" company="Payvision">
// Copyright (c) Payvision. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Refactoring.FraudDetection
{
    public interface IFraudRadar
    {
        IEnumerable<FraudRadar.FraudResult> Check(string filePath);
        IEnumerable<FraudRadar.FraudResult> Check(IEnumerable<FraudRadar.Order> orders);
        bool CheckFileExists(string filePath);
        IEnumerable<FraudRadar.FraudResult> CheckOrdersAndGetFrauds(IEnumerable<FraudRadar.Order> orders);
        IEnumerable<FraudRadar.Order> GetOrders(string filePath);
        string NormalizeEmail(string email);
        IEnumerable<FraudRadar.Order> NormalizeOrders(IEnumerable<FraudRadar.Order> orders);
        string NormalizeState(string state);
        string NormalizeStreet(string street);
    }
}