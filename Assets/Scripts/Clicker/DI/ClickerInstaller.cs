using Game.Clicker.Model;
using Game.Features.Clicker.Application;
using Game.Features.Clicker.Presenter;
using Game.Features.Clicker.Timer;
using Game.Features.Clicker.View;
using ProjectName.Features.Clicker.Config;
using UnityEngine;
using Zenject;

namespace Clicker.DI
{
    public class ClickerInstaller : MonoInstaller
    {
        [SerializeField] private ClickerBalanceConfig config;

        public override void InstallBindings()
        {
            BindConfig();
            BindView();
            BindModel();
            BindServices();
            BindPresentation();
        }

        private void BindConfig()
        {
            Container.Bind<ClickerBalanceConfig>()
                .FromInstance(config)
                .AsSingle();
        }

        private void BindView()
        {
            Container.Bind<IClickerView>()
                .To<ClickerView>()
                .FromComponentInHierarchy()
                .AsSingle();
        }

        private void BindModel()
        {
            Container.Bind<IClickerModel>()
                .To<ClickerModel>()
                .AsSingle();
        }

        private void BindServices()
        {
            Container.Bind<ManualClickService>().AsSingle();
            Container.Bind<AutoClickLoop>().AsSingle();
            Container.Bind<EnergyRegenLoop>().AsSingle();
        }

        private void BindPresentation()
        {
            Container.BindInterfacesAndSelfTo<ClickerPresenter>().AsSingle();
        }
    }
}
