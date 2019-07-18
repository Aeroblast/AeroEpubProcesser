using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace AeroEpubProcesser.ProcEpubByKindleUnpack
{
    public class HDImageMerger : EpubProcesser
    {
        Epub epub;
        string hdimgDir;
        public override void Process(Epub epub)
        {
            Log.log("[Start]" + ToString());
            Log.level = " ";
            this.epub = epub;
            hdimgDir = Path.Combine(Path.GetDirectoryName(epub.path), "HDimages");
            if (!Directory.Exists(hdimgDir)) { Log.log("[Error]Cannot find " + hdimgDir); return; }
            P();
            Log.level = "";
            Log.log("[End]" + ToString());
            Log.log("");
        }
        public override string ToString()
        {
            return "KindleHDImageMerger";
        }
        void P()
        {
            Regex hdnum = new Regex("HDimage([0-9]{5})");
            List<NormalItem> imgs = new List<NormalItem>();
            NormalItem cover = null;
            int offset = 0;
            foreach (var i in epub.items)
            {
                if (Path.GetDirectoryName(i.fullName).EndsWith("Images"))
                {
                    if (Path.GetFileName(i.fullName).Contains("cover"))
                    {
                        cover = (NormalItem)i;
                        Match m = Regex.Match(i.fullName, "cover([0-9]{5})");
                        offset = int.Parse(m.Groups[1].Value);
                    }
                    else imgs.Add((NormalItem)i);
                }
            }
            if (cover == null) { Log.log("[Error]Cannot find cover in epub."); return; }
            string[] hdimgs = Directory.GetFiles(hdimgDir);
            List<string> hdimgs_ = new List<string>(hdimgs);
            hdimgs_.Sort();
            while (hdimgs_.Count > 0 && !hdnum.Match(Path.GetFileName(hdimgs_[hdimgs_.Count - 1])).Success) { hdimgs_.RemoveAt(hdimgs_.Count - 1); }
            if (hdimgs_.Count == 0) { Log.log("[Error]Cannot find hdimg."); return; }
            string hdcover = hdimgs_[hdimgs_.Count - 1];

            hdimgs_.RemoveAt(hdimgs_.Count - 1);
            {
                Match m = hdnum.Match(hdcover);
                offset = offset - int.Parse(m.Groups[1].Value);
            }
            Replace(cover, hdcover);
            foreach (string hd in hdimgs_)
            {
                Match m = hdnum.Match(Path.GetFileName(hd));
                if (!m.Success) continue;
                string name = "image" + Util.Number(int.Parse(m.Groups[1].Value) + offset, 5);
                bool replaced=false;
                foreach (NormalItem img in imgs)
                {
                    if (img.fullName.Contains(name))
                    {
                        Replace(img, hd); replaced=true;break;
                    }
                }
                if(!replaced){Log.log("[Error ]Cannot replace "+hd);return;}

            }
        }
        void Replace(NormalItem i, string path)
        {
            byte[] dat = File.ReadAllBytes(path);
            //   /*debug */{ File.WriteAllBytes(Path.Combine("debug", Path.GetFileName(i.fullName)), i.data); File.WriteAllBytes(Path.Combine("debug2", Path.GetFileName(i.fullName)), dat); }

            i.data = dat;
            Log.log(string.Format("[Info ]Replace {0} by {1}", i.fullName, Path.GetFileName(path)));

        }
        void RenameHDImageToKindleUnpackEpub(string[] imgs, string hdimgDir)
        {
            int offset = 0;
            string covername = "";
            for (int i = 0; i < imgs.Length; i++)
            {
                imgs[i] = Path.GetFileName(imgs[i]);
                if (imgs[i].Contains("cover"))
                {
                    Match m = Regex.Match(imgs[i], "cover([0-9]{5})");
                    offset = int.Parse(m.Groups[1].Value);
                    covername = imgs[i];
                    break;
                }
            }
            string[] hdimgs = Directory.GetFiles(hdimgDir);
            List<string> hdimgs_ = new List<string>(hdimgs);
            hdimgs_.Sort();
            string hdcover = hdimgs_[hdimgs_.Count - 1];
            Regex hdnum = new Regex("HDimage([0-9]{5})");
            hdimgs_.RemoveAt(hdimgs_.Count - 1);
            {
                Match m = hdnum.Match(hdcover);
                offset = offset - int.Parse(m.Groups[1].Value);
            }
            File.Copy(hdcover, Path.Combine(hdimgDir, "rename", covername));

            foreach (string n in hdimgs_)
            {
                Match m = hdnum.Match(Path.GetFileName(n));
                if (!m.Success) continue;
                string name = "image" + Util.Number(int.Parse(m.Groups[1].Value) + offset, 5) + Path.GetExtension(n);

            }
        }


    }

}