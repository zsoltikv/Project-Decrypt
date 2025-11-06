using UnityEngine;

public class ToggleSwitch : MonoBehaviour
{
    public GameObject lever;
    public bool isOn = false;
    public int index;

    public void SetState(bool on)
    {
        isOn = on;
        lever.transform.localRotation = Quaternion.Euler(0, 0, on ? 0f : 30f);
    }

    public void Toggle()
    {
        SetState(!isOn);
    }
}