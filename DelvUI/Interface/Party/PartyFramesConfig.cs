using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Enums;
using DelvUI.Helpers;
using DelvUI.Interface.Bars;
using DelvUI.Interface.GeneralElements;
using DelvUI.Interface.PartyCooldowns;
using DelvUI.Interface.StatusEffects;
using ImGuiNET;
using System;
using System.Numerics;

namespace DelvUI.Interface.Party
{
    [Exportable(false)]
    [Section("小队框架", true)]
    [SubSection("通用", 0)]
    public class PartyFramesConfig : MovablePluginConfigObject
    {
        public new static PartyFramesConfig DefaultConfig()
        {
            var config = new PartyFramesConfig();
            config.Position = new Vector2(-ImGui.GetMainViewport().Size.X / 3 - 180, -120);

            return config;
        }

        [Checkbox("预览", isMonitored = true)]
        [Order(4)]
        public bool Preview = false;

        [DragInt("行", spacing = true, isMonitored = true, min = 1, max = 8, velocity = 0.2f)]
        [Order(10)]
        public int Rows = 4;

        [DragInt("列", isMonitored = true, min = 1, max = 8, velocity = 0.2f)]
        [Order(11)]
        public int Columns = 2;

        [Anchor("条的锚", isMonitored = true, spacing = true)]
        [Order(15)]
        public DrawAnchor BarsAnchor = DrawAnchor.TopLeft;

        [Checkbox("优先填充行", isMonitored = true)]
        [Order(20)]
        public bool FillRowsFirst = true;

        [Checkbox("玩家顺序覆盖启用（提示：Ctrl+Alt+Shift点击一个条来设置你想要的点位置）", spacing = true)]
        [Order(25)]
        public bool PlayerOrderOverrideEnabled = false;

        [Combo("玩家位置", "1", "2", "3", "4", "5", "6", "7", "8", "我的职能优先", isMonitored = true)]
        [Order(25, collapseWith = nameof(PlayerOrderOverrideEnabled))]
        public int PlayerOrder = 1;

        [Checkbox("单人时显示", spacing = true)]
        [Order(50)]
        public bool ShowWhenSolo = false;

        [Checkbox("显示陆行鸟", isMonitored = true)]
        [Order(55)]
        public bool ShowChocobo = true;

        [NestedConfig("小队标题标签", 60)]
        public PartyFramesTitleLabel ShowPartyTitleConfig = new PartyFramesTitleLabel(Vector2.Zero, "", DrawAnchor.Left, DrawAnchor.Left);

        [NestedConfig("可见性", 200)]
        public VisibilityConfig VisibilityConfig = new VisibilityConfig();
    }

    [Exportable(false)]
    [DisableParentSettings("FrameAnchor", "UseJobColor", "UseRoleColor")]
    public class PartyFramesTitleLabel : LabelConfig
    {
        public PartyFramesTitleLabel(Vector2 position, string text, DrawAnchor frameAnchor, DrawAnchor textAnchor) : base(position, text, frameAnchor, textAnchor)
        {
        }
    }

    [Exportable(false)]
    [Disableable(false)]
    [DisableParentSettings("Position", "Anchor", "BackgroundColor", "FillColor", "HideWhenInactive", "DrawBorder", "BorderColor", "BorderThickness")]
    [Section("小队框架", true)]
    [SubSection("生命条", 0)]
    public class PartyFramesHealthBarsConfig : BarConfig
    {
        public new static PartyFramesHealthBarsConfig DefaultConfig()
        {
            var config = new PartyFramesHealthBarsConfig(Vector2.Zero, new(180, 80), new PluginConfigColor(Vector4.Zero));
            config.MouseoverAreaConfig.Enabled = false;

            return config;
        }

        [DragInt2("填充", isMonitored = true, min = 0)]
        [Order(31)]
        public Vector2 Padding = new Vector2(0, 0);

        [NestedConfig("名字标签", 44)]
        public EditableLabelConfig NameLabelConfig = new EditableLabelConfig(Vector2.Zero, "[name:initials].", DrawAnchor.Center, DrawAnchor.Center);

        [NestedConfig("声明标签", 45)]
        public EditableLabelConfig HealthLabelConfig = new EditableLabelConfig(Vector2.Zero, "[health:current-short]", DrawAnchor.Right, DrawAnchor.Right);

        [NestedConfig("目标标签", 50)]
        public DefaultFontLabelConfig OrderNumberConfig = new DefaultFontLabelConfig(new Vector2(2, 4), "", DrawAnchor.TopLeft, DrawAnchor.TopLeft);

