USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_updateProduct]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[pro_shopStore_updateProduct]
	@f_pId	char(36),
	@f_name	nvarchar(20),
	@f_price	int,	
	@f_description	nvarchar(250),
	@f_categoryId	int,
	@f_stock	int,
	@f_isdel	bit,
	@f_isopen	bit,	
	@f_updatetime datetime
AS
BEGIN

	UPDATE t_products 
	SET f_name = @f_name,
	f_price = @f_price,
	f_description = @f_description,
	f_categoryId = @f_categoryId,
	f_stock = @f_stock,
	f_isdel = @f_isdel,
	f_isopen = @f_isopen,
	f_updatetime = @f_updatetime

	WHERE f_pId = @f_pId

END
GO
