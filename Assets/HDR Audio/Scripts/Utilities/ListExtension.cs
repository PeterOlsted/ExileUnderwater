using System;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HDRAudio.ExtensionMethods
{
    public static class ListExtension
    {
        public static int FindIndex<T>(this List<T> list, T toFind)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if(list[i].Equals(toFind))
                    return i;
            }

            return -1;
        }

        public static void RemoveLast<T>(this List<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        public static void SwapRemoveAt<T>(this List<T> list, int index)
        {
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }

        /// <summary>
        /// Finds an element in a list, removes it by swapping with the last element and decreases the size of the list
        /// </summary>
        /// <param name="list">The list to work on</param>
        /// <param name="toFind">The object to find in the list</param>
        /// <returns>Returns true if an element was removed</returns>
        public static bool FindSwapRemove<T>(this List<T> list, T toFind)
        {
            int index = FindIndex(list, toFind);
            if(index == -1)
                return false;
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return true;
        }

        public static List<T> SwapAtIndexes<T>(this List<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
            return list;
        }

        public static T TryGet<T>(this List<T> list, int index)
        {
            if (index < list.Count)
                return list[index];
            return default(T);
        }

        /*public static List<U> ConvertList<T, U>(this List<T> toConvert) where T : class where U : class
        {
            return toConvert.ConvertAll(obj => obj as U);
        }*/
    }

    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static string FormatedName(this Enum someEnum)
        {
            return someEnum.ToString().AddSpacesToSentence();
        }
    }

    public static class StringUtil
    {
        public static string AddSpacesToSentence(this string text, bool preserveAcronyms = false)
        {
            StringBuilder newText = new StringBuilder(text.Length*2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }

    public static class EventUtil
    {
        public static bool IsDragging(this Event unityEvent)
        {
            return unityEvent.type == EventType.DragUpdated || unityEvent.type == EventType.DragPerform;
        }

        public static bool ClickedWithin(this Event unityEvent, Rect area)
        {
            return unityEvent.type == EventType.MouseDown && area.Contains(unityEvent.mousePosition) ;
        }
    }
}
