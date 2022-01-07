USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_removeOrderList]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_removeOrderList]	
	@f_id char(14)
AS
BEGIN
	UPDATE t_orders
	SET f_isdel = 1, f_status = 4
	WHERE f_id = @f_id
END
GO
