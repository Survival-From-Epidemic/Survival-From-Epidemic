using _root.Scripts.Game;
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
        [SerializeField] private Camera mainCamera;

        [Space] [SerializeField] public Person selectedPerson;
        [SerializeField] public bool gizmoOn;
        private Vector3 _beforeEulerAngle;

        private Vector3 _beforePosition;

        private void Start()
        {
            mainCamera = MainCamera.Component;
            UpdatePosition();
            mainCamera.transform.position = transform.position;
        }

        private void Update()
        {
            if (selectedPerson is not null && !selectedPerson.gameObject.activeSelf)
            {
                selectedPerson = null;
                transform.position = _beforePosition;
                transform.eulerAngles = _beforeEulerAngle;
            }

            var scrollY = Input.mouseScrollDelta.y;
            if (scrollY > 0) distance -= Time.deltaTime * scrollMultiplier;
            else if (scrollY < 0) distance += Time.deltaTime * scrollMultiplier;
            distance = Mathf.Clamp(distance, minScroll, maxScroll);
            UpdatePosition();
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, transform.rotation * transform.position, Time.fixedDeltaTime * animDelta);

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(MainCamera.Component.ScreenPointToRay(Input.mousePosition), out var hit))
                {
                    Debugger.Log("Person Selected");
                    selectedPerson = hit.collider.GetComponent<Person>();
                    _beforePosition = transform.position;
                    _beforeEulerAngle = transform.eulerAngles;
                }
                else
                {
                    selectedPerson = null;
                    transform.position = _beforePosition;
                    transform.eulerAngles = _beforeEulerAngle;
                }
            }
        }

        private void UpdatePosition()
        {
            if (Time.timeScale == 0)
            {
                Cursor.lockState = CursorLockMode.None;
                return;
            }

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

            var par = (-Mathf.Abs(Mathf.Abs(angle.y % 180) - 90) + 90) / 90 + 1;

            var mainCameraTransform = mainCamera.transform;
            if (selectedPerson)
            {
                var personPosition = selectedPerson.transform.position;
                mainCameraTransform.rotation = Quaternion.Lerp(mainCameraTransform.rotation,
                    Quaternion.LookRotation(personPosition - mainCameraTransform.position), Time.fixedDeltaTime * animDelta);
                thisTransform.position = par * distance * 0.05f * Vector3.forward + personPosition;
            }
            else
            {
                mainCameraTransform.rotation = Quaternion.Lerp(mainCameraTransform.rotation,
                    Quaternion.LookRotation(-mainCameraTransform.position), Time.fixedDeltaTime * animDelta);
                thisTransform.position = par * distance * Vector3.forward;
            }
        }
    }
}