using TMPro;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    // Reference to the Game Over text UI element
    public TextMeshProUGUI gameOverText;

    void Start()
    {
        // Hide the Game Over text at start
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverText != null)
        {
            // Show the Game Over text
            gameOverText.gameObject.SetActive(true);

            // Ensure it's centered on screen
            RectTransform rectTransform = gameOverText.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }
}
