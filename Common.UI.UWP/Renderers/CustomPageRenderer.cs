﻿using Windows.UI;
using Windows.UI.ViewManagement;
using Common.UI.Controls;
using Common.UI.UWP.Common;
using Common.UI.UWP.Renderers;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomPage), typeof(CustomPageRenderer))]
namespace Common.UI.UWP.Renderers
{
    public class CustomPageRenderer:PageRenderer
    {
        Color? OriginalButtonBackgroundColor;
        Color? OriginalButtonForegroundColor;
        Color? OriginalBackgroundColor;
        Color? OriginalForegroundColor;
        bool savedTitleBarColors;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            if (e.NewElement != null)
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    var customPage = e.NewElement as CustomPage;
                    if (customPage == null)
                        return;

                    savedTitleBarColors = true;
                    OriginalButtonBackgroundColor = titleBar.ButtonBackgroundColor;
                    OriginalButtonForegroundColor = titleBar.ButtonForegroundColor;
                    OriginalBackgroundColor = titleBar.BackgroundColor;
                    OriginalForegroundColor = titleBar.ForegroundColor;

                    customPage.OnDetached = OnDetached;
                    titleBar.ButtonBackgroundColor = customPage.TitleBarColor.ToNativeColor();
                    titleBar.ButtonForegroundColor = customPage.TitleBarTextColor.ToNativeColor();
                    titleBar.BackgroundColor = customPage.TitleBarColor.ToNativeColor();
                    titleBar.ForegroundColor = customPage.TitleBarTextColor.ToNativeColor();
                }

            }
        }

        void OnDetached()
        {
            if (Element == null || savedTitleBarColors == false)
                return;

            var customPage = Element as CustomPage;
            if (customPage == null)
                return;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null)
            {
                titleBar.ButtonBackgroundColor = OriginalButtonBackgroundColor;
                titleBar.ButtonForegroundColor = OriginalButtonForegroundColor;
                titleBar.BackgroundColor = OriginalBackgroundColor;
                titleBar.ForegroundColor = OriginalForegroundColor;
            }
        }
    }
}
