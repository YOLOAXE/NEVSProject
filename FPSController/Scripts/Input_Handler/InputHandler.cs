using UnityEngine;
using NaughtyAttributes;

namespace VHS
{    
    public class InputHandler : MonoBehaviour
    {
        #region Data
            [Space,Header("Input Data")]
            [SerializeField] private CameraInputData cameraInputData = null;
            [SerializeField] private MovementInputData movementInputData = null;
            [SerializeField] private InteractionInputData interactionInputData = null;
        #endregion

        #region BuiltIn Methods
            void Start()
            {
                cameraInputData.ResetInput();
                movementInputData.ResetInput();
                interactionInputData.ResetInput();
            }

            void Update()
            {
                GetCameraInput();
                GetMovementInputData();
                GetInteractionInputData();
            }
        #endregion

        #region Custom Methods
            void GetInteractionInputData()
            {
                interactionInputData.InteractedClicked = GameInputManager.GetKeyDown("Interagire");
                interactionInputData.InteractedReleased = GameInputManager.GetKeyUp("Interagire");
            }

            void GetCameraInput()
            {
                cameraInputData.InputVectorX = Input.GetAxis("Mouse X");
                cameraInputData.InputVectorY = Input.GetAxis("Mouse Y");

                cameraInputData.ZoomClicked = GameInputManager.GetKeyDown("Viser");
                cameraInputData.ZoomReleased = GameInputManager.GetKeyUp("Viser");
            }

            void GetMovementInputData()
            {
                movementInputData.InputVectorX = GameInputManager.GetKey("strafeDroite") ? 1 : GameInputManager.GetKey("strafeGauche") ? -1 : 0;
                movementInputData.InputVectorY = GameInputManager.GetKey("Avancer") ? 1 : GameInputManager.GetKey("Reculer") ? -1 : 0;

                movementInputData.RunClicked = GameInputManager.GetKeyDown("Sprint");
                movementInputData.RunReleased = GameInputManager.GetKeyUp("Sprint");

                if(movementInputData.RunClicked)
                    movementInputData.IsRunning = true;

                if(movementInputData.RunReleased)
                    movementInputData.IsRunning = false;

                movementInputData.JumpClicked = GameInputManager.GetKeyDown("Sauter");
                movementInputData.CrouchClicked = GameInputManager.GetKeyDown("Acroupie");
            }
        #endregion
    }
}