﻿using Common.UI.Common;
using Common.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Common.UI.Controls
{
    public class Popup<T> : ContentView
    {
        public View ContentView => PopupContent.Children[0];

        // treat popups as a singleton -- there can be only one
        private static Stack<Popup<T>> _instanceStack;
        public static Popup<T> Instance
        {
            get
            {
                if (_instanceStack == null)
                    _instanceStack = new Stack<Popup<T>>();

                if(_instanceStack.Count == 0)
                {
                    var popup = new Popup<T>();
                    _instanceStack.Push(popup);
                    return popup;
                }
                else
                {
                    return _instanceStack.Peek();
                }
            }
        }

        public Task<T> Result
        {
            get
            {
                return _result.Task;
            }
        }
        private TaskCompletionSource<T> _result;

        public bool LightDismissedEnabled { get; set; }
        protected ICommand OnLightDismissedCommand { get; set; }

        protected T cancelledResult;
        protected bool preventScrollIn;

        protected Grid PopupContent;

        protected Grid border;
        private Grid background;

        public View BorderEdge;

        bool taskCompleted = false;
        bool fadeIn = false;

        public Popup(bool useScrollView = true)
        {
            LightDismissedEnabled = true;
            preventScrollIn = false;

            _result = new TaskCompletionSource<T>();
            background = new Grid()
            {
                InputTransparent = false,
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = Color.FromRgba(0, 0, 0, 200),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                ColumnDefinitions =
                {
                    new ColumnDefinition{Width = 14},
                    new ColumnDefinition{Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition{Width = 14},
                },
                RowDefinitions =
                {
                    new RowDefinition{Height = new GridLength(1, GridUnitType.Star)},
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = new GridLength(1, GridUnitType.Star)},
                }
            };
            background.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(OnTapped)
            });

            if(Device.RuntimePlatform == Device.iOS)
            {
                var blur = new BoxView
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                };
                Grid.SetColumnSpan(blur, 3);
                Grid.SetRowSpan(blur, 3);
                blur.On<Xamarin.Forms.PlatformConfiguration.iOS>().UseBlurEffect(BlurEffectStyle.Dark);
                background.BackgroundColor = Color.Transparent;
                background.Children.Add(blur);
            }

            border = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                BackgroundColor = Color.Transparent,
                InputTransparent = false,
                IsEnabled = true,
                                
                ColumnDefinitions =
                {
                    new ColumnDefinition{Width=10},
                    new ColumnDefinition{Width=GridLength.Auto},
                    new ColumnDefinition{Width=10},
                },
                RowDefinitions =
                {
                    new RowDefinition{Height=10},
                    new RowDefinition{Height=GridLength.Auto},
                    new RowDefinition{Height=10}
                }
            };
            border.SizeChanged += Border_SizeChanged;

            border.LayoutChanged += Border_LayoutChanged;



            if (Device.RuntimePlatform == Device.Android)
            {
                BorderEdge = new Frame
                {
                    InputTransparent = false,
                    BackgroundColor = Color.White,
                    BorderColor = Color.White,
                    CornerRadius = 5,
                    HasShadow = true,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                };
            }
            else
            {
                BorderEdge = new Border
                {
                    InputTransparent = false,
                    Color = Color.White,
                    BorderColor = Color.White,
                    CornerRadius = 5,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                };
            }
            border.Add(BorderEdge, 1,1);

            PopupContent = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                Padding = 0,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
            };
            border.Add(PopupContent, 1, 1);

            border.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {

                })
            });

            

            background.Opacity = 0;
            background.FadeTo(1, 400, Easing.Linear);


            if(useScrollView)
            {
                Content = new Xamarin.Forms.ScrollView
                {
                    Content = background
                };
            }
            else
            {
                Content = new Xamarin.Forms.ContentView
                {
                    Content = background
                };
            }
            

            background.Add(border, 1, 1);
        }

        private void Border_SizeChanged(object sender, EventArgs e)
        {
            Fade_in();
        }

        private void Border_LayoutChanged(object sender, EventArgs e)
        {
            Fade_in();
        }

        private void Fade_in()
        {
            Debug.WriteLine("Fade_in called");
            if (border.Height <= 0 || fadeIn)
                return;

            //if (border.IsEnabled)
            //    return;

            fadeIn = true;

            if (preventScrollIn == false)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    border.TranslationY = -background.Height;
                    border.TranslateTo(0, 0, 400, Easing.Linear).ContinueWith((b) =>
                    {
                        border.IsEnabled = true;
                        OnAppeared();
                    });
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    border.IsEnabled = true;
                    OnAppeared();
                });
            }

            
        }

        public static async Task<T> Launch(
            View view, 
            bool lightDismissEnabled, 
            Xamarin.Forms.Page page, 
            double innerMargin = 10, 
            double outerMargin = 14, 
            ICommand onLightDismissCommand = null,
            bool useScrollView = true)
        {
            var popup = new Popup<T> (useScrollView)
            {
                LightDismissedEnabled = lightDismissEnabled,
            };
            popup.SetContent(view, innerMargin, outerMargin);
            popup.OnLightDismissedCommand = onLightDismissCommand;

            if (_instanceStack == null)
                _instanceStack = new Stack<Popup<T>>();

            // this allows us to create stackable popups without breaking everything else we have
            _instanceStack.Push(popup);

            if(!(page.InternalChildren[0] is Grid))
            {
                throw new Exception("Page is not a Grid. Popup can't show");
            }

            return await popup.Show(page.InternalChildren[0] as Grid);
        }

        public void SetContent(View v, double innerMargin = 10, double outerMargin = 14)
        {
            PopupContent.Children?.Clear();
            PopupContent.Add(v);

            border.ColumnDefinitions[0].Width = innerMargin;
            border.ColumnDefinitions[2].Width = innerMargin;

            background.ColumnDefinitions[0].Width = outerMargin;
            background.ColumnDefinitions[2].Width = outerMargin;

            border.RowDefinitions[0].Height = innerMargin;
            border.RowDefinitions[2].Height = innerMargin;

            // don't set the outer margin on the top and bottom of the background
            // the popup content is vertically centered by default
            // background.RowDefinitions[0].Height = outerMargin;
            // background.RowDefinitions[2].Height = outerMargin;
        }

        protected virtual void OnAppeared() { }

        public async Task<T> Show(Grid parent, ICommand onLightDismissCommand = null)
        {
            OnLightDismissedCommand = onLightDismissCommand;

            parent.Add(this, 0, parent.ColumnDefinitions.Count, 0, parent.RowDefinitions.Count);

            var res = await Result;

            parent.Children.Remove(this);

            return res;
        }

        protected virtual void OnTapped(object sender)
        {
            if (!LightDismissedEnabled)
                return;

            if (OnLightDismissedCommand != null)
            {
                if (OnLightDismissedCommand.CanExecute(sender))
                    OnLightDismissedCommand.Execute(sender);
            }
            else
                ClosePopup(cancelledResult);
        }

        protected ICommand OnCommit { get; set; }


        public void ClosePopup(T result)
        {
            if (taskCompleted)
            {
                if(_instanceStack?.Count > 0)
                    _instanceStack?.Pop();
                return;
            }

            taskCompleted = true;

            background.FadeTo(0, 400, Easing.Linear);

            border.TranslateTo(0, background.Height, 400, Easing.Linear).ContinueWith((b) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    // remove from parent tree
                    _result.SetResult(result);

                    // forcefully remove child view from popup so it will clean itself up (unsubscribe etc.)
                    PopupContent?.Children.Clear();
                });
                return true;
            });

            if(_instanceStack?.Count > 0)
                _instanceStack?.Pop();
        }
    }
}
