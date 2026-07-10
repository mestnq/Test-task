using Game.Features.Clicker.Config;
using Game.Features.Clicker.Model;

namespace Game.Features.Clicker.Service
{
    public class ManualClickService
    {
        public int ClickEnergyCost => _config.ClickEnergyCost;
        public int ClickCurrencyReward => _config.ClickCurrencyReward;

        #region DI

        private readonly IClickerModel _model;
        private readonly ClickerBalanceConfig _config;
        
        public ManualClickService(IClickerModel model, ClickerBalanceConfig config)
        {
            _model = model;
            _config = config;
        }

        #endregion

        public bool CanClick()
        {
            return _model.Energy.Value >= ClickEnergyCost;
        }

        public bool TryClick()
        {
            if (!_model.TrySpendEnergy(ClickEnergyCost))
                return false;

            _model.AddCurrency(ClickCurrencyReward);
            return true;
        }
    }
}