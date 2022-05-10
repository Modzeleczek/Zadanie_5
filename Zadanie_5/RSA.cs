using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Zadanie_5
{
    public class RSA
    {
        public const int PUBLIC_EXPONENT = 65537; // często używana jako e liczba pierwsza Fermata 2^(2^4) + 1

        public class KeyPair
        {
            public BigInteger Modulus { get; set; }
            public int PublicExponent { get; set; }
            public BigInteger PrivateExponent { get; set; }
            public KeyPair(BigInteger modulus, int publicExponent, BigInteger privateExponent)
            {
                Modulus = modulus;
                PublicExponent = publicExponent;
                PrivateExponent = privateExponent;
            }
        }

        public KeyPair GenerateKeyPair(BigInteger p, BigInteger q)
        {
            // p, q - duże liczby pierwsze o długości około 1024 bitów, aby ich iloczyn miał długość około 2048 bitów
            var sb = new StringBuilder();
            if (!IsProbablePrime(p))
                sb.Append("p");
            if (!IsProbablePrime(q))
            {
                if (sb.Length > 0)
                    sb.Append(" i ");
                sb.Append("q");
            }
            if (sb.Length > 0)
                throw new ArgumentException(sb.ToString() + " nie jest liczbą pierwszą.");
            var n = p * q; // n = p * q – moduł
            // φ(n) = (p - 1) * (q - 1) = p * q - (p + q) + 1 = n - (p + q) + 1
            var fiN = n - (p + q) + 1;
            // e – liczba względnie pierwsza z φ(n)
            const int e = PUBLIC_EXPONENT;
            if (fiN % e == 0) // e jest liczbą pierwszą, więc jeżeli dzieli φ(n), to e i φ(n) nie są względnie pierwsze
                throw new ArgumentException($"φ(n) = (p-1) * (q-1) nie jest względnie pierwsze z e = {e}.");
            // d – liczba wyznaczona tak, że zachodzi (e * d) mod φ(n) = 1 -> d = e^(-1) mod φ(n)
            var d = ModularInverse(e, fiN);
            return new KeyPair(n, e, d);
        }

        public BigInteger GreatestCommonDivisor(BigInteger a, BigInteger b)
        {
            // standardowy algorytm Euklidesa
            while (a != b)
                if (a > b) a -= b;
                else b -= a;
            return a;
        }

        public BigInteger ModularInverse(BigInteger a, BigInteger m)
        {
            /* https://www.geeksforgeeks.org/multiplicative-inverse-under-modulo-m/
            złożoność czasowa log(m); zwraca rozwiązanie x równania a*x = 1 (mod m)
            odwrotność liczby a w mnożeniu modulo m istnieje tylko, gdy a i m są względnie pierwsze (NWD(a, m) = 1)
            przykład wykonania rozszerzonego algorytmu Euklidesa dla 21*x = 1 (mod 37)
            37 = 1*21 + 16
            21 = 1*16 + 5
            16 = 3*5 + 1
            5 = 5*1 + 0
            1 = 16 - 3*5 = 16 - 3*(21 - 1*16) = 16 - 3*21 + 3*16 = -3*21 + 4*16 = -3*21 + 4*(37 - 1*21) = -7*21 + 4*37 = -7*21 (mod 37) = 30*21 (mod 37) */
            BigInteger m0 = m; // zapisujemy oryginalne m, aby przesunąć o nie x, jeżeli wyjdzie ujemne
            BigInteger y = 0, x = 1;
            if (m == 1) return 0; // pierścień Z1 zawiera tylko jedną liczbę - 0
            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger t = m;
                m = a % m;
                a = t;
                t = y;
                y = x - q * y;
                x = t;
            }
            if (x < 0) // przesuwamy x o oryginalne m
                x += m0;
            return x;
        }

        public BigInteger PowerModulo(BigInteger b, BigInteger e, BigInteger m)
        {
            // https://www.geeksforgeeks.org/modular-exponentiation-power-in-modular-arithmetic/; złożoność czasowa log(e)
            b = b % m; // upewniamy się, że podstawa jest mniejsza od modułu
            if (b == 0) // jeżeli podstawa jest 0, to wynik też będzie 0 i nie trzeba potęgować
                return 0;
            BigInteger res = 1; // inicjujemy wynik jako 1
            while (e > 0)
            {
                // jeżeli aktualny najmłodszy bit wykładnika jest 1, to mnożymy wynik przez aktualną podstawę
                if ((e & 1) != 0)
                    res = (res * b) % m;
                e = e >> 1; // e /= 2; przesuwamy się na na kolejny po aktualnym najmłodszym bicie wykładnika
                b = (b * b) % m; // zmieniamy podstawę na jej kwadrat modulo m
            }
            return res;
        }

        unsafe public BigInteger GenerateProbablePrime(uint byteLength)
        {
            /* test pierwszości Millera-Rabina stwierdza, że liczba jest złożona lub prawdopodobnie (ale nie na pewno) pierwsza; trzeci parametr mpz_probable_prime_p równy np. 100 oznacza, że chcemy, aby prawdopodobieństwo, że liczba złożona jest nazwana pierwszą, wynosiło 2^(-100) */
            byte* digitsPtr;
            uint length;
            generate_probable_prime(byteLength, &digitsPtr, &length); // zwraca tablicę charów zakończoną \0
            var digits = new char[length]; // length to faktyczna liczba cyfr, a length + 1 to długość bufora z \0
            for (int i = 0; i < length; ++i) // wskaźnik char* przeskakuje o 2 bajty, bo w C# char ma 2 bajty
                digits[i] = (char)*(digitsPtr + i);
            free_array(digitsPtr);
            BigInteger ret;
            if (!BigInteger.TryParse(new string(digits), out ret))
                throw new FormatException("Nie sparsowano wygenerowanej liczby jako BigInteger.");
            return ret;
        }

        public bool IsProbablePrime(BigInteger number)
        {
            var nStr = number.ToString();
            var digits = new byte[nStr.Length + 1]; // + 1 na \0
            for (int i = 0; i < nStr.Length; ++i)
                digits[i] = (byte)nStr[i];
            digits[digits.Length - 1] = 0;
            return is_probable_prime(digits);
        }

        private const string MpirPrimeDLL = "mpir_prime.dll";
        private const CallingConvention Convention = CallingConvention.Cdecl;
        [DllImport(MpirPrimeDLL, CallingConvention = Convention)]
        unsafe private static extern void generate_probable_prime(uint length_bytes, byte** decimal_digits, uint* decimal_length);
        [DllImport(MpirPrimeDLL, CallingConvention = Convention)]
        unsafe private static extern void free_array(byte* array);
        [DllImport(MpirPrimeDLL, CallingConvention = Convention)]
        unsafe private static extern bool is_probable_prime(byte[] decimal_digits);
    }
}
