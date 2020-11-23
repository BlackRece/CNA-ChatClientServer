using Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class Client {
        Socket _socket;
        public string _name;

        IPEndPoint _ipEndPoint;
        public string _address {
            get { return _ipEndPoint.Address.ToString(); }
        }
        public int _port {
            get { return _ipEndPoint.Port; }
        }

        NetworkStream _stream;

        BinaryReader _reader;
        BinaryWriter _writer;

        BinaryFormatter _formatter;

        object _readLock;
        object _writeLock;

        public Client(Socket socket) {
            _readLock = new object();
            _writeLock = new object();

            _socket = socket;

            _stream = new NetworkStream(_socket);
            _formatter = new BinaryFormatter();
            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_stream);

            _ipEndPoint = _socket.RemoteEndPoint as IPEndPoint;
            _name = _port.ToString();
        }

        public void Close() {
            _stream.Close();
            _reader.Close();
            _writer.Close();
            _socket.Close();
        }

        public Packet Read() {
            lock (_readLock) {
                int numberOfBytes;
                Packet packet = new Packet();

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

        public void Send(Packet message) {
            lock(_writeLock) {
                //1 create obj
                MemoryStream memStream = new MemoryStream();

                //2 serialise packet and store in memoryStream
                _formatter.Serialize(memStream, message);

                //3 get byte array
                byte[] buffer = memStream.GetBuffer();

                //4 write length
                _writer.Write(buffer.Length);

                //5 write buffer
                _writer.Write(buffer);

                //6 flush
                _writer.Flush();
            }
        }
    }
}
