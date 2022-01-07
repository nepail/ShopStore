USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_getOrderList]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_getOrderList]	
AS
BEGIN
	SELECT 
	a.f_id AS Num,
	a.f_memberid AS MemberId,
	b.f_account AS MemberAccount,
	a.f_orderTime AS _date,
	c.f_name AS Status,
	c.f_badge AS StatusBadge,
	d.f_name AS ShippingMethod,
	d.f_badge AS ShippingBadge,
	a.f_total AS Total,
	a.f_ispaid AS IsPaid,
	a.f_isdel AS IsDel

	FROM t_orders AS a
	JOIN t_members AS b ON a.f_memberid = b.f_id
	JOIN t_orderStatus AS c ON a.f_status = c.f_id
	JOIN t_orderShipping AS d ON a.f_shippingmethod = d.f_id

	
END
GO
