using System;
using System.ComponentModel.DataAnnotations;

namespace Fuji.Models
{
    [MetadataType(typeof(AppleMetaData))]
    public partial class Apple
    {

    }

    public class AppleMetaData
    {
        [Required]
        public string VarietyName { get; set; } = null!;

        [Required(ErrorMessage = "Something is required")]
        public string Something { get; set; }
    }
    
}