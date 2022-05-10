using Microsoft.Win32;
using System;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Zadanie_5
{
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

        private void TransformTextButtonClick(object sender, RoutedEventArgs e)
        {
            BigInteger modulus, exponent;
            try { ParseKey(out modulus, out exponent); }
            catch (ArgumentException ex) { MessageBox.Show(ex.Message); return; }
            var inputTextTypeIndex = CheckedIndex(InputTextTypeStackPanel.Children);
            byte[] inputBytes;
            if (inputTextTypeIndex == 0)
                inputBytes = Encoding.UTF8.GetBytes(InputTextTextBox.Text);
            else
                try { inputBytes = HexStringToByteArray(InputTextTextBox.Text); }
                catch (ArgumentException ex) { MessageBox.Show(ex.Message); return; }
            var bigInt = new BigInteger(inputBytes); // tworzymy liczbę całkowitą z bajtów tekstu
            var pow = new RSA().PowerModulo(bigInt, exponent, modulus); // potęgujemy liczbę modulo według podanego przez użytkownika modułu
            var outputTextTypeIndex = CheckedIndex(OutputTextTypeStackPanel.Children);
            var powBytes = pow.ToByteArray();
            if (outputTextTypeIndex == 0)
                OutputTextTextBox.Text = Encoding.UTF8.GetString(powBytes);
            else
                OutputTextTextBox.Text = ByteArrayToHexString(powBytes);
        }

        private string BytesToString(byte[] bytes)
        {
            return string.Join(", ", bytes);
        }

        private void ParseKey(out BigInteger modulus, out BigInteger exponent)
        {
            if (!BigInteger.TryParse(ModulusTextBox.Text, out modulus))
                throw new ArgumentException("Podaj prawidłowy moduł.");
            if (!BigInteger.TryParse(ExponentTextBox.Text, out exponent))
                throw new ArgumentException("Podaj prawidłowy wykładnik.");
        }

        private int CheckedIndex(UIElementCollection radioButtons)
        {
            int index = 0;
            foreach (var rb in radioButtons)
            {
                if ((bool)(rb as RadioButton).IsChecked)
                    break;
                ++index;
            }
            return index;
        }

        private string ByteArrayToHexString(byte[] ba)
        {
            var chars = new char[ba.Length * 2];
            int ci = 0;
            for (int bi = ba.Length - 1; bi >= 0; --bi)
            {
                /* var bStr = ba[bi].ToString("x2");
                chars[ci++] = bStr[0];
                chars[ci++] = bStr[1]; */
                var b = ba[bi];
                var bH = b >> 4;
                chars[ci++] = (char)(bH <= 9 ? bH + '0' : bH - 10 + 'a');
                var bL = b & 0b1111;
                chars[ci++] = (char)(bL <= 9 ? bL + '0' : bL - 10 + 'a');
            }
            return new string(chars);
        }

        private byte[] HexStringToByteArray(string str)
        {
            if (str.Length % 2 != 0) // jeżeli str nie ma parzystej długości
                throw new ArgumentException("str nie ma parzystej długości.");
            var bytes = new byte[str.Length / 2];
            int ci = str.Length - 1;
            for (int bi = 0; bi < bytes.Length; ++bi)
            {
                var cL = str[ci--];
                var cH = str[ci--];
                int bInt = 0;
                bInt |= (cH <= '9' ? cH - '0' : cH + 10 - 'a') << 4;
                bInt |= cL <= '9' ? cL - '0' : cL + 10 - 'a';
                bytes[bi] = (byte)bInt;
            }
            return bytes;
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
