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

// Va en cada tarjeta (Headset1..Headset6). Guarda el estado del visor y actualiza sus textos.
public class HeadsetCard : MonoBehaviour
{
    [Header("Estado inicial")]
    [Tooltip("Si se deja vacío, usa el texto que ya tiene QuestName")]
    public string headsetName = "";
    public HeadsetStatus status = HeadsetStatus.Available;
    [Tooltip("Minutos que faltan para que esté disponible")]
    public float minutesRemaining = 0f;

    [Header("Referencias (se llenan solas si se dejan vacías)")]
    public TMP_Text nameText;
    public TMP_Text statusText;
    public TMP_Text readyToUseText;
    public Button actionButton;
    public Image buttonImage;
    public TMP_Text buttonText;

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

        // Que el botón desactivado conserve el color que le pongamos (si no, Unity lo pinta gris)
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
            Debug.LogWarning($"[{name}] No se encontró el hijo '{path}'", this);
            return null;
        }
        return child.GetComponent<T>();
    }

    public void SetStatus(HeadsetStatus newStatus, float minutes)
    {
        status = newStatus;
        minutesRemaining = newStatus == HeadsetStatus.Available ? 0f : minutes;
    }

    // Llamado por el manager para redibujar la tarjeta
    public void Refresh(Color buttonColor)
    {
        int minutesToShow = Mathf.CeilToInt(minutesRemaining);
        bool available = status == HeadsetStatus.Available;

        if (nameText != null) nameText.text = headsetName;
        if (statusText != null) statusText.text = status.ToString();
        if (readyToUseText != null) readyToUseText.text = available ? "Ready to use" : "Ready to use in";
        if (buttonText != null) buttonText.text = available ? "Start" : $"{minutesToShow} min";
        if (buttonImage != null) buttonImage.color = buttonColor;
        if (actionButton != null) actionButton.interactable = available;
    }
}
