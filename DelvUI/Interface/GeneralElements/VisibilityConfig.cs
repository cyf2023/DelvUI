using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Interface.GeneralElements;
using DelvUI.Interface.Party;
using System.Linq;

namespace DelvUI.Interface
{
    [Exportable(false)]
    public class VisibilityConfig : PluginConfigObject
    {
        [Checkbox("脱离战斗隐藏")]
        [Order(5)]
        public bool HideOutsideOfCombat = false;

        [Checkbox("进入战斗隐藏")]
        [Order(6)]
        public bool HideInCombat = false;

        [Checkbox("在金碟隐藏")]
        [Order(7)]
        public bool HideInGoldSaucer = false;

        [Checkbox("满血时隐藏")]
        [Order(8)]
        public bool HideOnFullHP = false;

        [Checkbox("处在任务中隐藏")]
        [Order(9)]
        public bool HideInDuty = false;

        [Checkbox("在无人岛时隐藏")]
        [Order(10)]
        public bool HideInIslandSanctuary = false;

        [Checkbox("在PvP中隐藏")]
        [Order(11)]
        public bool HideInPvP = false;

        [Checkbox("在任务中总是显示")]
        [Order(20)]
        public bool ShowInDuty = false;

        [Checkbox("在武器拔出时总是显示")]
        [Order(21)]
        public bool ShowOnWeaponDrawn = false;

        [Checkbox("在制作时总是显示")]
        [Order(22)]
        public bool ShowWhileCrafting = false;

        [Checkbox("在采集时总是显示")]
        [Order(23)]
        public bool ShowWhileGathering = false;

        [Checkbox("组队时总是显示")]
        [Order(24)]
        public bool ShowInParty = false;

        [Checkbox("在无人岛时总是显示")]
        [Order(25)]
        public bool ShowInIslandSanctuary = false;

        [Checkbox("在PvP中总是显示")]
        [Order(26)]
        public bool ShowInPvP = false;


        private bool IsInCombat() => Plugin.Condition[ConditionFlag.InCombat];

        private bool IsInDuty() => Plugin.Condition[ConditionFlag.BoundByDuty];

        private bool IsCrafting() => Plugin.Condition[ConditionFlag.Crafting] || Plugin.Condition[ConditionFlag.Crafting40];

        private bool IsGathering() => Plugin.Condition[ConditionFlag.Gathering] || Plugin.Condition[ConditionFlag.Gathering42];

        private bool HasWeaponDrawn() => (Plugin.ClientState.LocalPlayer != null && Plugin.ClientState.LocalPlayer.StatusFlags.HasFlag(StatusFlags.WeaponOut));

        private bool IsInGoldSaucer() => _goldSaucerIDs.Any(id => id == Plugin.ClientState.TerritoryType);

        private bool IsInIslandSanctuary() => Plugin.ClientState.TerritoryType == 1055;

        private readonly uint[] _goldSaucerIDs = { 144, 388, 389, 390, 391, 579, 792, 899, 941 };

        public bool IsElementVisible(HudElement? element = null)
        {
            if (!Enabled) { return true; }
            if (!ConfigurationManager.Instance.LockHUD) { return true; }
            if (element != null && element.GetType() == typeof(PlayerCastbarHud)) { return true; }
            if (element != null && !element.GetConfig().Enabled) { return false; }

            bool isInIslandSanctuary = IsInIslandSanctuary();
            bool isInDuty = IsInDuty() && !isInIslandSanctuary;

            if (ShowInDuty && isInDuty) { return true; }

            if (ShowOnWeaponDrawn && HasWeaponDrawn()) { return true; }

            if (ShowWhileCrafting && IsCrafting()) { return true; }

            if (ShowWhileGathering && IsGathering()) { return true; }

            if (ShowInParty && PartyManager.Instance.MemberCount > 1) { return true; }

            if (ShowInIslandSanctuary && isInIslandSanctuary) { return true; }

            if (ShowInPvP && Plugin.ClientState.IsPvP) { return true; }


            if (HideOutsideOfCombat && !IsInCombat()) { return false; }

            if (HideInCombat && IsInCombat()) { return false; }

            if (HideInGoldSaucer && IsInGoldSaucer()) { return false; }

            PlayerCharacter? player = Plugin.ClientState.LocalPlayer;
            if (HideOnFullHP && player != null && player.CurrentHp == player.MaxHp) { return false; }

            if (HideInDuty && isInDuty) { return false; }

            if (HideInIslandSanctuary && isInIslandSanctuary) { return false; }

            if (HideInPvP && Plugin.ClientState.IsPvP) { return false; }

            return true;
        }

        public void CopyFrom(VisibilityConfig config)
        {
            Enabled = config.Enabled;

            HideOutsideOfCombat = config.HideOutsideOfCombat;
            HideInGoldSaucer = config.HideInGoldSaucer;
            HideOnFullHP = config.HideOnFullHP;
            HideInDuty = config.HideInDuty;
            HideInIslandSanctuary = config.HideInIslandSanctuary;

            ShowInDuty = config.ShowInDuty;
            ShowOnWeaponDrawn = config.ShowOnWeaponDrawn;
            ShowWhileCrafting = config.ShowWhileCrafting;
            ShowWhileGathering = config.ShowWhileGathering;
            ShowInParty = config.ShowInParty;
            ShowInIslandSanctuary = config.ShowInIslandSanctuary;
        }

        public VisibilityConfig()
        {
            Enabled = false;
        }
    }
}


