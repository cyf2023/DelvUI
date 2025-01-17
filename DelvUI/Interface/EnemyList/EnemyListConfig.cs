﻿using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Enums;
using DelvUI.Helpers;
using DelvUI.Interface.Bars;
using DelvUI.Interface.GeneralElements;
using DelvUI.Interface.StatusEffects;
using ImGuiNET;
using System.Numerics;

namespace DelvUI.Interface.EnemyList
{
    public enum EnemyListGrowthDirection
    {
        Down = 0,
        Up
    }

    [Exportable(false)]
    [Section("敌对列表", true)]
    [SubSection("通用", 0)]
    public class EnemyListConfig : MovablePluginConfigObject
    {
        public new static EnemyListConfig DefaultConfig()
        {
            var config = new EnemyListConfig();
            Vector2 screenSize = ImGui.GetMainViewport().Size;
            config.Position = new Vector2(screenSize.X * 0.2f, -screenSize.Y * 0.2f);

            return config;
        }

        [Checkbox("预览", isMonitored = true)]
        [Order(4)]
        public bool Preview = false;

        [Combo("发展方向", "下", "上", spacing = true)]
        [Order(20)]
        public EnemyListGrowthDirection GrowthDirection = EnemyListGrowthDirection.Down;

        [DragInt("垂直填充", min = 0, max = 500)]
        [Order(25)]
        public int VerticalPadding = 10;
    }

    [Exportable(false)]
    [DisableParentSettings("Position", "Anchor", "HideWhenInactive")]
    [Section("敌对列表", true)]
    [SubSection("生命条", 0)]
    public class EnemyListHealthBarConfig : BarConfig
    {
        [NestedConfig("名字标签", 70)]
        public EditableLabelConfig NameLabel = new EditableLabelConfig(new Vector2(-5, 12), "[name]", DrawAnchor.TopRight, DrawAnchor.BottomRight);

        [NestedConfig("血量标签", 80)]
        public EditableLabelConfig HealthLabel = new EditableLabelConfig(new Vector2(30, 0), "[health:percent]%", DrawAnchor.Left, DrawAnchor.Left);

        [NestedConfig("目标标签", 90)]
        public DefaultFontLabelConfig OrderLabel = new DefaultFontLabelConfig(new Vector2(5, 0), "", DrawAnchor.Left, DrawAnchor.Left);

        [NestedConfig("颜色", 100)]
        public EnemyListHealthBarColorsConfig Colors = new EnemyListHealthBarColorsConfig();

        [NestedConfig("根据范围改变不透明度", 110)]
        public EnemyListRangeConfig RangeConfig = new EnemyListRangeConfig();

        [NestedConfig("使用平滑过渡", 120)]
        public SmoothHealthConfig SmoothHealthConfig = new SmoothHealthConfig();

        [NestedConfig("自定义鼠标悬停区域", 130)]
        public MouseoverAreaConfig MouseoverAreaConfig = new MouseoverAreaConfig();

        public new static EnemyListHealthBarConfig DefaultConfig()
        {
            Vector2 size = new Vector2(180, 40);

            var config = new EnemyListHealthBarConfig(Vector2.Zero, size, new PluginConfigColor(new(233f / 255f, 4f / 255f, 4f / 255f, 100f / 100f)));
            config.Colors.ColorByHealth.Enabled = false;

            config.NameLabel.FontID = FontsConfig.DefaultMediumFontKey;
            config.HealthLabel.FontID = FontsConfig.DefaultMediumFontKey;

            config.MouseoverAreaConfig.Enabled = false;

            return config;
        }

        public EnemyListHealthBarConfig(Vector2 position, Vector2 size, PluginConfigColor fillColor, BarDirection fillDirection = BarDirection.Right)
            : base(position, size, fillColor, fillDirection)
        {
        }
    }

    [Disableable(false)]
    [Exportable(false)]
    public class EnemyListHealthBarColorsConfig : PluginConfigObject
    {
        [NestedConfig("基于血量的颜色", 30, collapsingHeader = false)]
        public ColorByHealthValueConfig ColorByHealth = new ColorByHealthValueConfig();

        [Checkbox("用光标悬停或软选中时高亮显示", spacing = true)]
        [Order(40)]
        public bool ShowHighlight = true;

        [ColorEdit4("高亮颜色")]
        [Order(41, collapseWith = nameof(ShowHighlight))]
        public PluginConfigColor HighlightColor = new PluginConfigColor(new Vector4(255f / 255f, 255f / 255f, 255f / 255f, 5f / 100f));

        [Checkbox("缺失生命值颜色", spacing = true)]
        [Order(45)]
        public bool UseMissingHealthBar = false;

