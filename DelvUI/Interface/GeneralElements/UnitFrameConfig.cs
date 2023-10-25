using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Enums;
using DelvUI.Helpers;
using DelvUI.Interface.Bars;
using System.Numerics;

namespace DelvUI.Interface.GeneralElements
{
    [DisableParentSettings("HideWhenInactive", "HideHealthIfPossible", "RangeConfig", "EnemyRangeConfig")]
    [Section("单元框架")]
    [SubSection("玩家", 0)]
    public class PlayerUnitFrameConfig : UnitFrameConfig
    {
        [NestedConfig("盾姿指示器", 122, spacing = true)]
        public TankStanceIndicatorConfig TankStanceIndicatorConfig = new TankStanceIndicatorConfig();

        public PlayerUnitFrameConfig(Vector2 position, Vector2 size, EditableLabelConfig leftLabelConfig, EditableLabelConfig rightLabelConfig, EditableLabelConfig optionalLabelConfig)
            : base(position, size, leftLabelConfig, rightLabelConfig, optionalLabelConfig)
        {
        }

        public new static PlayerUnitFrameConfig DefaultConfig()
        {
            var size = HUDConstants.DefaultBigUnitFrameSize;
            var pos = new Vector2(-HUDConstants.UnitFramesOffsetX - size.X / 2f, HUDConstants.BaseHUDOffsetY);

            var leftLabelConfig = new EditableLabelConfig(new Vector2(5, 0), "[name]", DrawAnchor.TopLeft, DrawAnchor.BottomLeft);
            var rightLabelConfig = new EditableLabelConfig(new Vector2(-5, 0), "[health:current-short] | [health:percent]", DrawAnchor.TopRight, DrawAnchor.BottomRight);
            var optionalLabelConfig = new EditableLabelConfig(new Vector2(0, 0), "", DrawAnchor.Center, DrawAnchor.Center);

            var config = new PlayerUnitFrameConfig(pos, size, leftLabelConfig, rightLabelConfig, optionalLabelConfig);

            return config;
        }
    }

    public enum TankStanceCorner
    {
        TopLeft = 0,
        TopRight,
        BottomLeft,
        BottomRight
    }

    [Exportable(false)]
    public class TankStanceIndicatorConfig : PluginConfigObject
    {
        [Combo("角落", "上左", "上右", "下左", "下右")]
        [Order(5)]
        public TankStanceCorner Corner = TankStanceCorner.BottomLeft;

        [DragFloat2("大小", min = 1, max = 500)]
        [Order(10)]
        public Vector2 Size = new Vector2(HUDConstants.DefaultBigUnitFrameSize.Y - 20, HUDConstants.DefaultBigUnitFrameSize.Y - 20);

        [DragInt("厚度", min = 2, max = 20)]
        [Order(15)]
        public int Thickess = 4;

        [ColorEdit4("开启颜色")]
        [Order(20)]
        public PluginConfigColor ActiveColor = new PluginConfigColor(new Vector4(0f / 255f, 255f / 255f, 255f / 255f, 100f / 100f));

        [ColorEdit4("关闭颜色")]
        [Order(25)]
        public PluginConfigColor InactiveColor = new PluginConfigColor(new Vector4(255f / 255f, 0f / 255f, 0f / 255f, 100f / 100f));
    }

    [DisableParentSettings("HideWhenInactive")]
    [Section("单元框架")]
    [SubSection("目标", 0)]
    public class TargetUnitFrameConfig : UnitFrameConfig
    {
        public TargetUnitFrameConfig(Vector2 position, Vector2 size, EditableLabelConfig leftLabelConfig, EditableLabelConfig rightLabelConfig, EditableLabelConfig optionalLabelConfig)
            : base(position, size, leftLabelConfig, rightLabelConfig, optionalLabelConfig)
        {
        }

        public new static TargetUnitFrameConfig DefaultConfig()
        {
            var size = HUDConstants.DefaultBigUnitFrameSize;
            var pos = new Vector2(HUDConstants.UnitFramesOffsetX + size.X / 2f, HUDConstants.BaseHUDOffsetY);

            var leftLabelConfig = new EditableLabelConfig(new Vector2(5, 0), "[health:current-short] | [health:percent]", DrawAnchor.TopLeft, DrawAnchor.BottomLeft);
            var rightLabelConfig = new EditableLabelConfig(new Vector2(-5, 0), "[name]", DrawAnchor.TopRight, DrawAnchor.BottomRight);
            var optionalLabelConfig = new EditableLabelConfig(new Vector2(0, 0), "", DrawAnchor.Center, DrawAnchor.Center);

            return new TargetUnitFrameConfig(pos, size, leftLabelConfig, rightLabelConfig, optionalLabelConfig);
        }
    }

