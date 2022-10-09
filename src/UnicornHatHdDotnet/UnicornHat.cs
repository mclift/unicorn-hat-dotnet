using System.Drawing;

namespace Mclift.UnicornHatHdDotnet
{
    public sealed class UnicornHat : IDisposable
    {
        private readonly Display[] _displays;
        private readonly double _delay = 8;
        private readonly byte[] _rxBuffer = new byte[769];
        private double _brightness = 0.5;
        private readonly int _bufferWidth;
        private readonly int _bufferHeight;
        private readonly byte[,,] _buffer;
        private readonly byte[][] _windowBuffers;
        private readonly ISpiProvider _spiProvider;
        private bool _addressingEnabled;

        /// <summary>
        /// Tracks whether the default SPI provider was used and must be disposed.
        /// </summary>
        private bool _mustDispose = false;

        public byte[,,] Buffer => _buffer;

        public double Brightness
        {
            get => _brightness;
            set
            {
                if (value < 0.0 || 1.0 < value)
                {
                    throw new ArgumentOutOfRangeException("Brightness must be in the range 0.0–1.0.");
                }

                _brightness = value;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="UnicornHat"/>.
        /// </summary>
        /// <remarks>
        /// If a custom <see cref="ISpiProvider"/> object is provided in the
        /// <paramref name="spiProvider"/> parameter, the caller is responsible for disposing it.
        /// </remarks>
        /// <param name="spiProvider">
        /// When non-null, sets the custom <see cref="ISpiProvider"/> implementation that will be
        /// used to send data to the HAT; when omitted or set to <code>null</code>, the default
        /// SPI provider is used.
        /// </param>
        public UnicornHat(ISpiProvider? spiProvider = null)
        {
            if (spiProvider is null)
            {
                _spiProvider = new DefaultSpiProvider();
                _mustDispose = true;
            }
            else
            {
                _spiProvider = spiProvider;
                _mustDispose = false;
            }

            _brightness = 1.0;
            _bufferWidth = Display.Width * 8;
            _bufferHeight = Display.Height;
            _buffer = new byte[_bufferWidth, _bufferHeight, 3];
            _displays = Enumerable.Range(0, 8)
                .Select(i => new Display(i, this._buffer) { })
                .ToArray();
            _windowBuffers = Enumerable.Range(0, 8)
                .Select(i => new byte[769])
                .ToArray();
            SetAddressingEnabled(false);
        }

        /// <summary>
        /// Sets whether multi-panel addressing support is enabled (for Ubercorn).
        /// </summary>
        public void SetAddressingEnabled(bool enabled)
        {
            _addressingEnabled = enabled;
            for (int i = 0; i < 8; i++)
            {
                // TODO: Looks like SOF starts at 0x73 when displays are chained, but haven't tried this.
                _windowBuffers[i][0] = (byte)(_addressingEnabled ? 0x73 + i : 0x72);
            }
        }

        public void SetDisplayEnabled(int index, bool enabled)
        {
            if (index < 0 || 8 <= index)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Must be in range 0–7.");
            }

            _displays[index].Enabled = enabled;
        }

        public void SetAll(byte r, byte g, byte b)
        {
            for (int y = 0; y < _bufferHeight; y++)
            {
                for (int x = 0; x < _bufferWidth; x++)
                {
                    _buffer[x, y, 0] = r;
                    _buffer[x, y, 1] = g;
                    _buffer[x, y, 2] = b;
                }
            }
        }

        public void SetAll(Color color)
        {
            var r = color.R;
            var g = color.G;
            var b = color.B;
            for (int y = 0; y < _bufferHeight; y++)
            {
                for (int x = 0; x < _bufferWidth; x++)
                {
                    _buffer[x, y, 0] = r;
                    _buffer[x, y, 1] = g;
                    _buffer[x, y, 2] = b;
                }
            }
        }

        public void Set(int x, int y, byte r, byte g, byte b)
        {
            _buffer[x, y, 0] = r;
            _buffer[x, y, 1] = g;
            _buffer[x, y, 2] = b;
        }

        public void Set(int x, int y, Color color)
        {
            _buffer[x, y, 0] = color.R;
            _buffer[x, y, 1] = color.G;
            _buffer[x, y, 2] = color.B;
        }

        public void Clear()
        {
            SetAll(0, 0, 0);
        }

        public void Off()
        {
            Clear();
            Show();
        }

        public void Show()
        {
            if (_addressingEnabled)
            {
                foreach (int address in Enumerable.Range(0, 8))
                {
                    if (_displays[address].Enabled)
                    {
                        _displays[address].GetBufferWindow(_windowBuffers[address], _brightness);
                        _spiProvider.TransferFullDuplex(_windowBuffers[address], _rxBuffer);
                        Thread.Sleep((int)_delay);
                    }
                }
            }
            else
            {
                _displays[0].GetBufferWindow(_windowBuffers[0], _brightness);
                _spiProvider.TransferFullDuplex(_windowBuffers[0], _rxBuffer);
                Thread.Sleep((int)_delay);
            }
        }

        public void Dispose()
        {
            if (_mustDispose)
            {
                _spiProvider?.Dispose();
            }
        }
    }
}