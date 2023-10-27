using DelvUI.Config;
using DelvUI.Config.Attributes;
using System.Numerics;
using DelvUI.Interface.GeneralElements;
using DelvUI.Enums;

namespace DelvUI.Interface.Bars
{
    [Exportable(false)]
    public class BarConfig : AnchorablePluginConfigObject
    {
        [ColorEdit4("背景色")]
        [Order(16)]
        public PluginConfigColor BackgroundColor = new PluginConfigColor(new Vector4(0f / 255f, 0f / 255f, 0f / 255f, 50f / 100f));

        [ColorEdit4("填充颜色")]
        [Order(25)]
        public PluginConfigColor FillColor;

        [Combo("填充方向", new string[] { "左", "右", "上", "下" })]
        [Order(30)]
        public BarDirection FillDirection;

        [BarTexture("条的纹理", spacing = true, help = "默认情况下，条将使用在 颜色>杂项 中找到的条的全局梯度设置绘制。")]
        [Order(31)]
        public string BarTextureName = "";

        [BarTextureDrawMode("绘制模式")]
        [Order(32)]
        public BarTextureDrawMode BarTextureDrawMode = BarTextureDrawMode.Stretch;

        [Checkbox("显示边框", spacing = true)]
        [Order(35)]
        public bool DrawBorder = true;

        [ColorEdit4("边框颜色")]
        [Order(36, collapseWith = nameof(DrawBorder))]
        public PluginConfigColor BorderColor = new PluginConfigColor(new Vector4(0f / 255f, 0f / 255f, 0f / 255f, 100f / 100f));

        [DragInt("边框厚度", min = 1, max = 10)]
        [Order(37, collapseWith = nameof(DrawBorder))]
        public int BorderThickness = 1;

        [NestedConfig("阴影", 40, spacing = true)]
        public ShadowConfig ShadowConfig = new ShadowConfig() { Enabled = false };

        [Checkbox("非活跃时隐藏", spacing = true)]
        [Order(41)]
        public bool HideWhenInactive = false;

        public BarConfig(Vector2 position, Vector2 size, PluginConfigColor fillColor, BarDirection fillDirection = BarDirection.Right)
        {
            Position = position;
            Size = size;
            FillColor = fillColor;
            FillDirection = fillDirection;
        }
    }

    [Exportable(false)]
    public class BarGlowConfig : PluginConfigObject
    {
        [ColorEdit4("颜色")]
        [Order(5)]
        public PluginConfigColor Color = new PluginConfigColor(new Vector4(255f / 255f, 255f / 255f, 255f / 255f, 50f / 100f));

        [DragInt("尺寸", min = 1, max = 100)]
        [Order(25)]
        public int Size = 1;
    }

    public enum BarDirection
    {
        Left,
        Right,
        Up,
        Down
    }
}
