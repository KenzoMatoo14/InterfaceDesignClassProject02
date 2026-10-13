using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TimelineReservation
{
    [Tooltip("Days from today (0 = today, 1 = tomorrow...)")]
    public int dayOffset = 0;
    [Tooltip("Start time, HH:mm format")]
    public string startTime = "09:00";
    public int durationMinutes = 60;
}

// Builds the days x hours grid, scrolled to the current time.
public class ReservationTimeline : MonoBehaviour
{
    [Header("Containers")]
    [Tooltip("Fixed left column where the days go")]
    public RectTransform dayColumn;
    [Tooltip("ScrollRect Content, where the hours and cells go")]
    public RectTransform gridContent;

    [Header("Prefabs")]
    public Button cellPrefab;
    public TMP_Text hourLabelPrefab;
    public TMP_Text dayLabelPrefab;

    [Header("Range")]
    public int daysToShow = 5;
    public bool skipWeekends = true;
    public int slotMinutes = 30;
    [Tooltip("First column (0 = midnight)")]
    public int openHour = 6;
    [Tooltip("Closing time (24 = midnight)")]
    public int closeHour = 20;
    [Tooltip("On start, scrolls so the current time is on the left")]
    public bool scrollToCurrentTime = true;

    [Header("Sizes")]
    public Vector2 cellSize = new Vector2(45, 35);
    public float headerHeight = 25;
    public float dayColumnWidth = 70;
    public float spacing = 2;

    [Header("Auto fit")]
    [Tooltip("Square cells that fill the Viewport height. Headers and text scale with them")]
    public bool fitToHeight = true;
    [Tooltip("Height of the hours row, relative to the cell size")]
    public float headerRatio = 0.6f;
    [Tooltip("Width of the days column, relative to the cell size")]
    public float dayColumnRatio = 1.8f;
    [Tooltip("Font size of the hours, relative to the cell size")]
    public float hourFontRatio = 0.32f;
    [Tooltip("Font size of the days, relative to the cell size")]
    public float dayFontRatio = 0.36f;

    [Header("Colors")]
    public Color freeColor = new Color32(0xEC, 0xEC, 0xEE, 0xFF);
    public Color occupiedColor = new Color32(0xF0, 0x66, 0x66, 0xFF);
    public Color selectedColor = new Color32(0x6F, 0xB7, 0xF2, 0xFF);
    [Header("Occupancy")]
    public List<TimelineReservation> reservations = new List<TimelineReservation>();
    [Tooltip("Fills random occupied cells to test the design")]
    public bool randomDemoData = true;
    [Range(0f, 1f)] public float randomOccupiedChance = 0.2f;
    public int randomSeed = 1234;

    // Headset being shown; its booked times appear in red. Set by ReservationPanel.
    [NonSerialized] public HeadsetCard headset;

    [Header("Selection")]
    [Tooltip("Maximum number of consecutive slots that can be selected (3 x 30 min = 90 min)")]
    public int maxSelectedSlots = 3;

    public event Action OnSelectionChanged;
    // Start of the selection (null = nothing selected)
    public DateTime? SelectedSlot { get; private set; }
    public int SelectedSlotCount { get; private set; }
    // End of the selection = start + selected slots
    public DateTime? SelectedEnd => SelectedSlot?.AddMinutes(SelectedSlotCount * slotMinutes);

    static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");

    readonly Dictionary<DateTime, Button> freeCells = new Dictionary<DateTime, Button>();
    bool built;

    void Start()
    {
        // If ReservationPanel already built it when opening, don't build it again
        if (!built) Build();
    }

