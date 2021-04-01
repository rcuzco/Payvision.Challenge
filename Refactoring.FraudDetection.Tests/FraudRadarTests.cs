﻿// <copyright file="FraudRadarTests.cs" company="Payvision">
// Copyright (c) Payvision. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Refactoring.FraudDetection.Tests
{
    [TestClass]
    public class FraudRadarTests
    {
        [TestMethod]
        [DeploymentItem("./Files/OneLineFile.txt", "Files")]
        public void CheckFraud_OneLineFile_NoFraudExpected()
        {
            var result = ExecuteTest(Path.Combine(Environment.CurrentDirectory, "Files", "OneLineFile.txt"));

            result.Should().NotBeNull("The result should not be null.");
            result.Should().HaveCount(0, "The result should not contain fraudulent lines");
        }

        [TestMethod]
        [DeploymentItem("./Files/TwoLines_FraudulentSecond.txt", "Files")]
        public void CheckFraud_TwoLines_SecondLineFraudulent()
        {
            var result = ExecuteTest(Path.Combine(Environment.CurrentDirectory, "Files", "TwoLines_FraudulentSecond.txt"));

            result.Should().NotBeNull("The result should not be null.");
            result.Should().HaveCount(1, "The result should contains the number of lines of the file");
            result.First().IsFraudulent.Should().BeTrue("The first line is not fraudulent");
            result.First().OrderId.Should().Be(2, "The first line is not fraudulent");
        }

        [TestMethod]
        [DeploymentItem("./Files/ThreeLines_FraudulentSecond.txt", "Files")]
        public void CheckFraud_ThreeLines_SecondLineFraudulent()
        {
            var result = ExecuteTest(Path.Combine(Environment.CurrentDirectory, "Files", "ThreeLines_FraudulentSecond.txt"));

            result.Should().NotBeNull("The result should not be null.");
            result.Should().HaveCount(1, "The result should contains the number of lines of the file");
            result.First().IsFraudulent.Should().BeTrue("The first line is not fraudulent");
            result.First().OrderId.Should().Be(2, "The first line is not fraudulent");
        }

        [TestMethod]
        [DeploymentItem("./Files/FourLines_MoreThanOneFraudulent.txt", "Files")]
        public void CheckFraud_FourLines_MoreThanOneFraudulent()
        {
            var result = ExecuteTest(Path.Combine(Environment.CurrentDirectory, "Files", "FourLines_MoreThanOneFraudulent.txt"));

            result.Should().NotBeNull("The result should not be null.");
            result.Should().HaveCount(2, "The result should contains the number of lines of the file");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void CheckFraud_FileDoesNotExist()
        {
            var result = ExecuteTest(Path.Combine(Environment.CurrentDirectory, "Files", "NoneExistingFile.txt"));            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckFraud_EmptyFilePath()
        {
            var result = ExecuteTest(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void CheckFraud_FileIsNotCompliant()
        {
            var result = ExecuteTest(Path.Combine(Environment.CurrentDirectory, "Files", "NotCompliantFile.txt"));
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void CheckFraud_FileIsNotCompliantComasMissing()
        {
            var result = ExecuteTest(Path.Combine(Environment.CurrentDirectory, "Files", "NotCompliantFileComasMissing.txt"));
        }

        private static List<FraudResult> ExecuteTest(string filePath)
        {
            IHelper helper = new Helper();
            IOrderService orderService = new OrderService(helper);
            IFraudRadar fraudRadar = new FraudRadar(helper, orderService);

            //instead of passing the filePath, we will read it outside of the check method and pass
            //it the set of extracted lines from the file in the filepath
            var orders = orderService.GetOrders(filePath);
            return fraudRadar.Check(orders).ToList();
        }
    }
}