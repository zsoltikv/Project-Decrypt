using UnityEngine;
using UnityEngine.UI;

public class LightsOffPuzzle : MonoBehaviour
{
    public ToggleSwitch[] switches;
    private bool[] state;

    void Start()
    {
        state = new bool[switches.Length];

        for (int i = 0; i < switches.Length; i++)
        {
            state[i] = false;
            switches[i].SetState(false);

            int index = i;

            Button btn = switches[i].GetComponentInChildren<Button>();
            switches[i].GetComponent<ToggleSwitch>().index = i;
            btn.onClick.AddListener(() => Press(index));
        }
    }

    void Press(int index)
    {
        Toggle(index);

        if (index - 1 >= 0)
            Toggle(index - 1);

        if (index + 1 < switches.Length)
            Toggle(index + 1);

        CheckWin();
    }

    void Toggle(int index)
    {
        state[index] = !state[index];
        switches[index].SetState(state[index]);
    }

    void CheckWin()
    {
        foreach (bool b in state)
            if (!b) return;

        Debug.Log("✅ MINDEN KAPCSOLÓ BEKAPCSOLVA — GYŐZELEM!");
    }
}
    