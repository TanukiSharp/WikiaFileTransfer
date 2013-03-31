using PresentationToolKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiaFileTransfer.ViewModels
{
    public class RootViewModel : ViewModelBase
    {
        public UploadViewModel Upload { get; private set; }

        public RootViewModel()
        {
            Upload = new UploadViewModel();
        }
    }
}
