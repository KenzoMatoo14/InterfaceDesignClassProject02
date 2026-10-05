using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Controla todas las tarjetas y los contadores de arriba.
public class QuestHeadsetManager : MonoBehaviour
{
    [Header("Tarjetas (si se deja vacío, toma las hijas de LowerPanel)")]
    public Transform headsetContainer;
    public List<HeadsetCard> headsets = new List<HeadsetCard>();

    [Header("Contadores (si se dejan vacíos, se buscan por nombre)")]
    public TMP_Text availableCount;
    public TMP_Text reservedCount;
    public TMP_Text loanedCount;
    public TMP_Text chargingCount;
    public TMP_Text subtitleText;

    [Header("Préstamo")]
    [Tooltip("Minutos que se muestran al presionar Start")]
    public float loanMinutes = 60f;

    [Header("Color del botón")]
    public Color buttonColor = new Color32(0xE8, 0xC4, 0xC8, 0xFF);

    void Start()
    {
        FindReferences();

        foreach (HeadsetCard card in headsets)
        {
            card.FindReferences();
            if (card.actionButton != null)
            {
                HeadsetCard captured = card;
                card.actionButton.onClick.AddListener(() => OnStartPressed(captured));
            }
        }

        RefreshAll();
    }

    void OnStartPressed(HeadsetCard card)
    {
        if (card.status != HeadsetStatus.Available) return;

        card.SetStatus(HeadsetStatus.Loaned, loanMinutes);
        Debug.Log($"{card.headsetName} prestado por {loanMinutes} min");
        RefreshAll();
    }

    public void RefreshAll()
    {
        foreach (HeadsetCard card in headsets)
            card.Refresh(buttonColor);

        UpdateCounters();
    }

    void UpdateCounters()
    {
        int available = 0, reserved = 0, loaned = 0, charging = 0;
        foreach (HeadsetCard card in headsets)
        {
            switch (card.status)
            {
                case HeadsetStatus.Available: available++; break;
                case HeadsetStatus.Reserved: reserved++; break;
                case HeadsetStatus.Loaned: loaned++; break;
                case HeadsetStatus.Charging: charging++; break;
            }
        }

        if (availableCount != null) availableCount.text = available.ToString();
        if (reservedCount != null) reservedCount.text = reserved.ToString();
        if (loanedCount != null) loanedCount.text = loaned.ToString();
        if (chargingCount != null) chargingCount.text = charging.ToString();

        if (subtitleText != null)
            subtitleText.text = available > 0
                ? "Select an available headset to begin."
                : "No headsets available right now.";
    }

    void FindReferences()
    {
        if (headsetContainer == null)
        {
            GameObject lowerPanel = GameObject.Find("LowerPanel");
            if (lowerPanel != null) headsetContainer = lowerPanel.transform;
        }

        if (headsets.Count == 0 && headsetContainer != null)
            headsets.AddRange(headsetContainer.GetComponentsInChildren<HeadsetCard>());

        if (availableCount == null) availableCount = FindCounter("Status1Available");
        if (reservedCount == null) reservedCount = FindCounter("Status2Reserved");
        if (loanedCount == null) loanedCount = FindCounter("Status3Loaned");
        if (chargingCount == null) chargingCount = FindCounter("Status4Charging");

        if (subtitleText == null)
        {
            GameObject subtitle = GameObject.Find("Subtitle");
            if (subtitle != null) subtitleText = subtitle.GetComponent<TMP_Text>();
        }

        if (headsets.Count == 0)
            Debug.LogWarning("No se encontraron tarjetas. ¿Les agregaste el componente HeadsetCard?", this);
    }

    TMP_Text FindCounter(string statusObjectName)
    {
        GameObject statusObject = GameObject.Find(statusObjectName);
        if (statusObject == null)
        {
            Debug.LogWarning($"No se encontró '{statusObjectName}'", this);
            return null;
        }

        Transform number = statusObject.transform.Find("Number");
        return number != null ? number.GetComponent<TMP_Text>() : null;
    }
}
