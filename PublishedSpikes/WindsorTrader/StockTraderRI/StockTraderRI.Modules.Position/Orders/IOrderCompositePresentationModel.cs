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

using System;
using System.Windows.Input;
using StockTraderRI.Infrastructure;
using StockTraderRI.Modules.Position.Interfaces;

namespace StockTraderRI.Modules.Position.Orders
{
    public interface IOrderCompositePresentationModel
    {
        void SetTransactionInfo(string tickerSymbol, TransactionType transactionType);
        event EventHandler CloseViewRequested;
        IOrderCompositeView View { get; }

        ICommand SubmitCommand { get; }
        ICommand CancelCommand { get; }
    }
}