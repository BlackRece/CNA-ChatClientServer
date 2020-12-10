namespace CNA_Client {
    class WindowManager {
        private bool _isWPF;

        private string _name;
        public string name {
            get { return _name; }
            set {
                if (value.Length > 0) {
                    _name = value;
                    if (_isWPF)
                        _wpf.UpdateNickName(value);
                    else
                        _win.UpdateNickName(value);
                }
            }
        }

        public string[] UserList {
            set {
                if (value.Length < 1)
                    return;

                if (_isWPF)
                    _wpf.UpdateUserList(value);
                else
                    _win.UpdateUserList(value);
            }
        }

        public bool IsReady {
            get {
                bool result = false;
                if (_isWPF) {
                    try {
                        result = _wpf.IsInitialized;
                    } catch { }
                } else {
                    try {
                        result = _win.Visible;
                    } catch { }
                }
                return result;
            }
        }
        private bool _isRunning;
        public bool IsRunning { get { return _isRunning; } }

        private Client_WinForm _win;
        private Client_WPFForm _wpf;

        private Game_WinForm _winGame;
        
        public WindowManager(string choice, Client client) {
            if (choice == "1") {
                _isWPF = false;
                _win = new Client_WinForm(client);
                _win.ShowDialog();
            } else {
                _isWPF = true;
                _wpf = new Client_WPFForm(client);
                _wpf.ShowDialog();
            }
        }

        public bool StartGame(Client client) {
            bool result = false;

            if(!_isRunning) {
                _isRunning = true;
                _winGame = new Game_WinForm(client);
                _winGame.ShowDialog();
                _isRunning = false;
                _winGame = null;
            }

            return result;
        }

        public void UpdateChat(string message) {
            if (_isWPF)
                _wpf.UpdateChatWindow(message);
            else
                _win.UpdateChatWindow(message);
        }

    }
}
