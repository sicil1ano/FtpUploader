using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    static class Utilities
    {
        /// <summary>
        /// This is a class with extension methods.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="value"></param>

        /* This method takes an array as input argument and the starting value you want to initialize all the elements of the array. */ 

        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        /* This method throws a NullReferenceException if the input object is null, otherwise, if the object is not null, the method returns true. */ 

        public static bool ThrowIfNull<T>(this T variable)
        {
            if (variable == null)
                throw new NullReferenceException();
            else
                return true; // this means the object is not null. */
        }
    }
}
