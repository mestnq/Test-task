using UnityEngine;

namespace Game.Features.Clicker.Config
{
    [CreateAssetMenu(menuName = "Game/Clicker/ClickerBalanceConfig")]
    public class ClickerBalanceConfig : ScriptableObject
    {
        [field: Header("Rewards")]
        // Кол-во начисляемой валюты при клике
        [field: SerializeField] public int ClickCurrencyReward { get; private set; } = 1;
        // Кол-во начисляемой валюты при авто клике
        [field: SerializeField] public int AutoClickCurrencyReward { get; private set; } = 1;
        
        [field: Header("Costs")] 
        // Кол-во списаной энергии при клике
        [field: SerializeField] public int ClickEnergyCost { get; private set; } = 1;
        /// <summary> Кол-во списаной энергии при авто клике
        [field: SerializeField] public int AutoClickEnergyCost { get; private set; } = 1;

        [field: Header("Energy")] 
        [field: SerializeField] public int MaxEnergy { get; private set; } = 1000;
        [field: SerializeField] public int StartEnergy { get; private set; } = 1000;
        // Кол-во автоматически начисляемой энергии
        [field: SerializeField] public int EnergyRegenAmount { get; private set; } = 10;
        // Через сколько секунд автоматически начислится энергия
        [field: SerializeField] public float EnergyRegenIntervalSeconds { get; private set; } = 10f;

        [field: Header("Auto regen currency")] 
        // Через какой промежуток времени происходит автосбор валюты
        [field: SerializeField] public float AutoRegenCurrencyInIntervalSeconds { get; private set; } = 3f;
    }
}