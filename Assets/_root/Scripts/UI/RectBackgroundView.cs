using UnityEngine;

namespace _root.Scripts.UI
{
    public class RectBackgroundView : View
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform backgroundAnchor;

        protected void UpdateBackground()
        {
            backgroundAnchor.anchoredPosition3D = Utils.Utils.CenterAnchorPosition();
            background.anchoredPosition3D = Vector3.Lerp(background.anchoredPosition3D, backgroundAnchor.anchoredPosition3D, Time.deltaTime * 2);
        }
    }
}