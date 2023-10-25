using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Interface.Bars;
using System.Numerics;

namespace DelvUI.Interface.GeneralElements
{
    [DisableParentSettings("Position")]
    [Section("其他元素")]
    [SubSection("跳蓝监控", 0)]
    public class MPTickerConfig : MovablePluginConfigObject
    {
        [Checkbox("满蓝时隐藏", spacing = false)]
        [Order(15)]
        public bool HideOnFullMP = true;

        [Checkbox("仅当为黑魔时启用")]
        [Order(20)]
        public bool EnableOnlyForBLM = false;

        [Checkbox("仅当处于灵极冰时显示")]
        [Order(25, collapseWith = nameof(EnableOnlyForBLM))]
        public bool ShowOnlyDuringUmbralIce = true;

        [NestedConfig("跳蓝监控条", 30)]
        public MPTickerBarConfig Bar = new MPTickerBarConfig(
            Vector2.Zero,
            new Vector2(254, 8),
            new PluginConfigColor(new(240f / 255f, 92f / 255f, 232f / 255f, 100f / 100f))
        );

        [NestedConfig("可见性", 70)]
        public VisibilityConfig VisibilityConfig = new VisibilityConfig();

        public new static MPTickerConfig DefaultConfig()
        {
            var config = new MPTickerConfig();
            config.Enabled = false;
            config.Bar.Position = new Vector2(0, HUDConstants.BaseHUDOffsetY + 27);

            return config;
        }
    }

    [Disableable(false)]
    [DisableParentSettings("HideWhenInactive")]
    [Exportable(false)]
    public class MPTickerBarConfig : BarConfig
    {
        [NestedConfig("火3阈值（仅黑魔）", 50, separator = false, spacing = true)]
        public MPTickerFire3ThresholdConfig Fire3Threshold = new MPTickerFire3ThresholdConfig();

        public MPTickerBarConfig(Vector2 position, Vector2 size, PluginConfigColor fillColor)
            : base(position, size, fillColor)
        {
        }
    }

    [Exportable(false)]
    [DisableParentSettings("Value")]
    public class MPTickerFire3ThresholdConfig : ThresholdConfig
    {
        [DragFloat("估算火3咏唱时间", min = 0f, max = 10)]
        [Order(11)]
        public float Fire3CastTime = 1.5f;

        public MPTickerFire3ThresholdConfig()
        {
            Enabled = false;
            ThresholdType = ThresholdType.Above;
            ShowMarker = true;
            MarkerColor = new(new Vector4(255f / 255f, 136f / 255f, 0 / 255f, 90f / 100f));
        }
    }
}
