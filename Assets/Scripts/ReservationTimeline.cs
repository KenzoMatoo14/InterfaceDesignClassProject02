using System;
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
    [Tooltip("Hora a la que abre (si la hora actual es antes, empieza aquí)")]
    public int openHour = 6;
    [Tooltip("Hora a la que cierra (última columna)")]
    public int closeHour = 20;

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
    [Range(0f, 1f)] public float randomOccupiedChance = 0.4f;
    public int randomSeed = 1234;

    public event Action<DateTime> OnSlotSelected;
    public DateTime? SelectedSlot { get; private set; }

    static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");

    readonly Dictionary<Button, DateTime> cellTimes = new Dictionary<Button, DateTime>();
    Button selectedCell;

    void Start()
    {
        Build();
    }

    public void Build()
    {
        Clear(dayColumn);
        Clear(gridContent);
        cellTimes.Clear();
        selectedCell = null;
        SelectedSlot = null;

        GetStart(out DateTime firstDay, out TimeSpan firstTime);
        List<DateTime> days = GetDays(firstDay);
        List<TimeSpan> times = GetTimes(firstTime);

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
                bool occupied = IsOccupied(slot);

                Button cell = Instantiate(cellPrefab, row);
                cell.name = slot.ToString("ddd HH:mm", English);
                SetSize(cell.gameObject, cellSize.x, cellSize.y);
                cell.image.color = occupied ? occupiedColor : freeColor;
                cell.interactable = !occupied;

                // Que una celda ocupada no se vea gris por estar desactivada
                ColorBlock colors = cell.colors;
                colors.disabledColor = Color.white;
                cell.colors = colors;

                if (!occupied)
                {
                    cellTimes[cell] = slot;
                    cell.onClick.AddListener(() => Select(cell));
                }
            }
        }
    }

    // Primer día y primera hora: la hora actual redondeada hacia abajo al bloque
    void GetStart(out DateTime firstDay, out TimeSpan firstTime)
    {
        DateTime now = DateTime.Now;
        int minutes = (int)now.TimeOfDay.TotalMinutes;
        minutes -= minutes % slotMinutes;

        firstDay = now.Date;
        firstTime = TimeSpan.FromMinutes(minutes);

        if (firstTime < TimeSpan.FromHours(openHour))
            firstTime = TimeSpan.FromHours(openHour);

        // Si ya cerró, empieza mañana a la hora de apertura
        if (firstTime >= TimeSpan.FromHours(closeHour))
        {
            firstDay = firstDay.AddDays(1);
            firstTime = TimeSpan.FromHours(openHour);
        }
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

    List<TimeSpan> GetTimes(TimeSpan firstTime)
    {
        List<TimeSpan> times = new List<TimeSpan>();
        TimeSpan end = TimeSpan.FromHours(closeHour);
        for (TimeSpan t = firstTime; t < end; t += TimeSpan.FromMinutes(slotMinutes))
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

    void Select(Button cell)
    {
        if (selectedCell != null) selectedCell.image.color = freeColor;

        selectedCell = cell;
        selectedCell.image.color = selectedColor;
        SelectedSlot = cellTimes[cell];

        Debug.Log($"Seleccionado: {SelectedSlot.Value.ToString("dddd d, HH:mm", English)}");
        OnSlotSelected?.Invoke(SelectedSlot.Value);
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
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }
}
