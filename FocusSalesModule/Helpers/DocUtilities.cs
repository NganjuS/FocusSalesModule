using Focus.Common.DataStructs;
using FocusSalesModule.Data;
using FocusSalesModule.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FocusSalesModule.Helpers
{
    public class DocUtilities
    {
        public static string GetNextDocNo(int compId)
        {
            const string Abbrev = "POS";
            String docNo = DbCtx<String>.GetScalar(compId, TxnQueries.GetPOSNextDocNo());
            if(String.IsNullOrEmpty(docNo))
            {
                return $"{Abbrev}00001";
            }
            else
            {
                return GetNextNo(docNo);

            }
        }
        static string GetNextNo(string input)
        {
            string prefix = new string(input.TakeWhile(c => !char.IsDigit(c)).ToArray());
            string numericPart = input.Substring(prefix.Length);

            // Convert and increment
            int number = int.Parse(numericPart) + 1;

            // Keep same padding length
            string newString = prefix + number.ToString(new string('0', numericPart.Length));
            return newString;
        }
        static string RemoveOtherStr(string inputstr)
        {
            string sntzedstr = Regex.Replace(inputstr.Trim(), "[^0-9]+", "");
            return sntzedstr;
        }
    }
}