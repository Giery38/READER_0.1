using READER_0._1.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {
        private readonly Settings settings;
        public SettingsViewModel(Settings settings)
        {
            this.settings = settings;
        }

        public override void Deactivation()
        {
            throw new NotImplementedException();
        }
    }
}
