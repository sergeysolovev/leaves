using System.Collections.Generic;
using System.Linq;

namespace AbcLeaves.BasicMvcClient.DataContracts
{
    public class GetLeavesContract
    {
        public IList<GetLeavesItemContract> Items { get; set; }

        public static GetLeavesContract Empty
            => new GetLeavesContract {
                Items = Enumerable.Empty<GetLeavesItemContract>().ToList()
            };
    }
}
