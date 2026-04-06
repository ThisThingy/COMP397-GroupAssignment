using System;
using UnityEngine;

public class PlayerTransformSaveable : SaveableBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Rigidbody rb;

    [Header("Disable Before Restore")]
    [SerializeField] private Behaviour[] disableBeforeRestore;

    [Header("Enable After Restore")]
    [SerializeField] private Behaviour[] enableAfterRestore;

    [Serializable]
    private class PlayerTransformState
    {
        public float posX;
        public float posY;
        public float posZ;
        public float rotX;
        public float rotY;
        public float rotZ;
    }

    public override void BeforeRestore()
    {
        if (characterController != null)
            characterController.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (disableBeforeRestore != null)
        {
            foreach (var b in disableBeforeRestore)
            {
                if (b != null) b.enabled = false;
            }
        }
    }

    public override string CaptureAsJson()
    {
        PlayerTransformState state = new PlayerTransformState
        {
            posX = transform.position.x,
            posY = transform.position.y,
            posZ = transform.position.z,
            rotX = transform.eulerAngles.x,
            rotY = transform.eulerAngles.y,
            rotZ = transform.eulerAngles.z
        };

        return JsonUtility.ToJson(state);
    }

    public override void RestoreFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return;

        PlayerTransformState state = JsonUtility.FromJson<PlayerTransformState>(json);
        if (state == null) return;

        transform.position = new Vector3(state.posX, state.posY, state.posZ);
        transform.rotation = Quaternion.Euler(state.rotX, state.rotY, state.rotZ);
    }

    public override void AfterRestore()
    {
        if (rb != null)
            rb.isKinematic = false;

        if (characterController != null)
            characterController.enabled = true;

        if (enableAfterRestore != null)
        {
            foreach (var b in enableAfterRestore)
            {
                if (b != null) b.enabled = true;
            }
        }
    }
}