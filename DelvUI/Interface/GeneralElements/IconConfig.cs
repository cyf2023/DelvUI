using Dalamud.Game.ClientState.Objects.Types;
using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Enums;
using DelvUI.Helpers;
using DelvUI.Interface.Party;
using System;
using System.Numerics;

namespace DelvUI.Interface.GeneralElements
{
    public class IconConfig : AnchorablePluginConfigObject
    {
        [Anchor("界面锚")]
        [Order(16)]
        public DrawAnchor FrameAnchor = DrawAnchor.Center;

        // don't remove (used by json converter)
        public IconConfig()
        {
            Strata = StrataLevel.MID_HIGH;
        }

        public IconConfig(Vector2 position, Vector2 size, DrawAnchor anchor, DrawAnchor frameAnchor)
        {
            Position = position;
            Size = size;
            Anchor = anchor;
            FrameAnchor = frameAnchor;

            Strata = StrataLevel.MID_HIGH;
        }
    }

    public class IconWithLabelConfig : IconConfig
    {
        [NestedConfig("标签", 20)]
        public NumericLabelConfig NumericLabel = new NumericLabelConfig(Vector2.Zero, "", DrawAnchor.Center, DrawAnchor.Center);

        public IconWithLabelConfig(Vector2 position, Vector2 size, DrawAnchor anchor, DrawAnchor frameAnchor)
            : base(position, size, anchor, frameAnchor)
        {
        }
    }

    public class RoleJobIconConfig : IconConfig
    {
        public RoleJobIconConfig() : base() { }

        public RoleJobIconConfig(Vector2 position, Vector2 size, DrawAnchor anchor, DrawAnchor frameAnchor)
            : base(position, size, anchor, frameAnchor)
        {
        }

        [Combo("风格", "风格1", "风格2", "风格3", spacing = true)]
        [Order(25)]
        public int Style = 0;

        [Checkbox("使用职能图标", spacing = true)]
        [Order(30)]
        public bool UseRoleIcons = false;

        [Checkbox("使用特定的DPS职能图标")]
        [Order(35, collapseWith = nameof(UseRoleIcons))]
        public bool UseSpecificDPSRoleIcons = false;
    }

    public class SignIconConfig : IconConfig
    {
        public SignIconConfig() : base() { }

        public SignIconConfig(Vector2 position, Vector2 size, DrawAnchor anchor, DrawAnchor frameAnchor)
            : base(position, size, anchor, frameAnchor)
        {
        }

        [Checkbox("预览")]
        [Order(35)]
        public bool Preview = false;

        public uint? IconID(GameObject? actor)
        {
            if (Preview)
            {
                return 61231;
            }

            return Utils.SignIconIDForActor(actor);
        }
    }

    public class NameplateIconConfig : IconConfig
    {
        public NameplateIconConfig() : base() { }

        public NameplateIconConfig(Vector2 position, Vector2 size, DrawAnchor anchor, DrawAnchor frameAnchor)
            : base(position, size, anchor, frameAnchor)
        {
        }

        [Combo("名牌标签", new string[] { "Name", "Title", "Highest", "Lowest" }, spacing = true)]
        [Order(17)]
        public NameplateLabelAnchor NameplateLabelAnchor = NameplateLabelAnchor.Name;

        [Checkbox("当生命条可见时优先作为锚", help = "启用后，如果生命条可见，图标将锚定在生命条上。\n如果生命条消失，它将锚定回所需的标签。")]
        [Order(18)]
        public bool PrioritizeHealthBarAnchor = false;
    }

    public class NameplatePlayerIconConfig : NameplateIconConfig
    {
        public NameplatePlayerIconConfig() : base() { }

        public NameplatePlayerIconConfig(Vector2 position, Vector2 size, DrawAnchor anchor, DrawAnchor frameAnchor)
            : base(position, size, anchor, frameAnchor)
        {
        }

        [Checkbox("只显示断开连接图标", spacing = true)]
        [Order(19)]
        public bool OnlyShowDisconnected = false;

        public bool ShouldDrawIcon(int iconId)
        {
            if (!OnlyShowDisconnected) { return true; }

            return (iconId >= 61503 && iconId <= 61505) ||
                   (iconId >= 61553 && iconId <= 61555);
        }
    }

    public class NameplateRoleJobIconConfig : RoleJobIconConfig
    {
        public NameplateRoleJobIconConfig() : base() { }

        public NameplateRoleJobIconConfig(Vector2 position, Vector2 size, DrawAnchor anchor, DrawAnchor frameAnchor)
            : base(position, size, anchor, frameAnchor)
        {
        }

        [Combo("名牌标签", new string[] { "名字", "标题", "最高", "最低" }, spacing = true)]
        [Order(17)]
        public NameplateLabelAnchor NameplateLabelAnchor = NameplateLabelAnchor.Name;

        [Checkbox("当生命条可见时优先作为锚", help = "启用后，如果生命条可见，图标将锚定在生命条上。\n如果生命条消失，它将锚定回所需的标签。")]
        [Order(18)]
        public bool PrioritizeHealthBarAnchor = false;
    }

    public class PartyFramesIconsConverter : PluginConfigObjectConverter
    {
        public PartyFramesIconsConverter()
        {
            SameTypeFieldConverter<DrawAnchor> converter = new SameTypeFieldConverter<DrawAnchor>("FrameAnchor", DrawAnchor.Center);
            FieldConvertersMap.Add("HealthBarAnchor", converter);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PartyFramesRoleIconConfig) ||
                   objectType == typeof(PartyFramesLeaderIconConfig);
        }
    }

    public enum NameplateLabelAnchor
    {
        Name = 0,
        Title = 1,
        Highest = 2,
        Lowest = 3
    }
}
