using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Enums;
using DelvUI.Interface.Bars;
using System.Numerics;

namespace DelvUI.Interface.GeneralElements
{
    [DisableParentSettings("Size")]
    [Section("其他元素")]
    [SubSection("GCD指示器", 0)]
    public class GCDIndicatorConfig : AnchorablePluginConfigObject
    {
        [Checkbox("总是显示")]
        [Order(3)]
        public bool AlwaysShow = false;

        [Checkbox("锚定到鼠标")]
        [Order(4)]
        public bool AnchorToMouse = false;

        [ColorEdit4("背景色")]
        [Order(16)]
        public PluginConfigColor BackgroundColor = new PluginConfigColor(new Vector4(0f / 255f, 0f / 255f, 0f / 255f, 50f / 100f));

        [ColorEdit4("颜色")]
        [Order(17)]
        public PluginConfigColor FillColor = new PluginConfigColor(new(220f / 255f, 220f / 255f, 220f / 255f, 100f / 100f));

        [Checkbox("显示边界")]
        [Order(18)]
        public bool ShowBorder = true;

        [Checkbox("只提供即时GCD", spacing = true)]
        [Order(19)]
        public bool InstantGCDsOnly = false;

        [Checkbox("只有在GCD阈值以下显示", spacing = true)]
        [Order(20)]
        public bool LimitGCDThreshold = false;

        [DragFloat("GCD阈值", velocity = 0.01f)]
        [Order(21, collapseWith = nameof(LimitGCDThreshold))]
        public float GCDThreshold = 1.50f;

        [Checkbox("显示GCD队列指示灯", spacing = true)]
        [Order(24)]
        public bool ShowGCDQueueIndicator = true;

        [ColorEdit4("GCD队列颜色")]
        [Order(25, collapseWith = nameof(ShowGCDQueueIndicator))]
        public PluginConfigColor QueueColor = new PluginConfigColor(new(13f / 255f, 207f / 255f, 31f / 255f, 100f / 100f));

        [Checkbox("循环模式", spacing = true)]
        [Order(30)]
        public bool CircularMode = false;

        [DragInt("半径")]
        [Order(35, collapseWith = nameof(CircularMode))]
        public int CircleRadius = 40;

        [DragInt("厚度")]
        [Order(40, collapseWith = nameof(CircularMode))]
        public int CircleThickness = 10;

        [DragInt("起始角度", min = 0, max = 359)]
        [Order(45, collapseWith = nameof(CircularMode))]
        public int CircleStartAngle = 0;

        [Checkbox("逆时针旋转")]
        [Order(50, collapseWith = nameof(CircularMode))]
        public bool RotateCCW = false;

        [NestedConfig("条模式", 45, collapsingHeader = false)]
        public GCDBarConfig Bar = new GCDBarConfig(
            new Vector2(0, HUDConstants.BaseHUDOffsetY + 21),
            new Vector2(254, 8),
            new PluginConfigColor(Vector4.Zero)
        );

        [NestedConfig("可见性", 70)]
        public VisibilityConfig VisibilityConfig = new VisibilityConfig();

        public new static GCDIndicatorConfig DefaultConfig() { return new GCDIndicatorConfig() { Enabled = false, Strata = StrataLevel.MID_HIGH }; }
    }

    [DisableParentSettings("Position", "Anchor", "HideWhenInactive", "FillColor", "BackgroundColor", "DrawBorder")]
    [Exportable(false)]
    public class GCDBarConfig : BarConfig
    {
        public GCDBarConfig(Vector2 position, Vector2 size, PluginConfigColor fillColor, BarDirection fillDirection = BarDirection.Right)
            : base(position, size, fillColor, fillDirection)
        {
        }
    }
}
