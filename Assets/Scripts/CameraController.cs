using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform playerBody;
    [SerializeField] Transform target;
    [SerializeField] private Transform CamHolder;
    [SerializeField] float sensitivity = 100f;
    [SerializeField] Vector3 OffSet;
    public float xRotation = 0f;

    [Header("Weapon Stabilization")]
    [SerializeField] private Transform weapon;
    private Vector3 weaponInitialOffset;
    private Quaternion weaponInitialLocalYawRot;
    private bool weaponInitialized = false;

    void Start()
    {
        if (weapon != null)
        {
            float yaw = playerBody != null ? playerBody.eulerAngles.y : transform.eulerAngles.y;
            Quaternion yawOnly = Quaternion.Euler(0f, yaw, 0f);
            weaponInitialOffset = Quaternion.Inverse(yawOnly) * (weapon.position - transform.position);
            weaponInitialLocalYawRot = Quaternion.Inverse(yawOnly) * weapon.rotation;
            weaponInitialized = true;
        }
    }

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Old Input Manager axes: "Mouse X" and "Mouse Y"
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate the player around the y-axis
        playerBody.Rotate(Vector3.up * mouseX);
        // apply pitch (xRotation) and match the player's yaw so the camera rotates with the player
        transform.rotation = Quaternion.Euler(xRotation, playerBody.eulerAngles.y, 0f);
    }

    void LateUpdate()
    {
        transform.position = target.position + OffSet;

        if (weapon != null && weaponInitialized)
        {
            float yaw = playerBody != null ? playerBody.eulerAngles.y : transform.eulerAngles.y;
            Quaternion yawOnly = Quaternion.Euler(0f, yaw, 0f);

            // Position: follow camera position but only rotate the stored offset by yaw (no pitch)
            weapon.position = transform.position + yawOnly * weaponInitialOffset;

            // Rotation: apply only yaw to the weapon keeping its initial local yaw-relative rotation
            weapon.rotation = yawOnly * weaponInitialLocalYawRot;
        }
    }
}