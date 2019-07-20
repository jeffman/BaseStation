using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BaseStation
{
    public class BusyFrameIterator : IEnumerable<DisplayFrame>
    {
        private const byte BottomSide = 0b_0_000001_0;
        private const byte TopSide = 0b_0_001000_0;

        private (byte value, int characterIndex) GetValueAndCharacterIndex(int index, int totalPositions)
        {
            if (totalPositions < 6)
                throw new ArgumentOutOfRangeException(nameof(totalPositions));

            if (index < 0 || index >= totalPositions)
                throw new ArgumentOutOfRangeException(nameof(index));

            int numCharacters = ((totalPositions - 8) / 2) + 2;

            // If the index is one of the last two positions,
            // then it belongs on the right side of character
            // index 0
            if (index >= totalPositions - 2)
            {
                return ((byte)(1 << (7 - (totalPositions - index))), 0);
            }

            // If the index is one of the last two positions
            // in the bottom half, then it belongs on the left
            // side of the left-most character
            int half = totalPositions / 2;
            if (index >= (half - 2) && index < half)
            {
                return ((byte)(1 << (index - half + 4)), numCharacters - 1);
            }

            // If the index is in the bottom half, it's just
            // the bottom side of the index'th character
            if (index < half)
            {
                return (BottomSide, index);
            }

            // If the index is in the top half, we can reflect
            // it and treat it as the top side
            if (index >= half)
            {
                return (TopSide, half - index + 2);
            }

            throw new InvalidOperationException("All cases should have been exhausted!");
        }

        public IEnumerator<DisplayFrame> GetEnumerator()
        {
            int index = 0;
            int totalPositions = 8 + (DisplayFrame.CharacterCount - 2) * 2;

            for (;;)
            {
                (byte value, int characterIndex) = GetValueAndCharacterIndex(index, totalPositions);
                var character = DisplayCharacter.FromRaw(value);
                yield return new DisplayFrame().WithCharacter(characterIndex, character);

                index++;
                if (index >= totalPositions)
                    index = 0;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}