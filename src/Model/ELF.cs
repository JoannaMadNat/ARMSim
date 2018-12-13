using System.Runtime.InteropServices;
/// <summary>
/// File: Elf.cs
/// Demonstrates reading an ELF format file
/// </summary>
/// 

namespace armsim.Model
{
    // A struct that mimics memory layout of ELF file header
    // See http://www.sco.com/developers/gabi/latest/contents.html for details
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ELF
    {
        public byte EI_MAG0, EI_MAG1, EI_MAG2, EI_MAG3, EI_CLASS, EI_DATA, EI_VERSION;
        byte unused1, unused2, unused3, unused4, unused5, unused6, unused7, unused8, unused9;
        public ushort e_type;
        public ushort e_machine;
        public uint e_version;
        public uint e_entry;
        public uint e_phoff;
        public uint e_shoff;
        public uint e_flags;
        public ushort e_ehsize;
        public ushort e_phentsize;
        public ushort e_phnum;
        public ushort e_shentsize;
        public ushort e_shnum;
        public ushort e_shstrndx;
    }


    //a struct that holds all the contents of a program headers
    struct ProgramHeader
    {
        public short p_type;
        public short p_offset;
        public short p_vaddr;
        public short p_paddr;
        public short p_filesz;
        public short p_memsz;
        public short p_flags;
        public short p_align;

    }
}
