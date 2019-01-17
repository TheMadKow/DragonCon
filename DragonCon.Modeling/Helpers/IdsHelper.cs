using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.Modeling.Helpers
{
    public static class IdsHelper
    {
        public static string GetNumberFromId(this string ravenIdent)
        {
            try
            {
                return ravenIdent.Split('/')[1];
            }
            catch
            {
                return ravenIdent;
            }
        }

    }
}
