using Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNA_Server {
    // Server's representation of client in a game
    public class GameClient {
        struct PlayerData {
            public float[] _pos;
            public float[] _vel;
            public float _spd;
            public float _elapsed;
            public float _fired;
            public float _percent;
        }

        PlayerData _oldData;
        PlayerData _newData;
        PlayerData _guessData;

        public GameClient() {
            _oldData = new PlayerData();
            _newData = new PlayerData();
            _guessData = new PlayerData();
        }

        public Dictionary<string, string> UpdateData(Dictionary<string, string> data) {
            // store current data as old data
            _oldData = _newData;
            if(_oldData._pos == null) {
                _oldData._pos = new float[2];
                _oldData._vel = new float[2];
            }

            // convert strings to floats
            // and update player data
            StringToPos(data["pos"]);
            StringToVel(data["vel"]);
            StringToTime(data["time"]);
            _newData._spd = float.Parse(data["spd"]);

            // apply dead reckoning
            _oldData._percent = 0;
            _newData._percent = 1;
            _guessData = GenerateGuess();

            // test collisions
            // no time :(

            // convert to strings
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("slot", data["slot"]);
            result.Add("pos", string.Join<float>(",", _guessData._pos));
            result.Add("vel", string.Join<float>(",", _guessData._vel));
            result.Add("spd", _guessData._spd.ToString());
            result.Add("time", _guessData._elapsed + ":" + _guessData._fired);

            // send guess data
            return result;
        }

        public GamePacket UpdateRaw(GamePacket packet) {
            // store current data as old data
            _oldData = _newData;
            if (_oldData._pos == null) {
                _oldData._pos = new float[2];
                _oldData._vel = new float[2];
            }

            _newData._pos = packet._pPos;
            _newData._vel = packet._pVel;
            _newData._spd = packet._pSpd;
            _newData._elapsed = packet._pElapsed;
            _newData._fired = packet._pFired;

            // apply dead reckoning
            _oldData._percent = 0;
            _newData._percent = 1;
            _guessData = GenerateGuess();

            GamePacket result =
                new GamePacket(packet._slot, packet._packetSrc) {
                    _pPos = _guessData._pos,
                    _pVel= _guessData._vel,
                    _pSpd= _guessData._spd,
                    _pElapsed = _guessData._elapsed,
                    _pFired = _guessData._fired
                };

            return result;
        }

        public void StringToPos(string pos) {
            string[] sPos = pos.Split(',');
            _newData._pos = new float[] {
                float.Parse(sPos[0]),
                float.Parse(sPos[1])
            };
        }

        public void StringToVel(string vel) {
            string[] sVel = vel.Split(',');
            _newData._vel = new float[] {
                float.Parse(sVel[0]),
                float.Parse(sVel[1])
            };
        }

        public void StringToTime(string time) {
            string[] sTime = time.Split(':');
            _newData._elapsed = float.Parse(sTime[0]);
            _newData._fired = float.Parse(sTime[1]);
        }

        public void DeadReconing() {
            /* 
             * guess/predict new/next player's state
             * using BlendKinematicStateLinear algorythm
             */

        }

        private PlayerData GenerateGuess() {
            if(_oldData._pos == null) {
                return _newData;
            }
            PlayerData result = new PlayerData();

            // get the difference between old and old data
            PlayerData diff = new PlayerData();
            List<float> diffPos = new List<float>();
            List<float> diffVel = new List<float>();
            for (int i = 0; i < _oldData._pos.Length; i++) {
                diffPos.Add(_oldData._pos[i] - _newData._pos[i]);
                diffVel.Add(_oldData._vel[i] - _newData._vel[i]);
            }
            diff._pos = diffPos.ToArray();
            diff._vel = diffVel.ToArray();
            diff._spd = _oldData._spd - _newData._spd;
            diff._elapsed = _oldData._elapsed - _newData._elapsed;
            diff._fired = _oldData._fired - _newData._fired;

            // based on elapsed time received
            float diffPercent = diff._elapsed / _newData._elapsed;

            // populate guess
            diffPos = new List<float>();
            diffVel = new List<float>();
            for (int i = 0; i < diff._pos.Length; i++) {
                diffPos.Add(_newData._pos[i] + (diff._pos[i] * diffPercent));
                diffVel.Add(_newData._vel[i] + (diff._vel[i] * diffPercent));
            }
            result._pos = diffPos.ToArray();
            result._vel = diffVel.ToArray();
            result._spd = _newData._spd + (diff._spd * diffPercent);
            result._elapsed = _newData._elapsed + (diff._elapsed * diffPercent);
            result._fired = _newData._fired;

            // result is pos of player 
            // after applying speed & vel
            return result;
        }
        /*
        struct kinematicState {
            D3DXVECTOR3 position;
            D3DXVECTOR3 velocity;
            D3DXVECTOR3 acceleration;
        };

        void PredictPosition(kinematicState* old, kinematicState* prediction, float elapsedSeconds) {
            prediction->position = old->position + (old->velocity * (float)elapsedSeconds);

            prediction->position = old->position + (old->velocity * elapsedSeconds) + (0.5 * old->acceleration * (elapsedSeconds * elapsedSeconds));`
}

        kinematicState* BlendKinematicStateLinear(kinematicState* olStated, kinematicState* newState, float percentageToNew) {
            //Explanation of percentateToNew:
            //A value of 0.0 will return the exact same state as "oldState",
            //A value of 1.0 will return the exact same state as "newState",
            //A value of 0.5 will return a state with data exactly in the middle of that of "old" and "new".
            //Its value should never be outside of [0, 1].

            kinematicState* final = new kinematicState();

            //Many other interpolation algorithms would create a smoother blend,
            //But this is just a linear interpolation to keep it simple.

            //Implementation of a different algorithm should be straightforward.
            //I suggest starting with Catmull-Rom splines.

            float percentageToOld = 1.0 - percentageToNew;

            final->position = (percentageToOld * oldState->position) + (percentageToNew * newState > position);
            final->velocity = (percentageToOld * oldState->velocity) + (percentageToNew * newState->velocity);
            final->acceleration = (percentageToOld * oldState->acceleration) + (percentageToNew * newState->acceleration);

            return final;
        }
        */
    }
}
