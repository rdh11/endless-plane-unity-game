using DG.Tweening;
using TMPro;
using UnityEngine;

namespace EndlessPlane.Core.UI
{
    public class UIScore : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _scoreText = null;
        [SerializeField]
        RectTransform _scoreParentRect = null;
        [SerializeField]
        float _animationDuration = 0.3f;

        public void UpdateScore(string score)
        {
            _scoreText.text = score;
            AnimateScore();
        }

        void AnimateScore()
        {
            _scoreParentRect.DOScale(0.75f, _animationDuration * 25 / 100).onComplete = () =>
            {
                _scoreParentRect.DOScale(1.25f, _animationDuration * 50 / 100).onComplete = () =>
                {
                    _scoreParentRect.DOScale(1.0f, _animationDuration * 25 / 100);
                };
            };
        }
    }
}
