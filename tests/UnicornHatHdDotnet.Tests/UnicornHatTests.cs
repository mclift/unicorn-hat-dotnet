using Moq;
using System;
using System.Drawing;
using System.Linq;
using Xunit;

namespace Mclift.UnicornHatHdDotnet.Tests
{
    public class UnicornHatTests
    {
        [Fact]
        public void Set_WithBytes_SetsWhitePixelAtTopLeft()
        {
            var txBuffer = new byte[769];
            var spiProvider = CreateMockSpiProvider((tx, _) => txBuffer = tx);
            var unicornHat = new UnicornHat(spiProvider) { Brightness = 1.0 };
            unicornHat.Set(0, 0, Color.White.R, Color.White.G, Color.White.B);
            unicornHat.Show();
            Assert.Equal((byte)0x72, txBuffer[0]);
            Assert.Equal((byte)0xFF, txBuffer[1]);
            Assert.Equal((byte)0xFF, txBuffer[2]);
            Assert.Equal((byte)0xFF, txBuffer[3]);
            Assert.All(txBuffer.Skip(4), b => Assert.Equal((byte)0x00, b));
        }

        [Fact]
        public void Set_WithColor_SetsWhitePixelAtTopLeft()
        {
            var txBuffer = new byte[769];
            var spiProvider = CreateMockSpiProvider((tx, _) => txBuffer = tx);
            var unicornHat = new UnicornHat(spiProvider) { Brightness = 1.0 };
            unicornHat.Set(0, 0, Color.White);
            unicornHat.Show();
            Assert.Equal((byte)0x72, txBuffer[0]);
            Assert.Equal((byte)0xFF, txBuffer[1]);
            Assert.Equal((byte)0xFF, txBuffer[2]);
            Assert.Equal((byte)0xFF, txBuffer[3]);
            Assert.All(txBuffer.Skip(4), b => Assert.Equal((byte)0x00, b));
        }

        [Fact]
        public void Brightness_When50Percent_ReducesPixelBrightness()
        {
            var txBuffer = new byte[769];
            var spiProvider = CreateMockSpiProvider((tx, _) => txBuffer = tx);
            var unicornHat = new UnicornHat(spiProvider) { Brightness = 0.5 };
            unicornHat.Set(0, 0, Color.White);
            unicornHat.Show();
            Assert.Equal((byte)0x72, txBuffer[0]);
            Assert.Equal((byte)0x7F, txBuffer[1]);
            Assert.Equal((byte)0x7F, txBuffer[2]);
            Assert.Equal((byte)0x7F, txBuffer[3]);
            Assert.All(txBuffer.Skip(4), b => Assert.Equal((byte)0x00, b));
        }

        [Fact]
        public void Off_ClearsAndSendsBlankFrame()
        {
            var txBuffer = new byte[769];
            var spiProvider = CreateMockSpiProvider((tx, _) => txBuffer = (byte[])tx.Clone());
            var unicornHat = new UnicornHat(spiProvider) { Brightness = 1.0 };
            unicornHat.SetAll(Color.White);
            unicornHat.Off();
            Assert.Equal((byte)0x72, txBuffer[0]);
            Assert.All(txBuffer.Skip(1), b => Assert.Equal((byte)0x00, b));
        }

        [Fact]
        public void Show_WithAddressing_SendsOneFramePerEnabledDisplay()
        {
            var startBytes = new List<byte>();
            var spiProvider = CreateMockSpiProvider((tx, _) => startBytes.Add(tx[0]));
            var unicornHat = new UnicornHat(spiProvider);
            unicornHat.SetAddressingEnabled(true);
            unicornHat.SetDisplayEnabled(0, true);
            unicornHat.SetDisplayEnabled(2, true);
            unicornHat.Show();
            Assert.Equal(new byte[] { 0x73, 0x75 }, startBytes);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(8)]
        public void SetDisplayEnabled_WithOutOfRangeIndex_Throws(int index)
        {
            var unicornHat = new UnicornHat(CreateMockSpiProvider());
            Assert.Throws<ArgumentOutOfRangeException>(() => unicornHat.SetDisplayEnabled(index, true));
        }

        [Fact]
        public void Dispose_DoesNotDisposeCallerProvidedSpiProvider()
        {
            var spiProvider = new Mock<ISpiProvider>();
            new UnicornHat(spiProvider.Object).Dispose();
            spiProvider.Verify(m => m.Dispose(), Times.Never);
        }

        private static ISpiProvider CreateMockSpiProvider(Action<byte[], byte[]>? callback = null)
        {
            var spiProvider = new Mock<ISpiProvider>();
            spiProvider
                .Setup(m => m.TransferFullDuplex(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Callback<byte[], byte[]>((tx, rx) => callback?.Invoke(tx, rx));

            return spiProvider.Object;
        }
    }
}
