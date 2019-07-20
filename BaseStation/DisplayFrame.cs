using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BaseStation
{
    public class DisplayFrame
    {
        public static DisplayFrame Full { get; }

        public static DisplayFrame Empty { get; }

        private ReadOnlyDictionary<StatusLed, bool> _statusLeds;
        private ReadOnlyCollection<DisplayCharacter> _characters;

        public IReadOnlyDictionary<StatusLed, bool> StatusLeds => _statusLeds;
        public IReadOnlyList<DisplayCharacter> Characters => _characters;

        public static int CharacterCount => 3;
        public static decimal MinDecimalValue => -MaxDecimalValue;
        public static decimal MaxDecimalValue => 10m.Pow(CharacterCount) - 1;

        static DisplayFrame()
        {
            Full = new DisplayFrame(DisplayCharacter.Full, true);
            Empty = new DisplayFrame(DisplayCharacter.Empty, false);
        }

        public DisplayFrame() : this(DisplayCharacter.Empty, false)
        { }

        private DisplayFrame(DisplayCharacter defaultCharacter, bool defaultStatus)
            : this(Enumerable.Repeat(defaultCharacter, CharacterCount), defaultStatus)
        { }

        private DisplayFrame(IEnumerable<DisplayCharacter> characters, bool defaultStatus)
            : this(characters, Helpers.EnumerateEnum<StatusLed>().ToDictionary(s => s, _ => defaultStatus))
        { }

        public DisplayFrame(IEnumerable<DisplayCharacter> characters)
            : this(characters, false)
        { }

        public DisplayFrame(IEnumerable<DisplayCharacter> characters, IDictionary<StatusLed, bool> statusLeds)
        {
            statusLeds.ThrowIfNull(nameof(statusLeds));
            characters.ThrowIfNull(nameof(characters));

            _statusLeds = new ReadOnlyDictionary<StatusLed, bool>(Helpers.EnumerateEnum<StatusLed>()
                .ToDictionary(s => s, s =>
                {
                    if (statusLeds.TryGetValue(s, out bool value))
                        return value;
                    return false;
                }));

            var charactersArray = characters.ToArray();
            if (charactersArray.Length != CharacterCount)
                throw new ArgumentException($"Must have exactly {CharacterCount} characters");

            _characters = new ReadOnlyCollection<DisplayCharacter>(charactersArray);
        }

        public DisplayFrame WithStatus(StatusLed status, bool value)
        {
            if (!Enum.IsDefined(typeof(StatusLed), status))
                throw new ArgumentOutOfRangeException(nameof(status));

            var newStatusLeds = new Dictionary<StatusLed, bool>(_statusLeds);
            newStatusLeds[status] = value;
            return new DisplayFrame(_characters, newStatusLeds);
        }

        public DisplayFrame WithCharacter(int index, DisplayCharacter character)
        {
            character.ThrowIfNull(nameof(character));
            if (index < 0 || index > CharacterCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            var newCharacters = _characters.ToArray();
            newCharacters[index] = character;
            return new DisplayFrame(newCharacters, _statusLeds);
        }

        public static DisplayFrame FromString(string str)
        {
            str.ThrowIfNull(nameof(str));
            str = str.PadLeft(CharacterCount);

            return new DisplayFrame(str.Reverse().Select(DisplayCharacter.FromSymbol));
        }

        public static DisplayFrame FromDecimal(decimal value, StatusLed signLed)
        {
            if (value < MinDecimalValue || value > MaxDecimalValue)
                throw new ArgumentOutOfRangeException(nameof(value));

            bool sign = value < 0m;
            value = Math.Abs(value);

            int integerDigits = value.NumIntegerDigits();
            int fractionalDigits = value.NumFractionalDigits();
            int totalDigits = integerDigits + fractionalDigits;

            if (totalDigits > CharacterCount)
            {
                value = Decimal.Round(value, CharacterCount - integerDigits);
                totalDigits = CharacterCount;
                fractionalDigits = totalDigits - integerDigits;
            }

            var frame = new DisplayFrame();

            for (int i = 0; i < integerDigits; i++)
            {
                bool dot = (i == 0) && (fractionalDigits > 0);
                var character = DisplayCharacter.FromDigit(value.GetDigit(0), dot);
                frame = frame.WithCharacter(totalDigits - i - 1, character);
            }

            for (int i = 0; i < fractionalDigits; i++)
            {
                var character = DisplayCharacter.FromDigit(value.GetDigit(-i - 1));
                frame = frame.WithCharacter(fractionalDigits - i - 1, character);
            }

            if (sign)
            {
                if (totalDigits == CharacterCount)
                {
                    frame = frame.WithStatus(signLed, true);
                }
                else
                {
                    var character = DisplayCharacter.FromSymbol('-');
                    frame = frame.WithCharacter(totalDigits, character);
                }
            }

            return frame;
        }
    }

    public enum StatusLed
    {
        Red,
        Green,
        Blue
    }
}