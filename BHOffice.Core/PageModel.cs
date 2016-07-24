using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core
{
    [Serializable]
    public class PageModel<TModel>
    {
        public PageModel(IEnumerable<TModel> models, int count)
        {
            this.Models = (models ?? Enumerable.Empty<TModel>()).ToArray();
            this.Count = count;
        }

        public PageModel() : this(null, 0) { }

        public TModel[] Models { get; set; }
        public int Count { get; set; }
    }
}
