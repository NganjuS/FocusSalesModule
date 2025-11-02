using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Helpers
{
    public class UtilFuncs
    {
        public static int GetDateToInt(DateTime dt)
        {

            return (dt.Year * 65536 | dt.Month * 256 | dt.Day);
        }
        public static DateTime GetDateFromInt(int dt)
        {

            string dateprtyr = ((dt & 0xfff0000) / 65536).ToString().Length <= 4 ? ((dt & 0xfff0000) / 65536).ToString() : ((dt & 0xfff0000) / 65536).ToString().Substring(0, 4);
            string dateprtmnh = ((dt & 0xff00) / 256).ToString().Length < 2 ? "0" + ((dt & 0xff00) / 256).ToString() : ((dt & 0xff00) / 256).ToString().Substring(0, 2);
            string dateprtday = (dt & 0xff).ToString().Length < 2 ? "0" + (dt & 0xff).ToString() : (dt & 0xff).ToString().Substring(0, 2);
            return Convert.ToDateTime($"{dateprtyr}/{dateprtmnh}/{dateprtday}");
        }
        public static long GetFocusTime(string strCTime)
        {
            string strHour = string.Empty;
            string strMin = string.Empty;
            string strSec = string.Empty;
            string noon = string.Empty;
            long lResult = 0;
            string strTime = string.Empty;
            strHour = strCTime.Substring(0, strCTime.IndexOf(":"));
            strCTime = strCTime.Substring(strCTime.IndexOf(":") + 1);
            strHour = (int.Parse(strHour) * 256 * 256).ToString();
            strMin = strCTime.Substring(0, strCTime.IndexOf(":"));
            strTime = strCTime.Substring(strCTime.IndexOf(":") + 1);
            strMin = (int.Parse(strMin) * 256).ToString();
            noon = strCTime.Substring(strCTime.IndexOf(":", 1) + 1);
            strSec = noon.Substring(0, noon.IndexOf(" "));
            lResult = int.Parse(strHour) + int.Parse(strMin) + int.Parse(strSec);
            return lResult;
        }

        public static long getLongFocusDate(DateTime dtDate)
        {
            long lngTemp = 0;
            lngTemp = ((int.Parse(dtDate.Year.ToString()) - 1950) * 416) + (32 * int.Parse(dtDate.Month.ToString())) + int.Parse(dtDate.Day.ToString());
            return lngTemp;
        }

        public static DateTime getFocusIntToDate(int iDate)
        {
            //str((@dt%416)%32,2)+'-'+str((@dt%416)/32,2)+'-'+str(@dt/416+1950,4)set @dtstr=replace( @dtstr,' ','0') 
            CultureInfo provider = CultureInfo.InvariantCulture;
            string format = "d";
            DateTime dtRet;
            string sDay = string.Empty;
            string sMonth = string.Empty;
            string sYear = string.Empty;
            if (((iDate % 416) % 32).ToString().Length > 1)
                sDay = ((iDate % 416) % 32).ToString().Substring(0, 2);
            else
                sDay = "0" + ((iDate % 416) % 32).ToString().Substring(0, 1);

            if (((iDate % 416) / 32).ToString().Length > 1)
                sMonth = ((iDate % 416) / 32).ToString().Substring(0, 2);
            else
                sMonth = "0" + ((iDate % 416) / 32).ToString().Substring(0, 1);

            sYear = (iDate / 416 + 1950).ToString().Substring(0, 4);
            string dateString = sMonth + "/" + sDay + "/" + sYear;
            //dtRet = DateTime.Parse(string.Format("dd/MM/yyyy", (sDay + '/' + sMonth + '/' + (iDate / 416 + 1950).ToString().Substring(0, 4)).ToString()));
            dtRet = DateTime.ParseExact(dateString, format, provider);
            return dtRet;
        }
        public static bool IsNumeric(char o)
        {
            double result;
            //return o != null && Double.TryParse(o.ToString(), out result);
            return Double.TryParse(o.ToString(), out result);
        }
        private static int getCompCodeVal(char cCode)
        {
            int iRet = 0;
            char[] sLetters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            for (int i = 0; i < sLetters.Length; i++)
            {
                if (sLetters[i] == cCode)
                {
                    iRet = i;
                    break;
                }
            }
            return iRet + 10;
        }

        public static int GetCompID(string CompCode)
        {
            int iRet = 0;
            string sCompCode = CompCode;
            if (IsNumeric(sCompCode[0]))
            {
                iRet = (36 * 36) * int.Parse(sCompCode[0].ToString());
            }
            else
            {
                iRet = (36 * 36) * getCompCodeVal(sCompCode[0]);
            }
            if (IsNumeric(sCompCode[1]))
            {
                iRet += (36) * int.Parse(sCompCode[1].ToString());
            }
            else
            {
                iRet += (36) * getCompCodeVal(sCompCode[1]);
            }

            if (IsNumeric(sCompCode[2]))
            {
                iRet += (36 * 0) * int.Parse(sCompCode[2].ToString());
            }
            else
            {
                iRet += (36 * 0) * getCompCodeVal(sCompCode[2]);
            }
            return iRet;

        }

        public static string getCompanyCode(int value)
        {
            char[] base36Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            String returnValue = "";
            while (value != 0)
            {
                returnValue = base36Chars[value % 36] + returnValue;
                value /= 36;
            }
            return (returnValue.Trim().Length == 2 ? "0" + returnValue : returnValue);
        }
    }
}