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
    /* VS Keeps removing monogame control from form!!! */
    public partial class Game_WinForm : Form {
        Client _client;

        public Game_WinForm(Client client) {
            //public Game_WinForm(Client client) {
            _client = client;
            InitializeComponent();
            //invadersGameControl1.gameControler.GameControlerUpdate += OnGameControlerUpdate;
        }

        private void OnGameControlerUpdate(object sender, GameControler.GameControlerUpdateEventArgs e) {
            GameUpdateTextbox.Text =
                "ms: " + e.ElapsedMilliseconds + Environment.NewLine +
                "ticks: " + e.ElapsedTicks;
            
        }

        private void Game_WinForm_Activated(object sender, EventArgs e) {
            //invadersGameControl1.gameControler._client = _client;
        }

        private void Game_WinForm_Deactivate(object sender, EventArgs e) {
        }

        private void Game_WinForm_FormClosing(object sender, FormClosingEventArgs e) {
            bool reason = false;
            if (
                e.CloseReason == CloseReason.None ||
                e.CloseReason == CloseReason.WindowsShutDown ||
                e.CloseReason == CloseReason.TaskManagerClosing
                ) reason = true;
            
            _client.Send(new LeaveGamePacket(reason) { _packetSrc = _client._nick });
        }

        private void ShowPlayerPos(object sender, EventArgs e) {
            MessageWindow.Text += Environment.NewLine + gameControler1.GetPos;
            InputField.Text = gameControler1.GetTime;
        }

        private void Game_WinForm_Load(object sender, EventArgs e) {

        }

        private void On_KeyDown(object sender, KeyEventArgs e) {
            gameControler1.On_KeyDown(e);
        }
    }
}
