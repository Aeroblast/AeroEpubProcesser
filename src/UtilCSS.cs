using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AeroEpubProcesser
{
    public class CSSUtil
    {
        public static void EditInSegment(ref string css, string newContent, string segmentName)
        {

            Regex r = new Regex("\n/\\*SEG:" + segmentName + "\\*/[\\w\\W]*?/\\*ENDSEG\\*/");
            Match m = r.Match(css);
            if (m.Success)
                css=css.Remove(m.Index, m.Length);
            css += string.Format("\n/*SEG:{0}*/\n{1}\n/*ENDSEG*/", segmentName, newContent);

        }
    }
}