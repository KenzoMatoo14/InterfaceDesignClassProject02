using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecomendPage : MonoBehaviour
{
    [Header("Headsets on the recomended panel")]
    public Transform headsetContainer;
    public List<HeadsetCard> headsets = new List<HeadsetCard>();

    [Header("Reservation panel")]
    public ReservationPanel reservationPanel;

    [Header("Card button color")]
    public Color buttonColor = new Color32(0xE8, 0xC4, 0xC8, 0xFF);

    [Header("Main menu Headsets (needs this info to take a desition)")]
    public QuestHeadsetManager headsetManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(HeadsetCard card in headsets){
            card.FindReferences();
            HeadsetCard captured = card;

            // each card is now a button
            Button cardButton = card.GetComponent<Button>();
            if (cardButton == null) cardButton = card.gameObject.AddComponent<Button>();
            cardButton.targetGraphic = card.GetComponent<Image>();
            cardButton.onClick.AddListener(() => OpenReservation(captured));

            if (card.actionButton != null)
                card.actionButton.onClick.AddListener(() => OpenReservation(captured));
        }

        HeadsetCard best1 = null;
        HeadsetCard best2 = null;


        foreach(HeadsetCard pastCard in headsetManager.headsets){
            if(best1 == null || best1.minutesRemaining >=pastCard.minutesRemaining){
                best2 = best1;
                best1 = pastCard;
            }else if(best2 == null || best2.minutesRemaining >= pastCard.minutesRemaining){
                best2 = pastCard;
            }
            
        }

        if (headsetManager == null)
            headsetManager = FindFirstObjectByType<QuestHeadsetManager>();


        
    }

    void OpenReservation(HeadsetCard card)
    {
        if (reservationPanel == null)
        {
            Debug.LogWarning("No hay ReservationPanel. Agrégale el componente a ReservePanel.", this);
            return;
        }
        reservationPanel.Open(card);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
