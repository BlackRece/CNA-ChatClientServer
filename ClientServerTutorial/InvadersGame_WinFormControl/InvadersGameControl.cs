using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Forms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Windows.Forms;

namespace InvadersGame_WinFormControl
{
    public partial class InvadersGameControl: UserControl {
        
        public GameControler gameControler;
        
        public InvadersGameControl()
        {
            InitialiseComponent();

            // events
            //gameControler.GameControlerUpdate += OnGameControlerUpdate;
            //this.KeyDown += new KeyEventHandler(gameControler.KeyDown);
        }

        public void InitialiseComponent() {
            //initialise self
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 426);

            //initialise game
            gameControler = new GameControler();

            gameControler.Location = new System.Drawing.Point(0, 0);
            gameControler.Size = new System.Drawing.Size(this.Width, this.Height);
            gameControler.TabIndex = 0;
            //this.gameControler.GameControlerUpdate +=
                //new EventHandler<GameControler.GameControlerUpdateEventArgs>(EventArgs e);
                //System.EventHandler(this.ShowPlayerPos);

            Controls.Add(gameControler);
        }

        private void InvadersGameControl_Enter(object sender, EventArgs e) {

        }

        private void InvadersGameControl_KeyPress(object sender, KeyPressEventArgs e) {

        }

        private void On_KeyDown(object sender, KeyEventArgs e) {
            gameControler.On_KeyDown(e);
        }
    }   // End Class
}   // End namespace
