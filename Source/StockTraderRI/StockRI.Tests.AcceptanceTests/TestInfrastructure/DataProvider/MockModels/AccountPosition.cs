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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockTraderRI.AcceptanceTests.TestInfrastructure
{
    public class AccountPosition
    {
        public AccountPosition() { }

        public AccountPosition(string tickerSymbol, decimal costBasis, long shares)
        {
            TickerSymbol = tickerSymbol;
            CostBasis = costBasis;
            Shares = shares;
        }

        private string _tickerSymbol;

        public string TickerSymbol
        {
            get
            {
                return _tickerSymbol;
            }
            set
            {
                if (!value.Equals(_tickerSymbol))
                {
                    _tickerSymbol = value;
                }
            }
        }


        private decimal _costBasis;

        public decimal CostBasis
        {
            get
            {
                return _costBasis;
            }
            set
            {
                if (!value.Equals(_costBasis))
                {
                    _costBasis = value;
                }
            }
        }


        private long _shares;

        public long Shares
        {
            get
            {
                return _shares;
            }
            set
            {
                if (!value.Equals(_shares))
                {
                    _shares = value;
                }
            }
        }
    }
}
