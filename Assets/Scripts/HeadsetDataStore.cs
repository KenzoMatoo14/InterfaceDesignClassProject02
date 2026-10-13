using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Saves the headsets' state (status, minutes and booked hours) to a JSON file,
// so it's kept when switching between CaseA and CaseB and when pressing Play again.
public static class HeadsetDataStore
{
    [Serializable]
    class BookingData
    {
        public long start; // DateTime.Ticks (JsonUtility can't save DateTime)
        public long end;
        public HeadsetStatus status;
    }

    [Serializable]
    class HeadsetData
    {
        public string name;
        public HeadsetStatus status;
        public float minutesRemaining;
        public List<BookingData> bookings = new List<BookingData>();
    }

    [Serializable]
    class SaveData
    {
        public List<HeadsetData> headsets = new List<HeadsetData>();
    }

    static string FilePath => Path.Combine(Application.persistentDataPath, "headsets.json");

    public static void Save(List<HeadsetCard> cards)
    {
        SaveData data = new SaveData();
        foreach (HeadsetCard card in cards)
        {
            HeadsetData headset = new HeadsetData
            {
                name = card.headsetName,
                status = card.status,
                minutesRemaining = card.minutesRemaining
            };
            foreach (HeadsetCard.Booking booking in card.bookings)
                headset.bookings.Add(new BookingData { start = booking.start.Ticks, end = booking.end.Ticks, status = booking.status });
            data.headsets.Add(headset);
        }

        try
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(data, true));
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Couldn't save the headsets: {e.Message}");
        }
    }

    public static void Clear()
    {
        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Couldn't delete the saved headsets: {e.Message}");
        }
    }

    // Applies the saved data to each card (matched by headsetName).
    // Cards without saved data use the Inspector values, and are saved right away
    // so the other scene finds them the same.
    public static void Load(List<HeadsetCard> cards)
    {
        bool missing = false;

        Dictionary<string, HeadsetData> saved = new Dictionary<string, HeadsetData>();
        try
        {
            if (File.Exists(FilePath))
            {
                SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath));
                if (data != null)
                    foreach (HeadsetData headset in data.headsets)
                        saved[headset.name] = headset;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Couldn't read the saved headsets: {e.Message}");
        }

        foreach (HeadsetCard card in cards)
        {
            if (!saved.TryGetValue(card.headsetName, out HeadsetData headset))
            {
                card.InitBookings();
                missing = true;
                continue;
            }

            card.SetStatus(headset.status, headset.minutesRemaining);
            card.bookings.Clear();
            foreach (BookingData booking in headset.bookings)
                card.AddBooking(new DateTime(booking.start), new DateTime(booking.end), booking.status);
        }

        if (missing) Save(cards);
    }
}
