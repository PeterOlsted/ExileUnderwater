using System;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPool<T> where T : class, new()
{
	private List<T> freeObjects;

    private int freeObjectCount;

    private int allocateSize = 20;

    public int AllocateSize
    {
        get { return allocateSize; }
        set
        {
            if (value > 0)
            {
                allocateSize = value;
            }
        }
    }

    public ObjectPool(int initialSize)
    {
        freeObjects = new List<T>();
        ReserveExtra(initialSize);
    }

    public void ReleaseObject(T obj)
    {
        freeObjects.Add(obj);
        ++freeObjectCount;
    }

    public void ReserveExtra(int extra)
    {
        freeObjectCount += extra;
        for (int i = 0; i < extra; ++i)
        {
            freeObjects.Add(new T());
        }
    }

    public T GetObject()
    {
        if (freeObjectCount > 0)
        {
            ReserveExtra(allocateSize);
        }
        T go = freeObjects[freeObjects.Count - 1];
        freeObjects.RemoveAt(freeObjects.Count - 1);
        --freeObjectCount;
        return go;
    }
}
