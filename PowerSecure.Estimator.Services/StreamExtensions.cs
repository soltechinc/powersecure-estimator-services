using System.IO;
using System.Text;

namespace PowerSecure.Estimator.Services
{
    public static class StreamExtensions
    {
        public static StreamReader WithNonClosingReader(this Stream file) => new StreamReader(file, Encoding.UTF8, true, 4096, true);

        public static StreamReader WithClosingReader(this Stream file) => new StreamReader(file);
    }
}
