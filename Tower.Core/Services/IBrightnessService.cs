using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower.Core.Services
{
    public interface IBrightnessService
    {
        bool SetBrightness(double brightnessPercent);
    }
}
