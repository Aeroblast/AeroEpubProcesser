using System;
using System.Collections.Generic;

namespace AeroEpubProcesser.LightNovelFix
{
    public class LineHeightFixer:EpubProcesser
    {
        Epub epub;
        public override void Process(Epub epub)
        {
            this.epub=epub;
            foreach (Item item in epub.items)
            {
                if (item.fullName.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                {
                    ProcCSS((TextItem)item);
                }
            }
        }
        void ProcCSS(TextItem item)
        {
            CSSUtil.EditInSegment(ref item.data,"p{line-height:1.5;margin-top:0;margin-bottom:0;}",ToString());
        }

    }

}