using UnityEngine;

public class CameraAspectController : MonoBehaviour
{
    #region Variables
    [SerializeField] private float targetAspect ;
    [SerializeField] private float targetFOV;
    private Camera cam;
    #endregion

    #region Methods
    void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustFOV();
    }

    void AdjustFOV()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        float aspectRatioFactor = currentAspect / targetAspect;
        cam.fieldOfView = targetFOV / aspectRatioFactor;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 30f, 90f);
    }

    void Update()
    {
        AdjustFOV();
    }
    #endregion
}
