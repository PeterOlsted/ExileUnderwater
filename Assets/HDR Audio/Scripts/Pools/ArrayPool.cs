using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class ArrayPool<T> where T : class {

	// Use this for initialization
	private List<List<T[]>> freeObjects;

    private int allocateSize = 5;

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

    private int arrayCount = 5;

    public int ArrayCount
    {
        get { return arrayCount; }
        set
        {
            if (value > 0)
            {
                arrayCount = value;
            }
        }
    }

    public ArrayPool(int numOfArrays, int arrayCount)
    {
        freeObjects = new List<List<T[]>>();
        this.arrayCount = arrayCount;
        allocateSize = numOfArrays;
        ReserveExtra(arrayCount);
    }

    public void Release(T[] obj)
    {
        freeObjects[obj.Length].Add(obj);
    }

    private void ReserveExtra(int arraySize)
    {
        for (int i = freeObjects.Count; i < arraySize; ++i)
        {
            var newList = new List<T[]>();
            freeObjects.Add(newList); //Create a new list of arrays for this index
            ReserveExtraAt(i);
        }
    }

    private void ReserveExtraAt(int index)
    {
        freeObjects.Add(new List<T[]>());
        for (int i = 0; i < arrayCount; ++i)
        {
            var newArray = new T[index];
            freeObjects[index].Add(newArray);
        }


    }

    public T[] GetArray(int arraySize)
    {
        //arraySize -= 1;
        if (freeObjects.Count < arraySize)
        {
            ReserveExtra(arraySize);
        }
        
        var arrayList = freeObjects[arraySize];

        if (arrayList.Count == 0)
        {
            ReserveExtraAt(arraySize);
        }

        T[] obj = arrayList[arrayList.Count - 1];
        arrayList.RemoveAt(arrayList.Count - 1);
        
        return obj;
    }

}
