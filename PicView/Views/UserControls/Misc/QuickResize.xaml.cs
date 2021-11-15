﻿using PicView.Animations;
using PicView.Shortcuts;
using PicView.UILogic;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace PicView.Views.UserControls
{
    public partial class QuickResize : UserControl
    {
        public QuickResize()
        {
            InitializeComponent();

            Loaded += delegate
            {
                x2.MouseLeftButtonDown += (_, _) => Hide();
                WidthBox.GotKeyboardFocus += (_, _) => WidthBox.SelectAll();
                HeightBox.GotKeyboardFocus += (_, _) => HeightBox.SelectAll();

                // WidhtBox
                WidthBox.AcceptsReturn = false;
                WidthBox.PreviewKeyDown += async (_, e) => await QuickResizeShortcuts.QuickResizePreviewKeys(e, WidthBox.Text, HeightBox.Text).ConfigureAwait(false);

                // HeightBox
                HeightBox.AcceptsReturn = false;
                HeightBox.PreviewKeyDown += async (_, e) => await QuickResizeShortcuts.QuickResizePreviewKeys(e, WidthBox.Text, HeightBox.Text).ConfigureAwait(false);

                PercentageBox.PreviewKeyDown += (_, e) =>
                {
                    // Prevent alt tab navigation and instead move back to WidthBox
                    if (e.Key == Key.Tab)
                    {
                        e.Handled = true;
                        WidthBox.Focus();
                    }
                };

                WidthBox.KeyUp += (_, e) => QuickResizeShortcuts.QuickResizeAspectRatio(WidthBox, HeightBox, true, e);
                HeightBox.KeyUp += (_, e) => QuickResizeShortcuts.QuickResizeAspectRatio(WidthBox, HeightBox, false, e);

                var colorAnimation = new ColorAnimation { Duration = TimeSpan.FromSeconds(.1) };

                // ApplyButton
                ApplyButton.MouseEnter += delegate
                {
                    colorAnimation.From = AnimationHelper.GetPrefferedColorOver();
                    colorAnimation.To = AnimationHelper.GetPrefferedColorDown();
                    ApplyBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
                };
                ApplyButton.MouseLeave += delegate
                {
                    colorAnimation.From = AnimationHelper.GetPrefferedColorDown();
                    colorAnimation.To = AnimationHelper.GetPrefferedColorOver();
                    ApplyBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
                };

                ApplyButton.MouseLeftButtonDown += async (_, _) => await QuickResizeShortcuts.Fire(WidthBox.Text, HeightBox.Text).ConfigureAwait(false);
            };
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
            Animations.AnimationHelper.Fade(this, TimeSpan.FromSeconds(.4), TimeSpan.Zero, 0, 1);
            WidthBox.Text = ConfigureWindows.GetMainWindow.MainImage.Source?.Width.ToString();
            HeightBox.Text = ConfigureWindows.GetMainWindow.MainImage?.Source?.Height.ToString();

            var timer = new Timer(401) { AutoReset = false, Enabled = true };
            timer.Elapsed += delegate
            {
                ConfigureWindows.GetMainWindow.Dispatcher.Invoke(DispatcherPriority.Render, () =>
                {
                    Keyboard.Focus(WidthBox);
                });
            };
        }

        public void Hide()
        {
            ConfigureWindows.GetMainWindow.Dispatcher.Invoke(DispatcherPriority.Render, () =>
            {
                Animations.AnimationHelper.Fade(this, TimeSpan.FromSeconds(.6), TimeSpan.Zero, 1, 0);
            });

            var timer = new Timer(601) { AutoReset = false, Enabled = true };
            timer.Elapsed += delegate
            {
                ConfigureWindows.GetMainWindow.Dispatcher.Invoke(DispatcherPriority.Render, () =>
                {
                    Visibility = Visibility.Collapsed;
                });
            };
        }
    }
}