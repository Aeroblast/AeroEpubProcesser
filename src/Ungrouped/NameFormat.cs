namespace AeroEpubProcesser
{

    public class NameFormater : EpubProcesser
    {
        public override void Process(Epub epub)
        {
            string old = epub.filename;
            string name = string.Format("[{0}]{1}", epub.creator, epub.title);
            name = FilenameCheck(name);
            epub.filename = name;
            Log.log(string.Format("[Info]NameFormater old:{0} new:{1}", old, epub.filename));
        }
        public static string FilenameCheck(string s)
        {
            return s
            .Replace('?', '？')
            .Replace('\\', '＼')
            .Replace('/', '／')
            .Replace(':', '：')
            .Replace('*', '＊')
            .Replace('"', '＂')
            .Replace('|', '｜')
            .Replace('<', '＜')
            .Replace('>', '＞')
            ;
        }
        public override string ToString() { return ""; }
    }
}