    [DisableParentSettings("HideWhenInactive")]
    [Section("单元框架")]
    [SubSection("目标的目标", 0)]
    public class TargetOfTargetUnitFrameConfig : UnitFrameConfig
    {
        public TargetOfTargetUnitFrameConfig(Vector2 position, Vector2 size, EditableLabelConfig leftLabelConfig, EditableLabelConfig rightLabelConfig, EditableLabelConfig optionalLabelConfig)
            : base(position, size, leftLabelConfig, rightLabelConfig, optionalLabelConfig)
        {
        }

        public new static TargetOfTargetUnitFrameConfig DefaultConfig()
        {
            var size = HUDConstants.DefaultSmallUnitFrameSize;
            var pos = new Vector2(
                HUDConstants.UnitFramesOffsetX + HUDConstants.DefaultBigUnitFrameSize.X + 6 + size.X / 2f,
                HUDConstants.BaseHUDOffsetY - 15
            );

            var leftLabelConfig = new EditableLabelConfig(new Vector2(0, 0), "[name]", DrawAnchor.Top, DrawAnchor.Bottom);
            var rightLabelConfig = new EditableLabelConfig(new Vector2(0, 0), "", DrawAnchor.Center, DrawAnchor.TopLeft);
            var optionalLabelConfig = new EditableLabelConfig(new Vector2(0, 0), "", DrawAnchor.Center, DrawAnchor.BottomLeft);

            return new TargetOfTargetUnitFrameConfig(pos, size, leftLabelConfig, rightLabelConfig, optionalLabelConfig);
        }
    }

    [DisableParentSettings("HideWhenInactive")]
    [Section("单元框架")]
    [SubSection("焦点目标", 0)]
    public class FocusTargetUnitFrameConfig : UnitFrameConfig
    {
        public FocusTargetUnitFrameConfig(Vector2 position, Vector2 size, EditableLabelConfig leftLabelConfig, EditableLabelConfig rightLabelConfig, EditableLabelConfig optionalLabelConfig)
            : base(position, size, leftLabelConfig, rightLabelConfig, optionalLabelConfig)
        {
        }

        public new static FocusTargetUnitFrameConfig DefaultConfig()
        {
            var size = HUDConstants.DefaultSmallUnitFrameSize;
            var pos = new Vector2(
                -HUDConstants.UnitFramesOffsetX - HUDConstants.DefaultBigUnitFrameSize.X - 6 - size.X / 2f,
                HUDConstants.BaseHUDOffsetY - 15
            );

            var leftLabelConfig = new EditableLabelConfig(new Vector2(0, 0), "[name]", DrawAnchor.Top, DrawAnchor.Bottom);
            var rightLabelConfig = new EditableLabelConfig(new Vector2(0, 0), "", DrawAnchor.Center, DrawAnchor.Center);
            var optionalLabelConfig = new EditableLabelConfig(new Vector2(0, 0), "", DrawAnchor.Bottom, DrawAnchor.Bottom);

            return new FocusTargetUnitFrameConfig(pos, size, leftLabelConfig, rightLabelConfig, optionalLabelConfig);
        }
    }

    [DisableParentSettings("HideWhenInactive")]
    public class UnitFrameConfig : BarConfig
    {
        [Checkbox("使用职业颜色", spacing = true)]
        [Order(45)]
        public bool UseJobColor = true;

        [Checkbox("使用职能颜色")]
        [Order(46)]
        public bool UseRoleColor = false;

        [NestedConfig("基于血量的颜色", 50, collapsingHeader = false)]
        public ColorByHealthValueConfig ColorByHealth = new ColorByHealthValueConfig();

        [Checkbox("职业颜色作为背景色", spacing = true)]
        [Order(50)]
        public bool UseJobColorAsBackgroundColor = false;

        [Checkbox("职能颜色作为背景色")]
        [Order(51)]
        public bool UseRoleColorAsBackgroundColor = false;

        [Checkbox("缺失血量颜色")]
        [Order(55)]
        public bool UseMissingHealthBar = false;

        [Checkbox("职业颜色作为缺失血量颜色")]
        [Order(56, collapseWith = nameof(UseMissingHealthBar))]
        public bool UseJobColorAsMissingHealthColor = false;

