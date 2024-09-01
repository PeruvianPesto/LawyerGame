using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    public Transform cameraTransform;
    public float focusSpeed = 5f;
    public Transform target;

    public void FocusOnCharacter(Transform characterTransform)
    {
        target = characterTransform;
    }

    void Update()
    {
        // If there's no target, don't do anything
        if (target == null)
            return;

        // Smoothly move the camera towards the target character
        cameraTransform.position = Vector3.Lerp(cameraTransform.position,
                                                new Vector3(target.position.x, target.position.y, cameraTransform.position.z),
                                                focusSpeed * Time.deltaTime);
    }
}
