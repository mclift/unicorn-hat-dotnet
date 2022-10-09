namespace Mclift.UnicornHatHdDotnet
{
    public interface ISpiProvider : IDisposable
    {
        void TransferFullDuplex(byte[] txBuffer, byte[] rxBuffer);
    }
}