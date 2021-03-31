// <copyright file="PositiveBitCounter.cs" company="Payvision">
// Copyright (c) Payvision. All rights reserved.
// </copyright>

namespace Algorithms.CountingBits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PositiveBitCounter
    {
        public IEnumerable<int> Count(int input)
        {
            if (input < 0)
            {
                throw new ArgumentException("The value must be positive");
            }
            string numberBinary = Convert.ToString(input, 2); //Convert to binary in a string
            //Reverse the binary representation.
            var reversedBinary = numberBinary.Reverse().ToArray();

            //Count number of positive bits
            var positiveBitsCount = reversedBinary.Count(b => b == '1');
            List<int> positiveBitsPositions = new List<int>();
            for (int i = 0; i < reversedBinary.Length; i++)
            {
                if (reversedBinary[i] == '1')
                {
                    positiveBitsPositions.Add(i);
                }
            }
            List<int> returnValues = new List<int>();
            returnValues.Add(positiveBitsCount);
            returnValues.AddRange(positiveBitsPositions);
            return returnValues;
        }
    }
}
