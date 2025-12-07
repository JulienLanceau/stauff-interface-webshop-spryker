using stauff_interface_webshop_spryker_ui.Configuration.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace stauff_interface_webshop_spryker_ui.Configuration.Abstract {
    public abstract class CommonMainConfiguration<T> : ICommonMainConfiguration where T : class, new() {
        static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        static readonly string ConfigurationDirectory = System.IO.Path.Combine(BaseDirectory, "Configuration");
        static readonly string ConfigurationFileName = System.IO.Path.Combine(ConfigurationDirectory, typeof(T) + ".xml");


        #region Encrypt/Decrypt
        public static string Encrypt(string Value) {
            // Get the bytes of the string
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(Value);
            var passwordBytes = Encoding.UTF8.GetBytes("ERTINT");

            // Hash the password with SHA256
            passwordBytes = System.Security.Cryptography.SHA256.Create().ComputeHash(passwordBytes);
            var bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);

            return Convert.ToBase64String(bytesEncrypted);
        }

        public static string Decrypt(string encryptedText) {
            if(encryptedText.Length <= 0)
                return "";
            // Get the bytes of the string
            var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
            var passwordBytes = Encoding.UTF8.GetBytes("ERTINT");

            passwordBytes = System.Security.Cryptography.SHA256.Create().ComputeHash(passwordBytes);

            var bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);

            return Encoding.UTF8.GetString(bytesDecrypted);
        }

        private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes) {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using(System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
                using(System.Security.Cryptography.RijndaelManaged AES = new System.Security.Cryptography.RijndaelManaged()) {
                    var key = new System.Security.Cryptography.Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = System.Security.Cryptography.CipherMode.CBC;

                    using(var cs = new System.Security.Cryptography.CryptoStream(ms, AES.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write)) {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes) {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using(System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
                using(System.Security.Cryptography.RijndaelManaged AES = new System.Security.Cryptography.RijndaelManaged()) {
                    var key = new System.Security.Cryptography.Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = System.Security.Cryptography.CipherMode.CBC;

                    using(var cs = new System.Security.Cryptography.CryptoStream(ms, AES.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write)) {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        #endregion


        public void Save() {
            System.IO.Directory.CreateDirectory(ConfigurationDirectory);
            using(var stringwriter = new System.IO.StringWriter()) {
                var serializer = new XmlSerializer(typeof(T));
                using(FileStream fs = new FileStream(ConfigurationFileName, FileMode.Create, FileAccess.Write)) {
                    serializer.Serialize(fs, this);
                }
            }
        }
        public ICommonMainConfiguration Load() {
            return (ICommonMainConfiguration)LoadStatic();
        }

        public static T LoadStatic() {
            System.IO.Directory.CreateDirectory(ConfigurationDirectory);
            T c = new T();
            try {
                var serializer = new XmlSerializer(typeof(T));
                using(FileStream fs = new FileStream(ConfigurationFileName, FileMode.Open, FileAccess.Read)) {
                    c = serializer.Deserialize(fs) as T;
                }
            } catch {
            }
            return c;
        }
    }
}
