using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EasyBilling.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Телефонный номер")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Логин")]
            public string UserName { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                UserName = userName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Не могу загрузить аккаунт с ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Не могу загрузить аккаунт с ID '{_userManager.GetUserId(User)}'.");
            }

            var checkUniqUserName = await _userManager.FindByNameAsync(Input.UserName);
            if (checkUniqUserName != null)
            {
                ModelState.AddModelError("Input.UserName", "Такой логин уже есть измените на другой.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userName = await _userManager.GetUserNameAsync(user);
            if (Input.UserName != userName)
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.UserName);
                if (!setUserNameResult.Succeeded)
                {
                    StatusMessage = "Неизвестная ошибка, повторите ввод логина.";
                    return RedirectToPage();
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Неизвестная ошибка, повторите ввод номера телефона.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Ваш профиль успешно обновлён";
            return RedirectToPage();
        }
    }
}