using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvadersGame_WinFormControl {
    public class Enemy : FlyingObjects {
        public Directions _dir;

        public Enemy(Rectangle gameArea) {
            _vel = new Vector2(0.0f, 0.0f);
            _gameArea = gameArea;
        }

        private void AI_VerticalBounce() {
            switch (_dir) {
                case Directions.West:
                    if(_pos.X <= 0.0f + _imgArea.Width) {
                        _dir = Directions.East;
                    } else {
                        if (_vel.X > -_maxVel)
                            _vel.X -= _spd;
                    }
                    break;
                case Directions.East:
                    if (_pos.X >= _gameArea.Width - (_imgArea.Width*2)) {
                        _dir = Directions.West;
                    } else {
                        if (_vel.X < _maxVel)
                            _vel.X += _spd;
                    }
                    break;
            }
        }

        public override void Update(float deltaTime) {
            AI_VerticalBounce();

            base.Update(deltaTime);
        }
    }
}
