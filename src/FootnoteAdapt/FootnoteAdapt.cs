
using System.Collections.Generic;
using System.IO;
namespace AeroEpubProcesser.FootnoteAdapt
{
    public enum FootnoteAdaptOption
    {
        Main,Main_Duokan
        
    }

    public class FootnoteAdapter:EpubProcesser
    {
        FootnoteAdaptOption option;

        public FootnoteAdapter(FootnoteAdaptOption option=FootnoteAdaptOption.Main_Duokan)
        {
            this.option=option;
        }

        public override void Process(Epub epub)
        {
            List<TextItem> css = new List<TextItem>();
            Item jstobedeleted = null;
            foreach (Item item in epub.items)
            {
                if (Path.GetFileName(item.fullName) == "notereplace.js")
                {
                    jstobedeleted = item;
                    continue;
                }
                if (Path.GetExtension(item.fullName).ToLower() == ".xhtml")
                {
                    var x = new ProcXHTML((TextItem)item,option);
                    if (x.contain_footnote)
                    {
                        if (x.css.Count > 0)
                        {
                            bool exi = false;
                            foreach (var a in css) if (a.fullName == x.css[0]) exi = true;
                            if (!exi)
                            {
                                TextItem i = epub.GetItem<TextItem>(x.css[0]);
                                if (i != null) css.Add(i);
                            }
                        }
                    }
                }
            }
            epub.items.Remove(jstobedeleted);
            Log.log("Removed notereplace.js");
            foreach (TextItem p in css)
            {
                new ProcCSS(p,option);
            }
            new ProcOPF(epub);
            epub.DeleteEmpty();
        }
    }

    

}