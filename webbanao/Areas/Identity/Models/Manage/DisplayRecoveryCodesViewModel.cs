using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webbanao.Areas.Identity.Models.Manage
{
    public class DisplayRecoveryCodesViewModel
    {
        [Required]
        public IEnumerable<string> Codes { get; set; }

    }
}
