using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client {
    /// <summary>
    /// Interaction logic for Client_WPFForm.xaml
    /// </summary>
    public partial class Client_WPFForm : Window {
        Client _client;
        public Client_WPFForm(Client client) {
            InitializeComponent();

            _client = client;
        }

        public void UpdateChatWindow(string message) {
            try {
                MessageWindow.Dispatcher.Invoke(() => {
                    MessageWindow.Text += message + Environment.NewLine;
                    MessageWindow.ScrollToEnd();
                });
            } catch (Exception e) {
                MessageWindow.Text +=
                    "ERROR: " + Environment.NewLine +
                    e.Message + Environment.NewLine;
                MessageWindow.ScrollToEnd();
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            _client.SendPacket(InputField.Text);
            InputField.Clear();
            InputField.Focus();
        }
    }
}
