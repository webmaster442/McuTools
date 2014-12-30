using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using McuTools.Interfaces;

namespace MTools.classes
{
    internal enum HashAlgorithms
    {
        MD5, SHA1, SHA256, SHA512
    }

    internal enum KeySizeAES
    {
        bit128 = 128, bit192 = 192, bit256 = 256
    }

    internal static class CaesarRules
    {
        private static char[] ValidInputs
        {
            get { return "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToArray(); }
        }

        public static Dictionary<char, char> Classic
        {
            get
            {
                Dictionary<char, char> _ret = new Dictionary<char, char>(65);
                for (int i = 0; i < ValidInputs.Length - 3; i++)
                {
                    _ret.Add(ValidInputs[i], ValidInputs[i + 3]);
                }
                _ret.Add(ValidInputs[ValidInputs.Length - 3], ValidInputs[0]);
                _ret.Add(ValidInputs[ValidInputs.Length - 2], ValidInputs[1]);
                _ret.Add(ValidInputs[ValidInputs.Length - 1], ValidInputs[2]);
                return _ret;
            }
        }

        public static Dictionary<char, char> Random
        {
            get
            {
                Random r = new System.Random();
                List<char> used = new List<char>(65);
                Dictionary<char, char> _ret = new Dictionary<char, char>(65);
                char c = ' ';
                for (int i = 0; i < ValidInputs.Length; i++)
                {
                    do
                    {
                        c = (char)r.Next(0, ValidInputs.Length - 1);
                    }
                    while (used.Contains(c));
                    used.Add(c);
                    _ret.Add(ValidInputs[i], c);
                }
                return _ret;
            }
        }

        public static string SerializeRule(Dictionary<char, char> rules)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var rule in rules)
            {
                sb.AppendFormat("{0}{1}", rule.Key, rule.Value);
            }
            return sb.ToString();
        }

