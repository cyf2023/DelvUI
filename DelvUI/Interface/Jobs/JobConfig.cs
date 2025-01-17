﻿using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Helpers;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace DelvUI.Interface.Jobs
{
    public abstract class JobConfig : MovablePluginConfigObject
    {
        [JsonIgnore]
        public abstract uint JobId { get; }

        [Checkbox("显示通用法力条")]
        [Order(20)]
        public bool UseDefaultPrimaryResourceBar = false;

        [NestedConfig("可见性", 2000)]
        public VisibilityConfig VisibilityConfig = new VisibilityConfig();

        [JsonIgnore]
        public PrimaryResourceTypes PrimaryResourceType = PrimaryResourceTypes.MP;

        public new static JobConfig? DefaultConfig()
        {
            var type = MethodBase.GetCurrentMethod()?.DeclaringType;
            if (type is null)
            {
                return null;
            }

            return (JobConfig?)Activator.CreateInstance(type);
        }

        public JobConfig()
        {
            Position.Y = HUDConstants.JobHudsBaseY;
        }
    }
}
