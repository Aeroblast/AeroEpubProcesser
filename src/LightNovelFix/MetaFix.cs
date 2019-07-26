namespace AeroEpubProcesser.LightNovelFix
{

    public class MetaFixer : EpubProcesser
    {


        public override void Process(Epub epub)
        {
            Log.log("[Start]" + ToString());
            Log.level = " ";
            TextItem opf = epub.OPF;
            XFragment f = XFragment.FindFragment("metadata", opf.data);
            foreach (var e in f.root.childs)
            {
                if (e.tag.tagname == "dc:creator")
                {
                    string a = e.tag.GetAttribute("opf:file-as");
                    bool r = e.tag.RemoveAttribute("opf:file-as");
                    Log.log("[Info ]Removed meta info opf:file-as=" + a);
                }
            }
            f.Apply(ref opf.data);
            Log.level = "";
            Log.log("[End]" + ToString());
            Log.log("");
        }
         public override string ToString(){return "LightNovelFix.MetaFix";}
    }
}