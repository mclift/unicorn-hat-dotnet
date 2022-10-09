using System.Device.Spi;

namespace Mclift.UnicornHatHdDotnet
{
    public sealed class DefaultSpiProvider : ISpiProvider
    {
        private readonly SpiDevice _spiDevice;

        public DefaultSpiProvider()
        {
            var connectionSettings = new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = 9000000,
                Mode = SpiMode.Mode0
            };

            _spiDevice = SpiDevice.Create(connectionSettings);
        }

        public void TransferFullDuplex(byte[] txBuffer, byte[] rxBuffer)
        {
            _spiDevice.TransferFullDuplex(txBuffer, rxBuffer);
        }

        public void Dispose()
        {
            _spiDevice?.Dispose();
        }
    }
}