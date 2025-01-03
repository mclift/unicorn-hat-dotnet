﻿using Mclift.UnicornHatHdDotnet;

// Ported from https://github.com/pimoroni/unicorn-hat-hd/tree/master/examples
Console.WriteLine("Press a key to exit...");
var unicornHat = new UnicornHat() { Brightness = 0.6 };

var candle = Enumerable.Range(0, 256).Select(_ => 0.0).ToArray();
var palette = Enumerable.Range(0, 256).Select(i =>
    {
        var h = i / 5.0 / 360.0;
        var s = 1.0 / (Math.Sqrt(i / 50.0) + 0.01);
        s = Math.Max(0.0, Math.Min(1.0, s));

        var v = i / 200.0;
        if (i < 60.0)
        {
            v /= 2.0;
        }

        v = Math.Max(0.0, Math.Min(1.0, v));

        return ColorHelper.ColorFromHsv(h, s, v);
    }).ToArray();

static void SetPixel(double[] b, int x, int y, double v)
{
    b[y * 16 + x] = (int)v;
}

static int GetPixel(double[] b, double x, double y)
{
    // out of range sample lookup
    if (x < 0.0 || y < 0.0 || x >= 16.0 || y >= 16.0)
    {
        return 0;
    }

    // subpixel sample lookup
    if (Math.Truncate(x) != x && x < 15.0)
    {
        var f = x - Math.Truncate(x);
        return (int)(b[(int)(y * 16) + (int)x] * (1.0 - f) + (b[(int)(y * 16) + (int)(x + 1)] * f));
    }

    // fixed pixel sample lookup
    return (int)b[(int)y * 16 + (int)x];
}

var step = 0;
var random = new Random();
var temp = new double[256];
while (!Console.KeyAvailable)
{
    // step for waving animation, adds some randomness
    step += random.Next(0, 15);

    // clone the current candle
    Array.Copy(candle, temp, candle.Length);

    // seed new heat
    var v = 500.0;

    SetPixel(candle, 6, 15, v);
    SetPixel(candle, 7, 15, v);
    SetPixel(candle, 8, 15, v);
    SetPixel(candle, 9, 15, v);
    SetPixel(candle, 6, 14, v);
    SetPixel(candle, 7, 14, v);
    SetPixel(candle, 8, 14, v);
    SetPixel(candle, 9, 14, v);

    // blur, wave, and shift up one step
    for (int x = 0; x < 16; x++)
    {
        for (int y = 0; y < 16; y++)
        {
            var s = Math.Sin((y / 30.0) + (step / 10.0)) * ((16 - y) / 20.0);
            v = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // r = randint(0, 2) - 1
                    v += GetPixel(candle, x + i + s - 1, y + j);
                }
            }

            v /= 10;
            SetPixel(temp, x, y, v);
        }
    }

    Array.Copy(temp, candle, candle.Length);

    // copy candle into UHHD with palette
    for (int x = 0; x < 16; x++)
    {
        for (int y = 0; y < 16; y++)
        {
            var i = 2;
            var o = (i * 3) + 1;
            var colorIndex = (int)Math.Min(255, Math.Max(0, GetPixel(candle, x, y)));
            unicornHat.Set(x, y, palette[colorIndex]);
        }
    }

    unicornHat.Show();
}

unicornHat.Off();
