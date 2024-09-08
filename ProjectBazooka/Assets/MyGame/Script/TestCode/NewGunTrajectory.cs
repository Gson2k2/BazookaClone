using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Script.TestCode
{
    public class NewGunTrajectory : MonoBehaviour
    {
        [SerializeField] int maxSegment = 16;
        [SerializeField] float timeStep = 0.05f;
        [SerializeField] float scaleRate = 0.5f;

        [SerializeField] LayerMask layerMask;
        [SerializeField] GameObject dotPf;
        [SerializeField] GameObject dotParent;
        [SerializeField] List<GameObject> dotList;

        int numSegments = 0;

        public void Start()
        {
            for (int i = 0; i < maxSegment; i++)
            {
                GameObject dotGo = Instantiate(dotPf,dotParent.transform);
                dotList.Add(dotGo);
            }
            Hide();
        }

        public void Hide()
        {
            foreach (var dot in dotList)
            {
                dot.SetActive(false);
            }
        }

        public void SimulatePath(Transform shooter, Vector3 velocity, float radius)
        {

            Collider[] hitColliders = new Collider[1];
            for (numSegments = 0; numSegments < maxSegment; numSegments++)
            {
                Vector3 position = GetPos(shooter.position, velocity, Physics.gravity, timeStep * numSegments);
                if (Physics.OverlapSphereNonAlloc(position, radius, hitColliders, layerMask) > 0)
                {
                    break;
                }

                if (!dotList[numSegments].activeInHierarchy)
                    dotList[numSegments].SetActive(true);
                dotList[numSegments].transform.position = position;
            }

            Vector3 baseScale = dotPf.transform.localScale;
            Vector3 scale = 2 * baseScale * scaleRate / numSegments;

            for (int i = 0; i < numSegments; i++)
            {
                dotList[i].transform.localScale = baseScale - Mathf.Abs(numSegments / 2 - i) * scale;

            }
            for (; numSegments < maxSegment; numSegments++)
            {
                if (dotList[numSegments].activeInHierarchy)
                    dotList[numSegments].SetActive(false);
            }
        }

        Vector3 GetPos(Vector3 origin, Vector3 v, Vector3 g, float time)
        {
            return origin + v * time + g * time * time / 2;
        }
    }
}
