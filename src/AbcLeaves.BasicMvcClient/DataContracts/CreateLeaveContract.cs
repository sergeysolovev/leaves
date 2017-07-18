using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AbcLeaves.BasicMvcClient.DataContracts
{
    public class CreateLeaveContract
    {
        [Required, DataType(DataType.DateTime)]
        public DateTime Start { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime End { get; set; }
    }
}