        [NestedConfig("颜色", 55)]
        public PartyFramesColorsConfig ColorsConfig = new PartyFramesColorsConfig();

        [NestedConfig("盾", 60)]
        public ShieldConfig ShieldConfig = new ShieldConfig();

        [NestedConfig("根据距离改变不透明度", 65)]
        public PartyFramesRangeConfig RangeConfig = new PartyFramesRangeConfig();

        [NestedConfig("使用平滑过渡", 70)]
        public SmoothHealthConfig SmoothHealthConfig = new SmoothHealthConfig();

        [NestedConfig("自定义鼠标悬停区域", 75)]
        public MouseoverAreaConfig MouseoverAreaConfig = new MouseoverAreaConfig();

        public PartyFramesHealthBarsConfig(Vector2 position, Vector2 size, PluginConfigColor fillColor, BarDirection fillDirection = BarDirection.Right)
            : base(position, size, fillColor, fillDirection)
        {
        }
    }

    [Disableable(false)]
    [Exportable(false)]
    public class PartyFramesColorsConfig : PluginConfigObject
    {
        [Checkbox("显示边框")]
        [Order(4)]
        public bool ShowBorder = true;

        [ColorEdit4("边框颜色")]
        [Order(5, collapseWith = nameof(ShowBorder))]
        public PluginConfigColor BorderColor = new PluginConfigColor(new Vector4(0f / 255f, 0f / 255f, 0f / 255f, 100f / 100f));

        [ColorEdit4("目标边框颜色")]
        [Order(6, collapseWith = nameof(ShowBorder))]
        public PluginConfigColor TargetBordercolor = new PluginConfigColor(new Vector4(255f / 255f, 255f / 255f, 255f / 255f, 100f / 100f));

        [DragInt("非活跃边框厚度", min = 1, max = 10, help = "这是边框在默认状态（即不被选中，不显示仇恨等）时使用的边框厚度。")]
        [Order(6, collapseWith = nameof(ShowBorder))]
        public int InactiveBorderThickness = 1;

        [DragInt("活跃边框厚度", min = 1, max = 10, help = "这是边框活跃时（即被选中，显示仇恨等）使用的边框厚度。")]
        [Order(7, collapseWith = nameof(ShowBorder))]
        public int ActiveBorderThickness = 1;

        [ColorEdit4("背景色", spacing = true)]
        [Order(15)]
        public PluginConfigColor BackgroundColor = new PluginConfigColor(new Vector4(0f / 255f, 0f / 255f, 0f / 255f, 70f / 100f));

        [ColorEdit4("无法检索的背景色", help = "当玩家的数据无法被检索时（即玩家断开连接）使用的背景色。")]
        [Order(15)]
        public PluginConfigColor OutOfReachBackgroundColor = new PluginConfigColor(new Vector4(50f / 255f, 50f / 255f, 50f / 255f, 70f / 100f));

        [Checkbox("使用死亡指示器背景色", isMonitored = true, spacing = true)]
        [Order(18)]
        public bool UseDeathIndicatorBackgroundColor = false;

        [ColorEdit4("死亡指示器背景色")]
        [Order(19, collapseWith = nameof(UseDeathIndicatorBackgroundColor))]
        public PluginConfigColor DeathIndicatorBackgroundColor = new PluginConfigColor(new Vector4(204f / 255f, 3f / 255f, 3f / 255f, 80f / 100f));

        [Checkbox("使用职能颜色", isMonitored = true, spacing = true)]
        [Order(20)]
        public bool UseRoleColors = false;

        [NestedConfig("基于生命值的颜色", 30, collapsingHeader = false)]
        public ColorByHealthValueConfig ColorByHealth = new ColorByHealthValueConfig();

        [Checkbox("用光标悬停或软选中时高亮显示", spacing = true)]
        [Order(40)]
        public bool ShowHighlight = true;

        [ColorEdit4("高亮颜色")]
        [Order(45, collapseWith = nameof(ShowHighlight))]
        public PluginConfigColor HighlightColor = new PluginConfigColor(new Vector4(255f / 255f, 255f / 255f, 255f / 255f, 5f / 100f));


        [Checkbox("缺失生命值颜色", spacing = true)]
        [Order(46)]
        public bool UseMissingHealthBar = false;

        [Checkbox("职业颜色作为缺失生命值颜色")]
        [Order(47, collapseWith = nameof(UseMissingHealthBar))]
        public bool UseJobColorAsMissingHealthColor = false;

