using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Forms.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace InvadersGame_WinFormControl {
    public partial class GameControler : MonoGameControl { 
        public event EventHandler<GameControlerUpdateEventArgs> GameControlerUpdate;
        public class GameControlerUpdateEventArgs : EventArgs {
            public float ElapsedMilliseconds { get; set; }
            public float ElapsedTicks { get; set; }
        }

        private Timer _gameTimer;
        private int _gameMS;
        public float _ticks {
            get;
            set;
        }

        public struct PlayerObject {
            public Texture2D img;
            public Vector2 pos;
            public Vector2 vel;
            public float spd;
            public float maxVel;
            public string name;

            public void MoveX(float factor, float deltaTime) {
                AccelX(factor);
                float t = vel.X * deltaTime;
                pos.X += spd * t;
            }

            public void MoveY(float factor, float deltaTime) {
                float step = spd * vel.Y;
                pos.Y += step * deltaTime;
            }

            public void AccelX(float factor) {
                if(Math.Abs(vel.X) <= maxVel) {
                    vel.X += factor;
                }
            }
        }

        public string GetPos { 
            get {
                return _player.pos.X.ToString() + ", " + _player.pos.Y.ToString();
            } 
        }

        GameTime _gameTime;
        public string GetTime {
            get {
                //return _gameMS.ToString();
                return _gameTime.ElapsedGameTime.TotalMilliseconds.ToString();
            }
        }

        public string tempStr;
        Texture2D temp;
        int tempDir;
        Vector2 tempPos;

        KeyboardState keyState;
        public PlayerObject _player;

        int gameTicks = 0;
        public GameControler() {
            _gameTime = new GameTime();

            _gameTimer = new Timer(1);
            _gameTimer.AutoReset = true;
            _gameTimer.Elapsed += OnTickEvent;
            _gameMS = 0;

            tempDir = 1;
        }

        private void OnTickEvent(Object src, ElapsedEventArgs e) {
            _gameMS++;
        }

        private void UpdateTimer() {
            if (!_gameTimer.Enabled)
                _gameTimer.Start();
            _gameMS = 0;
        }

        ~GameControler() {
            _gameTimer.Dispose();
        }

        public void On_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            switch (e.KeyCode) {
                case System.Windows.Forms.Keys.W:
                    _player.MoveY(-1, _gameMS);
                    break;
                case System.Windows.Forms.Keys.S:
                    _player.MoveY(1, _gameMS);
                    break;
                case System.Windows.Forms.Keys.A:
                    _player.MoveX(-1, _gameMS);
                    break;
                case System.Windows.Forms.Keys.D:
                    _player.MoveX(1, _gameMS);
                    break;
                    /*

                case Keys.Space:
                    break;
                case Keys.Left:
                    break;
                case Keys.Up:
                    break;
                case Keys.Right:
                    break;
                case Keys.Down:
                    break;
                case Keys.A:
                case Keys.D:
                case Keys.S:
                    break;
                case Keys.W:
                    break;

                default:
                    break;
                    */
            }

        }
        
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();
            
            _player = new PlayerObject() {
                pos = new Vector2(  (Editor.graphics.Viewport.Width / 2),
                                    (Editor.graphics.Viewport.Height / 2)),
                vel = new Vector2(0.0f,0.0f),
                maxVel = 1.0f,
                spd = 1.0f
            };

            keyState = Keyboard.GetState();

            LoadContent();

        }

        protected void LoadContent() {
            string pathTo = Path.Combine(
                System.Environment.CurrentDirectory,
                "Content\\image\\RedShip.png"
            );

            using (var fileStream = new FileStream(pathTo, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                temp = Texture2D.FromStream(GraphicsDevice, fileStream);
            }

            using (var fileStream = new FileStream(pathTo, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                _player.img = Texture2D.FromStream(GraphicsDevice, fileStream);
            }

        }

        public virtual void OnGameControlerUpdate(GameControlerUpdateEventArgs e) {
            EventHandler<GameControlerUpdateEventArgs> handler = GameControlerUpdate;
            if(handler != null) {
                handler(this, e);
            }
        }

        protected override void Update(GameTime gameTime) {
        //public void Update(GameTime gameTime) { 
            // TODO: use this.Content to load your game content here
            base.Update(gameTime);

            //get deltaTime from timer
            _gameTime = gameTime;
            UpdateTimer();

            //update game ticks every update
            /*
             * get milliseconds
             *      get timer
             *      store ms
             *      reset 
             */

            if(tempPos.X < 10) {
                tempPos.X = 10;
                tempDir = 1;
            }

            if(tempPos.X > Editor.graphics.Viewport.Width - 10) {
                tempPos.X = Editor.graphics.Viewport.Width - 10;
                tempDir = -1;
            }

            tempPos.X += tempDir;

            //raise update event
            GameControlerUpdateEventArgs args = new GameControlerUpdateEventArgs();
            args.ElapsedMilliseconds = _gameMS;
            //args.ElapsedMilliseconds = (float)_gameTime.ElapsedGameTime.TotalMilliseconds;
            //args.ElapsedTicks = (float)_gameTime.ElapsedGameTime.Ticks;
            OnGameControlerUpdate(args);
        }

        protected override void Draw() {
            base.Draw();
            //Update(gameTime);
            
            // TODO: Add your drawing code here
            Editor.spriteBatch.Begin();

            // draw text
            /*
            Editor.spriteBatch.DrawString(
                Editor.Font, _client._nick,
                new Vector2(
                    (Editor.graphics.Viewport.Width / 2) - (Editor.Font.MeasureString(_client._nick).X / 2),
                    (Editor.graphics.Viewport.Height / 2) - (Editor.FontHeight / 2)),
                    Color.White
                );
            */
            Editor.spriteBatch.DrawString(
                Editor.Font, tempStr, tempPos, Color.White
            );

            if (temp != null) {
                Editor.spriteBatch.Draw(temp, tempPos, Color.White);
            }

            // draw player
            Editor.spriteBatch.Draw(_player.img, _player.pos, Color.White);

            Editor.spriteBatch.End();
            
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if(temp != null & !temp.IsDisposed) {
                temp.Dispose();
            }

            if(_player.img != null & !_player.img.IsDisposed) {
                _player.img.Dispose();
            }

        }
    }
}
