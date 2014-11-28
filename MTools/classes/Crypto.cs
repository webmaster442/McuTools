using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            List<char> iv = new List<char>();
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
