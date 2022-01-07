USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_addMenu]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_addMenu]	
	@f_name	nvarchar(10)
	
AS
BEGIN
	INSERT INTO t_manager_menu
	(f_name, f_isdel)
	VALUES
	(@f_name, 0)
END
GO
