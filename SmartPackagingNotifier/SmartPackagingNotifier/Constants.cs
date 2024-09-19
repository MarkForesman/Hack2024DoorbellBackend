using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPackagingNotifier
{
    internal static class Constants
    {
        public const int BACKDOOR_BUTTON_NUM = 1;
        public const int FRONTDOOR_BUTTON_NUM = 1;
        public const string BACKDOOR = "Back door";
        public const string FRONTDOOR = "Front door";

        public static string GetLocation(int button)
        {
            return button == Constants.BACKDOOR_BUTTON_NUM ? Constants.BACKDOOR : Constants.FRONTDOOR;
        }
    }
}
