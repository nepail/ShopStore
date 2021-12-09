using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopStore.Common
{
    public class BaseLog
    {
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 紀錄Log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logType"></param>
        //public void SetLogger(string message, LogType logType)
        //{
        //    switch (logType)
        //    {
        //        case LogType.Trace:
        //            logger.Trace(message);
        //            break;
        //        case LogType.Debug:
        //            logger.Debug(message);
        //            break;
        //        case LogType.Info:
        //            logger.Info(message);
        //            break;
        //        case LogType.Warn:
        //            logger.Warn(message);
        //            break;
        //        case LogType.Error:
        //            logger.Error(message);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        ///// <summary>
        ///// 撰寫追蹤LOG為主
        ///// </summary>
        ///// <param name="objLogInfo"></param>
        ///// <param name="ex"></param>
        //public void WriteTraceLog(string strMessage)
        //{
        //    SetLogger(strMessage, LogType.Trace);
        //}

        ///// <summary>
        ///// 撰寫SQLLOG為主
        ///// </summary>
        ///// <param name="objLogInfo"></param>
        ///// <param name="ex"></param>
        //public void WriteSqlLog(LogInfoModel objLogInfo)
        //{
        //    objLogInfo.Type = LogType.Info;
        //    SetLogger(FormatLog(objLogInfo), LogType.Debug);
        //}

        ///// <summary>
        ///// 撰寫錯誤LOG為主
        ///// </summary>
        ///// <param name="objLogInfo"></param>
        ///// <param name="ex"></param>
        //public void WriteErrorLog(LogInfoModel objLogInfo, string ex)
        //{
        //    objLogInfo.Type = LogType.Error;
        //    objLogInfo.Exception = ex;
        //    SetLogger(FormatLog(objLogInfo), LogType.Error);
        //}

        ///// <summary>
        ///// 撰寫登入LOG為主
        ///// </summary>
        ///// <param name="objLogInfo"></param>
        ///// <param name="ex"></param>
        //public void WriteLoginTime(EmpUserModel loginUser, string ip)
        //{
        //    StringBuilder strLog = new StringBuilder();
        //    strLog.AppendLine("[Login]" + loginUser.USER_ID);
        //    strLog.AppendLine("[IP]" + ip);

        //    SetLogger(strLog.ToString(), LogType.Info);
        //}

        ///// <summary>
        ///// 撰寫後台管理者登入LOG
        ///// </summary>
        ///// <param name="objLogInfo"></param>
        ///// <param name="ex"></param>
        //public void WriteAdminLoginInfo(string userID, string ip)
        //{
        //    StringBuilder strLog = new StringBuilder();
        //    strLog.AppendLine("[Login]" + userID);
        //    strLog.AppendLine("[IP]" + ip);

        //    SetLogger(strLog.ToString(), LogType.Warn);
        //}

        ///// <summary>
        ///// 格式化LOG訊息
        ///// </summary>
        ///// <param name="objLogInfo">LOG訊息物件</param>
        ///// <returns>格式化後的LOG訊息</returns>
        //public string FormatLog(LogInfoModel objLogInfo)
        //{
        //    StringBuilder logInfo = new StringBuilder();

        //    //依造domain object定義順序依序寫出裡面內容
        //    foreach (System.Reflection.PropertyInfo objItem in objLogInfo.GetType().GetProperties())
        //    {
        //        if (objItem.GetValue(objLogInfo, null) != null)
        //        {
        //            logInfo.AppendLine("[" + objItem.Name + "]");
        //            logInfo.AppendLine(objItem.GetValue(objLogInfo, null).ToString());
        //            logInfo.AppendLine("");
        //        }
        //    }

        //    return logInfo.ToString();
        //}
    }

}
