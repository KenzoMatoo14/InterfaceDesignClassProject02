using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Controls all the cards and the counters at the top.
public class QuestHeadsetManager : MonoBehaviour
{
    [Header("Cards (if left empty, uses the children of LowerPanel)")]
    public Transform headsetContainer;
    public List<HeadsetCard> headsets = new List<HeadsetCard>();

    [Header("Counters (if left empty, they're found by name)")]
    public TMP_Text availableCount;
    public TMP_Text reservedCount;
    public TMP_Text loanedCount;
    public TMP_Text chargingCount;
    public TMP_Text subtitleText;

    [Header("Reservation (if left empty, it's found in the scene)")]
    public ReservationPanel reservationPanel;
    [Header("Recomendation panel")]
    public RecomendPage recomandationPanel;

    [Header("Button color")]
    public Color buttonColor = new Color32(0xE8, 0xC4, 0xC8, 0xFF);

    void Start()
    {
        FindReferences();

        foreach (HeadsetCard card in headsets)
        {
            card.FindReferences();
            card.InitBookings();
            HeadsetCard captured = card;

            // The whole card works as a button
            Button cardButton = card.GetComponent<Button>();
            if (cardButton == null) cardButton = card.gameObject.AddComponent<Button>();
            cardButton.targetGraphic = card.GetComponent<Image>();

            cardButton.onClick.AddListener(() => OpenReservationOrRecomend(captured));
            if (card.actionButton != null)
                card.actionButton.onClick.AddListener(() => OpenReservationOrRecomend(captured));
        }

        // Always loads the saved data. Resetting the data is controlled from
        // HeadsetSession in MainMenu, so CaseA and CaseB stay in sync
        HeadsetDataStore.Load(headsets);

        RefreshAll();
    }

    // Saves status, minutes and booked hours for the next scene or the next Play
    public void SaveState()
    {
        HeadsetDataStore.Save(headsets);
    }

    void OpenReservation(HeadsetCard card)
    {
        if (reservationPanel == null)
        {
            Debug.LogWarning("No ReservationPanel found. Add the component to ReservePanel.", this);
            return;
        }
        reservationPanel.Open(card);
    }
    public void OpenRecomended()
    {
        if (recomandationPanel == null)
        {
            Debug.LogWarning("No RecomendPage assigned.", this);
            return;
        }

        // The background cards stay visible; Recomend is already after Background
        // in the Canvas, so it's drawn on top of them
        recomandationPanel.gameObject.SetActive(true);
    }

    void OpenReservationOrRecomend(HeadsetCard card)
    {
        if (SceneManager.GetActiveScene().name == "CaseB")
            OpenRecomended();
        else
            OpenReservation(card);
    }



    public void RefreshAll()
    {
        foreach (HeadsetCard card in headsets)
            card.Refresh(buttonColor);

        UpdateCounters();



        foreach(HeadsetCard card in headsets){
            Button cbutton = card.GetComponent<Button>();
            if(card.status != HeadsetStatus.Available ){
                ColorBlock colors = cbutton.colors;
                //gets the disable button color
                Color bye = colors.disabledColor;
                colors.normalColor = bye;
                colors.highlightedColor = bye;
                colors.selectedColor = bye;
                cbutton.colors = colors;

            }
            else{
                ColorBlock colors = cbutton.colors;
                colors.normalColor = new Color(1,1,1,1);
                colors.highlightedColor = new Color(0.96f, 0.96f, 0.96f, 1);
                colors.selectedColor = new Color(0.96f, 0.96f, 0.96f, 1);
                cbutton.colors = colors;
            }
        }
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
        if (recomandationPanel == null && SceneManager.GetActiveScene().name == "CaseB")
            recomandationPanel = FindFirstObjectByType<RecomendPage>(FindObjectsInactive.Include);
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

        // ReservePanel starts disabled, so the search includes inactive objects
        if (reservationPanel == null)
            reservationPanel = FindFirstObjectByType<ReservationPanel>(FindObjectsInactive.Include);

        if (headsets.Count == 0)
            Debug.LogWarning("No cards found. Did you add the HeadsetCard component to them?", this);
    }

    TMP_Text FindCounter(string statusObjectName)
    {
        GameObject statusObject = GameObject.Find(statusObjectName);
        if (statusObject == null)
        {
            Debug.LogWarning($"'{statusObjectName}' not found", this);
            return null;
        }

        Transform number = statusObject.transform.Find("Number");
        return number != null ? number.GetComponent<TMP_Text>() : null;
    }
}
