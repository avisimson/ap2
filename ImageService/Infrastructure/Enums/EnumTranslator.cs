using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure.Enums
{
    public static class EnumTranslator
    {
        public static InfoEnums CommandToInfo(int num)
        {
            switch (num)
            {
                case (int)CommandEnum.GetConfigCommand:
                    return InfoEnums.AppConfigInfo;
                case (int)CommandEnum.LogCommand:
                    return InfoEnums.LogHistoryInfo;
            }
            return InfoEnums.NotInfo;
        }

        public static CommandEnum InfoToCommand(int num)
        {
            switch (num)
            {
                case (int)InfoEnums.AppConfigInfo:
                    return CommandEnum.GetConfigCommand;
                case (int)InfoEnums.LogHistoryInfo:
                    return CommandEnum.LogCommand;
            }
            return CommandEnum.NotCommand;
        }
    }
}
