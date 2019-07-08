using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace AeroEpubProcesser.FootnoteAdapt
{
    public class ProcXHTML
    {
        public const string noteTemplate_Main_Duokan = "<aside epub:type=\"footnote\" id=\"{0}\"><a href=\"#{1}\"></a><ol class=\"duokan-footnote-content\" style=\"list-style:none;margin-left:-1em;\"><li class=\"duokan-footnote-item\" id=\"{0}\">{2}</li></ol></aside>";
        public const string noteTemplate_Main = "<aside epub:type=\"footnote\" id=\"{0}\"><a href=\"#{1}\"></a><p class=\"ae_note_inside\" >{2}</p></aside>";

        string template = noteTemplate_Main_Duokan;
        TextItem xhtml;
        string text { get { return xhtml.data; } set { xhtml.data = value; } }
        public bool contain_footnote = false;
        public List<string> css = new List<string>();
        public ProcXHTML(TextItem item, FootnoteAdaptOption option)
        {
            switch (option)
            {
                case FootnoteAdaptOption.Main: template = noteTemplate_Main; break;
                case FootnoteAdaptOption.Main_Duokan: template = noteTemplate_Main_Duokan; break;
            }
            xhtml = item;
            CheckFootnotes();
            if (contain_footnote) CheckNamespace();
            CheckHead();
        }
        void CheckHead()
        {
            Regex reg = new Regex("<head>[\\w\\W]*?</head>");
            Match m = reg.Match(text);
            if (!m.Success) { Log.log("Warn:No head tag in xhtml"); return; }
            Regex reg_link = new Regex("<link .*?>");
            Regex reg_script = new Regex("<script .*?>");
            var ms = reg_link.Matches(m.Value);
            foreach (Match link in ms)
            {
                XTag tag = new XTag(link.Value);
                if (tag.GetAttribute("type").ToLower() == "text/css")
                {
                    string url = tag.GetAttribute("href");
                    url = Util.ReferPath(xhtml.fullName, url);
                    css.Add(url);
                }
            }
            int pos = m.Index;
            Match scpt = reg_script.Match(text, pos);
            while (scpt.Success)
            {
                XTag tag = new XTag(scpt.Value);
                if (tag.GetAttribute("src").Contains("notereplace.js"))
                {
                    string scpt_end = "</script>";
                    int sei = text.IndexOf(scpt_end, scpt.Index);
                    if (sei < 0) { Log.log("Error:Unclosed script tag."); break; }
                    text = text.Remove(scpt.Index, sei - scpt.Index + scpt_end.Length);
                    Log.log("Removed reference to notereplace.js");
                    break;
                }
                else
                {
                    pos = scpt.Index + scpt.Length;
                }
                scpt = reg_script.Match(text, pos);
            }


        }
        void CheckNamespace()
        {
            Regex html_tag = new Regex("<html.*?>");
            MatchCollection ms = html_tag.Matches(text);
            if (ms.Count != 1)
            {
                Log.log("Warn:None or multiple html tag found.");
            }
            else
            {
                string s = ms[0].Groups[0].Value;
                if (!s.Contains("xmlns:epub"))
                {
                    text = text.Replace(s, s.Insert(s.Length - 1, " xmlns:epub=\"http://www.idpf.org/2007/ops\""));
                    Log.log("Info:Added xmlns:epub to " + xhtml.fullName);
                }
            }
        }

        void CheckFootnotes()
        {
            Regex reg_link = new Regex("<a .*?>");
            int pos = 0;
            Match link = reg_link.Match(text);
            while (link.Success)
            {
                XTag tag = new XTag(link.Value);
                if (Util.Contains(tag.GetClassNames(), "duokan-footnote")
                || tag.GetAttribute("epub:type") == "noteref")
                {
                    ProcNote(link, tag);
                }
                pos = link.Index + 1;//假定注释本体都在链接后面
                link = reg_link.Match(text, pos);
            }
        }
        void ProcNote(Match m, XTag tag)
        {
            string note_id = "", ref_id;
            //Link tag solve
            {
                var a = tag.GetClassNames();
                if (!Util.Contains(a, "duokan-footnote"))
                {
                    string added = "duokan-footnote";
                    if (a.Length != 0)
                    {
                        added = " " + added;
                    }
                    tag.SetAttribute("class", tag.GetAttribute("class") + added);
                }
            }

            if (tag.GetAttribute("epub:type") != "noteref")
            {
                tag.SetAttribute("epub:type", "noteref");
            }
            {
                string href = tag.GetAttribute("href");
                int pt = href.IndexOf('#');
                if (pt < 0)
                { Log.log("Error:Not a valid link :" + href + ""); return; }
                if (pt != 0) { Log.log("Warn: href=\":" + href + "\""); }
                note_id = href.Substring(pt + 1);
            }

            ref_id = note_id + "_ref";
            tag.SetAttribute("id", ref_id);

            text = text.Remove(m.Index, m.Length);
            text = text.Insert(m.Index, tag.ToString());


            //Note content
            ProcNoteContent(note_id, ref_id);

        }
        void ProcNoteContent(string note_id, string ref_id)
        {

            Regex reg_tag = new Regex("<.*?>");
            Regex reg_duokan = new Regex("<ol .*?>");
            Regex reg_aside = new Regex("<aside .*?>");
            int index = -1, length = 0;
            string note_content = null; string list_value = "1";

            Match m = reg_aside.Match(text);
            while (m.Success)
            {
                XTag tag = new XTag(m.Value);
                if (tag.GetAttribute("id") == note_id)
                {
                    index = m.Index;
                    XFragment frag = new XFragment(text, index);
                    if (frag.root != null)
                    {
                        var dk = frag.root.GetElementById(note_id);
                        if (dk != null)
                        {
                            //做过兼容，aside里套多看li
                            note_content = dk.innerXHTML;
                            list_value = dk.tag.GetAttribute("value");
                        }
                        else
                        {
                            note_content = frag.root.innerXHTML;
                        }
                        length = frag.originalLength;

                    }
                    else
                    {
                        Log.log("Error:Found note but failure on parsing. id=" + note_id); return;
                    }
                    break;
                }
                m = m.NextMatch();
            }

            if (index < 0)//如果只对多看适配，没有aside 
            {
                m = reg_duokan.Match(text);
                while (m.Success)
                {
                    XFragment frg = new XFragment(text, m.Index);
                    if (frg.root != null)
                    {
                        if (Util.Contains(frg.root.tag.GetClassNames(), "duokan-footnote-content"))
                        {
                            var a = frg.root.GetElementById(note_id);
                            if (a != null)
                            {
                                index = m.Index;
                                note_content = a.innerXHTML;
                                length = frg.originalLength;
                                break;
                            }
                        }

                    }
                    m = m.NextMatch();

                }
            }

            if (note_content == null) { Log.log("Error:cannot find note"); return; }
            string note_full = string.Format(template, note_id, ref_id, note_content);
            text = text.Remove(index, length);
            text = text.Insert(index, note_full);

            Log.log("Formated:" + note_id + ":" + note_content);
            contain_footnote = true;
        }


    }



}