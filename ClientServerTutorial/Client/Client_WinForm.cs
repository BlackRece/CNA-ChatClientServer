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
                MessageWindow.Text +=
                    "ERROR: " + Environment.NewLine +
                    e.Message + Environment.NewLine;
                MessageWindow.SelectionStart = MessageWindow.Text.Length;
                MessageWindow.ScrollToCaret();
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e) {
            if (_client.SendPacket(InputField.Text)) {
                InputField.Clear();
                InputField.Focus();
            }
        }
    }
}
