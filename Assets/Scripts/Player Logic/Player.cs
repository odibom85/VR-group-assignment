using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;  

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;

    [SerializeField] private PlayerCamera playerCamera;


    private PlayerInputActions _inputActions;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
    }

    void OnDestroy()
    {
        if (_inputActions != null)
        {
        _inputActions.Dispose();
        }
    }

    void OnDisable()
    {
        if (_inputActions != null)
        {
            _inputActions.Disable();
        }
    }

    void Update()
    {
        var input = _inputActions.Gameplay;
        var deltaTime = Time.deltaTime;

        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };

        // Only rotate camera if NOT rotating held item
        if (!InteractionState.IsRotatingHeldItem)
        {
            playerCamera.UpdateRotation(cameraInput);
        }
        
        var characterInput = new CharacterInput
        {
            Rotation = playerCamera.transform.rotation,
            Move = input.Move.ReadValue<Vector2>(),
            Jump = input.Jump.WasPressedThisFrame(),
            JumpSustain = input.Jump.IsPressed(),
            Crouch = input.Crouch.WasPressedThisFrame()
                ? CrouchInput.Toggle
                : CrouchInput.None
        };
        playerCharacter.UpdateInput(characterInput);
        // REMOVED: playerCharacter.UpdateBody(deltaTime);

        if (input.MenuOpenClose.WasPressedThisFrame())
        {
            // Load scene 0 explicitly
            SceneManager.LoadScene(0);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

#if UNITY_EDITOR
    if (Keyboard.current.tKey.wasPressedThisFrame)
    {
        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out var hit))
        {
            Teleport(hit.point);
        }
    }
#endif
    }

    void LateUpdate()
    {
        // Update body AFTER KCC has updated the capsule
        playerCharacter.UpdateBody(Time.deltaTime);

        // Then update camera position
        playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
    }


    public void Teleport(Vector3 position)
    {
        playerCharacter.SetPosition(position);
    }
}
