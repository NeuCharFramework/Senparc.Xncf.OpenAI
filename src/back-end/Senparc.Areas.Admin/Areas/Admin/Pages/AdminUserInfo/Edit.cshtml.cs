using Microsoft.AspNetCore.Mvc;
using Senparc.Areas.Admin.Domain;
using Senparc.CO2NET.Extensions;
using Senparc.Ncf.AreaBase.Admin.Filters;
using Senparc.Ncf.Core.Enums;
using Senparc.Ncf.Core.Models;
using Senparc.Ncf.Core.Validator;
using System;

namespace Senparc.Areas.Admin.Areas.Admin.Pages
{
    //[IgnoreAntiforgeryToken]
    public class AdminUserInfo_EditModel : BaseAdminPageModel, IValidatorEnvironment
    {
        /// <summary>
        /// Id
        /// </summary>
        //[BindProperty]
        public int Id { get; set; }

        public bool IsEdit { get; set; }


        //[BindProperty]
        public CreateOrUpdate_AdminUserInfoDto AdminUserInfo { get; set; } = new CreateOrUpdate_AdminUserInfoDto();
        //public CreateUpdate_AdminUserInfoDto AdminUserInfo { get; set; }

        private readonly IServiceProvider _serviceProvider;
        private readonly AdminUserInfoService _adminUserInfoService;

        public AdminUserInfo_EditModel(IServiceProvider serviceProvider, AdminUserInfoService adminUserInfoService)
        {
            _serviceProvider = serviceProvider;
            _adminUserInfoService = adminUserInfoService;
        }

        public IActionResult OnGet(int id)
        {
            return Page();
        }

        /// <summary>
        /// Handler=Detail
        /// ��ȡ����
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CustomerResource("admin-edit")]
        public IActionResult OnGetDetail(int id)
        {
            var adminUserInfo = _adminUserInfoService.GetAdminUserInfo(id);
            return Ok(adminUserInfo ?? new CreateOrUpdate_AdminUserInfoDto());
        }

        /// <summary>
        /// Handler=Save
        /// ����
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [CustomerResource("admin-add", "admin-edit")]
        public IActionResult OnPostSave([FromBody] CreateOrUpdate_AdminUserInfoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(false);
            }
            bool isExists = this._adminUserInfoService.CheckUserNameExisted(dto.Id, dto.UserName);
            if (isExists)
            {
                return Ok(false);
            }
            if (dto.Id > 0)
            {
                //dto.Id = Id;
                _adminUserInfoService.UpdateAdminUserInfo(dto);
            }
            else
            {
                _adminUserInfoService.CreateAdminUserInfo(dto);
            }
            return Ok(true);
        }

        public IActionResult OnPost()
        {
            IsEdit = Id > 0;
            this.Validator(AdminUserInfo.UserName, "�û���", "UserName", false)
                .IsFalse(z => this._adminUserInfoService.CheckUserNameExisted(Id, z), "�û����Ѵ��ڣ�", true);
            if (!IsEdit)
            {

                this.Validator(AdminUserInfo.Password, "����", "Password", false).MinLength(6);
            }
            else
            {
                if (!AdminUserInfo.Password.IsNullOrEmpty())
                {
                    this.Validator(AdminUserInfo.Password, "����", "Password", false).MinLength(6);
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (IsEdit)
            {
                AdminUserInfo.Id = Id;
                _adminUserInfoService.UpdateAdminUserInfo(AdminUserInfo);
            }
            else
            {
                _adminUserInfoService.CreateAdminUserInfo(AdminUserInfo);
            }

            base.SetMessager(MessageType.success, $"{(IsEdit ? "�޸�" : "����")}�ɹ���");
            return RedirectToPage("./Index");
        }
    }
}