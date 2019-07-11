
using System.Text.RegularExpressions;
namespace AeroEpubProcesser.FootnoteAdapt
{
    public class ProcCSS
    {

        public ProcCSS(TextItem css, FootnoteAdaptOption option)
        {
            switch (option)
            {
                case FootnoteAdaptOption.Main:
                    {
                        string c = "@media amzn-kf8{\naside{display:none;}\n.ae_note_inside{page-break-after:always;}\n}\n.ae_note_inside{text-indent:0;}";
                        CSSUtil.EditInSegment(ref css.data, c, "AeroEpubProcesser.FootnoteAdapter");
                    }
                    break;
                case FootnoteAdaptOption.Main_Duokan:
                    {
                        string c = "@media amzn-kf8{\naside{display:none;}\n.duokan-footnote-item{page-break-after:always;}\n}";
                        CSSUtil.EditInSegment(ref css.data, c, "AeroEpubProcesser.FootnoteAdapter");
                    }
                    break;
            }
            Log.log("[Info ]Added style to "+css.fullName);

        }

    }

}