using UnityEngine;
using System.Collections;

namespace HDRAudio
{
    public static class GUIDCreator
    {
        public static int Create()
        {
            int guid = System.DateTime.Now.Millisecond + Random.Range(-100000,100000);

            guid = ((guid >> 16) ^ guid) * 0x45d9f3b;
            guid = ((guid >> 16) ^ guid) * 0x45d9f3b;
            guid = ((guid >> 16) ^ guid);

            return guid;
        }
    }
}