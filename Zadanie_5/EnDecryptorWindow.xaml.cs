using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Zadanie_5
{
    /// <summary>
    /// Interaction logic for EnDecryptorWindow.xaml
    /// </summary>
    public partial class EnDecryptorWindow : Window
    {
        public EnDecryptorWindow()
        {
            InitializeComponent();
        }

        private void HelpButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tekst/Plik zaszyfrowany jednym kluczem z pary (prywatnym lub publicznym) może być odszyfrowany odpowiadającym mu drugim kluczem z pary (odpowiednio publicznym lub prywatnym).");
        }
        private void ParseKey(out BigInteger modulus, out BigInteger exponent)
        {
            if (!BigInteger.TryParse(ModulusTextBox.Text, out modulus))
                throw new ArgumentException("Podaj prawidłowy moduł.");
            if (!BigInteger.TryParse(ExponentTextBox.Text, out exponent))
                throw new ArgumentException("Podaj prawidłowy wykładnik.");
        }
        private void SelectInputFileButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Plik wejściowy";
            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            // dialog.Filter = "Wszystkie pliki|*"; // aby wyświetlać listę z wyborem rodzaju pliku
            if (dialog.ShowDialog() != true) // jeżeli użytkownik anulował
                return;
            InputFilePathTextBox.Text = dialog.FileName;
        }

        private void SaveAsFileButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "Plik wyjściowy";
            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            if (dialog.ShowDialog() != true) // jeżeli użytkownik anulował
                return;
            BigInteger modulus, exponent;
            try { ParseKey(out modulus, out exponent); }
            catch (ArgumentException ex) { MessageBox.Show(ex.Message); return; }
            if (InputFilePathTextBox.Text.Length == 0)
            { MessageBox.Show("Wybierz plik wejściowy."); return; }
            var inputBytes = File.ReadAllBytes(InputFilePathTextBox.Text);
            var bigInt = new BigInteger(inputBytes); // tworzymy liczbę całkowitą z bajtów pliku
            var pow = new RSA().PowerModulo(bigInt, exponent, modulus); // potęgujemy liczbę modulo według podanego przez użytkownika modułu
            File.WriteAllBytes(dialog.FileName, pow.ToByteArray());
        }
    }
}
