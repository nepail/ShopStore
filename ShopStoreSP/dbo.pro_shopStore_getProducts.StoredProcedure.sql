USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_getProducts]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[pro_shopStore_getProducts]
	@f_isopen int
AS

if @f_isopen = 1
BEGIN
	SELECT 
	a.f_id
	,a.f_pId
	,a.f_name
	,a.f_price
	--,a.f_picName
	,a.f_description
	,a.f_categoryId
	,c.f_name as categoryName
	,a.f_stock
	,a.f_isopen
	,a.f_isdel	
	,a.f_createtime
	FROM t_products a WITH(NOLOCK)
	--JOIN t_productDetail b WITH(NOLOCK) ON a.f_id = b.f_id
	JOIN t_categories c WITH(NOLOCK) ON c.f_id = a.f_categoryId
	WHERE a.f_isopen = @f_isopen
END
ELSE
BEGIN
	SELECT 
	a.f_id
	,a.f_pId
	,a.f_name
	,a.f_price
	--,a.f_picName
	,a.f_description
	,a.f_categoryId
	,c.f_name as categoryName
	,a.f_stock
	,a.f_isopen
	,a.f_isdel	
	,a.f_createtime
	FROM t_products a WITH(NOLOCK)
	--JOIN t_productDetail b WITH(NOLOCK) ON a.f_id = b.f_id
	JOIN t_categories c WITH(NOLOCK) ON c.f_id = a.f_categoryId
END


--WHERE a.f_isopen = @f_isopen
--AND a.f_isdel = 0
GO
