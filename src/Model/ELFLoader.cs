using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
/// <summary>
/// File: ELFLoader.cs
/// A class that loads ELF file into RAM.
/// </summary>
/// 

namespace armsim.Model
{
    //a class that loads ELF files into memory (RAM) by decoding the ELF headers to find the program segments.s
    class ELFLoader
    {
        public List<ProgramHeader> progHeaders = new List<ProgramHeader>();
        public ELF elfHeader = new ELF();

        string log = "ELFLoader.ReadELF: "; //To make logging easier.

        //Takes ELF file and reads the program segments from it
        public void ReadELF(ref Memory RAM, string fileName)
        {
            if (!File.Exists(fileName))
                throw new IOException(log + "The file requested does not exist.");

            Tracer.Log(log + "Opening " + fileName + "...");
            //Initial code from class examples
            using (FileStream strm = new FileStream(fileName, FileMode.Open))
            {
                byte[] data = new byte[Marshal.SizeOf(elfHeader)];

                // Read ELF header data
                strm.Read(data, 0, data.Length);

                //Just in case the file isn't ELF.
                if (data[0] != 0x7f || data[1] != 'E' || data[2] != 'L' || data[3] != 'F')
                    throw new FormatException(log + "The file provided is not an ELF file.");

                // Convert to struct
                elfHeader = ByteArrayToStructure<ELF>(data);

                Tracer.Log(log + "Program entry point: " + elfHeader.e_entry.ToString("X4"));
                Tracer.Log(log + "Number of program header entries: " + elfHeader.e_phnum);
                //the stuff is stored in elfHeader

                Tracer.Log(log + "Extracting program headers.");
                ReadProgramHeaders(strm);
                Tracer.Log(log + "Done...");

                Tracer.Log(log + "Reading program headers...");
                LoadToRAM(ref RAM, strm);
                Tracer.Log(log + "Successfully read program headers.");
            }
        }

        //Reads program header from ELF file
        void ReadProgramHeaders(FileStream strm)
        {
            byte[] data = new byte[elfHeader.e_phentsize];

            // Read first program header entry
            strm.Seek(elfHeader.e_phoff, SeekOrigin.Begin);

            for (int i = 0; i < elfHeader.e_phnum; ++i)
            {
                strm.Read(data, 0, elfHeader.e_phentsize);

                progHeaders.Add(ExtractProgramHeader(data));
                Tracer.Log(log + "Segment " + (i + 1) + "- Address = " + progHeaders[i].p_vaddr + " - Offset = " + progHeaders[i].p_offset + " - Size = " + progHeaders[i].p_filesz);
            }
        }

        //Loads data to RAM, BRING IN THE FIREWORKS!!!!
        void LoadToRAM(ref Memory RAM, FileStream strm)
        {
            if (progHeaders == null) //I doubt this will ever be reached, but just in case...
                throw new NullReferenceException(log + "There are no segments to load.");

            for (int i = 0; i < progHeaders.Count; ++i)
            {
                strm.Seek(progHeaders[i].p_offset, SeekOrigin.Begin);

                Tracer.Log(log + "Loading segment " + (i + 1) + " into RAM at address " + progHeaders[i].p_vaddr);

                for (int j = 0; j < progHeaders[i].p_filesz; ++j) //load bytes into RAM
                    RAM.WriteByte(progHeaders[i].p_vaddr + j, (byte)strm.ReadByte());
            }
        }

        //Converts array of bytes into program header struct
        ProgramHeader ExtractProgramHeader(byte[] data)
        {
            ProgramHeader pHeader;

            pHeader.p_type = FourBytesToInt(data, 0);
            pHeader.p_offset = FourBytesToInt(data, 4);
            pHeader.p_vaddr = FourBytesToInt(data, 8);
            pHeader.p_paddr = FourBytesToInt(data, 12);
            pHeader.p_filesz = FourBytesToInt(data, 16);
            pHeader.p_memsz = FourBytesToInt(data, 20);
            pHeader.p_flags = FourBytesToInt(data, 24);
            pHeader.p_align = FourBytesToInt(data, 28);

            return pHeader;
        }

        //Converts little endian group of 4 bytes into a regular integer number.
        short FourBytesToInt(byte[] arr, int offset)
        {
            short res = 0;

            res += arr[offset];
            res += (short)(arr[offset + 1] << 8);
            res += (short)(arr[offset + 2] << 16);
            res += (short)(arr[offset + 3] << 24);

            return res;
        }

        // Converts a byte array to a struct
        static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),
                typeof(T));
            handle.Free();
            return stuff;
        }

    }
}
