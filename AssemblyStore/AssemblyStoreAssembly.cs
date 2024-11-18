using K4os.Compression.LZ4;
using System;
using System.IO;
using System.Text;

namespace Xamarin.Android.AssemblyStore
{
    public class AssemblyStoreAssembly
    {
        public uint DataOffset { get; }
        public uint DataSize { get; }
        public uint DebugDataOffset { get; }
        public uint DebugDataSize { get; }
        public uint ConfigDataOffset { get; }
        public uint ConfigDataSize { get; }

        public uint Hash32 { get; set; }
        public ulong Hash64 { get; set; }
        public string Name { get; set; } = String.Empty;
        public uint RuntimeIndex { get; set; }

        public AssemblyStoreReader Store { get; }
        public string DllName => MakeFileName("dll");
        public string PdbName => MakeFileName("pdb");
        public string ConfigName => MakeFileName("dll.config");

        internal AssemblyStoreAssembly(BinaryReader reader, AssemblyStoreReader store)
        {
            Store = store;

            DataOffset = reader.ReadUInt32();
            DataSize = reader.ReadUInt32();
            DebugDataOffset = reader.ReadUInt32();
            DebugDataSize = reader.ReadUInt32();
            ConfigDataOffset = reader.ReadUInt32();
            ConfigDataSize = reader.ReadUInt32();
        }

        public void ExtractImage(string outputDirPath, string? fileName = null, bool decompress = true)
        {
            var outputFilePath = MakeOutputFilePath(outputDirPath, "dll", fileName);
            Store.ExtractAssemblyImage(this, outputFilePath);
            if (decompress)
                DecompressDll(outputFilePath);
        }

        public static void DecompressDll(string path)
        {
            var compressedData = File.ReadAllBytes(path);
            var header = Encoding.ASCII.GetString(compressedData[0..4]);
            if (header != "XALZ")
                return;

            var unpackLength = BitConverter.ToInt32(compressedData[8..12]);
            var payload = compressedData[12..];
            byte[] decompressedData = new byte[unpackLength];
            LZ4Codec.Decode(payload, decompressedData);
            File.WriteAllBytes(path, decompressedData);
            //Console.WriteLine($"Decompressed LZ4: {path}");
        }

        public void ExtractImage(Stream output)
        {
            Store.ExtractAssemblyImage(this, output);
        }

        public void ExtractDebugData(string outputDirPath, string? fileName = null)
        {
            Store.ExtractAssemblyDebugData(this, MakeOutputFilePath(outputDirPath, "pdb", fileName));
        }

        public void ExtractDebugData(Stream output)
        {
            Store.ExtractAssemblyDebugData(this, output);
        }

        public void ExtractConfig(string outputDirPath, string? fileName = null)
        {
            Store.ExtractAssemblyConfig(this, MakeOutputFilePath(outputDirPath, "dll.config", fileName));
        }

        public void ExtractConfig(Stream output)
        {
            Store.ExtractAssemblyConfig(this, output);
        }

        string MakeOutputFilePath(string outputDirPath, string extension, string? fileName)
        {
            return Path.Combine(outputDirPath, MakeFileName(extension, fileName));
        }

        string MakeFileName(string extension, string? fileName = null)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = Name;

                if (String.IsNullOrEmpty(fileName))
                {
                    fileName = $"{Hash32:x}_{Hash64:x}";
                }

                fileName = $"{fileName}.{extension}";
            }

            return fileName!;
        }
    }
}
