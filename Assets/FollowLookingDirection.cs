using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowLookingDirection : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float maxYawOffset = 20f;
    [SerializeField] private float maxPitchOffset = 15f;
    [SerializeField] private bool followRoll = false;
    [SerializeField] private float maxRollOffset = 10f;
    [SerializeField] private float followSpeed = 12f;

    private Vector3 baseLocalEuler;

    void Awake()
    {
        baseLocalEuler = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCamera == null)
        {
            return;
        }

        Quaternion targetLocalRotation = playerCamera.rotation;
        if (transform.parent != null)
        {
            targetLocalRotation = Quaternion.Inverse(transform.parent.rotation) * playerCamera.rotation;
        }

        Vector3 targetEuler = targetLocalRotation.eulerAngles;

        float yawOffset = Mathf.DeltaAngle(baseLocalEuler.y, targetEuler.y);
        float pitchOffset = Mathf.DeltaAngle(baseLocalEuler.x, targetEuler.x);
        float rollOffset = Mathf.DeltaAngle(baseLocalEuler.z, targetEuler.z);

        yawOffset = Mathf.Clamp(yawOffset, -maxYawOffset, maxYawOffset);
        pitchOffset = Mathf.Clamp(pitchOffset, -maxPitchOffset, maxPitchOffset);
        rollOffset = followRoll ? Mathf.Clamp(rollOffset, -maxRollOffset, maxRollOffset) : 0f;

        Quaternion constrained = Quaternion.Euler(
            baseLocalEuler.x + pitchOffset,
            baseLocalEuler.y + yawOffset,
            baseLocalEuler.z + rollOffset
        );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            constrained,
            followSpeed * Time.deltaTime
        );
    }
}
