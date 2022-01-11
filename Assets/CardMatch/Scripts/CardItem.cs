using UnityEngine;
using System.Collections;
using System.Linq;

public class CardItem : MonoBehaviour
{
    #region Properties
    public string ID;
    public Entry entry;
    public Vector3? startPosition;
    public CardLayoutManager cardLayoutManager;
    //public CardManager cardManager;
    public bool isSelected;
    public bool isReady;
    public bool isInit;
    //public int count;
    #endregion


    #region MonoBehaviour Methods
    void Awake()
    {
        isReady = true;
    }
    void Start()
    {
        isSelected = false;
        isReady = true;
        isInit = false;
    }

    void Update()
    {
        if (isInit) { return; }

        if (CardManager.GetCardCount() == CardMatch.TOTAL_CARDS && CardManager.GetCardItems().All(n => n.isReady))
        {
            MoveToPosition();
            isInit = true;
        }
    }

    #endregion

    #region Interaction Methods
    /// <summary>
    /// Get an available card position form CardLayoutManager and move card to that position.
    /// </summary>
    public void MoveToPosition()
    {
        var position = cardLayoutManager.GetNextCardPosition();

        if (position != null)
        {
            var endPos = (Vector3)position;
            StartCoroutine(MoveToPosition(endPos, 1.0f));
        }
        else { Debug.Log(string.Format("Unable to set card position: {0}", entry.getId())); }
    }

    private IEnumerator MoveToPosition(Vector3 endPos, float time)
    {
        float elapsedTime = 0;
        Vector3 startPos = transform.position;

        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = endPos;
        yield return null;
    }

    /// <summary>
    /// Marks GameObject selected and scales up to indicate that it has been selected
    /// </summary>
    public void Select()
    {
        isSelected = true;
        gameObject.transform.localScale = new Vector3(.125f, .125f, .125f);
    }

    /// <summary>
    /// Marks the GameObject not selected and sets scale to original size
    /// </summary>
    public void Unselect()
    {
        isSelected = false;
        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    /// <summary>
    /// Fades out the GameObject and then set to inactive
    /// </summary>
    public void Fadeout()
    {
        if (!isSelected) { return; }

        StartCoroutine(Fade(0.0f));
    }

    /// <summary>
    /// Fade out the card and then disable the gameobject
    /// </summary>
    /// <returns></returns>
    IEnumerator Fade(float seconds)
    {
        // TODO: implement the fading of the object instead of setting to inactive
        //yield return new WaitForSeconds(seconds);

        gameObject.SetActive(false);

        yield return null;
    }

    #endregion
}