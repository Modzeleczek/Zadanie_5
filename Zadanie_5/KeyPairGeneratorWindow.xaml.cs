using System;
using System.Numerics;
using System.Windows;

namespace Zadanie_5
{
    public partial class KeyPairGeneratorWindow : Window
    {
        public KeyPairGeneratorWindow()
        {
            InitializeComponent();
        }

        private void HelpButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
            "p, q – liczby pierwsze, najlepiej duże, tj. rzędu 2²⁰⁴⁸\n" +
            "n = p⋅q – moduł\n\n" +
            "e – liczba względnie pierwsza z φ(n) = (p-1)(q-1). Przyjmujemy e = 65537.\n" +
            "Para liczb n, e stanowi klucz publiczny.\n\n" +
            "d – liczba wyznaczona tak, że zachodzi \n(e⋅d) mod φ(n) = 1 -> d = e⁻¹ mod φ(n)\n" +
            "Para liczb n, d stanowi klucz prywatny.\n\n" +
            "Liczby prawdopodobnie pierwsze są generowane za pomocą testu pierwszości Millera-Rabina. Prawdopodobieństwo, że liczba złożona zostanie określona jako pierwsza, wynosi 2⁻¹⁰⁰.");
        }

        private void GenerateKeysButtonClick(object sender, RoutedEventArgs e)
        {
            BigInteger p, q;
            if (!BigInteger.TryParse(PTextBox.Text, out p))
            {
                MessageBox.Show("Podaj prawidłową liczbę p.");
                return;
            }
            if (!BigInteger.TryParse(QTextBox.Text, out q))
            {
                MessageBox.Show("Podaj prawidłową liczbę q.");
                return;
            }
            try
            {
                var pair = new RSA().GenerateKeyPair(p, q);
                ModulusTextBox.Text = pair.Modulus.ToString();
                PrivateExponentTextBox.Text = pair.PrivateExponent.ToString();
                PublicExponentTextBox.Text = pair.PublicExponent.ToString();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                ModulusTextBox.Text = PrivateExponentTextBox.Text = PublicExponentTextBox.Text = "";
            }
        }

        private void GeneratePButtonClick(object sender, RoutedEventArgs e)
        {
            try { PTextBox.Text = new RSA().GenerateProbablePrime(128).ToString(); }
            catch (FormatException ex) { MessageBox.Show(ex.Message); }
        }

        private void GenerateQButtonClick(object sender, RoutedEventArgs e)
        {
            try { QTextBox.Text = new RSA().GenerateProbablePrime(128).ToString(); }
            catch (FormatException ex) { MessageBox.Show(ex.Message); }
        }
    }
}