        [Checkbox("职能颜色作为缺失生命值颜色")]
        [Order(48, collapseWith = nameof(UseMissingHealthBar))]
        public bool UseRoleColorAsMissingHealthColor = false;

        [ColorEdit4("Color" + "##MissingHealth")]
        [Order(49, collapseWith = nameof(UseMissingHealthBar))]
        public PluginConfigColor HealthMissingColor = new(new Vector4(255f / 255f, 0f / 255f, 0f / 255f, 100f / 100f));

        [Checkbox("职业颜色作为背景色")]
        [Order(50)]
        public bool UseJobColorAsBackgroundColor = false;

        [Checkbox("职能颜色作为背景色")]
        [Order(51)]
        public bool UseRoleColorAsBackgroundColor = false;

        [Checkbox("显示一仇颜色", spacing = true)]
        [Order(54)]
        public bool ShowEnmityBorderColors = true;

        [ColorEdit4("一仇颜色")]
        [Order(55, collapseWith = nameof(ShowEnmityBorderColors))]
        public PluginConfigColor EnmityLeaderBordercolor = new PluginConfigColor(new Vector4(255f / 255f, 40f / 255f, 40f / 255f, 100f / 100f));

        [Checkbox("显示二仇")]
        [Order(60, collapseWith = nameof(ShowEnmityBorderColors))]
        public bool ShowSecondEnmity = true;

        [Checkbox("在轻锐小队隐藏二仇")]
        [Order(65, collapseWith = nameof(ShowSecondEnmity))]
        public bool HideSecondEnmityInLightParties = true;

        [ColorEdit4("二仇颜色")]
        [Order(70, collapseWith = nameof(ShowSecondEnmity))]
        public PluginConfigColor EnmitySecondBordercolor = new PluginConfigColor(new Vector4(255f / 255f, 175f / 255f, 40f / 255f, 100f / 100f));
    }

    [Exportable(false)]
    public class PartyFramesRangeConfig : PluginConfigObject
    {
        [DragInt("距离(yalms)", min = 1, max = 500)]
        [Order(5)]
        public int Range = 30;

        [DragFloat("不透明度", min = 1, max = 100)]
        [Order(10)]
        public float Alpha = 25;

        [Checkbox("使用额外距离检查")]
        [Order(15)]
        public bool UseAdditionalRangeCheck = false;

        [DragInt("额外距离(yalms)", min = 1, max = 500)]
        [Order(20, collapseWith = nameof(UseAdditionalRangeCheck))]
        public int AdditionalRange = 15;

        [DragFloat("额外透明度", min = 1, max = 100)]
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

    public class PartyFramesManaBarConfigConverter : PluginConfigObjectConverter
    {
        public PartyFramesManaBarConfigConverter()
        {
            NewTypeFieldConverter<bool, PartyFramesManaBarDisplayMode> converter;
            converter = new NewTypeFieldConverter<bool, PartyFramesManaBarDisplayMode>(
                "PartyFramesManaBarDisplayMode",
                PartyFramesManaBarDisplayMode.HealersOnly,
                (oldValue) =>
                {
                    return oldValue ? PartyFramesManaBarDisplayMode.HealersOnly : PartyFramesManaBarDisplayMode.Always;
                });

            FieldConvertersMap.Add("ShowOnlyForHealers", converter);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PartyFramesManaBarConfig);
        }
    }

    public enum PartyFramesManaBarDisplayMode
    {
        HealersAndRaiseJobs,
        HealersOnly,
        Always,
    }

    [DisableParentSettings("HideWhenInactive", "Label")]
    [Exportable(false)]
    [Section("小队框架", true)]
    [SubSection("魔力条", 0)]
    public class PartyFramesManaBarConfig : PrimaryResourceConfig
    {
        public new static PartyFramesManaBarConfig DefaultConfig()
        {
            var config = new PartyFramesManaBarConfig(Vector2.Zero, new(180, 6));
            config.HealthBarAnchor = DrawAnchor.Bottom;
            config.Anchor = DrawAnchor.Bottom;
            config.ValueLabel.Enabled = false;
            return config;
        }

        [Anchor("生命条锚")]
        [Order(14)]
        public DrawAnchor HealthBarAnchor = DrawAnchor.BottomLeft;

        [RadioSelector("为 所有 有复活的职业显示", "只为治疗职业显示", "为 所有 职业显示")]
        [Order(42)]
        public PartyFramesManaBarDisplayMode ManaBarDisplayMode = PartyFramesManaBarDisplayMode.HealersOnly;

