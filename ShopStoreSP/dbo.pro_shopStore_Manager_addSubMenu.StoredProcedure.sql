USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_addSubMenu]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_addSubMenu]	
	@f_id int,
	@f_menuid int,
	@f_name nvarchar(30),
	@f_controller varchar(50),
	@f_isopen int,
	@f_level int
	
AS
BEGIN
	INSERT INTO t_manager_menusub
	(f_menuid, f_name, f_controller, f_isopen, f_level, f_isdel)
	VALUES
	(@f_menuid, @f_name, @f_controller, @f_isopen, @f_level, 0)
END
GO
