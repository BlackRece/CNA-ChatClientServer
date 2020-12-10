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
    public partial class Game_WinForm : Form {
        Client _client;

        public Game_WinForm(Client client) {
            //public Game_WinForm(Client client) {
            _client = client;
            InitializeComponent();
        }

        private void Game_WinForm_Activated(object sender, EventArgs e) {
            gameControl1._client = _client;
        }
    }
}
