USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_editManagerSubMenu]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 編輯後台子選單
    日期: 2021-12-08

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_editManagerSubMenu]
				 @f_id INT ,
				 @f_menuid INT ,
				 @f_name NVARCHAR(20) ,
				 @f_controller VARCHAR(50) ,
				 @f_level INT ,
				 @f_isopen INT ,
				 @f_isdel INT ,
				 @f_issys INT
AS
	BEGIN
		UPDATE t_manager_menuSub WITH(ROWLOCK) SET f_name = @f_name, f_isOpen = @f_isopen WHERE f_id = @f_id			   				  		
	END
GO
