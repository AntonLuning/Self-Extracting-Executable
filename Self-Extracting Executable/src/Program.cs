using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace SelfEE
{
    class Program
    {
        static void Main()
        {
            XDocument xmlDoc = XDocument.Load(@"Projects\ExampleProject.xml");    // This is where the project is determined
            XElement? projXml = xmlDoc.Root;

            if (projXml != null && new SFX().Generate(projXml))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\nGenerating Self-extracting executable SUCCEEDED. ");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\nGenerating Self-extracting executable FAILED. ");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to exit...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
