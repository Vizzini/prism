//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace RegionNavigation.Infrastructure.Behaviors
{
    /// <summary>
    /// Base class for trigger actions that handle an interaction request by popping up a child window.
    /// </summary>
    public abstract class PopupChildWindowActionBase : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            if (args == null)
            {
                return;
            }

            var childWindow = this.GetChildWindow(args.Context);

            var callback = args.Callback;
            EventHandler handler = null;
            handler =
                (o, e) =>
                {
                    childWindow.Closed -= handler;
                    callback();
                };
            childWindow.Closed += handler;

            childWindow.Show();
        }

        protected abstract ChildWindow GetChildWindow(object context);
    }
}
