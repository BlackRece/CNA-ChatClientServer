﻿using Packets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InvadersGame_WinFormControl;
using MonoGame.Forms.Controls;


namespace CNA_Client {
    /* VS Keeps removing monogame control from form!!! */
    public partial class Game_WinForm : Form {
        Client _client;

        private GameControler gameControler;

        public Game_WinForm(Client client, int slot) {
            //public Game_WinForm(Client client) {
            _client = client;
            InitializeComponent();

            {
                this.gameControler = new GameControler();
                // 
                // gameControler
                // 
                this.gameControler.Location = new System.Drawing.Point(174, 12);
                this.gameControler.MouseHoverUpdatesOnly = false;
                this.gameControler.Name = "gameControler";
                this.gameControler.Size = new System.Drawing.Size(614, 426);
                this.gameControler.TabIndex = 0;
                this.gameControler.tempStr = client._nick;
                this.gameControler.Text = "gameControler";

                this.gameControler.KeyDown += 
                    new KeyEventHandler(this.gameControler.On_KeyDown);
                this.gameControler.KeyUp += 
                    new KeyEventHandler(this.gameControler.On_KeyUp);

                this.Controls.Add(this.gameControler);

                this.gameControler.GameArea_Width = gameControler.Size.Width;
                this.gameControler.GameArea_Height = gameControler.Size.Height;

                this.gameControler.c_PlayerMaxVel = 80.0f;
                this.gameControler.c_PlayerSpeed = 20.0f;

                this.gameControler.c_EnemyMaxVel = 50.0f;
                this.gameControler.c_EnemySpeed = 10.0f;

                this.gameControler._playerIndex = slot;
                
                /* TODO:
                 * send udp packets of:
                 * A) player pos, vel, spd (dead reckoning)
                 * B) time since last update (milliseconds/seconds?) (dead reckoning)
                 * C) score
                 * D) time when bullet fired (use player pos for bullet start)
                 * 
                 * receive udp packets of:
                 * A) other players' pos
                 * B) enemy pos
                 * C) bullet pos
                 * D) players' scores
                 */
            }

        }

        private void OnGameControlerUpdate(object sender, GameControler.GameControlerUpdateEventArgs e) {
            GameUpdateTextbox.Text =
                "ms: " + e.ElapsedMilliseconds + Environment.NewLine +
                "ticks: " + e.ElapsedTicks;
            
        }

        private void Game_WinForm_Activated(object sender, EventArgs e) {
            //invadersGameControl1.gameControler._client = _client;
            gameControler.Focus();
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
            MessageWindow.Text += Environment.NewLine + gameControler.GetPos;
            InputField.Text = gameControler.GetTime;
        }

        private void Game_WinForm_Load(object sender, EventArgs e) {
            gameControler.Focus();
        }

        private void On_KeyDown(object sender, KeyEventArgs e) {
            gameControler.On_KeyDown(sender, e);
        }

        private void ShowPlayerPos
            (object sender, GameControler.GameControlerUpdateEventArgs e) {
            MessageWindow.Text += Environment.NewLine + gameControler.GetPos;
            InputField.Text = gameControler.GetTime;
        }

        private void Game_WinForm_KeyDown(object sender, KeyEventArgs e) {
            On_KeyDown(sender, e);
            MessageWindow.Text = e.KeyCode.ToString();

        }

        private void Game_WinForm_Click(object sender, EventArgs e) {
            gameControler.Focus();
            MessageWindow.Text += Environment.NewLine + "Click!";
        }
    }
}
