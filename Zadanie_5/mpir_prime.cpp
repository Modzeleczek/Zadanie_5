// dllmain.cpp : Defines the entry point for the DLL application.
#include "mpir.h"
#include <time.h>

// musi byc tez extern "C", bo C# moze wywolywac funkcje o niemanglowanych nazwach,
// a w C++ przez obsluge przeciazania funkcji nazwy sa manglowane
#define EXPORT extern "C" __declspec(dllexport)

EXPORT void generate_probable_prime(unsigned int length_bytes, char** decimal_digits, unsigned int* decimal_length)
{
    if (length_bytes == 0)
    {
        *decimal_length = 0;
        *decimal_digits = (char*)malloc(1 * sizeof(char));
        return;
    }
    mpz_t number;
    mpz_init(number);
    srand(time(0));
    unsigned char* bytes = (unsigned char*)malloc(length_bytes * sizeof(unsigned char));
    unsigned int bi = 0;
    unsigned int whole_quad_bytes = length_bytes / 4, remainder = length_bytes % 4;
    while (bi < whole_quad_bytes)
    {
        int quad_byte = rand();
        for (unsigned int i = 0; i < 4; ++i)
            bytes[bi++] = ((quad_byte >> (8 * i)) & 0xff);
    }
    int quad_byte = rand();
    for (unsigned int i = 0; i < remainder; ++i)
        bytes[bi++] = ((quad_byte >> (8 * i)) & 0xff);
    bytes[0] = bytes[0] | 0b00000001; // ustawiamy pierwszy bit liczby na 1, aby byla nieparzysta
    mpz_import(number, 128, -1, sizeof(unsigned char), 0, 0, bytes); // budujemy liczbe calkowita z bajtow
    free(bytes);
    gmp_randstate_t rng; // generator pseudolosowy do algorytmu testu pierwszosci Millera-Rabina
    gmp_randinit_default(rng);
    gmp_randseed_ui(rng, 34); // seed jest typu unsigned long long
    while (1) // zwiekszamy liczbe (nieparzysta) o 2 dopoki nie trafimy na liczbe prawdopodobnie pierwsza
    {
        if (mpz_probable_prime_p(number, rng, 100, 0) == 0) // na pewno zlozona
            mpz_add_ui(number, number, 2); // zwiekszamy liczbe o 2
        else // prawdopodobnie (nie na pewno) pierwsza
            break;
    }
    // tworzymy stringa w postaci dziesietnej
    *decimal_length = mpz_sizeinbase(number, 10);
    *decimal_digits = (char*)malloc((*decimal_length + 1) * sizeof(char)); // + 1, bo mpz_get_str wstawia \0 na koncu,
    // ale nie trzeba dawac + 2 (miejsce na ewentualny -), bo mamy tylko liczby dodatnie
    mpz_get_str(*decimal_digits, 10, number);
    mpz_clear(number);
}

EXPORT void free_array(char* array)
{
    free(array);
}

EXPORT bool is_probable_prime(char* decimal_digits)
{
    mpz_t number;
    mpz_init(number);
    if (mpz_set_str(number, decimal_digits, 10) == -1) // nie udalo sie sparsowac liczby
        return false;
    gmp_randstate_t rng; // generator pseudolosowy do algorytmu testu pierwszosci Millera-Rabina
    gmp_randinit_default(rng);
    gmp_randseed_ui(rng, 34); // seed jest typu unsigned long long
    bool ret = mpz_probable_prime_p(number, rng, 100, 0) != 0; // jezeli 0, to na pewno zlozona
    mpz_clear(number);
    return ret;
}
