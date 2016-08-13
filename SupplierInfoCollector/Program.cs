using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupplierInfoCollector.Task;
using SupplierInfoCollector.TaskFramework;

namespace SupplierInfoCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskExecutor taskExecutor = new TaskExecutor();
            taskExecutor.Start();

            FetchSupplierListTask task1 = new FetchSupplierListTask();
            taskExecutor.AddTask(task1);

            FetchSupplierInfoTask task2 = new FetchSupplierInfoTask();
            taskExecutor.AddTask(task2);

            //CleanSupplierInfoDataTask task3 = new CleanSupplierInfoDataTask();
            //taskExecutor.AddTask(task3);

            //FetchAlibabaProductTypeTask task5 = new FetchAlibabaProductTypeTask();
            //taskExecutor.AddTask(task5);

            //FetchAlibabaSuppplierListTask task4 = new FetchAlibabaSuppplierListTask();
            //taskExecutor.AddTask(task4);

            Console.ReadLine();
        }
    }
}
