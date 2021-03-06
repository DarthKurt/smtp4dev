using Xunit;
using System;
using System.IO;
using System.Linq;

namespace Rnwood.SmtpServer.Tests
{
    public abstract class MessageBuilderTests
    {
        [Fact]
        public void AddTo()
        {
            var builder = GetInstance();

            builder.To.Add("foo@bar.com");
            builder.To.Add("bar@foo.com");

            Assert.Equal(2, builder.To.Count);
            Assert.Equal(builder.To.ElementAt(0), "foo@bar.com");
            Assert.Equal(builder.To.ElementAt(1), "bar@foo.com");
        }

        protected abstract IMessageBuilder GetInstance();

        [Fact]
        public void WriteData_Accepted()
        {
            var builder = GetInstance();

            var writtenBytes = new byte[64 * 1024];
            new Random().NextBytes(writtenBytes);

            using (var stream = builder.WriteData())
            {
                stream.Write(writtenBytes, 0, writtenBytes.Length);
            }

            byte[] readBytes;
            using (var stream = builder.GetData())
            {
                readBytes = new byte[stream.Length];
                stream.Read(readBytes, 0, readBytes.Length);
            }

            Assert.Equal(writtenBytes, readBytes);
        }
    }
}