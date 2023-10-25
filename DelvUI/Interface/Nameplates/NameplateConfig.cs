using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Enums;
using DelvUI.Helpers;
using DelvUI.Interface.Bars;
using DelvUI.Interface.StatusEffects;
using System;
using System.Numerics;

namespace DelvUI.Interface.GeneralElements
{
    public enum NameplatesOcclusionMode
    {
        None = 0,
        Simple = 1,
        Full
    };

    public enum NameplatesOcclusionType
    {
        Walls = 0,
        WallsAndObjects = 1
    };

    [DisableParentSettings("Strata", "Position")]
    [Section("名牌")]
    [SubSection("通用", 0)]
    public class NameplatesGeneralConfig : MovablePluginConfigObject
    {
        public new static NameplatesGeneralConfig DefaultConfig() => new NameplatesGeneralConfig();

        [Combo("遮挡模式", new string[] { "禁用", "简易", "完全" }, help = "这控制你是否能透过墙壁和物体看到名牌。\n\n禁用：在范围内的单位将始显示名牌。\n简易：使用简单的计算来检查名牌是否被墙壁或物体遮挡。使用它可以获得更好的性能。\n完全：使用更复杂的计算来检查名牌是否被墙壁或物体遮挡。使用这个可以获得更好的效果。")]
        [Order(10)]
        public NameplatesOcclusionMode OcclusionMode = NameplatesOcclusionMode.Full;

        [Combo("遮挡种类", new string[] { "墙", "墙与物体" }, help = "这控制哪一种对象将遮挡名牌。\n\n\n墙：默认设置。只有墙壁会遮挡名牌。\n\n墙壁和物体：像柱子和树这样的物体也会遮挡名牌。\n这种遮挡类型可能会产生一些意料之外的结果，比如计数器后面的NPC的名牌不可见。")]
        [Order(11)]
        public NameplatesOcclusionType OcclusionType = NameplatesOcclusionType.Walls;

        [Checkbox("尽量保持名牌在屏幕上", spacing = true, help = "免责声明：DelvUI严重依赖于游戏的默认名牌，所以这个设置不会有很大的提升。\n这个设置试图防止名牌在屏幕边缘被切断，但它不会一直显示游戏不会显示的名牌。")]
        [Order(20)]
        public bool ClampToScreen = true;

        [Checkbox("总是显示目标的名牌")]
        [Order(21)]
        public bool AlwaysShowTargetNameplate = true;

        public int RaycastFlag() => OcclusionType == NameplatesOcclusionType.WallsAndObjects ? 0x2000 : 0x4000;
    }

    [DisableParentSettings("HideWhenInactive")]
    [Section("名牌")]
    [SubSection("玩家", 0)]
    public class PlayerNameplateConfig : NameplateWithPlayerBarConfig
    {
        public PlayerNameplateConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig,
            NameplatePlayerBarConfig barConfig)
            : base(position, nameLabel, titleLabelConfig, barConfig)
        {
        }

