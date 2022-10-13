using Mclift.UnicornHatHdDotnet;

// Ported from https://github.com/pimoroni/unicorn-hat-hd/tree/master/examples
Console.WriteLine("Press a key to exit...");
var unicornHat = new UnicornHat() { Brightness = 0.6 };

var step = 0;
while (!Console.KeyAvailable)
{
    step++;
    for (var x = 0; x < 16; x++)
    {
        for (var y = 0; y < 16; y++)
        {
            var dx = (Math.Sin(step / 20.0) * 15.0) + 7.0;
            var dy = (Math.Cos(step / 15.0) * 15.0) + 7.0;
            var sc = (Math.Cos(step / 10.0) * 10.0) + 16.0;

            var h = Math.Sqrt(Math.Pow(x - dx, 2) + Math.Pow(y - dy, 2)) / sc;
            var color = ColorHelper.ColorFromHsv(h * 255.0, 1, 1);
            unicornHat.Set(x, y, color);
        }
    }

    unicornHat.Show();
}

unicornHat.Off();