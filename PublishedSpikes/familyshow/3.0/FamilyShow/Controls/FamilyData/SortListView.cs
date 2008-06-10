//===============================================================================
// Microsoft patterns & practices
// Composite WPF (PRISM)
//===============================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===============================================================================

/*
 * Classes that sorts a ListView control.
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;

namespace Microsoft.FamilyShow
{
    /// <summary>
    /// A column in the SortListView object that allows the SortProperty and 
    /// SortStyle to be specified in XAML. The SortProperty specifies the 
    /// underlying bound property that is used when sorting and SortStyle
    /// specifies the resource that is used for the column header.
    /// </summary>
    public class SortListViewColumn : GridViewColumn
    {
        public string SortProperty
        {
            get { return (string)GetValue(SortPropertyProperty); }
            set { SetValue(SortPropertyProperty, value); }
        }

        public static readonly DependencyProperty SortPropertyProperty =
            DependencyProperty.Register("SortProperty",
            typeof(string), typeof(SortListViewColumn));

        public string SortStyle
        {
            get { return (string)GetValue(SortStyleProperty); }
            set { SetValue(SortStyleProperty, value); }
        }

        public static readonly DependencyProperty SortStyleProperty =
            DependencyProperty.Register("SortStyle",
            typeof(string), typeof(SortListViewColumn));
    }


    /// <summary>
    /// A ListView control that supports sorting.
    /// </summary>
    public class SortListView : ListView
    {
        // The current column that is sorted.
        private SortListViewColumn sortColumn;

        // The previous column that was sorted.
        private SortListViewColumn previousSortColumn;

        // The current direction the header is sorted;
        private ListSortDirection sortDirection;

        protected override void OnInitialized(EventArgs e)
        {
            // Handle the event when a header is clicked.
            this.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnHeaderClicked));
            base.OnInitialized(e);
        }

        /// <summary>
        /// A header was clicked. Sort the associated column.
        /// </summary>
        private void OnHeaderClicked(object sender, RoutedEventArgs e)
        {
            // Make sure the column is really being sorted.
            GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
            if (header == null || header.Role == GridViewColumnHeaderRole.Padding)
                return;

            SortListViewColumn column = header.Column as SortListViewColumn;
            if (column == null)
                return;

            // See if a new column was clicked, or the same column was clicked.
            if (sortColumn != column)
            {
                // A new column was clicked.
                previousSortColumn = sortColumn;
                sortColumn = column;
                sortDirection = ListSortDirection.Ascending;
            }
            else
            {
                // The same column was clicked, change the sort order.
                previousSortColumn = null;
                sortDirection = (sortDirection == ListSortDirection.Ascending) ?
                    ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            // Sort the data.
            SortList(column.SortProperty);

            // Update the column header based on the sort column and order.
            UpdateHeaderTemplate();
        }

        /// <summary>
        /// Sort the data.
        /// </summary>
        private void SortList(string propertyName)
        {
            // Get the data to sort.
            ICollectionView dataView = CollectionViewSource.GetDefaultView(this.ItemsSource);

            // Specify the new sorting information.
            dataView.SortDescriptions.Clear();
            SortDescription description = new SortDescription(propertyName, sortDirection);
            dataView.SortDescriptions.Add(description);

            dataView.Refresh();
        }

        /// <summary>
        /// Update the column header based on the sort column and order.
        /// </summary>
        private void UpdateHeaderTemplate()
        {
            Style headerStyle;

            // Restore the previous header.
            if (previousSortColumn != null && previousSortColumn.SortStyle != null)
            {
                headerStyle = this.TryFindResource(previousSortColumn.SortStyle) as Style;
                if (headerStyle != null)
                    previousSortColumn.HeaderContainerStyle = headerStyle;
            }

            // Update the current header.
            if (sortColumn.SortStyle != null)
            {
                // The name of the resource to use for the header.
                string resourceName = sortColumn.SortStyle + sortDirection.ToString();

                headerStyle = this.TryFindResource(resourceName) as Style;
                if (headerStyle != null)
                    sortColumn.HeaderContainerStyle = headerStyle;
            }
        }
    }
}
