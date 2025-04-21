// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
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
        
        public void ComputeStrain(LazerBeatmap beatmap, int rulesetId)
        {
            ErrorCode code = Native.ComputeStrain(beatmap.Handle, rulesetId);
            if (code != ErrorCode.Success)
                throw new LazerNativeException(code);
        }
    }
}
