USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_getOrders]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 後台取得訂單資料
    日期: 2021-12-15

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_getOrders]
AS
	BEGIN
		SELECT a.f_id AS Num , a.f_memberId AS MemberId , b.f_account AS MemberAccount , a.f_orderTime AS _date , c.f_name AS Status , c.f_badge AS StatusBadge , d.f_name AS ShippingMethod , d.f_badge AS ShippingBadge , a.f_total AS Total , a.f_isPaid AS IsPaid , a.f_isDel AS IsDel
		FROM t_orders AS a WITH (NOLOCK) 
		JOIN t_members AS b WITH (NOLOCK) ON a.f_memberId = b.f_id 
		JOIN t_orderStatus AS c WITH(NOLOCK) ON a.f_status = c.f_id 
		JOIN t_orderShipping AS d WITH(NOLOCK) ON a.f_shippingMethod = d.f_id
	END
GO
