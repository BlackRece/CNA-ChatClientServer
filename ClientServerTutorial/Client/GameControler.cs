using CNA_InvadersGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Forms.Controls;
using System.IO;
using XNA_Keys = Microsoft.Xna.Framework.Input.Keys;

namespace CNA_Client {
    public class GameControler : MonoGameControl {
        public Client _client;
        private InvaderGame _game;

        public struct PlayerObject {
            public string _name;
            public Vector2 pos;
            public Texture2D img;
            public int spd;
        }

        PlayerObject _player;

        KeyboardState _keyboardState;

        public GameControler() {
            _player = new PlayerObject {
                pos = new Vector2(50, 50),
                spd = 1
            };

            _keyboardState = new KeyboardState();

            
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }

        protected override void Draw() {
            Editor.spriteBatch.Begin();

            if (_player._name != null) {
                Editor.spriteBatch.DrawString(
                    Editor.Font, _player._name,
                    new Vector2(
                        (Editor.graphics.Viewport.Width / 2) - (Editor.Font.MeasureString(_player._name).X / 2),
                        (Editor.graphics.Viewport.Height / 2) - (Editor.FontHeight / 2)),
                        Color.White
                    );
            }

            if (_player.img != null) {
                Editor.spriteBatch.Draw(_player.img, _player.pos, Color.White);
            }

            Editor.spriteBatch.End();
            base.Draw();
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();
            //Console.WriteLine("Current path: " + Editor.Content.RootDirectory);
            //Editor.Content.RootDirectory = "..\\..\\DesktopGL\\";
            //temp = Editor.Content.Load<Texture2D>("..\\..\\DesktopGL\\image\\RedShip");
            //temp = Editor.Content.Load<Texture2D>("image\\RedShip");
            LoadContent();
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

        public void OnKeyPressed(XNA_Keys[] keyArray) {
            foreach (XNA_Keys key in keyArray) {
                switch (key) {
                    case XNA_Keys.W:
                        _player.pos.Y -= _player.spd;
                        break;
                    case XNA_Keys.S:
                        _player.pos.Y += _player.spd;
                        break;
                    case XNA_Keys.A:
                        _player.pos.X -= _player.spd;
                        break;
                    case XNA_Keys.D:
                        _player.pos.X += _player.spd;
                        break;
                    case XNA_Keys.Space:
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetName(string name) {
            _player._name = name;
        }

        protected override void Update(GameTime gameTime) {
            // TODO: use this.Content to load your game content here
            _keyboardState = Keyboard.GetState();

            OnKeyPressed(_keyboardState.GetPressedKeys());

            base.Update(gameTime);
        }
    }
}
