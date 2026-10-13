using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Goes on ConfirmationTicket. Shows the reservation's pick-up and return times,
// opens the "Send to email" panel and, when closed, returns to the main screen.
public class ConfirmationTicket : MonoBehaviour
{
    [Header("References (filled automatically if left empty)")]
    public TMP_Text pickUpText;
    public Button closeButton;
    public Button sendToEmailButton;
    [Tooltip("Sibling panel of ConfirmationTicket inside the Canvas")]
    public GameObject sendToEmailPanel;
    public Button emailCloseButton;
    public QuestHeadsetManager headsetManager;

    [Header("Texts")]
    public string dropOffLocation = "booth x";

    static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");

    bool initialized;
    HeadsetCard reservedHeadset;
    DateTime pickUpTime;
    DateTime returnBy;

    void Initialize()
    {
        if (initialized) return;
        initialized = true;

        if (pickUpText == null) pickUpText = FindChild<TMP_Text>(transform, "BottomPanel/PickUpPanel/PickUpText");
        if (closeButton == null) closeButton = FindChild<Button>(transform, "MiddlePanel/XButton");
        if (sendToEmailButton == null) sendToEmailButton = FindChild<Button>(transform, "BottomPanel/SendToEmailButton");

        // transform.Find does find disabled objects
        if (sendToEmailPanel == null && transform.parent != null)
        {
            Transform panel = transform.parent.Find("SendToEmailPanel");
            if (panel != null) sendToEmailPanel = panel.gameObject;
            else Debug.LogWarning("'SendToEmailPanel' not found in the Canvas", this);
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
            Debug.LogWarning($"[{root.name}] Child '{path}' not found", this);
            return null;
        }
        return child.GetComponent<T>();
    }

    public void Show(HeadsetCard headset, DateTime pickUp, DateTime returnTime)
    {
        Initialize();
        reservedHeadset = headset;
        pickUpTime = pickUp;
        returnBy = returnTime;

        // The headset is booked and saved as soon as it's confirmed, so it isn't lost
        // even if the user leaves through another button (for example Gomainmenu)
        SaveReservation();

        gameObject.SetActive(true);
        transform.SetAsLastSibling(); // so it stays on top of the other panels

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

    // X on the ticket or on the email panel: returns to the main screen
    void FinishAndGoHome(){
        if (headsetManager != null) headsetManager.RefreshAll();

        if (sendToEmailPanel != null) sendToEmailPanel.SetActive(false);
        gameObject.SetActive(false);

        RecomendPage recomend = FindFirstObjectByType<RecomendPage>();
        if (recomend != null) recomend.Close();
    }

    // Sets the headset to Reserved, books the selected block and saves it to the file
    void SaveReservation()
    {
        if (reservedHeadset != null)
        {
            // Only the selected block (pick up -> return) is booked, on the real headset.
            // The card only changes to Reserved if that block is happening now
            HeadsetCard source = reservedHeadset.Source;
            source.AddBooking(pickUpTime, returnBy, HeadsetStatus.Reserved);
            if (headsetManager != null) source.UpdateFromBookings(headsetManager.reservedLeadMinutes);
            else source.UpdateFromBookings();

            // If it was a copy from the recommendation panel, show the same as the original
            if (reservedHeadset != source)
                reservedHeadset.SetStatus(source.status, source.minutesRemaining);

            reservedHeadset = null;
        }

        if (headsetManager != null)
        {
            headsetManager.RefreshAll();
            headsetManager.SaveState();
        }
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
