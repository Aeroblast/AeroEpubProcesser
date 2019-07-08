namespace AeroEpubProcesser.LightNovelFix
{

    public class MetaFixer : EpubProcesser
    {
        

        public override void Process(Epub epub)
        {
            TextItem opf = epub.GetOPF();
            XFragment f = XFragment.FindFragment("metadata", opf.data);
            foreach(var e in f.root.childs)
            {
                if(e.tag.tagname=="dc:creator")
                {
                    e.tag.SetAttribute("opf:file-as","");
                }
            }
            f.Apply(ref opf.data);
        }
    }
}