using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PBL3.EF
{
    public class LoginV
    {
        [Required(ErrorMessage = "Tên tài khoản không được để trống!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        public string Password { get; set; }
    }
}