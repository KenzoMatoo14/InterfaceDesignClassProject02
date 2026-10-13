using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum HeadsetStatus
{
    Available,
    Reserved,
    Loaned,
    Charging
}

// Goes on each card (Headset1..Headset6). Stores the headset state and updates its texts.
public class HeadsetCard : MonoBehaviour
{
    [HideInInspector] public HeadsetCard original;
    [Header("Initial state")]
    [Tooltip("If left empty, uses the text QuestName already has")]
    public string headsetName = "";
    public HeadsetStatus status = HeadsetStatus.Available;
    [Tooltip("Minutes left until it becomes available")]
    public float minutesRemaining = 0f;

    [Header("References (filled automatically if left empty)")]
    public TMP_Text nameText;
    public TMP_Text statusText;
    public TMP_Text readyToUseText;
    public Button actionButton;
    public Image buttonImage;
    public TMP_Text buttonText;

    public struct Booking
    {
        public DateTime start;
        public DateTime end;
    }

    // Times booked for this headset (the ones shown in red on the timeline)
    [NonSerialized] public List<Booking> bookings = new List<Booking>();

    // Cards on the recommendation panel are copies; the real data lives in 'original'
    public HeadsetCard Source => original != null ? original : this;

    void Reset()
    {
        FindReferences();
    }

    public void FindReferences()
    {
        if (nameText == null) nameText = FindChild<TMP_Text>("UpperText/QuestName");
        if (statusText == null) statusText = FindChild<TMP_Text>("UpperText/Status");
        if (readyToUseText == null) readyToUseText = FindChild<TMP_Text>("ReadyToUse");
        if (actionButton == null) actionButton = FindChild<Button>("StartQuest");
        if (buttonImage == null) buttonImage = FindChild<Image>("StartQuest");
        if (buttonText == null) buttonText = FindChild<TMP_Text>("StartQuest/Text (TMP)");

        if (string.IsNullOrEmpty(headsetName) && nameText != null)
            headsetName = nameText.text;

        // Keep the color we set when the button is disabled (otherwise Unity tints it gray)
        if (actionButton != null)
        {
            ColorBlock colors = actionButton.colors;
            colors.disabledColor = Color.white;
            actionButton.colors = colors;
        }
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

    public void SetStatus(HeadsetStatus newStatus, float minutes)
    {
        status = newStatus;

        minutesRemaining = newStatus == HeadsetStatus.Available ? 0f : minutes;
    }

    // Turns the Inspector state into a booked block: from now until it becomes free
    public void InitBookings()
    {
        bookings.Clear();
        if (status != HeadsetStatus.Available)
        {
            DateTime now = DateTime.Now;
            AddBooking(now, now.AddMinutes(Mathf.Max(1f, minutesRemaining)));
        }
    }

    public void AddBooking(DateTime start, DateTime end)
    {
        bookings.Add(new Booking { start = start, end = end });
    }

    public bool IsBooked(DateTime slotStart, DateTime slotEnd)
    {
        foreach (Booking booking in bookings)
            if (slotStart < booking.end && slotEnd > booking.start) return true;
        return false;
    }

    // Called by the manager to redraw the card
    public void Refresh(Color buttonColor)
    {
        int minutesToShow = Mathf.CeilToInt(minutesRemaining);
        bool available = status == HeadsetStatus.Available;

        string textmin;
        if(minutesToShow >= 60){
            int hour = minutesToShow / 60;
            int minute = minutesToShow % 60;
            textmin = $"{hour}h {minute:00}min";
        }else{
            textmin = $"{minutesToShow} min";
        }

        if (nameText != null) nameText.text = headsetName;
        if (statusText != null) statusText.text = status.ToString();
        if (readyToUseText != null) readyToUseText.text = available ? "Ready to use" : "Ready to use in";
        if (buttonText != null) buttonText.text = available ? "Start" : textmin;
        if (buttonImage != null) buttonImage.color = buttonColor;
        if (actionButton != null) actionButton.interactable = available;
    }
}
