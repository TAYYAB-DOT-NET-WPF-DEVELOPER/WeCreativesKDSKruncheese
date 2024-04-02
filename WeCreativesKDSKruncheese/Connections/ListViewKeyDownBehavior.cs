using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace WeCreatives_KDSPJ.Connections
{
    public static class ListViewKeyDownBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ListViewKeyDownBehavior), new PropertyMetadata(null, OnCommandChanged));

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView listView)
            {
                if (e.NewValue is ICommand command)
                {
                    listView.PreviewKeyDown += (sender, args) =>
                    {
                        if (command.CanExecute(args.Key))
                        {
                            command.Execute(args.Key);
                        }
                    };
                }
            }
        }
    }
    }
