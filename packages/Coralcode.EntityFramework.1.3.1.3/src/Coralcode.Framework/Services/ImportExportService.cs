using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Models;

namespace Coralcode.Framework.Services
{
    public class ImportExportService<TViewModel, TSearch,TOrder> : CoralService
        where TViewModel : class, IViewModel, new()
        where TSearch : SearchBase, new()
        where TOrder : OrderBase, new()
    {
        private readonly ICrudCoralService<TViewModel, TSearch,TOrder> _service;
        public ImportExportService(ICrudCoralService<TViewModel, TSearch, TOrder> service)
        {
            _service = service;
        }

        /// <summary>
        /// 获取导出数据模板路径
        /// </summary>
        /// <returns></returns>
        public MemoryStream ExportTemplate()
        {
            return Export(new List<TViewModel>());
        }

        /// <summary>
        /// 获取导出文件路径
        /// </summary>
        /// <param name="viewModels"></param>
        /// <returns></returns>
        public MemoryStream Export(List<TViewModel> viewModels)
        {
            return viewModels.ExportWithDescription().Export();
        }


        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="valideAndPreProcess">验证以及预处理</param>
        /// <param name="createFunc"></param>
        /// <returns></returns>
        public List<ImportMessage<TViewModel>> Import(Stream stream, Func<TViewModel, int, ImportMessage<TViewModel>> valideAndPreProcess, Action<TViewModel> createFunc)
        {
            List<ImportMessage> errorMessages;
            var datas = new List<TViewModel>().ImportWithDescription(new DataTable().ImportBySheetIndex(stream), out errorMessages);

            var result = new List<ImportMessage<TViewModel>>();
            if (errorMessages.Count > 0)
            {
                errorMessages.ForEach(e =>
                {
                    result.Add(new ImportMessage<TViewModel>
                    {
                        Index = e.Index,
                        State = e.State,
                        ErrorMessage = e.ErrorMessage
                    });
                });
            }

            int index = 0;

            foreach (var item in datas)
            {
                var tmp = valideAndPreProcess(item, index);
                if (tmp.State == ResultState.Success)
                {
                    createFunc(item);
                }
                result.Add(tmp);
                index++;
            }
            return result;
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="valideAndPreProcess">验证以及预处理</param>
        /// <returns></returns>
        public List<ImportMessage<TViewModel>> Import(Stream stream, Func<TViewModel, int, ImportMessage<TViewModel>> valideAndPreProcess)
        {
            List<ImportMessage> errorMessages;
            var datas = new List<TViewModel>().ImportWithDescription(new DataTable().ImportBySheetIndex(stream), out errorMessages);

            var result = new List<ImportMessage<TViewModel>>();
            if (errorMessages.Count > 0)
            {
                errorMessages.ForEach(e =>
                {
                    result.Add(new ImportMessage<TViewModel>
                    {
                        Index = e.Index,
                        State = e.State,
                        ErrorMessage = e.ErrorMessage
                    });
                });
            }

            int index = 0;

            foreach (var item in datas)
            {
                var tmp = valideAndPreProcess(item, index);
                if (tmp.State == ResultState.Success)
                {
                    
                    _service.Create(item);
                }
                result.Add(tmp);
                index++;
            }
            return result;
        }

    }


    public class ImportExportService<TViewModel, TSearch> : ImportExportService<TViewModel, TSearch, OrderBase>
        where TViewModel : class, IViewModel, new()
        where TSearch : SearchBase, new()
    {
        public ImportExportService(ICrudCoralService<TViewModel, TSearch, OrderBase> service) : base(service)
        {
        }
    }


    public class ImportExportService<TViewModel> : ImportExportService<TViewModel, SearchBase, OrderBase>
        where TViewModel : class, IViewModel, new()
    {
        public ImportExportService(ICrudCoralService<TViewModel, SearchBase, OrderBase> service)
            : base(service)
        {
        }
    }


    public class ImportMessage
    {
        /// <summary>
        /// 行数
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public ResultState State { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
    public class ImportMessage<TViewModel> : ImportMessage
           where TViewModel : class, IViewModel, new()
    {
        public TViewModel Model { get; set; }
    }
}