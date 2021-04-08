﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Controls.Auto.Forms
{
    public enum AutoFormsType
    {
        Entry,
        Combo,
        Checkbox,
        Radio,
        ActionList,
        Button,
        DateTime,
        Custom,
        Group,
        Label,
        Auto, // pick a control based on it's property type
        SelectButton,
    }
}
