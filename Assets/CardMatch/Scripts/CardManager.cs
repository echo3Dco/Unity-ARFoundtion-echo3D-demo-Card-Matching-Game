using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Linq;
using System;
using System.Collections;

/// <summary>
/// CardManager manages 
/// </summary>
public class CardManager : MonoBehaviour
{
    #region Properties
    List<GameObject> SelectedCards;
    [SerializeField]
    ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    Camera arCam;
    [SerializeField]
    TMPro.TextMeshPro textMesh;
    GameObject cardMatchRoot;
    bool cardsInit = false;
    Coroutine MsgCoroutine;
    #endregion

    #region Constants
    private const int MAX_SELECTION = 2;

    #endregion

    #region Monobehaviour Methods
    void Start()
    {
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        SelectedCards = new List<GameObject>();
        cardsInit = false;
    }

    void Update()
    {
        if (cardMatchRoot == null)
        {
            cardMatchRoot = GameObject.Find("CardMatchRoot");
        }

        if (!cardsInit)
        {
            //check that all cards are now ready
            if (GetCardCount() > 0 && GetCardItems().All(n => n.isInit))
            {
                PopMessage("Ready!");
                cardsInit = true;
            }
        }

        if (Input.touchCount == 0) { return; }

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit))
                {

                    if (hit.collider.gameObject.tag == CardMatch.CARD_TAG
                        && !hit.collider.gameObject.GetComponent<CardItem>().selected)
                    {
                        // hit on a card that has not been selected: 
                        // initiate SELECT animation
                        // add card to SelectedCards
                        GameObject gO = hit.collider.gameObject;
                        CardItem cardItem = gO.GetComponent<CardItem>();
                        cardItem.Select();
                        try
                        {
                            AddSelectedCard(gO);
                        }
                        catch (Exception ex)
                        {
                            textMesh.text = ex.Message;
                        }

                        HandleMatch();
                    }
                    else if (hit.collider.gameObject.tag == CardMatch.CARD_TAG
                        && hit.collider.gameObject.GetComponent<CardItem>().selected)
                    {
                        // hit on a card that has already been selected: 
                        // initiate UNSELECT animation
                        // remove card from SelectedCards ls
                        GameObject gO = hit.collider.gameObject;
                        CardItem cardItem = gO.GetComponent<CardItem>();
                        cardItem.Unselect();
                        RemoveSelectedCard(gO);
                    }
                }
            }
            // else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            // {

            // }

            // if (Input.GetTouch(0).phase == TouchPhase.Ended)
            // {
            //     selectedCard = null;
            // }
        }
    }
    #endregion

    #region Methods

    /// <summary>
    /// Display a message for 3 seconds.
    /// </summary>
    /// <param name="msg"></param>
    void PopMessage(string msg)
    {
        if (MsgCoroutine != null)
        {
            StopCoroutine(MsgCoroutine);
        }

        MsgCoroutine = StartCoroutine(PopMessageCo(msg));
    }

    /// <summary>
    /// Shows message for two seconds then goes away
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    IEnumerator PopMessageCo(string msg)
    {
        textMesh.text = msg;

        yield return new WaitForSecondsRealtime(3f);

        textMesh.text = "";

        yield return null;
    }

    /// <summary>
    /// Checks for and handles card states 
    /// </summary>
    void HandleMatch()
    {
        // check to see if all the selected match
        if (SelectedCards.Count == MAX_SELECTION && HasMatch())
        {
            // broadcast a message to all selected cards to fade away
            PopMessage("Match! Yay!");
            //cardMatchRoot.BroadcastMessage("Fadeout");
            foreach (var card in SelectedCards)
            {
                card.GetComponent<CardItem>().Fadeout();
            }

            SelectedCards.Clear();

            // check for win
            var allCards = GetCardItems();
            if (allCards.Count() == 0) { textMesh.text = "Winner!"; }
        }
        else if (SelectedCards.Count == MAX_SELECTION)
        {
            PopMessage("No Match :(");
            // the max number is selected but there is no match, so clear out the variables
            RemoveSelectedCardAll();
        }
    }

    /// <summary>
    /// Checks to see if all the cards match
    /// </summary>
    /// <returns></returns>
    bool HasMatch()
    {
        textMesh.text = "checking for match";

        return (SelectedCards.Count == MAX_SELECTION && SelectedCards.All(n => n.GetComponent<CardItem>().ID == SelectedCards[0].GetComponent<CardItem>().ID));
    }

    /// <summary>
    /// Get count of CardItem objects in the scene
    /// </summary>
    /// <returns></returns>
    public static int GetCardCount()
    {
        return GameObject.FindObjectsOfType<CardItem>().Count();
    }

    public static CardItem[] GetCardItems()
    {
        return GameObject.FindObjectsOfType<CardItem>();
    }

    #endregion

    #region Interaction Methods
    /// <summary>
    /// Add GameObject to the List of SelectedCards
    /// </summary>
    /// <param name="card">GameObject of the CardItem</param>
    public void AddSelectedCard(GameObject card)
    {
        if (SelectedCards.Count < MAX_SELECTION) { SelectedCards.Add(card); }
    }

    /// <summary>
    /// Remove GameObject from the SelectedCards List
    /// </summary>
    /// <param name="card"></param>
    public void RemoveSelectedCard(GameObject card)
    {
        SelectedCards.Remove(card);
    }

    /// <summary>
    /// Removes all the GameObjects from the SelectedCards List
    /// </summary>
    public void RemoveSelectedCardAll()
    {
        // insert a quick pause before resetting
        StartCoroutine(RemoveSelectedCardAllCoroutine(.5f));
    }

    IEnumerator RemoveSelectedCardAllCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        foreach (var card in SelectedCards)
        {
            card.GetComponent<CardItem>().Unselect();
        }

        SelectedCards.Clear();
    }

    #endregion
}