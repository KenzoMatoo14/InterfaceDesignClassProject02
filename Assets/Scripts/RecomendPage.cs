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
        if (headsetManager == null)
            headsetManager = FindFirstObjectByType<QuestHeadsetManager>();

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

    }

    void OnEnable(){
        if (headsetManager == null)
            headsetManager = FindFirstObjectByType<QuestHeadsetManager>();

        if (headsetManager != null)
            winnerBestToEnd();
    }

    public void Close()
    {
        if (headsetManager != null && headsetManager.headsetContainer != null)
            headsetManager.headsetContainer.gameObject.SetActive(true);

        gameObject.SetActive(false);
    }

    //a void funtion that takes the nearest to completion on the reserbation time and updates the canvas so it shows all the info it needs
    void winnerBestToEnd(){
        //variable to save the 2 bests cards
        HeadsetCard best1 = null;
        HeadsetCard best2 = null;

        //serches for each previous card the two nereast to compleation and saves it on best1 and best 2
        foreach(HeadsetCard pastCard in headsetManager.headsets){
            if(best1 == null || best1.minutesRemaining >=pastCard.minutesRemaining){
                best2 = best1;
                best1 = pastCard;
            }else if(best2 == null || best2.minutesRemaining >= pastCard.minutesRemaining){
                best2 = pastCard;
            }
            
        }
        //the list saves best 1 and best 2 for easy acces to each viarable
        List<HeadsetCard> bestOfBest = new List<HeadsetCard>();
        bestOfBest.Add(best1);
        bestOfBest.Add(best2);
        //paste the information acording to the 2 new cards and the 2 best from the other panel
        for(int i = 0; i<headsets.Count; i++){
            headsets[i].SetStatus(bestOfBest[i].status, bestOfBest[i].minutesRemaining);
            headsets[i].headsetName = bestOfBest[i].headsetName;
            headsets[i].Refresh(buttonColor);
            headsets[i].original = bestOfBest[i];
        }

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

   
}
