﻿using System;
using InterfacesiOS.Helpers;
using UIKit;
using Vx.Views;

namespace Vx.iOS.Views
{
    public class iOSBorder : iOSView<Vx.Views.Border, UIView>
    {
        public iOSBorder()
        {
        }

        protected override void ApplyProperties(Border oldView, Border newView)
        {
            base.ApplyProperties(oldView, newView);

            View.BackgroundColor = newView.BackgroundColor.ToUI();
            View.Layer.BorderWidth = newView.BorderThickness.Top;
            View.Layer.BorderColor = newView.BorderColor.ToUI().CGColor;

            // Incorporate padding with child's margin
            var paddingPlusMargin = newView.Padding;
            if (newView != null)
            {
                paddingPlusMargin = paddingPlusMargin.Combine(newView.Padding);
            }

            ReconcileContent(oldView?.Content, newView.Content, overriddenChildMargin: paddingPlusMargin);
        }
    }
}