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
    public partial class Game_WinForm : Form {
        Client _client;

        public Game_WinForm(Client client) {
            _client = client;
            InitializeComponent();
        }
    }
}
