using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class Secure
    {
        // lock objects
        private object _encrypt;
        private object _decrypt;

        RSACryptoServiceProvider _RSAProvider;
        private RSAParameters _privateKey;          // decrypt incoming data from someone
        private RSAParameters _publicKey;           // someone else to encrypt data to come to me
        public RSAParameters PublicKey { get { return _publicKey; } }
        public RSAParameters ExternalKey;           // to encrypt data going to someone

        public Secure() {
            _RSAProvider = new RSACryptoServiceProvider(1024);
            _publicKey = _RSAProvider.ExportParameters(false);
            _privateKey = _RSAProvider.ExportParameters(true);

            _encrypt = new object();
            _decrypt = new object();
        }

        public byte[] Decrypt(byte[] data) {
            byte[] result = data;
            lock (_decrypt) {
                try {
                    _RSAProvider.ImportParameters(_privateKey);
                    result = _RSAProvider.Decrypt(data, false);
                } catch (Exception e) {
                    Console.WriteLine("Encrypt Error: " + e.Message);
                    Console.WriteLine("PrivateKey: " + _privateKey);
                }
            }
            return result;
        }

        public string DecryptString(byte[] message) {
            byte[] data = Decrypt(message);

            string result;

            if (data == message) {
                result = "Unable to decrypt message, sorry.";
            } else {
                result = Encoding.UTF8.GetString(data);
            }

            return result;
        }

        public byte[] Encrypt(byte[] data) {
            byte[] result = data;
            lock (_encrypt) {
                try {
                    _RSAProvider.ImportParameters(ExternalKey);
                    result = _RSAProvider.Encrypt(data, false);
                } catch (Exception e) {
                    Console.WriteLine("Encrypt Error: " + e.Message);
                    Console.WriteLine("ExternalKey: " + ExternalKey);
                }
            }
            return result;
        }

        public byte[] EncryptString(string message) {
            byte[] data = Encoding.UTF8.GetBytes(message);
            return Encrypt(data);
        }
    }
}
