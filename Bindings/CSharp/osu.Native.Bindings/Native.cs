// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Runtime.InteropServices;

namespace osu.Native.Bindings
{
    internal static unsafe class Native
    {
#if NETFRAMEWORK
        private const string lib_name = "osu.Native.dll";
#else
        private const string lib_name = "osu.Native";
#endif

        [DllImport(lib_name, EntryPoint = "LoadBeatmap_FromText")]
        public static extern ErrorCode LoadBeatmapFromText([MarshalAs(UnmanagedType.LPStr)] string beatmapData, out int beatmapHandle);

        [DllImport(lib_name, EntryPoint = "LoadBeatmap_FromFile")]
        public static extern ErrorCode LoadBeatmapFromFile([MarshalAs(UnmanagedType.LPStr)] string filePath, out int beatmapHandle);

        [DllImport(lib_name, EntryPoint = "FreeBeatmap")]
        public static extern ErrorCode FreeBeatmap(int beatmapHandle);
        
        [DllImport(lib_name, EntryPoint = "ComputeDifficulty")]
        public static extern ErrorCode ComputeDifficulty(int beatmapHandle, int rulesetId, uint mods, out double starRating);
        
        [DllImport(lib_name, EntryPoint = "ComputeStrain")]
        public static extern ErrorCode ComputeStrain(int beatmapHandle, int rulesetId, out StrainEntry* entryPtr, out int entryCount);
        
        [DllImport(lib_name, EntryPoint = "FreeStrainEntries")]
        public static extern ErrorCode FreeStrainEntries(StrainEntry* entryPtr, int entryCount);

        [DllImport(lib_name, EntryPoint = "SetLogger")]
        public static extern ErrorCode SetLogger(IntPtr logger);

        public unsafe delegate void LogDelegate(char* message);
    }
}
