using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceWPF.VModel
{
    interface ISettingsViewModel
    {
        string OutputDirectory { get; }
        string SourceName { get; }
        string LogName { get; }
        int ThumbnailSize { get; }
    }
}