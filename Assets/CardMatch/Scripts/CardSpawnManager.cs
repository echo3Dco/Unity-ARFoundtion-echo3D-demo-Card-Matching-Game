using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CardSpawnManager : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    [SerializeField]
    GameObject spawnablePrefab;
    Camera arCam;
    GameObject spawnedObject;
    [SerializeField]
    TMPro.TextMeshPro textMesh;
    bool initiated;

    // Start is called before the first frame update
    void Start()
    {
        spawnedObject = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        initiated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0 || initiated) { return; }

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        spawnedObject = hit.collider.gameObject;
                    }
                    else
                    {
                        // If hit object that isn't "Spawnable" then spawn the prefab
                        SpawnPrefab(m_Hits[0].pose.position);
                        initiated = true;
                    }
                }
            }
        }
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        spawnedObject = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);

        textMesh.text = "Please wait while cards load...";
    }
}
