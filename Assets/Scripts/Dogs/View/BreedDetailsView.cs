using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dogs.UI.BreedsTab
{
    public sealed class BreedDetailsView : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Button closeButton;

        public void Initialize(string title, string description, Action onClose = null)
        {
            titleText.text = title;
            descriptionText.text = description;

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(() => onClose?.Invoke());
            }
        }
    }
}