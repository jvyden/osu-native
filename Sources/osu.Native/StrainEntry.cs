using System;
using System.Runtime.InteropServices;

namespace osu.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct StrainEntry
{
    public IntPtr SkillType;
    public IntPtr Values;
    public int ValueCount;
}