    public void Build()
    {
        built = true;

        // When entering from MainMenu, the random hours use its settings (same in both scenes)
        HeadsetSession session = HeadsetSession.Instance;
        if (session != null)
        {
            randomDemoData = session.randomDemoData;
            randomOccupiedChance = session.randomOccupiedChance;
        }

        Clear(dayColumn);
        Clear(gridContent);
        freeCells.Clear();
        SelectedSlot = null;
        SelectedSlotCount = 0;

        DateTime now = DateTime.Now;
        GetStart(now, out DateTime firstDay, out TimeSpan currentSlot);
        List<DateTime> days = GetDays(firstDay);
        List<TimeSpan> times = GetTimes();

        if (fitToHeight) FitToViewport(days.Count);

        SetupVertical(dayColumn);
        SetupVertical(gridContent);

        // Empty corner above the days, to line up with the hours row
        CreateSpacer(dayColumn, dayColumnWidth, headerHeight);

        // Hours row
        RectTransform header = CreateRow(gridContent, "HourRow");
        foreach (TimeSpan time in times)
        {
            TMP_Text label = Instantiate(hourLabelPrefab, header);
            label.text = $"{time.Hours}:{time.Minutes:00}";
            SetSize(label.gameObject, cellSize.x, headerHeight);
            if (fitToHeight) SetFontSize(label, cellSize.y * hourFontRatio);
        }

        // One row per day
        foreach (DateTime day in days)
        {
            TMP_Text dayLabel = Instantiate(dayLabelPrefab, dayColumn);
            dayLabel.text = day.ToString("ddd d", English).ToUpper();
            SetSize(dayLabel.gameObject, dayColumnWidth, cellSize.y);
            if (fitToHeight) SetFontSize(dayLabel, cellSize.y * dayFontRatio);

            RectTransform row = CreateRow(gridContent, day.ToString("ddd"));
            foreach (TimeSpan time in times)
            {
                DateTime slot = day.Date + time;
                // Hours that already passed are marked as occupied
                bool past = slot.AddMinutes(slotMinutes) <= now;
                // Times this headset already has booked
                bool busy = headset != null && headset.IsBooked(slot, slot.AddMinutes(slotMinutes));
                bool occupied = past || busy || IsOccupied(slot);

                Button cell = Instantiate(cellPrefab, row);
                cell.name = slot.ToString("ddd HH:mm", English);
                SetSize(cell.gameObject, cellSize.x, cellSize.y);
                cell.image.color = occupied ? occupiedColor : freeColor;
                cell.interactable = !occupied;

                // So Unity doesn't tint disabled cells gray
                ColorBlock colors = cell.colors;
                colors.disabledColor = Color.white;
                cell.colors = colors;

                if (cell.interactable)
                {
                    freeCells[slot] = cell;
                    cell.onClick.AddListener(() => OnCellClicked(slot));
                }
            }
        }

        // Fix the days column width so the panel layout doesn't squash it
        LayoutElement dayColumnLayout = dayColumn.GetComponent<LayoutElement>();
        if (dayColumnLayout == null) dayColumnLayout = dayColumn.gameObject.AddComponent<LayoutElement>();
        dayColumnLayout.minWidth = dayColumnLayout.preferredWidth = dayColumnWidth;
        dayColumnLayout.flexibleWidth = 0;
        dayColumn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dayColumnWidth);

        FitScrollViewWidth();

