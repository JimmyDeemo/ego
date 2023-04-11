using UnityEngine;
using UnityEngine.UIElements;

public class HudUI : MonoBehaviour
{
    public Label Score;
    public Label PrevScore;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        Score = root.Q("lblScore") as Label;
        PrevScore = root.Q("lblPrevScore") as Label;
    }
}
