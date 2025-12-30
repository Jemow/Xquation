using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color defaultColor = Color.white;

    public Button Button { get; private set; }
    public TextMeshProUGUI Tmp { get; private set; }
    
    private Image _image;

    private void Awake()
    {
        Button = GetComponent<Button>();
        Tmp = GetComponentInChildren<TextMeshProUGUI>();
        _image = GetComponent<Image>();
        
        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (_image)
        {
            _image.color = isSelected ? selectedColor : defaultColor;
        }
    }
}