using Mclift.UnicornHatHdDotnet;

// Ported from https://github.com/pimoroni/unicorn-hat-hd/tree/master/examples
Console.WriteLine("Press a key to exit...");
var unicornHat = new UnicornHat() { Brightness = 0.6 };

var random = new Random();
double[] CreateStar()
{
    return new[] { random.NextDouble() * 7 + 4, random.NextDouble() * 7 + 4, 0 };
}

try
{
    var starCount = 25;
    var starSpeed = 0.05;
    var stars = new List<double[]>();

    for (var i = 0; i < starCount; i++)
    {
        stars.Add(CreateStar());
    }

    while (!Console.KeyAvailable)
    {
        unicornHat.Clear();

        for (int i = 0; i < starCount; i++)
        {
            var star = stars[i];
            star[0] = star[0] + ((star[0] - 8.1) * starSpeed);
            star[1] = star[1] + ((star[1] - 8.1) * starSpeed);
            star[2] = star[2] + starSpeed * 50;

            if (star[0] < 0.0 || star[1] < 0.0 || star[0] >= 16.0 || star[1] >= 16.0)
            {
                star = stars[i] = CreateStar();
            }

            var brightness = (byte)Math.Min(255, star[2]);
            try
            {
                unicornHat.Set((int)star[0], (int)star[1], brightness, brightness, brightness);
            }
            catch(Exception)
            {
                Console.WriteLine($"Failed at: {(int)star[0]},{(int)star[1]},{brightness}");
                throw;
            }
        }

        unicornHat.Show();
    }
}
finally
{
    unicornHat.Off();
}


