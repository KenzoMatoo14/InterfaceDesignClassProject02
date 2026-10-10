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
    [Tooltip("Días a partir de hoy (0 = hoy, 1 = mañana...)")]
    public int dayOffset = 0;
    [Tooltip("Hora de inicio, formato HH:mm")]
    public string startTime = "09:00";
    public int durationMinutes = 60;
}

// Genera la tabla de días x horas empezando en la hora actual.
public class ReservationTimeline : MonoBehaviour
{
    [Header("Contenedores")]
    [Tooltip("Columna fija de la izquierda donde van los días")]
    public RectTransform dayColumn;
    [Tooltip("Content del ScrollRect, donde van las horas y las celdas")]
    public RectTransform gridContent;

    [Header("Prefabs")]
    public Button cellPrefab;
    public TMP_Text hourLabelPrefab;
    public TMP_Text dayLabelPrefab;

    [Header("Rango")]
    public int daysToShow = 5;
    public bool skipWeekends = true;
    public int slotMinutes = 30;
    [Tooltip("Primera columna (0 = medianoche)")]
    public int openHour = 6;
    [Tooltip("Hora a la que cierra (24 = medianoche)")]
    public int closeHour = 20;
    [Tooltip("Al iniciar, mueve el scroll para que la hora actual quede a la izquierda")]
    public bool scrollToCurrentTime = true;

    [Header("Tamaños")]
    public Vector2 cellSize = new Vector2(45, 35);
    public float headerHeight = 25;
    public float dayColumnWidth = 70;
    public float spacing = 2;

    [Header("Ajuste automático")]
    [Tooltip("Celdas cuadradas que llenan el alto del Viewport. Encabezados y textos se escalan con ellas")]
    public bool fitToHeight = true;
    [Tooltip("Alto de la fila de horas, relativo al tamaño de la celda")]
    public float headerRatio = 0.6f;
    [Tooltip("Ancho de la columna de días, relativo al tamaño de la celda")]
    public float dayColumnRatio = 1.8f;
    [Tooltip("Tamaño de letra de las horas, relativo al tamaño de la celda")]
    public float hourFontRatio = 0.32f;
    [Tooltip("Tamaño de letra de los días, relativo al tamaño de la celda")]
    public float dayFontRatio = 0.36f;

    [Header("Colores")]
    public Color freeColor = new Color32(0xEC, 0xEC, 0xEE, 0xFF);
    public Color occupiedColor = new Color32(0xF0, 0x66, 0x66, 0xFF);
    public Color selectedColor = new Color32(0x6F, 0xB7, 0xF2, 0xFF);
    [Header("Ocupación")]
    public List<TimelineReservation> reservations = new List<TimelineReservation>();
    [Tooltip("Rellena celdas ocupadas al azar para probar el diseño")]
    public bool randomDemoData = true;
    [Range(0f, 1f)] public float randomOccupiedChance = 0.2f;
    public int randomSeed = 1234;

    // Hasta cuándo está ocupado el visor actual (null = libre). Lo pone ReservationPanel.
    [NonSerialized] public DateTime? busyUntil;

    [Header("Selección")]
    [Tooltip("Máximo de casillas seguidas que se pueden elegir (3 x 30 min = 90 min)")]
    public int maxSelectedSlots = 3;

    public event Action OnSelectionChanged;
    // Inicio de la selección (null = nada elegido)
    public DateTime? SelectedSlot { get; private set; }
    public int SelectedSlotCount { get; private set; }
    // Fin de la selección = inicio + casillas elegidas
    public DateTime? SelectedEnd => SelectedSlot?.AddMinutes(SelectedSlotCount * slotMinutes);

    static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");

    readonly Dictionary<DateTime, Button> freeCells = new Dictionary<DateTime, Button>();
    bool built;

    void Start()
    {
        // Si ReservationPanel ya la construyó al abrirse, no se repite
        if (!built) Build();
    }

