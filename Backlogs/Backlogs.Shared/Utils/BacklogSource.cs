using Backlogs.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Toolkit.Collections;
using System.Linq;

namespace Backlogs.Utils.Uno
{
    public class BacklogSource : IIncrementalSource<Backlog>
    {
        private ObservableCollection<Backlog> m_backlogs;
        public BacklogSource(ObservableCollection<Backlog> backlogs)
        {
            m_backlogs = backlogs;
        }

        public async Task<IEnumerable<Backlog>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            var result = (from b in m_backlogs select b).Skip(pageIndex * pageSize).Take(pageSize);
            await Task.Delay(100);
            return result;
        }
    }
}
