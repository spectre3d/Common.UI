﻿using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace Common.UI.Controls.Auto.Forms.Controls
{
    public class ControlLabel : ControlBase
    {
        public ControlLabel(ControlConfig config) : base(config)
        {
        }

        protected override View InitializeControl()
        {
            var g = new Grid
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                ColumnDefinitions =
                    {
                        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                    },
                RowDefinitions =
                    {
                        new RowDefinition {Height = GridLength.Auto}
                    }
            };

            var v = _control = CreateControl(BindingName, _propertyType);
            if (v != null)
                g.Children.Add(v);

            return g;
        }

        public override View CreateControlLabel() => null;

        protected override View CreateControl(string bindingName, Type fieldType)
        {
            if (fieldType != typeof(string))
            {
                Debug.WriteLine($"field:{bindingName} error. Wrong type {fieldType.ToString()} should be string");
                return null;
            }

            var l = new Label
            {
                Style = _itemStyle ?? LabelStyle,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Start,
                LineBreakMode = LineBreakMode.WordWrap,
            };

            l.SetBinding(Xamarin.Forms.Label.TextProperty, new Binding(bindingName, BindingMode.TwoWay));

            return l;
        }
    }
}