    public void Build()
    {
        built = true;
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

        // Esquina vacía arriba de los días, para alinear con la fila de horas
        CreateSpacer(dayColumn, dayColumnWidth, headerHeight);

        // Fila de horas
        RectTransform header = CreateRow(gridContent, "HourRow");
        foreach (TimeSpan time in times)
        {
            TMP_Text label = Instantiate(hourLabelPrefab, header);
            label.text = $"{time.Hours}:{time.Minutes:00}";
            SetSize(label.gameObject, cellSize.x, headerHeight);
            if (fitToHeight) SetFontSize(label, cellSize.y * hourFontRatio);
        }

        // Una fila por día
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
                // Las horas que ya pasaron se marcan como ocupadas
                bool past = slot.AddMinutes(slotMinutes) <= now;
                // Si el visor está reservado o prestado, desde ahora hasta que se libere
                bool busy = busyUntil.HasValue && slot < busyUntil.Value;
                bool occupied = past || busy || IsOccupied(slot);

                Button cell = Instantiate(cellPrefab, row);
                cell.name = slot.ToString("ddd HH:mm", English);
                SetSize(cell.gameObject, cellSize.x, cellSize.y);
                cell.image.color = occupied ? occupiedColor : freeColor;
                cell.interactable = !occupied;

                // Que una celda desactivada no se vea gris por Unity
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

        // Fija el ancho de la columna de días para que el layout del panel no la aplaste
        LayoutElement dayColumnLayout = dayColumn.GetComponent<LayoutElement>();
        if (dayColumnLayout == null) dayColumnLayout = dayColumn.gameObject.AddComponent<LayoutElement>();
        dayColumnLayout.minWidth = dayColumnLayout.preferredWidth = dayColumnWidth;
        dayColumnLayout.flexibleWidth = 0;
        dayColumn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dayColumnWidth);

        FitScrollViewWidth();

        // Acomoda todo el panel ya, sin esperar al siguiente frame
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);

        if (scrollToCurrentTime)
        {
            int column = times.IndexOf(currentSlot);
            ScrollToColumn(column);
            // Se repite un frame después, cuando Unity ya terminó de acomodar el layout
            if (isActiveAndEnabled) StartCoroutine(ScrollNextFrame(column));
        }
    }

    // El ScrollView ocupa exactamente lo que sobra del panel después de la columna de días
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

    // Primer día y la columna de la hora actual (redondeada hacia abajo al bloque)
    void GetStart(DateTime now, out DateTime firstDay, out TimeSpan currentSlot)
    {
        int minutes = (int)now.TimeOfDay.TotalMinutes;
        minutes -= minutes % slotMinutes;

        firstDay = now.Date;
        currentSlot = TimeSpan.FromMinutes(minutes);

        if (currentSlot < TimeSpan.FromHours(openHour))
            currentSlot = TimeSpan.FromHours(openHour);

        // Si ya cerró, empieza mañana a la hora de apertura
        if (currentSlot >= TimeSpan.FromHours(closeHour))
        {
            firstDay = firstDay.AddDays(1);
            currentSlot = TimeSpan.FromHours(openHour);
        }
    }

    void ScrollToColumn(int column)
    {
        RectTransform viewport = gridContent.parent as RectTransform;
        if (viewport == null || column < 0) return;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridContent);
        float maxScroll = Mathf.Max(0f, gridContent.rect.width - viewport.rect.width);
        float x = Mathf.Min(column * (cellSize.x + spacing), maxScroll);

        // Frena la inercia del ScrollRect para que no regrese la posición
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
            if (!skipWeekends || !weekend) days.Add(day);
            day = day.AddDays(1);
        }
        return days;
    }

    // Todas las columnas del día, de la hora de apertura a la de cierre
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
            // Mismo resultado para la misma celda cada vez que se construye
            uint h = (uint)(slot.Ticks / TimeSpan.TicksPerMinute) ^ (uint)randomSeed;
            h ^= h >> 16; h *= 0x7feb352du; h ^= h >> 15; h *= 0x846ca68bu; h ^= h >> 16;
            return (h % 1000) / 1000f < randomOccupiedChance;
        }

        return false;
    }

    // Reglas de selección (siempre en el mismo día y casillas seguidas):
    // - Click junto a la selección: la extiende, hasta maxSelectedSlots
    // - Click en un extremo de la selección: lo quita
    // - Click en otro lugar (u otro día): empieza una selección nueva ahí
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
            Debug.Log($"Seleccionado: {SelectedSlot.Value.ToString("dddd d, HH:mm", English)} - {SelectedEnd.Value:HH:mm}");
        OnSelectionChanged?.Invoke();
    }

    void PaintSelection(Color color)
    {
        if (!SelectedSlot.HasValue) return;
        for (int i = 0; i < SelectedSlotCount; i++)
            if (freeCells.TryGetValue(SelectedSlot.Value.AddMinutes(i * slotMinutes), out Button cell))
                cell.image.color = color;
    }

    // --- Helpers de layout ---

    // Alto del viewport = encabezado (cell * headerRatio) + filas (cell * rowCount) + espacios
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
        // Se sacan del contenedor antes de destruirlos, porque Destroy espera al final del
        // frame y si no, el layout los seguiría contando al reconstruir
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            child.SetParent(null, false);
            Destroy(child.gameObject);
        }
    }
}
