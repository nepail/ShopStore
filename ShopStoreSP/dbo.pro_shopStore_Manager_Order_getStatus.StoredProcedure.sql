USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_Order_getStatus]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_Order_getStatus]	
AS
BEGIN
	SELECT 
		'030600' + CAST(f_id AS VARCHAR) AS Code,
		f_name AS Name,
		'bg-' + f_badge AS Style

	FROM t_orderStatus

	SELECT 
		'030610' + CAST(f_id AS VARCHAR) AS Code,
		f_name AS Name,
		f_badge AS Style

	FROM t_orderShipping
END
GO
