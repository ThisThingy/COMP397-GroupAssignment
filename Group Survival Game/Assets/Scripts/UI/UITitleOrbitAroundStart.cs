using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UITitleRotateAroundStartPivot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private RectTransform rotationPivot; 

    [Header("Rotation")]
    [SerializeField] private float maxAngle = 30f;
    [SerializeField] private float rotateSpeed = 12f;
    [SerializeField] private float returnSpeed = 10f;
    [SerializeField] private bool invert = false;

    private bool isHovering = false;
    private float currentAngle = 0f;
    private float targetAngle = 0f;
    private float initialPivotZ = 0f;

    private void Awake()
    {
        if (rotationPivot == null)
        {
            Debug.LogError("[UITitleRotateAroundStartPivot] rotationPivot 未赋值。", this);
            enabled = false;
            return;
        }

        initialPivotZ = rotationPivot.localEulerAngles.z;
        if (initialPivotZ > 180f) initialPivotZ -= 360f;
    }

    private void Update()
    {
        if (rotationPivot == null) return;

        if (isHovering)
        {
            UpdateTargetAngleFromMouse();
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.unscaledDeltaTime * rotateSpeed);
        }
        else
        {
            targetAngle = 0f;
            currentAngle = Mathf.Lerp(currentAngle, 0f, Time.unscaledDeltaTime * returnSpeed);
        }

        rotationPivot.localRotation = Quaternion.Euler(0f, 0f, initialPivotZ + currentAngle);
    }

    private void UpdateTargetAngleFromMouse()
    {
        if (Mouse.current == null)
        {
            targetAngle = 0f;
            return;
        }

        Vector2 mouse = Mouse.current.position.ReadValue();
        Vector2 center = rotationPivot.position;

        
        float mouseAngle = Mathf.Atan2(mouse.y - center.y, mouse.x - center.x) * Mathf.Rad2Deg;

      
        float delta = Mathf.DeltaAngle(0f, mouseAngle);

        if (invert) delta = -delta;

        targetAngle = Mathf.Clamp(delta, -maxAngle, maxAngle);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}