using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie_5
{
    class RSA
    {
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
    }
}
