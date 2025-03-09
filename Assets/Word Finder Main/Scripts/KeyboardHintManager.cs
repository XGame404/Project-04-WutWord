using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyboardHintManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private GameObject keyboard;
    private KeyboardKey[] keys;

    [Header(" UI Elements ")]
    [SerializeField] private TextMeshProUGUI keyboardPriceText;

    [Header(" Settings ")]
    [SerializeField] private int keyboardHintPrice;

    private void Awake()
    {
        keys = keyboard.GetComponentsInChildren<KeyboardKey>();
    }

    void Start()
    {
        keyboardPriceText.text = keyboardHintPrice.ToString();
    }

    public void UseKeyboardHint()
    {
        if (DataManager.instance.GetCoins() < keyboardHintPrice)
            return;

        string secretWord = WordManager.instance.GetSecretWord();

        List<KeyboardKey> untouchedKeys = new List<KeyboardKey>();

        foreach (var key in keys)
        {
            if (key.IsUntouched())
                untouchedKeys.Add(key);
        }

        List<KeyboardKey> nonSecretWordKeys = new List<KeyboardKey>(untouchedKeys);

        foreach (var key in untouchedKeys)
        {
            if (secretWord.Contains(key.GetLetter()))
                nonSecretWordKeys.Remove(key);
        }

        if (nonSecretWordKeys.Count <= 0)
            return;

        int randomKeyIndex = Random.Range(0, nonSecretWordKeys.Count);
        nonSecretWordKeys[randomKeyIndex].SetInvalid();

        DataManager.instance.RemoveCoins(keyboardHintPrice);
    }
}
