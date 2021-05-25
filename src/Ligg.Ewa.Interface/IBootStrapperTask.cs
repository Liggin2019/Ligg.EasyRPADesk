using System.Collections;
using System.Collections.Generic;

namespace Ligg.EasyWinApp.Interface
{
    public interface IBootStrapperTask
    {
        bool ExecuteThenJudge();
    }

    public class BootStrapperTask
    {
        public IBootStrapperTask Task;
        public string Name;
        public BootStrapperTask(IBootStrapperTask task,string name)
        {
            Task = task;
            Name = name;
        }


    }
}