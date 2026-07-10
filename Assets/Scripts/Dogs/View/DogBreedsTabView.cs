using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dogs.UI.BreedsTab
{
    public sealed class DogBreedsTabView : MonoBehaviour
    {
        [Header("Breeds list")] 
        [SerializeField] private Transform listRoot;

        [SerializeField] private GameObject breedsLoader;
        [SerializeField] private GameObject emptyState;

        [Header("Breed details")] 
        [SerializeField] private Transform detailsRoot;
        [SerializeField] private GameObject breedDetailsLoader;

        private readonly List<BreedListItemView> _spawnedItems = new();
        private BreedDetailsView _currentDetailsView;

        public event Action Shown;
        public event Action Hidden;
        public event Action<string> BreedClicked;

        public Transform ListRoot => listRoot;
        public Transform DetailsRoot => detailsRoot;

        public void CreateState(bool isActive)
        {
            if (isActive)
            {
                gameObject.SetActive(true);
                Shown?.Invoke();
            }
            else
            {
                Hidden?.Invoke();
                gameObject.SetActive(false);
            }
        }

        public void RaiseBreedClicked(string breedId)
        {
            BreedClicked?.Invoke(breedId);
        }

        public void SetBreedsLoaderVisible(bool visible)
        {
            if (breedsLoader != null)
                breedsLoader.SetActive(visible);
        }

        public void SetBreedDetailsLoaderVisible(bool visible)
        {
            if (breedDetailsLoader != null)
                breedDetailsLoader.SetActive(visible);
        }

        public void SetEmptyVisible(bool visible)
        {
            if (emptyState != null)
                emptyState.SetActive(visible);
        }

        public void SetItems(IReadOnlyList<BreedListItemView> items)
        {
            ClearItems();
            _spawnedItems.AddRange(items);
        }

        public void SetDetailsView(BreedDetailsView detailsView)
        {
            ClearDetailsView();
            _currentDetailsView = detailsView;
        }

        public BreedDetailsView GetCurrentDetailsView()
        {
            return _currentDetailsView;
        }

        public void ClearDetailsView()
        {
            if (_currentDetailsView != null)
            {
                Destroy(_currentDetailsView.gameObject);
                _currentDetailsView = null;
            }
        }

        public void ClearItems()
        {
            foreach (var item in _spawnedItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }

            _spawnedItems.Clear();
        }
    }
}