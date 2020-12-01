using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = @"C:\Users\Administrator\Desktop\ConsoleApp4\ConsoleApp4\接口数据库.mdb";
            var acce = new AccessCommon(file);
            var tableNames = new List<string> {
                "Fangwu",
                "FenTan",
                "QuYu",
                "LouCeng",
                "HuShi",
                "Hushitsuchuan"};
            //var length = 10;
            //var count = 0;
            //var sw = Stopwatch.StartNew();

            //for (int i = 0; i < length; i++)
            //{
            //    var tb = acce.GetTableByName("Fangwu");
            //    var graphic_fangwu = tb.ToList<Fangwu>();
            //    count += graphic_fangwu.Count();
            //    tb = acce.GetTableByName("FenTan");
            //    var graphic_fentan = tb.ToList<FenTan>();
            //    count += graphic_fentan.Count();
            //}
            //sw.Stop();
            //Console.WriteLine($"耗时1：{sw.ElapsedMilliseconds}");
            //Console.WriteLine($"数量:{count}");
            //count = 0;
            //sw.Restart();
            //for (int i = 0; i < length; i++)
            //{
            //    var tb = acce.GetTableByName("Fangwu");
            //    var graphic_fangwu = tb.ToList1<Fangwu>();
            //    count += graphic_fangwu.Count();
            //    tb = acce.GetTableByName("FenTan");
            //    var graphic_fentan = tb.ToList1<FenTan>();
            //    count += graphic_fentan.Count();
            //}
            //sw.Stop();
            //Console.WriteLine($"耗时2：{sw.ElapsedMilliseconds}");
            //Console.WriteLine($"数量:{count}");
            //return;

            var tb = acce.GetTableByName("Fangwu");
            var graphic_fangwu = tb.ToList1<Fangwu>();
            tb = acce.GetTableByName("FenTan");
            var graphic_fentan = tb.ToList1<FenTan>();
            tb = acce.GetTableByName("QuYu");
            var QuYu = tb.ToList1<QuYu>();

            tb = acce.GetTableByName("LouCeng");
            var LouCeng = tb.ToList<LouCeng>();

            tb = acce.GetTableByName("HuShi");
            var HuShi = tb.ToList1<HuShi>();

            tb = acce.GetTableByName("Hushitsuchuan");
            var Hushitsuchuan = tb.ToList1<Hushitsuchuan>();

            //foreach (var tableName in tableNames)
            //{
            //    var tb = acce.GetTableByName(tableName);
            //    PrintTable(tb);
            //}
        }

        static void PrintTable(DataTable dt)
        {
            foreach (DataColumn column in dt.Columns)
            {
                Console.Write(column.ColumnName);
                Console.Write("\t");
            }
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    Console.Write(row[column].ToString());
                    Console.Write("\t");
                }
                Console.WriteLine();
            }
        }
    }
}
