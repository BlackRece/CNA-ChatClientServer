using Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client {
    public class Client {
        bool _isWPF;
        struct WinClient {
            bool IsWPF { get; }
            private bool _isWPF;

            private Client_WinForm _win;
            private Client_WPFForm _wpf;

            public void NewWin(string choice, Client client) {
                if (choice == "1") {
                    _isWPF = false;
                    _win = new Client_WinForm(client);
                    _win.ShowDialog();
                } else {
                    _isWPF = true;
                    _wpf = new Client_WPFForm(client);
                    _wpf.ShowDialog();
                }
            }

            public void UpdateChat(string message) {
                if (_isWPF)
                    _wpf.UpdateChatWindow(message);
                else
                    _win.UpdateChatWindow(message);
            }
        }
        
        TcpClient _tcpClient;

        NetworkStream _stream;
        BinaryFormatter _formatter;

        BinaryReader _reader;
        BinaryWriter _writer;

        WinClient _cWin;
        Client_WinForm _win;
        Client_WPFForm _wpf;

        string _nick;

        public Client() {
            _tcpClient = new TcpClient();
        }

        public bool Connect(string ipAddress, int port) {
            try {
                _tcpClient.Connect(ipAddress, port);

                _nick = port.ToString();

                _stream = _tcpClient.GetStream();
                _formatter = new BinaryFormatter();

                _reader = new BinaryReader(_stream);
                _writer = new BinaryWriter(_stream);

                return true;

            } catch (Exception e) {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        public void Run() {
            /*
            _win = new Client_WinForm(this);
            _wpf = new Client_WPFForm(this);
            */

            //rename to "_win"
            _cWin = new WinClient();

            //Thread thread = new Thread(() => ProcessServerResponse());
            Thread thread = new Thread(() => ProcessServerPacket());
            thread.Start();

            Console.WriteLine(
                "Select client type:\n" +
                "1:\tWinForm Client\n" +
                "2:\tWPF Client\n"
            );

            string choice = string.Empty;
            while (choice != "1" && choice != "2") 
                choice = Console.ReadLine();

            _cWin.NewWin(choice, this);

            /*
            //thread blocking statements
            if (choice == "1")
                _win.ShowDialog();
            else if (choice == "2") {
                _wpf.ShowDialog();
                _isWPF = true;
            }
            */

            _tcpClient.Close();
        }

        bool ProcessServerResponse() {
            bool result = true;
            string serverMessage = string.Empty;
            while (_reader != null) {
                if (!_reader.BaseStream.CanRead) {
                    result = false;
                    break;
                } else { 
                    //serverMessage = _reader.ReadLine();

                    if (serverMessage == "ERROR") {
                        //serverMessage = _reader.ReadLine();
                        result = false;
                    }
                }

                _cWin.UpdateChat(serverMessage);
                //_win.UpdateChatWindow(serverMessage);

                if (!result) _ = Console.ReadLine();
            }
            return result;
        }

        void ProcessServerPacket() {
            Packet packet = new Packet();

            while (_tcpClient.Connected) {
                if(_reader != null) {
                //if(_reader.BaseStream.CanSeek) { 
                    packet = ReadPacket();

                    switch (packet._packetType) {
                        case Packet.PacketType.EMPTY:
                            /* do nothing */
                            break;
                        case Packet.PacketType.CHATMESSAGE:
                            ChatMessagePacket chatPacket = (ChatMessagePacket)packet;
                            _cWin.UpdateChat(chatPacket._message);

                            break;
                        case Packet.PacketType.PRIVATEMESSAGE:
                            PrivateMessagePacket privPacket = (PrivateMessagePacket)packet;
                            _cWin.UpdateChat(privPacket._packetSrc + ": " + privPacket._message);

                            break;
                    }
                }
            }
        }


        public Packet ReadPacket() {
            int numberOfBytes;
            Packet packet = new Packet();
            
            // check reader and store return val
            if((numberOfBytes = _reader.ReadInt32()) > 0) {
                byte[] buffer = _reader.ReadBytes(numberOfBytes);
                MemoryStream memStream = new MemoryStream(buffer);
                packet = _formatter.Deserialize(memStream) as Packet;
            }

            return packet;
        }

        public bool SendPacket(string message) {
            bool result = false;
            //if (_writer.BaseStream.CanSeek) {
            if (_writer != null) {
                //1 create obj
                ChatMessagePacket chatPacket = new ChatMessagePacket(message);
                MemoryStream memStream = new MemoryStream();

                //2 serialise packet and store in memoryStream
                _formatter.Serialize(memStream, chatPacket);

                //3 get byte array
                byte[] buffer = memStream.GetBuffer();

                //4 write length
                _writer.Write(buffer.Length);

                //5 write buffer
                _writer.Write(buffer);

                //6 flush
                _writer.Flush();

                result = true;
            }
            return result;
        }
    }
}