        [ColorEdit4("Color" + "##MissingHealth")]
        [Order(46, collapseWith = nameof(UseMissingHealthBar))]
        public PluginConfigColor HealthMissingColor = new PluginConfigColor(new Vector4(255f / 255f, 0f / 255f, 0f / 255f, 100f / 100f));

        [ColorEdit4("目标边框颜色", spacing = true)]
        [Order(50)]
        public PluginConfigColor TargetBordercolor = new PluginConfigColor(new Vector4(255f / 255f, 255f / 255f, 255f / 255f, 100f / 100f));

        [DragInt("目标边框厚度", min = 1, max = 10)]
        [Order(51)]
        public int TargetBorderThickness = 1;

        [Checkbox("显示仇恨边框颜色", spacing = true)]
        [Order(60)]
        public bool ShowEnmityBorderColors = true;

        [ColorEdit4("一仇颜色")]
        [Order(61, collapseWith = nameof(ShowEnmityBorderColors))]
        public PluginConfigColor EnmityLeaderBorderColor = new PluginConfigColor(new Vector4(255f / 255f, 40f / 255f, 40f / 255f, 100f / 100f));

        [ColorEdit4("接近一仇颜色")]
        [Order(62, collapseWith = nameof(ShowEnmityBorderColors))]
        public PluginConfigColor EnmitySecondBorderColor = new PluginConfigColor(new Vector4(255f / 255f, 175f / 255f, 40f / 255f, 100f / 100f));
    }

    [Exportable(false)]
    public class EnemyListRangeConfig : PluginConfigObject
    {
        [DragInt("距离(yalms)", min = 1, max = 500)]
        [Order(5)]
        public int Range = 30;

        [DragFloat("透明度", min = 1, max = 100)]
        [Order(10)]
        public float Alpha = 25;


        public float AlphaForDistance(int distance, float alpha = 100f)
        {
            if (!Enabled)
            {
                return 100f;
            }

            return distance > Range ? Alpha : alpha;
        }
    }

    [DisableParentSettings("FrameAnchor")]
    [Exportable(false)]
    [Section("敌对列表", true)]
    [SubSection("仇恨图标", 0)]
    public class EnemyListEnmityIconConfig : IconConfig
    {
        [Anchor("生命条锚")]
        [Order(16)]
        public DrawAnchor HealthBarAnchor = DrawAnchor.TopLeft;

        public new static EnemyListEnmityIconConfig DefaultConfig() =>
            new EnemyListEnmityIconConfig(new Vector2(5), new Vector2(24), DrawAnchor.Center, DrawAnchor.TopLeft);

        public EnemyListEnmityIconConfig(Vector2 position, Vector2 size, DrawAnchor anchor, DrawAnchor frameAnchor)
            : base(position, size, anchor, frameAnchor)
        {
            HealthBarAnchor = frameAnchor;
        }
    }

    [DisableParentSettings("FrameAnchor")]
    [Exportable(false)]
    [Section("敌对列表", true)]
    [SubSection("标记图标", 0)]
    public class EnemyListSignIconConfig : SignIconConfig
    {
        [Anchor("生命条锚")]
        [Order(16)]
        public DrawAnchor HealthBarAnchor = DrawAnchor.TopLeft;

        [Checkbox("替换目标标签", help = "当启用时，如果敌人分配了一个标记，将绘制标记图标而不是次序标签。")]
        [Order(30)]
        public bool ReplaceOrderLabel = true;

        public new static EnemyListSignIconConfig DefaultConfig() =>
            new EnemyListSignIconConfig(new Vector2(0), new Vector2(30), DrawAnchor.Center, DrawAnchor.Left);

        public EnemyListSignIconConfig(Vector2 position, Vector2 size, DrawAnchor anchor, DrawAnchor frameAnchor)
            : base(position, size, anchor, frameAnchor)
        {
            HealthBarAnchor = frameAnchor;
        }
    }

    [DisableParentSettings("AnchorToUnitFrame", "UnitFrameAnchor", "HideWhenInactive", "FillDirection")]
    [Exportable(false)]
    [Section("敌人列表", true)]
    [SubSection("咏唱栏", 0)]
    public class EnemyListCastbarConfig : TargetCastbarConfig
    {
        public new static EnemyListCastbarConfig DefaultConfig()
        {
            var size = new Vector2(180, 10);

            var castNameConfig = new LabelConfig(new Vector2(0, 0), "", DrawAnchor.Center, DrawAnchor.Center);
            castNameConfig.FontID = FontsConfig.DefaultMediumFontKey;
            var castTimeConfig = new NumericLabelConfig(new Vector2(-5, 0), "", DrawAnchor.Right, DrawAnchor.Right);
            castTimeConfig.Enabled = false;
            castTimeConfig.FontID = FontsConfig.DefaultMediumFontKey;
            castTimeConfig.NumberFormat = 1;

            var config = new EnemyListCastbarConfig(Vector2.Zero, size, castNameConfig, castTimeConfig);
            config.HealthBarAnchor = DrawAnchor.Bottom;
            config.Anchor = DrawAnchor.Bottom;
            config.ShowIcon = false;

            return config;
        }

