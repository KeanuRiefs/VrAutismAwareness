using UnityEngine;


public class GlassesManager : MonoBehaviour
{
    [Header("Setup Socket & Animasi")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor glassesSocket;
    public Animator aimanAnimator;

    public void OnGlassesAttached()
    {
        Debug.Log("Cermin mata berjaya dipasang kat muka Aiman!");

        // Contoh kalau nak petik suis animasi kat sini
        if (aimanAnimator != null)
        {
            aimanAnimator.SetTrigger("Calm");
        }
    }

    public void OnGlassesRemoved()
    {
        Debug.Log("Cermin mata dicabut!");
        // Boleh letak apa-apa efek kalau player cabut balik cermin mata
    }
}