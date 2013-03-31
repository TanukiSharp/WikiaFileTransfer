using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WikiaFileTransfer.Behaviors
{
    public class ListBoxSelectionBehavior
    {
        public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.RegisterAttached(
            "IsAttached",
            typeof(bool),
            typeof(ListBoxSelectionBehavior),
            new PropertyMetadata(IsAttachedPropertyChanged));

        public static bool GetIsAttached(DependencyObject target)
        {
            return (bool)target.GetValue(IsAttachedProperty);
        }

        public static void SetIsAttached(DependencyObject target, bool value)
        {
            target.SetValue(IsAttachedProperty, value);
        }

        public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.RegisterAttached(
            "SelectionChangedCommand",
            typeof(ICommand),
            typeof(ListBoxSelectionBehavior));

        public static ICommand GetSelectionChangedCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(SelectionChangedCommandProperty);
        }

        public static void SetSelectionChangedCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(SelectionChangedCommandProperty, value);
        }

        private static void IsAttachedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as ListBox;
            if (control == null)
                return;

            if ((bool)e.OldValue)
                control.SelectionChanged -= control_SelectionChanged;

            if ((bool)e.NewValue)
                control.SelectionChanged += control_SelectionChanged;
        }

        private static void control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = sender as ListBox;
            if (control == null)
                return;

            var command = GetSelectionChangedCommand(control);
            if (command == null)
                return;

            if (command.CanExecute(control.SelectedItems))
                command.Execute(control.SelectedItems);
        }
    }
}
