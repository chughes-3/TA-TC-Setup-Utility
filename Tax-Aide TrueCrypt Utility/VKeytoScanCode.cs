using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxAide_TrueCrypt_Utility
{
    static public class VKeytoScanCodeCls
    {
        //Scan code list for sending keys
        private static Dictionary<char, int> vKeyToScanCode;
#region Dictionary Data Initialization
        static VKeytoScanCodeCls()
		    {
                VKeytoScanCodeCls.vKeyToScanCode = new Dictionary<char, int>();
                vKeyToScanCode.Add('a', 0x1E );
                vKeyToScanCode.Add('b', 0x30 );
                vKeyToScanCode.Add('c', 0x2E );
                vKeyToScanCode.Add('d', 0x20 );
                vKeyToScanCode.Add('e', 0x12 );
                vKeyToScanCode.Add('f', 0x21 );
                vKeyToScanCode.Add('g', 0x22 );
                vKeyToScanCode.Add('h', 0x23 );
                vKeyToScanCode.Add('i', 0x17 );
                vKeyToScanCode.Add('j', 0x24 );
                vKeyToScanCode.Add('k', 0x25 );
                vKeyToScanCode.Add('l', 0x26 );
                vKeyToScanCode.Add('m', 0x32 );
                vKeyToScanCode.Add('n', 0x31 );
                vKeyToScanCode.Add('o', 0x18 );
                vKeyToScanCode.Add('p', 0x19 );
                vKeyToScanCode.Add('q', 0x10 );
                vKeyToScanCode.Add('r', 0x13 );
                vKeyToScanCode.Add('s', 0x1F );
                vKeyToScanCode.Add('t', 0x14 );
                vKeyToScanCode.Add('u', 0x16 );
                vKeyToScanCode.Add('v', 0x2F );
                vKeyToScanCode.Add('w', 0x11 );
                vKeyToScanCode.Add('x', 0x2D );
                vKeyToScanCode.Add('y', 0x15 );
                vKeyToScanCode.Add('z', 0x2C );
                vKeyToScanCode.Add('A', 0x1E );
                vKeyToScanCode.Add('B', 0x30 );
                vKeyToScanCode.Add('C', 0x2E );
                vKeyToScanCode.Add('D', 0x20 );
                vKeyToScanCode.Add('E', 0x12 );
                vKeyToScanCode.Add('F', 0x21 );
                vKeyToScanCode.Add('G', 0x22 );
                vKeyToScanCode.Add('H', 0x23 );
                vKeyToScanCode.Add('I', 0x17 );
                vKeyToScanCode.Add('J', 0x24 );
                vKeyToScanCode.Add('K', 0x25 );
                vKeyToScanCode.Add('L', 0x26 );
                vKeyToScanCode.Add('M', 0x32 );
                vKeyToScanCode.Add('N', 0x31 );
                vKeyToScanCode.Add('O', 0x18 );
                vKeyToScanCode.Add('P', 0x19 );
                vKeyToScanCode.Add('Q', 0x10 );
                vKeyToScanCode.Add('R', 0x13 );
                vKeyToScanCode.Add('S', 0x1F );
                vKeyToScanCode.Add('T', 0x14 );
                vKeyToScanCode.Add('U', 0x16 );
                vKeyToScanCode.Add('V', 0x2F );
                vKeyToScanCode.Add('W', 0x11 );
                vKeyToScanCode.Add('X', 0x2D );
                vKeyToScanCode.Add('Y', 0x15 );
                vKeyToScanCode.Add('Z', 0x2C );
                vKeyToScanCode.Add(':', 0x27 );
                vKeyToScanCode.Add('<', 0x33 );
                vKeyToScanCode.Add('_', 0x0C );
                vKeyToScanCode.Add('>', 0x34 );
                vKeyToScanCode.Add('?', 0x35 );
                vKeyToScanCode.Add('~', 0x29 );
                vKeyToScanCode.Add('{', 0x1A );
                vKeyToScanCode.Add('}', 0x1B );
                vKeyToScanCode.Add('|', 0x2B );
                vKeyToScanCode.Add('\"', 0x28 );
                vKeyToScanCode.Add(';', 0x27 );
                vKeyToScanCode.Add(',', 0x33 );
                vKeyToScanCode.Add('-', 0x0C );
                vKeyToScanCode.Add('.', 0x34 );
                vKeyToScanCode.Add('/', 0x35 );
                vKeyToScanCode.Add('`', 0x29 );
                vKeyToScanCode.Add('[', 0x1A );
                vKeyToScanCode.Add(']', 0x1B );
                vKeyToScanCode.Add('\\', 0x2B );
                vKeyToScanCode.Add('\'', 0x28 );
                vKeyToScanCode.Add('0', 0x0B );
                vKeyToScanCode.Add('1', 0x2 );
                vKeyToScanCode.Add('2', 0x3 );
                vKeyToScanCode.Add('3', 0x4 );
                vKeyToScanCode.Add('4', 0x5 );
                vKeyToScanCode.Add('5', 0x6 );
                vKeyToScanCode.Add('6', 0x7 );
                vKeyToScanCode.Add('7', 0x8 );
                vKeyToScanCode.Add('8', 0x9 );
                vKeyToScanCode.Add('9', 0x0A );
                vKeyToScanCode.Add(' ', 0x39 );
                vKeyToScanCode.Add(')', 0x0B );
                vKeyToScanCode.Add('!', 0x2 );
                vKeyToScanCode.Add('@', 0x3 );
                vKeyToScanCode.Add('#', 0x4 );
                vKeyToScanCode.Add('$', 0x5 );
                vKeyToScanCode.Add('%', 0x6 );
                vKeyToScanCode.Add('^', 0x7 );
                vKeyToScanCode.Add('&', 0x8 );
                vKeyToScanCode.Add('*', 0x9 );
                vKeyToScanCode.Add('+', 0xD	);
                vKeyToScanCode.Add('=', 0xD );
                vKeyToScanCode.Add('\n', 0x1C);
                vKeyToScanCode.Add('\r', 0x1C);
                vKeyToScanCode.Add('\u001B', 0x1);  //Escape key
                vKeyToScanCode.Add('(', 0x0A);
                vKeyToScanCode.Add('\t', 0x0F);
            }
#endregion
        public static int Lookup(char c)
        {
            return VKeytoScanCodeCls.vKeyToScanCode[c];
        }
    }
}
