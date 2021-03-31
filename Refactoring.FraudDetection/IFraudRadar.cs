// <copyright file="FraudRadar.cs" company="Payvision">
// Copyright (c) Payvision. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Refactoring.FraudDetection
{
    public interface IFraudRadar
    {
        IEnumerable<FraudResult> Check(string filePath);
        IEnumerable<FraudResult> Check(IEnumerable<Order> orders);
        
        IEnumerable<FraudResult> CheckOrdersAndGetFrauds(IEnumerable<Order> orders);
        
        
    }
}