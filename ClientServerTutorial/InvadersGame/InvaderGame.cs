using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Forms.Controls;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace InvadersGame {
    public class InvaderGame : MonoGameControl{
        public struct PlayerObject {
            public Vector2 pos;
            public Texture2D img;
            public int spd;
        }

        public string _name;

        Texture2D temp;

        PlayerObject _player;
        public InvaderGame() {
            //_name = name;

            MonoGame.Forms.Controls.GameControl gameControl;
            //gameControl.al
            AlwaysEnableKeyboardInput = false;
            GraphicsDeviceControl gdc;

            PlayerObject _player = new PlayerObject {
                spd = 1
            };
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }

        protected override void Draw() {
            Editor.spriteBatch.Begin();

            if (_name != null) {
                Editor.spriteBatch.DrawString(
                    Editor.Font, _name,
                    new Vector2(
                        (Editor.graphics.Viewport.Width / 2) - (Editor.Font.MeasureString(_name).X / 2),
                        (Editor.graphics.Viewport.Height / 2) - (Editor.FontHeight / 2)),
                        Color.White
                    );
            }

            if (_player.img != null) {
                Editor.spriteBatch.Draw(_player.img, _player.pos, Color.White);
            }

            Editor.spriteBatch.End();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            switch (e.KeyValue) {
                case Sdl.SDLK_w:
                    _player.pos.Y -= _player.spd;
                    break;
                case Sdl.SDLK_s:
                    _player.pos.Y += _player.spd;
                    break;
                case Sdl.SDLK_a:
                    _player.pos.X -= _player.spd;
                    break;
                case Sdl.SDLK_d:
                    _player.pos.X += _player.spd;
                    break;
                case Sdl.SDLK_SPACE:
                    break;
                default:
                    break;
            }
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();
            //Console.WriteLine("Current path: " + Editor.Content.RootDirectory);
            //Editor.Content.RootDirectory = "..\\..\\DesktopGL\\";
            //temp = Editor.Content.Load<Texture2D>("..\\..\\DesktopGL\\image\\RedShip");
            //temp = Editor.Content.Load<Texture2D>("image\\RedShip");
            LoadContent();

            //Microsoft.Xna.Framework.Input.Keyboard;
        }

        protected void LoadContent() {
            string pathTo = Path.Combine(
                System.Environment.CurrentDirectory,
                "Content\\image\\RedShip.png"
            );

            using (var fileStream = new FileStream(pathTo, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                _player.img = Texture2D.FromStream(GraphicsDevice, fileStream);
            }
        }
        
        public void SetName(string name) {
            _name = name;
        }

        protected override void Update(GameTime gameTime) {
            // TODO: use this.Content to load your game content here
            base.Update(gameTime);

            
        }
    }
}
