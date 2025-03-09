using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LetterHintManager : MonoBehaviour
{
    public static LetterHintManager instance;

    [SerializeField] private TextMeshProUGUI letterPriceText;
    private List<int> letterHintGivenIndices = new List<int>();
    private const int maxHints = 4;
    private bool shouldReset = false;
    private bool adsRemoved = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    private void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.Game && shouldReset)
        {
            letterHintGivenIndices.Clear();
            shouldReset = false;
        }
        else if (gameState == GameState.LevelComplete || gameState == GameState.Gameover)
        {
            shouldReset = true;
        }
    }

    public void RequestLetterHint()
    {
        if (adsRemoved)
        {
            Debug.Log("Giving the hint letter");
            GiveLetterHint();
        }
        else
        {
            Debug.Log("Request for hint letter. Show the ads");
            RewardedAdsManager.instance.ShowRewardedAd();
        }
    }

    public void GiveLetterHint()
    {
        if (letterHintGivenIndices.Count >= maxHints)
        {
            Debug.Log("Maximum the hint letter");
            return;
        }

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if (!letterHintGivenIndices.Contains(i))
                availableIndices.Add(i);
        }

        if (availableIndices.Count == 0) return;

        WordContainer currentWordContainer = InputManager.instance.GetCurrentWordContainer();
        string secretWord = WordManager.instance.GetSecretWord();

        int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        letterHintGivenIndices.Add(randomIndex);

        currentWordContainer.AddAsHint(randomIndex, secretWord[randomIndex]);

        Debug.Log("The letter hint is shown in " + randomIndex);
    }
}
