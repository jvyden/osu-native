using System;
using System.Diagnostics;

namespace osu.Native.Bindings
{
    public sealed class LazerNativeException : Exception
    {
        public LazerNativeException(ErrorCode code) : base(GetMessageForCode(code))
        {
            Debug.Assert(code != ErrorCode.Success, "should never throw native exception for success");
        }

        private static string GetMessageForCode(ErrorCode code)
        {
            return code switch
            {
                ErrorCode.Success => "The operation was a success.",
                ErrorCode.EmptyBeatmap => "The provided beatmap was empty.",
                ErrorCode.FileReadError => "The file could not be read.",
                ErrorCode.BadBeatmapHandle => "The handle passed was invalid or missing.",
                ErrorCode.InvalidRuleset => "The ruleset does not support this operation.",
                ErrorCode.Failure => "An unknown failure occurred. Check the log for more details.",
                _ => $"Unknown error code {code.ToString()}. Check the log for more details."
            };
        }
    }
}