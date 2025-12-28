using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Smooth settings")]
    public float smoothSpeed = 5f;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Mouse follow")]
    public float mouseOffsetAmount = 2f; // максимальное смещение в сторону мыши
    public float mouseSmoothSpeed = 5f;  // скорость сглаживания смещения мыши

    private Vector3 currentMouseOffset;

    void LateUpdate()
    {
        if (target == null) return;

        // Получаем позицию мыши в мировых координатах
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        // Рассчитываем желаемое смещение в сторону мыши
        Vector3 targetMouseOffset = (mouseWorldPosition - target.position) * 0.1f;
        targetMouseOffset = Vector3.ClampMagnitude(targetMouseOffset, mouseOffsetAmount);

        // Плавно приближаем текущее смещение к целевому
        currentMouseOffset = Vector3.Lerp(
            currentMouseOffset,
            targetMouseOffset,
            mouseSmoothSpeed * Time.deltaTime
        );

        // Рассчитываем желаемую позицию камеры с учетом смещения мыши
        Vector3 desiredPosition = target.position + offset + currentMouseOffset;

        // Плавное движение камеры к желаемой позиции
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
