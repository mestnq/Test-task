using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dogs.Config;
using Dogs.Domain.Models;
using Dogs.Domain.Services;
using Dogs.Infrastructure;
using UnityEngine;
using Zenject;

namespace Dogs.UI.BreedsTab
{
    public sealed class DogBreedsTabPresenter : IInitializable, IDisposable
    {
        private readonly DogBreedsTabView _view;
        private readonly IDogApiClient _dogApiClient;
        private readonly BreedListItemFactory _breedListItemFactory;
        private readonly BreedDetailsFactory _breedDetailsFactory;
        private readonly DogApiConfig _config;

        private readonly RequestSerialQueue _breedDetailsQueue = new();

        private CancellationTokenSource _tabLifetimeCts;
        private CancellationTokenSource _breedsRequestCts;

        public DogBreedsTabPresenter(
            DogBreedsTabView view,
            IDogApiClient dogApiClient,
            BreedListItemFactory breedListItemFactory,
            BreedDetailsFactory breedDetailsFactory,
            DogApiConfig config)
        {
            _view = view;
            _dogApiClient = dogApiClient;
            _breedListItemFactory = breedListItemFactory;
            _breedDetailsFactory = breedDetailsFactory;
            _config = config;
        }

        public void Initialize()
        {
            _view.Shown += OnTabShown;
            _view.Hidden += OnTabHidden;
            _view.BreedClicked += OnBreedClicked;
        }

        public void Dispose()
        {
            _view.Shown -= OnTabShown;
            _view.Hidden -= OnTabHidden;
            _view.BreedClicked -= OnBreedClicked;

            CancelAllRequests();
            _breedDetailsQueue.Dispose();
        }

        private void OnTabShown()
        {
            _tabLifetimeCts?.Cancel();
            _tabLifetimeCts?.Dispose();
            _tabLifetimeCts = new CancellationTokenSource();

            LoadBreedsAsync(_tabLifetimeCts.Token).Forget();
        }

        private void OnTabHidden()
        {
            CancelAllRequests();

            _view.SetBreedsLoaderVisible(false);
            _view.SetBreedDetailsLoaderVisible(false);
            _view.ClearDetailsView();
        }

        private void OnBreedClicked(string breedId)
        {
            LoadBreedDetailsQueuedAsync(breedId).Forget();
        }

        private void CancelAllRequests()
        {
            _breedsRequestCts?.Cancel();
            _breedsRequestCts?.Dispose();
            _breedsRequestCts = null;

            _breedDetailsQueue.CancelCurrent();

            _tabLifetimeCts?.Cancel();
            _tabLifetimeCts?.Dispose();
            _tabLifetimeCts = null;
        }

        private async UniTaskVoid LoadBreedsAsync(CancellationToken tabToken)
        {
            _breedsRequestCts?.Cancel();
            _breedsRequestCts?.Dispose();

            _breedsRequestCts = CancellationTokenSource.CreateLinkedTokenSource(tabToken);

            _view.SetBreedsLoaderVisible(true);
            _view.SetEmptyVisible(false);
            _view.ClearItems();
            _view.ClearDetailsView();

            try
            {
                IReadOnlyList<BreedShortInfo> breeds = await _dogApiClient.GetBreedsAsync(_config.BreedsPageSize, _breedsRequestCts.Token);

                if (_breedsRequestCts.Token.IsCancellationRequested)
                    return;

                BuildBreedsList(breeds);
                _view.SetEmptyVisible(breeds == null || breeds.Count == 0);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load breeds list: {e}");
                _view.SetEmptyVisible(true);
            }
            finally
            {
                _view.SetBreedsLoaderVisible(false);
            }
        }

        private void BuildBreedsList(IReadOnlyList<BreedShortInfo> breeds)
        {
            var itemViews = new List<BreedListItemView>(breeds.Count);

            for (int i = 0; i < breeds.Count; i++)
            {
                BreedShortInfo breed = breeds[i];

                BreedListItemView itemView = _breedListItemFactory.Create();
                itemView.transform.SetParent(_view.ListRoot, false);

                int index = i + 1; // человеческая нумерация с 1 начинается
                itemView.Bind(index, breed.Id, breed.Name, _view.RaiseBreedClicked);

                itemViews.Add(itemView);
            }

            _view.SetItems(itemViews);
        }

        private async UniTaskVoid LoadBreedDetailsQueuedAsync(string breedId)
        {
            _view.ClearDetailsView();
            _view.SetBreedDetailsLoaderVisible(true);

            await _breedDetailsQueue.Enqueue(async cancellationToken =>
            {
                try
                {
                    BreedDetails breedDetails =
                        await _dogApiClient.GetBreedDetailsAsync(breedId, cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                        return;

                    ShowBreedDetails(breedDetails);
                }
                catch (OperationCanceledException)
                {
                    // Запрос отменили — это ожидаемое поведение
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load breed details: {e}");
                }
                finally
                {
                    _view.SetBreedDetailsLoaderVisible(false);
                }
            });
        }

        private void ShowBreedDetails(BreedDetails breedDetails)
        {
            BreedDetailsView detailsView = _breedDetailsFactory.Create();
            detailsView.transform.SetParent(_view.DetailsRoot, false);

            detailsView.Initialize(
                breedDetails.Name,
                breedDetails.Description,
                onClose: () => _view.ClearDetailsView());

            _view.SetDetailsView(detailsView);
        }
    }
}