using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SlotMachine : MonoBehaviour
{
    [Header("Reel Images")]
    [SerializeField] private Image reel1Image;
    [SerializeField] private Image reel2Image;
    [SerializeField] private Image reel3Image;

    [Header("Slot Symbols")]
    [SerializeField] private Sprite[] symbols;

    [Header("UI")]
    [SerializeField] private Button spinButton;
    [SerializeField] private TMP_Text resultText;

    [Header("Spin Settings")]
    [SerializeField] private float reel1SpinDuration = 1.0f;
    [SerializeField] private float reel2SpinDuration = 1.5f;
    [SerializeField] private float reel3SpinDuration = 2.0f;
    [SerializeField] private float symbolChangeSpeed = 0.08f;

    [Header("Payouts")]
    [SerializeField] private int cherryPayout = 100;
    [SerializeField] private int bellPayout = 200;
    [SerializeField] private int lemonPayout = 50;
    [SerializeField] private int defaultPayout = 75;

    private bool isSpinning = false;

    private void Start()
    {
        if (resultText != null)
        {
            resultText.text = "Spin to win!";
            resultText.color = Color.white;
        }
    }

    public void Spin()
    {
        if (isSpinning)
            return;

        if (symbols == null || symbols.Length == 0)
        {
            Debug.LogError("Symbols array is empty. Please assign sprites in Inspector.");
            return;
        }

        StartCoroutine(SpinReels());
    }

    private IEnumerator SpinReels()
    {
        isSpinning = true;
        spinButton.interactable = false;

        resultText.text = "Spinning...";
        resultText.color = Color.white;

        Coroutine reel1 = StartCoroutine(SpinSingleReel(reel1Image, reel1SpinDuration));
        Coroutine reel2 = StartCoroutine(SpinSingleReel(reel2Image, reel2SpinDuration));
        Coroutine reel3 = StartCoroutine(SpinSingleReel(reel3Image, reel3SpinDuration));

        yield return reel1;
        yield return reel2;
        yield return reel3;

        CheckResult();

        spinButton.interactable = true;
        isSpinning = false;
    }

    private IEnumerator SpinSingleReel(Image reelImage, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            int randomIndex = Random.Range(0, symbols.Length);
            reelImage.sprite = symbols[randomIndex];

            yield return new WaitForSeconds(symbolChangeSpeed);
            elapsedTime += symbolChangeSpeed;
        }

        int finalIndex = Random.Range(0, symbols.Length);
        reelImage.sprite = symbols[finalIndex];
    }

    private void CheckResult()
    {
        Sprite sprite1 = reel1Image.sprite;
        Sprite sprite2 = reel2Image.sprite;
        Sprite sprite3 = reel3Image.sprite;

        if (sprite1 == sprite2 && sprite2 == sprite3)
        {
            int payout = GetPayout(sprite1);
            resultText.text = "YOU WIN! +" + payout + " Coins";
            resultText.color = Color.green;
            Debug.Log("Player won. Payout: " + payout);
        }
        else
        {
            resultText.text = "TRY AGAIN";
            resultText.color = Color.red;
            Debug.Log("Player lost.");
        }
    }

    private int GetPayout(Sprite winningSprite)
    {
        if (winningSprite == null)
            return defaultPayout;

        string spriteName = winningSprite.name.ToLower();

        if (spriteName.Contains("cherry"))
            return cherryPayout;

        if (spriteName.Contains("bell"))
            return bellPayout;

        if (spriteName.Contains("lemon"))
            return lemonPayout;

        return defaultPayout;
    }
}