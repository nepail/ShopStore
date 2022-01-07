USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_UserPermissionsUpdate]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_UserPermissionsUpdate]	
	@f_userId int,
	@f_menuSubId int,
	@f_permissionCode int,
	@f_updateTime DateTime,
	@f_groupId int,
	@UpdateType bit
AS
BEGIN	
	BEGIN TRANSACTION;
	SAVE TRANSACTION MySavePoint;
	BEGIN TRY
	
	IF @UpdateType = 1
		BEGIN
			DECLARE @DataNum int
			SELECT @DataNum = COUNT(*) FROM t_manager_users_permissions WITH(NOLOCK) WHERE f_userId = @f_userId AND f_menuSubId = @f_menuSubId
						
			IF @DataNum > 0
				BEGIN
					UPDATE t_manager_users_permissions SET f_permissionCode = @f_permissionCode	WHERE f_userId = @f_userId AND f_menuSubId = @f_menuSubId											
				END
			ELSE
				BEGIN
					INSERT t_manager_users_permissions(f_userId, f_menuSubId, f_permissionCode)VALUES(@f_userId, @f_menuSubId, @f_permissionCode)															
				END

			IF @f_groupId > 0
			BEGIN
				UPDATE t_manager_users SET f_groupId = @f_groupId WHERE f_id = @f_userId								
			END
		END
	ELSE
		BEGIN
			UPDATE t_manager_users SET f_groupId = @f_groupId WHERE f_id = @f_userId						
		END

			UPDATE t_manager_users SET f_updateTime = @f_updateTime WHERE f_id = @f_userId						
			COMMIT TRANSACTION
	END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				BEGIN					
					ROLLBACK TRANSACTION MySavePoint;
				END
		END CATCH
END;
GO
