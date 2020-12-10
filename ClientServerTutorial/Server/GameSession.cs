using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNA_Server {
    public sealed class GameSession {
        private static readonly Lazy<GameSession>
            lazy = new Lazy<GameSession>(() => new GameSession());

        public static GameSession Instance { get { return lazy.Value; } }

        private class Session {
            public Client _host;
            public List<Client> _players;

            public Session(Client host) {
                _host = host;
                _players = new List<Client>();
                _players.Clear();
            }
        }

        private class PlayerSorting : IComparer<Session> {
            int IComparer<Session>.Compare (Session s1, Session s2) {
                int comparePlayer = s1._players.Count.CompareTo(s2._players.Count);
                if(comparePlayer == 0) {
                    return s1._host._name.CompareTo(s2._host._name);
                }
                return comparePlayer;
            }
        }

        const int _gamePlayerMax = 3;
        List<Session> _sessions;

        private object _lock;
        public int SessionCount { get { return _sessions.Count; } }


        private GameSession() {
            _sessions = new List<Session>();
            _sessions.Clear();
        }

        private int FindHostedSession(ref Client target) {
            int  result = -1;

            foreach (Session currentGame in _sessions) {
                //look for matching host
                if (currentGame._host._name == target._name) {
                    result = _sessions.IndexOf(currentGame);
                }
            }

            return result;
        }

        private int FindFreeSession() {
            int result = -1;

            int playerCount = _gamePlayerMax;
            //IComparer<Session> comparer = new PlayerSorting();

            //_sessions.Sort(comparer);

            foreach (Session session in _sessions) {
                if(session._players.Count == 0) {
                    result = _sessions.IndexOf(session);
                    break;
                }

                if(session._players.Count < playerCount) {
                    playerCount = session._players.Count;
                    result = _sessions.IndexOf(session);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Start a new session with no players
        /// </summary>
        /// <param name="host"></param>
        public void StartNewSession(Client host) {
            Session newSession = new Session(host);
            _sessions.Add(newSession);
        }

        public bool JoinHostedSession(Client player, int index) {
            bool result = false;
            Session target = _sessions.ElementAt(index);

            if(target._players.Count < _gamePlayerMax) {
                target._players.Add(player);
                result = true;
            }

            return result;
        }

        public bool JoinFreeSession(Client player, int index) {
            bool result = false;
            Session target = _sessions.ElementAt(index);

            if (target._players.Count < _gamePlayerMax) {
                target._players.Add(player);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Join a session that has a host and less than 3 player (total 4 users)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool JoinSession(ref Client player, Client host) {
            bool result = false;
            int index = FindSession(ref host);


            if(index < 0) {
                StartNewSession(player);
                result = true;
            } else {
                Session target = _sessions.ElementAt(index);

                if (target._players.Count < _gamePlayerMax) {
                    target._players.Add(player);
                    result = true;
                }
            }

            return result;
        }

        public int FindSession(ref Client host) {
            int result = -1;

            if (SessionCount > 0) {

                if (host != null) {
                    result = FindHostedSession(ref host);
                    
                } else {
                    //find session with fewest players
                    result = FindFreeSession();
                }
            }

            return result;
        }
    }
}
