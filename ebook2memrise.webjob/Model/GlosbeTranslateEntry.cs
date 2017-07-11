using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ebook2memrise.webjob.Model
{
    public class Phrase
    {
        public string text { get; set; }
        public string language { get; set; }
    }

    public class Meaning
    {
        public string language { get; set; }
        public string text { get; set; }
    }

    public class Tuc
    {
        public Phrase phrase { get; set; }
        public List<Meaning> meanings { get; set; }
        public object meaningId { get; set; }
        public List<int> authors { get; set; }
    }

    public class __invalid_type__93088
    {
        public string U { get; set; }
        public int id { get; set; }
        public string N { get; set; }
        public string url { get; set; }
    }

    public class __invalid_type__1
    {
        public string U { get; set; }
        public int id { get; set; }
        public string N { get; set; }
        public string url { get; set; }
    }

    public class __invalid_type__20
    {
        public string U { get; set; }
        public int id { get; set; }
        public string N { get; set; }
        public string url { get; set; }
    }

    public class __invalid_type__97990
    {
        public string U { get; set; }
        public int id { get; set; }
        public string N { get; set; }
        public string url { get; set; }
    }

    public class __invalid_type__93369
    {
        public string U { get; set; }
        public int id { get; set; }
        public string N { get; set; }
        public string url { get; set; }
    }

    public class __invalid_type__91945
    {
        public string U { get; set; }
        public int id { get; set; }
        public string N { get; set; }
        public string url { get; set; }
    }

    public class __invalid_type__25018
    {
        public string U { get; set; }
        public int id { get; set; }
        public string N { get; set; }
        public string url { get; set; }
    }

    public class Authors
    {
        public __invalid_type__93088 __invalid_name__93088 { get; set; }
        public __invalid_type__1 __invalid_name__1 { get; set; }
        public __invalid_type__20 __invalid_name__20 { get; set; }
        public __invalid_type__97990 __invalid_name__97990 { get; set; }
        public __invalid_type__93369 __invalid_name__93369 { get; set; }
        public __invalid_type__91945 __invalid_name__91945 { get; set; }
        public __invalid_type__25018 __invalid_name__25018 { get; set; }
    }

    public class GlosbeTranslateEntry
    {
        public string result { get; set; }
        public List<Tuc> tuc { get; set; }
        public string phrase { get; set; }
        public string from { get; set; }
        public string dest { get; set; }
        public Authors authors { get; set; }
    }
}
