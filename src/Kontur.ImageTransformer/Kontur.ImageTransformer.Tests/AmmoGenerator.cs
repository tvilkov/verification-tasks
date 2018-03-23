using System;
using System.Drawing;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Kontur.ImageTransformer.Tests
{
    [TestFixture]
    public class AmmoGenerator
    {
        [Test]
        public void GenerateAmmo()
        {
            var sourceFolder = Path.Combine(TestData.RootFolder, "32bit");
            const string outputFile = @"d:\ammo.txt";
            var filters = "rotate-cw,rotate-ccw,flip-h,flip-v".Split(',');

            using (var output = File.Create(outputFile))
            {
                var buf = Encoding.ASCII.GetBytes("[Connection: Keep-Alive]\n[User-Agent: Yandex-Tank]\n[Content-Type: application/octet-stream]\n");
                output.Write(buf, 0, buf.Length);

                foreach (var file in Directory.GetFiles(sourceFolder, "*.png"))
                {
                    if (file.IndexOf("zebra", StringComparison.InvariantCulture) != -1) continue;

                    var image = Image.FromFile(file, true);
                    var imageContent = File.ReadAllBytes(file);
                    Console.WriteLine($"Generating ammo for '{file}'");
                    foreach (var filter in filters)
                    {
                        Console.Write($"\tFilter '{filter}'... ");
                        buf = Encoding.ASCII.GetBytes($"{imageContent.Length} /process/{filter}/0,0,{image.Width - 1},{image.Height - 1}\n");
                        output.Write(buf, 0, buf.Length);
                        output.Write(imageContent, 0, imageContent.Length);
                        buf = Encoding.ASCII.GetBytes("\n");
                        output.Write(buf, 0, buf.Length);
                        Console.WriteLine("Done");
                    }
                }

                output.Flush();
            }

        }
    }
}