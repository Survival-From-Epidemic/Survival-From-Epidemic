using _root.Scripts.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _root.Scripts.UI
{
    public class UIImage : MonoBehaviour
    {
        [SerializeField] private Sprite defaultImage;
        [SerializeField] private Sprite hoverImage;
        [SerializeField] private Sprite clickImage;
        [SerializeField] private Sprite selectImage;
        public bool isSelected;

        [Space] public EventTrigger.TriggerEvent onHover;

        public EventTrigger.TriggerEvent onHoverOut;
        public EventTrigger.TriggerEvent onClickDown;
        public EventTrigger.TriggerEvent onClickUp;
        public Image image;
        private bool _isClicked;
        private EventTrigger _trigger;

        private void Awake()
        {
            image = GetComponent<Image>();
            image.sprite = GetDefaultImage();
            _trigger = image.gameObject.AddComponent<EventTrigger>();
            onHover = new EventTrigger.TriggerEvent();
            onHoverOut = new EventTrigger.TriggerEvent();
            onClickDown = new EventTrigger.TriggerEvent();
            onClickUp = new EventTrigger.TriggerEvent();

            _trigger.triggers.Add(new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter,
                callback = onHover
            });
            _trigger.triggers.Add(new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit,
                callback = onHoverOut
            });
            _trigger.triggers.Add(new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown,
                callback = onClickDown
            });
            _trigger.triggers.Add(new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp,
                callback = onClickUp
            });
            _isClicked = false;


            if (!defaultImage) return;

            onHover.AddListener(_ => image.sprite = hoverImage ? hoverImage : GetDefaultImage());

            onHoverOut.AddListener(_ =>
            {
                if (_isClicked) return;
                image.sprite = GetDefaultImage();
            });

            onClickDown.AddListener(_ =>
            {
                _isClicked = true;
                if (!clickImage) return;
                image.sprite = clickImage;
            });

            onClickUp.AddListener(_ =>
            {
                _isClicked = false;
                if (!clickImage) return;
                image.sprite = GetDefaultImage();
            });
        }

        public void ForceChangeImage(Sprite sprite)
        {
            if (!image) Debugger.Log(name);
            image.sprite = sprite;
        }

        private Sprite GetDefaultImage() => isSelected ? selectImage : defaultImage;
    }
}