using Packets;
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
            UpdateNickName(_client._nick);
        }

        public void UpdateChatWindow(string message) {
            try {
                MessageWindow.Dispatcher.Invoke(() => {
                    MessageWindow.Text += message + Environment.NewLine;
                    MessageWindow.ScrollToEnd();
                });
            } catch (Exception e) {
                DisplayError(e);
            }
        }

        public void UpdateNickName(string name) {
            try {
                NickName.Dispatcher.Invoke(() => {
                    NickName.Content = name;
                });
            } catch (Exception e) {
                DisplayError(e);
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            ChatMessagePacket chatPacket = new ChatMessagePacket(InputField.Text);

            //if (_client.TcpSendPacket(chatPacket)) {
            if (_client.TcpSendSecurePacket(InputField.Text)) {
                InputField.Clear();
                InputField.Focus();
            }
        }

        private void NickName_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

        }


        private void ChangeName_button_Click(object sender, RoutedEventArgs e) {
            ClientNamePacket namePacket = new ClientNamePacket(ChangeName_textbox.Text);

            if(ChangeName_textbox.Text.Length > 0) {
                if(_client.TcpSendPacket(namePacket)) {
                    ChangeName_textbox.Clear();
                    InputField.Focus();
                }
            }
        }

        private void DisplayError(Exception e) {
            MessageWindow.Text += Environment.NewLine +
                    "ERROR: " + Environment.NewLine +
                    e.Message + Environment.NewLine;
            MessageWindow.ScrollToEnd();
        }
    }
}
