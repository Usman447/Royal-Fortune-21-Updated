using UnityEngine;
using TMPro;


public class Fps : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        var fps = Mathf.RoundToInt(1 / Time.deltaTime);
        text.text = fps.ToString();
    }
}
