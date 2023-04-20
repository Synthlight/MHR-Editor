﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using RE_Editor.Windows.Models;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace RE_Editor.Windows {
    // A fuck ton of dynamic because we can't make an xaml window generic.
    public partial class GetNewItemId {
        public dynamic CurrentItem { get; set; }
        public bool    Cancelled   { get; private set; }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public dynamic dataProxy { get; }

        public GetNewItemId(dynamic currentItem, dynamic data, bool showAsHex) {
            CurrentItem = currentItem;
            dataProxy   = ConvertToIdAndNamePair(data, showAsHex);

            InitializeComponent();

            // Center on cursor.
            // ReSharper disable PossibleNullReferenceException
            var transform = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformFromDevice;
            // ReSharper restore PossibleNullReferenceException
            var mouse = transform.Transform(GetMousePosition());
            Left = mouse.X - Width / 2;
            Top  = mouse.Y - 10;

            cbx_current_item.KeyUp  += Cbx_current_item_KeyUp;
            cbx_current_item.Loaded += (_, _) => cbx_current_item.EditableTextBox.Focus(); // Focus & highlight the text.
            btn_ok.Click            += (_, _) => Ok();
            btn_cancel.Click        += (_, _) => Cancel();
        }

        private static Dictionary<T, IdNamePair<T>> ConvertToIdAndNamePair<T>(Dictionary<T, string> data, bool showAsHex) where T : struct {
            var       dict       = new Dictionary<T, IdNamePair<T>>();
            using var enumerator = data.GetEnumerator();
            enumerator.MoveNext();
            var type        = enumerator.Current.Key.GetType();
            var genericType = typeof(IdNamePair<>).MakeGenericType(type);

            foreach (var key in data.Keys) {
                dict[key] = (IdNamePair<T>) Activator.CreateInstance(genericType, key, data[key], showAsHex)!;
            }

            return dict;
        }

        private void Cbx_current_item_KeyUp(object sender, KeyEventArgs e) {
            if (cbx_current_item.IsDropDownOpen) return;
            if (CurrentItem == null) {
                Cancel();
                return;
            }

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (e.Key) {
                case Key.Enter:
                    Ok();
                    break;
                case Key.Escape:
                    Cancel();
                    break;
            }
        }

        private void Ok() {
            Cancelled = false;
            Close();
        }

        private void Cancel() {
            Cancelled = true;
            Close();
        }

        public static Point GetMousePosition() {
            var point = Control.MousePosition;
            return new Point(point.X, point.Y);
        }
    }
}