using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Player
{
    public class Player : SingleMono<Player>
    {
        [SerializeField] public float speed = 5;
        [SerializeField] private float distance = 25;

        [Space] [SerializeField] private float minScroll = 15;

        [SerializeField] private float maxScroll = 200;
        [SerializeField] private float scrollMultiplier = 10;

        [Space] [SerializeField] private float mouseMultiplier = 10;

        [SerializeField] private float animDelta = 10;

        private Camera _camera;

        private void Start()
        {
            _camera = MainCamera.Component;
            UpdatePosition();
            _camera.transform.position = transform.position;
        }

        private void Update()
        {
            var scrollY = Input.mouseScrollDelta.y;
            if (scrollY > 0) distance -= Time.deltaTime * scrollMultiplier;
            else if (scrollY < 0) distance += Time.deltaTime * scrollMultiplier;
            distance = Mathf.Clamp(distance, minScroll, maxScroll);
            UpdatePosition();
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, transform.rotation * transform.position, Time.deltaTime * animDelta);
        }

        private void UpdatePosition()
        {
            var thisTransform = transform;
            var angle = thisTransform.eulerAngles;
            if (Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                var mouseX = mouseMultiplier * Input.GetAxis("Mouse X");
                var mouseY = mouseMultiplier * Input.GetAxis("Mouse Y");
                angle += mouseX * Vector3.up + mouseY * Vector3.right;
                // if (angle.x + mouseY - 180 <= 0) mouseY = 0;
                // angle += mouseX * Vector3.up;
                // if(angle.x <= 0) 
                // angle.x = Mathf.Clamp(angle.x - 180, 0, 180) + 180;
                thisTransform.eulerAngles = angle;
            }
            else Cursor.lockState = CursorLockMode.None;

            var par = (-Mathf.Abs(Mathf.Abs(angle.y % 180) - 90) + 90) * 1.25f / 90 + 1;

            _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, Quaternion.LookRotation(-_camera.transform.position), Time.deltaTime * animDelta);
            thisTransform.position = par * distance * Vector3.forward;
        }
    }
}