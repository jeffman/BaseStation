using System;

namespace BaseStation
{
    public class LedDriverSettings
    {
        public int DataInPin { get; set; } = 22;
        public int ClockPin { get; set; } = 23;
        public int LatchPin { get; set; } = 24;
        public int EnablePin { get; set; } = 25;
    }
}
