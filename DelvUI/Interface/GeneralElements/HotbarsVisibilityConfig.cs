using DelvUI.Config;
using DelvUI.Config.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelvUI.Interface.GeneralElements
{
    [Disableable(false)]
    [Exportable(false)]
    [Section("可见性")]
    [SubSection("快捷栏", 0)]
    public class HotbarsVisibilityConfig : PluginConfigObject
    {
        public new static HotbarsVisibilityConfig DefaultConfig() { return new HotbarsVisibilityConfig(); }

        [NestedConfig("快捷栏1", 50)]
        public VisibilityConfig HotbarConfig1 = new VisibilityConfig();

        [NestedConfig("快捷栏2", 51)]
        public VisibilityConfig HotbarConfig2 = new VisibilityConfig();

        [NestedConfig("快捷栏3", 52)]
        public VisibilityConfig HotbarConfig3 = new VisibilityConfig();

        [NestedConfig("快捷栏4", 53)]
        public VisibilityConfig HotbarConfig4 = new VisibilityConfig();

        [NestedConfig("快捷栏5", 54)]
        public VisibilityConfig HotbarConfig5 = new VisibilityConfig();

        [NestedConfig("快捷栏6", 55)]
        public VisibilityConfig HotbarConfig6 = new VisibilityConfig();

        [NestedConfig("快捷栏7", 56)]
        public VisibilityConfig HotbarConfig7 = new VisibilityConfig();

        [NestedConfig("快捷栏8", 57)]
        public VisibilityConfig HotbarConfig8 = new VisibilityConfig();

        [NestedConfig("快捷栏9", 58)]
        public VisibilityConfig HotbarConfig9 = new VisibilityConfig();

        [NestedConfig("快捷栏10", 59)]
        public VisibilityConfig HotbarConfig10 = new VisibilityConfig();

        [NestedConfig("十字快捷栏", 60)]
        public VisibilityConfig HotbarConfigCross = new VisibilityConfig();

        private List<VisibilityConfig> _configs;
        public List<VisibilityConfig> GetHotbarConfigs() => _configs;

        public HotbarsVisibilityConfig()
        {
            _configs = new List<VisibilityConfig>() {
                HotbarConfig1,
                HotbarConfig2,
                HotbarConfig3,
                HotbarConfig4,
                HotbarConfig5,
                HotbarConfig6,
                HotbarConfig7,
                HotbarConfig8,
                HotbarConfig9,
                HotbarConfig10
            };
        }
    }
}
