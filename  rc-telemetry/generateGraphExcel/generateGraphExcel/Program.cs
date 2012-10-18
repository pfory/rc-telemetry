using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Office.Interop.Excel;


namespace generateGraphExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            generateGraph();
        }

        static void generateGraph()
        {
            Application excel = new Application();
            excel.Visible = true;


            Workbook wb = (Workbook)excel.Workbooks.Add();
            Worksheet sheet = excel.Sheets.Add();

            getDataFromSQL(wb);

            Chart chart = excel.ActiveWorkbook.Charts.Add(After: excel.ActiveSheet);
            chart.SetSourceData(sheet.get_Range("A1:D28"), XlRowCol.xlColumns);

            Axis axis = chart.Axes(XlAxisType.xlValue, XlAxisGroup.xlPrimary);
            axis.HasTitle = true;
            axis.AxisTitle.Text = "Teplota [°C]";

            axis = chart.Axes(XlAxisType.xlCategory, XlAxisGroup.xlPrimary);
            axis.HasTitle = true;
            axis.AxisTitle.Text = "Datum";
            axis.CategoryType = XlCategoryType.xlTimeScale;

            //chart.ChartType = Microsoft.Office.Interop.Excel.XlChartType.xlStockHLC;
            //chart.ChartStyle = 45;
        }

        static void getDataFromSQL(Microsoft.Office.Interop.Excel.Workbook wb)
        {
            string connectString = "SERVER=HOMEROVO;UID=sa;PWD=fikus12zeleny;APP=Microsoft Office 2003;WSID=PFORY;DATABASE=teploty;";
            SqlConnection con = new SqlConnection(connectString);
            con.Open();

            string sql = "select datumCas, mint2, avgt2, maxt2 from dbo.dataDEN where datumCas between '2012-01-01 00:00' and '2012-02-01 00:00'";

            SqlCommand command = new SqlCommand(sql, con);
            command.CommandType = System.Data.CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            int i = 0;
            while (reader.Read())
            {
                Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)wb.ActiveSheet;
                FormatConditions fc = sheet.get_Range("A1:A28").FormatConditions;
                //fc.Add(DateOperator:
                sheet.Range["A" + ++i, Type.Missing].Value2 = reader[0];
                sheet.Range["A" + ++i, Type.Missing].FormatConditions.Add(XlColumnDataType.xlDMYFormat);


                sheet.get_Range("B" + i).Value2 = reader[1];
                sheet.get_Range("C" + i).Value2 = reader[2];
                sheet.get_Range("D" + i).Value2 = reader[3];
            }
            
            reader.Close();
        }
    }
}
