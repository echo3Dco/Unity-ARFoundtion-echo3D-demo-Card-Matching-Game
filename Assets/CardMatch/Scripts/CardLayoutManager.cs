using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CardLayoutManager is used to assist laying out cards in a grid.
/// </summary>
public class CardLayoutManager
{
    #region Properties
    public List<Vector3> CardPositions;
    public int CurrentPositionIndex;
    #endregion

    #region Contructors
    public CardLayoutManager(int totalCards, float spacing)
    {
        //Debug.Log("CardLayoutManager: " + totalCards);
        CardPositions = GenerateCardPositions(totalCards, spacing);
        CurrentPositionIndex = 0;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Get the next valid Vector3 card position
    /// Returns null if the end of the List has been reached
    /// </summary>
    /// <returns></returns>
    public Vector3? GetNextCardPosition()
    {
        if (CurrentPositionIndex < CardPositions.Count)
        {
            return (Vector3?)CardPositions[CurrentPositionIndex++];
        }
        else return null;
    }

    /// <summary>
    /// Generates. a list of CardPosition. The number of columns will be equal to the floor of the squareroot of the totalCards
    /// </summary>
    /// <param name="totalCards">Total number of cards that will be displayed</param>
    /// <param name="spacing">The space between cards measured from the GameObject origin</param>
    /// <returns>Returns a List of Vector3 that represents valid spots></returns>
    public List<Vector3> GenerateCardPositions(int totalCards, float spacing)
    {
        List<Vector3> points = new List<Vector3>();
        int columns;
        int rows;
        int count;
        float startingX;
        float startingZ;

        columns = Mathf.CeilToInt(Mathf.Sqrt(totalCards));
        rows = Mathf.CeilToInt((float)totalCards / columns);

        // To center the collection of cards around the parent origin,
        // we will start the positioning in the upper left if looking 
        // from top-down front
        startingX = -(columns * spacing / 2f);
        startingZ = rows * spacing / 2f;

        count = 0;

        for (int i = rows; i > 0; i--)
        {
            for (int j = 0; j < columns; j++)
            {
                if (count < totalCards)
                {
                    points.Add(new Vector3(startingX + (j + j * spacing), 1f, startingZ - (i + i * spacing)));
                    count++;
                }
            }
        }

        // Randomize the order of cards
        ShufflePoints(points);

        return points;
    }

    /// <summary>
    /// Randomizes the order of points.
    /// </summary>
    /// <param name="points">List of Vector3 points</param>
    public void ShufflePoints(List<Vector3> points)
    {
        int count = points.Count;

        for (var i = 0; i < count - 1; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = points[i];
            points[i] = points[r];
            points[r] = tmp;
        }
    }
    #endregion
}