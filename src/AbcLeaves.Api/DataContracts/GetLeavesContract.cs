using System.Collections.Generic;

namespace AbcLeaves.Api
{
    public class GetLeavesContract
    {
        public IList<GetLeavesItemContract> Items { get; set; }
    }
}
