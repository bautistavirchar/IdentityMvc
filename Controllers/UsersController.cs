using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace IdentityMvc.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public UsersController(RoleManager<ApplicationRole> roleManager, 
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        
        public IActionResult Index() => View(_userManager.Users);
        
        
        [HttpGet("[controller]/[action]/{id}")]
        public async Task<IActionResult> Edit(string id){
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            return View(new UserModel{
                Name = user.Name,
                Email = user.Email
            });
        }
        
        [HttpPost("[controller]/[action]/{id}"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,CanEditUser")]
        public async Task<IActionResult> Edit(string id, UserModel userModel){
            if(!ModelState.IsValid){
                AddModelErrors(ModelState);
                return View(userModel);
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            user.Name = userModel.Name;
            user.UserName = userModel.Email;
            user.Email = userModel.Email;
            var result = await _userManager.UpdateAsync(user);
            if(result.Succeeded){
                return RedirectToAction(nameof(Index));
            }
            AddModelErrors(ModelState);
            return View(userModel);
        }

        public IActionResult Add() => View();
        
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,CanAddUser")]
        public async Task<IActionResult> Add(string id, UserModel userModel){
            if(!ModelState.IsValid){
                AddModelErrors(ModelState);
                return View(userModel);
            }
            var user = new ApplicationUser {
                Name = userModel.Name,
                UserName = userModel.Email,
                Email = userModel.Email,
                ImagePath = Guid.NewGuid() + ".jpg",
                DateCreated = DateTime.Now
            };
            var result = await _userManager.CreateAsync(user,"123456");

            if(result.Succeeded){
                return RedirectToAction(nameof(Index));
            }
            foreach(var error in result.Errors){
                ModelState.AddModelError(string.Empty,error.Description);
            }
            return View(userModel);
        }

        [Authorize(Roles = "Administrator,CanDeleteUser")]
        public async Task<IActionResult> Delete(string id){
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            var result = await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("[controller]/[action]/{id}")]
        public async Task<IActionResult> Roles(string id){
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            var roles = await _userManager.GetRolesAsync(user);
            var ids = new List<string>();
            
            var allRoles = _roleManager.Roles.Where(r => roles.Any(x => x == r.Name));
            foreach(var role in allRoles){
                ids.Add(role.Id);
            }
            var userRoles = new UserRolesModel{
                Roles = ids
            };
            return View(userRoles);
        }

        [HttpPost("[controller]/[action]/{id}"), ValidateAntiForgeryToken]
        
        [Authorize(Roles = "Administrator,CanAddUserRole")]
        public async Task<IActionResult> Roles(string id, UserRolesModel userRolesModel){
            if(!ModelState.IsValid){
                AddModelErrors(ModelState);
                return View(userRolesModel);
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            var roles = await _userManager.GetRolesAsync(user);
            // await _roleManager.DeleteAsync(_roleManager.);
            var removeResult = await _userManager.RemoveFromRolesAsync(user,roles);
            if(removeResult.Succeeded){
                var result = await _userManager.AddToRolesAsync(user,GetSelectedRoles());
                if(result.Succeeded) return RedirectToAction(nameof(Index));
            }
            foreach(var error in removeResult.Errors){
                ModelState.AddModelError(string.Empty,error.Description);
            }
            return View(userRolesModel);

            IEnumerable<string> GetSelectedRoles(){
                var allRoles = _roleManager.Roles.Where(r => userRolesModel.Roles.Any(x => x == r.Id));
                foreach(var r in allRoles){
                    yield return r.Name;
                }
            }
        }

        private void AddModelErrors(ModelStateDictionary models){
            foreach(var err in models){
                foreach(var error in err.Value.Errors){
                    ModelState.AddModelError(string.Empty,error.ErrorMessage);
                }
            }
        }

        // ...
    }
}