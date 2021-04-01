// <copyright file="FraudRadar.cs" company="Payvision">
// Copyright (c) Payvision. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Refactoring.FraudDetection
{
    public interface IHelper
    {
        bool CheckFileExists(string filePath);
        string NormalizeEmail(string email);
        string NormalizeState(string state);
        string NormalizeStreet(string street);
        void ValidateFile(string filePath);
        IEnumerable<string> ReadAllOrderLines(string filePath);
    }
}