using System;
using Tower.Core.Services;
using Tower.Services.Time;

namespace Tower.Application.DesignTimeServices
{
    // Regular run-time TimeService is fine to use during design time
    internal class DesignTimeTimeService : TimeService
    {
    }
}
