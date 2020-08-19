using CommandLine;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ArchiShow
{
    class Program
    {
        static Options _options;
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o => _options = o);

            string path = _options.GetPath();
            var files = File.Exists(path)
                ? new string[] { path }
                : (Directory.Exists(path) ? Directory.GetFiles(path) : new string[0]);

            var filesFiltered = files.Where(f => IsExecutable(f));

            //write header
            Console.WriteLine("Architecture,FileName");

            //write all files data
            foreach (var file in filesFiltered)
            {
                Console.Write(GetArchitecture(file));
                Console.Write(",");
                Console.Write(Path.GetFileName(file));
                Console.WriteLine();
            }
        }

        const ushort PE32 = 0x10B;
        const ushort PE64 = 0x20B;
        public static ushort GetPEArchitecture(string pFilePath)
        {
            //taken from: https://stackoverflow.com/a/9767750/1451645
            ushort architecture = 0;
            try
            {
                using (FileStream fStream = new FileStream(pFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BinaryReader bReader = new BinaryReader(fStream))
                    {
                        if (bReader.ReadUInt16() == 23117) //check the MZ signature
                        {
                            fStream.Seek(0x3A, SeekOrigin.Current); //seek to e_lfanew.
                            fStream.Seek(bReader.ReadUInt32(), SeekOrigin.Begin); //seek to the start of the NT header.
                            if (bReader.ReadUInt32() == 17744) //check the PE\0\0 signature.
                            {
                                fStream.Seek(20, SeekOrigin.Current); //seek past the file header,
                                architecture = bReader.ReadUInt16(); //read the magic number of the optional header.
                            }
                        }
                    }
                }
            }
            catch { }

            return architecture;
        }

        static string GetDotNetArchitecture(string file, string defaultValue)
        {
            try
            {
                var asm = Assembly.ReflectionOnlyLoadFrom(file);
                var asmName = asm.GetName();
                switch (asmName.ProcessorArchitecture)
                {
                    case ProcessorArchitecture.None:
                        break;
                    case ProcessorArchitecture.MSIL:
                        return Architecture.AnyCpu;
                    case ProcessorArchitecture.X86:
                        return Architecture.X86;
                    case ProcessorArchitecture.IA64:
                        return Architecture.X64;
                    case ProcessorArchitecture.Amd64:
                        return Architecture.X64;
                    default:
                        return asmName.ProcessorArchitecture.ToString();
                }
            }
            catch (BadImageFormatException)
            {
                //not a .NET assembly
            }

            return defaultValue;
        }

        static string GetArchitecture(string file)
        {
            var architecture = GetPEArchitecture(file);
            if (architecture == PE32)
            {
                return GetDotNetArchitecture(file, Architecture.X86);
            }
            else if (architecture == PE64)
            {
                return Architecture.X64;
            }
            else
            {
                return $"0x{architecture:X}";
            }
        }

        static string[] _executableExtensions = { "exe", "dll" };
        public static bool IsExecutable(string file)
        {
            return _executableExtensions.Contains(Path.GetExtension(file).Trim('.'), StringComparer.InvariantCultureIgnoreCase);
        }

        static class Architecture
        {
            public static string X86 = "x86";
            public static string X64 = "x64";
            public static string AnyCpu = "AnyCPU";
        }
    }
}
