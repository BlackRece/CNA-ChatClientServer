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
using System.Resources;


namespace CNA_Client {
    public partial class Game_WinForm : Form {
        Client _client;
        Graphics gfx;
        Bitmap player;

        public Game_WinForm(Client client) {
            //public Game_WinForm(Client client) {
            _client = client;
            InitializeComponent();
            
            gfx = gamePanel.CreateGraphics();

            System.Reflection.Assembly myAss = this.GetType().Assembly;
            System.Resources.ResourceManager resMan = new ResourceManager("Client.Resources", myAss);

            player = (Bitmap)resMan.GetObject("F7");
            gfx.DrawImage(player, 0, 0);
        }

        private void Game_WinForm_Activated(object sender, EventArgs e) {
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
    }
}
