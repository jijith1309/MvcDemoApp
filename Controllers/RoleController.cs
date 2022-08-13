using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyDemoApp.Data;
using MyDemoApp.Models;

namespace MyDemoApp.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly DemoAppContext _context;

        public RoleController(DemoAppContext context)
        {
            _context = context;
        }


        // GET: Role
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.FromSqlInterpolated($"exec GetAllRoles").ToListAsync();
            return View(roles);
        }

        // GET: Role/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }
            RolePermissionViewModel model = new RolePermissionViewModel();
            var role = await _context.Roles.Where(r => r.RoleId == id).FirstOrDefaultAsync();
            var rolepermissions = await _context.RolePermissions.Include(r => r.Module).Where(r => r.RoleId == id).ToListAsync();

            model.RoleName = role.RoleName;
            model.RoleId = role.RoleId;
            if (rolepermissions != null && rolepermissions.Count > 0)
            {
                model.ModulePermissionList = rolepermissions.Select(s => new ModulePermission
                {
                    Add = s.Add,
                    Delete = s.Delete,
                    Edit = s.Edit,
                    View = s.View,
                    Module = s.Module.ModuleName

                }).ToList();
            }

            if (role == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            var modules = _context.Modules.Select(m => new ModulePermission { ModuleId = m.ModuleId, Module = m.ModuleName }).ToList();
            RolePermissionViewModel model = new RolePermissionViewModel
            {
                ModulePermissionList = modules
            };
            return View(model);
        }

        // POST: Role/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolePermissionViewModel role)
        {
            if (ModelState.IsValid)
            {
                var checkRoleExists = _context.Roles.Where(r => r.RoleName == role.RoleName).FirstOrDefault();
                if (checkRoleExists == null)
                {
                    Role newRole = new Role { RoleName = role.RoleName };
                    _context.Add(newRole);

                    List<RolePermission> rolePermissions = new List<RolePermission>();
                    foreach (var rolePermission in role.ModulePermissionList.Where(i => i.IsSelected).ToList())
                    {
                        RolePermission newItem = new RolePermission
                        {
                            Role = newRole,
                            ModuleId = rolePermission.ModuleId,
                            View = rolePermission.View,
                            Add = rolePermission.Add,
                            Edit = rolePermission.Edit,
                            Delete = rolePermission.Delete
                        };
                        rolePermissions.Add(newItem);
                    }
                    _context.AddRange(rolePermissions);
                    await _context.SaveChangesAsync();
                }


                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            RolePermissionViewModel model = new RolePermissionViewModel();
            var modules = _context.Modules.Select(m => new ModulePermission { ModuleId = m.ModuleId, Module = m.ModuleName }).ToList();
            var rolepermissions = await _context.RolePermissions.Include(r => r.Module).Where(r => r.RoleId == id).ToListAsync();
            model.RoleName = role.RoleName;
            model.RoleId = role.RoleId;
            foreach (var module in modules)
            {
                if (rolepermissions != null && rolepermissions.Count > 0)
                {
                    var rp = rolepermissions.Where(rpp => rpp.Module.ModuleId == module.ModuleId).FirstOrDefault();
                    if (rp != null)
                    {
                        module.ModuleId = rp.ModuleId;
                        module.Module = rp.Module.ModuleName;
                        module.IsSelected = true;
                        module.Add = rp.Add;
                        module.View = rp.View;
                        module.Delete = rp.Delete;
                        module.Edit = rp.Edit;
                    }
                }
            }
            model.ModulePermissionList = modules;
            //if (rolepermissions != null && rolepermissions.Count > 0)
            //{
            //    model.ModulePermissionList = rolepermissions.Select(s => new ModulePermission
            //    {
            //        Add = s.Add,
            //        Delete = s.Delete,
            //        Edit = s.Edit,
            //        View = s.View,
            //        Module = s.Module.ModuleName,
            //        IsSelected=true

            //    }).ToList();
            //}
            return View(model);
        }

        // POST: Role/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RolePermissionViewModel role)
        {
            if (ModelState.IsValid)
            {
                var checkRoleExists = _context.Roles.Where(r => r.RoleName == role.RoleName && r.RoleId != role.RoleId).FirstOrDefault();
                if (checkRoleExists == null)
                {
                    var existingRole = _context.Roles.Where(r => r.RoleId == role.RoleId).FirstOrDefault();
                    existingRole.RoleName = role.RoleName;
                    _context.Roles.Update(existingRole);

                    var rp = _context.RolePermissions.Where(r => r.RoleId == role.RoleId).ToList();
                    if (rp != null && rp.Count > 0)
                    {
                        _context.RemoveRange(rp);
                    }
                    List<RolePermission> rolePermissions = new List<RolePermission>();
                    foreach (var rolePermission in role.ModulePermissionList.Where(i => i.IsSelected).ToList())
                    {
                        RolePermission newItem = new RolePermission
                        {
                            Role = existingRole,
                            ModuleId = rolePermission.ModuleId,
                            View = rolePermission.View,
                            Add = rolePermission.Add,
                            Edit = rolePermission.Edit,
                            Delete = rolePermission.Delete
                        };
                        rolePermissions.Add(newItem);
                    }
                    _context.AddRange(rolePermissions);
                    await _context.SaveChangesAsync();
                }


                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Roles == null)
            {
                return Problem("Entity set 'DemoAppContext.Roles'  is null.");
            }
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.RoleId == id);
        }
    }
}