        [Checkbox("职能颜色作为缺失血量颜色")]
        [Order(57, collapseWith = nameof(UseMissingHealthBar))]
        public bool UseRoleColorAsMissingHealthColor = false;

        [ColorEdit4("颜色" + "##MissingHealth")]
        [Order(60, collapseWith = nameof(UseMissingHealthBar))]
        public PluginConfigColor HealthMissingColor = new PluginConfigColor(new Vector4(255f / 255f, 0f / 255f, 0f / 255f, 100f / 100f));

        [Checkbox("死亡指示器背景色", spacing = true)]
        [Order(61)]
        public bool UseDeathIndicatorBackgroundColor = false;

        [ColorEdit4("颜色" + "##DeathIndicator")]
        [Order(62, collapseWith = nameof(UseDeathIndicatorBackgroundColor))]
        public PluginConfigColor DeathIndicatorBackgroundColor = new PluginConfigColor(new Vector4(204f / 255f, 3f / 255f, 3f / 255f, 50f / 100f));

        [Checkbox("坦克无敌", spacing = true)]
        [Order(95)]
        public bool ShowTankInvulnerability = true;

        [Checkbox("坦克无敌自定义颜色")]
        [Order(100, collapseWith = nameof(ShowTankInvulnerability))]
        public bool UseCustomInvulnerabilityColor = true;

        [ColorEdit4("坦克无敌颜色##TankInvulnerabilityCustom")]
        [Order(105, collapseWith = nameof(UseCustomInvulnerabilityColor))]
        public PluginConfigColor CustomInvulnerabilityColor = new PluginConfigColor(new Vector4(211f / 255f, 235f / 255f, 215f / 245f, 50f / 100f));

        [Checkbox("行尸走肉自定义颜色")]
        [Order(110, collapseWith = nameof(ShowTankInvulnerability))]
        public bool UseCustomWalkingDeadColor = true;

        [ColorEdit4("行尸走肉颜色##TankWalkingDeadCustom")]
        [Order(115, collapseWith = nameof(UseCustomWalkingDeadColor))]
        public PluginConfigColor CustomWalkingDeadColor = new PluginConfigColor(new Vector4(158f / 255f, 158f / 255f, 158f / 255f, 50f / 100f));

        [NestedConfig("使用平滑过渡", 120, collapsingHeader = false)]
        public SmoothHealthConfig SmoothHealthConfig = new SmoothHealthConfig();

        [Checkbox("尽可能隐藏血条", spacing = true, help = "如果角色没有生命值（如宠物，友好NPC等），隐藏任何带有生命值标签的标签。")]
        [Order(121)]
        public bool HideHealthIfPossible = true;

        [NestedConfig("左侧文本", 125)]
        public EditableLabelConfig LeftLabelConfig = null!;

        [NestedConfig("右侧文本", 130)]
        public EditableLabelConfig RightLabelConfig = null!;

        [NestedConfig("可选文本", 131)]
        public EditableLabelConfig OptionalLabelConfig = null!;

        [NestedConfig("职能/职业图标", 135)]
        public RoleJobIconConfig RoleIconConfig = new RoleJobIconConfig(
            new Vector2(5, 0),
            new Vector2(30, 30),
            DrawAnchor.Left,
            DrawAnchor.Left
        );

        [NestedConfig("标记图标", 136)]
        public SignIconConfig SignIconConfig = new SignIconConfig(
            new Vector2(0, 0),
            new Vector2(30, 30),
            DrawAnchor.Center,
            DrawAnchor.Top
        );

        [NestedConfig("盾牌", 140)]
        public ShieldConfig ShieldConfig = new ShieldConfig();

        [NestedConfig("根据距离改变友好单位的不透明度", 145)]
        public UnitFramesRangeConfig RangeConfig = new();

        [NestedConfig("根据距离改变敌对单位的不透明度", 146)]
        public UnitFramesRangeConfig EnemyRangeConfig = new();

        [NestedConfig("自定义鼠标悬停区域", 150)]
        public MouseoverAreaConfig MouseoverAreaConfig = new MouseoverAreaConfig();

        [NestedConfig("可见性", 200)]
        public VisibilityConfig VisibilityConfig = new VisibilityConfig();

