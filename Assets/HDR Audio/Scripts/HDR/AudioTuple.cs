using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDRAudio
{
    public class AudioTuple
    {
        public AudioTuple(HDRAudioSource source)
        {
            Audio = source;
            //ActualLoudness = source.Decibel;
            //DynamicLoudness = ActualLoudness;
        }

        public HDRAudioSource Audio;
        public float DynamicLoudness;
        public float ActualLoudness;
    }
}
