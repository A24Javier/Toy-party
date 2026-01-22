using UnityEngine;

public class Flecha : MonoBehaviour
{
    public RectTransform uiImage;
    public float followSpeed = 10f;

    void Update()
    {
        if (uiImage == null) return;

        Vector3 targetPos = Input.mousePosition;

        uiImage.position = Vector3.Lerp(
            uiImage.position,
            targetPos,
            followSpeed * Time.deltaTime
        );
    }
}
