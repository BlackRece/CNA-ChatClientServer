using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvadersGame_WinFormControl {
    public abstract class FlyingObjects {
        public Texture2D _img;
        public Vector2 _scale;
        public Rectangle _imgArea;

        public Vector2 _pos;

        public Vector2 _vel;
        public float _maxVel;       // aka max speed 
        public float _spd;          // speed of acceleration

        public enum Directions {
            None,
            North,
            South,
            East,
            West
        }

        public FlyingObjects() {
            _vel = new Vector2(0.0f, 0.0f);
            _scale = new Vector2(0.0f, 0.0f);
        }

        ~FlyingObjects() {
            if (_img != null & !_img.IsDisposed) _img.Dispose();
        }

        // fix minuscule movement velovity
        public void FixVelocity() {
            if (_vel.Y > -1.0f && _vel.Y < 1.0f) _vel.Y = 0.0f;
            if (_vel.X > -1.0f && _vel.X < 1.0f) _vel.X = 0.0f;
        }

        public virtual void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(_img, GetDestRect(), Color.White);
        }

        public virtual void Update(float deltaTime) {
            _pos.X += _vel.X * deltaTime;
            _pos.Y += _vel.Y * deltaTime;
        }

        #region Utility Methods and Properites
        public string GetPosString {
            get {
                return _pos.X.ToString() + ", " + _pos.Y.ToString();
            }
        }

        public string GetVelString {
            get {
                return _vel.X.ToString() + ", " + _vel.Y.ToString();
            }
        }

        public Rectangle GetDestRect() {
            return new Rectangle(
                (int)_pos.X, (int)_pos.Y,
                (int)(_img.Width * _scale.X),
                (int)(_img.Height * _scale.Y)
            );
        }
        #endregion
    }
}
