using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Xml2CSharp;
using Excel = Microsoft.Office.Interop.Excel;

namespace Xml_To_Excel_01
{
    class Program
    {        
        static void Main(string[] args)
        {
            #region FromXml
            XmlSerializer formatter = new XmlSerializer(typeof(Bill));
            using (StreamReader sr = new StreamReader(@"C:\Users\eveks\Downloads\Telegram Desktop\01.2016  .xml", Encoding.Default))
            using (StreamWriter sw = new StreamWriter(@"C:\Users\eveks\Downloads\Telegram Desktop\01.2016_New.xml"))
            {
                while (!sr.EndOfStream)
                {
                    var tmp = sr.ReadLine().Replace("\x0c", " ");
                    sw.WriteLine(tmp, Encoding.Default);
                }
            }
            Bill bill;
            using (FileStream fs = new FileStream(@"C:\Users\eveks\Downloads\Telegram Desktop\01.2016_New.xml", FileMode.Open))
            {
                XmlReader reader = XmlReader.Create(fs);
                
                bill = (Bill)formatter.Deserialize(reader);
            }
            #endregion

            #region XmlToArray
            var temp = bill.Ch_details.Charges_d.Charge_d.Select(d => new { call = d.C_num, tot = d.C_tot }).ToArray();
            string[,] arrResult = new string[temp.Length,2];
            for (int i = 0; i < arrResult.GetUpperBound(0); i++)
            {
                arrResult[i, 0] = temp[i].call;
                arrResult[i, 1] = temp[i].tot;
            }
            #endregion

            #region ToExcel
            Excel.Application xlApp = new Excel.Application();

            Excel.Workbook xlWb;
            Excel.Worksheet xlSht;
            //Книга.
            xlWb = xlApp.Workbooks.Add(System.Reflection.Missing.Value);
            //Таблица.
            xlSht = (Excel.Worksheet)xlWb.Sheets[1];

            xlSht.Range["A1"]
                .Resize[arrResult.GetUpperBound(0) + 1, arrResult.GetUpperBound(1) + 1]
                .Value = arrResult; //выгрузка массива на лист Excel начиная с А1

            xlWb.Close(true);//закрываем файл и сохраняем изменения, если не сохранять, то false                
            xlApp.Quit(); //закрываем Excel
            #endregion
        }
    }
}
