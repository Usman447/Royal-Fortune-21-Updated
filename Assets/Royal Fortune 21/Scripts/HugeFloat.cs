using System;
using System.Diagnostics;
using UnityEngine;

namespace RoyalFortune21
{
    public class HugeFloat
    {
        public string number;

        public HugeFloat(string number)
        {
            this.number = NormalizeNumber(number);
        }

        public static HugeFloat operator +(HugeFloat a, HugeFloat b)
        {
            string result = Add(a.number, b.number);
            return new HugeFloat(result);
        }

        public static HugeFloat operator -(HugeFloat a, HugeFloat b)
        {
            string result = Subtract(a.number, b.number);
            return new HugeFloat(result);
        }

        public static HugeFloat operator *(HugeFloat a, HugeFloat b)
        {
            string result = Multiply(a.number, b.number);
            return new HugeFloat(result);
        }

        private static string Add(string num1, string num2)
        {
            string[] parts1 = num1.Split('.');
            string[] parts2 = num2.Split('.');

            int integerPartLen1 = parts1[0].Length;
            int integerPartLen2 = parts2[0].Length;

            int decimalPartLen1 = (parts1.Length > 1) ? parts1[1].Length : 0;
            int decimalPartLen2 = (parts2.Length > 1) ? parts2[1].Length : 0;

            int maxIntegerPartLen = Math.Max(integerPartLen1, integerPartLen2);
            int maxDecimalPartLen = Math.Max(decimalPartLen1, decimalPartLen2);

            parts1[0] = parts1[0].PadLeft(maxIntegerPartLen, '0');
            parts2[0] = parts2[0].PadLeft(maxIntegerPartLen, '0');

            if (parts1.Length > 1)
                parts1[1] = parts1[1].PadRight(maxDecimalPartLen, '0');
            else
                parts1 = new string[] { parts1[0], "" };

            if (parts2.Length > 1)
                parts2[1] = parts2[1].PadRight(maxDecimalPartLen, '0');
            else
                parts2 = new string[] { parts2[0], "" };

            int carry = 0;
            string resultIntegerPart = "";
            string resultDecimalPart = "";

            for (int i = maxDecimalPartLen - 1; i >= 0; i--)
            {
                int digit1 = parts1[1][i] - '0';
                int digit2 = parts2[1][i] - '0';

                int sum = digit1 + digit2 + carry;
                carry = sum / 10;
                sum %= 10;

                resultDecimalPart = sum.ToString() + resultDecimalPart;
            }

            for (int i = maxIntegerPartLen - 1; i >= 0; i--)
            {
                int digit1 = parts1[0][i] - '0';
                int digit2 = parts2[0][i] - '0';

                int sum = digit1 + digit2 + carry;
                carry = sum / 10;
                sum %= 10;

                resultIntegerPart = sum.ToString() + resultIntegerPart;
            }

            if (carry > 0)
                resultIntegerPart = carry.ToString() + resultIntegerPart;

            string result = resultIntegerPart + (resultDecimalPart.Length > 0 ? "." + resultDecimalPart : "");
            return result.TrimStart('0');
        }

