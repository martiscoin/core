using System;
using System.Collections.Generic;
using System.Text;

namespace Martiscoin.Interfaces.UI
{
    public interface INavigationItem
    {
        public string Name { get; }

        public string Navigation { get; }

        public string Icon { get; }

        public bool IsVisible { get; }

        public int NavOrder { get; }
    }
}