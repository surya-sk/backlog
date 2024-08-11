using System;
using System.Collections.Generic;
using System.Text;

namespace Backlogs.Utils.Uno
{
    public class SystemLauncher : ISystemLauncher
    {
        public async Task LaunchUriAsync(Uri uri)
        {
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