        // Lay out the whole panel now, without waiting for the next frame
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);

        if (scrollToCurrentTime)
        {
            int column = times.IndexOf(currentSlot);
            ScrollToColumn(column);
            // Repeated one frame later, once Unity has finished the layout
            if (isActiveAndEnabled) StartCoroutine(ScrollNextFrame(column));
        }
    }

    // The ScrollView takes exactly the space left in the panel after the days column
    void FitScrollViewWidth()
    {
        RectTransform viewport = gridContent.parent as RectTransform;
        RectTransform scrollView = viewport != null ? viewport.parent as RectTransform : null;
        RectTransform panel = (RectTransform)transform;
        if (scrollView == null || scrollView.parent != panel) return;

        float width = panel.rect.width - dayColumnWidth;
        HorizontalOrVerticalLayoutGroup panelLayout = panel.GetComponent<HorizontalOrVerticalLayoutGroup>();
        if (panelLayout != null)
            width -= panelLayout.padding.left + panelLayout.padding.right + panelLayout.spacing;

        scrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(0f, width));
    }

    IEnumerator ScrollNextFrame(int column)
    {
        yield return null;
        ScrollToColumn(column);
    }

    // First day (always today) and the current time column (rounded down to the slot)
    void GetStart(DateTime now, out DateTime firstDay, out TimeSpan currentSlot)
    {
        int minutes = (int)now.TimeOfDay.TotalMinutes;
        minutes -= minutes % slotMinutes;

        firstDay = now.Date;
        currentSlot = TimeSpan.FromMinutes(minutes);

        if (currentSlot < TimeSpan.FromHours(openHour))
            currentSlot = TimeSpan.FromHours(openHour);

        // If it's already closed, stay on today's last column (all of today shows in red)
        TimeSpan lastSlot = TimeSpan.FromHours(closeHour) - TimeSpan.FromMinutes(slotMinutes);
        if (currentSlot > lastSlot)
            currentSlot = lastSlot;
    }

    void ScrollToColumn(int column)
    {
        RectTransform viewport = gridContent.parent as RectTransform;
        if (viewport == null || column < 0) return;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridContent);
        float maxScroll = Mathf.Max(0f, gridContent.rect.width - viewport.rect.width);
        float x = Mathf.Min(column * (cellSize.x + spacing), maxScroll);

        // Stop the ScrollRect inertia so it doesn't move the position back
        ScrollRect scrollRect = gridContent.GetComponentInParent<ScrollRect>();
        if (scrollRect != null) scrollRect.StopMovement();

        Vector2 position = gridContent.anchoredPosition;
        position.x = -x;
        gridContent.anchoredPosition = position;
    }

    List<DateTime> GetDays(DateTime firstDay)
    {
        List<DateTime> days = new List<DateTime>();
        DateTime day = firstDay;
        while (days.Count < daysToShow)
        {
            bool weekend = day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday;
            // Today always shows, even on weekends
            if (day == firstDay || !skipWeekends || !weekend) days.Add(day);
            day = day.AddDays(1);
        }
        return days;
    }

    // Every column of the day, from opening to closing time
    List<TimeSpan> GetTimes()
    {
        List<TimeSpan> times = new List<TimeSpan>();
        TimeSpan end = TimeSpan.FromHours(closeHour);
        for (TimeSpan t = TimeSpan.FromHours(openHour); t < end; t += TimeSpan.FromMinutes(slotMinutes))
            times.Add(t);
        return times;
    }

    bool IsOccupied(DateTime slot)
    {
        DateTime slotEnd = slot.AddMinutes(slotMinutes);
        foreach (TimelineReservation r in reservations)
        {
            if (!TimeSpan.TryParse(r.startTime, out TimeSpan start)) continue;
            DateTime resStart = DateTime.Today.AddDays(r.dayOffset) + start;
            DateTime resEnd = resStart.AddMinutes(r.durationMinutes);
            if (slot < resEnd && slotEnd > resStart) return true;
        }

        if (randomDemoData)
        {
            // Same result for the same cell every time it's built
            uint h = (uint)(slot.Ticks / TimeSpan.TicksPerMinute) ^ (uint)randomSeed;
            h ^= h >> 16; h *= 0x7feb352du; h ^= h >> 15; h *= 0x846ca68bu; h ^= h >> 16;
            return (h % 1000) / 1000f < randomOccupiedChance;
        }

        return false;
    }

    // Selection rules (always the same day and consecutive slots):
    // - Click next to the selection: extends it, up to maxSelectedSlots
    // - Click on an end of the selection: removes it
    // - Click anywhere else (or another day): starts a new selection there
    void OnCellClicked(DateTime slot)
    {
        if (!SelectedSlot.HasValue || SelectedSlot.Value.Date != slot.Date)
        {
            SetSelection(slot, 1);
            return;
        }

        DateTime start = SelectedSlot.Value;
        DateTime last = start.AddMinutes((SelectedSlotCount - 1) * slotMinutes);

        if (slot >= start && slot <= last)
        {
            if (SelectedSlotCount == 1) SetSelection(null, 0);
            else if (slot == start) SetSelection(start.AddMinutes(slotMinutes), SelectedSlotCount - 1);
            else if (slot == last) SetSelection(start, SelectedSlotCount - 1);
            else SetSelection(slot, 1);
            return;
        }

        DateTime newStart = slot < start ? slot : start;
        DateTime newLast = slot > last ? slot : last;
        int count = (int)((newLast - newStart).TotalMinutes / slotMinutes) + 1;

        if (count <= maxSelectedSlots && AllFree(newStart, count))
            SetSelection(newStart, count);
        else
            SetSelection(slot, 1);
    }

    bool AllFree(DateTime start, int count)
    {
        for (int i = 0; i < count; i++)
            if (!freeCells.ContainsKey(start.AddMinutes(i * slotMinutes))) return false;
        return true;
    }

    void SetSelection(DateTime? start, int count)
    {
        PaintSelection(freeColor);
        SelectedSlot = start;
        SelectedSlotCount = start.HasValue ? count : 0;
        PaintSelection(selectedColor);

        if (SelectedSlot.HasValue)
            Debug.Log($"Selected: {SelectedSlot.Value.ToString("dddd d, HH:mm", English)} - {SelectedEnd.Value:HH:mm}");
        OnSelectionChanged?.Invoke();
    }

    void PaintSelection(Color color)
    {
        if (!SelectedSlot.HasValue) return;
        for (int i = 0; i < SelectedSlotCount; i++)
            if (freeCells.TryGetValue(SelectedSlot.Value.AddMinutes(i * slotMinutes), out Button cell))
                cell.image.color = color;
    }

    // --- Layout helpers ---

    // Viewport height = header (cell * headerRatio) + rows (cell * rowCount) + spacing
    void FitToViewport(int rowCount)
    {
        RectTransform viewport = gridContent.parent as RectTransform;
        if (viewport == null || rowCount == 0) return;

        Canvas.ForceUpdateCanvases();
        float available = viewport.rect.height - spacing * rowCount;
        if (available <= 0) return;

        float cell = available / (rowCount + headerRatio);
        cellSize = new Vector2(cell, cell);
        headerHeight = cell * headerRatio;
        dayColumnWidth = cell * dayColumnRatio;
    }

    static void SetFontSize(TMP_Text label, float size)
    {
        label.enableAutoSizing = false;
        label.fontSize = size;
    }

    void SetupVertical(RectTransform container)
    {
        VerticalLayoutGroup layout = container.GetComponent<VerticalLayoutGroup>();
        if (layout == null) layout = container.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = container.GetComponent<ContentSizeFitter>();
        if (fitter == null) fitter = container.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    RectTransform CreateRow(Transform parent, string rowName)
    {
        GameObject row = new GameObject(rowName, typeof(RectTransform), typeof(HorizontalLayoutGroup));
        row.transform.SetParent(parent, false);

        HorizontalLayoutGroup layout = row.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = spacing;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        return (RectTransform)row.transform;
    }

    void CreateSpacer(Transform parent, float width, float height)
    {
        GameObject spacer = new GameObject("Corner", typeof(RectTransform));
        spacer.transform.SetParent(parent, false);
        SetSize(spacer, width, height);
    }

    static void SetSize(GameObject go, float width, float height)
    {
        LayoutElement element = go.GetComponent<LayoutElement>();
        if (element == null) element = go.AddComponent<LayoutElement>();
        element.minWidth = element.preferredWidth = width;
        element.minHeight = element.preferredHeight = height;
    }

    static void Clear(Transform container)
    {
        // Children are detached before being destroyed, because Destroy waits until the end
        // of the frame and the layout would otherwise still count them when rebuilding
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            child.SetParent(null, false);
            Destroy(child.gameObject);
        }
    }
}
