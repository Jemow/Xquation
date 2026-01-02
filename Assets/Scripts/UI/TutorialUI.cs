using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] tutorialObjects;
    [SerializeField] private TextMeshProUGUI tutorialTmp;

    private GameObject _lastTutorialObject;
    
    private int _tutorialIndex;

    private void OnEnable() => UpdateTutorialObject();

    private void OnDisable() => _tutorialIndex = 0;

    public void IncrementIndex()
    {
        _tutorialIndex = (_tutorialIndex + 1) % tutorialObjects.Length;
        UpdateTutorialObject();
    }

    public void DecrementIndex()
    {
        _tutorialIndex = (_tutorialIndex - 1 + tutorialObjects.Length) % tutorialObjects.Length;
        UpdateTutorialObject();
    }

    private void UpdateTutorialObject()
    {
        if(_lastTutorialObject) _lastTutorialObject.SetActive(false);
        GameObject tutorialObject = tutorialObjects[_tutorialIndex];
        tutorialObject.SetActive(true);
        _lastTutorialObject = tutorialObject;
        tutorialTmp.SetText($"{_tutorialIndex + 1} / {tutorialObjects.Length}");
    }
}