        public new static PlayerNameplateConfig DefaultConfig()
        {
            return NameplatesHelper.GetNameplateWithBarConfig<PlayerNameplateConfig, NameplatePlayerBarConfig>(
                0xFFD0E5E0,
                0xFF30444A,
                HUDConstants.DefaultPlayerNameplateBarSize
            );
        }
    }

    [DisableParentSettings("HideWhenInactive", "TitleLabelConfig", "SwapLabelsWhenNeeded")]
    [Section("名牌")]
    [SubSection("敌人", 0)]
    public class EnemyNameplateConfig : NameplateWithEnemyBarConfig
    {
        public EnemyNameplateConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig,
            NameplateEnemyBarConfig barConfig)
            : base(position, nameLabel, titleLabelConfig, barConfig)
        {
        }

        public new static EnemyNameplateConfig DefaultConfig()
        {
            EnemyNameplateConfig config = NameplatesHelper.GetNameplateWithBarConfig<EnemyNameplateConfig, NameplateEnemyBarConfig>(
                0xFF993535,
                0xFF000000,
                HUDConstants.DefaultEnemyNameplateBarSize
            );

            config.SwapLabelsWhenNeeded = false;

            config.NameLabelConfig.Position = new Vector2(-8, 0);
            config.NameLabelConfig.Text = "Lv[level] [name]";
            config.NameLabelConfig.FrameAnchor = DrawAnchor.TopRight;
            config.NameLabelConfig.TextAnchor = DrawAnchor.Right;
            config.NameLabelConfig.Color = PluginConfigColor.FromHex(0xFFFFFFFF);

            config.BarConfig.LeftLabelConfig.Enabled = true;
            config.BarConfig.OnlyShowWhenNotFull = false;

            // debuffs
            LabelConfig durationConfig = new LabelConfig(new Vector2(0, -4), "", DrawAnchor.Bottom, DrawAnchor.Center);
            durationConfig.FontID = FontsConfig.DefaultMediumFontKey;

            LabelConfig stacksConfig = new LabelConfig(new Vector2(-3, 4), "", DrawAnchor.TopRight, DrawAnchor.Center);
            durationConfig.FontID = FontsConfig.DefaultMediumFontKey;
            stacksConfig.Color = new(Vector4.UnitW);
            stacksConfig.OutlineColor = new(Vector4.One);

            StatusEffectIconConfig iconConfig = new StatusEffectIconConfig(durationConfig, stacksConfig);
            iconConfig.Size = new Vector2(30, 30);
            iconConfig.DispellableBorderConfig.Enabled = false;

            Vector2 pos = new Vector2(2, -20);
            Vector2 size = new Vector2(230, 70);

            EnemyNameplateStatusEffectsListConfig debuffs = new EnemyNameplateStatusEffectsListConfig(
                DrawAnchor.TopLeft,
                pos,
                size,
                false,
                true,
                false,
                GrowthDirections.Right | GrowthDirections.Up,
                iconConfig
            );
            debuffs.Limit = 7;
            debuffs.ShowPermanentEffects = true;
            debuffs.IconConfig.DispellableBorderConfig.Enabled = false;
            debuffs.IconPadding = new Vector2(1, 6);
            debuffs.ShowOnlyMine = true;
            debuffs.ShowTooltips = false;
            debuffs.DisableInteraction = true;
            config.DebuffsConfig = debuffs;

            // castbar
            Vector2 castbarSize = new Vector2(config.BarConfig.Size.X, 10);

            LabelConfig castNameConfig = new LabelConfig(new Vector2(0, -1), "", DrawAnchor.Center, DrawAnchor.Center);
            castNameConfig.FontID = FontsConfig.DefaultSmallFontKey;

            NumericLabelConfig castTimeConfig = new NumericLabelConfig(new Vector2(-5, 0), "", DrawAnchor.Right, DrawAnchor.Right);
            castTimeConfig.Enabled = false;
            castTimeConfig.FontID = FontsConfig.DefaultSmallFontKey;
            castTimeConfig.NumberFormat = 1;

            NameplateCastbarConfig castbarConfig = new NameplateCastbarConfig(Vector2.Zero, castbarSize, castNameConfig, castTimeConfig);
            castbarConfig.HealthBarAnchor = DrawAnchor.BottomLeft;
            castbarConfig.Anchor = DrawAnchor.TopLeft;
            castbarConfig.ShowIcon = false;
            config.CastbarConfig = castbarConfig;

            return config;
        }
    }

    [DisableParentSettings("HideWhenInactive")]
    [Section("名牌")]
    [SubSection("小队成员", 0)]
    public class PartyMembersNameplateConfig : NameplateWithPlayerBarConfig
    {
        public PartyMembersNameplateConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig,
            NameplatePlayerBarConfig barConfig)
            : base(position, nameLabel, titleLabelConfig, barConfig)
        {
        }

        public new static PartyMembersNameplateConfig DefaultConfig()
        {
            PartyMembersNameplateConfig config = NameplatesHelper.GetNameplateWithBarConfig<PartyMembersNameplateConfig, NameplatePlayerBarConfig>(
                0xFFD0E5E0,
                0xFF000000,
                HUDConstants.DefaultPlayerNameplateBarSize
            );

            config.BarConfig.UseRoleColor = true;
            config.NameLabelConfig.UseRoleColor = true;
            config.TitleLabelConfig.UseRoleColor = true;
            return config;
        }
    }

    [DisableParentSettings("HideWhenInactive")]
    [Section("名牌")]
    [SubSection("团队成员", 0)]
    public class AllianceMembersNameplateConfig : NameplateWithPlayerBarConfig
    {
        public AllianceMembersNameplateConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig,
            NameplatePlayerBarConfig barConfig)
            : base(position, nameLabel, titleLabelConfig, barConfig)
        {
        }

        public new static AllianceMembersNameplateConfig DefaultConfig()
        {
            return NameplatesHelper.GetNameplateWithBarConfig<AllianceMembersNameplateConfig, NameplatePlayerBarConfig>(
                0xFF99BE46,
                0xFF3D4C1C,
                HUDConstants.DefaultPlayerNameplateBarSize
            );
        }
    }

    [DisableParentSettings("HideWhenInactive")]
    [Section("名牌")]
    [SubSection("好友", 0)]
    public class FriendPlayerNameplateConfig : NameplateWithPlayerBarConfig
    {
        public FriendPlayerNameplateConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig,
            NameplatePlayerBarConfig barConfig)
            : base(position, nameLabel, titleLabelConfig, barConfig)
        {
        }

        public new static FriendPlayerNameplateConfig DefaultConfig()
        {
            return NameplatesHelper.GetNameplateWithBarConfig<FriendPlayerNameplateConfig, NameplatePlayerBarConfig>(
                0xFFEB6211,
                0xFF4A2008,
                HUDConstants.DefaultPlayerNameplateBarSize
            );
        }
    }

    [DisableParentSettings("HideWhenInactive")]
    [Section("名牌")]
    [SubSection("其他玩家", 0)]
    public class OtherPlayerNameplateConfig : NameplateWithPlayerBarConfig
    {
        public OtherPlayerNameplateConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig,
            NameplatePlayerBarConfig barConfig)
            : base(position, nameLabel, titleLabelConfig, barConfig)
        {
        }

        public new static OtherPlayerNameplateConfig DefaultConfig()
        {
            return NameplatesHelper.GetNameplateWithBarConfig<OtherPlayerNameplateConfig, NameplatePlayerBarConfig>(
                0xFF91BBD8,
                0xFF33434E,
                HUDConstants.DefaultPlayerNameplateBarSize
            );
        }
    }

    [DisableParentSettings("HideWhenInactive")]
    [Section("名牌")]
    [SubSection("宠物", 0)]
    public class PetNameplateConfig : NameplateWithNPCBarConfig
    {
        public PetNameplateConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig,
            NameplateBarConfig barConfig)
            : base(position, nameLabel, titleLabelConfig, barConfig)
        {
        }

        public new static PetNameplateConfig DefaultConfig()
        {
            PetNameplateConfig config = NameplatesHelper.GetNameplateWithBarConfig<PetNameplateConfig, NameplateBarConfig>(
                0xFFD1E5C8,
                0xFF2A2F28,
                HUDConstants.DefaultPlayerNameplateBarSize
            );
            config.OnlyShowWhenTargeted = true;
            config.SwapLabelsWhenNeeded = false;
            config.NameLabelConfig.Text = "Lv[level] [name]";
            config.NameLabelConfig.FontID = FontsConfig.DefaultSmallFontKey;
            config.TitleLabelConfig.FontID = FontsConfig.DefaultSmallFontKey;

            return config;
        }
    }

    [DisableParentSettings("HideWhenInactive")]
    [Section("名牌")]
    [SubSection("NPC", 0)]
    public class NPCNameplateConfig : NameplateWithNPCBarConfig
    {
        public NPCNameplateConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig,
            NameplateBarConfig barConfig)
            : base(position, nameLabel, titleLabelConfig, barConfig)
        {
        }

        public new static NPCNameplateConfig DefaultConfig()
        {
            NPCNameplateConfig config = NameplatesHelper.GetNameplateWithBarConfig<NPCNameplateConfig, NameplateBarConfig>(
                0xFFD1E5C8,
                0xFF3A4b1E,
                HUDConstants.DefaultPlayerNameplateBarSize
            );
            config.NameLabelConfig.Position = new Vector2(0, -20);
            config.TitleLabelConfig.Position = Vector2.Zero;

            return config;
        }
    }

    [DisableParentSettings("HideWhenInactive", "SwapLabelsWhenNeeded")]
    [Section("名牌")]
    [SubSection("随从", 0)]
    public class MinionNPCNameplateConfig : NameplateConfig
    {
        public MinionNPCNameplateConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig)
            : base(position, nameLabel, titleLabelConfig)
        {
        }

        public new static MinionNPCNameplateConfig DefaultConfig()
        {
            MinionNPCNameplateConfig config = NameplatesHelper.GetNameplateConfig<MinionNPCNameplateConfig>(0xFFFFFFFF, 0xFF000000);
            config.OnlyShowWhenTargeted = true;
            config.SwapLabelsWhenNeeded = false;
            config.NameLabelConfig.Position = new Vector2(0, -17);
            config.NameLabelConfig.FontID = FontsConfig.DefaultSmallFontKey;
            config.TitleLabelConfig.Position = new Vector2(0, 0);
            config.TitleLabelConfig.FontID = FontsConfig.DefaultSmallFontKey;

            return config;
        }
    }

    [DisableParentSettings("HideWhenInactive", "SwapLabelsWhenNeeded")]
    [Section("名牌")]
    [SubSection("物体", 0)]
    public class ObjectsNameplateConfig : NameplateConfig
    {
        public ObjectsNameplateConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig)
            : base(position, nameLabel, titleLabelConfig)
        {
        }

        public new static ObjectsNameplateConfig DefaultConfig()
        {
            ObjectsNameplateConfig config = NameplatesHelper.GetNameplateConfig<ObjectsNameplateConfig>(0xFFFFFFFF, 0xFF000000);
            config.SwapLabelsWhenNeeded = false;

            return config;
        }
    }

    public class NameplateConfig : MovablePluginConfigObject
    {
        [Checkbox("仅在被选中时显示")]
        [Order(1)]
        public bool OnlyShowWhenTargeted = false;

        [Checkbox("当需要时，交换名称和标题标签", spacing = true, help = "这将根据标题在玩家名字之前还是之后来交换这些标签的内容。")]
        [Order(20)]
        public bool SwapLabelsWhenNeeded = true;

        [NestedConfig("名字的标签", 21)]
        public EditableLabelConfig NameLabelConfig = null!;

        [NestedConfig("标题标签", 22)]
        public EditableNonFormattableLabelConfig TitleLabelConfig = null!;

        [NestedConfig("根据范围改变不透明度", 145)]
        public NameplateRangeConfig RangeConfig = new();

        [NestedConfig("可见性", 200)]
        public VisibilityConfig VisibilityConfig = new VisibilityConfig();

        public NameplateConfig(Vector2 position, EditableLabelConfig nameLabelConfig, EditableNonFormattableLabelConfig titleLabelConfig)
            : base()
        {
            Position = position;
            NameLabelConfig = nameLabelConfig;
            TitleLabelConfig = titleLabelConfig;
        }

        public NameplateConfig() : base() { } // don't remove
    }

    public interface NameplateWithBarConfig
    {
        public NameplateBarConfig GetBarConfig();
    }

    public class NameplateWithNPCBarConfig : NameplateConfig, NameplateWithBarConfig
    {
        [NestedConfig("生命条", 40)]
        public NameplateBarConfig BarConfig = null!;

        public NameplateBarConfig GetBarConfig() => BarConfig;

        public NameplateWithNPCBarConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig,
            NameplateBarConfig barConfig)
            : base(position, nameLabel, titleLabelConfig)
        {
            BarConfig = barConfig;
        }

        public NameplateWithNPCBarConfig() : base() { } // don't remove
    }

    public class NameplateWithPlayerBarConfig : NameplateConfig, NameplateWithBarConfig
    {
        [NestedConfig("生命条", 40)]
        public NameplatePlayerBarConfig BarConfig = null!;

        [NestedConfig("职能/职业图标", 50)]
        public NameplateRoleJobIconConfig RoleIconConfig = new NameplateRoleJobIconConfig(
            new Vector2(-5, 0),
            new Vector2(30, 30),
            DrawAnchor.Right,
            DrawAnchor.Left
        )
        { Strata = StrataLevel.LOWEST };

        [NestedConfig("玩家状态图标", 55)]
        public NameplatePlayerIconConfig StateIconConfig = new NameplatePlayerIconConfig(
            new Vector2(5, 0),
            new Vector2(30, 30),
            DrawAnchor.Left,
            DrawAnchor.Right
        )
        { Strata = StrataLevel.LOWEST };

        public NameplateBarConfig GetBarConfig() => BarConfig;

        public NameplateWithPlayerBarConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig,
            NameplatePlayerBarConfig barConfig)
            : base(position, nameLabel, titleLabelConfig)
        {
            BarConfig = barConfig;
        }

        public NameplateWithPlayerBarConfig() : base() { } // don't remove
    }

    public class NameplateWithEnemyBarConfig : NameplateConfig, NameplateWithBarConfig
    {
        [NestedConfig("生命条", 40)]
        public NameplateEnemyBarConfig BarConfig = null!;

        [NestedConfig("图标", 45)]
        public NameplateIconConfig IconConfig = new NameplateIconConfig(
            new Vector2(0, 0),
            new Vector2(40, 40),
            DrawAnchor.Right,
            DrawAnchor.Left
        )
        { PrioritizeHealthBarAnchor = true, Strata = StrataLevel.LOWEST };

        [NestedConfig("减益效果", 50)]
        public EnemyNameplateStatusEffectsListConfig DebuffsConfig = null!;

        [NestedConfig("咏唱栏", 55)]
        public NameplateCastbarConfig CastbarConfig = null!;

        public NameplateBarConfig GetBarConfig() => BarConfig;

        public NameplateWithEnemyBarConfig(
            Vector2 position,
            EditableLabelConfig nameLabel,
            EditableNonFormattableLabelConfig titleLabelConfig,
            NameplateEnemyBarConfig barConfig)
            : base(position, nameLabel, titleLabelConfig)
        {
            BarConfig = barConfig;
        }

        public NameplateWithEnemyBarConfig() : base() { } // don't remove
    }

    [DisableParentSettings("HideWhenInactive")]
    public class NameplateBarConfig : BarConfig
    {
        [Checkbox("仅在生命值未满时显示")]
        [Order(1)]
        public bool OnlyShowWhenNotFull = true;

        [Checkbox("生命值耗尽时隐藏生命条", help = "当角色生命值降为0时，这将隐藏生命条")]
        [Order(2)]
        public bool HideHealthAtZero = true;

        [Checkbox("禁用交互")]
        [Order(3)]
        public bool DisableInteraction = false;

        [Checkbox("被选中后使用不同尺寸", spacing = true)]
        [Order(31)]
        public bool UseDifferentSizeWhenTargeted = false;

        [DragInt2("被选中时的尺寸", min = 1, max = 4000)]
        [Order(32, collapseWith = nameof(UseDifferentSizeWhenTargeted))]
        public Vector2 SizeWhenTargeted;

        [ColorEdit4("被选中边框颜色")]
        [Order(38, collapseWith = nameof(DrawBorder))]
        public PluginConfigColor TargetedBorderColor = PluginConfigColor.FromHex(0xFFFFFFFF);

        [DragInt("被选中边框厚度", min = 1, max = 10)]
        [Order(39, collapseWith = nameof(DrawBorder))]
        public int TargetedBorderThickness = 2;

        [NestedConfig("基于生命值的颜色", 50, collapsingHeader = false)]
        public ColorByHealthValueConfig ColorByHealth = new ColorByHealthValueConfig();

        [Checkbox("尽可能隐藏生命值", spacing = true, help = "如果角色没有生命值，这将隐藏任何带有生命值标签的标签(如随从，友好npc等)。")]
        [Order(121)]
        public bool HideHealthIfPossible = true;

        [NestedConfig("左侧文本", 125)]
        public EditableLabelConfig LeftLabelConfig = null!;

        [NestedConfig("右侧文本", 130)]
        public EditableLabelConfig RightLabelConfig = null!;

        [NestedConfig("可选文本", 131)]
        public EditableLabelConfig OptionalLabelConfig = null!;

        [NestedConfig("盾", 140)]
        public ShieldConfig ShieldConfig = new ShieldConfig();

        [NestedConfig("自定义鼠标悬停区域", 150)]
        public MouseoverAreaConfig MouseoverAreaConfig = new MouseoverAreaConfig();

        public NameplateBarConfig(Vector2 position, Vector2 size, EditableLabelConfig leftLabelConfig, EditableLabelConfig rightLabelConfig, EditableLabelConfig optionalLabelConfig)
            : base(position, size, new PluginConfigColor(new(40f / 255f, 40f / 255f, 40f / 255f, 100f / 100f)))
        {
            Position = position;
            Size = size;
            LeftLabelConfig = leftLabelConfig;
            RightLabelConfig = rightLabelConfig;
            OptionalLabelConfig = optionalLabelConfig;
            BackgroundColor = new PluginConfigColor(new(0f / 255f, 0f / 255f, 0f / 255f, 100f / 100f));
            ColorByHealth.Enabled = false;
            MouseoverAreaConfig.Enabled = false;
        }

        public bool IsVisible(uint hp, uint maxHp)
        {
            return Enabled && (!OnlyShowWhenNotFull || hp < maxHp) && !(HideHealthAtZero && hp <= 0);
        }

        public Vector2 GetSize(bool targeted)
        {
            return targeted && UseDifferentSizeWhenTargeted ? SizeWhenTargeted : Size;
        }

        public NameplateBarConfig() : base(Vector2.Zero, Vector2.Zero, new(Vector4.Zero)) { } // don't remove
    }

    public class NameplatePlayerBarConfig : NameplateBarConfig
    {
        [Checkbox("使用职业颜色", spacing = true)]
        [Order(45)]
        public bool UseJobColor = false;

        [Checkbox("使用职能颜色")]
        [Order(46)]
        public bool UseRoleColor = false;

        [Checkbox("使用职业颜色作为背景色", spacing = true)]
        [Order(50)]
        public bool UseJobColorAsBackgroundColor = false;

        [Checkbox("使用职能颜色作为背景色")]
        [Order(51)]
        public bool UseRoleColorAsBackgroundColor = false;

        public NameplatePlayerBarConfig(Vector2 position, Vector2 size, EditableLabelConfig leftLabelConfig, EditableLabelConfig rightLabelConfig, EditableLabelConfig optionalLabelConfig)
            : base(position, size, leftLabelConfig, rightLabelConfig, optionalLabelConfig)
        {
        }
    }

    public class NameplateEnemyBarConfig : NameplateBarConfig
    {
        [Checkbox("使用状态颜色", spacing = true)]
        [Order(45)]
        public bool UseStateColor = true;

        [ColorEdit4("脱战")]
        [Order(46, collapseWith = nameof(UseStateColor))]
        public PluginConfigColor OutOfCombatColor = PluginConfigColor.FromHex(0xFFDA9D2E);

        [ColorEdit4("脱战（敌对）")]
        [Order(47, collapseWith = nameof(UseStateColor))]
        public PluginConfigColor OutOfCombatHostileColor = PluginConfigColor.FromHex(0xFF994B35);

        [ColorEdit4("战斗中")]
        [Order(48, collapseWith = nameof(UseStateColor))]
        public PluginConfigColor InCombatColor = PluginConfigColor.FromHex(0xFF993535);

        [Checkbox("当被选中时使用自定义颜色", spacing = true, help = "当敌人选中玩家时，这将改变条的颜色。")]
        [Order(49)]
        public bool UseCustomColorWhenBeingTargeted = false;

        [ColorEdit4("当被选中时使用自定义颜色")]
        [Order(50, collapseWith = nameof(UseCustomColorWhenBeingTargeted))]
        public PluginConfigColor CustomColorWhenBeingTargeted = PluginConfigColor.FromHex(0xFFC4216D);

        [NestedConfig("目标标签", 132)]
        public DefaultFontLabelConfig OrderLabelConfig = new DefaultFontLabelConfig(new Vector2(5, 0), "", DrawAnchor.Right, DrawAnchor.Left)
        {
            Strata = StrataLevel.LOWEST
        };

        public NameplateEnemyBarConfig(Vector2 position, Vector2 size, EditableLabelConfig leftLabelConfig, EditableLabelConfig rightLabelConfig, EditableLabelConfig optionalLabelConfig)
            : base(position, size, leftLabelConfig, rightLabelConfig, optionalLabelConfig)
        {

        }
    }

    [Exportable(false)]
    public class NameplateRangeConfig : PluginConfigObject
    {
        [DragInt("淡出起始距离(yalms)", min = 1, max = 500)]
        [Order(5)]
        public int StartRange = 50;

        [DragInt("淡出结束距离(yalms)", min = 1, max = 500)]
        [Order(10)]
        public int EndRange = 64;

        public float AlphaForDistance(float distance, float maxAlpha = 1f)
        {
            float diff = distance - StartRange;
            if (!Enabled || diff <= 0)
            {
                return maxAlpha;
            }

            float a = diff / (EndRange - StartRange);
            return Math.Max(0, Math.Min(maxAlpha, 1 - a));
        }
    }

    public class EnemyNameplateStatusEffectsListConfig : StatusEffectsListConfig
    {
        [Anchor("生命条锚")]
        [Order(4)]
        public DrawAnchor HealthBarAnchor = DrawAnchor.BottomLeft;

        public EnemyNameplateStatusEffectsListConfig(DrawAnchor anchor, Vector2 position, Vector2 size, bool showBuffs, bool showDebuffs, bool showPermanentEffects,
            GrowthDirections growthDirections, StatusEffectIconConfig iconConfig)
            : base(position, size, showBuffs, showDebuffs, showPermanentEffects, growthDirections, iconConfig)
        {
            HealthBarAnchor = anchor;
        }
    }

    [DisableParentSettings("AnchorToUnitFrame", "UnitFrameAnchor", "HideWhenInactive", "FillDirection")]
    public class NameplateCastbarConfig : TargetCastbarConfig
    {
        [Checkbox("宽度与生命条匹配")]
        [Order(11)]
        public bool MatchWidth = false;

        [Checkbox("高度与生命条匹配")]
        [Order(12)]
        public bool MatchHeight = false;

        [Anchor("生命条锚")]
        [Order(16)]
        public DrawAnchor HealthBarAnchor = DrawAnchor.BottomLeft;

        public NameplateCastbarConfig(Vector2 position, Vector2 size, LabelConfig castNameConfig, NumericLabelConfig castTimeConfig)
            : base(position, size, castNameConfig, castTimeConfig)
        {

        }
    }

    internal static class NameplatesHelper
    {
        internal static T GetNameplateConfig<T>(uint bgColor, uint borderColor) where T : NameplateConfig
        {
            EditableLabelConfig nameLabelConfig = new EditableLabelConfig(new Vector2(0, 0), "[name]", DrawAnchor.Top, DrawAnchor.Bottom)
            {
                Color = PluginConfigColor.FromHex(bgColor),
                OutlineColor = PluginConfigColor.FromHex(borderColor),
                FontID = FontsConfig.DefaultMediumFontKey
            };

            EditableNonFormattableLabelConfig titleLabelConfig = new EditableNonFormattableLabelConfig(new Vector2(0, -25), "<[title]>", DrawAnchor.Top, DrawAnchor.Bottom)
            {
                Color = PluginConfigColor.FromHex(bgColor),
                OutlineColor = PluginConfigColor.FromHex(borderColor),
                FontID = FontsConfig.DefaultMediumFontKey
            };

            return (T)Activator.CreateInstance(typeof(T), Vector2.Zero, nameLabelConfig, titleLabelConfig)!;
        }

        internal static T GetNameplateWithBarConfig<T, B>(uint bgColor, uint borderColor, Vector2 barSize)
            where T : NameplateConfig
            where B : NameplateBarConfig
        {
            EditableLabelConfig leftLabelConfig = new EditableLabelConfig(new Vector2(5, 0), "[health:current-short]", DrawAnchor.Left, DrawAnchor.Left)
            {
                Enabled = false,
                FontID = FontsConfig.DefaultMediumFontKey,
                Strata = StrataLevel.LOWEST
            };
            EditableLabelConfig rightLabelConfig = new EditableLabelConfig(new Vector2(-5, 0), "", DrawAnchor.Right, DrawAnchor.Right)
            {
                Enabled = false,
                FontID = FontsConfig.DefaultMediumFontKey,
                Strata = StrataLevel.LOWEST
            };
            EditableLabelConfig optionalLabelConfig = new EditableLabelConfig(new Vector2(0, 0), "", DrawAnchor.Center, DrawAnchor.Center)
            {
                Enabled = false,
                FontID = FontsConfig.DefaultSmallFontKey,
                Strata = StrataLevel.LOWEST
            };

            var barConfig = Activator.CreateInstance(typeof(B), new Vector2(0, -5), barSize, leftLabelConfig, rightLabelConfig, optionalLabelConfig)!;
            if (barConfig is BarConfig bar)
            {
                bar.FillColor = PluginConfigColor.FromHex(bgColor);
                bar.BackgroundColor = PluginConfigColor.FromHex(0xAA000000);
            }

            if (barConfig is NameplateBarConfig nameplateBar)
            {
                nameplateBar.SizeWhenTargeted = nameplateBar.Size;
            }

            EditableLabelConfig nameLabelConfig = new EditableLabelConfig(new Vector2(0, -20), "[name]", DrawAnchor.Top, DrawAnchor.Bottom)
            {
                Color = PluginConfigColor.FromHex(bgColor),
                OutlineColor = PluginConfigColor.FromHex(borderColor),
                FontID = FontsConfig.DefaultMediumFontKey,
                Strata = StrataLevel.LOWEST
            };
            EditableNonFormattableLabelConfig titleLabelConfig = new EditableNonFormattableLabelConfig(new Vector2(0, 0), "<[title]>", DrawAnchor.Top, DrawAnchor.Bottom)
            {
                Color = PluginConfigColor.FromHex(bgColor),
                OutlineColor = PluginConfigColor.FromHex(borderColor),
                FontID = FontsConfig.DefaultMediumFontKey,
                Strata = StrataLevel.LOWEST
            };

            return (T)Activator.CreateInstance(typeof(T), Vector2.Zero, nameLabelConfig, titleLabelConfig, barConfig)!;
        }
    }
}
