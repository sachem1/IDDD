using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Mapper;
using Coralcode.Framework.Models;
using NPOI.SS.Formula.Functions;

namespace Coralcode.Framework.Services
{
    public class BatchImportExportService<TEntity, TViewModel> : CoralService
        where TViewModel : class, IViewModel, new()
        where TEntity : Entity, new()
    {

        /// <summary>
        /// Excel导入大数据
        /// </summary>
        /// <param name="errorMsgColumn">验证信息导出字段名称</param>
        /// <param name="stream">原文件流</param>
        /// <param name="valideAndPreProcess">验证以及预处理</param>
        /// <param name="saveAction">入库委托</param>
        /// <param name="ftpUploadAction">上传委托</param>
        /// <returns></returns>
        public void Import(string errorMsgColumn, Stream stream, 
            Func<List<TViewModel>, List<ImportMessage<TViewModel>>> valideAndPreProcess,
            Action<List<DataTable>> saveAction,
            Action<Stream, Stream> ftpUploadAction)
        {

            var originalStream = ConvertMemoryStream(stream);

            var originalDataTable = new DataTable().ImportBySheetIndex(stream)
                .AddColumn(errorMsgColumn, typeof(string));

            var result = ValidViewModel(originalDataTable, valideAndPreProcess);
            try
            {
                var errorStream = SetErrorData(errorMsgColumn, originalDataTable, result);

                if (ftpUploadAction != null)
                    ftpUploadAction(originalStream, errorStream);

                PutDataInDb(result, saveAction);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <param name="originalDataTable">Excel转成的DataTable</param>
        /// <param name="valideAndPreProcess">校验委托</param>
        /// <returns></returns>
        private List<ImportMessage<TViewModel>> ValidViewModel(DataTable originalDataTable, Func<List<TViewModel>, List<ImportMessage<TViewModel>>> valideAndPreProcess)
        {
            // Excel转DataTable验证
            List<ImportMessage> errorMessages;
            var datas = new List<TViewModel>().ImportWithDescription(originalDataTable, out errorMessages);

            var result = errorMessages.Select(item => new ImportMessage<TViewModel>
            {
                Index = item.Index,
                State = item.State,
                ErrorMessage = item.ErrorMessage
            }).ToList();

            // 无需自定义验证
            if (valideAndPreProcess == null) return result;

            // 自定义校验
            var selfValidList = valideAndPreProcess(datas);
            if (selfValidList != null && selfValidList.Count > 0)
                result.AddRange(selfValidList);

            return result;
        }

        /// <summary>
        /// 设置错误数据
        /// </summary>
        /// <param name="columnName">提示消息字段名称</param>
        /// <param name="originalDataTable">Excel转成的DataTable</param>
        /// <param name="dataList">已校验数据列表</param>
        /// <returns></returns>
        private Stream SetErrorData(string columnName, DataTable originalDataTable, List<ImportMessage<TViewModel>> dataList)
        {
            // 模板校验异常数据
            var errorData = dataList.Where(item => item.State == ResultState.Fail && item.Model != null)
                .Select(item => item.Model)
                .ToList()
                .ExportWithDescription();

            dataList.Where(item => item.State == ResultState.Fail && item.Model == null).ForEach(item =>
            {
                var errorRow = originalDataTable.Rows[item.Index];
                errorRow[columnName] = item.ErrorMessage;
                errorData.ImportRow(errorRow);
            });

            if (errorData.Rows.Count <= 0) return null;
            // 导出数据
            return errorData.Export();
        }

        /// <summary>
        /// 入库
        /// </summary>
        /// <param name="dataList">数据</param>
        /// <param name="saveAction">入库委托</param>
        private void PutDataInDb(List<ImportMessage<TViewModel>> dataList, Action<List<DataTable>> saveAction)
        {
            // 入库数据
            var successData = Convert2TEntities(dataList.Where(item => item.State == ResultState.Success).Select(item => item.Model).ToList())
                .Export();
            // 入库数据已切片
            var sliceSuccessData = SliceDataTable(successData);
            saveAction(sliceSuccessData);
        }
        
        /// <summary>
        /// TViewModel转TEntity
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private List<TEntity> Convert2TEntities(List<TViewModel> datas)
        {
            return DataMapperProvider.Mapper.Convert<List<TViewModel>, List<TEntity>>(datas);
        }

        /// <summary>
        /// dataTable切块
        /// </summary>
        /// <param name="blockSize">每块大小</param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private List<DataTable> SliceDataTable(DataTable dataTable, int blockSize = 5000)
        {
            // 设置最大块为30000
            if (blockSize > 30000)
                blockSize = 30000;

            var rows = dataTable.Rows;
            var rowsCount = rows.Count;
            var remainder = rowsCount % blockSize;
            var dividend = rowsCount / blockSize;

            // 只有一块
            if (dividend == 0)
            {
                return EqualsDataTable(1, rows.Count, rows, dataTable.Clone());
            }

            // 大于一块且被整除或未被整除
            var result = (dividend != 0 && remainder == 0)
                ? EqualsDataTable(dividend, blockSize, rows, dataTable.Clone())
                : EqualsDataTable(dividend + 1, blockSize, rows, dataTable.Clone());

            return result;
        }

        /// <summary>
        /// 切片块赋值
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="rows"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private List<DataTable> EqualsDataTable(int count, int size, DataRowCollection rows, DataTable dataTable)
        {
            var result = new List<DataTable>(count);
            for (int i = 0; i < count; i++)
            {
                var da = dataTable.Clone();
                var idList = IdentityGenerator.GetIdList(size);
                for (int j = 0; j < size; j++)
                {
                    var index = size * i + j;
                    if (index < rows.Count)
                    {
                        da.ImportRow(rows[index]);
                        da.Rows[j]["Id"] = idList[j];
                    }
                    else
                        break;
                }
                result.Add(da);
            }

            return result;
        }

        /// <summary>
        /// 流拷贝
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        private MemoryStream ConvertMemoryStream(Stream input)
        {
            int bufferSize = 2048;
            byte[] buffer = new byte[bufferSize];
            MemoryStream result = new MemoryStream();
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    input.Seek(0, SeekOrigin.Begin);
                    result.Seek(0, SeekOrigin.Begin);
                    return result;
                }
                result.Write(buffer, 0, read);
            }
        }
    }
}
