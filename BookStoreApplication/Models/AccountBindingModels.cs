using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BookStoreApplication.Models
{
    // Модель для регистрации нового пользователя 
    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    } 

    // Модель для добавления нового пользователя администратором
    public class NewUserBindingModel: RegisterBindingModel
    {
       public string[] Roles { get; set; }
    }
}
