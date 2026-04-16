using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Helpers
{
    public class QuickFuncs
    {
        public static void RemoveExtraMasterFields(Hashtable hashtable)
        {
            var suffixes = new[] { "__Name", "__Code", "__Alias" };
            var keysToRemove = hashtable.Keys.Cast<string>().Where(k => suffixes.Any(s => k.EndsWith(s))).ToList();

            foreach (var key in keysToRemove)
            {
                hashtable.Remove(key);
            }

        }
    }
}