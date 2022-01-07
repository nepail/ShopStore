USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_addProduct]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[pro_shopStore_addProduct]
	@f_id int,
	@f_pId	char(36),
	@f_name	nvarchar(10),
	@f_price	int,
	@f_picName	varchar(250),
	@f_description	nvarchar(250),
	@f_content varchar(20),
	@f_categoryId	int,
	@f_stock	int,
	@f_isDel	bit,
	@f_isOpen	bit,	
	@f_updateTime datetime,
	@f_createTime datetime
AS
BEGIN
	--begin TRANSACTION;  --開始交易
	--begin TRY
		INSERT INTO [ShoppingDB].[dbo].[t_products]
		(f_pId
		,f_name
		,f_price
		--,f_picName
		,f_description
		,f_categoryId
		,f_stock
		,f_isDel
		,f_isOpen
		,f_createTime
		) 
		VALUES 
		(@f_pId
		,@f_name
		,@f_price
		--,@f_picName
		,@f_description
		,@f_categoryId
		,@f_stock
		,@f_isDel
		,@f_isOpen
		,@f_createTime
		)
	--end TRY
	--begin CATCH
	--	ROLLBACK TRANSACTION;
	--end CATCH
	--COMMIT TRANSACTION;
END
GO
