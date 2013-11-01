using HDRAudio;
using HDRAudio.ExtensionMethods;
using UnityEngine;
using System.Collections.Generic;

public class AudioPlayer : MonoBehaviour
{
    private RuntimePlayer player;

    public void Play(GameObject controllingObject, AudioNode audioNode, GameObject attachedTo)
    {
        var poolObject = pool.GetObject();
        poolObject.transform.parent = attachedTo.transform;
        poolObject.transform.localPosition = new Vector3();
        if (player == null)
            player = poolObject.GetComponent<RuntimePlayer>();
        GetValue(GOAudioNodes, controllingObject).Add(new RuntimeTuple(audioNode, player));
        player.Play(audioNode);
    }

    public void PlayAtPosition(GameObject go, AudioNode audioNode, Vector3 position)
    {
        var poolObject = pool.GetObject();
        poolObject.transform.position = position;
        if (player == null)
            player = poolObject.GetComponent<RuntimePlayer>();
        GetValue(GOAudioNodes, go).Add(new RuntimeTuple(audioNode, player));
        player.Play(audioNode);
    }

    public void StopAll(GameObject controllingObject)
    {
        List<RuntimeTuple> valueTupleList;
        GOAudioNodes.TryGetValue(controllingObject, out valueTupleList);
        if (valueTupleList != null)
        {
            for (int i = 0; i < valueTupleList.Count; ++i)
            {
                RuntimePlayer player = valueTupleList[i].Player;
                player.Stop();
                valueTupleList.SwapRemoveAt(i);
            }
        }
    }

    public void Break(GameObject controllingObject, AudioNode toBreak)
    {
        List<RuntimeTuple> valueTupleList;
        GOAudioNodes.TryGetValue(controllingObject, out valueTupleList);
        if (valueTupleList != null)
        {
            for (int i = 0; i < valueTupleList.Count; ++i)
            {
                if (valueTupleList[i].Node == toBreak)
                {
                    valueTupleList[i].Player.Break();
                }
            }
        }
    }

    public void StopByNode(GameObject controllingObject, AudioNode nodeToStop)
    {
        List<RuntimeTuple> valueTupleList;
        GOAudioNodes.TryGetValue(controllingObject, out valueTupleList);
        if (valueTupleList != null)
        {
            for (int i = 0; i < valueTupleList.Count; ++i)
            {
                if (valueTupleList[i].Node == nodeToStop)
                {
                    valueTupleList.SwapRemoveAt(i);
                }
            }
        }
    }

    private List<RuntimeTuple> GetValue(Dictionary<GameObject, List<RuntimeTuple>> dictionary, GameObject go)
    {
        List<RuntimeTuple> tupleList;
        if (!dictionary.TryGetValue(go, out tupleList))
        {
            tupleList = new List<RuntimeTuple>();
            dictionary.Add(go, tupleList);
        }
        return tupleList;
    }

    private AudioGOPool pool;

    private Dictionary<GameObject, List<RuntimeTuple>> GOAudioNodes = new Dictionary<GameObject, List<RuntimeTuple>>();

    void OnEnable()
    {
        if (pool == null)
        {
            pool = GetComponent<AudioGOPool>();
        }
        if (GOAudioNodes == null)
        {
            GOAudioNodes = new Dictionary<GameObject, List<RuntimeTuple>>();
        }
    }
}
