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

        public Client() {
            _net = new NetworkManager(ref _nick);
        }

        public bool Connect(string ipAddress, int port) {
            return _net.TcpConnect(ipAddress, port);
        }
               
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
            Thread tcpThread = new Thread(() => TcpProcessServerPacket());
            tcpThread.Start();

            Thread udpThread = new Thread(() => UdpProcessServerPacket());
            udpThread.Start();

            // login to server
            _net.TcpBeginLogin(_nick);

            // launch selected window
            _win.ShowWin();         // this blocks the thread until window is closed

            // clean up connections before exit
            _net.Close();
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

        public bool SendSecure(string message, bool viaTcp = true) {
            bool result = false;

            if (viaTcp)
                result = _net.TcpSendSecurePacket(message, _nick);
            else
                result = _net.UdpSendSecurePacket(message, _nick);

            return result;
        }

        #endregion

        #region Tcp/Udp Receive Methods

        private void TcpProcessServerPacket() {
            Packet packet = new Packet();

            while(_net.IsTcpConnected) {
                packet = _net.TcpReadPacket();
                Debug("TcpProcessServerPacket: packet = " + packet);

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
                            PrivateMessagePacket privPacket = (PrivateMessagePacket)packet;
                            _win.UpdateChat(privPacket._packetSrc + ": " + privPacket._message);

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
                            _net.Close();

                            break;
                        case Packet.PacketType.JOINGAME:
                            _win.StartGame(this);

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
