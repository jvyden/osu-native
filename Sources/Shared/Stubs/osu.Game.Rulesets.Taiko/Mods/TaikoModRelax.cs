﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;

// ReSharper disable once CheckNamespace

namespace osu.Game.Rulesets.Taiko.Mods
{
    public class TaikoModRelax : ModRelax
    {
        public override LocalisableString Description => "No need to remember which key is correct anymore!";

        public void ApplyToDrawableHitObject(DrawableHitObject drawableHitObject)
        {
            throw new NotImplementedException();
        }
    }
}
