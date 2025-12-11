using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombinedInteraction : MonoBehaviour
{
    [Header("Pickup Settings")]
    public GameObject player;
    public Transform holdPos;
    public float throwForce = 500f;
    public float interactDistance = 5f;
    public float useDistance = 3f; // Distance to use items
    private float rotationSensitivity = 0.7f;
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private IUsableItem heldItemScript; // Reference to the item's script
    private bool canDrop = true;
    private int LayerNumber;

    [Header("Input Actions")]
    public InputActionReference interactAction;
    public InputActionReference useAction; // Left click - use item or throw
    public InputActionReference rotateAction;
    private Vector2 mouseInput;

    [Header("UI")]
    public CrosshairManager crosshairManager;

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
        if (useAction != null)
        {
            useAction.action.Enable();
            useAction.action.performed += OnUse;
        }
        if (rotateAction != null)
        {
            rotateAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
            interactAction.action.Disable();
        }
        if (useAction != null)
        {
            useAction.action.performed -= OnUse;
            useAction.action.Disable();
        }
        if (rotateAction != null)
        {
            rotateAction.action.Disable();
        }
    }

    void Update()
    {
        if (heldObj != null)
        {
            // Holding an object: move/rotate it, and hide the crosshair
            MoveObject();
            RotateObject();

            if (crosshairManager != null)
            {
                crosshairManager.HideAll();
            }

            return;
        }

        // Not holding anything update hover state & crosshair
        UpdateHoverCrosshair();
    }

    void UpdateHoverCrosshair()
    {
        if (crosshairManager == null)
            return;

        // Raycast from this object forward (same as OnInteract)
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            // Check if the thing we are looking at is interactable or pickupable,
            // same logic as in OnInteract()
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            bool isPickup = hit.transform.CompareTag("canPickUp");

            if (interactable != null || isPickup)
            {
                // Hovering something interactable show hand
                crosshairManager.ShowHand();
                return;
            }
        }

        // Nothing interactable under crosshair show default cross
        crosshairManager.ShowDefault();
    }


    private void OnInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("E pressed - trying interaction");

        // If holding an object, drop it
        if (heldObj != null)
        {
            if (canDrop == true)
            {
                Debug.Log("Dropping object");
                StopClipping();
                DropObject();
            }
            return;
        }

        // Raycast to find interactable objects
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.name + " | Tag: " + hit.transform.gameObject.tag);

            // First, check for IInteractable (for doors, switches, etc.)
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log("Found IInteractable - interacting");
                interactable.Interact();
                return;
            }

            // Second, check for pickupable objects with tag
            if (hit.transform.gameObject.tag == "canPickUp")
            {
                Debug.Log("Found pickupable object - picking up");
                PickUpObject(hit.transform.gameObject);
                return;
            }

            Debug.Log("No interactable or pickupable object found");
        }
        else
        {
            Debug.Log("No raycast hit");
        }
    }

    private void OnUse(InputAction.CallbackContext ctx)
    {
        if (heldObj != null && canDrop == true)
        {
            // If item has special use functionality, use it
            if (heldItemScript != null)
            {
                Debug.Log("Using item: " + heldItemScript.GetItemName());
                heldItemScript.Use();
            }
            else
            {
                // Otherwise, just throw it
                Debug.Log("Throwing object");
                StopClipping();
                ThrowObject();
            }
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform;
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);

            // Check if item has special use functionality
            heldItemScript = pickUpObj.GetComponent<IUsableItem>();
            if (heldItemScript != null)
            {
                Debug.Log("Picked up usable item: " + heldItemScript.GetItemName());
            }
        }
    }

    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldItemScript = null;
        heldObj = null;

        if (crosshairManager != null)
        {
            crosshairManager.ShowDefault();
        }
    }

    void MoveObject()
    {
        heldObj.transform.position = holdPos.transform.position;
    }

    void RotateObject()
    {
        if (rotateAction != null && rotateAction.action.IsPressed())
        {
            canDrop = false;
            InteractionState.IsRotatingHeldItem = true;

            mouseInput = Mouse.current.delta.ReadValue();
            float XaxisRotation = mouseInput.x * rotationSensitivity * 0.1f;
            float YaxisRotation = mouseInput.y * rotationSensitivity * 0.1f;

            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            canDrop = true;
            InteractionState.IsRotatingHeldItem = false;
        }
    }

    void ThrowObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldItemScript = null;
        heldObj = null;

        if (crosshairManager != null)
        {
            crosshairManager.ShowDefault();
        }
    }

    void StopClipping()
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }

    // Helper method to get what player is looking at
    public RaycastHit? GetLookTarget()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, useDistance))
        {
            return hit;
        }
        return null;
    }
}