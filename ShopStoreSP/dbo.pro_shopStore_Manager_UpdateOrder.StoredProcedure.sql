USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_UpdateOrder]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_UpdateOrder]	
	@f_id char(14),
	@f_status tinyint,
	@f_shippingMethod tinyint
AS
BEGIN
	DECLARE @SQL NVARCHAR(100)
	SET @SQL = 'UPDATE t_orders SET '
	
	if @f_status != 0
	BEGIN
		SET @SQL += 'f_status = ' + CONVERT(varchar, @f_status) + ' ,'
	END

	if @f_shippingMethod != 0
	BEGIN
		SET @SQL += 'f_shippingMethod = ' + CONVERT(varchar, @f_shippingMethod) + ' ,'
	END	

	SET @SQL = LEFT(@SQL, LEN(@SQL)-1)

	SET @SQL += 'WHERE f_id = ' + @f_id

	--print @SQL
	EXEC SP_EXECUTESQL @SQL	
END
GO
