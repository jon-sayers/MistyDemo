using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MistyDemo.Classes
{
    public class MReading
    {
        public MReading()
        {
            Id = Guid.NewGuid().ToString();
            Payload = new List<KeyValuePair<string, string>>();
        }

        public string Id { get; set; }
        public int TypeId { get; set; }
        public DateTime Time { get; set; }
        public List<KeyValuePair<string, string>> Payload { get; set; }
        public string Hash { get; set; }

        public void CreateHash()
        {
            string toHash = JsonConvert.SerializeObject(Payload) + Id + "8/gRFZW5LkhahVM41Dbh9UH7c0o1B0ttOtz0yIvrqHU=";

            byte[] hash = null;

            using (SHA256Managed sha256 = new SHA256Managed())
            {
                hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(toHash));
            }

            var sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            Hash = sb.ToString();
        }
    }
}
