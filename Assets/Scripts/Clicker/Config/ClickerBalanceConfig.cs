using UnityEngine;

namespace ProjectName.Features.Clicker.Config
{
    [CreateAssetMenu(menuName = "Game/Clicker/ClickerBalanceConfig")]
    public class ClickerBalanceConfig : ScriptableObject
    {
        [Header("Rewards")]
        /// <summary> Кол-во начисляемой валюты при клике </summary>
        public int clickCurrencyReward = 1;
        /// <summary> Кол-во начисляемой валюты при авто клике </summary>
        public int autoClickCurrencyReward = 1;
        
        [Header("Costs")] 
        /// <summary> Кол-во списаной энергии при клике </summary>
        public int clickEnergyCost = 1;
        /// <summary> Кол-во списаной энергии при авто клике </summary>
        public int autoClickEnergyCost = 1;

        [Header("Energy")] 
        public int maxEnergy = 1000;
        public int startEnergy = 1000;
        /// <summary> Кол-во автоматически начисляемой энергии </summary>
        public int energyRegenAmount = 10;
        /// <summary> Через сколько секунд автоматически начислится энергия </summary>
        public float energyRegenIntervalSeconds = 10f;

        [Header("Auto regen currency")] 
        /// <summary> Через какой промежуток времени происходит автосбор валюты </summary>
        public float autoRegenCurrencyInIntervalSeconds = 3f;
    }
}