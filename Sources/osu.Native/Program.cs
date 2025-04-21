// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace osu.Native
{
    public unsafe partial class Program
    {
        private static LogDelegate? _logger;
        private static readonly BeatmapHandleStore Handles = new();

        /// <summary>
        /// Sets the logger.
        /// </summary>
        /// <param name="handler">A <see cref="LogDelegate"/> callback to handle the message.</param>
        [UnmanagedCallersOnly(EntryPoint = "SetLogger", CallConvs = [typeof(CallConvCdecl)])]
        public static ErrorCode SetLogger(IntPtr handler)
        {
            _logger = Marshal.GetDelegateForFunctionPointer<LogDelegate>(handler);
            return ErrorCode.Success;
        }

        /// <summary>
        /// Computes difficulty given a beatmap.
        /// </summary>
        /// <param name="beatmapHandle">The handle of the beatmap.</param>
        /// <param name="rulesetId">The ruleset.</param>
        /// <param name="mods">The mods.</param>
        /// <param name="starRating">The returned star rating.</param>
        [UnmanagedCallersOnly(EntryPoint = "ComputeDifficulty", CallConvs = [typeof(CallConvCdecl)])]
        public static ErrorCode ComputeDifficulty(int beatmapHandle, int rulesetId, uint mods, double* starRating)
        {
            WorkingBeatmap? beatmap = Handles.Get(beatmapHandle);
            if (beatmap == null)
                return ErrorCode.BadBeatmapHandle;
            
            return ComputeDifficulty(beatmap, rulesetId, mods, starRating);
        }
        
        /// <summary>
        /// Computes strain given a beatmap.
        /// </summary>
        /// <param name="beatmapHandle">The handle of the beatmap.</param>
        /// <param name="rulesetId">The ruleset.</param>
        [UnmanagedCallersOnly(EntryPoint = "ComputeStrain", CallConvs = [typeof(CallConvCdecl)])]
        public static ErrorCode ComputeStrain(int beatmapHandle, int rulesetId)
        {
            WorkingBeatmap? beatmap = Handles.Get(beatmapHandle);
            if (beatmap == null)
                return ErrorCode.BadBeatmapHandle;

            return ComputeStrain(beatmap, rulesetId);
        }
        
        /// <summary>
        /// Computes difficulty given beatmap content.
        /// </summary>
        /// <param name="beatmapTextPtr">The beatmap content.</param>
        /// <param name="beatmapHandle">The returned handle of the beatmap.</param>
        [UnmanagedCallersOnly(EntryPoint = "LoadBeatmap_FromText", CallConvs = [typeof(CallConvCdecl)])]
        public static ErrorCode LoadBeatmapFromText(char* beatmapTextPtr, int* beatmapHandle)
        {
            string? beatmapText = Marshal.PtrToStringUTF8((IntPtr)beatmapTextPtr);
            return LoadBeatmap(beatmapText, beatmapHandle);
        }

        /// <summary>
        /// Loads a beatmap given a beatmap file path.
        /// </summary>
        /// <param name="filePathPtr">The file path.</param>
        /// <param name="beatmapHandle">The returned handle of the beatmap.</param>
        [UnmanagedCallersOnly(EntryPoint = "LoadBeatmap_FromFile", CallConvs = [typeof(CallConvCdecl)])]
        public static ErrorCode LoadBeatmapFromFile(char* filePathPtr, int* beatmapHandle)
        {
            string? filePath = Marshal.PtrToStringUTF8((IntPtr)filePathPtr);

            if (string.IsNullOrEmpty(filePath))
                return Error(ErrorCode.FileReadError, "Path is empty.");

            try
            {
                return LoadBeatmap(File.ReadAllText(filePath), beatmapHandle);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.Failure, ex.ToString());
            }
        }

        /// <summary>
        /// Frees a beatmap handle.
        /// </summary>
        /// <param name="beatmapHandle">The handle of the beatmap.</param>
        [UnmanagedCallersOnly(EntryPoint = "FreeBeatmap", CallConvs = [typeof(CallConvCdecl)])]
        public static ErrorCode FreeBeatmap(int beatmapHandle)
        {
            if (!Handles.Has(beatmapHandle))
                return ErrorCode.BadBeatmapHandle;
            
            Handles.Release(beatmapHandle);
            return ErrorCode.Success;
        }

        private static ErrorCode LoadBeatmap(string? beatmapText, int* beatmapHandle)
        {
            if (string.IsNullOrEmpty(beatmapText))
                return Error(ErrorCode.EmptyBeatmap, "Beatmap is empty.");

            try
            {
                WorkingBeatmap workingBeatmap = new StringBackedWorkingBeatmap(beatmapText);
                int id = Handles.Store(workingBeatmap);
                *beatmapHandle = id;
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.Failure, ex.ToString());
            }

            return ErrorCode.Success;
        }

        private static ErrorCode ComputeDifficulty(WorkingBeatmap workingBeatmap, int rulesetId, uint mods, double* starRating)
        {
            try
            {
                Ruleset ruleset = RulesetHelper.CreateRuleset(rulesetId);
                Mod[] rulesetMods = ruleset.ConvertFromLegacyMods((LegacyMods)mods).ToArray();

                *starRating = ruleset.CreateDifficultyCalculator(workingBeatmap)
                                     .Calculate(rulesetMods)
                                     .StarRating;

                return ErrorCode.Success;
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.Failure, ex.ToString());
            }
        }
        
        private static ErrorCode ComputeStrain(WorkingBeatmap workingBeatmap, int rulesetId)
        {
            try
            {
                Ruleset ruleset = RulesetHelper.CreateRuleset(rulesetId);

                DifficultyCalculator calculator = ruleset.CreateDifficultyCalculator(workingBeatmap);
                if (calculator is not IStrainCalculator strainCalculator)
                    return ErrorCode.InvalidRuleset;

                Dictionary<StrainSkill, List<double>> strain = strainCalculator.CalculateStrain();
                foreach (KeyValuePair<StrainSkill, List<double>> kvp in strain)
                {
                    Console.WriteLine(kvp.Key.GetType().Name);
                    foreach (double d in kvp.Value)
                    {
                        Console.WriteLine(d);
                    }
                }
        
                return ErrorCode.Success;
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.Failure, ex.ToString());
            }
        }

        private static ErrorCode Error(ErrorCode code, string description)
        {
            if (_logger != null)
            {
                IntPtr msgPtr = Marshal.StringToHGlobalUni(description);
                _logger((char*)msgPtr);
                Marshal.FreeHGlobal(msgPtr);
            }

            return code;
        }

        private delegate void LogDelegate(char* message);
    }
}
