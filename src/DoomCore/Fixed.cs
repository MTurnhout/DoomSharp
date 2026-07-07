using System;

namespace DoomCore
{
    // Fixed-point wrapper preserving 16.16 semantics from original DOOM (fixed_t)
    public readonly struct Fixed : IEquatable<Fixed>
    {
        public readonly int Raw;
        public const int FRACBITS = 16;
        public const int FRACUNIT = 1 << FRACBITS;
        public const int MININT = int.MinValue;
        public const int MAXINT = int.MaxValue;

        public Fixed(int raw) => Raw = raw;

        public static Fixed FromInt(int i) => new Fixed(i << FRACBITS);
        public int ToInt() => Raw >> FRACBITS;

        public static Fixed operator +(Fixed a, Fixed b) => new Fixed(a.Raw + b.Raw);
        public static Fixed operator -(Fixed a, Fixed b) => new Fixed(a.Raw - b.Raw);

        // Multiply using 64-bit intermediate, like FixedMul
        public static Fixed operator *(Fixed a, Fixed b) => new Fixed((int)(((long)a.Raw * (long)b.Raw) >> FRACBITS));

        // Division mirrors FixedDiv/FixedDiv2 behavior: use double scaling to match original C behavior
        public static Fixed operator /(Fixed a, Fixed b)
        {
            if (b.Raw == 0)
                throw new DivideByZeroException();

            // Check overflow similar to original: if ( (abs(a)>>14) >= abs(b)) return (a^b)<0 ? MININT : MAXINT;
            if ((Math.Abs(a.Raw) >> 14) >= Math.Abs(b.Raw))
            {
                bool negative = (a.Raw ^ b.Raw) < 0;
                return new Fixed(negative ? MININT : MAXINT);
            }

            double c = ((double)a.Raw) / ((double)b.Raw) * FRACUNIT;
            if (c >= 2147483648.0 || c < -2147483648.0)
                throw new OverflowException("Fixed division overflow");
            return new Fixed((int)c);
        }

        public override string ToString() => $"Fixed(Raw={Raw})";
        public bool Equals(Fixed other) => Raw == other.Raw;
        public override bool Equals(object? obj) => obj is Fixed f && Equals(f);
        public override int GetHashCode() => Raw;
        public static bool operator ==(Fixed a, Fixed b) => a.Raw == b.Raw;
        public static bool operator !=(Fixed a, Fixed b) => a.Raw != b.Raw;
    }
}
