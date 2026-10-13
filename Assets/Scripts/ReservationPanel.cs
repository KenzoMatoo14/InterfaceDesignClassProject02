using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Goes on ReservePanel. Shows the data of the selected Quest3.
public class ReservationPanel : MonoBehaviour
{
    [Header("References (filled automatically if left empty)")]
    public TMP_Text deviceText;
    public TMP_Text statusText;
    public Image questImage;
    public Button closeButton;
    public Button confirmButton;
    public ReservationTimeline timeline;
    public ConfirmationTicket confirmationTicket;

    [Header("Texts")]
    public string deviceModel = "Oculus Quest 3";

    [Tooltip("Each headset uses a different seed so its occupancy is different")]
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
        // ConfirmationTicket starts disabled, so the search includes inactive objects
        if (confirmationTicket == null)
            confirmationTicket = FindFirstObjectByType<ConfirmationTicket>(FindObjectsInactive.Include);

        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (confirmButton != null) confirmButton.onClick.AddListener(Confirm);

        // Confirm can only be pressed when a time is selected
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
            Debug.LogWarning("Select a time before confirming.", this);
            return;
        }

        DateTime pickUp = timeline.SelectedSlot.Value;
        DateTime returnBy = timeline.SelectedEnd.Value;

        if (confirmationTicket == null)
        {
            Debug.LogWarning("No ConfirmationTicket found. Add the component to the ConfirmationTicket object.", this);
            return;
        }

        HeadsetCard headset = CurrentHeadset; // Close() clears it
        Close();
        confirmationTicket.Show(headset, pickUp, returnBy);
    }

    T FindChild<T>(string path) where T : Component
    {
        Transform child = transform.Find(path);
        if (child == null)
        {
            Debug.LogWarning($"[{name}] Child '{path}' not found", this);
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

        // Uses the same image as the card
        Image cardImage = card.transform.Find("QuestImage")?.GetComponent<Image>();
        if (questImage != null && cardImage != null) questImage.sprite = cardImage.sprite;

        if (timeline != null)
        {
            // When entering from MainMenu, the seed comes from there (same in CaseA and CaseB)
            int seed = HeadsetSession.Instance != null ? HeadsetSession.Instance.randomSeed : baseSeed;
            timeline.randomSeed = seed + number * 7919;

            // If it's a copy from the recommendation panel, use the original headset's times
            timeline.headset = card.Source;

            timeline.Build();
        }

        // Nothing is selected yet when opening
        SetConfirmEnabled(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        CurrentHeadset = null;
    }

    // "Quest3 01" -> 1. If the name doesn't end in a number, uses the object's (Headset1 -> 1)
    static int GetDeviceNumber(HeadsetCard card)
    {
        Match match = Regex.Match(card.headsetName ?? "", @"(\d+)\s*$");
        if (!match.Success) match = Regex.Match(card.name, @"(\d+)\s*$");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }
}
