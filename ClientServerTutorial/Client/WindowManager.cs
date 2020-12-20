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
                if (!IsReady) return;

                if (value.Length < 1)
                    return;

                if (_isWPF)
                    _wpf.UpdateUserList(value);
                else
                    _win.UpdateUserList(value);
            }
        }

        public bool _isReady;

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
                _isReady = true;

            } else {
                _isWPF = true;
                _wpf = new Client_WPFForm(client);
                _isReady = true;
            }
        }

        public void ShowWin() {
            if (_isWPF) {
                _wpf.ShowDialog();
                _isReady = false;
            } else {
                _win.ShowDialog();
                _isReady = false;
            }
        }

        public bool StartGame(Client client, int slot) {
            bool result = false;

            if (!_isRunning) {
                _isRunning = true;
                _winGame = new Game_WinForm(client, slot);
                _winGame.ShowDialog();
                _isRunning = false;
                _winGame = null;
            }

            return result;
        }

        public void StopGame() {
            if (_isRunning) {
                _winGame.Dispose();
                _isRunning = false;
                _winGame = null;
            }
        }

        public void UpdateChat(string message) {
            if (_isWPF)
                _wpf.UpdateChatWindow(message);
            else
                _win.UpdateChatWindow(message);
        }

        public void UpdateGame2(int slot, string name, string pos, string vel, string spd, string time) {
            string[] sPos = pos.Split(',');
            string[] sVel = vel.Split(',');
            string[] sTime = time.Split(':');

            float[] fPos = { float.Parse(sPos[0]), float.Parse(sPos[1]) };
            float[] fVel = { float.Parse(sVel[0]), float.Parse(sVel[1]) };
            float fSpd = float.Parse(spd);
            float elapsed = float.Parse(sTime[0]);
            float fired = float.Parse(sTime[1]);

            _winGame.gameControler.UpdateGameData(
                slot, name, fPos, fVel, fSpd, elapsed, fired
            );
        }

        public void UpdateGame(int slot, string name, float[] pos, float[] vel,
            float spd, float elapsed, float fired) {
            if(_isRunning)
                _winGame.gameControler.UpdateGameData
                    (slot, name, pos, vel, spd, elapsed, fired);
        }
    }
}
