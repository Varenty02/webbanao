using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Required(ErrorMessage ="Phải nhập {0}")]
        [Display(Name ="Họ tên")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Phải nhập {0}")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage ="Phải là địa chỉ email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public DateTime DateSent { get; set; }
        [Display(Name = "Nội dung")]
        public string Message { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Phải nhập {0}")]
        [Phone(ErrorMessage = "Phải là số điện thoại")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }
    }
}
