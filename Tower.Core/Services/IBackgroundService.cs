using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower.Core.Services
{
    public interface IBackgroundService
    {
        string SelectedBackgroundUri { get; set; }
        List<string> AvailableBackgroundUris { get; }
    }
}
