using System.Reflection;
using VSLibrary.Common.MVVM.Core;

namespace ChamberControl
{
    public class ChamberModule
    {
        public void RegisterServices(VSContainer services)
        {
            services.AutoInitialize(Assembly.GetExecutingAssembly());
            //UIInitializer.RegisterServices(services);
        }
    }
}
