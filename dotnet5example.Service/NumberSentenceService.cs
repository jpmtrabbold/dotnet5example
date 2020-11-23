using System;
using System.Collections.Generic;
using System.Globalization;

namespace dotnet5example.Service
{
    public class NumberSentenceService
    {
        private readonly string[] thousands = new string[] { "THOUSAND", "MILLION", "BILLION", "TRILLION", "QUADRILLION", "QUINTILLION", "SEXTILLION", "ARE_YOU_SERIOUSILLION" };
        private readonly string[] tens = new string[] { "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };
        private readonly string[] teens = new string[] { "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
        private readonly string[] digits = new string[] { "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" };

        public string GenerateNumberSentence(decimal number)
        {
            var sentence = "";

            var s = number.ToString();
            var parts = s.Split(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);

            var integerPart = parts[0];

            // makes sure it's divided in groups of three
            if ((integerPart.Length % 3) != 0)
                integerPart = integerPart.PadLeft((integerPart.Length + 3) - (integerPart.Length % 3), '0');

            // create list of groups of three
            var thousandGroups = StringChunks(integerPart, 3);

            for (int i = 0; i < thousandGroups.Count; i++)
            {
                var group = thousandGroups[i];
                var sub = GenerateSubThousandSentence(group);
                if (!string.IsNullOrWhiteSpace(sub))
                {
                    var prev = (string.IsNullOrWhiteSpace(sentence) ? "" : (i + 1 == thousandGroups.Count) ? " AND " : ", ");
                    var thousandsIndex = thousandGroups.Count - 2 - i;
                    var thousandsSentence = thousandsIndex >= 0 ? " "+thousands[thousandsIndex] : "";
                    sentence += $"{prev}{sub}{thousandsSentence}";
                }
            }

            if (integerPart == "000")
                sentence += "ZERO";

            sentence += " DOLLAR" + (integerPart != "001" ? "S" : "");

            var decimalPart = parts.Length > 1 ? parts[1] : "";
            if (!string.IsNullOrWhiteSpace(decimalPart))
            {
                if (decimalPart.Length > 2)
                    decimalPart = decimalPart.Substring(0, 2); // truncate any additional decimal other than 2
                
                decimalPart = decimalPart.PadRight(2, '0');
                var sub = GenerateSubThousandSentence(decimalPart);
                if (!string.IsNullOrWhiteSpace(sub))
                    sentence += $" AND {sub} CENT{(decimalPart != "01" ? "S" : "")}";
            }

            return sentence;
        }
        private string GenerateSubThousandSentence(string numberParam)
        {
            var number = (numberParam.Length == 3 ? $"{numberParam}" : numberParam.PadLeft(3, '0'));
            var sentence = "";
            var hundred = number[0];
            var ten = number[1];
            var digit = number[2];

            var hasHundred = hundred != '0';
            var hasTen = ten != '0';
            var hasDigit = digit != '0';

            if (hasHundred)
            {
                hasHundred = true;
                var hundredIndex = int.Parse(hundred.ToString()) - 1;
                sentence += $"{digits[hundredIndex]} HUNDRED";
            }

            if (hasTen)
            {
                if (ten == '1')
                {
                    if (!hasDigit)
                    {
                        if (hasHundred)
                            sentence += " AND ";
                        sentence += tens[0];
                    }
                }
                else
                {
                    if (hasHundred)
                        sentence += " AND ";

                    var tenIndex = int.Parse(ten.ToString()) - 1;
                    sentence += $"{tens[tenIndex]}";
                }
            }

            if (hasDigit)
            {
                if (hasTen)
                {
                    if (ten == '1')
                    {
                        var teenIndex = int.Parse(digit.ToString()) - 1;
                        if (teenIndex != -1)
                        {
                            if (hasHundred)
                                sentence += " AND ";

                            sentence += teens[teenIndex];
                        }
                    }
                    else
                    {
                        var digitIndex = int.Parse(digit.ToString()) - 1;
                        sentence += "-" + digits[digitIndex];
                    }
                }
                else
                {
                    var digitIndex = int.Parse(digit.ToString()) - 1;
                    if (digitIndex != -1)
                    {
                        if (hasHundred)
                            sentence += " AND ";

                        sentence += digits[digitIndex];
                    }
                }
            }
            return sentence;
        }

        private static List<string> StringChunks(string str, int chunkSize)
        {
            var list = new List<string>();
            for (int i = 0; i < str.Length; i += chunkSize)
                list.Add(str.Substring(i, chunkSize));
            return list;
        }

    }
}
