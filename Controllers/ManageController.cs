using System.Linq;
using System.Threading.Tasks;
using IdentityMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityMvc.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public ManageController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index(){
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet("[controller]/[action]/{id}")]
        public async Task<IActionResult> Edit(string id){
            var role = await _roleManager.FindByIdAsync(id);
            return View(new RoleModel{
                Name = role.Name,
                Description = role.Description
            });
        }

        [HttpPost("[controller]/[action]/{id}"), ValidateAntiForgeryToken]
        [Authorize(Roles="Administrator,CanEditRole")]
        public async Task<IActionResult> Edit(string id, RoleModel roleModel){
            if(!ModelState.IsValid){
                AddModelErrors(ModelState);
                return View(roleModel);
            }
            var role = await _roleManager.FindByIdAsync(id);
            role.Name = roleModel.Name;
            role.Description = roleModel.Description;
            await _roleManager.UpdateAsync(role);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Status() => View();

        [Authorize(Roles="Administrator,CanDeleteRole")]
        public async Task<IActionResult> Delete(string id){
            var role = await _roleManager.FindByIdAsync(id);
            var result = await _roleManager.DeleteAsync(role);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Add() => View();

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles="Administrator,CanAddRole")]
        public async Task<IActionResult> Add(RoleModel roleModel){
            if(!ModelState.IsValid){
                AddModelErrors(ModelState);
                return View(roleModel);
            }
            var adminRole = new ApplicationRole {
                Name = roleModel.Name,
                Description = roleModel.Description
            };
            await _roleManager.CreateAsync(adminRole);
            return RedirectToAction(nameof(Index));
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