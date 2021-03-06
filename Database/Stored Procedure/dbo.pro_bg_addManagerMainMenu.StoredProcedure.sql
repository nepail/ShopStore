USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_addManagerMainMenu]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 新增後台主選單
    日期: 2021-12-09

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_addManagerMainMenu]
				 @f_id INT ,
				 @f_name NVARCHAR(30) ,
				 @f_icon VARCHAR(30) ,
				 @f_level INT ,
				 @f_isopen INT ,
				 @f_issys INT ,
				 @f_isdel INT
AS
	BEGIN

		IF EXISTS (SELECT TOP 1 1
				FROM t_manager_menu
				WHERE f_id = @f_id
			)
			BEGIN
				UPDATE t_manager_menu
					   SET f_name = @f_name ,
						   f_icon = @f_icon ,
						   f_level = @f_level ,
						   f_isOpen = @f_isopen ,
						   f_isSys = @f_issys ,
						   f_isDel = @f_isdel
				WHERE f_id = @f_id
			END
		ELSE
			BEGIN
				INSERT INTO t_manager_menu (f_name , f_icon , f_level , f_isOpen , f_isSys , f_isDel)
				VALUES (@f_name , @f_icon , @f_level , @f_isopen , @f_issys , @f_isdel)
			END

	END
GO
