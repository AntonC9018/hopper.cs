using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hopper.Mine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("Hello");

            try
            {
                Assembly lib = typeof(Hopper.Core.Action).Assembly;
                foreach (Type type in lib.GetTypes())
                {
                    Console.WriteLine(type.FullName);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {                
                        if(!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();

                Console.WriteLine(errorMessage);
            }
        }
    }
}