        private static string Subtract(string num1, string num2)
        {
            string[] parts1 = num1.Split('.');
            string[] parts2 = num2.Split('.');

            int integerPartLen1 = parts1[0].Length;
            int integerPartLen2 = parts2[0].Length;

            int decimalPartLen1 = (parts1.Length > 1) ? parts1[1].Length : 0;
            int decimalPartLen2 = (parts2.Length > 1) ? parts2[1].Length : 0;

            int maxIntegerPartLen = Math.Max(integerPartLen1, integerPartLen2);
            int maxDecimalPartLen = Math.Max(decimalPartLen1, decimalPartLen2);

            parts1[0] = parts1[0].PadLeft(maxIntegerPartLen, '0');
            parts2[0] = parts2[0].PadLeft(maxIntegerPartLen, '0');

            if (parts1.Length > 1)
                parts1[1] = parts1[1].PadRight(maxDecimalPartLen, '0');
            else
                parts1 = new string[] { parts1[0], "" };

            if (parts2.Length > 1)
                parts2[1] = parts2[1].PadRight(maxDecimalPartLen, '0');
            else
                parts2 = new string[] { parts2[0], "" };

            int borrow = 0;
            string resultIntegerPart = "";
            string resultDecimalPart = "";

            for (int i = maxDecimalPartLen - 1; i >= 0; i--)
            {
                int digit1 = parts1[1][i] - '0';
                int digit2 = parts2[1][i] - '0';

                digit1 -= borrow;

                if (digit1 < digit2)
                {
                    digit1 += 10;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                int diff = digit1 - digit2;
                resultDecimalPart = diff.ToString() + resultDecimalPart;
            }

            for (int i = maxIntegerPartLen - 1; i >= 0; i--)
            {
                int digit1 = parts1[0][i] - '0';
                int digit2 = parts2[0][i] - '0';

                digit1 -= borrow;

                if (digit1 < digit2)
                {
                    digit1 += 10;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                int diff = digit1 - digit2;
                resultIntegerPart = diff.ToString() + resultIntegerPart;
            }

            return NormalizeNumber(resultIntegerPart + (resultDecimalPart.Length > 0 ? "." + resultDecimalPart : ""));
        }

        private static string Multiply(string num1, string num2)
        {
            string[] parts1 = num1.Split('.');
            string[] parts2 = num2.Split('.');

            int integerPartLen1 = parts1[0].Length;
            int integerPartLen2 = parts2[0].Length;

            int decimalPartLen1 = (parts1.Length > 1) ? parts1[1].Length : 0;
            int decimalPartLen2 = (parts2.Length > 1) ? parts2[1].Length : 0;

            int maxIntegerPartLen = Math.Max(integerPartLen1, integerPartLen2);
            int maxDecimalPartLen = decimalPartLen1 + decimalPartLen2;

            parts1[0] = parts1[0].PadLeft(maxIntegerPartLen, '0');
            parts2[0] = parts2[0].PadLeft(maxIntegerPartLen, '0');

            if (parts1.Length > 1)
                parts1[1] = parts1[1].PadRight(decimalPartLen1 + decimalPartLen2, '0');
            else
                parts1 = new string[] { parts1[0], "".PadRight(decimalPartLen1 + decimalPartLen2, '0') };

            if (parts2.Length > 1)
                parts2[1] = parts2[1].PadRight(decimalPartLen1 + decimalPartLen2, '0');
            else
                parts2 = new string[] { parts2[0], "".PadRight(decimalPartLen1 + decimalPartLen2, '0') };

            int[] products = new int[maxIntegerPartLen + maxDecimalPartLen];

            for (int i = maxDecimalPartLen - 1; i >= 0; i--)
            {
                int digit2 = parts2[1][i] - '0';

                if (digit2 == 0)
                    continue;

                int position1 = i;
                int position2 = i + 1;

                for (int j = maxIntegerPartLen - 1; j >= 0; j--)
                {
                    int digit1 = parts1[0][j] - '0';

                    int product = digit1 * digit2 + products[position2];
                    products[position1] += product / 10;
                    products[position2] = product % 10;

                    position1--;
                    position2--;
                }
            }

            string result = "";
            int carry = 0;

            for (int i = maxIntegerPartLen + maxDecimalPartLen - 1; i >= 0; i--)
            {
                int sum = products[i] + carry;
                carry = sum / 10;
                sum %= 10;

                result = sum.ToString() + result;
            }

            string integerPart = result.Substring(0, maxIntegerPartLen);
            string decimalPart = result.Substring(maxIntegerPartLen).TrimEnd('0');

            if (decimalPart.Length > 0)
                return NormalizeNumber(integerPart + "." + decimalPart);
            else
                return NormalizeNumber(integerPart);
        }

        public static bool operator <(HugeFloat a, HugeFloat b)
        {
            return Compare(a.number, b.number) < 0;
        }

        public static bool operator >(HugeFloat a, HugeFloat b)
        {
            return Compare(a.number, b.number) > 0;
        }

        public static bool operator <=(HugeFloat a, HugeFloat b)
        {
            return Compare(a.number, b.number) <= 0;
        }

        public static bool operator >=(HugeFloat a, HugeFloat b)
        {
            int com = Compare(a.number, b.number);
            UnityEngine.Debug.Log($"{com} {a.number} {b.number}");

            return com >= 0;
        }

        public static bool operator ==(HugeFloat a, HugeFloat b)
        {
            return Compare(a.number, b.number) == 0;
        }

        public static bool operator !=(HugeFloat a, HugeFloat b)
        {
            return Compare(a.number, b.number) != 0;
        }

        public static HugeFloat NormalizeNumber(float number)
        {
            string num = number.ToString();
            string newNum = "";

            foreach (char n in num)
            {
                if (n != ',')
                {
                    newNum += n;
                }
            }

            return new HugeFloat(newNum);
        }

        private static int Compare(string num1, string num2)
        {
            if (num1 == num2)
                return 0;

            bool isNegative1 = num1.StartsWith("-");
            bool isNegative2 = num2.StartsWith("-");

            if (isNegative1 && !isNegative2)
                return -1;
            if (!isNegative1 && isNegative2)
                return 1;

            bool isSwap = false;
            if (isNegative1 && isNegative2)
            {
                num1 = num1.Substring(1);
                num2 = num2.Substring(1);
                isSwap = true;
            }

            if (num1.Length < num2.Length)
                return isSwap ? 1 : -1;
            if (num1.Length > num2.Length)
                return isSwap ? -1 : 1;

            for (int i = 0; i < num1.Length; i++)
            {
                if (num1[i] < num2[i])
                    return isSwap ? 1 : -1;
                if (num1[i] > num2[i])
                    return isSwap ? -1 : 1;
            }

            return 0;
        }

        private static string NormalizeNumber(string number)
        {
            number = number.TrimStart('0');

            if (number.Length == 0)
                return "0.0";

            if (number[0] == '.')
                number = "0" + number;

            if (number[number.Length - 1] == '.')
                number += "0";

            if (!isDot(number))
            {
                number += ".0";
            }

            return number;
        }

        static bool isDot(string _number)
        {
            for (int i = 0; i < _number.Length; i++)
            {
                if (_number[i] == '.')
                    return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is HugeFloat @float &&
                   number == @float.number;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(number);
        }
    }
}
    