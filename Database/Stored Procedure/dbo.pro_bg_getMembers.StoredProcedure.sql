USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_getMembers]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 取得會員列表
    日期: 2022-01-03

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_getMembers]

AS
	BEGIN
		SELECT f_id AS ID , f_name AS Name , f_nickName AS NickName , f_account AS Account , f_groupId AS Level , f_cash AS Money , f_isSuspend AS IsSuspend
		FROM t_members WITH (NOLOCK)
	END
GO
