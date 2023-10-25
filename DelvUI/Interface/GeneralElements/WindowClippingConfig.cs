using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Helpers;
using ImGuiNET;

namespace DelvUI.Interface.GeneralElements
{
    public enum WindowClippingMode
    {
        Full,
        Hide,
        Performance
    }

    [Exportable(false)]
    [Disableable(false)]
    [Section("杂项")]
    [SubSection("窗口剪切", 0)]
    public class WindowClippingConfig : PluginConfigObject
    {
        public new static WindowClippingConfig DefaultConfig() => new WindowClippingConfig();

        public WindowClippingMode Mode = WindowClippingMode.Full;

        public bool NameplatesClipRectsEnabled = true;
        public bool TargetCastbarClipRectEnabled = false;
        public bool HotbarsClipRectsEnabled = false;
        public bool ChatBubblesClipRectsEnabled = true;

        private bool _showConfirmationDialog = false;

        [ManualDraw]
        public bool Draw(ref bool changed)
        {
            ImGuiHelper.NewLineAndTab();

            if (ImGui.Checkbox("启用", ref Enabled))
            {
                if (Enabled)
                {
                    Enabled = false;
                    _showConfirmationDialog = true;
                }
                else
                {
                    changed = true;
                }
            }

            // confirmation dialog
            if (_showConfirmationDialog)
            {
                string[] lines = new string[] { "众所周知，这个特性会导致随机", "导致一小部分用户崩溃！！", "你确定要启用它吗?" };
                var (didConfirm, didClose) = ImGuiHelper.DrawConfirmationModal("警告！", lines);

                if (didConfirm)
                {
                    Enabled = true;
                    changed = true;
                }

                if (didConfirm || didClose)
                {
                    _showConfirmationDialog = false;
                }
            }

            if (!Enabled) { return changed; }

            // mode
            ImGuiHelper.NewLineAndTab();
            ImGui.SameLine();
            ImGui.Text("模式：");

            ImGui.SameLine();
            if (ImGui.RadioButton("充满", Mode == WindowClippingMode.Full))
            {
                Mode = WindowClippingMode.Full;
            }

            ImGui.SameLine();
            if (ImGui.RadioButton("隐藏", Mode == WindowClippingMode.Hide))
            {
                Mode = WindowClippingMode.Hide;
            }

            ImGui.SameLine();
            if (ImGui.RadioButton("卓越", Mode == WindowClippingMode.Performance))
            {
                Mode = WindowClippingMode.Performance;
            }

            // nameplates
            ImGui.NewLine();
            ImGuiHelper.NewLineAndTab();
            changed |= ImGui.Checkbox("为名牌启用特殊剪切", ref NameplatesClipRectsEnabled);
            ImGuiHelper.SetTooltip("当启用时，名牌将被游戏UI元素覆盖，不会像通常那样覆盖DelvUI元素。");

            if (NameplatesClipRectsEnabled)
            {
                ImGuiHelper.Tab(); ImGuiHelper.Tab();
                changed |= ImGui.Checkbox("默认目标咏唱栏", ref TargetCastbarClipRectEnabled);
                ImGuiHelper.SetTooltip("当启用时，游戏的目标咏唱栏将不会被DelvUI名牌覆盖。\n专门给那些喜欢使用默认目标咏唱栏而不是DelvUI的玩家。");

                ImGuiHelper.Tab(); ImGuiHelper.Tab();
                changed |= ImGui.Checkbox("热键栏", ref HotbarsClipRectsEnabled);
                ImGuiHelper.SetTooltip("启用后，启用的热键栏将不会被DelvUI名牌覆盖。\n请注意，这种计算方式并不完美，它可能在有空槽的热键栏上不生效。");

                ImGuiHelper.Tab(); ImGuiHelper.Tab();
                changed |= ImGui.Checkbox("聊天气泡", ref ChatBubblesClipRectsEnabled);
            }

            // text
            ImGui.NewLine();
            ImGuiHelper.NewLineAndTab();
            ImGui.SameLine();

            switch (Mode)
            {
                case WindowClippingMode.Full:
                    ImGui.Text("在这个模式DelvUI将尝试窗口剪切来不覆盖游戏窗口。");
                    break;

                case WindowClippingMode.Hide:
                    ImGui.Text("在这个模式DelvUI将尝试不绘制触及游戏窗口的元素来不覆盖游戏窗口。");
                    break;

                case WindowClippingMode.Performance:
                    ImGui.Text("为了提高性能，窗口裁剪功能将被减少。\n同一时间只能裁剪一个游戏窗口。这可能会产生意想不到的/丑陋的结果。\n\n注意：此模式不适用于名牌。");
                    break;
            }

            ImGuiHelper.NewLineAndTab();
            ImGui.Text("如果您遇到随机崩溃或性能不佳，我们建议您尝试不同的模式\n或者完全禁用窗口剪切");

            return false;
        }
    }
}
