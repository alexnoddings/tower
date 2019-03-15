using Tower.Core.Services;

namespace Tower.Services.WmiBrightness
{
    public class WmiBrightnessService : IBrightnessService
    {
        private readonly bool _canUseWmiBrightness;

        public WmiBrightnessService()
        {
            _canUseWmiBrightness = WmiHelper.CanBeUsed();
        }

        public bool SetBrightness(double brightnessPercent)
        {
            if (!_canUseWmiBrightness) return false;

            WmiHelper.SetBrightness(brightnessPercent);
            return true;
        }
    }
}
