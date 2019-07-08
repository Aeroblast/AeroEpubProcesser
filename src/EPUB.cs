using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
namespace AeroEpubProcesser
{


    public class Epub
    {
        public List<Item> items;
        public TextItem GetOPF()
        {
            TextItem i = GetItem<TextItem>("META-INF/container.xml");
            if (i == null) { throw new EpubErrorException(); }
            Regex reg = new Regex("<rootfile .*?>");
            XTag tag = XTag.FindTag("rootfile", i.data);
            string opf_path = tag.GetAttribute("full-path");
            TextItem opf = GetItem<TextItem>(opf_path);
            if (opf == null) { throw new EpubErrorException();}
            return  opf;
        }

        public void DeleteEmpty()//只查一层……谁家epub也不会套几个文件夹
        {
            List<Item> tobedelete = new List<Item>();
            foreach (var item in items)
            {
                if (item.fullName.EndsWith('/'))
                {
                    bool refered=false;
                    foreach (var item2 in items)
                    {
                        if (item2.fullName != item.fullName && item2.fullName.StartsWith(item.fullName))
                        {
                            refered=true;
                            break;
                        }
                    }
                    if(!refered)tobedelete.Add(item);
                }

            }
            foreach(var a in tobedelete)items.Remove(a);
        }

        public Item GetItem(string fullName)
        {
            foreach (var i in items) if (i.fullName == fullName) return i;
            return null;
        }
        public T GetItem<T>(string fullName) where T : Item
        {
            Item r = null;
            foreach (var i in items) if (i.fullName == fullName) r = i;
            if (r == null || r.GetType() != typeof(T)) return null;
            return (T)r;

        }
        public void Save(string path, FileMode fileMode = FileMode.Create)
        {
            using (FileStream zipToOpen = new FileStream(path, fileMode))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    foreach (var item in items) { item.PutInto(archive); }
                }
            }
        }
        public Epub(string path)
        {
            items = new List<Item>();
            using (FileStream zipToOpen = new FileStream(path, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        string ext = Path.GetExtension(entry.Name).ToLower();
                        if (entry.FullName == "mimetype")
                        {
                            using (var stm = entry.Open())
                            using (StreamReader r = new StreamReader(stm))
                            {
                                string s = r.ReadToEnd();
                                if (s != "application/epub+zip") throw new EpubtypeException();
                                var i = new MIMETypeItem();
                                items.Insert(0, i);
                            }

                        }
                        else
                            switch (ext)
                            {
                                case ".xhtml":
                                case ".html":
                                case ".xml":
                                case ".css":
                                case ".opf":
                                case ".ncx":
                                case ".svg":
                                case ".js":

                                    using (var stm = entry.Open())
                                    using (StreamReader r = new StreamReader(stm))
                                    {
                                        string s = r.ReadToEnd();
                                        var i = new TextItem(entry.FullName, s);
                                        items.Add(i);
                                    }
                                    break;
                                default:
                                    using (var stm = entry.Open())

                                    {
                                        byte[] d = new byte[entry.Length];
                                        if (entry.Length < int.MaxValue)
                                        {
                                            stm.Read(d, 0, (int)entry.Length);
                                            var i = new NormalItem(entry.FullName, d);
                                            items.Add(i);
                                        }
                                        else { throw new ItemTooLargeException(); }
                                    }
                                    break;
                            }


                    }
                }
            }
            if (items.Count == 0) throw new EpubtypeException();
            if (items[0].GetType() != typeof(MIMETypeItem)) throw new EpubtypeException();
        }
    }

    public abstract class Item
    {
        public string fullName;
        public abstract void PutInto(ZipArchive zip);
    }
    public class TextItem : Item
    {
        public string data;

        public TextItem(string fullName, string data)
        {
            this.fullName = fullName;
            this.data = data;

        }
        public override void PutInto(ZipArchive zip)
        {
            var entry = zip.CreateEntry(fullName);
            using (StreamWriter writer = new StreamWriter(entry.Open()))
            {
                writer.Write(data);
            }

        }
    }
    public class NormalItem : Item
    {
        byte[] data;
        public NormalItem(string fullName, byte[] data)
        {
            this.fullName = fullName;
            this.data = data;

        }
        public override void PutInto(ZipArchive zip)
        {
            var entry = zip.CreateEntry(fullName);
            using (Stream stream = entry.Open())
            {
                stream.Write(data);
            }

        }
    }
    public class MIMETypeItem : Item
    {
        public MIMETypeItem() { fullName = "mimetype"; }
        public override void PutInto(ZipArchive zip)
        {
            var entry = zip.CreateEntry("mimetype", CompressionLevel.NoCompression);//没啥意义，还是Deflate，不是Store

            using (StreamWriter writer = new StreamWriter(entry.Open()))
            {
                writer.Write("application/epub+zip");
            }

        }
    }
    public class ItemTooLargeException : System.Exception { }
    public class EpubtypeException : System.Exception { }
     public class EpubErrorException : System.Exception { }

}