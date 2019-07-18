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
            TextItem opf = epub.OPF;
            XFragment f = XFragment.FindFragment("metadata", opf.data);
            int a = f.root.tagEndRef;
            int b = f.IndexInSource(a);
            opf.data=opf.data.Insert(b, string.Format("\n    <meta name=\"AeroEpubProc\" content=\"{0}\" />\n", metaValue));
        }
    }
}