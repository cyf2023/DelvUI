using DelvUI.Config;
using DelvUI.Config.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DelvUI.Interface.GeneralElements
{
    [Disableable(false)]
    [Section("杂项")]
    [SubSection("HUD设置", 0)]
    public class HUDOptionsConfig : PluginConfigObject
    {
        [Checkbox("全局HUD位置")]
        [Order(5)]
        public bool UseGlobalHudShift = false;

        [DragInt2("位置", min = -4000, max = 4000)]
        [Order(6, collapseWith = nameof(UseGlobalHudShift))]
        public Vector2 HudOffset = new(0, 0);

        [Checkbox("当不聚焦时，调暗DelvUI的设置窗口")]
        [Order(10)]
        public bool DimConfigWindow = false;

        [Checkbox("自动禁用HUD元素预览", help = "如果启用，当DelvUI的设置窗口关闭时，禁用所有HUD元素预览模式。")]
        [Order(11)]
        public bool AutomaticPreviewDisabling = true;

        [Checkbox("使用DelvUI样式", help = "如果启用，DelvUI将使用自己的风格设置窗口，而不是一般的Dalamud风格。")]
        [Order(12)]
        public bool OverrideDalamudStyle = true;

        [Checkbox("鼠标悬停", separator = true)]
        [Order(15)]
        public bool MouseoverEnabled = true;

        [Checkbox("自动模式", help =
            "启用时：当你的光标在一个单元框架上时，你的所有动作都将自动被认为是悬停。\n" +
            "鼠标悬停宏或其他鼠标悬停插件是不必要的，且不会在此模式下工作！\n\n" +
            "禁用时：DelvUI单元框架将表现得像游戏中的一样。\n" +
            "在这种模式下，你需要使用鼠标悬停宏或其他鼠标悬停相关插件。")]
        [Order(16, collapseWith = nameof(MouseoverEnabled))]
        public bool MouseoverAutomaticMode = true;

        [Checkbox("隐藏默认职业量谱", isMonitored = true, separator = true)]
        [Order(40)]
        public bool HideDefaultJobGauges = false;

        [Checkbox("隐藏默认咏唱栏", isMonitored = true)]
        [Order(45)]
        public bool HideDefaultCastbar = false;

        [Checkbox("隐藏默认倒计时", isMonitored = true)]
        [Order(50)]
        public bool HideDefaultPulltimer = false;

        [Checkbox("使用地区数字格式", help = "启用后，DelvUI将在显示数字时使用你的系统的区域格式设置。\n当禁用时，DelvUI将使用英文数字格式。", separator = true)]
        [Order(60)]
        public bool UseRegionalNumberFormats = true;

        // saves original positions for all 4 layouts
        public Vector2[] CastBarOriginalPositions = new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        public Vector2[] PulltimerOriginalPositions = new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
        public Dictionary<string, Vector2>[] JobGaugeOriginalPositions = new Dictionary<string, Vector2>[] { new(), new(), new(), new() };

        public new static HUDOptionsConfig DefaultConfig() => new();
    }

    public class HUDOptionsConfigConverter : PluginConfigObjectConverter
    {
        public HUDOptionsConfigConverter()
        {
            Func<Vector2, Vector2[]> func = (value) =>
            {
                Vector2[] array = new Vector2[4];
                for (int i = 0; i < 4; i++)
                {
                    array[i] = value;
                }

                return array;
            };

            TypeToClassFieldConverter<Vector2, Vector2[]> castBar = new TypeToClassFieldConverter<Vector2, Vector2[]>(
                "CastBarOriginalPositions",
                new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero },
                func
            );

            TypeToClassFieldConverter<Vector2, Vector2[]> pullTimer = new TypeToClassFieldConverter<Vector2, Vector2[]>(
                "PulltimerOriginalPositions",
                new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero },
                func
            );

            NewClassFieldConverter<Dictionary<string, Vector2>, Dictionary<string, Vector2>[]> jobGauge =
                new NewClassFieldConverter<Dictionary<string, Vector2>, Dictionary<string, Vector2>[]>(
                    "JobGaugeOriginalPositions",
                    new Dictionary<string, Vector2>[] { new(), new(), new(), new() },
                    (oldValue) =>
                    {
                        Dictionary<string, Vector2>[] array = new Dictionary<string, Vector2>[4];
                        for (int i = 0; i < 4; i++)
                        {
                            array[i] = oldValue;
                        }

                        return array;
                    });

            FieldConvertersMap.Add("CastBarOriginalPosition", castBar);
            FieldConvertersMap.Add("PulltimerOriginalPosition", pullTimer);
            FieldConvertersMap.Add("JobGaugeOriginalPosition", jobGauge);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HUDOptionsConfig);
        }
    }
}
