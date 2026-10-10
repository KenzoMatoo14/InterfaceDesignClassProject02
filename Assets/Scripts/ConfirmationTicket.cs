using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Va en ConfirmationTicket. Muestra la hora de recogida y de entrega de la reservación,
// abre el panel de "Send to email" y, al cerrar, aparta el visor y regresa al inicio.
public class ConfirmationTicket : MonoBehaviour
{
    [Header("Referencias (se llenan solas si se dejan vacías)")]
    public TMP_Text pickUpText;
    public Button closeButton;
    public Button sendToEmailButton;
    [Tooltip("Panel hermano de ConfirmationTicket dentro del Canvas")]
    public GameObject sendToEmailPanel;
    public Button emailCloseButton;
    public QuestHeadsetManager headsetManager;

    [Header("Textos")]
    public string dropOffLocation = "booth x";

    static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");

    bool initialized;
    HeadsetCard reservedHeadset;
    DateTime returnBy;

    void Initialize()
    {
        if (initialized) return;
        initialized = true;

        if (pickUpText == null) pickUpText = FindChild<TMP_Text>(transform, "BottomPanel/PickUpPanel/PickUpText");
        if (closeButton == null) closeButton = FindChild<Button>(transform, "MiddlePanel/XButton");
        if (sendToEmailButton == null) sendToEmailButton = FindChild<Button>(transform, "BottomPanel/SendToEmailButton");

        // transform.Find sí encuentra objetos desactivados
        if (sendToEmailPanel == null && transform.parent != null)
        {
            Transform panel = transform.parent.Find("SendToEmailPanel");
            if (panel != null) sendToEmailPanel = panel.gameObject;
            else Debug.LogWarning("No se encontró 'SendToEmailPanel' en el Canvas", this);
        }
        if (emailCloseButton == null && sendToEmailPanel != null)
            emailCloseButton = FindChild<Button>(sendToEmailPanel.transform, "XButton");

        if (headsetManager == null) headsetManager = FindFirstObjectByType<QuestHeadsetManager>();

        if (closeButton != null) closeButton.onClick.AddListener(FinishAndGoHome);
        if (sendToEmailButton != null) sendToEmailButton.onClick.AddListener(OpenSendToEmail);
        if (emailCloseButton != null) emailCloseButton.onClick.AddListener(FinishAndGoHome);
    }

    T FindChild<T>(Transform root, string path) where T : Component
    {
        Transform child = root.Find(path);
        if (child == null)
        {
            Debug.LogWarning($"[{root.name}] No se encontró el hijo '{path}'", this);
            return null;
        }
        return child.GetComponent<T>();
    }

    public void Show(HeadsetCard headset, DateTime pickUp, DateTime returnTime)
    {
        Initialize();
        reservedHeadset = headset;
        returnBy = returnTime;

        gameObject.SetActive(true);
        transform.SetAsLastSibling(); // que quede encima de los demás paneles

        if (pickUpText != null)
        {
            pickUpText.text =
                $"Pick up time {FormatTime(pickUp)}.\n" +
                $"Return by {FormatTime(returnTime)}.\n" +
                $"Drop off at {dropOffLocation}.";
        }
    }

    void OpenSendToEmail()
    {
        if (sendToEmailPanel == null) return;
        gameObject.SetActive(false);
        sendToEmailPanel.SetActive(true);
        sendToEmailPanel.transform.SetAsLastSibling();
    }

    // X del ticket o del panel de email: aparta el visor y regresa a la pantalla inicial
    void FinishAndGoHome()
    {
        if (reservedHeadset != null)
        {
            // "Ready to use in" = minutos que faltan para que lo devuelvan
            float minutes = (float)(returnBy - DateTime.Now).TotalMinutes;
            reservedHeadset.SetStatus(HeadsetStatus.Reserved, Mathf.Max(1f, minutes));
            reservedHeadset = null;
        }

        if (headsetManager != null) headsetManager.RefreshAll();

        if (sendToEmailPanel != null) sendToEmailPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    // "6:00 pm today", "7:30 pm tomorrow", "9:00 am Mon 12"
    static string FormatTime(DateTime time)
    {
        string hour = time.ToString("h:mm tt", English).ToLower();

        if (time.Date == DateTime.Today) return $"{hour} today";
        if (time.Date == DateTime.Today.AddDays(1)) return $"{hour} tomorrow";
        return $"{hour} {time.ToString("ddd d", English)}";
    }
}
