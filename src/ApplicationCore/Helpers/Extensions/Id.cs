using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Helpers
{
    public class TickId
    {
        public static string Create() => ConvertToBase(DateTime.Now.Ticks);

        static String ConvertToBase(long num)
        {
            int nbase = 36;
            String chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            long r;
            String newNumber = "";

            // in r we have the offset of the char that was converted to the new base
            while (num >= nbase)
            {
                r = num % nbase;
                newNumber = chars[(int)r] + newNumber;
                num = num / nbase;
            }
            // the last number to convert
            newNumber = chars[(int)num] + newNumber;

            return newNumber.ToLower();
        }
    }
}
