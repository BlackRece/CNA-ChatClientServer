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
using System.Threading.Tasks;

namespace CNA_Server {
    public class Client {
        Socket _socket;
        public string _name;

        public IPEndPoint _endPoint;
        public string _address {
            get { return _endPoint.Address.ToString(); }
        }
        public int _port {
            get { return _endPoint.Port; }
        }

        NetworkStream _stream;

        BinaryReader _reader;
        BinaryWriter _writer;

        BinaryFormatter _formatter;

        object _readLock;
        object _writeLock;

        private Secure _crypt;
        /*
        RSACryptoServiceProvider _RSAProvider;
        private RSAParameters _publicKey;
        private RSAParameters _privateKey;
        private RSAParameters _clientKey;
        */

        public Client(Socket socket) {
            /*
            _RSAProvider = new RSACryptoServiceProvider(2048);
            _publicKey = _RSAProvider.ExportParameters(false);
            _privateKey = _RSAProvider.ExportParameters(true);
            */
            _crypt = new Secure();

            _readLock = new object();
            _writeLock = new object();

            _socket = socket;

            _stream = new NetworkStream(_socket);
            _formatter = new BinaryFormatter();
            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_stream);

            //_endPoint = _socket.RemoteEndPoint as IPEndPoint;
            //_name = _port.ToString();
        }

        ~Client() {
            Close();
        }

        public void Close() {
            _stream.Close();
            if(_reader != null) _reader.Close();
            if(_writer != null) _writer.Close();
            _socket.Close();

            _reader = null;
            _writer = null;
        }

        /*
        private byte[] Decrypt(byte[] data) {

        }

        private string DecryptString(byte[] message) { }
        private byte[] Encrypt(byte[] data) {

        }

        private byte[] EncryptString(string message) { }
        */

        public Packet TcpRead() {
            lock (_readLock) {
                int numberOfBytes;
                Packet packet = null;

                if (_reader == null)
                    return packet;

                //packet = new Packet();

                //1 check reader and store returned val
                if ((numberOfBytes = _reader.ReadInt32()) != -1) {
                    //2 store bytes 
                    byte[] buffer = _reader.ReadBytes(numberOfBytes);

                    //3 create stream
                    MemoryStream memStream = new MemoryStream(buffer);

                    //4 return packet
                    packet = _formatter.Deserialize(memStream) as Packet;
                }
                return packet;
            }
        }

        public void TcpSend(Packet packet) {
            lock(_writeLock) {
                //1 create obj
                MemoryStream memStream = new MemoryStream();

                //2 serialise packet and store in memoryStream
                _formatter.Serialize(memStream, packet);

                //3 get byte array
                byte[] buffer = memStream.GetBuffer(); //large but guaranteed file size
                // use ToArray() = volatile file size

                //4 write length
                _writer.Write(buffer.Length);

                //5 write buffer
                _writer.Write(buffer);

                //6 flush
                _writer.Flush();
            }
        }

        public void UdpSend(Packet packet, ref UdpClient udpClient) {
            MemoryStream memStream = new MemoryStream();

            _formatter.Serialize(memStream, packet);

            byte[] buffer = memStream.GetBuffer();

            udpClient.Send(buffer, buffer.Length, _endPoint);
        }

        public RSAParameters UpdateRSA(LoginPacket logPacket) {
            _crypt.ExternalKey = logPacket._clientKey;
            return _crypt.PublicKey;
        }

        public string GetSecureMessage(SecurePacket safePacket) {
            return _crypt.DecryptString(safePacket._data);
        }

        public byte[] SetSecureMessage(string message) {
            return _crypt.EncryptString(message);
        }
    }
}
