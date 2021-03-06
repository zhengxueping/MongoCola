﻿using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoUtility.Basic;

namespace MongoUtility.Security
{
    public class EachDatabaseUser
    {
        public Dictionary<string, User> UserList =
            new Dictionary<string, User>();

        /// <summary>
        ///     获得数据库角色
        /// </summary>
        /// <param name="DatabaseName"></param>
        /// <returns></returns>
        public BsonArray GetRolesByDBName(string DatabaseName)
        {
            var roles = new BsonArray();
            //当前DB的System.user的角色
            if (UserList.ContainsKey(DatabaseName))
            {
                roles = UserList[DatabaseName].roles;
            }
            //Admin的OtherDBRoles和当前数据库角色合并
            var adminRoles = GetOtherDBRoles(DatabaseName);
            foreach (string item in adminRoles)
            {
                if (!roles.Contains(item))
                {
                    roles.Add(item);
                }
            }
            //ADMIN的ANY系角色的追加
            if (UserList.ContainsKey(ConstMgr.DATABASE_NAME_ADMIN))
            {
                if (UserList[ConstMgr.DATABASE_NAME_ADMIN].roles.Contains(Role.UserRole_dbAdminAnyDatabase))
                {
                    roles.Add(Role.UserRole_dbAdminAnyDatabase);
                }
                if (UserList[ConstMgr.DATABASE_NAME_ADMIN].roles.Contains(Role.UserRole_readAnyDatabase))
                {
                    roles.Add(Role.UserRole_readAnyDatabase);
                }
                if (
                    UserList[ConstMgr.DATABASE_NAME_ADMIN].roles.Contains(
                        Role.UserRole_readWriteAnyDatabase))
                {
                    roles.Add(Role.UserRole_readWriteAnyDatabase);
                }
                if (
                    UserList[ConstMgr.DATABASE_NAME_ADMIN].roles.Contains(
                        Role.UserRole_userAdminAnyDatabase))
                {
                    roles.Add(Role.UserRole_userAdminAnyDatabase);
                }
            }
            return roles;
        }

        /// <summary>
        ///     获得Admin的otherDBRoles
        /// </summary>
        /// <returns></returns>
        public BsonArray GetOtherDBRoles(string DataBaseName)
        {
            var roles = new BsonArray();
            var OtherDBRoles = new BsonDocument();
            if (UserList.ContainsKey(ConstMgr.DATABASE_NAME_ADMIN))
            {
                OtherDBRoles = UserList[ConstMgr.DATABASE_NAME_ADMIN].otherDBRoles;
                if (OtherDBRoles != null && OtherDBRoles.Contains(DataBaseName))
                {
                    roles = OtherDBRoles[DataBaseName].AsBsonArray;
                }
            }
            return roles;
        }

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Username"></param>
        public void AddUser(MongoDatabase db, string Username)
        {
            var userInfo =
                db.GetCollection(ConstMgr.COLLECTION_NAME_USER).FindOneAs<BsonDocument>(Query.EQ("user", Username));
            if (userInfo != null)
            {
                var user = new User();
                user.roles = userInfo["roles"].AsBsonArray;
                if (userInfo.Contains("otherDBRoles"))
                {
                    user.otherDBRoles = userInfo["otherDBRoles"].AsBsonDocument;
                }
                UserList.Add(db.Name, user);
            }
        }

        /// <summary>
        ///     重载ToString方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var UserInfo = string.Empty;
            foreach (var item in UserList.Keys)
            {
                UserInfo += "DataBase:" + item + Environment.NewLine;
                if (UserList[item].roles != null)
                {
                    UserInfo += "Roles:" + UserList[item].roles + Environment.NewLine;
                }
                if (UserList[item].otherDBRoles != null)
                {
                    UserInfo += "otherDBRoles:" + UserList[item].otherDBRoles + Environment.NewLine;
                }
                UserInfo += Environment.NewLine;
            }
            return UserInfo;
        }
    }
}