using System.Numerics;
using System.Windows;

namespace Zadanie_5
{
    public partial class InverseModuloCalculatorWindow : Window
    {
        public InverseModuloCalculatorWindow()
        {
            InitializeComponent();
        }

        private void HelpButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
            "Program znajduje rozwiązanie x równania a⋅x = 1 (mod m), " +
            "czyli liczbę x = a⁻¹ (mod m)\n" +
            "Odwrotność a⁻¹ liczby a w mnożeniu modulo m istnieje tylko, " +
            "gdy a i m są względnie pierwsze, czyli największy wspólny dzielnik a i m jest równy 1.");
        }

        private void CalculateButtonClick(object sender, RoutedEventArgs e)
        {
            BigInteger a, m;
            if (!BigInteger.TryParse(NumberTextBox.Text, out a))
            {
                MessageBox.Show("Podaj prawidłową liczbę a.");
                return;
            }
            if (!BigInteger.TryParse(ModulusTextBox.Text, out m))
            {
                MessageBox.Show("Podaj prawidłową liczbę m.");
                return;
            }
            var rsa = new RSA();
            if (rsa.GreatestCommonDivisor(a, m) != 1)
            {
                InverseTextBox.Text = "Odwrotność liczby a nie istnieje, ponieważ a i m nie są względnie pierwsze.";
                return;
            }
            InverseTextBox.Text = rsa.ModularInverse(a, m).ToString();
        }
    }
}
