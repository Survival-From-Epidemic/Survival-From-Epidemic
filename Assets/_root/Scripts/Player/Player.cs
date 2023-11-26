using _root.Scripts.Game;
using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Player
{
    public class Player : SingleMono<Player>
    {
        [SerializeField] public float speed = 5;
        [SerializeField] private float distance = 25;
        [SerializeField] private float scrollMultiplier = 10;
        [SerializeField] private float mouseMultiplier = 10;
        [SerializeField] private float delta = 10;

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
            distance = Mathf.Clamp(distance, 10, 80);
            UpdatePosition();
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, transform.rotation * transform.position, Time.deltaTime * delta);
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
                // angle.x = Mathf.Clamp(angle.x, -90, 0);
                thisTransform.eulerAngles = angle;
                Debugger.Log($"Mouse: {mouseX} / {mouseY}");
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }

            var par = (-Mathf.Abs(Mathf.Abs(angle.y) + 90) + 90) / 90 + 1;

            _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, Quaternion.LookRotation(-_camera.transform.position), Time.deltaTime * delta);
            thisTransform.position = distance * Vector3.forward;
        }
    }
}