        public PartyFramesManaBarConfig(Vector2 position, Vector2 size)
            : base(position, size)
        {
        }
    }

    [Exportable(false)]
    [Section("小队锚", true)]
    [SubSection("咏唱栏", 0)]
    public class PartyFramesCastbarConfig : CastbarConfig
    {
        public new static PartyFramesCastbarConfig DefaultConfig()
        {
            var size = new Vector2(182, 10);
            var pos = new Vector2(-1, 0);

            var castNameConfig = new LabelConfig(new Vector2(5, 0), "", DrawAnchor.Left, DrawAnchor.Left);
            var castTimeConfig = new NumericLabelConfig(new Vector2(-5, 0), "", DrawAnchor.Right, DrawAnchor.Right);
            castTimeConfig.Enabled = false;
            castTimeConfig.NumberFormat = 1;

            var config = new PartyFramesCastbarConfig(pos, size, castNameConfig, castTimeConfig);
            config.HealthBarAnchor = DrawAnchor.BottomLeft;
            config.Anchor = DrawAnchor.TopLeft;
            config.ShowIcon = false;
            config.Enabled = false;

            return config;
        }

        [Checkbox("咏唱时隐藏名称")]
        [Order(6)]
        public bool HideNameWhenCasting = false;

        [Anchor("生命条锚")]
        [Order(16)]
        public DrawAnchor HealthBarAnchor = DrawAnchor.BottomLeft;

        public PartyFramesCastbarConfig(Vector2 position, Vector2 size, LabelConfig castNameConfig, NumericLabelConfig castTimeConfig)
            : base(position, size, castNameConfig, castTimeConfig)
        {

        }
    }

    [Disableable(false)]
    [Exportable(false)]
    [Section("小队框架", true)]
    [SubSection("图标", 0)]
    public class PartyFramesIconsConfig : PluginConfigObject
    {
        public new static PartyFramesIconsConfig DefaultConfig() { return new PartyFramesIconsConfig(); }

        [NestedConfig("职能/职业", 10, separator = false)]
        public PartyFramesRoleIconConfig Role = new PartyFramesRoleIconConfig(
            new Vector2(20, 0),
            new Vector2(20, 20),
            DrawAnchor.TopLeft,
            DrawAnchor.TopLeft
        );

        [NestedConfig("标记", 11)]
        public SignIconConfig Sign = new SignIconConfig(
            new Vector2(0, -10),
            new Vector2(30, 30),
            DrawAnchor.Top,
            DrawAnchor.Top
        );

        [NestedConfig("领袖", 12)]
        public PartyFramesLeaderIconConfig Leader = new PartyFramesLeaderIconConfig(
            new Vector2(-12, -12),
            new Vector2(24, 24),
            DrawAnchor.TopLeft,
            DrawAnchor.TopLeft
        );

        [NestedConfig("玩家状态", 13)]
        public PartyFramesPlayerStatusConfig PlayerStatus = new PartyFramesPlayerStatusConfig();

        [NestedConfig("准备缺人状态", 14)]
        public PartyFramesReadyCheckStatusConfig ReadyCheckStatus = new PartyFramesReadyCheckStatusConfig();

        [NestedConfig("谁的发言", 15)]
        public PartyFramesWhosTalkingConfig WhosTalking = new PartyFramesWhosTalkingConfig();
    }

    [Exportable(false)]
    public class PartyFramesRoleIconConfig : RoleJobIconConfig
    {
        public PartyFramesRoleIconConfig() : base() { }

        public PartyFramesRoleIconConfig(Vector2 position, Vector2 size, DrawAnchor anchor, DrawAnchor frameAnchor)
            : base(position, size, anchor, frameAnchor)
        {
        }
    }

    [Exportable(false)]
    public class PartyFramesLeaderIconConfig : IconConfig
    {
        public PartyFramesLeaderIconConfig() : base() { }

        public PartyFramesLeaderIconConfig(Vector2 position, Vector2 size, DrawAnchor anchor, DrawAnchor frameAnchor)
            : base(position, size, anchor, frameAnchor)
        {
        }
    }

    [Exportable(false)]
    public class PartyFramesPlayerStatusConfig : PluginConfigObject
    {
        public new static PartyFramesPlayerStatusConfig DefaultConfig()
        {
            var config = new PartyFramesPlayerStatusConfig();
            config.Label.Enabled = false;

            return config;
        }

