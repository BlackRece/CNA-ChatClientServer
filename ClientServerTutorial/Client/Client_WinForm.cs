using Packets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client {
    public partial class Client_WinForm : Form {
        Client _client;

        public Client_WinForm(Client client) {
            InitializeComponent();

            _client = client;
            UpdateNickName(_client._nick);
        }

        public void UpdateChatWindow(string message) {
            try { 
                if (MessageWindow.InvokeRequired) {
                    Invoke(new Action(() => {
                        UpdateChatWindow(message);
                    }));
                } else {
                    MessageWindow.Text += message + Environment.NewLine;
                    MessageWindow.SelectionStart = MessageWindow.Text.Length;
                    MessageWindow.ScrollToCaret();
                }
            } catch (Exception e) {
                DisplayError(e);
            }
        }

        public void UpdateNickName(string name) {
            try {
                if (NickName_label.InvokeRequired) {
                    Invoke(new Action(() => {
                        UpdateNickName(name);
                    }));
                } else {
                    NickName_label.Text = _client._nick;
                }
            } catch (Exception e) {
                DisplayError(e);
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e) {
            //if (_client.TcpSendPacket(new ChatMessagePacket(InputField.Text))) {
            if (_client.UdpSendPacket(new ChatMessagePacket(InputField.Text))) {
                InputField.Clear();
                InputField.Focus();
            }
        }

        private void ChangeName_Button_Click(object sender, EventArgs e) {
            if(ChangeName_textbox.TextLength > 0) {
                if (_client.TcpSendPacket(new ClientNamePacket(ChangeName_textbox.Text))) {
                    ChangeName_textbox.Clear();
                    InputField.Focus();
                }
            }
        }

        private void DisplayError(Exception e) {
            MessageWindow.Text += Environment.NewLine +
                    "ERROR: " + Environment.NewLine +
                    e.Message + Environment.NewLine;
            MessageWindow.SelectionStart = MessageWindow.Text.Length;
            MessageWindow.ScrollToCaret();
        }
    }
}
