using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Services.Description;
using Coralcode.Framework.Common;
using Microsoft.CSharp;

namespace Coralcode.Framework.WCF
{
    public class WsUtil
    {
        /// <summary> 
        /// 动态调用web服务 
        /// </summary> 
        /// <param name="url">WSDL服务地址</param> 
        /// <param name="methodname">方法名</param> 
        /// <param name="args">参数</param> 
        /// <returns></returns> 
        public static object InvokeWebService(string url, string methodname, Dictionary<string, object> args)
        {
            return InvokeWebService(url, null, methodname, args);
        }

        /// <summary>
        /// 动态调用web服务 
        /// </summary>
        /// <param name="url">WSDL服务地址</param>
        /// <param name="classname">类名</param>
        /// <param name="methodname">方法名</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static object InvokeWebService(string url, string classname, string methodname, Dictionary<string,object> args)
        {
            string @namespace = "";
            if ((classname == null) || (classname == ""))
            {
                classname = GetWsClassName(url);
            }
            try
            { //获取WSDL 
                var wc = new WebClient();
                var stream = wc.OpenRead(url + "?WSDL");
                var sd = ServiceDescription.Read(stream);
                var sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                var cn = new CodeNamespace(@namespace);
                //生成客户端代理类代码 
                var ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                var icc = new CSharpCodeProvider();
                //设定编译参数 
                var cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                //编译代理类 
                var cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (cr.Errors.HasErrors)
                {
                    var sb = new StringBuilder();
                    foreach (CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce);
                        sb.Append(Environment.NewLine);
                    }
                    throw new System.Exception(sb.ToString());
                }
                //生成代理实例，并调用方法 
                var assembly = cr.CompiledAssembly;
                var t = assembly.GetType(@namespace + "." + classname, true, true);
                var service = Activator.CreateInstance(t);
                var methodInfo = t.GetMethod(methodname);
                var paramsInfo = methodInfo.GetParameters();
                if(paramsInfo.Length!=args.Count)
                    throw new System.Exception("参数个数不匹配!");
                var obj = new List<object>();
                for (int i = 0; i < paramsInfo.Length; i++)
                {
                    obj.Add(CoralConvert.Convert(args[paramsInfo[i].Name],paramsInfo[i].ParameterType));
                }
                return methodInfo.Invoke(service, obj.ToArray());
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message, new System.Exception(ex.StackTrace));
            }
        }
        private static string GetWsClassName(string wsUrl)
        {
            var parts = wsUrl.Split('/');
            var pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }
    }
}