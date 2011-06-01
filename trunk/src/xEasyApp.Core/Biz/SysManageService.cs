﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xEasyApp.Core.Interfaces;
using xEasyApp.Core.Repositories;
using xEasyApp.Core.Exceptions;
using xEasyApp.Core.Configurations;
using xEasyApp.Core.Entities;
using System.Transactions;

namespace xEasyApp.Core.Biz
{
    public class SysManageService : ISysManageService
    {
        public SysManageService()
        {
            _roleRepository=new RoleInfoRepository();
            _deptRepository =new DepartmentRepository();
            _userRepository = new UserInfoRepository();
        }
        private RoleInfoRepository _roleRepository;
        private DepartmentRepository _deptRepository;
        private UserInfoRepository _userRepository;
        public List<RoleInfo> QueryRoleList()
        {
            return _roleRepository.QueryAll();
        }

        public void SaveRoleInfo(RoleInfo ri)
        {
            if (ri.IsNew)
            {
               bool IsValid =  _roleRepository.ValidRoleCode(ri.RoleCode);
               if (!IsValid)
               {
                  throw new BizException("角色代码必须唯一");
               }
            }
            _roleRepository.Save(ri);
        }
        public RoleInfo GetRoleInfo(string roleCode)
        {
           return _roleRepository.Get(roleCode);
        }
       
        public int DeleteRoleInfo(string roleCode)
        {
            RoleInfo ri = _roleRepository.Get(roleCode);
            if (ri == null)
            {
                return 0;
            }
            if (ri.IsSystem.HasValue && ri.IsSystem.Value)
            {
                throw new BizException("系统角色不允许被删除");
            }
            return _roleRepository.Delete(roleCode);
        }


        public bool ValidRoleCode(string roleCode)
        {
            return _roleRepository.ValidRoleCode(roleCode);
        }


        public List<Department> QueryDepartmentList()
        {
           return _deptRepository.QueryAll();
        }

        public Department GetDeptInfo(string deptCode)
        {
            Department d= _deptRepository.GetDepartment(deptCode);
            if (d.ParentCode == AppConfig.RootDeptCode)
            {
                d.ParentName = AppConfig.RootDeptName;
            }
            return d;
        }
        public Department GetRootDepartment()
        {
            Department dept = new Department();
            dept.DeptCode = AppConfig.RootDeptCode;
            dept.DeptName = AppConfig.RootDeptName;
            return dept;
        }

        public List<Department> GetChildDeptsByParentCode(string parentCode)
        {
            return _deptRepository.GetChildDeptsByParentCode(parentCode);
        }

        public void SaveDeptInfo(Department department)
        {
            _deptRepository.SaveDeptInfo(department);
        }

        public bool ValidDeptCode(string deptCode)
        {
            return _deptRepository.ValidDeptCode(deptCode);
        }
        public PagedList<UserInfo> QueryDeptUserList(PageView view, string deptCode)
        { 
            return _deptRepository.QueryDeptUserList(view,deptCode);
        }
        public int DeleteDeptInfo(string id)
        {
            int ret = -1;
            try
            {              
               ret = _deptRepository.DeleteDeptInfo(id);
              
            }
            catch (Exception ex)
            {
                throw new BizException(ex.Message, ex);
            }
            return ret;

        }
        public UserInfo GetUserInfo(string UserUID)
        {
            return _userRepository.Get(UserUID);
        }

        public bool ValidUserUID(string UserUID)
        {
            return _userRepository.ValidUserUID(UserUID);
        }

        public int DeleteUserInfo(string id)
        {
            int ret = -1;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ret = _userRepository.DeleteUserInfo(id);
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new BizException(ex.Message, ex);
            }
            return ret;
        }

        public void SaveUserInfo(UserInfo user)
        {
            _userRepository.Save(user);
        }

        
    }
}