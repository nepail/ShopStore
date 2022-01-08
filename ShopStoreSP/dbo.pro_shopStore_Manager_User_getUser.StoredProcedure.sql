USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_User_getUser]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_User_getUser]
  @Account VARCHAR(20),
  @Pcode   VARCHAR(20)

AS

BEGIN

SELECT f_id AS ID,
	   f_account AS Account,
	   f_name AS Name,
	   f_groupId AS GroupId

FROM t_manager_users

WHERE f_account=@Account
	  AND f_pcode=@Pcode


END
GO
