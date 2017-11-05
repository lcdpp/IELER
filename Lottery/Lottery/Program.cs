using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Lottery
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(">>>开始双色球");
            Lottery _lottery_ssq = new Lottery();
            _lottery_ssq.ImportLocalData("lottery_ssq.xml");
            _lottery_ssq.ImportWebData("http://f.apiplus.net/ssq.xml");
            _lottery_ssq.MergeToLocalData();
            _lottery_ssq.ExportDataToLocal("lottery_ssq.xml");

            Console.WriteLine(">>>开始大乐透");
            Lottery _lottery_dlt = new Lottery();
            _lottery_dlt.ImportLocalData("lottery_dlt.xml");
            _lottery_dlt.ImportWebData("http://f.apiplus.net/dlt.xml");
            _lottery_dlt.MergeToLocalData();
            _lottery_dlt.ExportDataToLocal("lottery_dlt.xml");

            Console.Read();
        }
    }

    class Lottery
    {
        class LotteryData
        {
            public string expect;
            public string opencode;
            public string opentime;
        }

        class LotterDataComparer : IComparer<LotteryData>
        {
            public int Compare(LotteryData a, LotteryData b)
            {
                int r = a.expect.CompareTo(b.expect);
                if (r > 0) return -1;
                if (r < 0) return 1;
                return 0;
            }
        }

        List<LotteryData> lstLocalData = new List<LotteryData>();

        List<LotteryData> lstWebData = new List<LotteryData>();

        public void ImportLocalData(string filePath)
        {
            Console.WriteLine(">>>开始导入本地数据");

            lstLocalData.Clear();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(File.ReadAllText(filePath));

            var rows = xmldoc.DocumentElement.GetElementsByTagName("row");
            if(rows != null && rows.Count > 0)
            {
                var it_row = rows.GetEnumerator();
                while(it_row.MoveNext())
                {
                    LotteryData _data = new LotteryData();

                    var row = it_row.Current as XmlElement;
                    _data.expect = row.GetAttribute("expect");
                    _data.opencode = row.GetAttribute("opencode");
                    _data.opentime = row.GetAttribute("opentime");

                    lstLocalData.Add(_data);
                }
            }

            Console.WriteLine(">>>完成导入本地数据：" + lstLocalData.Count.ToString());
        }

        public void ImportWebData(string url)
        {
            Console.WriteLine(">>>开始导入网络数据");

            lstWebData.Clear();

            XmlReader xmlreader = XmlReader.Create(url);
            while (xmlreader.Read())
            {
                if (xmlreader.IsStartElement())
                {
                    if (xmlreader.Name == "row")
                    {
                        LotteryData _data = new LotteryData();
                        _data.expect = xmlreader.GetAttribute("expect");
                        _data.opencode = xmlreader.GetAttribute("opencode");
                        _data.opentime = xmlreader.GetAttribute("opentime");
                        lstWebData.Add(_data);
                    }
                }
            }

            Console.WriteLine(">>>完成导入网络数据：" + lstWebData.Count.ToString());
        }

        public void MergeToLocalData()
        {
            Console.WriteLine(">>>合并网络数据");
            foreach(var _webData in lstWebData)
            {
                var _localData = lstLocalData.Find(x => x.expect == _webData.expect);
                if (_localData == null)
                {
                    lstLocalData.Add(_webData);
                    Console.WriteLine(string.Format(">>>已合并：{0} {1} {2}", _webData.expect, _webData.opencode, _webData.opentime));
                }
                else
                {
                    Console.WriteLine(string.Format(">>>已存在：{0} {1} {2}", _webData.expect, _webData.opencode, _webData.opentime));
                }
            }
        }

        public void ExportDataToLocal(string filePath)
        {
            Console.WriteLine(">>>开始保存数据到本地");

            lstLocalData.Sort(new LotterDataComparer());

            XmlDocument xmldoc = new XmlDocument();
            var xml = xmldoc.CreateElement("xml");
            xmldoc.AppendChild(xml);
            for (int i = 0; i < lstLocalData.Count; ++i)
            {
                var _localData = lstLocalData[i];

                var row = xmldoc.CreateElement("row");
                row.SetAttribute("expect", _localData.expect);
                row.SetAttribute("opencode", _localData.opencode);
                row.SetAttribute("opentime", _localData.opentime);

                xml.AppendChild(row);
            }
            xmldoc.Save(filePath);

            Console.WriteLine(">>>完成保存数据到本地");
        }
    }
}
