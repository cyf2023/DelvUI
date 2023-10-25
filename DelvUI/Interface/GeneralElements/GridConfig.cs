using DelvUI.Config;
using DelvUI.Config.Attributes;

namespace DelvUI.Interface.GeneralElements
{
    [Exportable(false)]
    [Section("杂项")]
    [SubSection("网格", 0)]
    public class GridConfig : PluginConfigObject
    {
        public new static GridConfig DefaultConfig()
        {
            var config = new GridConfig();
            config.Enabled = false;

            return config;
        }

        [DragFloat("背景不透明度", min = 0, max = 1, velocity = .05f)]
        [Order(10)]
        public float BackgroundAlpha = 0.3f;

        [Checkbox("显示中心线")]
        [Order(15)]
        public bool ShowCenterLines = true;
        [Checkbox("显示锚点")]
        [Order(20)]

        public bool ShowAnchorPoints = true;
        [Checkbox("显示网格", spacing = true)]
        [Order(25)]
        public bool ShowGrid = true;

        [DragInt("大网格间距", min = 50, max = 500)]
        [Order(30, collapseWith = nameof(ShowGrid))]
        public int GridDivisionsDistance = 50;

        [DragInt("小网格间距", min = 1, max = 10)]
        [Order(35, collapseWith = nameof(ShowGrid))]
        public int GridSubdivisionCount = 4;
    }
}
