using Game.Features.Clicker.Application;
using Game.Features.Clicker.Config;
using Game.Features.Clicker.Model;
using Game.Features.Clicker.Presentation;
using Game.Features.Clicker.Service;
using Game.Features.Clicker.View;
using UnityEngine;
using Zenject;

namespace Game.Features.Clicker.Installers
{
    public class ClickerInstaller : MonoInstaller
    {
        [SerializeField] private ClickerBalanceConfig config;

        public override void InstallBindings()
        {
            BindConfig();
            BindViews();
            BindModel();
            BindApplication();
            BindPresentation();
        }

        private void BindConfig()
        {
            Container.Bind<ClickerBalanceConfig>().FromInstance(config).AsSingle();
        }

        private void BindViews()
        {
            Container.Bind<IClickerView>().To<ClickerView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IClickerFeedbackView>().To<ClickerFeedbackView>().FromComponentInHierarchy().AsSingle();
        }

        private void BindModel()
        {
            Container.Bind<IClickerModel>().To<ClickerModel>().AsSingle();
        }

        private void BindApplication()
        {
            Container.Bind<ManualClickService>().AsSingle();
            Container.Bind<AutoClickLoop>().AsSingle();
            Container.Bind<EnergyRegenLoop>().AsSingle();
            Container.Bind<ClickerLoopRunner>().AsSingle();
        }

        private void BindPresentation()
        {
            Container.BindInterfacesAndSelfTo<ClickerPresenter>().AsSingle();
        }
    }
}