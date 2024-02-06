using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webbanao.Models
{
    
    public class ContactModel
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName="nvarchar")]
        [StringLength(50)]
        [Required]
        public string FullName { get; set; }
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        public DateTime DateSent { get; set; }
        public string Message { get; set; }
        [StringLength(50)]
        public string Phone { get; set; }
    }
}
