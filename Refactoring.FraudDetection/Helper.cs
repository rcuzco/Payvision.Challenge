using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Refactoring.FraudDetection
{
    public class Helper : IHelper
    {
        public string NormalizeEmail(string email)
        {
            /*
             perhaps we could just throw an argumentNullException... it could depend on the functional 
            specifications, for now we just return null 
             */
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }
            if (!email.Contains("@"))
            {
                throw new ArgumentException(FraudConstants.EmailMustContainAtSymbol);
            }
            string normalizedEmail = string.Empty;
            var aux = email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

            aux[0] = atIndex < 0 ? aux[0]
                .Replace(".", "") : aux[0]
                .Replace(".", "")
                .Remove(atIndex);

            normalizedEmail = string.Join("@", new string[] { aux[0], aux[1] });
            return normalizedEmail;
        }

        public string NormalizeStreet(string street)
        {
            var normalizedStreet = string.Empty;
            normalizedStreet = street
                .Replace("st.", FraudConstants.StreetLiteral)
                .Replace("rd.", FraudConstants.RoadLiteral);
            return normalizedStreet;
        }

        public string NormalizeState(string state)
        {
            var normalizedState = string.Empty;
            normalizedState = state
                .Replace("il", FraudConstants.Illinois)
                .Replace("ca", FraudConstants.California)
                .Replace("ny", FraudConstants.NewYork);
            return normalizedState;
        }

        public bool CheckFileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void ValidateFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }            
            if (!CheckFileExists(filePath)) throw new FileNotFoundException(FraudConstants.FileDoesNotExist, filePath);
        }

        public IEnumerable<string> ReadAllOrderLines(string filePath)
        {
            ValidateFile(filePath);
            return File.ReadAllLines(filePath);
        }
    }
}
