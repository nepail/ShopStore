USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_updateSubMenu]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_updateSubMenu]	
	@f_id int,
	@f_menuid int,
	@f_name nvarchar(20),
	@f_controller varchar(50),
	@f_level int,
	@f_isopen int,
	@f_isdel int,
	@f_issys int
AS
BEGIN
	UPDATE t_manager_menusub 
	SET	f_name = @f_name, f_isopen = @f_isopen
	WHERE f_id = @f_id
END
GO