        [Anchor("生命条锚")]
        [Order(16)]
        public DrawAnchor HealthBarAnchor = DrawAnchor.BottomLeft;

        public EnemyListCastbarConfig(Vector2 position, Vector2 size, LabelConfig castNameConfig, NumericLabelConfig castTimeConfig)
            : base(position, size, castNameConfig, castTimeConfig)
        {

        }
    }

    [Exportable(false)]
    [Section("敌人列表", true)]
    [SubSection("增益效果", 0)]
    public class EnemyListBuffsConfig : EnemyListStatusEffectsListConfig
    {
        public new static EnemyListBuffsConfig DefaultConfig()
        {
            var durationConfig = new LabelConfig(new Vector2(0, -4), "", DrawAnchor.Bottom, DrawAnchor.Center);
            var stacksConfig = new LabelConfig(new Vector2(-3, 4), "", DrawAnchor.TopRight, DrawAnchor.Center);
            stacksConfig.Color = new(Vector4.UnitW);
            stacksConfig.OutlineColor = new(Vector4.One);

            var iconConfig = new StatusEffectIconConfig(durationConfig, stacksConfig);
            iconConfig.DispellableBorderConfig.Enabled = false;
            iconConfig.Size = new Vector2(24, 24);

            var pos = new Vector2(5, 8);
            var size = new Vector2(iconConfig.Size.X * 4 + 6, iconConfig.Size.Y);

            var config = new EnemyListBuffsConfig(DrawAnchor.TopRight, pos, size, true, false, false, GrowthDirections.Right | GrowthDirections.Down, iconConfig);
            config.Limit = 4;
            config.ShowPermanentEffects = true;
            config.IconConfig.DispellableBorderConfig.Enabled = false;

            return config;
        }

        public EnemyListBuffsConfig(DrawAnchor anchor, Vector2 position, Vector2 size, bool showBuffs, bool showDebuffs, bool showPermanentEffects,
            GrowthDirections growthDirections, StatusEffectIconConfig iconConfig)
            : base(anchor, position, size, showBuffs, showDebuffs, showPermanentEffects, growthDirections, iconConfig)
        {
        }
    }

    [Exportable(false)]
    [Section("敌人列表", true)]
    [SubSection("减益效果", 0)]
    public class EnemyListDebuffsConfig : EnemyListStatusEffectsListConfig
    {
        public new static EnemyListDebuffsConfig DefaultConfig()
        {
            var durationConfig = new LabelConfig(new Vector2(0, -4), "", DrawAnchor.Bottom, DrawAnchor.Center);
            var stacksConfig = new LabelConfig(new Vector2(-3, 4), "", DrawAnchor.TopRight, DrawAnchor.Center);
            stacksConfig.Color = new(Vector4.UnitW);
            stacksConfig.OutlineColor = new(Vector4.One);

            var iconConfig = new StatusEffectIconConfig(durationConfig, stacksConfig);
            iconConfig.Size = new Vector2(24, 24);

            var pos = new Vector2(-5, 8);
            var size = new Vector2(iconConfig.Size.X * 4 + 6, iconConfig.Size.Y);

            var config = new EnemyListDebuffsConfig(DrawAnchor.TopLeft, pos, size, false, true, false, GrowthDirections.Left | GrowthDirections.Down, iconConfig);
            config.Limit = 4;
            config.ShowPermanentEffects = true;
            config.IconConfig.DispellableBorderConfig.Enabled = false;

            return config;
        }

        public EnemyListDebuffsConfig(DrawAnchor anchor, Vector2 position, Vector2 size, bool showBuffs, bool showDebuffs, bool showPermanentEffects,
            GrowthDirections growthDirections, StatusEffectIconConfig iconConfig)
            : base(anchor, position, size, showBuffs, showDebuffs, showPermanentEffects, growthDirections, iconConfig)
        {
        }
    }

    public class EnemyListStatusEffectsListConfig : StatusEffectsListConfig
    {
        [Anchor("生命条锚")]
        [Order(4)]
        public DrawAnchor HealthBarAnchor = DrawAnchor.BottomLeft;

        public EnemyListStatusEffectsListConfig(DrawAnchor anchor, Vector2 position, Vector2 size, bool showBuffs, bool showDebuffs, bool showPermanentEffects,
            GrowthDirections growthDirections, StatusEffectIconConfig iconConfig)
            : base(position, size, showBuffs, showDebuffs, showPermanentEffects, growthDirections, iconConfig)
        {
            HealthBarAnchor = anchor;
        }
    }
}
