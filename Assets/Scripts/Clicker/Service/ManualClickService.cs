using Game.Clicker.Model;
using ProjectName.Features.Clicker.Config;

namespace Game.Features.Clicker.Application
{
    public class ManualClickService
    {
        private readonly IClickerModel _model;
        private readonly ClickerBalanceConfig _config;

        public int ClickCost => _config.clickEnergyCost;

        public ManualClickService(IClickerModel model, ClickerBalanceConfig config)
        {
            _model = model;
            _config = config;
        }

        public bool CanClick(int currentEnergy)
        {
            return currentEnergy >= ClickCost;
        }

        public bool TryProcessClick()
        {
            if (!_model.TrySpendEnergy(ClickCost))
                return false;

            _model.AddCurrency(_config.clickCurrencyReward);
            return true;
        }
    }
}
