using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace AeroEpubProcesser.FootnoteAdapt
{
    public class ProcOPF
    {
        public ProcOPF(Epub epub)
        {
            TextItem opf=epub.GetOPF();
            int pos = 0;
            XTag t = XTag.FindTag("item", opf.data, ref pos);
            while (t != null)
            {
                if (Path.GetFileName(t.GetAttribute("href")) == "notereplace.js")
                {
                    opf.data = opf.data.Remove(pos, t.originalText.Length); break;
                }
                pos++;
                t = XTag.FindTag("item", opf.data, ref pos);   
            }

        }
    }
}