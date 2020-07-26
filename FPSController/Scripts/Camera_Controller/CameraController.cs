using NaughtyAttributes;
using UnityEngine;

namespace VHS
{
    public class CameraController : MonoBehaviour
    {
        #region Variables
        #region Data
        [Space, Header("Data")]
        [SerializeField] private CameraInputData camInputData = null;

        [Space, Header("Custom Classes")]
        [SerializeField] private CameraZoom cameraZoom = null;
        [SerializeField] private CameraSwaying cameraSway = null;

        #endregion

        #region Settings
        [Space, Header("Look Settings")]
        [SerializeField] private Vector2 sensitivity = Vector2.zero;
        [SerializeField] private Vector2 smoothAmount = Vector2.zero;
        [SerializeField] [MinMaxSlider(-90f, 90f)] private Vector2 lookAngleMinMax = Vector2.zero;
        [Space, Header("MenuInGame Setting")]
        [SerializeField] private GameObject uiMenu = null;
        [SerializeField] private bool inMenu = false;
        #endregion

        #region Private
        private float m_yaw;
        private float m_pitch;

        private float m_desiredYaw;
        private float m_desiredPitch;

        #region Components                    
        private Transform m_pitchTranform;
        private Camera m_cam;
        #endregion
        #endregion

        #endregion

        #region BuiltIn Methods  
        void Awake()
        {
            GetComponents();
            InitValues();
            InitComponents();
            ChangeCursorState(false);
            float sensi = PlayerPrefs.GetFloat("Sensibilite", 150);
            sensitivity.x = sensi;
            sensitivity.y = sensi;
            uiMenu = GameObject.Find("InGameMenu");
        }

        void LateUpdate()
        {
            if (!inMenu)
            {
                CalculateRotation();
            }
            SmoothRotation();
            ApplyRotation();
            HandleZoom();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                inMenu = !inMenu;
                ChangeCursorState(inMenu);
                uiMenu.transform.localPosition = inMenu ?new Vector3(0,0,0): new Vector3(0, 10000, 0);
            }
        }
        #endregion

        #region Custom Methods
        void GetComponents()
        {
            m_pitchTranform = transform.GetChild(0).transform;
            m_cam = GetComponentInChildren<Camera>();
        }

        void InitValues()
        {
            m_yaw = transform.eulerAngles.y;
            m_desiredYaw = m_yaw;
        }

        void InitComponents()
        {
            cameraZoom.Init(m_cam, camInputData);
            cameraSway.Init(m_cam.transform);
        }

        void CalculateRotation()
        {
            m_desiredYaw += camInputData.InputVector.x * sensitivity.x * Time.deltaTime;
            m_desiredPitch -= camInputData.InputVector.y * sensitivity.y * Time.deltaTime;

            m_desiredPitch = Mathf.Clamp(m_desiredPitch, lookAngleMinMax.x, lookAngleMinMax.y);
        }

        void SmoothRotation()
        {
            m_yaw = Mathf.Lerp(m_yaw, m_desiredYaw, smoothAmount.x * Time.deltaTime);
            m_pitch = Mathf.Lerp(m_pitch, m_desiredPitch, smoothAmount.y * Time.deltaTime);
        }

        void ApplyRotation()
        {
            transform.eulerAngles = new Vector3(0f, m_yaw, 0f);
            m_pitchTranform.localEulerAngles = new Vector3(m_pitch, 0f, 0f);
        }

        public void HandleSway(Vector3 _inputVector, float _rawXInput)
        {
            cameraSway.SwayPlayer(_inputVector, _rawXInput);
        }

        void HandleZoom()
        {
            if (camInputData.ZoomClicked || camInputData.ZoomReleased)
                cameraZoom.ChangeFOV(this);

        }

        public void ChangeRunFOV(bool _returning)
        {
            cameraZoom.ChangeRunFOV(_returning, this);
        }

        public void ChangeCursorState(bool state)
        {
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = state;
        }
        #endregion
    }
}