        [Checkbox("显示状态时隐藏名称")]
        [Order(5)]
        public bool HideName = false;

        [NestedConfig("图标", 10)]
        public IconConfig Icon = new IconConfig(
            new Vector2(0, 5),
            new Vector2(16, 16),
            DrawAnchor.Top,
            DrawAnchor.Top
        );

        [NestedConfig("标签", 15)]
        public LabelConfig Label = new LabelConfig(Vector2.Zero, "", DrawAnchor.Center, DrawAnchor.Center);
    }

    [Exportable(false)]
    public class PartyFramesReadyCheckStatusConfig : PluginConfigObject
    {
        public new static PartyFramesReadyCheckStatusConfig DefaultConfig() => new PartyFramesReadyCheckStatusConfig();

        [Checkbox("显示状态时隐藏名称")]
        [Order(5)]
        public bool HideName = false;

        [DragInt("持续时间（秒）", min = 1, max = 60, help = "准备确认完成后图标将显示多长时间。")]
        [Order(6)]
        public int Duration = 10;

        [NestedConfig("图标", 10)]
        public IconConfig Icon = new IconConfig(
            new Vector2(0, 0),
            new Vector2(24, 24),
            DrawAnchor.TopRight,
            DrawAnchor.TopRight
        );
    }

    [Exportable(false)]
    public class PartyFramesWhosTalkingConfig : PluginConfigObject
    {
        public new static PartyFramesWhosTalkingConfig DefaultConfig() => new PartyFramesWhosTalkingConfig();

        [Checkbox("活跃时替换职能/职业图标")]
        [Order(5)]
        public bool ReplaceRoleJobIcon = false;

        [Checkbox("显示发言状态", spacing = true)]
        [Order(10)]
        public bool ShowSpeaking = true;

        [Checkbox("显示静音状态")]
        [Order(10)]
        public bool ShowMuted = true;

        [Checkbox("显示屏蔽状态")]
        [Order(10)]
        public bool ShowDeafened = true;

        [NestedConfig("图标", 20)]
        public IconConfig Icon = new IconConfig(
            new Vector2(0, 0),
            new Vector2(24, 24),
            DrawAnchor.TopRight,
            DrawAnchor.TopRight
        );

        [Checkbox("活跃时改变生命条边框", spacing = true, help = "启用此选项将覆盖其他边框设置!")]
        [Order(30)]
        public bool ChangeBorders = false;

        [DragInt("边框厚度", min = 1, max = 10)]
        [Order(31, collapseWith = nameof(ChangeBorders))]
        public int BorderThickness = 1;

        [ColorEdit4("发言边框颜色")]
        [Order(32, collapseWith = nameof(ChangeBorders))]
        public PluginConfigColor SpeakingBorderColor = PluginConfigColor.FromHex(0xFF40BB40);

        [ColorEdit4("静音边框颜色")]
        [Order(33, collapseWith = nameof(ChangeBorders))]
        public PluginConfigColor MutedBorderColor = PluginConfigColor.FromHex(0xFF008080);

        [ColorEdit4("屏蔽边框颜色")]
        [Order(34, collapseWith = nameof(ChangeBorders))]
        public PluginConfigColor DeafenedBorderColor = PluginConfigColor.FromHex(0xFFFF4444);

        public bool EnabledForState(WhosTalkingState state)
        {
            switch (state)
            {
                case WhosTalkingState.Speaking: return ShowSpeaking;
                case WhosTalkingState.Muted: return ShowMuted;
                case WhosTalkingState.Deafened: return ShowDeafened;
            }

            return false;
        }

        public PluginConfigColor? ColorForState(WhosTalkingState state)
        {
            if (state == WhosTalkingState.Speaking && ShowSpeaking) { return SpeakingBorderColor; }
            if (state == WhosTalkingState.Muted && ShowMuted) { return MutedBorderColor; }
            if (ShowDeafened) { return DeafenedBorderColor; }

            return null;
        }
    }

