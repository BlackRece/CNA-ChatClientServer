using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvadersGame_WinFormControl {
    public class Player : FlyingObjects {
        public string _name;

        public Player() {
        }

        public void MoveTo(Directions dir) {
            switch (dir) {
                case Directions.None:
                    break;
                case Directions.North:
                    if (_vel.Y > -_maxVel) _vel.Y -= _spd;
                    break;
                case Directions.South:
                    if (_vel.Y < _maxVel) _vel.Y += _spd;
                    break;
                case Directions.East:
                    if (_vel.X < _maxVel) _vel.X += _spd;
                    break;
                case Directions.West:
                    if (_vel.X > -_maxVel) _vel.X -= _spd;
                    break;
                default:
                    break;
            }

            
        }

        private void AddDrag() {
            float drag = _spd * 0.1f;
            if (_vel.Y > 0.0f) _vel.Y -= drag;
            if (_vel.Y < 0.0f) _vel.Y += drag;
            if (_vel.X < 0.0f) _vel.X += drag;
            if (_vel.X > 0.0f) _vel.X -= drag;
        }

        public override void Update(float deltaTime) {
            AddDrag();

            FixVelocity();

            base.Update(deltaTime);
        }

    }
}
