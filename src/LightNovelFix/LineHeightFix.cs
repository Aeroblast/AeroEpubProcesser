using System;
using System.Collections.Generic;

namespace AeroEpubProcesser.LightNovelFix
{
    public class LineHeightFixer : EpubProcesser
    {
        Epub epub;
        public override void Process(Epub epub)
        {
            Log.log("[Start]" + ToString());
            Log.level = " ";
            this.epub = epub;
            foreach (Item item in epub.items)
            {
                if (item.fullName.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                {
                    ProcCSS((TextItem)item);
                }
            }
            Log.level = "";
            Log.log("[End]" + ToString());
            Log.log("");
        }
        void ProcCSS(TextItem item)
        {
            CSSUtil.EditInSegment(ref item.data, "p{line-height:1.5;margin-top:0;margin-bottom:0;}", ToString());
            Log.log("[Info ]Added style to " + item.fullName);
        }

    }

}