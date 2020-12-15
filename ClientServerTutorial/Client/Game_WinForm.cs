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

        private InvadersGame_WinFormControl.GameControler gameControler;

        public Game_WinForm(Client client) {
            //public Game_WinForm(Client client) {
            _client = client;

            {
                this.gameControler = new InvadersGame_WinFormControl.GameControler();
                // 
                // gameControler1
                // 
                this.gameControler._ticks = 0F;
                this.gameControler.Location = new System.Drawing.Point(174, 12);
                this.gameControler.MouseHoverUpdatesOnly = false;
                this.gameControler.Name = "gameControler";
                this.gameControler.Size = new System.Drawing.Size(614, 426);
                this.gameControler.TabIndex = 0;
                this.gameControler.tempStr = client._nick;
                this.gameControler.Text = "gameControler";
                this.gameControler.KeyDown += new KeyEventHandler(this.gameControler.On_KeyDown);
                this.Controls.Add(this.gameControler);
            }

            InitializeComponent();
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
            gameControler.Focus();
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
            MessageWindow.Text += Environment.NewLine + gameControler.GetPos;
            InputField.Text = gameControler.GetTime;
        }

        private void Game_WinForm_Load(object sender, EventArgs e) {

        }

        private void On_KeyDown(object sender, KeyEventArgs e) {
            gameControler.On_KeyDown(sender, e);
        }

        private void ShowPlayerPos(object sender, InvadersGame_WinFormControl.GameControler.GameControlerUpdateEventArgs e) {
            MessageWindow.Text += Environment.NewLine + gameControler.GetPos;
            InputField.Text = gameControler.GetTime;
        }
    }
}
