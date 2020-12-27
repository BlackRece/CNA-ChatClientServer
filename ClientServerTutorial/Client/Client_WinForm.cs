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

namespace CNA_Client {
    public partial class Client_WinForm : Form {
        Client _client;

        public Client_WinForm(Client client) {
            InitializeComponent();

            _client = client;
            UpdateNickName(_client._nick);
            GetUserList();
        }

        private void GetUserList() {
            UserListPacket userList = new UserListPacket(null);
            _client.Send(userList);
        }

        private void Client_WinForm_Shown(Object sender, EventArgs e) {
            GetUserList();
        }

        #region Control Update methods

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
                    NickName_label.Text = name;
                }
            } catch (Exception e) {
                DisplayError(e);
            }
        }

        public void UpdateUserList(string[] userList) {
            try {
                if (UserList.InvokeRequired) {
                    Invoke(new Action(() => {
                        UpdateUserList(userList);
                    }));
                } else {
                    UserList.Items.Clear();
                    foreach(string user in userList) {
                        if(user != null) 
                            UserList.Items.Add(user);
                    }
                }
            } catch (Exception e) {
                DisplayError(e);
            }
        }

        #endregion 

        private void SubmitButton_Click(object sender, EventArgs e) {
            string target = null;

            if (UserList.SelectedItem != null)
                target = UserList.SelectedItem.ToString();

            if (_client.SendSecure(InputField.Text, target)) {
                InputField.Clear();
                InputField.Focus();
            }

            GetUserList();
        }

        private void ChangeName_Button_Click(object sender, EventArgs e) {
            if(ChangeName_textbox.TextLength > 0) {
                if (_client.Send(new ClientNamePacket(ChangeName_textbox.Text) 
                    { _packetSrc = _client._nick })) {
                    ChangeName_textbox.Clear();
                    InputField.Focus();
                }
            }
            GetUserList();
        }

        private void DisplayError(Exception e) {
            MessageWindow.Text += Environment.NewLine +
                    "ERROR: " + Environment.NewLine +
                    e.Message + Environment.NewLine;
            MessageWindow.SelectionStart = MessageWindow.Text.Length;
            MessageWindow.ScrollToCaret();
        }

        private void Client_WinForm_FormClosing(object sender, FormClosingEventArgs e) {
            // notify server of closing
            _client.Send(new EndSessionPacket());

        }

        private void LaunchGame_Click(object sender, EventArgs e) {
            string host = null;
            if(UserList.SelectedItem != null)
                host =UserList.SelectedItem.ToString();

            JoinGamePacket joinGame = new JoinGamePacket(host) {
                _packetSrc = _client._nick
            };

            _client.Send(joinGame);
        }

        private void Clear_Selected_Click(object sender, EventArgs e) {
            GetUserList();
            UserList.ClearSelected();
        }
    }
}
