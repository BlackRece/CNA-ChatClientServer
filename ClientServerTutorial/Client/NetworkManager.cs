using Packets;
using Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CNA_Client {
    class NetworkManager {
        IPEndPoint _endPoint = new IPEndPoint(IPAddress.Any, 0);

        UdpClient _udpClient;
        TcpClient _tcpClient;

        NetworkStream _stream;
        BinaryFormatter _formatter;

        BinaryReader _reader;
        BinaryWriter _writer;

        private Secure _crypt;

        public bool IsTcpConnected { get { return _tcpClient.Connected; } }
        public bool IsUdpConnected { get { return _udpClient.Client.Connected; } }

        public Packet UdpReceive 
            { get { return UdpReadPacket(_udpClient.Receive(ref _endPoint)); }  }

        public NetworkManager(ref string name) {
            _crypt = new Secure();
            name = _crypt.PublicKey.GetHashCode().ToString();
        }

        ~NetworkManager() {
            Close();
        }

        public  void Close() {
            // close local connections
            _tcpClient.Close();
            _udpClient.Close();
        }

        #region TCP related code

        public bool TcpConnect(string ipAddress, int port) {
            try {
                _tcpClient = new TcpClient(ipAddress, port);
                _udpClient = new UdpClient(ipAddress, port);

                _stream = _tcpClient.GetStream();
                _formatter = new BinaryFormatter();

                _reader = new BinaryReader(_stream);
                _writer = new BinaryWriter(_stream);

                return true;
            } catch (Exception e) {
                Console.WriteLine("NetworkManager Exception: " + e.Message);
                return false;
            }
        }

        public void TcpBeginLogin(string src) {
            Console.WriteLine("Logging in to server...");

            LoginPacket logPacket = 
                new LoginPacket((IPEndPoint)_tcpClient.Client.LocalEndPoint) { 
                    _packetSrc = src, _clientKey = _crypt.PublicKey
                };
            
            TcpSendPacket(logPacket);
        }

        public void TcpFinishLogin(LoginPacket logPacket) {
            _crypt.ExternalKey = logPacket._serverKey;
        }

        public Packet TcpReadPacket() {
            int numberOfBytes;
            Packet packet = null;

            try {
                // check reader and store return val
                if ((numberOfBytes = _reader.ReadInt32()) > 0) {
                    byte[] buffer = _reader.ReadBytes(numberOfBytes);
                    MemoryStream memStream = new MemoryStream(buffer);
                    packet = new Packet();
                    packet = _formatter.Deserialize(memStream) as Packet;
                }
            } catch (Exception e) {
                Console.WriteLine("Error: " + e.Message);
            }

            return packet;
        }

        public bool TcpSendPacket(Packet packet) {
            bool result = false;
            //if (_writer.BaseStream.CanSeek) {
            if (_writer != null) {
                //1 create obj
                MemoryStream memStream = new MemoryStream();

                //2 serialise packet and store in memoryStream
                _formatter.Serialize(memStream, packet);

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

        public bool TcpSendSecurePacket(string message, string src) {
            SecurePacket securePacket = new SecurePacket(_crypt.EncryptString(message)) {
                _packetSrc = src
            };

            return TcpSendPacket(securePacket);
        }

        #endregion

        #region UDP related methods

        public Packet UdpReadPacket(byte[] buffer) {

            MemoryStream memStream = new MemoryStream(buffer);
            return _formatter.Deserialize(memStream) as Packet;
        }

        public bool UdpSendPacket(Packet packet) {
            //1 create obj
            MemoryStream memStream = new MemoryStream();

            //2 serialise packet and store in memoryStream
            _formatter.Serialize(memStream, packet);

            //3 get byte array
            byte[] buffer = memStream.GetBuffer();

            //4 send packet
            int result = _udpClient.Send(buffer, buffer.Length);

            return result > 0;
        }

        public bool UdpSendSecurePacket(string message, string src) {
            SecurePacket securePacket = new SecurePacket(_crypt.EncryptString(message)) {
                _packetSrc = src
            };

            return UdpSendPacket(securePacket);
        }

        #endregion

        #region RSA Encryption and Decryption Methods

        public string DecryptString(byte[] message) {
            byte[] data = _crypt.Decrypt(message);
            string result = Encoding.UTF8.GetString(data);
            return result;
        }

        public byte[] EncryptString(string message) {
            byte[] data = Encoding.UTF8.GetBytes(message);
            return _crypt.Encrypt(data);
        }

        #endregion
    }
}
