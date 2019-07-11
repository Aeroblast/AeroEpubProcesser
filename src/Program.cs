using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace AeroEpubProcesser
{
    class Program
    {
        static bool warnBeforeProc = false;
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: <epub file> process[para] ... ");
                return;
            }
            if (!File.Exists(args[0]) || !args[0].EndsWith(".epub", StringComparison.OrdinalIgnoreCase))
            {
                Log.log("[Error]Invaild input file.");
                return;
            }

            Epub epub = new Epub(args[0]);
            List<EpubProcesser> proc = new List<EpubProcesser>();
            Regex procRegex = new Regex("([a-zA-Z\\.]{1,50})(\\[(.*?)\\])*");

            for (int i = 1; i < args.Length; i++)
            {
                Match m = procRegex.Match(args[i]);
                if (!m.Success) { Log.log("[Warn ]Unrecognized command:" + args[i]); warnBeforeProc = true; continue; }
                EpubProcesser p = MappingProcess(m.Groups[1].Value, m.Groups[3].Value.Split(','));
                if (p != null) { proc.Add(p); }
            }
            if (warnBeforeProc)
            {
                Console.WriteLine(" Contine? N(Default)/Y");
                string ys = Console.ReadLine();
                if (ys.ToLower() != "y")
                {
                    Log.log("[Warn ]Process cancelled.");
                    return;
                }
            }
            proc.ForEach((p) => p.Process(epub));
            epub.filename+="[AEP]";
            epub.Save(Path.GetDirectoryName(args[0]));

            //proc.Add(new LightNovelFix.TextIndentFixer());
            //proc.Add(new LightNovelFix.LineHeightFixer());
            //proc.Add(new LightNovelFix.MetaFixer());
            // proc.Add(new FootnoteAdapt.FootnoteAdapter(FootnoteAdapt.FootnoteAdaptOption.Main_Duokan));
            //proc.Add(new FootnoteAdapt.FootnoteAdapter(FootnoteAdapt.FootnoteAdaptOption.Main));
            //proc.Add(new AddInfo("IndentFix+LineHeightFix+MetaFix+FootnoteAdapt"));
            //proc.ForEach((p) => p.Process(e));
            //e.Save("testout.epub");
        }
        static EpubProcesser MappingProcess(string procName, string[] options)
        {
            EpubProcesser p = null;
            switch (procName)
            {
                case "LightNovelFix.LineHeightFix": p = new LightNovelFix.LineHeightFixer(); break;
                case "LightNovelFix.MetaFix": p = new LightNovelFix.MetaFixer(); break;
                case "LightNovelFix.TextIndentFix": p = new LightNovelFix.TextIndentFixer(); break;
                case "FootnoteAdapt":
                    {
                        if (options.Length > 0)
                            switch (options[0].ToLower())
                            {
                                case "main":
                                    p = new FootnoteAdapt.FootnoteAdapter(FootnoteAdapt.FootnoteAdaptOption.Main);
                                    break;
                                case "main_duokan":
                                    p = new FootnoteAdapt.FootnoteAdapter(FootnoteAdapt.FootnoteAdaptOption.Main_Duokan);
                                    break;
                                    default:
                                    p = new FootnoteAdapt.FootnoteAdapter();
                                    Log.log("[Warn ]Unrecognized option:"+options[0]);
                                    break;
                            }
                        else
                            p = new FootnoteAdapt.FootnoteAdapter();
                    }
                    break;
                case "NameFormat":p = new NameFormater(); break;
            }
            if(p==null){ Log.log("[Warn ]Unreogenized command: "+procName);warnBeforeProc=true;return  null;}
            Log.log("[Info ]Created "+p.ToString());
            return p;
        }
    }
}
