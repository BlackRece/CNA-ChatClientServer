using Packets;
using Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CNA_Client {
    public class Client {
        const bool DEBUG = true;

        private NetworkManager _net;
        private WindowManager _win;

        public string _nick;

        Thread _tcpThread;
        Thread _udpThread;

        public Client() {
            _net = new NetworkManager(ref _nick);
        }

        public bool Connect(string ipAddress, int port) {
            return _net.TcpConnect(ipAddress, port);
        }

        [STAThread]
        public void Run() {
            // select window type
            Console.WriteLine(
                "Select client type:\n" +
                "1:\tWinForm Client\n" +
                "2:\tWPF Client\n"
            );

            string choice = string.Empty;
            while (choice != "1" && choice != "2")
                choice = Console.ReadLine();

            // create window
            _win = new WindowManager(choice, this);

            // start network worker threads
            _tcpThread = new Thread(() => TcpProcessServerPacket());
            _tcpThread.Name = "TCP-Thread";
            _tcpThread.Start();

            _udpThread = new Thread(() => UdpProcessServerPacket());
            _udpThread.Name = "UDP-Thread";
            _udpThread.Start();

            // login to server
            _net.TcpBeginLogin(_nick);

            // launch selected window
            _win.ShowWin();         // this blocks the thread until window is closed

            // clean up connections before exit
            Close();

            Debug("Client successfully terminated.");
            _ = Console.ReadLine();
        }

        public void Close() {
            // stop network connections
            _net.Close();

            // stop threads
            Debug("Close: stopping _tcpThread = " + _tcpThread.ThreadState);
            _tcpThread.Abort("Closing Client");

            Debug("Close: stopping " + _udpThread.Name + " [" + _udpThread.ThreadState + "]");
            _udpThread.Abort("Closing Client");
        }

        #region Tcp/Udp Transmit Methods

        public bool Send(Packet packet, bool viaTcp = true) {
            bool result = false;

            if (viaTcp)
                result = _net.TcpSendPacket(packet);
            else
                result = _net.UdpSendPacket(packet);

            return result;
        }

        public bool SendSecure(string message, string target = null, bool viaTcp = true) {
            bool result = false;

            if (target == null) {

                if (viaTcp)
                    result = _net.TcpSendSecurePacket(message, _nick);
                else
                    result = _net.UdpSendSecurePacket(message, _nick);
            } else {
                result = _net.TcpSendSecurePrivatePacket(target, message, _nick);
            }

            return result;
        }

        public bool SendUpdate(int slot, float[] pos, float[] vel, float spd, float elapsed, float fired) {
            GamePacket updatePacket = new GamePacket(slot, _nick) {
                _pPos = pos, _pVel = vel, _pSpd = spd,
                _pElapsed = elapsed, _pFired = fired
            };

            return Send(updatePacket);
        }

        public bool SendSecureUpdates(int slot, string pos, string vel, string spd, string time) {
            bool result = false;

            /* NOTE: Idealy it would be a good idea to enrypt
             * the data sent to and from the game to prevent
             * others from trying to change the package contents
             * to gain an unfair advantage. 
             * For example, changing position to gain access to 
             * areas of the game's level or adjusting the amount
             * of in-game currency.
             */

            GameUpdatePacket updatePacket = null;

            try {
                updatePacket = new GameUpdatePacket(_nick, slot) {
                    _pPos = _net.EncryptString(pos),
                    _pVel = _net.EncryptString(vel),
                    _spd = _net.EncryptString(spd),
                    _time = _net.EncryptString(time)
                };

                result = true;
            } catch (Exception e) {
                Debug("SendSecureUpdates: Error = " + e.Message);
            }

            /* NOTE: Idealy, should use UDP for frequent packets sends.
             * However, there is a bug that the endpoint used when
             * logging in does not match the one stored endpoint on the
             * server meaning the client never receives the udp packet
             * to update the game state.
             * As an alternative, the TCP method is used.
             */
            //if(result) result = Send(updatePacket, false);
            if (result) result = Send(updatePacket);

            return result;
        }

        #endregion

        #region Tcp/Udp Receive Methods

        [STAThread]
        private void TcpProcessServerPacket() {
            Packet packet = new Packet();

            while(_net.IsTcpConnected) {
                
                packet = _net.TcpReadPacket();
                
                //Debug("TcpProcessServerPacket: packet = " + packet);

                if(packet != null) {

                    switch (packet._packetType) {
                        case Packet.PacketType.EMPTY:
                            /* do nothing */
                            break;
                        case Packet.PacketType.CLIENTNAME:
                            ClientNamePacket namePacket = (ClientNamePacket)packet;

                            _win.name = namePacket._name;
                            _nick = namePacket._name;

                            break;
                        case Packet.PacketType.CHATMESSAGE:
                            ChatMessagePacket chatPacket = (ChatMessagePacket)packet;
                            _win.UpdateChat(chatPacket._message);

                            break;
                        case Packet.PacketType.PRIVATEMESSAGE:
                            if (packet._isSecure) {
                                PrivateSecureMessagePacket privPacket = 
                                    (PrivateSecureMessagePacket)packet;
                                _win.UpdateChat(
                                    "PRIVATE from " + privPacket._packetSrc + 
                                    ": " + _net.DecryptString(privPacket._data)
                                    );
                            } else {
                                PrivateMessagePacket privPacket = (PrivateMessagePacket)packet;
                                _win.UpdateChat(
                                    "PRIVATE from " + privPacket._packetSrc + 
                                    ": " + privPacket._message
                                    );
                            }

                            break;
                        case Packet.PacketType.LOGIN:
                            Console.WriteLine("Logged in to server.");
                            _net.TcpFinishLogin((LoginPacket)packet);

                            break;
                        case Packet.PacketType.SECUREMESSAGE:
                            SecurePacket safePacket = (SecurePacket)packet;
                            _win.UpdateChat(
                                "Secure Message Received [" + safePacket._packetSrc + "]: " +
                                _net.DecryptString(safePacket._data)
                            );

                            break;
                        case Packet.PacketType.ENDSESSION:
                            Console.WriteLine("Disconnecting from server.");
                            _tcpThread.Abort();
                            _net.Close();

                            break;
                        case Packet.PacketType.JOINGAME:
                            JoinGamePacket joinPacket = (JoinGamePacket)packet;
                            _win.StartGame(this, joinPacket._slot);

                            break;
                        case Packet.PacketType.USERLIST:
                            UserListPacket userList = (UserListPacket)packet;
                            _win.UserList = userList._users;

                            break;
                        case Packet.PacketType.LEAVEGAME:
                            LeaveGamePacket leaveGame = (LeaveGamePacket)packet;
                            if(leaveGame._wasForced) { _win.StopGame(); }
                            break;
                        case Packet.PacketType.SERVERMESSAGE:
                            ServerMessagePacket serverMessage = (ServerMessagePacket)packet;
                            _win.UpdateChat(serverMessage._messageSent);
                            break;
                        case Packet.PacketType.GAMEUPDATESECURE:
                            GameUpdatePacket updatePacket = (GameUpdatePacket)packet;

                            _win.UpdateGame2(
                                updatePacket._slot, updatePacket._packetSrc,
                                _net.DecryptString(updatePacket._pPos),
                                _net.DecryptString(updatePacket._pVel),
                                _net.DecryptString(updatePacket._spd),
                                _net.DecryptString(updatePacket._time)
                                );
                            break;
                        case Packet.PacketType.GAMEPACKET:
                            GamePacket data = (GamePacket)packet;

                            _win.UpdateGame(
                                data._slot, data._packetSrc,
                                data._pPos, data._pVel, data._pSpd,
                                data._pElapsed, data._pFired
                                );
                            break;
                    }
                } 
            }
        }

        private void UdpProcessServerPacket() {
            Debug("UdpProcessServerPacket");
            try {
                while(_net.IsUdpConnected) {
                    Packet packet = _net.UdpReadPacket();

                    switch (packet._packetType) {
                        case Packet.PacketType.EMPTY:
                            /* do nothing */
                            break;
                        case Packet.PacketType.GAMEUPDATESECURE:
                            GameUpdatePacket updatePacket = (GameUpdatePacket)packet;

                            _win.UpdateGame2(
                                updatePacket._slot, updatePacket._packetSrc,
                                _net.DecryptString(updatePacket._pPos),
                                _net.DecryptString(updatePacket._pVel),
                                _net.DecryptString(updatePacket._spd),
                                _net.DecryptString(updatePacket._time)
                                );
                            break;
                        /*
                        case Packet.PacketType.LOGIN:
                            LoginPacket logPacket = (LoginPacket)packet;

                            _win.UpdateChat(
                                "Logged in on IP: " + logPacket._endPoint.Address.ToString() +
                                " on Port: " + logPacket._endPoint.Port.ToString()
                            );
                            break;
                        */
                        /*
                        case Packet.PacketType.ENDSESSION:
                            Console.WriteLine("Disconnecting from server.");

                            _udpClient.Close();
                            break;
                        */
                    }
                }
            } catch (SocketException e) {
                Console.WriteLine("Client UDP Read Method Exception: " + e.Message);
            }
        }

        #endregion
        private void Debug(string msg) {
            if (DEBUG) Console.WriteLine("Client - " + msg);
        }
    }
}
