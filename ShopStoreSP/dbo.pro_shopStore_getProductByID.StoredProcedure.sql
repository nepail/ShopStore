USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_getProductByID]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[pro_shopStore_getProductByID]
@ID char(36)
AS
SELECT 

TA.f_id,
TA.f_name,
TA.f_price,
TA.f_categoryId,
TA.f_description,
TA.f_picName,
TB.f_content

FROM [ShoppingDB].[dbo].[t_products] TA WITH(NOLOCK)
LEFT JOIN [ShoppingDB].[dbo].[t_productDetail] TB WITH(NOLOCK)on TA.f_id = TB.f_id
WHERE TA.f_id = @ID
AND TA.f_isopen = 1
AND TA.f_isdel = 0
AND TA.f_stock > 0
GO
