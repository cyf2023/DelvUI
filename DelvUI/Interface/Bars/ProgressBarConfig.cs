using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Enums;
using DelvUI.Interface.GeneralElements;
using System.Numerics;

namespace DelvUI.Interface.Bars
{
    [Exportable(false)]
    public class ProgressBarConfig : BarConfig
    {
        [NestedConfig("阈值", 45)]
        public ThresholdConfig ThresholdConfig = new ThresholdConfig();

        [NestedConfig("条文本", 1000)]
        public NumericLabelConfig Label;

        public ProgressBarConfig(
            Vector2 position,
            Vector2 size,
            PluginConfigColor fillColor,
            BarDirection fillDirection = BarDirection.Right,
            PluginConfigColor? threshHoldColor = null,
            float threshold = 0f) : base(position, size, fillColor, fillDirection)
        {
            Label = new NumericLabelConfig(Vector2.Zero, "", DrawAnchor.Center, DrawAnchor.Center);
            ThresholdConfig.Color = threshHoldColor ?? ThresholdConfig.Color;
            ThresholdConfig.Value = threshold;
        }
    }

    [Exportable(false)]
    public class ThresholdConfig : PluginConfigObject
    {
        [DragFloat("阈值", min = 0f, max = 10000f)]
        [Order(10)]
        public float Value = 0f;

        [Checkbox("改变颜色")]
        [Order(15)]
        public bool ChangeColor = true;

        [Combo("高于/低于阈值时激活", "高于", "低于")]
        [Order(20, collapseWith = nameof(ChangeColor))]
        public ThresholdType ThresholdType = ThresholdType.Below;

        [ColorEdit4("颜色")]
        [Order(25, collapseWith = nameof(ChangeColor))]
        public PluginConfigColor Color = new PluginConfigColor(new(230f / 255f, 33f / 255f, 33f / 255f, 100f / 100f));

        [Checkbox("显示阈值标记")]
        [Order(30)]
        public bool ShowMarker = false;

        [DragInt("阈值标记大小", min = 0, max = 10000)]
        [Order(35, collapseWith = nameof(ShowMarker))]
        public int MarkerSize = 2;

        [ColorEdit4("阈值标记颜色")]
        [Order(40, collapseWith = nameof(ShowMarker))]
        public PluginConfigColor MarkerColor = new PluginConfigColor(new Vector4(0f / 255f, 0f / 255f, 0f / 255f, 100f / 100f));

        public bool IsActive(float current)
        {
            return Enabled && (ThresholdType == ThresholdType.Below && current < Value ||
                               ThresholdType == ThresholdType.Above && current > Value);
        }

        public ThresholdConfig()
        {
            Enabled = false;
        }
    }

    public enum ThresholdType
    {
        Above,
        Below
    }
}