        public static Dictionary<char, char> DeserializeRule(string input)
        {
            Dictionary<char, char> rules = new Dictionary<char, char>();
            if (input.Length % 2 != 0) throw new ArgumentException("Input is invalid");
            for (int i = 0; i < input.Length; i += 2)
            {
                rules.Add(input[i], input[i + 1]);
            }
            return rules;
        }
    }

    internal static class Cryptog
    {

        private static string MakeHashString(byte[] input)
        {
            StringBuilder hash = new StringBuilder(32);
            foreach (var i in input)
            {
                hash.Append(i.ToString("X2").ToUpper());
            }
            return hash.ToString();
        }

        public static string HashInputString(string input, HashAlgorithms alg = HashAlgorithms.MD5)
        {
            HashAlgorithm hashAlgorithm = null;

            switch (alg)
            {
                case HashAlgorithms.MD5:
                    hashAlgorithm = MD5.Create();
                    break;
                case HashAlgorithms.SHA1:
                    hashAlgorithm = SHA1.Create();
                    break;
                case HashAlgorithms.SHA256:
                    hashAlgorithm = SHA256.Create();
                    break;
                case HashAlgorithms.SHA512:
                    hashAlgorithm = SHA512.Create();
                    break;
            }
            byte[] data = Encoding.UTF8.GetBytes(input);
            byte[] output = hashAlgorithm.ComputeHash(data);
            return MakeHashString(output);
        }

        private static string ComputeHash(CancellationToken ct, string inputfile, IProgress<double> progress, HashAlgorithms alg = HashAlgorithms.MD5)
        {
            HashAlgorithm hashAlgorithm = null;

            switch (alg)
            {
                case HashAlgorithms.MD5:
                    hashAlgorithm = MD5.Create();
                    break;
                case HashAlgorithms.SHA1:
                    hashAlgorithm = SHA1.Create();
                    break;
                case HashAlgorithms.SHA256:
                    hashAlgorithm = SHA256.Create();
                    break;
                case HashAlgorithms.SHA512:
                    hashAlgorithm = SHA512.Create();
                    break;
            }

            byte[] buffer = new byte[4096];
            byte[] oldBuffer;
            int bytesRead = 0;
            int oldBytesRead;

            using (Stream infile = File.OpenRead(inputfile))
            {
                do
                {
                    oldBytesRead = bytesRead;
                    oldBuffer = buffer;
                    buffer = new byte[4096];
                    bytesRead = infile.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0) hashAlgorithm.TransformFinalBlock(oldBuffer, 0, oldBytesRead);

                    else hashAlgorithm.TransformBlock(oldBuffer, 0, oldBytesRead, oldBuffer, 0);

                    if (progress != null)
                    {
                        double percent = ((double)infile.Position / (double)infile.Length) * 100.00d;
                        progress.Report(percent);
                    }
                    if (infile.Position % 4096 == 0) ct.ThrowIfCancellationRequested();
                }
                while (bytesRead != 0);
            }

            return MakeHashString(hashAlgorithm.Hash);
        }

        private static void XorEncrypt(CancellationToken ct, string infile, string keyfile, string outputfile, IProgress<double> progress)
        {
            byte[] buffer = new byte[4096];
            using (Stream inp = File.OpenRead(infile))
            {
                using (Stream key = File.Create(keyfile))
                {
                    using (Stream outp = File.Create(outputfile))
                    {
                        int read = 0;
                        do
                        {
                            read = inp.Read(buffer, 0, buffer.Length);
                            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
                            byte[] keyb = new byte[read];
                            byte[] w = new byte[read];
                            rand.GetBytes(keyb);
                            for (int i = 0; i < read; i++) w[i] = (byte)(buffer[i] ^ keyb[i]);
                            outp.Write(w, 0, w.Length);
                            key.Write(keyb, 0, keyb.Length);

                            if (progress != null)
                            {
                                double percent = ((double)inp.Position / (double)inp.Length) * 100.00d;
                                progress.Report(percent);
                            }
                            if (inp.Position % 4096 == 0) ct.ThrowIfCancellationRequested();
                        }
                        while (read != 0);
                    }
                }
            }
        }

        private static void XorDecrypt(CancellationToken ct, string infile, string keyfile, string outputfile, IProgress<double> progress)
        {
            byte[] buffer = new byte[4096];
            byte[] keybuffer = new byte[4096];
            byte[] output = new byte[4096];
            using (Stream inp = File.OpenRead(infile))
            {
                using (Stream key = File.OpenRead(keyfile))
                {
                    using (Stream outp = File.Create(outputfile))
                    {
                        int read1 = 0;
                        int read2 = 0;
                        do
                        {
                            read1 = inp.Read(buffer, 0, buffer.Length);
                            read2 = key.Read(keybuffer, 0, keybuffer.Length);
                            if (read1 != read2) throw new IOException("Key file and input length mismatch");

                            for (int i = 0; i < read1; i++)
                            {
                                output[i] = (byte)(buffer[i] ^ keybuffer[i]);
                            }
                            outp.Write(output, 0, read1);

                            if (progress != null)
                            {
                                double percent = ((double)inp.Position / (double)inp.Length) * 100.00d;
                                progress.Report(percent);
                            }
                            if (inp.Position % 4096 == 0) ct.ThrowIfCancellationRequested();
                        }
                        while (read1 != 0);
                    }
                }
            }
        }

        private static string GenerateAESKey(int keySize)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = keySize;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;
            aesEncryption.GenerateIV();
            string ivStr = Convert.ToBase64String(aesEncryption.IV);
            aesEncryption.GenerateKey();
            string keyStr = Convert.ToBase64String(aesEncryption.Key);

            //Console.WriteLine("Using key '{0}'", keyStr, ivStr);           
            //Console.WriteLine("Using iv '{0}'", ivStr);           
            string completeKey = ivStr + "," + keyStr + "," + keySize.ToString();

            return Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(completeKey));
        }

        private static string AESEncrypt(CancellationToken ct, string inputfile, string outfile, KeySizeAES size, IProgress<double> progress)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = (int)size;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;

            string completeEncodedKey = GenerateAESKey(aesEncryption.KeySize);
            string[] key = ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(completeEncodedKey)).Split(',');

            aesEncryption.IV = Convert.FromBase64String(key[0]);
            aesEncryption.Key = Convert.FromBase64String(key[1]);
            ICryptoTransform crypto = aesEncryption.CreateEncryptor();

            Stream infile = File.OpenRead(inputfile);
            Stream o = File.Create(outfile);

            using (CryptoStream outp = new CryptoStream(o, crypto, CryptoStreamMode.Write))
            {
                int read = 0;
                byte[] buffer = new byte[4096];
                do
                {
                    read = infile.Read(buffer, 0, buffer.Length);
                    outp.Write(buffer, 0, read);
                    if (progress != null)
                    {
                        if (progress != null)
                        {
                            double percent = ((double)infile.Position / (double)infile.Length) * 100.00d;
                            progress.Report(percent);
                        }
                    }
                    if (infile.Position % 4096 == 0) ct.ThrowIfCancellationRequested();
                }
                while (read != 0);
            }

            return completeEncodedKey;
        }

        private static void AESDecrypt(CancellationToken ct, string inputfile, string outfile, string completeEncodedKey, IProgress<double> progress)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();

            string[] s = ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(completeEncodedKey)).Split(',');
            aesEncryption.BlockSize = 128;
            aesEncryption.KeySize = Convert.ToInt32(s[2]);
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;
            aesEncryption.IV = Convert.FromBase64String(s[0]);
            aesEncryption.Key = Convert.FromBase64String(s[1]);

            ICryptoTransform decrypto = aesEncryption.CreateDecryptor();

            Stream infile = File.OpenRead(inputfile);

            using (Stream outp = File.Create(outfile))
            {
                using (CryptoStream inp = new CryptoStream(infile, decrypto, CryptoStreamMode.Read))
                {
                    int read = 0;
                    byte[] buffer = new byte[4096];
                    do
                    {
                        read = inp.Read(buffer, 0, buffer.Length);
                        outp.Write(buffer, 0, read);

                        if (progress != null)
                        {
                            double percent = ((double)infile.Position / (double)infile.Length) * 100.00d;
                            progress.Report(percent);
                        }
                        if (inp.Position % 4096 == 0) ct.ThrowIfCancellationRequested();
                    }
                    while (read != 0);
                }
            }

        }

        public static string GeneratePassWord(bool lowercase, bool uppercase, bool numbers, string special, int length)
        {
            List<char> iv = new List<char>(100);
            if (lowercase) iv.AddRange("abcdefghijklmnopqrstuvwxyz".ToCharArray());
            if (uppercase) iv.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());
            if (numbers) iv.AddRange("0123456789".ToCharArray());
            if (!string.IsNullOrEmpty(special)) iv.AddRange(special.ToCharArray());

            iv = (from i in iv orderby Guid.NewGuid() select i).ToList();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var ch = (from c in iv orderby Guid.NewGuid() select c).Take(1).FirstOrDefault();
                sb.Append(ch);
            }
            return sb.ToString();
        }

        public static string CaesarCrypt(string input, Dictionary<char, char> Rules, bool decrypt)
        {
            StringBuilder sb = new StringBuilder(input.Length);
            if (decrypt)
            {
                foreach (var chr in input)
                {
                    var output = (from rule in Rules.AsParallel() where rule.Value == chr select rule.Key).FirstOrDefault();
                    if (output == (char)0) sb.Append(chr);
                    else sb.Append(output);
                }
            }
            else
            {
                foreach (var chr in input)
                {
                    var output = (from rule in Rules.AsParallel() where rule.Key == chr select rule.Value).FirstOrDefault();
                    if (output == (char)0) sb.Append(chr);
                    else sb.Append(output);
                }
            }
            return sb.ToString();
        }

        public static string RowTransposeEncode(string input, int columns, int[] keysequence)
        {
            StringBuilder sb = new StringBuilder();
            int rows = input.Length / columns;
            DenseArray<char> matrix = new DenseArray<char>(rows + 1, columns);

            foreach (var key in keysequence)
            {
                var column = matrix.GetColumn(key);
                sb.Append(column);
            }

            return sb.ToString();
        }

        public static Task<string> ComputeHashTask(CancellationToken ct, string inputfile, IProgress<double> progress, HashAlgorithms alg = HashAlgorithms.MD5)
        {
            return Task.Run(() => ComputeHash(ct, inputfile, progress, alg), ct);
        }

        public static Task XorEncryptTask(CancellationToken ct, string inputfile, string keyfile, string outputfile, IProgress<double> progress)
        {
            if (File.Exists(keyfile)) return Task.Run(() => XorDecrypt(ct, inputfile, keyfile, outputfile, progress), ct);
            else return Task.Run(() => XorEncrypt(ct, inputfile, keyfile, outputfile, progress), ct);
        }

        public static Task<string> AesEncryptTask(CancellationToken ct, string inputfile, string outfile, KeySizeAES size, IProgress<double> progress)
        {
            return Task<string>.Run(() => AESEncrypt(ct, inputfile, outfile, size, progress), ct);
        }

        public static Task AesDecryptTask(CancellationToken ct, string inputfile, string outfile, string completeEncodedKey, IProgress<double> progress)
        {
            return Task.Run(() => AESDecrypt(ct, inputfile, outfile, completeEncodedKey, progress), ct);
        }
    }
}
