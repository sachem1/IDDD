using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Coralcode.Framework.Extensions
{
    public static class DataSetExtensions
    {
        /// <summary>
        /// 按照sheet的名称导入
        /// </summary>
        /// <param name="table"></param>
        /// <param name="stream"></param>
        /// <param name="sheetName"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static DataTable ImportBySheetName(this DataTable table, Stream stream, string sheetName = "Sheet1", int rowIndex = 1)
        {
            //根据路径通过已存在的excel来创建HSSFWorkbook，即整个excel文档
            var workbook = WorkbookFactory.Create(stream);

            //获取excel的第一个sheet
            var sheet = workbook.GetSheet(sheetName);

            //生成表头
            sheet.GetRow(0).Cells.ForEach(item =>
            {
                var column = new DataColumn(item.StringCellValue);
                table.Columns.Add(column);
            });

            //从第二行开始取数据
            for (int i = (sheet.FirstRowNum + rowIndex); i <= sheet.LastRowNum; i++)
            {
                DataRow dataRow = table.NewRow();
                sheet.GetRow(i)
                    .Cells.Where(item => item != null)
                    .ToList()
                    .ForEach(item => { dataRow[item.ColumnIndex] = item.ToString(); });
                table.Rows.Add(dataRow);
            }
            return table;
        }

        /// <summary>
        /// 按照sheet的索引导入
        /// </summary>
        /// <param name="table"></param>
        /// <param name="stream"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static DataTable ImportBySheetIndex(this DataTable table, Stream stream, int sheetIndex = 0, int rowIndex = 1)
        {

            //根据路径通过已存在的excel来创建HSSFWorkbook，即整个excel文档
            var workbook = WorkbookFactory.Create(stream);

            //获取excel的第一个sheet
            var sheet = workbook.GetSheetAt(sheetIndex);

            //生成表头
            sheet.GetRow(0).Cells.ForEach(item =>
            {
                if (string.IsNullOrEmpty(item.StringCellValue))
                    return;
                if (string.IsNullOrWhiteSpace(item.StringCellValue))
                    return;
                var column = new DataColumn(item.StringCellValue.Trim());

                table.Columns.Add(column);
            });

            //从第三行开始取数据
            for (int i = (sheet.FirstRowNum + rowIndex); i <= sheet.LastRowNum; i++)
            {
                DataRow dataRow = table.NewRow();
                var row = sheet.GetRow(i);
                if (row == null || row.FirstCellNum == -1 || string.IsNullOrEmpty(row.Cells[0].ToString()))
                    continue;
                row.Cells.Where(item => item != null).ToList().ForEach(item =>
                {
                    if (item.CellType == CellType.Numeric)
                    {
                        short format = item.CellStyle.DataFormat;
                        if (format == 14 || format == 31 || format == 57 || format == 58 || format == 176)
                        {
                            DateTime date = item.DateCellValue;
                            dataRow[item.ColumnIndex] = date.ToString("yyyy-MM-dd");
                            return;
                        }
                    }

                    if (item.ColumnIndex < table.Columns.Count)
                        dataRow[item.ColumnIndex] = item.ToString().Trim().Trim('_');
                });
                table.Rows.Add(dataRow);
            }
            return table;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static MemoryStream Export(this DataTable table)
        {
            var ms = new MemoryStream();
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();
            IDataFormat dataformat = workbook.CreateDataFormat();
            ICellStyle style1 = workbook.CreateCellStyle();
            style1.DataFormat = dataformat.GetFormat("text");
            var headerRow = sheet.CreateRow(0);
            // handling header.
            foreach (DataColumn column in table.Columns)
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);
            //If Caption not set, returns the ColumnName value
            // handling value.
            int rowIndex = 1;
            foreach (DataRow row in table.Rows)
            {
                var dataRow = sheet.CreateRow(rowIndex);

                foreach (DataColumn column in table.Columns)
                {
                    var columnValue = row[column].ToString();
                    if (column.DataType == typeof(Enum)
                            || column.DataType.BaseType == typeof(Enum))
                    {
                        columnValue = column.DataType.GetDescription((int)row[column]);
                    }

                    var cell = dataRow.CreateCell(column.Ordinal);
                    cell.CellStyle = style1;
                    cell.SetCellValue(columnValue);
                }
                rowIndex++;
            }

            workbook.Write(ms);

            var returnStream = new System.IO.MemoryStream(ms.ToArray());
            return returnStream;
        }

        /// <summary>
        /// 按照列名导出
        /// </summary>
        /// <param name="table"></param>
        /// <param name="header"></param>
        /// <param name="focusHeader">是否只导出对应的列</param>
        /// <returns></returns>
        public static MemoryStream Export(this DataTable table, Dictionary<string, string> header, bool focusHeader = false)
        {
            var ms = new MemoryStream();
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet();

            var headerRow = sheet.CreateRow(0);

            int columnIndex = 0;
            // handling header.
            foreach (DataColumn column in table.Columns)
            {
                if (header.ContainsKey(column.ColumnName))
                {
                    headerRow.CreateCell(columnIndex).SetCellValue(header[column.ColumnName]);
                    columnIndex++;
                }
                else if (!focusHeader)
                {
                    headerRow.CreateCell(columnIndex).SetCellValue(column.Caption);
                    columnIndex++;
                }
            }

            //If Caption not set, returns the ColumnName value
            // handling value.
            int rowIndex = 1;
            foreach (DataRow row in table.Rows)
            {
                var dataRow = sheet.CreateRow(rowIndex);
                columnIndex = 0;
                foreach (DataColumn column in table.Columns)
                {
                    if (focusHeader && !header.ContainsKey(column.ColumnName))
                    {
                        continue;
                    }
                    dataRow.CreateCell(columnIndex).SetCellValue(row[column].ToString());
                    columnIndex++;
                }
                rowIndex++;
            }
            workbook.Write(ms);
            return ms;
        }

        #region 其他代码
        //private static string Export<T>(string title, DataTable table) where T : class, new()
        //{
        //    hssfworkbook = new HSSFWorkbook();

        //    if (string.IsNullOrEmpty(title))
        //        title = "_ExportSheet_";
        //    var sheet = hssfworkbook.CreateSheet(title);

        //    var headerRow = sheet.CreateRow(0);
        //    var columns = GetPropertyAttributes<T>();

        //    for (var i = 0; i < columns.Count; i++)
        //    {
        //        var current = columns[i];
        //        headerRow.CreateCell(i).SetCellValue(current.Name);
        //    }
        //    int rowIndex = 1;
        //    var columFields = columns.Select(v => v.Field);
        //    foreach (DataRow row in table.Rows)
        //    {
        //        var dataRow = sheet.CreateRow(rowIndex);
        //        int columnIndex = 0;
        //        foreach (
        //            DataColumn column in
        //                table.Columns.ToList<DataColumn>().Where(v => columFields.Contains(v.ColumnName)).ToList())
        //        {
        //            dataRow.CreateCell(columnIndex).SetCellValue(row[column].ToString());
        //            columnIndex++;
        //        }
        //        rowIndex++;
        //    }

        //    var filepath = ReturnFilePath<T>(title, "导出数据");
        //    return filepath;
        //}

        //public static string Export<T>(string title, IList<T> data) where T : class, new()
        //{
        //    var dt = new DataTable().Import<T>(data );
        //    return Export<T>(title, dt);
        //}



        //static HSSFWorkbook hssfworkbook;
        //public static string SetExportTemplate<T>(string title, Dictionary<string, IEnumerable<string>> data)
        //    where T : class, new()
        //{
        //    InitializeWorkbook();
        //    if (string.IsNullOrEmpty(title))
        //        title = "_ImportTemplateSheet_";

        //    ISheet sheet1 = hssfworkbook.CreateSheet(title);
        //    IRow row = sheet1.CreateRow(0);

        //    var columns = GetPropertyAttributes<T>();
        //    //表头导出,仅导出有标识的属性
        //    if (data == null || data.Count < 1)
        //    {
        //        for (var i = 0; i < columns.Count; i++)
        //        {
        //            var current = columns[i];
        //            row.CreateCell(i).SetCellValue(current.Name);
        //        }
        //    }
        //    //自定义下拉框列表导出
        //    else
        //    {
        //        var exceptResult = data.Keys.ToList().Except(columns.Select(v => v.Field).ToList());
        //        if (exceptResult.Any())
        //        {
        //            //错误数据追加到Excel
        //            AddErrorMessage(row, 0, string.Format("{0}【{1}】", "错误信息:非法导出属性,", string.Join(",", exceptResult)));
        //        }
        //        else
        //        {
        //            IEnumerable<string> outResult;
        //            List<string> errorMessages = new List<string>();
        //            for (var i = 0; i < columns.Count; i++)
        //            {
        //                var current = columns[i];
        //                if (data.Keys.Any(v => v == current.Field))
        //                {
        //                    data.TryGetValue(current.Field, out outResult);
        //                    if (!outResult.Any())
        //                    {
        //                        errorMessages.Add(current.Field);
        //                        continue;
        //                    }
        //                }
        //                if (errorMessages.Any())
        //                {
        //                    //错误数据追加到Excel
        //                    AddErrorMessage(row, 1, string.Format("{0}【{1}】", "错误信息:下拉框为空,", string.Join(",", errorMessages)));
        //                }
        //                else
        //                {
        //                    row.CreateCell(i).SetCellValue(current.Name);
        //                    int index = 0;
        //                    foreach (HSSFDataValidation dataValidation in from item in data
        //                                                                  where item.Key.Equals(current.Field)
        //                                                                  select CreateListConstaint(hssfworkbook, item.Key, true, index + i, item.Value))
        //                    {
        //                        dataValidation.CreateErrorBox("输入不合法", "请输入下拉列表中的值!");
        //                        ((HSSFSheet)sheet1).AddValidationData(dataValidation);
        //                        sheet1.DisplayFormulas = true;
        //                        sheet1.DisplayGuts = true;
        //                        sheet1.DisplayGridlines = true;
        //                        sheet1.DisplayZeros = true;
        //                        index++;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    var filepath = ReturnFilePath<T>(title, "导入模板");
        //    return filepath;
        //}

        ///// <summary>
        ///// 错误数据追加到Excel
        ///// </summary>
        ///// <param name="row"></param>
        ///// <param name="index"></param>
        ///// <param name="errorMessage"></param>
        //private static void AddErrorMessage(IRow row,int index, string errorMessage)
        //{
        //    row.CreateCell(index).SetCellValue(errorMessage);
        //}

        //private static string ReturnFilePath<T>(string title, string fileType) where T : class, new()
        //{
        //    string importTemplate = ConfigurationManager.AppSettings["ImportTemplate"];

        //    if (!Directory.Exists(importTemplate))
        //    {
        //        Directory.CreateDirectory(importTemplate);
        //    }
        //    string flax = string.Format("{0:yyyyMMddHHmmssff}", DateTime.Now);
        //    string filepath = string.Format("{0}{1}_{2}_{3}.{4}", importTemplate + "\\", title, fileType, flax, "xls");

        //    FileStream file = new FileStream(filepath, FileMode.Create);
        //    hssfworkbook.Write(file);
        //    file.Close();
        //    file.Dispose();
        //    return filepath;
        //}

        ////todo 这里需要区分导入字段 和导出字段，因为有些字段是只支持查看，而有些字段只支持导入
        //private static List<DataGridColumn> GetPropertyAttributes<T>() where T : class, new()
        //{
        //    var viewModelType = typeof (T);
        //    var properties =
        //        viewModelType.GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        //    var columns = new List<DataGridColumn>();
        //    foreach (var propertyInfo in properties)
        //    {
        //        var descAttr = propertyInfo.GetCustomAttribute<DataGridColumnAttribute>();
        //        if (descAttr == null) continue;

        //        var column = descAttr.DataGridColumn;
        //        if (string.IsNullOrWhiteSpace(column.Field))
        //        {
        //            column.Field = propertyInfo.Name;
        //        }
        //        if (string.IsNullOrEmpty(column.Name))
        //            continue;
        //        columns.Add(column);
        //    }
        //    return columns;
        //}

        //private static HSSFDataValidation CreateListConstaint(HSSFWorkbook book, string sheetName, bool isHidden,
        //    Int32 columnIndex, IEnumerable<String> values)
        //{
        //    if (string.IsNullOrWhiteSpace(sheetName))
        //        sheetName = "_constraintSheet_";
        //    ISheet sheet = book.GetSheet(sheetName) ?? book.CreateSheet(sheetName);
        //    var firstRow = sheet.GetRow(0);
        //    var conColumnIndex = firstRow == null ? 0 : firstRow.PhysicalNumberOfCells;
        //    var rowIndex = 0;
        //    var lastValue = "";

        //    var enumerable = values as IList<string> ?? values.ToList();
        //    foreach (var value in enumerable)
        //    {
        //        var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
        //        row.CreateCell(conColumnIndex).SetCellValue(value);
        //        rowIndex++;
        //        lastValue = value;
        //    }

        //    if (enumerable.Count() == 1)
        //    {
        //        var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
        //        row.CreateCell(conColumnIndex).SetCellValue(lastValue);
        //        rowIndex++;
        //    }

        //    IName range = book.CreateName();
        //    range.RefersToFormula = String.Format("{2}!${0}$1:${0}${1}", (Char) ('A' + conColumnIndex), rowIndex,
        //        sheetName);
        //    string rangeName = "dicRange" + columnIndex;
        //    range.NameName = rangeName;
        //    var cellRegions = new CellRangeAddressList(1, 65535, columnIndex, columnIndex);
        //    var constraint = DVConstraint.CreateFormulaListConstraint(rangeName);
        //    book.SetSheetHidden(book.GetSheetIndex(sheet), isHidden);
        //    return new HSSFDataValidation(cellRegions, constraint);
        //}

        ///// <summary>
        ///// 把Excel文件读取到列表
        ///// </summary>
        ///// <typeparam name="T">需要转换成的对象</typeparam>
        ///// <param name="path">文件路径</param>
        ///// <param name="sheetName">待读取表单名称</param>
        ///// <param name="columnMap">Key:Excel的列名称，Value:对应的T的属性名称</param>
        ///// <param name="minMatchedColumn">最少匹配列数</param>
        ///// <returns></returns>
        //public static Dictionary<List<string>, IEnumerable<T>> ReadAs<T>(string path, string sheetName,
        //    IDictionary<string, string> columnMap) where T : class, new()
        //{
        //    var tResult = new Dictionary<List<string>, IEnumerable<T>>();

        //    var properties =
        //        typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        //    var list = new List<T>();

        //    var errorMessages = new List<string>();
        //    path = Path.GetFullPath(path);
        //    if (!File.Exists(path))
        //    {
        //        errorMessages.Add(string.Format("指定路径：{0}的文件：{1}不存在", Path.GetDirectoryName(path),
        //            Path.GetFileName(path)));
        //    }

        //    using (var fs = File.OpenRead(path))
        //    {
        //        IWorkbook workBook = null;
        //        switch (Path.GetExtension(path))
        //        {
        //            case ".xls":
        //                workBook = new HSSFWorkbook(fs);
        //                break;
        //            case ".xlsx":
        //                workBook = new XSSFWorkbook(fs);
        //                break;
        //        }
        //        if (workBook == null)
        //        {

        //            errorMessages.Add("不能解析Excel以外的文件");
        //        }
        //        ISheet workSheet = null;
        //        workSheet = !string.IsNullOrWhiteSpace(sheetName)
        //            ? workBook.GetSheet(sheetName)
        //            : workBook.GetSheetAt(0);
        //        if (workSheet == null)
        //        {
        //            errorMessages.Add("未找到Sheet页");
        //        }

        //        columnMap = new Dictionary<string, string>();
        //        foreach (var attribute in GetPropertyAttributes<T>())
        //        {
        //            columnMap.Add(attribute.Name, attribute.Field);
        //        }

        //        bool hasColumnMap = columnMap.Count > 0;

        //        var dic = new Dictionary<string, int>();

        //        var row = workSheet.GetRow(0);

        //        //首行值验证
        //        if (row == null)
        //        {
        //            errorMessages.Add("表格第1行值不能为空！");
        //            tResult.Add(errorMessages, list);
        //            return tResult;
        //        }

        //        for (var j = 0; j < row.LastCellNum; j++)
        //        {
        //            var cell = row.GetCell(j);

        //            if (cell == null)

        //                errorMessages.Add(string.Format("表格第一行第[{0}]列，值不能为空！", j));

        //            var cellValue = cell.ToString();
        //            if (!string.IsNullOrWhiteSpace(cellValue))
        //            {
        //                if (hasColumnMap)
        //                {
        //                    if (columnMap.Keys.Contains(cellValue))
        //                    {
        //                        dic.Add(columnMap[cellValue], j);
        //                    }
        //                    else
        //                        errorMessages.Add(string.Format("实体类中不包含属性：[{0}]", cellValue));
        //                }
        //            }
        //        }
        //        //列名合法验证
        //        if (errorMessages.Any())
        //        {
        //            tResult.Add(errorMessages, list);
        //            return tResult;
        //        }
        //        //属性赋值
        //        //TODO:NPOI处理空白行会追加到RowNum,待调整
        //        for (int i = 1; i <= workSheet.LastRowNum; i++)
        //        {
        //            var currentRow = workSheet.GetRow(i);
        //            var instance = new T();
        //            foreach (var property in properties)
        //            {
        //                var descAttr = property.GetCustomAttribute<DataGridColumnAttribute>();
        //                if (descAttr == null) continue;

        //                var column = descAttr.DataGridColumn;
        //                if (string.IsNullOrEmpty(column.Field))
        //                    column.Field = property.Name;
        //                if (dic.Keys.Contains(column.Field) && property.CanWrite)
        //                {
        //                    var cell = currentRow.GetCell(dic[column.Field]);
        //                    if (cell != null)
        //                    {
        //                        property.SetValue(instance, Convert.ChangeType(cell.ToString(), property.PropertyType));
        //                    }
        //                }
        //            }
        //            list.Add(instance);
        //        }
        //    }
        //    tResult.Add(errorMessages, list);
        //    return tResult;
        //}

        //public static Dictionary<List<string>, IEnumerable<T>> ReadAs<T>(string path) where T : class, new()
        //{
        //    return ReadAs<T>(path, null, null);
        //}

        //static void InitializeWorkbook()
        //{
        //    hssfworkbook = new HSSFWorkbook();

        //    //create a entry of DocumentSummaryInformation
        //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
        //    dsi.Company = "NPOI Team";
        //    hssfworkbook.DocumentSummaryInformation = dsi;

        //    //create a entry of SummaryInformation
        //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
        //    si.Subject = "NPOI SDK Example";
        //    hssfworkbook.SummaryInformation = si;
        //}
        #endregion
    }
}