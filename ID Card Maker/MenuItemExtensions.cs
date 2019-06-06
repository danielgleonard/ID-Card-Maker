using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ID_Card_Maker
{
    /// <summary>
    /// Extension of the <code>MenuItem</code> class
    /// </summary>
    /// <remarks>
    /// Code taken from
    /// https://stackoverflow.com/a/3652980
    /// by StackOverflow user Patrick
    /// </remarks>
    public class MenuItemExtensions : DependencyObject
    {
        public static Dictionary<MenuItem, String> ElementToGroupNames = new Dictionary<MenuItem, String>();
        public static Dictionary<MenuItem, CardPreview.Design> ElementToDesigns = new Dictionary<MenuItem, CardPreview.Design>();

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.RegisterAttached("GroupName",
                                         typeof(String),
                                         typeof(MenuItemExtensions),
                                         new PropertyMetadata(String.Empty, OnGroupNameChanged));

        public static readonly DependencyProperty DesignProperty =
            DependencyProperty.RegisterAttached("Design",
                                         typeof(CardPreview.Design),
                                         typeof(MenuItemExtensions),
                                         new PropertyMetadata(null, OnDesignChanged));

        public static void SetDesign(MenuItem element, CardPreview.Design design)
        {
            element.SetValue(DesignProperty, design);
        }
        public static String GetDesign(MenuItem element)
        {
            return element.GetValue(DesignProperty).ToString();
        }

        public static void SetGroupName(MenuItem element, String value)
        {
            element.SetValue(GroupNameProperty, value);
        }

        public static String GetGroupName(MenuItem element)
        {
            return element.GetValue(GroupNameProperty).ToString();
        }

        private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Add an entry to the group name collection
            var menuItem = d as MenuItem;

            if (menuItem != null)
            {
                String newGroupName = e.NewValue.ToString();
                String oldGroupName = e.OldValue.ToString();
                if (String.IsNullOrEmpty(newGroupName))
                {
                    //Removing the toggle button from grouping
                    RemoveCheckboxFromGrouping(menuItem);
                }
                else
                {
                    //Switching to a new group
                    if (newGroupName != oldGroupName)
                    {
                        if (!String.IsNullOrEmpty(oldGroupName))
                        {
                            //Remove the old group mapping
                            RemoveCheckboxFromGrouping(menuItem);
                        }
                        ElementToGroupNames.Add(menuItem, e.NewValue.ToString());
                        menuItem.Click += MenuItemClicked;
                    }
                }
            }
        }
        private static void OnDesignChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Add an entry to the group name collection
            var menuItem = d as MenuItem;

            if (menuItem != null)
            {
                String newGroupName = e.NewValue.ToString();
                String oldGroupName = e.OldValue.ToString();
                if (String.IsNullOrEmpty(newGroupName))
                {
                    //Removing the toggle button from grouping
                    RemoveCheckboxFromGrouping(menuItem);
                }
                else
                {
                    //Switching to a new group
                    if (newGroupName != oldGroupName)
                    {
                        if (!String.IsNullOrEmpty(oldGroupName))
                        {
                            //Remove the old group mapping
                            RemoveCheckboxFromGrouping(menuItem);
                        }
                        ElementToGroupNames.Add(menuItem, e.NewValue.ToString());
                        menuItem.Click += MenuItemClicked;
                    }
                }
            }
        }

        private static void RemoveCheckboxFromGrouping(MenuItem checkBox)
        {
            ElementToGroupNames.Remove(checkBox);
            checkBox.Click -= MenuItemClicked;
        }

        /// <summary>
        /// Handle click event of <code>MenuItem</code>
        /// </summary>
        /// <remarks>
        /// Modification to <code>MenuItemExtensions</code> class from
        /// https://stackoverflow.com/a/18643222
        /// by StackOverflow user MK10
        /// </remarks>
        static void MenuItemClicked(object sender, RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as MenuItem;
            if (menuItem.IsChecked)
            {
                foreach (var item in ElementToGroupNames)
                {
                    if (item.Key != menuItem && item.Value == GetGroupName(menuItem))
                    {
                        item.Key.IsChecked = false;
                    }
                }
            }
            else // it's not possible for the user to deselect an item
            {
                menuItem.IsChecked = true;
            }
        }
    }
}
