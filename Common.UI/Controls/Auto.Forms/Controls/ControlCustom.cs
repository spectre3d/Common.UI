﻿using System;
using Xamarin.Forms;

namespace Common.UI.Controls.Auto.Forms.Controls
{
    public class ControlCustom : ControlBase
    {
        public ControlCustom() : base (null)
        { }

        public void InitializeCustom(ControlConfig config)
        {
            _config = config;
        }

        protected override View CreateControl(string bindingName, Type fieldType)
        {
            return null;
        }
    }
}
