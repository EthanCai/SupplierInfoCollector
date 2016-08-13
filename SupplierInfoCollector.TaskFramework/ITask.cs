using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SupplierInfoCollector.TaskFramework
{
    public interface ITask
    {
        TastExecuteResult Execute();
    }

    public class TastExecuteResult
    {
        /// <summary>
        /// 0：成功
        /// 其它数字：失败
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 结果消息
        /// </summary>
        public string Message { get; set; }
    }
}
