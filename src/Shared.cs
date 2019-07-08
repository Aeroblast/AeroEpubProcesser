namespace AeroEpubProcesser
{
    public class Shared
    {
        public static string tempPath = "_temp_";
        public static void ClearTemp(){Util.DeleteDir(tempPath);}

    }
    public abstract class EpubProcesser
    {
        public abstract void Process(Epub epub); 
    }
}