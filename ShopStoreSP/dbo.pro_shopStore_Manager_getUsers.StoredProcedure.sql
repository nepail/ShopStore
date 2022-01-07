USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_getUsers]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_getUsers]				
	
AS
BEGIN

SELECT 
	a.f_id AS ID,
	a.f_account AS Account,
	a.f_name AS Name,
	b.f_id AS GroupId,
	b.f_name AS GroupName,
	a.f_createTime,
	a.f_updateTime
FROM t_manager_users AS a WITH(NOLOCK)
JOIN t_manager_group AS b WITH(NOLOCK) ON a.f_groupId = b.f_id

--SELECT
--	a.f_groupId,
--	b.f_name AS MenuName,
--	a.f_permissionCode AS PermissionCode
--FROM t_manager_permissions AS a WITH(NOLOCK)
--JOIN t_manager_menusub AS b WITH(NOLOCK) ON a.f_menuId = b.f_id

END
GO
