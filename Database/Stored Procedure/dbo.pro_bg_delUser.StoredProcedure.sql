USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_delUser]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 刪除後台用戶
    日期: 2021-12-29

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_delUser]
				 @userId INT
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
