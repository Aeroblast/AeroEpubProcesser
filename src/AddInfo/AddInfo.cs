namespace AeroEpubProcesser
{

    public class AddInfo : EpubProcesser
    {
        string metaValue;
        public AddInfo(string v)
        {
            metaValue = v;
        }

        public override void Process(Epub epub)
        {
            TextItem opf = epub.GetOPF();
            XFragment f = XFragment.FindFragment("metadata", opf.data);
            int a = f.root.childs[f.root.childs.Count - 1].tagEndRef;
            int b = f.IndexInSource(a) + f.parts[a].originalText.Length;
            opf.data=opf.data.Insert(b, string.Format("    <meta name=\"AeroEpubProc\" content=\"{0}\" />", metaValue));
        }
    }
}