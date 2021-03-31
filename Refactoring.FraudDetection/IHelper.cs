// <copyright file="FraudRadar.cs" company="Payvision">
// Copyright (c) Payvision. All rights reserved.
// </copyright>

namespace Refactoring.FraudDetection
{
    public interface IHelper
    {
        bool CheckFileExists(string filePath);
        string NormalizeEmail(string email);
        string NormalizeState(string state);
        string NormalizeStreet(string street);
    }
}