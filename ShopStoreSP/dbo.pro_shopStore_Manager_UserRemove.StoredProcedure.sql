USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_UserRemove]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_UserRemove]				
	@userId int
AS
BEGIN

	BEGIN TRAN
	SAVE TRAN SavePoint;
	BEGIN TRY
		DELETE FROM t_manager_users WHERE f_id = @userId
		DELETE FROM t_manager_users_permissions WHERE f_userId = @userId
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRAN SavePoint;
		END
	END CATCH
END
GO
