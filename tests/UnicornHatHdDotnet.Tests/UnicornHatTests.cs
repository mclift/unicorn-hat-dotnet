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
            Assert.All(txBuffer.Skip(4), b => Assert.Equal(b, (byte)0x00));
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
            Assert.All(txBuffer.Skip(4), b => Assert.Equal(b, (byte)0x00));
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
            Assert.All(txBuffer.Skip(4), b => Assert.Equal(b, (byte)0x00));
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
