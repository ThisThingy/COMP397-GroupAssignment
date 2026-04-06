using System;
using UnityEngine;

public abstract class SaveableBehaviour : MonoBehaviour
{
    [SerializeField] private string saveId;
    [SerializeField] private int loadPriority = 0;

    public string SaveId => saveId;
    public int LoadPriority => loadPriority;

    // Load 前：先关掉会干扰恢复的脚本
    public virtual void BeforeRestore() { }

    // Save：把当前状态转成 json
    public abstract string CaptureAsJson();

    // Load：从 json 恢复状态
    public abstract void RestoreFromJson(string json);

    // Load 后：重新启用控制、物理、相机、UI 等
    public virtual void AfterRestore() { }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(saveId))
        {
            saveId = Guid.NewGuid().ToString("N");
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}