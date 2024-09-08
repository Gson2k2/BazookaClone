using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GunTrajectory : MonoBehaviour
{ 
    // [SerializeField] private int maxSegment = 16;
    // [SerializeField] private float timeStep = 0.05f;
    // [SerializeField] private float scaleRate = 0.5f;
    //
    // [SerializeField] private LayerMask layerMask;
    // [SerializeField] private GameObject dotPf;
    // [SerializeField] private GameObject aimHolder;
    // [SerializeField] private List<GameObject> dotList;
    //
    // int numSegments = 0;
    //
    // #region OLD_GunTrajectory
    //
    //
    //
    //
    // public void Start()
    // {
    //     for (int i = 0; i < maxSegment; i++)
    //     {
    //         GameObject dotGo = Instantiate(dotPf,aimHolder.transform);
    //         dotList.Add(dotGo);
    //     }
    //     Hide();
    // }
    //
    // public void Hide()
    // {
    //     foreach (var dot in dotList)
    //     {
    //         dot.SetActive(false);
    //     }
    // }
    //
    // public void SimulatePath(Transform shooter, Vector3 velocity, float radius)
    // {
    //
    //     Collider[] hitColliders = new Collider[1];
    //     for (numSegments = 0; numSegments < maxSegment; numSegments++)
    //     {
    //         Vector3 position = GetPos(shooter.position, velocity, Physics.gravity, timeStep * numSegments);
    //         if (Physics.OverlapSphereNonAlloc(position, radius, hitColliders, layerMask) > 0)
    //         {
    //             break;
    //         }
    //
    //         if (!dotList[numSegments].activeInHierarchy)
    //             dotList[numSegments].SetActive(true);
    //         dotList[numSegments].transform.position = position;
    //     }
    //
    //     Vector3 baseScale = dotPf.transform.localScale;
    //     Vector3 scale = 2 * baseScale * scaleRate / numSegments;
    //
    //     for (int i = 0; i < numSegments; i++)
    //     {
    //         dotList[i].transform.localScale = baseScale - Mathf.Abs(numSegments / 2 - i) * scale;
    //
    //     }
    //     for (; numSegments < maxSegment; numSegments++)
    //     {
    //         if (dotList[numSegments].activeInHierarchy)
    //             dotList[numSegments].SetActive(false);
    //     }
    // }
    //
    // private Vector3 GetPos(Vector3 origin, Vector3 v, Vector3 g, float time)
    // {
    //     return origin + v * time + g * time * time / 2;
    // }
    // #endregion

    #region NEW_GunTrajectory

    [SerializeField] private int dotNumber;
    [SerializeField] private GameObject dotParent;
    [SerializeField] private GameObject dotPrefabs;
    [SerializeField] private float dotSpacing;
    [SerializeField][Range(0.01f,0.3f)] private float dotMinScale;
    [SerializeField][Range(0.3f,1f)] private float dotMaxScale;

    private Transform[] dotsList;
    private Vector2 pos;
    private float timeStamp;

    private void Start()
    {
        OnHide();
        PrepareDots();
    }

    void PrepareDots()
    {
        dotsList = new Transform[dotNumber];
        for (int i = 0; i < dotNumber; i++)
        {
            dotsList[i] = Instantiate(dotPrefabs, dotParent.transform).transform;
        }
        
    }
    
    public void UpdateTrajectory(Vector3 gunPos,Vector3 forceApplied)
    {
        timeStamp = dotSpacing;
        for (int i = 0; i < dotNumber; i++)
        {
            pos.x = (gunPos.x + forceApplied.x * timeStamp);
            pos.y = (gunPos.y + forceApplied.y * timeStamp)  - (Physics2D.gravity.magnitude * timeStamp *timeStamp)/2f;

            dotsList[i].position = pos;
            timeStamp += dotSpacing;
        }
    }

    public void OnHide()
    {
        dotParent.SetActive(false);
    }

    public void OnShow()
    {
        dotParent.SetActive(true);
    }
    #endregion
}
