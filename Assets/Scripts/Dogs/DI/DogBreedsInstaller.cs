using Dogs.Config;
using Dogs.Domain.Services;
using Dogs.UI.BreedsTab;
using UnityEngine;
using Zenject;

namespace Dogs.Infrastructure
{
    public sealed class DogBreedsInstaller : MonoInstaller
    {
        [SerializeField] private DogApiConfig dogApiConfig;
        [SerializeField] private DogBreedsTabView dogBreedsTabView;
        [SerializeField] private BreedListItemView breedListItemPrefab;
        [SerializeField] private BreedDetailsView breedDetailsPrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(dogApiConfig).AsSingle();

            Container.Bind<IDogApiClient>().To<DogApiClient>().AsSingle();

            Container.Bind<DogBreedsTabView>().FromInstance(dogBreedsTabView).AsSingle();

            Container.BindFactory<BreedListItemView, BreedListItemFactory>().FromComponentInNewPrefab(breedListItemPrefab);

            Container.BindFactory<BreedDetailsView, BreedDetailsFactory>().FromComponentInNewPrefab(breedDetailsPrefab);

            Container.BindInterfacesAndSelfTo<DogBreedsTabPresenter>().AsSingle().NonLazy();
        }
    }
}