        public UnitFrameConfig(Vector2 position, Vector2 size, EditableLabelConfig leftLabelConfig, EditableLabelConfig rightLabelConfig, EditableLabelConfig optionalLabelConfig)
            : base(position, size, new PluginConfigColor(new(40f / 255f, 40f / 255f, 40f / 255f, 100f / 100f)))
        {
            Position = position;
            Size = size;
            LeftLabelConfig = leftLabelConfig;
            RightLabelConfig = rightLabelConfig;
            OptionalLabelConfig = optionalLabelConfig;
            BackgroundColor = new PluginConfigColor(new(0f / 255f, 0f / 255f, 0f / 255f, 100f / 100f));
            RoleIconConfig.Enabled = false;
            SignIconConfig.Enabled = false;
            ColorByHealth.Enabled = false;
            MouseoverAreaConfig.Enabled = false;
        }

        public UnitFrameConfig() : base(Vector2.Zero, Vector2.Zero, new(Vector4.Zero)) { } // don't remove
    }

    [Exportable(false)]
    public class ShieldConfig : PluginConfigObject
    {
        [DragInt("厚度")]
        [Order(5)]
        public int Height = 26; // Should be 'Size' instead of 'Height' but leaving as is to avoid breaking configs

        [Checkbox("厚度（像素）")]
        [Order(10)]
        public bool HeightInPixels = false;

        [Checkbox("优先填充血条")]
        [Order(15)]
        public bool FillHealthFirst = true;

        [ColorEdit4("颜色##Shields")]
        [Order(20)]
        public PluginConfigColor Color = new PluginConfigColor(new Vector4(198f / 255f, 210f / 255f, 255f / 255f, 70f / 100f));
    }

    [Exportable(false)]
    public class SmoothHealthConfig : PluginConfigObject
    {
        [DragFloat("速度", min = 1f, max = 100f)]
        [Order(5)]
        public float Velocity = 25f;
    }

    [Exportable(false)]
    public class MouseoverAreaConfig : PluginConfigObject
    {
        [Checkbox("预览")]
        [Order(5)]
        public bool Preview = false;

        [Checkbox("忽略鼠标悬停", help = "启用此选项将使鼠标悬停完全忽略该元素。\n该区域仍然可以被定义左击和右击。")]
        [Order(6)]
        public bool Ignore = false;

        [DragInt2("上左偏移", min = -500, max = 500)]
        [Order(10)]
        public Vector2 TopLeftOffset = Vector2.Zero;

        [DragInt2("下右偏移", min = -500, max = 500)]
        [Order(11)]
        public Vector2 BottomRightOffset = Vector2.Zero;

        public MouseoverAreaConfig()
        {
            Enabled = false;
        }

        public (Vector2, Vector2) GetArea(Vector2 pos, Vector2 size)
        {
            if (!Enabled) { return (pos, pos + size); }

            Vector2 start = pos + TopLeftOffset;
            Vector2 end = pos + size + BottomRightOffset;

            return (start, end);
        }

        public BarHud? GetBar(Vector2 pos, Vector2 size, string id, DrawAnchor anchor = DrawAnchor.TopLeft)
        {
            if (!Enabled || !Preview) { return null; }

            BarHud bar = new BarHud(
                id,
                true,
                new(Vector4.One),
                2
            );

            var barPos = Utils.GetAnchoredPosition(Vector2.Zero, size, anchor);
            var (start, end) = GetArea(barPos + pos, size);
            Rect background = new Rect(start, end - start, new(new(1, 1, 1, 0.5f)));
            bar.SetBackground(background);

            return bar;
        }
    }

    [Exportable(false)]
    public class UnitFramesRangeConfig : PluginConfigObject
    {
        [DragInt("距离(yalms)", min = 1, max = 500)]
        [Order(5)]
        public int Range = 30;

        [DragFloat("不透明度", min = 1, max = 100)]
        [Order(10)]
        public float Alpha = 24;

        [Checkbox("使用额外距离检查")]
        [Order(15)]
        public bool UseAdditionalRangeCheck = false;

        [DragInt("额外距离(yalms)", min = 1, max = 500)]
        [Order(20, collapseWith = nameof(UseAdditionalRangeCheck))]
        public int AdditionalRange = 15;

        [DragFloat("额外不透明度", min = 1, max = 100)]
        [Order(25, collapseWith = nameof(UseAdditionalRangeCheck))]
        public float AdditionalAlpha = 60;

        public float AlphaForDistance(int distance, float alpha = 100f)
        {
            if (!Enabled)
            {
                return 100f;
            }

            if (!UseAdditionalRangeCheck)
            {
                return distance > Range ? Alpha : alpha;
            }

            if (Range > AdditionalRange)
            {
                return distance > Range ? Alpha : (distance > AdditionalRange ? AdditionalAlpha : alpha);
            }

            return distance > AdditionalRange ? AdditionalAlpha : (distance > Range ? Alpha : alpha);
        }
    }
}
