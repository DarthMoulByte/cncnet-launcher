﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CnCNetLauncher
{
    public class UpdateFile
    {
        public string SHA1 { get; protected set; }
        public int Size { get; protected set; }
        public string Path { get; protected set; }
        public bool Deleted { get; protected set; }
        public bool AnyVersion { get; protected set; }

        public string URL
        {
            get
            {
                return Configuration.UrlTo(SHA1 + ".gz");
            }
        }

        string _localSha1;

        public bool Valid {
            get {
                if (_localSha1 == null)
                    return Validate();

                return _localSha1.Equals(SHA1);
            }
        }

        public UpdateFile(string sha1, int size, string path, bool deleted, bool anyVersion)
        {
            SHA1 = sha1;
            Size = size;
            Path = path;
            Deleted = deleted;
            AnyVersion = anyVersion;
        }

        public bool Validate()
        {
            string filePath = Configuration.FilePath(Path);

            if (Deleted)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);

                return true;
            }

            if (!File.Exists(filePath))
                return false;

            if (AnyVersion)
                return true;

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            using (BufferedStream bs = new BufferedStream(fs))
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    byte[] hash = sha1.ComputeHash(bs);
                    StringBuilder formatted = new StringBuilder(2 * hash.Length);
                    foreach (byte b in hash)
                    {
                        formatted.AppendFormat("{0:x2}", b);
                    }
                    _localSha1 = formatted.ToString();
                }
            }

            return SHA1.Equals(_localSha1);
        }
    }
}