    [Exportable(false)]
    [Section("小队框架", true)]
    [SubSection("增益状态", 0)]
    public class PartyFramesBuffsConfig : PartyFramesStatusEffectsListConfig
    {
        public new static PartyFramesBuffsConfig DefaultConfig()
        {
            var durationConfig = new LabelConfig(new Vector2(0, -4), "", DrawAnchor.Bottom, DrawAnchor.Center);
            var stacksConfig = new LabelConfig(new Vector2(-3, 4), "", DrawAnchor.TopRight, DrawAnchor.Center);
            stacksConfig.Color = new(Vector4.UnitW);
            stacksConfig.OutlineColor = new(Vector4.One);

            var iconConfig = new StatusEffectIconConfig(durationConfig, stacksConfig);
            iconConfig.DispellableBorderConfig.Enabled = false;
            iconConfig.Size = new Vector2(24, 24);

            var pos = new Vector2(-2, 2);
            var size = new Vector2(iconConfig.Size.X * 4 + 6, iconConfig.Size.Y);

            var config = new PartyFramesBuffsConfig(DrawAnchor.TopRight, pos, size, true, false, false, GrowthDirections.Left | GrowthDirections.Down, iconConfig);
            config.Limit = 4;

            return config;
        }

        public PartyFramesBuffsConfig(DrawAnchor anchor, Vector2 position, Vector2 size, bool showBuffs, bool showDebuffs, bool showPermanentEffects,
            GrowthDirections growthDirections, StatusEffectIconConfig iconConfig)
            : base(anchor, position, size, showBuffs, showDebuffs, showPermanentEffects, growthDirections, iconConfig)
        {
        }
    }

    [Exportable(false)]
    [Section("小队框架", true)]
    [SubSection("减益状态", 0)]
    public class PartyFramesDebuffsConfig : PartyFramesStatusEffectsListConfig
    {
        public new static PartyFramesDebuffsConfig DefaultConfig()
        {
            var durationConfig = new LabelConfig(new Vector2(0, -4), "", DrawAnchor.Bottom, DrawAnchor.Center);
            var stacksConfig = new LabelConfig(new Vector2(-3, 4), "", DrawAnchor.TopRight, DrawAnchor.Center);
            stacksConfig.Color = new(Vector4.UnitW);
            stacksConfig.OutlineColor = new(Vector4.One);

            var iconConfig = new StatusEffectIconConfig(durationConfig, stacksConfig);
            iconConfig.Size = new Vector2(24, 24);

            var pos = new Vector2(-2, -2);
            var size = new Vector2(iconConfig.Size.X * 4 + 6, iconConfig.Size.Y);

            var config = new PartyFramesDebuffsConfig(DrawAnchor.BottomRight, pos, size, false, true, false, GrowthDirections.Left | GrowthDirections.Up, iconConfig);
            config.Limit = 4;

            return config;
        }

        public PartyFramesDebuffsConfig(DrawAnchor anchor, Vector2 position, Vector2 size, bool showBuffs, bool showDebuffs, bool showPermanentEffects,
            GrowthDirections growthDirections, StatusEffectIconConfig iconConfig)
            : base(anchor, position, size, showBuffs, showDebuffs, showPermanentEffects, growthDirections, iconConfig)
        {
        }
    }

    public class PartyFramesStatusEffectsListConfig : StatusEffectsListConfig
    {
        [Anchor("生命条锚")]
        [Order(4)]
        public DrawAnchor HealthBarAnchor = DrawAnchor.BottomLeft;

        public PartyFramesStatusEffectsListConfig(DrawAnchor anchor, Vector2 position, Vector2 size, bool showBuffs, bool showDebuffs, bool showPermanentEffects,
            GrowthDirections growthDirections, StatusEffectIconConfig iconConfig)
            : base(position, size, showBuffs, showDebuffs, showPermanentEffects, growthDirections, iconConfig)
        {
            HealthBarAnchor = anchor;
        }
    }

    [Disableable(false)]
    [Exportable(false)]
    [Section("小队框架", true)]
    [SubSection("监控器", 0)]
    public class PartyFramesTrackersConfig : PluginConfigObject
    {
        public new static PartyFramesTrackersConfig DefaultConfig() { return new PartyFramesTrackersConfig(); }

        [NestedConfig("复活监控器", 10, separator = false)]
        public PartyFramesRaiseTrackerConfig Raise = new PartyFramesRaiseTrackerConfig();

        [NestedConfig("无敌监控器", 15)]
        public PartyFramesInvulnTrackerConfig Invuln = new PartyFramesInvulnTrackerConfig();

        [NestedConfig("康复监控器", 15)]
        public PartyFramesCleanseTrackerConfig Cleanse = new PartyFramesCleanseTrackerConfig();
    }

    [Exportable(false)]
    public class PartyFramesRaiseTrackerConfig : PluginConfigObject
    {
        public new static PartyFramesRaiseTrackerConfig DefaultConfig() { return new PartyFramesRaiseTrackerConfig(); }

