using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace AeroEpubProcesser
{
    class Program
    {
        static bool warnBeforeProc = false;
        static string procRecord = "";
        static List<EpubProcesser> proc = new List<EpubProcesser>();
        static string target = null;
        static string output = null;
        static string mode = null;
        static void Main(string[] args)
        {
            int i = 0;
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: <epub file> process[para] ... ");
                return;
            }
            if (args[0].ToLower() == "-d")
            {
                if (!Directory.Exists(args[1]))
                {
                    Log.log("[Error]Invaild input.");
                    return;
                }
                target = args[1];
                mode = args[0];
                i = 2;
            }
            else
            {
                if (!File.Exists(args[0]) || !args[0].EndsWith(".epub", StringComparison.OrdinalIgnoreCase))
                {
                    Log.log("[Error]Invaild input.");
                    return;
                }
                else
                {
                    target = args[0];
                    mode = "";
                    i = 1;
                }
            }

            Regex procRegex = new Regex("([a-zA-Z\\.]{1,50})(\\[(.*?)\\])*");
            for (; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-o":

                        i++;
                        if (i >= args.Length) { Log.log("[Error]Invaild input."); return; }
                        output = args[i];

                        break;
                    default:
                        {
                            Match m = procRegex.Match(args[i]);
                            if (!m.Success) { Log.log("[Warn ]Unrecognized command:" + args[i]); warnBeforeProc = true; continue; }
                            EpubProcesser p = MappingProcess(m.Groups[1].Value, m.Groups[3].Value.Split(','));
                            if (p != null) { proc.Add(p); }
                        }
                        break;
                }
            }
            if (output != null && mode.ToLower() == "-d")
                if (output.EndsWith(".epub", StringComparison.OrdinalIgnoreCase))
                {
                    warnBeforeProc = true;
                    Log.log("[Warn ]!!!A full output path for massive process:" + output);
                    output = Path.GetDirectoryName(output);
                    Log.log("[Warn ]Output directory will be:" + output);

                }
            if (warnBeforeProc)
            {
                Console.WriteLine("Detected warning. Contine? N(Default)/Y");
                string ys = Console.ReadLine();
                if (ys.ToLower() != "y")
                {
                    Log.log("[Warn ]Process cancelled.");
                    return;
                }
            }
            proc.Add(new AddInfo(procRecord));


            switch (mode)
            {
                case "":
                    SingleProc(target);
                    break;
                case "-d":
                    DirectoryProc(target);
                    break;
                case "-D":
                    DirectoryProcWithChildren(target);
                    break;
            }
        }
        static void SingleProc(string path)
        {
            try
            {
                Log.log(path);
                Epub epub = new Epub(path);
                proc.ForEach((p) => p.Process(epub));
                epub.filename += "[AEP]";
                if (output == null)
                {
                    output = Path.GetDirectoryName(path);
                }
                epub.Save(output);
            }
            catch (Exception e)
            {
               Log.log("[Error]" + e);
              // throw;
            }

        }
        static void DirectoryProc(string d)
        {
            foreach (var a in Directory.GetFiles(d))
                if (a.EndsWith(".epub", StringComparison.OrdinalIgnoreCase))
                {
                    SingleProc(a);
                }
        }
        static void DirectoryProcWithChildren(string d)
        {
            foreach (var a in Directory.GetFiles(d))
                if (a.EndsWith(".epub", StringComparison.OrdinalIgnoreCase))
                {
                    SingleProc(a);
                }
            foreach (var a in Directory.GetDirectories(d))
                DirectoryProcWithChildren(a);
        }
        static EpubProcesser MappingProcess(string procName, string[] options)
        {
            EpubProcesser p = null;
            switch (procName)
            {
                case "LightNovelFix.LineHeightFix": p = new LightNovelFix.LineHeightFixer(); break;
                case "LightNovelFix.MetaFix": p = new LightNovelFix.MetaFixer(); break;
                case "LightNovelFix.TextIndentFix": p = new LightNovelFix.TextIndentFixer(); break;
                case "LightNovelFix.SeparatorCentralize":p=new LightNovelFix.SeparatorCentralizer();break;
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
                                    Log.log("[Warn ]Unrecognized option:" + options[0]);
                                    break;
                            }
                        else
                            p = new FootnoteAdapt.FootnoteAdapter();
                    }
                    break;
                case "NameFormat": p = new NameFormater(); break;
                case "KindleHDImageMerge": p = new ProcEpubByKindleUnpack.HDImageMerger(); break;
            }
            if (p == null) { Log.log("[Warn ]Unreogenized command: " + procName); warnBeforeProc = true; return null; }
            Log.log("[Info ]Created " + p.ToString());
            procRecord += p.ToString() + " ";
            return p;
        }
    }
}
