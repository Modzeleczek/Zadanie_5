using System.ComponentModel;
using System.Windows;

namespace Zadanie_5
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InverseModuloCalculatorButtonClick(object sender, RoutedEventArgs e)
        {
            InverseModuloCalculatorButton.IsEnabled = false;
            var subWindow = new InverseModuloCalculatorWindow();
            subWindow.Closing += new CancelEventHandler((_sender, _e) => InverseModuloCalculatorButton.IsEnabled = true);
            subWindow.Show();
        }

        private void KeyPairGeneratorButtonClick(object sender, RoutedEventArgs e)
        {
            KeyPairGeneratorButton.IsEnabled = false;
            var subWindow = new KeyPairGeneratorWindow();
            subWindow.Closing += new CancelEventHandler((_sender, _e) => KeyPairGeneratorButton.IsEnabled = true);
            subWindow.Show();
        }

        private void EnDecryptorButtonClick(object sender, RoutedEventArgs e)
        {
            EnDecryptorButton.IsEnabled = false;
            var subWindow = new EnDecryptorWindow();
            subWindow.Closing += new CancelEventHandler((_sender, _e) => EnDecryptorButton.IsEnabled = true);
            subWindow.Show();
        }
    }
}
