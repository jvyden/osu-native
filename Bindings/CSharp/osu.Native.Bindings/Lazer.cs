// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace osu.Native.Bindings
{
    public unsafe class Lazer
    {
        public event Action<string>? Log;

        private static Native.LogDelegate? logHandler;

        public Lazer()
        {
            Native.SetLogger(Marshal.GetFunctionPointerForDelegate(logHandler = onLazerLog));
        }

        private void onLazerLog(char* message)
        {
            Log?.Invoke(Marshal.PtrToStringUni((IntPtr)message));
        }

        public LazerBeatmap LoadDifficultyFromFile(FileInfo file)
        {
            ErrorCode code = Native.LoadBeatmapFromFile(file.FullName, out int beatmapHandle);
            if (code != ErrorCode.Success)
                throw new LazerNativeException(code);

            return new LazerBeatmap(beatmapHandle);
        }
        
        public LazerBeatmap LoadDifficultyFromText(string beatmapData)
        {
            ErrorCode code = Native.LoadBeatmapFromText(beatmapData, out int beatmapHandle);
            if (code != ErrorCode.Success)
                throw new LazerNativeException(code);

            return new LazerBeatmap(beatmapHandle);
        }

        public double ComputeDifficulty(LazerBeatmap beatmap, int rulesetId, uint mods)
        {
            ErrorCode code = Native.ComputeDifficulty(beatmap.Handle, rulesetId, mods, out double starRating);
            if (code != ErrorCode.Success)
                throw new LazerNativeException(code);

            return starRating;
        }
        
        public unsafe Dictionary<string, double[]> ComputeStrain(LazerBeatmap beatmap, int rulesetId)
        {
            ErrorCode code = Native.ComputeStrain(beatmap.Handle, rulesetId, out StrainEntry* entryPtr, out int entryCount);
            if (code != ErrorCode.Success)
                throw new LazerNativeException(code);
            
            Dictionary<string, double[]> strains = new(entryCount);

            for (int i = 0; i < entryCount; i++)
            {
                StrainEntry entry = entryPtr[i];
                double[] values = new double[entry.ValueCount];
                
                Marshal.Copy(entry.Values, values, 0, entry.ValueCount);
                strains.Add(Marshal.PtrToStringUni(entry.SkillType)!, values);
            }

            code = Native.FreeStrainEntries(entryPtr, entryCount);
            if (code != ErrorCode.Success)
                throw new LazerNativeException(code);

            return strains;
        }
    }
}
