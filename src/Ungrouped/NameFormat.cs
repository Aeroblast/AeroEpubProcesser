namespace AeroEpubProcesser
{

    public class NameFormater : EpubProcesser
    {
        public override void Process(Epub epub)
        {
            string old=epub.filename;
            epub.filename=string.Format("[{0}]{1}",epub.creator,epub.title);
            Log.log(string.Format("[Info]NameFormater old:{0} new:{1}",old,epub.filename));
        }
    }
}