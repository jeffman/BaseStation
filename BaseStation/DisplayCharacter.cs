using System;
using System.Collections.Generic;

namespace BaseStation
{
    public struct DisplayCharacter
    {
        public static DisplayCharacter Full { get; } = DisplayCharacter.FromRaw(0b11111111);

        public static DisplayCharacter Empty { get; } = DisplayCharacter.FromRaw(0);

        private static byte[] _digitValues =
        {
            0b_0_111111_0,
            0b_0_110000_0,
            0b_1_011011_0,
            0b_1_111001_0,
            0b_1_110100_0,
            0b_1_011101_0,
            0b_1_101111_0,
            0b_0_111000_0,
            0b_1_111111_0,
            0b_1_111101_0
        };

        private static Dictionary<char, byte> _symbolValues = new Dictionary<char, byte>
        {
            [' '] = 0b_0_000000_0,
            ['.'] = 0b_0_000000_1,
            ['_'] = 0b_0_000001_0,
            ['-'] = 0b_1_000000_0,
            ['?'] = 0b_1_011010_0,
            ['!'] = 0b_0_110000_1,
            ['A'] = 0b_1_111110_0,
            ['b'] = 0b_1_100111_0,
            ['c'] = 0b_1_000011_0,
            ['C'] = 0b_0_001111_0,
            ['d'] = 0b_1_110011_0,
            ['E'] = 0b_1_001111_0,
            ['F'] = 0b_1_001110_0,
            ['g'] = 0b_1_111101_0,
            ['h'] = 0b_1_100110_0,
            ['H'] = 0b_1_110110_0,
            ['i'] = 0b_0_100000_0,
            ['I'] = 0b_0_110000_0,
            ['J'] = 0b_0_110011_0,
            ['k'] = 0b_1_000110_0,
            ['l'] = 0b_0_110000_0,
            ['L'] = 0b_0_000111_0,
            ['m'] = 0b_1_110010_0,
            ['n'] = 0b_1_100010_0,
            ['N'] = 0b_0_111110_0,
            ['o'] = 0b_1_100011_0,
            ['O'] = 0b_0_111111_0,
            ['P'] = 0b_1_011110_0,
            ['q'] = 0b_1_111100_0,
            ['r'] = 0b_1_000010_0,
            ['S'] = 0b_1_011101_0,
            ['t'] = 0b_1_000111_0,
            ['u'] = 0b_0_100011_0,
            ['U'] = 0b_0_110111_0,
            ['V'] = 0b_1_010110_0,
            ['w'] = 0b_1_110100_0,
            ['x'] = 0b_1_110110_0,
            ['y'] = 0b_1_110101_0,
            ['Z'] = 0b_1_011011_0
        };

        public byte Value { get; }

        private DisplayCharacter(byte value)
        {
            Value = value;
        }

        public static DisplayCharacter FromRaw(byte value)
            => new DisplayCharacter(value);

        public static DisplayCharacter FromDigit(int digit)
            => FromDigit(digit, false);

        public static DisplayCharacter FromDigit(int digit, bool dot)
        {
            if (digit < 0 || digit > 9)
                throw new ArgumentOutOfRangeException(nameof(digit));

            byte digitValue = _digitValues[digit];
            return new DisplayCharacter(Dotify(digitValue, dot));
        }

        public static DisplayCharacter FromSymbol(char symbol)
            => FromSymbol(symbol, false);

        public static DisplayCharacter FromSymbol(char symbol, bool dot)
        {
            if (_symbolValues.TryGetValue(symbol, out byte value) ||
                _symbolValues.TryGetValue(char.ToUpperInvariant(symbol), out value) ||
                _symbolValues.TryGetValue(char.ToLowerInvariant(symbol), out value))
                return new DisplayCharacter(Dotify(value, dot));

            return new DisplayCharacter(Dotify(_symbolValues['?'], dot));
        }

        private static byte Dotify(byte value, bool dot)
        {
            return dot ? (byte)(value | 1) : value;
        }
    }
}