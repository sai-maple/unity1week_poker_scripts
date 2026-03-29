using Laughter.Poker.View.UI.Common;
using UnityEngine;
using UnityEngine.UI;


namespace Laughter.Poker.View.UI.Title
{
    public class TitleCanvas : ViewCommon
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private GameObject _gameLoop;

        private void Start()
        {
            Present();

            _startButton.onClick.AddListener(() =>
            {
                Dismiss();
                Instantiate(_gameLoop);
            });
        }
    }
}
