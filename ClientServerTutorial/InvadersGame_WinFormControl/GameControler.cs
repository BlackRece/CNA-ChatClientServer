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
                return _players[_playerIndex].GetPosString;
            } 
        }

        GameTime _gameTime;
        public string GetTime {
            get {
                //return _gameMS.ToString();
                return _gameTime.ElapsedGameTime.TotalMilliseconds.ToString();
            }
        }

        private Rectangle _gameArea;
        public int GameArea_Width {
            set { if (value > 0) _gameArea.Width = value; }
        }
        public int GameArea_Height {
            set { if (value > 0) _gameArea.Height = value; }
        }

        public string tempStr;
        Texture2D temp;
        int tempDir;
        Vector2 tempPos;

        private Player[] _players;
        private Vector2 _playerScale = new Vector2(0.75f, 0.75f);
        const int _playerMax = 4;
        public int _playerIndex;
        public float c_PlayerMaxVel = 80.0f;
        public float c_PlayerSpeed = 20.0f;

        private Enemy[] _enemies;
        private Vector2 _enemyScale = new Vector2(0.5f, 0.5f);
        const int _enemyMax = 20;
        public float c_EnemyMaxVel = 50.0f;
        public float c_EnemySpeed = 10.0f;

        int gameTicks = 0;
        public GameControler() {
            _gameTime = new GameTime();

            _players = new Player[_playerMax];
            _enemies = new Enemy[_enemyMax];
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
            if (!_players[_playerIndex]._inputs.Contains(e.KeyCode)) {
                _players[_playerIndex]._inputs.Add(e.KeyCode);
            }
        }

        public void On_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
            if (_players[_playerIndex]._inputs.Contains(e.KeyCode)) {
                _players[_playerIndex]._inputs.Remove(e.KeyCode);
            }
        }

        #region MonoGame Code

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();

            LoadContent();

        }

        protected void LoadContent() {
            //temp image
            string path_redship = "Content\\image\\RedShip.png";

            temp = LoadImage(path_redship);

            // Player Images
            string[] path_PlayerImages = {
                "Content\\image\\PlayerRed.png",
                "Content\\image\\PlayerBlue.png",
                "Content\\image\\PlayerOrange.png",
                "Content\\image\\PlayerGreen.png"
            };

            int imgID = 0;
            for (int i = 0; i < _playerMax; i++) {
                // Init new player
                _players[i] = new Player(_gameArea);

                // Load image & set scale
                _players[i]._img = LoadImage(path_PlayerImages[imgID]);
                _players[i]._scale = _playerScale;
                _players[i]._imgArea = _players[i].GetDestRect();

                // Set starting pos
                _players[i]._pos.X = i * _players[i]._imgArea.Width;
                _players[i]._pos.Y = _gameArea.Height - _players[i]._imgArea.Height;

                // Set velocity vars
                _players[i]._maxVel = c_PlayerMaxVel;
                _players[i]._spd = c_PlayerSpeed;

                // Reset image path index
                if (imgID >= path_PlayerImages.Length - 1) imgID = 0;
                else imgID++;
            }

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

            imgID = 0;
            for(int i = 0; i<_enemyMax; i++) {
                // Init new enemy
                _enemies[i] = new Enemy(_gameArea);
                
                // Load image & set scale
                _enemies[i]._img = LoadImage(path_EnemyImages[imgID]);
                _enemies[i]._scale = _enemyScale;
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
                _enemies[i]._maxVel = c_EnemyMaxVel;
                _enemies[i]._spd = c_EnemySpeed;

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
            foreach (Player player in _players) {
                player.Update(deltaTime);
            }

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

            // Draw player(s)
            foreach (Player player in _players) {
                player.Draw(Editor.spriteBatch);
            }

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
