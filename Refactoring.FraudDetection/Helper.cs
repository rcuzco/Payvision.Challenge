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

        public bool CheckFileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
