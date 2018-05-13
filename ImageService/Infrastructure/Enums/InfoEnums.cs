using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure.Enums
{
    /// <summary>
    /// set of enums for possible info that can be sent to the GUI from the service.
    /// </summary>
    public enum InfoEnums : int
    {
        NotInfo = 0,
        LogInfo = 1,
        LogHistoryInfo = 2,
        AppConfigInfo = 3,
        CloseHandlerInfo = 4
    }
}
