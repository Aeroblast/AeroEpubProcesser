using System;
using System.Collections.Generic;

namespace AeroEpubProcesser.LightNovelFix
{
    public class TextIndentFixer:EpubProcesser
    {
        Epub epub;
        List<TextItem> css = new List<TextItem>();
        public override void Process(Epub epub)
        {
            this.epub=epub;
            foreach (Item item in epub.items)
            {
                if (item.fullName.EndsWith(".xhtml", StringComparison.OrdinalIgnoreCase))
                {
                    ProcXHTML((TextItem)item);
                }
            }
            ProcCSS();
        }
        void ProcXHTML(TextItem item)
        {
            XTag tag = XTag.FindTag("link", item.data);
            if (tag != null)
            {
                if (tag.GetAttribute("type").ToLower() == "text/css")
                {
                    string url = tag.GetAttribute("href");
                    url = Util.ReferPath(item.fullName, url);
                    bool already = false;
                    foreach (var c in css) { if (c.fullName == url) { already = true; break; } }
                    if (!already)
                    {
                        TextItem i = epub.GetItem<TextItem>(url);
                        if (i != null)
                        {
                            css.Add(i);
                        }
                        else { Log.log("Warn:Cannot find CSS:" + url); }
                    }
                }
                else Log.log("Warn:Cannot find CSS reference.");
            }
            else Log.log("Warn:Cannot find CSS reference.");

            int pos = 0;
            tag = XTag.FindTag("p", item.data, ref pos);
            while (tag != null)
            {
                switch (item.data[pos +tag.originalText.Length])
                {
                    case '「':
                    case '（':
                    case '『':
                        tag.AddClassName("ae_draw_out");
                        item.data = item.data.Remove(pos, tag.originalText.Length);
                        item.data = item.data.Insert(pos, tag.ToString());
                        break;
                }
                pos++;
                tag = XTag.FindTag("p", item.data, ref pos);
            }
        }

        void ProcCSS()
        {
            foreach (TextItem item in css)
            {
                CSSUtil.EditInSegment(ref item.data,".ae_draw_out{text-indent:1.5em;}",ToString());
            }
        }
    }

}