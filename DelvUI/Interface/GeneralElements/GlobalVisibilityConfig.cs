using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Helpers;
using ImGuiNET;
using Newtonsoft.Json;
using System.Numerics;

namespace DelvUI.Interface.GeneralElements
{
    [Disableable(false)]
    [Exportable(false)]
    [Section("可见性")]
    [SubSection("全局", 0)]
    public class GlobalVisibilityConfig : PluginConfigObject
    {
        public new static GlobalVisibilityConfig DefaultConfig() { return new GlobalVisibilityConfig(); }

        [NestedConfig("可见性", 50, collapsingHeader = false)]
        public VisibilityConfig VisibilityConfig = new VisibilityConfig();

        [JsonIgnore]
        private bool _applying = false;

        [ManualDraw]
        public bool Draw(ref bool changed)
        {
            ImGui.NewLine();

            if (ImGui.Button("应用于所有元素", new Vector2(200, 30)))
            {
                _applying = true;
            }

            if (_applying)
            {
                string[] lines = new string[] { "这将取代可见性设置", "对于 所有 DelvUI元素！", "你确定吗？" };
                var (didConfirm, didClose) = ImGuiHelper.DrawConfirmationModal("应用？", lines);

                if (didConfirm)
                {
                    ConfigurationManager.Instance.OnGlobalVisibilityChanged(VisibilityConfig);
                    changed = true;
                }

                if (didConfirm || didClose)
                {
                    _applying = false;
                }
            }

            return false;
        }
    }
}
