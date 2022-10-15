using Mclift.UnicornHatHdDotnet;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

Console.WriteLine("Press a key to exit...");

// Image source: https://opengameart.org/content/16x16-chibi-rpg-character-builder-v2
// See README.md for licence information.

using var spriteSheet = Image.Load<Rgb24>("example-characters.png");
using var unicornHat = new UnicornHat() { Brightness = 0.6 };

int[] frameSequence = { 0, 1, 2, 1 };

(int x, int y) GetFrameOffset(int step, int movement)
{
    return ((frameSequence[step % 4]) * 16, movement * 16);
}

void DrawFrame(int xOffset, int yOffset)
{
    for (int x = 0; x < 16; x++)
    {
        for (int y = 0; y < 16; y++)
        {
            var color = spriteSheet[xOffset + x, yOffset + y];
            unicornHat.Set(x, y, color.R, color.G, color.B);
        }
    }
}

var done = false;
var step = 0;
var movement = 0;

while (!done)
{
    for (var ty = 0; ty < 8 && !done; ty++)
    {
        for (var tx = 0; tx < 8 && !done; tx++)
        {
            while (movement < 4)
            {
                var frameOffset = GetFrameOffset(step, movement);
                DrawFrame(tx * 48 + frameOffset.x, ty * 64 + frameOffset.y);
                unicornHat.Show();

                if (Console.KeyAvailable)
                {
                    done = true;
                    break;
                }

                Thread.Sleep(100);
                if (++step % 8 == 0)
                {
                    movement++;
                }
            }

            movement = 0;
        }
    }
}

unicornHat.Off();