using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC5Exercise2.Models.ViewModels
{
    public class 客戶聯絡人BatchUpdate
    {
        public int Id { get; set; }
        [Required]
        public string 職稱 { get; set; }
        [Required]
        public string 手機 { get; set; }
        [Required]
        public string 電話 { get; set; }
    }
}