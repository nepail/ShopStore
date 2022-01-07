USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_addMainMenu]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_addMainMenu]			
	@f_id int,
	@f_name nvarchar(30),
	@f_icon varchar(30),
	@f_level int,
	@f_isopen int,
	@f_issys int,
	@f_isdel int	
AS
BEGIN
	IF EXISTS(select top 1 1 from t_manager_menu WHERE f_id = @f_id)
	BEGIN	
		UPDATE t_manager_menu 
		SET f_name = @f_name, f_icon = @f_icon, f_level = @f_level, f_isopen = @f_isopen, f_issys = @f_issys, f_isdel = @f_isdel
		WHERE f_id = @f_id
	END
	ELSE
	BEGIN
		INSERT INTO t_manager_menu
		(f_name, f_icon, f_level, f_isopen, f_issys, f_isdel)
		VALUES
		(@f_name, @f_icon, @f_level, @f_isopen, @f_issys, @f_isdel)
	END
END
GO
