using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace HDRAudio
{

    public class AudioGOPool : MonoBehaviour
    {
        private List<GameObject> freeObjects = new List<GameObject>();

        private Vector3 offscreen = new Vector3(-10000, -10000, -10000);

        private int freeObjectCount;

        [Range(0, 128)]
        public int InitialSize = 10;
        
        public int ChunkSize = 20;

        /*public int ChunkSize
        {
            get { return allocateSize; }
            set
            {
                if (value > 0)
                {
                    allocateSize = value;
                } 
            }
        }*/

        public GameObject RuntimeAudioPrefab;
        

        void Awake()
        {
            ReserveExtra(InitialSize);
        }


        public void ReleaseObject(GameObject go)
        {
            go.transform.parent = transform;
            go.transform.localPosition = offscreen;
            freeObjects.Add(go);
            go.SetActive(false);
            ++freeObjectCount;
        }

        public void ReserveExtra(int extra)
        {
            freeObjectCount += extra;
            for (int i = 0; i < extra; ++i)
            {
                var go = Object.Instantiate(RuntimeAudioPrefab, offscreen, Quaternion.identity) as GameObject;
                go.transform.parent = transform;
                freeObjects.Add(go);
                freeObjects[freeObjects.Count - 1].SetActive(false);
                var runtimeAudio = freeObjects[freeObjects.Count - 1].GetComponent<RuntimePlayer>();
                runtimeAudio.Initialize(this);
            }
        }

        public GameObject GetObject()
        {
            GameObject go;
            if (freeObjectCount > 0)
            {
                go = freeObjects[freeObjects.Count - 1];
                freeObjects.RemoveAt(freeObjects.Count - 1);
            }
            else
            {
                ReserveExtra(ChunkSize);
                go = freeObjects[freeObjects.Count - 1];
                freeObjects.RemoveAt(freeObjects.Count - 1);
            }
            --freeObjectCount;
            go.SetActive(true);
            return go;
        }
    }
}
