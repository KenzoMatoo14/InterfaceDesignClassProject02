using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Va en ReservePanel. Muestra los datos del Quest3 que se eligió.
public class ReservationPanel : MonoBehaviour
{
    [Header("Referencias (se llenan solas si se dejan vacías)")]
    public TMP_Text deviceText;
    public TMP_Text statusText;
    public Image questImage;
    public Button closeButton;
    public Button confirmButton;
    public ReservationTimeline timeline;
    public ConfirmationTicket confirmationTicket;

    [Header("Textos")]
    public string deviceModel = "Oculus Quest 3";

    [Tooltip("Cada visor usa una semilla distinta para que su ocupación sea diferente")]
    public int baseSeed = 1234;

    public HeadsetCard CurrentHeadset { get; private set; }

    bool initialized;

    void Initialize()
    {
        if (initialized) return;
        initialized = true;

        if (deviceText == null) deviceText = FindChild<TMP_Text>("UpperPanel/RightPanel/DeviceText");
        if (statusText == null) statusText = FindChild<TMP_Text>("UpperPanel/RightPanel/StatusText");
        if (questImage == null) questImage = FindChild<Image>("UpperPanel/LeftPanel/QuestImage");
        if (closeButton == null) closeButton = FindChild<Button>("UpperPanel/LeftPanel/XButton");
        if (confirmButton == null) confirmButton = FindChild<Button>("ButtonPanel/Confirm");
        if (timeline == null) timeline = GetComponentInChildren<ReservationTimeline>(true);
        // ConfirmationTicket empieza desactivado, por eso se busca incluyendo inactivos
        if (confirmationTicket == null)
            confirmationTicket = FindFirstObjectByType<ConfirmationTicket>(FindObjectsInactive.Include);

        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (confirmButton != null) confirmButton.onClick.AddListener(Confirm);

        // Confirm solo se puede presionar cuando hay una hora elegida
        if (timeline != null)
            timeline.OnSelectionChanged += () => SetConfirmEnabled(timeline.SelectedSlot.HasValue);
    }

    void SetConfirmEnabled(bool enabled)
    {
        if (confirmButton != null) confirmButton.interactable = enabled;
    }

    void Confirm()
    {
        if (timeline == null || !timeline.SelectedSlot.HasValue)
        {
            Debug.LogWarning("Elige una hora antes de confirmar.", this);
            return;
        }

        DateTime pickUp = timeline.SelectedSlot.Value;
        DateTime returnBy = timeline.SelectedEnd.Value;

        if (confirmationTicket == null)
        {
            Debug.LogWarning("No hay ConfirmationTicket. Agrégale el componente al objeto ConfirmationTicket.", this);
            return;
        }

        HeadsetCard headset = CurrentHeadset; // Close() lo borra
        Close();
        confirmationTicket.Show(headset, pickUp, returnBy);
    }

    T FindChild<T>(string path) where T : Component
    {
        Transform child = transform.Find(path);
        if (child == null)
        {
            Debug.LogWarning($"[{name}] No se encontró el hijo '{path}'", this);
            return null;
        }
        return child.GetComponent<T>();
    }

    public void Open(HeadsetCard card)
    {
        Initialize();
        CurrentHeadset = card;
        gameObject.SetActive(true);

        int number = GetDeviceNumber(card);
        if (deviceText != null) deviceText.text = $"DEVICE {number}: {deviceModel}";
        if (statusText != null) statusText.text = $"STATUS: {card.status.ToString().ToUpper()}";

        // Usa la misma imagen que tiene la tarjeta
        Image cardImage = card.transform.Find("QuestImage")?.GetComponent<Image>();
        if (questImage != null && cardImage != null) questImage.sprite = cardImage.sprite;

        if (timeline != null)
        {
            timeline.randomSeed = baseSeed + number * 7919;

            // Reserved, Loaned o Charging: ocupado desde ahora hasta que termine su tiempo
            // (mínimo 1 minuto, para que al menos la hora actual salga en rojo)
            if (card.status == HeadsetStatus.Available)
                timeline.busyUntil = null;
            else
                timeline.busyUntil = DateTime.Now.AddMinutes(Mathf.Max(1f, card.minutesRemaining));

            timeline.Build();
        }

        // Al abrir todavía no hay hora elegida
        SetConfirmEnabled(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        CurrentHeadset = null;
    }

    // "Quest3 01" -> 1. Si el nombre no termina en número, usa el del objeto (Headset1 -> 1)
    static int GetDeviceNumber(HeadsetCard card)
    {
        Match match = Regex.Match(card.headsetName ?? "", @"(\d+)\s*$");
        if (!match.Success) match = Regex.Match(card.name, @"(\d+)\s*$");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }
}
