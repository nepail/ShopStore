USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_Member_GetList]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_Member_GetList]
  
AS
BEGIN
SELECT f_id AS ID,
	   f_name AS Name,
	   f_nickName AS NickName,
	   f_account AS Account,
	   f_groupId AS Level,
	   f_cash AS Money,
	   f_isSuspend AS IsSuspend
FROM t_members WITH (NOLOCK)


END
GO
