using System;
using System.ComponentModel.DataAnnotations;

namespace bst.Model
{
    public class Lab
    {
        [Key]
        public Guid LabID { get; set; }
        public string name { get; set; }
    }
}
