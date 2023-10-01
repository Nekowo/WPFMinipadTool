﻿using MinipadWPFTest.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MinipadWPFTest.Controls
{
    public class HotKeyTextBox : TextBox
    {
        public static readonly DependencyProperty HotKeyProperty = DependencyProperty.Register(
            nameof(HotKey),
            typeof(HotKey),
            typeof(HotKeyTextBox),
            new FrameworkPropertyMetadata(
                default(HotKey),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (sender, _) =>
                {
                    var control = (HotKeyTextBox)sender;
                    control.Text = control.HotKey.ToString();
                })
        );

        public HotKey HotKey
        {
            get => (HotKey)GetValue(HotKeyProperty);
            set => SetValue(HotKeyProperty, value);
        }

        public HotKeyTextBox()
        {
            IsReadOnly = true;
            IsReadOnlyCaretVisible = false;
            IsUndoEnabled = false;

            if (ContextMenu is not null)
                ContextMenu.Visibility = Visibility.Collapsed;

            Text = HotKey.ToString();
        }

        private static bool HasKeyChar(Key key) => key is
            // A - Z
            >= Key.A and <= Key.Z or
            // 0 - 9
            >= Key.D0 and <= Key.D9 or
            // Numpad 0 - 9
            >= Key.NumPad0 and <= Key.NumPad9 or
            // The rest
            Key.OemQuestion or Key.OemQuotes or Key.OemPlus or Key.OemOpenBrackets or Key.OemCloseBrackets or
            Key.OemMinus or Key.DeadCharProcessed or Key.Oem1 or Key.Oem5 or Key.Oem7 or Key.OemPeriod or
            Key.OemComma or Key.Add or Key.Divide or Key.Multiply or Key.Subtract or Key.Oem102 or Key.Decimal;

        protected override void OnPreviewKeyDown(KeyEventArgs args)
        {
            args.Handled = true;

            // Get modifiers and key data
            var modifiers = Keyboard.Modifiers;
            var key = args.Key;

            // If nothing was pressed - return
            if (key == Key.None)
                return;

            // If Alt is used as modifier - the key needs to be extracted from SystemKey
            if (key == Key.System)
                key = args.SystemKey;

            if (key is Key.Escape)
            {
                Keyboard.ClearFocus();
                return;
            }

            // If Delete/Backspace/Escape is pressed without modifiers - clear current value and return
            if (key is Key.Delete or Key.Back or Key.Escape && modifiers == ModifierKeys.None)
            {
                HotKey = HotKey.None;
                return;
            }

            // If the only key pressed is one of the modifier keys - return
            if (key is
                Key.LeftCtrl or Key.RightCtrl or Key.LeftAlt or Key.RightAlt or
                Key.LeftShift or Key.RightShift or Key.LWin or Key.RWin or
                Key.Clear or Key.OemClear or Key.Apps)
                return;

            // Set value
            //HotKey = new HotKey(key, modifiers);
            HotKey = new HotKey(key);
        }
    }
}
