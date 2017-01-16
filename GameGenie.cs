using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.Famicom
{
    public static class GameGenie
    {
        public static byte[] Patch(byte[] data, string code)
        {
            code = code.ToUpper().Trim();
            if (string.IsNullOrEmpty(code)) return data;
            var result = (byte[])data.Clone();

            var binaryCode = new StringBuilder(code);
            foreach (var l in letterValues.Keys)
                binaryCode.Replace(l.ToString(), letterValues[l]);

            byte value, compare;
            Int16 address;

            if (binaryCode.Length == 24)
            {
                if (binaryCode[8] != '0') throw new GameGenieFormatException();

                try
                {
                    value = Convert.ToByte(new string(new char[] { binaryCode[0], binaryCode[5], binaryCode[6], binaryCode[7], binaryCode[20], binaryCode[1], binaryCode[2], binaryCode[3] }), 2);
                    address = Convert.ToInt16(new string(new char[] { binaryCode[13], binaryCode[14], binaryCode[15], binaryCode[16], binaryCode[21], binaryCode[22], binaryCode[23], binaryCode[4], binaryCode[9], binaryCode[10], binaryCode[11], binaryCode[12], binaryCode[17], binaryCode[18], binaryCode[19] }), 2);
                }
                catch
                {
                    throw new GameGenieFormatException();
                }

                if (result.Length <= 0x8000)
                {
                    result[result.Length % address] = value;
                }
                else
                {
                    int pos = address % 0x2000;
                    while (pos < result.Length)
                    {
                        result[pos] = value;
                        pos += 0x2000;
                    }
                }
            }
            else if (binaryCode.Length == 32)
            {
                if (binaryCode[8] != '1') throw new GameGenieFormatException();

                try
                {
                    value = Convert.ToByte(new string(new char[] { binaryCode[0], binaryCode[5], binaryCode[6], binaryCode[7], binaryCode[28], binaryCode[1], binaryCode[2], binaryCode[3] }), 2);
                    address = Convert.ToInt16(new string(new char[] { binaryCode[13], binaryCode[14], binaryCode[15], binaryCode[16], binaryCode[21], binaryCode[22], binaryCode[23], binaryCode[4], binaryCode[9], binaryCode[10], binaryCode[11], binaryCode[12], binaryCode[17], binaryCode[18], binaryCode[19] }), 2);
                    compare = Convert.ToByte(new string(new char[] { binaryCode[24], binaryCode[29], binaryCode[30], binaryCode[31], binaryCode[20], binaryCode[25], binaryCode[26], binaryCode[27] }), 2);
                }
                catch
                {
                    throw new GameGenieFormatException();
                }

                bool replaced = false;
                int pos = address % 0x2000;
                while (pos < result.Length)
                {
                    if (result[pos] == compare)
                    {
                        result[pos] = value;
                        replaced = true;
                    }
                    pos += 0x2000;
                }
                if (!replaced) throw new GameGenieNotFoundException("Can't apply code: " + code);
            }
            else throw new GameGenieFormatException();

            return result;
        }

        static Dictionary<char, string> letterValues = new Dictionary<char, string>()
        {
            { 'A', "0000" },
            { 'P', "0001" },
            { 'Z', "0010" },
            { 'L', "0011" },
            { 'G', "0100" },
            { 'I', "0101" },
            { 'T', "0110" },
            { 'Y', "0111" },
            { 'E', "1000" },
            { 'O', "1001" },
            { 'X', "1010" },
            { 'U', "1011" },
            { 'K', "1100" },
            { 'S', "1101" },
            { 'V', "1110" },
            { 'N', "1111" }
        };
    }

    public class GameGenieFormatException : Exception
    {
        public GameGenieFormatException()
            : base("Неправильный формат Game Genie кода")
        {
        }
        public GameGenieFormatException(string message) : base(message)
        {
        }
    }

    public class GameGenieNotFoundException : Exception
    {
        public GameGenieNotFoundException(string message) : base(message)
        {
        }
    }
}
