using System;
using System.Collections.Generic;
using System.Text;

namespace stauff_interface_webshop_spryker_ui.Configuration.Interface {
    public interface ICommonMainConfiguration {
        void Save();
        ICommonMainConfiguration Load();
    }
}
