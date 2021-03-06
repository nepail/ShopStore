USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_addManagerSubMenu]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 新增後台子選單
    日期: 2021-12-08

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_addManagerSubMenu]
				 @f_id INT ,
				 @f_menuid INT ,
				 @f_name NVARCHAR(30) ,
				 @f_controller VARCHAR(50) ,
				 @f_isopen INT ,
				 @f_level INT

AS
	BEGIN
		INSERT INTO t_manager_menuSub (f_menuid , f_name , f_controller , f_isOpen , f_level , f_isDel)
		VALUES (@f_menuid , @f_name , @f_controller , @f_isopen , @f_level , 0)
	END
GO
