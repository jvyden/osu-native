// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Extensions.EnumExtensions;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Catch.Beatmaps;
using osu.Game.Rulesets.Catch.Difficulty;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Taiko.Mods;

// ReSharper disable once CheckNamespace

namespace osu.Game.Rulesets.Catch
{
    public class CatchRuleset : Ruleset, ILegacyRuleset
    {
        public const string SHORT_NAME = "fruits";

        public override string Description => "osu!catch";

        public override string ShortName => SHORT_NAME;

        public CatchRuleset()
        {
            RulesetInfo.CreateInstanceFunc = () => new CatchRuleset();
        }

        public override Mod CreateMod<T>()
        {
            if (typeof(T) == typeof(TaikoModDoubleTime))
                return new TaikoModDoubleTime();
            if (typeof(T) == typeof(TaikoModEasy))
                return new TaikoModEasy();
            if (typeof(T) == typeof(TaikoModHalfTime))
                return new TaikoModHalfTime();
            if (typeof(T) == typeof(TaikoModHardRock))
                return new TaikoModHardRock();

            throw new System.NotImplementedException();
        }

        public override IEnumerable<Mod> ConvertFromLegacyMods(LegacyMods mods)
        {
            if (mods.HasFlagFast(LegacyMods.Nightcore))
                yield return new TaikoModDoubleTime();
            else if (mods.HasFlagFast(LegacyMods.DoubleTime))
                yield return new TaikoModDoubleTime();

            if (mods.HasFlagFast(LegacyMods.Easy))
                yield return new TaikoModEasy();

            if (mods.HasFlagFast(LegacyMods.HalfTime))
                yield return new TaikoModHalfTime();

            if (mods.HasFlagFast(LegacyMods.HardRock))
                yield return new TaikoModHardRock();

            if (mods.HasFlagFast(LegacyMods.ScoreV2))
                yield return new ModScoreV2();
        }

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap)
            => new CatchDifficultyCalculator(RulesetInfo, beatmap);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap)
            => new CatchBeatmapConverter(beatmap, this);

        public override IBeatmapProcessor? CreateBeatmapProcessor(IBeatmap beatmap)
            => new CatchBeatmapProcessor(beatmap);

        public int LegacyID => 2;
    }
}
