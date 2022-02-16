using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace SelfEE
{
    class Program
    {
        static void Main()
        {
            XDocument xmlDoc = XDocument.Load(@"Projects\Project1.xml");    // This is where the project is determined
            var projXml = xmlDoc.Root;

            if (projXml != null && new SFX().Generate(projXml))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\nSucceeded in generating Self-extracting executable. ");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\nFailed to generate Self-extracting executable. ");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to exit...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
