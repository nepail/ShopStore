USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_getUsersPermissions]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_getUsersPermissions]				
	@userId int
AS
BEGIN

--SELECT	
--	a.f_groupId,
--	b.f_name AS MenuSubName,
--	a.f_permissionCode AS PermissionCode
--FROM t_manager_users_permissions AS a WITH(NOLOCK)
--LEFT JOIN t_manager_group_permissions AS c ON a.f_groupId = c.f_id
--INNER JOIN t_manager_menuSub AS b WITH(NOLOCK) ON a.f_menuSubId = b.f_id
--WHERE a.f_userId = @userId

	DECLARE @groupID int

	SET @groupID =  (SELECT TOP 1 f_groupId FROM t_manager_users WITH(NOLOCK) WHERE f_id = @userId);

	SELECT			
		menu.f_id AS MenuId,
		menu.f_name AS MenuName,		
		ISNULL(users.f_permissionCode, 0) AS PermissionCode
	FROM (SELECT f_id, f_groupId, f_menuSubId FROM t_manager_group_permissions WITH(NOLOCK) WHERE f_groupId = @groupID) a
	LEFT JOIN t_manager_users_permissions users WITH(NOLOCK) ON a.f_menuSubId = users.f_menuSubId AND users.f_userId = @userId
	INNER JOIN t_manager_menuSub menu WITH(NOLOCK) ON a.f_menuSubId = menu.f_id
	

END
GO
