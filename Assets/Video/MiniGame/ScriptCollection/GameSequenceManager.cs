using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class GameSequenceManager : MonoBehaviour
{
    public VideoPlayer popupVideoPlayer;
    public GameObject popupScreen3D;

    public VideoPlayer boardVideoPlayer;
    public GameObject boardScreen3D;       // BARU: Objek Papan (untuk ditutup bila Quit)

    public VideoPlayer tutorialVideoPlayer;
    public GameObject tutorialScreen3D;    // BARU: Objek Tutorial keseluruhan (untuk ditutup bila Next/Quit)

    public AudioSource voiceOverSource;
    public AudioClip quizTimeVoice;
    public AudioSource bgmSource;

    void Start()
    {
        StartCoroutine(StartMiniGameSequence());
    }

    IEnumerator StartMiniGameSequence()
    {
        // 1. Hidupkan Pop-up
        if (popupScreen3D != null) popupScreen3D.SetActive(true);
        if (popupVideoPlayer != null) popupVideoPlayer.Play();
        if (voiceOverSource != null && quizTimeVoice != null) voiceOverSource.PlayOneShot(quizTimeVoice);

        // 2. Tunggu 4 saat
        yield return new WaitForSeconds(4f);

        // 3. TUTUP TERUS SKRIN POP-UP
        if (popupScreen3D != null) popupScreen3D.SetActive(false);

        // 4. Mainkan Papan Gerimis Sunyi
        if (boardScreen3D != null) boardScreen3D.SetActive(true);
        if (boardVideoPlayer != null) boardVideoPlayer.Play();

        // 5. Hidupkan Skrin Tutorial
        if (tutorialScreen3D != null) tutorialScreen3D.SetActive(true);
        if (tutorialVideoPlayer != null) tutorialVideoPlayer.Play();

        // 6. Mainkan BGM
        if (bgmSource != null) bgmSource.Play();
    }

    // --- FUNGSI UNTUK BUTANG ---

    // Dipanggil bila pemain tekan "Next"
    public void OnNextButtonClicked()
    {
        Debug.Log("Pemain tekan NEXT. Tutup tutorial, mula main!");
        // Tutup skrin tutorial sahaja. Papan mini game & BGM terus berjalan.
        if (tutorialScreen3D != null) tutorialScreen3D.SetActive(false);
        if (tutorialVideoPlayer != null) tutorialVideoPlayer.Stop();
    }

    // Dipanggil bila pemain tekan "Quit"
    public void OnQuitButtonClicked()
    {
        Debug.Log("Pemain tekan QUIT. Tutup semua mini game.");

        // Tutup semua skrin 3D
        if (tutorialScreen3D != null) tutorialScreen3D.SetActive(false);
        if (boardScreen3D != null) boardScreen3D.SetActive(false);
        if (popupScreen3D != null) popupScreen3D.SetActive(false);

        // Berhentikan semua audio & video
        if (tutorialVideoPlayer != null) tutorialVideoPlayer.Stop();
        if (boardVideoPlayer != null) boardVideoPlayer.Stop();
        if (bgmSource != null) bgmSource.Stop();
    }
}