        [Checkbox("被复活时隐藏名字")]
        [Order(10)]
        public bool HideNameWhenRaised = true;

        [Checkbox("咏唱完毕后仍保持图标")]
        [Order(15)]
        public bool KeepIconAfterCastFinishes = true;

        [Checkbox("被复活时改变背景色", spacing = true)]
        [Order(20)]
        public bool ChangeBackgroundColorWhenRaised = true;

        [ColorEdit4("复活背景色")]
        [Order(25, collapseWith = nameof(ChangeBackgroundColorWhenRaised))]
        public PluginConfigColor BackgroundColor = new(new Vector4(211f / 255f, 235f / 255f, 215f / 245f, 50f / 100f));

        [Checkbox("被复活时改变边框颜色", spacing = true)]
        [Order(30)]
        public bool ChangeBorderColorWhenRaised = true;

        [ColorEdit4("复活边框颜色")]
        [Order(35, collapseWith = nameof(ChangeBorderColorWhenRaised))]
        public PluginConfigColor BorderColor = new(new Vector4(47f / 255f, 169f / 255f, 215f / 255f, 100f / 100f));

        [NestedConfig("图标", 50)]
        public IconWithLabelConfig Icon = new IconWithLabelConfig(
            new Vector2(0, 0),
            new Vector2(50, 50),
            DrawAnchor.Center,
            DrawAnchor.Center
        );
    }

    [Exportable(false)]
    public class PartyFramesInvulnTrackerConfig : PluginConfigObject
    {
        public new static PartyFramesInvulnTrackerConfig DefaultConfig() { return new PartyFramesInvulnTrackerConfig(); }

        [Checkbox("无敌时隐藏名字")]
        [Order(10)]
        public bool HideNameWhenInvuln = true;

        [Checkbox("无敌时改变背景色", spacing = true)]
        [Order(15)]
        public bool ChangeBackgroundColorWhenInvuln = true;

        [ColorEdit4("无敌背景色")]
        [Order(20, collapseWith = nameof(ChangeBackgroundColorWhenInvuln))]
        public PluginConfigColor BackgroundColor = new(new Vector4(211f / 255f, 235f / 255f, 215f / 245f, 50f / 100f));

        [Checkbox("行尸走肉自定义颜色")]
        [Order(25, collapseWith = nameof(ChangeBackgroundColorWhenInvuln))]
        public bool UseCustomWalkingDeadColor = true;

        [ColorEdit4("行尸走肉背景色")]
        [Order(30, collapseWith = nameof(UseCustomWalkingDeadColor))]
        public PluginConfigColor WalkingDeadBackgroundColor = new(new Vector4(158f / 255f, 158f / 255f, 158f / 255f, 50f / 100f));

        [NestedConfig("图标", 50)]
        public IconWithLabelConfig Icon = new IconWithLabelConfig(
            new Vector2(0, 0),
            new Vector2(50, 50),
            DrawAnchor.Center,
            DrawAnchor.Center
        );
    }

