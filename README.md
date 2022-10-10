# A .NET Library for the Pimoroni Unicorn HAT HD

A .NET library to drive the Pimoroni [Unicorn HAT HD](http://shop.pimoroni.com/products/unicorn-hat-hd) RGB LED matrix on the Raspberry Pi. This library and usage examples are based on [Pimoroni's Python source code](https://github.com/pimoroni/unicorn-hat-hd) on GitHub.

## Requirements

* A Raspberry Pi with the .NET 6 runtime installed on Linux.
* The Unicorn HAT HD connected to the Raspberry Pi.
* SPI enabled – see [Manual install](https://github.com/pimoroni/unicorn-hat-hd#manual-install) notes in the pimoroni/unicorn-hat-hd repository.

## Usage

Create a new console application using the CLI command:
```
dotnet new console
```

Include the library with the command:
```
dotnet add package Mclift.UnicornHatHdDotnet
```

Edit Program.cs and change its contents to:
```
using Mclift.UnicornHatHdDotnet;

using var hat = new UnicornHat();
hat.SetAll(128, 0, 64);
hat.Show();
Console.WriteLine("Press RETURN to exit...");
Console.ReadLine();
hat.Off();
```

Run the sample with the command, replacing /path/to/dotnet with the actual path to your .NET binary – you can find this by typing `which dotnet`:
```
sudo -E /path/to/dotnet run
```

The LED matrix should light up purple and then turn black again when you press RETURN.

## Contributing

Issues and pull requests are welcome!
