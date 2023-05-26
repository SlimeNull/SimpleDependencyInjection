using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDependencyInjection
{
    public class LazyService<TService> where TService : class
    {
        public LazyService(IServiceProvider serviceProvider)
        {
            laziedService = new Lazy<TService>(() => serviceProvider.GetService<TService>());
        }

        private Lazy<TService> laziedService;
        public TService Service => laziedService.Value;
    }
}