    public class PartyFramesTrackerConfigConverter : PluginConfigObjectConverter
    {
        public PartyFramesTrackerConfigConverter()
        {
            SameTypeFieldConverter<Vector2> pos = new SameTypeFieldConverter<Vector2>("Icon.Position", Vector2.Zero);
            FieldConvertersMap.Add("Position", pos);

            SameTypeFieldConverter<Vector2> size = new SameTypeFieldConverter<Vector2>("Icon.Size", new Vector2(50, 50));
            FieldConvertersMap.Add("IconSize", size);

            SameTypeFieldConverter<DrawAnchor> anchor = new SameTypeFieldConverter<DrawAnchor>("Icon.Anchor", DrawAnchor.Center);
            FieldConvertersMap.Add("Anchor", anchor);

            SameTypeFieldConverter<DrawAnchor> frameAnchor = new SameTypeFieldConverter<DrawAnchor>("Icon.FrameAnchor", DrawAnchor.Center);
            FieldConvertersMap.Add("HealthBarAnchor", frameAnchor);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PartyFramesRaiseTrackerConfig) ||
                   objectType == typeof(PartyFramesInvulnTrackerConfig);
        }
    }

    [DisableParentSettings("Position", "Strata")]
    [Exportable(false)]
    public class PartyFramesCleanseTrackerConfig : MovablePluginConfigObject
    {
        public new static PartyFramesCleanseTrackerConfig DefaultConfig() { return new PartyFramesCleanseTrackerConfig(); }

        [Checkbox("仅在有康复的职业时展示", spacing = true)]
        [Order(10)]
        public bool CleanseJobsOnly = true;

        [Checkbox("改变生命条颜色", spacing = true)]
        [Order(15)]
        public bool ChangeHealthBarCleanseColor = true;

        [ColorEdit4("生命条颜色")]
        [Order(20, collapseWith = nameof(ChangeHealthBarCleanseColor))]
        public PluginConfigColor HealthBarColor = new(new Vector4(255f / 255f, 0f / 255f, 104f / 255f, 100f / 100f));

        [Checkbox("改变边框颜色", spacing = true)]
        [Order(25)]
        public bool ChangeBorderCleanseColor = true;

        [ColorEdit4("边框颜色")]
        [Order(30, collapseWith = nameof(ChangeBorderCleanseColor))]
        public PluginConfigColor BorderColor = new(new Vector4(255f / 255f, 0f / 255f, 104f / 255f, 100f / 100f));
    }

    [Exportable(false)]
    [DisableParentSettings("Anchor")]
    [Section("小队框架", true)]
    [SubSection("冷却时间", 0)]
    public class PartyFramesCooldownListConfig : AnchorablePluginConfigObject
    {
        public new static PartyFramesCooldownListConfig DefaultConfig()
        {
            PartyFramesCooldownListConfig config = new PartyFramesCooldownListConfig();
            config.Position = new Vector2(-2, 0);
            config.Size = new Vector2(40 * 8 + 6, 40);

            return config;
        }

        [Anchor("生命条锚")]
        [Order(3)]
        public DrawAnchor HealthBarAnchor = DrawAnchor.Left;

        [Checkbox("提示框", spacing = true)]
        [Order(20)]
        public bool ShowTooltips = true;

        [Checkbox("预览", isMonitored = true)]
        [Order(21)]
        public bool Preview;

        [DragInt2("图标尺寸", min = 1, max = 4000, spacing = true)]
        [Order(30)]
        public Vector2 IconSize = new Vector2(40, 40);

        [DragInt2("图标填充", min = 0, max = 500)]
        [Order(31)]
        public Vector2 IconPadding = new(4, 4);

        [Checkbox("优先填充行")]
        [Order(32)]
        public bool FillRowsFirst = true;

        [Combo("图标增长方向",
            "右下",
            "右上",
            "左下",
            "左上",
            "中上",
            "中下",
            "中左",
            "中右"
        )]
        [Order(33)]
        public int Directions = 3; // left & up

        [Checkbox("显示边框", spacing = true)]
        [Order(35)]
        public bool DrawBorder = true;

        [ColorEdit4("边框颜色")]
        [Order(36, collapseWith = nameof(DrawBorder))]
        public PluginConfigColor BorderColor = new PluginConfigColor(new Vector4(0f / 255f, 0f / 255f, 0f / 255f, 100f / 100f));

        [DragInt("边框厚度", min = 1, max = 10)]
        [Order(37, collapseWith = nameof(DrawBorder))]
        public int BorderThickness = 1;

        [Checkbox("活跃时改变图标边框")]
        [Order(45, collapseWith = nameof(DrawBorder))]
        public bool ChangeIconBorderWhenActive = true;

        [ColorEdit4("图标活跃边框颜色")]
        [Order(46, collapseWith = nameof(ChangeIconBorderWhenActive))]
        public PluginConfigColor IconActiveBorderColor = new PluginConfigColor(new Vector4(255f / 255f, 200f / 255f, 35f / 255f, 100f / 100f));

        [DragInt("图标活跃边框厚度", min = 1, max = 10)]
        [Order(47, collapseWith = nameof(ChangeIconBorderWhenActive))]
        public int IconActiveBorderThickness = 3;

        [Checkbox("活跃时改变标签颜色", spacing = true)]
        [Order(50)]
        public bool ChangeLabelsColorWhenActive = false;

        [ColorEdit4("标签活跃颜色")]
        [Order(51, collapseWith = nameof(ChangeLabelsColorWhenActive))]
        public PluginConfigColor LabelsActiveColor = new PluginConfigColor(new Vector4(255f / 255f, 200f / 255f, 35f / 255f, 100f / 100f));

        [NestedConfig("时间标签", 80)]
        public PartyCooldownTimeLabelConfig TimeLabel = new PartyCooldownTimeLabelConfig(new Vector2(0, 0), "", DrawAnchor.Center, DrawAnchor.Center) { NumberFormat = 1 };
    }
}
