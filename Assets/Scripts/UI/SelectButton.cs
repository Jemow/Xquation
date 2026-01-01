using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image backgroundImage;
    
    [Header("Parameters")]
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color defaultColor = Color.white;

    [Header("Rarity")] 
    [SerializeField] private Sprite commonImage;
    [SerializeField] private Sprite rareImage;
    [SerializeField] private Sprite epicImage;

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
        if (backgroundImage) backgroundImage.color = isSelected ? selectedColor : defaultColor;
    }

    public void SetRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: _image.sprite = commonImage; break;
            case Rarity.Rare: _image.sprite = rareImage; break;
            case Rarity.Epic: _image.sprite = epicImage; break;
        }
    }
}