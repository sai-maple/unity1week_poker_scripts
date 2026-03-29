using UnityEngine;

namespace Laughter.Poker.View.UI.Common
{
    /// <summary>
    /// マウスカーソルに追従するライト
    /// </summary>
    public class CursorLight : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            var mousePos = Input.mousePosition; 
            var worldPos = _camera.ScreenToWorldPoint(mousePos);
            worldPos.z = 0; 
            transform.position = worldPos;
        }
    }
}
