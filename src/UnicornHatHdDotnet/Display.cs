namespace Mclift.UnicornHatHdDotnet
{
    public sealed class Display
    {
        private readonly int _x;
        private readonly byte[,,] _buffer;

        internal const int Width = 16;
        internal const int Height = 16;

        public bool Enabled { get; set; } = false;

        public Display(int displayIndex, byte[,,] buffer)
        {
            _buffer = buffer;
            _x = displayIndex * Width;
        }

        /// <summary>
        /// Writes the data for this display to the window buffer from offset 1.
        /// </summary>
        public void GetBufferWindow(byte[] windowBuffer, double brightness)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        var start = 1 + (x + y * Height) * 3;
                        windowBuffer[start + c] = (byte)(brightness * _buffer[x + _x, y, c]);
                    }
                }
            }
        }
    }
}