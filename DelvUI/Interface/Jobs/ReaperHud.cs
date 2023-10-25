using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Enums;
using DelvUI.Helpers;
using DelvUI.Interface.Bars;
using DelvUI.Interface.GeneralElements;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DelvUI.Interface.Jobs
{
    public class ReaperHud : JobHud
    {
        private new ReaperConfig Config => (ReaperConfig)_config;

        public ReaperHud(JobConfig config, string? displayName = null) : base(config, displayName)
        {
        }

        protected override (List<Vector2>, List<Vector2>) ChildrenPositionsAndSizes()
        {
            List<Vector2> positions = new();
            List<Vector2> sizes = new();

            if (Config.DeathsDesignBar.Enabled)
            {
                positions.Add(Config.Position + Config.DeathsDesignBar.Position);
                sizes.Add(Config.DeathsDesignBar.Size);
            }

            if (Config.SoulBar.Enabled)
            {
                positions.Add(Config.Position + Config.SoulBar.Position);
                sizes.Add(Config.SoulBar.Size);
            }

            if (Config.ShroudBar.Enabled)
            {
                positions.Add(Config.Position + Config.ShroudBar.Position);
                sizes.Add(Config.ShroudBar.Size);
            }

            if (Config.DeathGauge.Enabled)
            {
                positions.Add(Config.Position + Config.DeathGauge.Position);
                sizes.Add(Config.DeathGauge.Size);
            }

            return (positions, sizes);
        }

        public override void DrawJobHud(Vector2 origin, PlayerCharacter player)
        {
            Vector2 pos = origin + Config.Position;
            RPRGauge gauge = Plugin.JobGauges.Get<RPRGauge>();

            if (Config.DeathsDesignBar.Enabled)
            {
                DrawDeathsDesignBar(pos, player);
            }

            if (Config.SoulBar.Enabled)
            {
                DrawSoulGauge(pos, gauge, player);
            }

            if (Config.ShroudBar.Enabled)
            {
                DrawShroudGauge(pos, gauge, player);
            }

            if (Config.DeathGauge.Enabled)
            {
                DrawDeathGauge(pos, gauge, player);
            }
        }

        private void DrawDeathsDesignBar(Vector2 origin, PlayerCharacter player)
        {
            GameObject? actor = Plugin.TargetManager.SoftTarget ?? Plugin.TargetManager.Target;
            float duration = 0f;

            if (actor is BattleChara target)
            {
                duration = Utils.StatusListForBattleChara(target).FirstOrDefault(o => o.StatusId is 2586 && o.SourceId == player.ObjectId && o.RemainingTime > 0)?.RemainingTime ?? 0f;
            }

            if (!Config.DeathsDesignBar.HideWhenInactive || duration > 0)
            {
                Config.DeathsDesignBar.Label.SetValue(duration);

                BarHud[] bars = BarUtilities.GetChunkedProgressBars(Config.DeathsDesignBar, 2, duration, 60f, 0f, player);
                foreach (BarHud bar in bars)
                {
                    AddDrawActions(bar.GetDrawActions(origin, Config.DeathsDesignBar.StrataLevel));
                }
            }
        }

        private void DrawSoulGauge(Vector2 origin, RPRGauge gauge, PlayerCharacter player)
        {
            float soul = gauge.Soul;

            if (!Config.SoulBar.HideWhenInactive || soul > 0)
            {
                Config.SoulBar.Label.SetValue(soul);

                BarHud[] bars = BarUtilities.GetChunkedProgressBars(Config.SoulBar, 2, soul, 100f, 0f, player);
                foreach (BarHud bar in bars)
                {
                    AddDrawActions(bar.GetDrawActions(origin, Config.SoulBar.StrataLevel));
                }
            }
        }

        private void DrawShroudGauge(Vector2 origin, RPRGauge gauge, PlayerCharacter player)
        {
            float shroud = gauge.Shroud;

            if (!Config.ShroudBar.HideWhenInactive || shroud > 0)
            {
                Config.ShroudBar.Label.SetValue(shroud);

                BarHud[] bars = BarUtilities.GetChunkedProgressBars(Config.ShroudBar, 2, shroud, 100f, 0f, player);
                foreach (BarHud bar in bars)
                {
                    AddDrawActions(bar.GetDrawActions(origin, Config.ShroudBar.StrataLevel));
                }
            }
        }

        private void DrawDeathGauge(Vector2 origin, RPRGauge gauge, PlayerCharacter player)
        {
            var lemureShroud = gauge.LemureShroud;
            var voidShroud = gauge.VoidShroud;

            if (!Config.DeathGauge.HideWhenInactive || gauge.EnshroudedTimeRemaining > 0)
            {
                var deathChunks = new Tuple<PluginConfigColor, float, LabelConfig?>[5];

                int i = 0;
                for (; i < lemureShroud && i < deathChunks.Length; i++)
                {
                    deathChunks[i] = new(Config.DeathGauge.LemureShroudColor, 1f, i == 2 ? Config.DeathGauge.EnshroudTimerLabel : null);
                }

                for (; i < lemureShroud + voidShroud && i < deathChunks.Length; i++)
                {
                    deathChunks[i] = new(Config.DeathGauge.VoidShroudColor, 1f, i == 2 ? Config.DeathGauge.EnshroudTimerLabel : null);
                }

                for (; i < deathChunks.Length; i++)
                {
                    deathChunks[i] = new(Config.DeathGauge.VoidShroudColor, 0f, i == 2 ? Config.DeathGauge.EnshroudTimerLabel : null);
                }

                Config.DeathGauge.EnshroudTimerLabel.SetValue(gauge.EnshroudedTimeRemaining / 1000);

                BarHud[] bars = BarUtilities.GetChunkedBars(Config.DeathGauge, deathChunks, player);
                foreach (BarHud bar in bars)
                {
                    AddDrawActions(bar.GetDrawActions(origin, Config.DeathGauge.StrataLevel));
                }
            }
        }
    }

    [Section("职业特殊条")]
    [SubSection("近战", 0)]
    [SubSection("钐镰客", 1)]
    public class ReaperConfig : JobConfig
    {
        [JsonIgnore] public override uint JobId => JobIDs.RPR;

        public new static ReaperConfig DefaultConfig()
        {
            var config = new ReaperConfig();
            config.DeathsDesignBar.UseChunks = false;
            config.DeathsDesignBar.Label.Enabled = true;

            config.SoulBar.UseChunks = false;
            config.SoulBar.Label.Enabled = true;

            config.ShroudBar.UseChunks = false;
            config.ShroudBar.Label.Enabled = true;

            config.DeathGauge.EnshroudTimerLabel.HideIfZero = true;

            return config;
        }

        [NestedConfig("死亡烙印条", 35)]
        public ChunkedProgressBarConfig DeathsDesignBar = new ChunkedProgressBarConfig(
            new(0, -10),
            new(254, 20),
            new PluginConfigColor(new Vector4(145f / 255f, 0f / 255f, 25f / 255f, 100f / 100f))
        );

        [NestedConfig("灵魂条", 40)]
        public ChunkedProgressBarConfig SoulBar = new ChunkedProgressBarConfig(
            new(0, -32),
            new(254, 20),
            new PluginConfigColor(new Vector4(254f / 255f, 21f / 255f, 94f / 255f, 100f / 100f))
        );

        [NestedConfig("魂衣条", 45)]
        public ChunkedProgressBarConfig ShroudBar = new ChunkedProgressBarConfig(
            new(0, -54),
            new(254, 20),
            new PluginConfigColor(new Vector4(0f / 255f, 176f / 255f, 196f / 255f, 100f / 100f))
        );

        [NestedConfig("夜游魂量谱", 50)]
        public DeathGauge DeathGauge = new DeathGauge(
            new(0, -76),
            new(254, 20),
            new PluginConfigColor(new(0, 0, 0, 0))
        );
    }

    [DisableParentSettings("FillColor")]
    public class DeathGauge : ChunkedBarConfig
    {
        [ColorEdit4("夜游魂颜色")]
        [Order(21)]
        public PluginConfigColor LemureShroudColor = new PluginConfigColor(new Vector4(0f / 255f, 176f / 255f, 196f / 255f, 100f / 100f));

        [ColorEdit4("虚无魂颜色")]
        [Order(22)]
        public PluginConfigColor VoidShroudColor = new PluginConfigColor(new Vector4(150f / 255f, 90f / 255f, 144f / 255f, 100f / 100f));

        [NestedConfig("夜游魂衣持续时间文本", 50, spacing = true)]
        public NumericLabelConfig EnshroudTimerLabel;

        public DeathGauge(Vector2 position, Vector2 size, PluginConfigColor fillColor, int padding = 2) : base(position, size, fillColor, padding)
        {
            EnshroudTimerLabel = new NumericLabelConfig(Vector2.Zero, "", DrawAnchor.Center, DrawAnchor.Center);
        }
    }
}
