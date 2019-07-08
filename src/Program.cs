using System;
using System.Collections.Generic;

namespace AeroEpubProcesser
{
    class Program
    {
        static void Main(string[] args)
        {
            Epub e = new Epub("test.epub");
            List<EpubProcesser> proc = new List<EpubProcesser>();
            proc.Add(new LightNovelFix.TextIndentFixer());
            proc.Add(new LightNovelFix.LineHeightFixer());
            proc.Add(new LightNovelFix.MetaFixer());
           // proc.Add(new FootnoteAdapt.FootnoteAdapter(FootnoteAdapt.FootnoteAdaptOption.Main_Duokan));
            proc.Add(new FootnoteAdapt.FootnoteAdapter(FootnoteAdapt.FootnoteAdaptOption.Main));
            proc.Add(new AddInfo("IndentFix+LineHeightFix+MetaFix+FootnoteAdapt"));
            proc.ForEach((p) => p.Process(e));
            e.Save("testout.epub");
        }
    }
}
