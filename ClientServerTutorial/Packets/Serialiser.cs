using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Packets {
    public static class Serialiser {
        private const bool DEBUG = true;

        /* USE ZLIB COMPRESSION!!! */

        private static BinaryFormatter _formatter;
        private static MemoryStream _memStream;

        public static byte[] Serialise(Packet packet, bool useProto = false) {
            byte[] buffer = null;

            if (useProto) { 
            } else {
                _formatter = new BinaryFormatter();
                _memStream = new MemoryStream();

                _formatter.Serialize(_memStream, packet);
                //buffer = _memStream.GetBuffer();
                buffer = _memStream.ToArray();
            }

            Debug("Serialise: buffer sise = " + buffer.Length.ToString());
            return buffer;
        }

        public static Packet Deserialise(byte[] buffer, bool useProto = false) {
            Packet packet = null;

            if (useProto) {

            } else {
                _formatter = new BinaryFormatter();
                _memStream = new MemoryStream(buffer);

                packet = _formatter.Deserialize(_memStream) as Packet;
            }

            return packet;
        }


        #region Protobuf Serialisation Methods
        /*
        public void SendProtoMsg(Packet packet) {
            Packets.Protobus.AllPackets allPackets = new Packets.Protobus.AllPackets();

            allPackets.Type = (int)packet._packetType;
            allPackets.Src = packet._packetSrc;
            allPackets.Safe = packet._isSecure;

            if (packet._packetType == Packet.PacketType.CHATMESSAGE) {
                ChatMessagePacket cmp = (ChatMessagePacket)packet;
                allPackets.Message = cmp._message;
            }

            Google.Protobuf.CodedOutputStream outputStream = new Google.Protobuf.CodedOutputStream(_net._stream, true);

            allPackets.WriteTo(outputStream);
            outputStream.Flush();

            if (!_net._stream.CanWrite) {
                Console.WriteLine("Stream ended!!");
            }

            //Google.Protobuf.CodedOutputStream.ComputeMessageSize
        }

        public Packet ReceiveProtoMsg() {
            Packet result = null;

            Google.Protobuf.CodedInputStream inputStream = new Google.Protobuf.CodedInputStream(_net._stream, true);
            Packets.Protobus.AllPackets returnPacket = Packets.Protobus.AllPackets.Parser.ParseFrom(inputStream);

            switch ((Packet.PacketType)returnPacket.Type) {
                case Packet.PacketType.CHATMESSAGE:
                    ChatMessagePacket chatMessage = new ChatMessagePacket(returnPacket.Message);
                    chatMessage._packetSrc = returnPacket.Src;

                    result = chatMessage;
                    break;
            }

            return result;
        }
        */
        #endregion
        private static void Debug(string msg) {
            if (DEBUG) Console.WriteLine("Serialiser - " + msg);
        }
    }
}
