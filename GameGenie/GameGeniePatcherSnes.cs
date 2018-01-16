using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    public static class GameGeniePatcherSnes
    {
        public static byte[] Patch(byte[] data, string code)
        {
            code = code.ToUpper().Trim();
            if (string.IsNullOrEmpty(code)) return data;
            if (code.Length != 9 || code[4] != '-')
                throw new GameGenieFormatException(code);
            code = Regex.Replace(code, "[^0-9A-F]", "");
            if (code.Length != 8)
                throw new GameGenieFormatException(code);

            byte value = Convert.ToByte(code.Substring(0, 2), 16);
            code = code.Substring(2);

            var binaryCode = new StringBuilder();
            foreach (char c in code)
                binaryCode.Append(letterValues[c]);
            var decoded = new StringBuilder(new string(' ', binaryCode.Length));
            for (int i = 0; i < binaryCode.Length; i++)
            {
                var c = codeSeq[i];
                var pos = clearSeq.IndexOf(c);
                decoded[pos] = binaryCode[i];
            }

            SnesGame.SnesRomType romType;
            string gameTitle;
            SnesGame.GetCorrectHeader(data, out romType, out gameTitle);

            UInt32 address = (Convert.ToUInt32(decoded.ToString(), 2)) & 0x3FFFFF;
            if (romType == SnesGame.SnesRomType.LoRom)
                address = address & 0x3FFFF;

            var result = (byte[])data.Clone();
            if (address >= result.Length)
                throw new GameGenieFormatException(code);
            result[address] = value;
            return result;
        }

        static Dictionary<char, string> letterValues = new Dictionary<char, string>()
        {
            { 'D', "0000" },
            { 'F', "0001" },
            { '4', "0010" },
            { '7', "0011" },
            { '0', "0100" },
            { '9', "0101" },
            { '1', "0110" },
            { '5', "0111" },
            { '6', "1000" },
            { 'B', "1001" },
            { 'C', "1010" },
            { '8', "1011" },
            { 'A', "1100" },
            { '2', "1101" },
            { '3', "1110" },
            { 'E', "1111" }
        };

        const string codeSeq = "ijklqrstopabcduvwxefghmn";
        const string clearSeq = "abcdefghijklmnopqrstuvwx";
    }
}
