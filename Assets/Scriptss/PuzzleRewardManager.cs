using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PuzzleColor { Blue, Green, Red, Yellow }

public class PuzzleRewardManager : MonoBehaviour
{
    [Header("UI Canvas Group")]
    [SerializeField] private CanvasGroup puzzlePopUpCanvasGroup;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float spawnDistance = 1.4f;
    [SerializeField] private float displayDuration = 3.5f;

    [Header("Slow/Gentle Animation Settings")]
    [SerializeField] private float fadeTime = 1.5f;
    [SerializeField] private float slideUpAmount = 0.3f;

    [Header("Auto Scene Transition")]
    [SerializeField] private bool autoLoadNextScene = true;
    [SerializeField] private string nextSceneName = "Cafe 2";

    [Header("Debug & Testing")]
    [Tooltip("Check this ONLY in the very first scene of the game to wipe old saves.")]
    [SerializeField] private bool clearSaveOnStart = false;

    [Header("UI Colors")]
    [Tooltip("Color of the puzzle piece in the UI before it is collected.")]
    [SerializeField] private Color uncollectedColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // Default dark/transparent
    [Tooltip("Color of the puzzle piece in the UI after it is collected.")]
    [SerializeField] private Color collectedColor = Color.white;

    [Header("Individual Puzzle UI Images")]
    [SerializeField] private Image uiBluePiece;
    [SerializeField] private Image uiGreenPiece;
    [SerializeField] private Image uiRedPiece;
    [SerializeField] private Image uiYellowPiece;

    private Coroutine activePopUpRoutine;
    private Vector3 standardScale = new Vector3(0.002f, 0.002f, 0.002f);

    private void Start()
    {
        if (clearSaveOnStart)
        {
            ResetAllProgress();
        }

        if (puzzlePopUpCanvasGroup != null)
        {
            puzzlePopUpCanvasGroup.alpha = 0f;
            puzzlePopUpCanvasGroup.transform.localScale = Vector3.zero;
        }

        LoadProgress();
    }

    public void CollectPiece(string colorName)
    {
        if (System.Enum.TryParse(colorName, out PuzzleColor collectedColorEnum))
        {
            SetPieceColor(collectedColorEnum);

            PlayerPrefs.SetInt("Puzzle_" + colorName, 1);
            PlayerPrefs.Save();

            if (activePopUpRoutine != null)
            {
                StopCoroutine(activePopUpRoutine);
            }
            activePopUpRoutine = StartCoroutine(PlaySlowPopUpSequence(collectedColorEnum));
        }
    }

    private void SetPieceColor(PuzzleColor color)
    {
        switch (color)
        {
            case PuzzleColor.Blue:
                if (uiBluePiece != null) uiBluePiece.color = collectedColor;
                break;
            case PuzzleColor.Green:
                if (uiGreenPiece != null) uiGreenPiece.color = collectedColor;
                break;
            case PuzzleColor.Red:
                if (uiRedPiece != null) uiRedPiece.color = collectedColor;
                break;
            case PuzzleColor.Yellow:
                if (uiYellowPiece != null) uiYellowPiece.color = collectedColor;
                break;
        }
    }

    private void LoadProgress()
    {
        if (PlayerPrefs.GetInt("Puzzle_Blue", 0) == 1) SetPieceColor(PuzzleColor.Blue);
        if (PlayerPrefs.GetInt("Puzzle_Green", 0) == 1) SetPieceColor(PuzzleColor.Green);
        if (PlayerPrefs.GetInt("Puzzle_Red", 0) == 1) SetPieceColor(PuzzleColor.Red);
        if (PlayerPrefs.GetInt("Puzzle_Yellow", 0) == 1) SetPieceColor(PuzzleColor.Yellow);
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey("Puzzle_Blue");
        PlayerPrefs.DeleteKey("Puzzle_Green");
        PlayerPrefs.DeleteKey("Puzzle_Red");
        PlayerPrefs.DeleteKey("Puzzle_Yellow");
        PlayerPrefs.Save();

        // Now it uses the custom color you set in the inspector!
        if (uiBluePiece != null) uiBluePiece.color = uncollectedColor;
        if (uiGreenPiece != null) uiGreenPiece.color = uncollectedColor;
        if (uiRedPiece != null) uiRedPiece.color = uncollectedColor;
        if (uiYellowPiece != null) uiYellowPiece.color = uncollectedColor;

        Debug.Log("Puzzle Progress wiped & UI reset to Uncollected Color!");
    }

    private IEnumerator PlaySlowPopUpSequence(PuzzleColor color)
    {
        Vector3 finalTargetPos = Vector3.zero;

        if (playerCamera != null)
        {
            finalTargetPos = playerCamera.position + (playerCamera.forward * spawnDistance);
            finalTargetPos.y = playerCamera.position.y;

            Vector3 spawnStartPos = finalTargetPos - new Vector3(0, slideUpAmount, 0);

            puzzlePopUpCanvasGroup.transform.position = spawnStartPos;
            puzzlePopUpCanvasGroup.transform.rotation = Quaternion.LookRotation(finalTargetPos - playerCamera.position);
        }

        puzzlePopUpCanvasGroup.transform.localScale = standardScale;

        float elapsedTime = 0f;
        Vector3 initialSpawnPos = puzzlePopUpCanvasGroup.transform.position;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeTime;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            puzzlePopUpCanvasGroup.alpha = Mathf.Lerp(0f, 1f, smoothProgress);
            puzzlePopUpCanvasGroup.transform.position = Vector3.Lerp(initialSpawnPos, finalTargetPos, smoothProgress);

            yield return null;
        }

        puzzlePopUpCanvasGroup.alpha = 1f;
        puzzlePopUpCanvasGroup.transform.position = finalTargetPos;

        yield return new WaitForSeconds(displayDuration);

        elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeTime;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            puzzlePopUpCanvasGroup.alpha = Mathf.Lerp(1f, 0f, smoothProgress);
            yield return null;
        }

        puzzlePopUpCanvasGroup.alpha = 0f;
        puzzlePopUpCanvasGroup.transform.localScale = Vector3.zero;

        if (autoLoadNextScene)
        {
            if (color == PuzzleColor.Green)
            {
                if (PlayerPrefs.GetInt("Puzzle_Blue", 0) == 1)
                {
                    Debug.Log("Green (Last Piece) collected! Loading Cafe...");
                    SceneManager.LoadScene(nextSceneName);
                }
            }
            else if (color == PuzzleColor.Blue)
            {
                if (PlayerPrefs.GetInt("Puzzle_Green", 0) == 1)
                {
                    Debug.Log("Blue (Last Piece) collected! Loading Cafe...");
                    SceneManager.LoadScene(nextSceneName);
                }
            }
        }
    }
}