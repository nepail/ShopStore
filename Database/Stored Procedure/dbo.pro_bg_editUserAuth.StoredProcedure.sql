USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_editUserAuth]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 更新用戶權限
    日期: 2021-12-28

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_editUserAuth]
				 @f_userId INT ,
				 @f_menuSubId INT ,
				 @f_permissionCode INT ,
				 @f_updateTime DATETIME ,
				 @f_groupId INT ,
				 @UpdateType BIT
AS
	BEGIN
		BEGIN TRANSACTION;
		SAVE TRANSACTION MySavePoint;

		BEGIN TRY

			IF @UpdateType = 1
				BEGIN
					DECLARE @DataNum INT
					SELECT @DataNum = COUNT(*)
					FROM t_manager_users_permissions WITH (NOLOCK)
					WHERE f_userId = @f_userId
						  AND
						  f_menuSubId = @f_menuSubId

					IF @DataNum > 0
						BEGIN
							UPDATE t_manager_users_permissions WITH (ROWLOCK)
								   SET f_permissionCode = @f_permissionCode
							WHERE f_userId = @f_userId
								   AND
								   f_menuSubId = @f_menuSubId
						END
					ELSE
						BEGIN
							INSERT t_manager_users_permissions (f_userId , f_menuSubId , f_permissionCode)
							VALUES (@f_userId , @f_menuSubId , @f_permissionCode)
						END

					IF @f_groupId > 0
						BEGIN
							UPDATE t_manager_users WITH (ROWLOCK)
								   SET f_groupId = @f_groupId
							WHERE f_id = @f_userId
						END

				END
			ELSE
				BEGIN
					UPDATE t_manager_users WITH (ROWLOCK)
						   SET f_groupId = @f_groupId
					WHERE f_id = @f_userId
				END

			UPDATE t_manager_users WITH (ROWLOCK)
				   SET f_updateTime = @f_updateTime
			WHERE f_id = @f_userId
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
