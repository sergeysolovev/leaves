using System.Collections.Generic;

namespace Leaves.Api
{
    public class GetLeavesContract
    {
        public IList<GetLeavesItemContract> Items { get; set; }
    }
}
