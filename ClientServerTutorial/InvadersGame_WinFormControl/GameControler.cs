using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Forms.Controls;
using MonoGame.Forms.GL;
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
        public string GetPos { 
            get {
                return _player.GetPosString;
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

        public Rectangle _gameArea;

        KeyboardState keyState;
        public Player _player;

        private Enemy[] _enemies;
        const int enemyMax = 20;

        int gameTicks = 0;
        public GameControler() {
            _gameTime = new GameTime();
            
            _enemies = new Enemy[enemyMax];
        }

        ~GameControler() {
        }

        private float UpdateTimer(GameTime gameTime) {
            _gameTime = gameTime;
            return (float)_gameTime.ElapsedGameTime.TotalSeconds;
        }

        #region GameControl's Update Event Propagation Code

        public event EventHandler<GameControlerUpdateEventArgs> GameControlerUpdate;
        
        public class GameControlerUpdateEventArgs : EventArgs {
            public float ElapsedMilliseconds { get; set; }
            public float ElapsedSeconds { get; set; }
            public float ElapsedTicks { get; set; }
        }

        public virtual void OnGameControlerUpdate(GameControlerUpdateEventArgs e) {
            EventHandler<GameControlerUpdateEventArgs> handler = GameControlerUpdate;
            if (handler != null) {
                handler(this, e);
            }
        }

        #endregion

        public void On_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            
            switch (e.KeyCode) {
                case System.Windows.Forms.Keys.W:
                    _player.MoveTo(Player.Directions.North);
                    break;
                case System.Windows.Forms.Keys.S:
                    _player.MoveTo(Player.Directions.South);
                    break;
                case System.Windows.Forms.Keys.A:
                    _player.MoveTo(Player.Directions.West);
                    break;
                case System.Windows.Forms.Keys.D:
                    _player.MoveTo(Player.Directions.East);
                    break;
            }

        }

        #region MonoGame Code

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();

            // Create this client's player object
            _player = new Player() {
                /*
                _pos = new Vector2((Editor.graphics.Viewport.Width / 2),
                                    (Editor.graphics.Viewport.Height / 2)),
                */
                _pos = new Vector2(_gameArea.Width * 0.5f, _gameArea.Height * 0.5f),
                _maxVel = 80.0f,
                _spd = 20.0f,

                _scale = new Vector2(0.75f, 0.75f)
            };

            /*
            _gameArea = new Rectangle(
                0, 0,
                Editor.graphics.Viewport.Width,
                Editor.graphics.Viewport.Height
            );
            */

            keyState = Keyboard.GetState();

            LoadContent();

        }

        protected void LoadContent() {
            //temp image
            string path_redship = "Content\\image\\RedShip.png";

            temp = LoadImage(path_redship);

            // Player Images
            string path_RedPlayer = "Content\\image\\PlayerRed.png";
            string path_BluePlayer = "Content\\image\\PlayerBlue.png";
            string path_OrangePlayer = "Content\\image\\PlayerOrange.png";
            string path_GreenPlayer = "Content\\image\\PlayerGreen.png";

            _player._img = LoadImage(path_BluePlayer);
            _player._imgArea = _player.GetDestRect();

            // Enemy Images
            string[] path_EnemyImages = {
                "Content\\image\\EnemyA.png",
                "Content\\image\\EnemyB.png",
                "Content\\image\\EnemyC.png",
                "Content\\image\\EnemyD.png",
                "Content\\image\\EnemyE.png",
                "Content\\image\\EnemyF.png",
                "Content\\image\\EnemyG.png"
            };

            int imgID = 0;
            for(int i = 0; i<enemyMax; i++) {
                // Init new enemy
                _enemies[i] = new Enemy(_gameArea);
                
                // Load image & set scale
                _enemies[i]._img = LoadImage(path_EnemyImages[imgID]);
                _enemies[i]._scale = new Vector2(0.5f, 0.5f);
                _enemies[i]._imgArea = _enemies[i].GetDestRect();

                // Set starting pos
                _enemies[i]._pos.X = imgID * _enemies[i]._imgArea.Width;
                _enemies[i]._pos.Y = _enemies[i]._imgArea.Height * 0.5f;

                // Set starting dir
                if (i % 2 == 0)
                    _enemies[i]._dir = FlyingObjects.Directions.East;
                else {
                    _enemies[i]._dir = FlyingObjects.Directions.West;
                    _enemies[i]._pos.Y += _enemies[i]._pos.Y;
                }

                // Set velocity vars
                _enemies[i]._maxVel = 50;
                _enemies[i]._spd = 10;

                // Reset image path index
                if (imgID >= path_EnemyImages.Length - 1) imgID = 0;
                else imgID++;
            }

        }

        protected override void Update(GameTime gameTime) {
            // TODO: use this.Content to load your game content here
            base.Update(gameTime);

            //get deltaTime from timer
            float deltaTime = UpdateTimer(gameTime);

            // Update current player
            _player.Update(deltaTime);

            #region Update temp image
            /* 
            if(tempPos.X < 10) {
                tempPos.X = 10;
                tempDir = 1;
            }

            if(tempPos.X > Editor.graphics.Viewport.Width - 10) {
                tempPos.X = Editor.graphics.Viewport.Width - 10;
                tempDir = -1;
            }

            tempPos.X += tempDir;
            */
            #endregion

            // Update Enemies
            foreach (Enemy enemy in _enemies) {
                enemy.Update(deltaTime);
            }

            # region Raise update event
            GameControlerUpdateEventArgs args = new GameControlerUpdateEventArgs();
            args.ElapsedSeconds = deltaTime;
            args.ElapsedMilliseconds = (float)_gameTime.ElapsedGameTime.TotalMilliseconds;
            args.ElapsedTicks = (float)_gameTime.ElapsedGameTime.Ticks;
            OnGameControlerUpdate(args);
            #endregion
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

            #region temp image
            /*
            if (temp != null) {
                Editor.spriteBatch.Draw(temp, tempPos, Color.White);
            }
            */
            #endregion

            // Draw player
            _player.Draw(Editor.spriteBatch);

            // Draw enemies
            foreach(Enemy enemy in _enemies) {
                enemy.Draw(Editor.spriteBatch);
            }

            Editor.spriteBatch.End();
            
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if(temp != null & !temp.IsDisposed) {
                temp.Dispose();
            }

        }

        #endregion

        #region Helper Methods
        private Texture2D LoadImage(string path) {
            string pathTo = Path.Combine(Environment.CurrentDirectory,path);
            Texture2D result;
            using (
                var fileStream = new FileStream(pathTo,
                FileMode.Open, FileAccess.Read, FileShare.Read)) {
                result = Texture2D.FromStream(GraphicsDevice, fileStream);
            }
            return result;
        }
        #endregion

    }   // End class
}   //End namespace
