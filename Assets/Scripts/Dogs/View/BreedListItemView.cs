using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dogs.UI.BreedsTab
{
    public sealed class BreedListItemView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text titleText;

        private string _breedId;
        private Action<string> _onClick;

        public void Bind(int index, string breedId, string breedName, Action<string> onClick)
        {
            _breedId = breedId;
            _onClick = onClick;

            titleText.text = $"{index} - {breedName}";

            button.onClick.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            _onClick?.Invoke(_breedId);
        }
    }
}
