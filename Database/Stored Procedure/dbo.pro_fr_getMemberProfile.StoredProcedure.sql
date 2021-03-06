USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_fr_getMemberProfile]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    描述: 取得會員資料
    日期: 2021-01-06

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_fr_getMemberProfile] (
				 @memberId INT
)
AS
	BEGIN
		SELECT a.f_nickName AS NickName , b.f_name AS GroupName , a.f_cash AS Cash , a.f_name AS RealName , a.f_mail AS Email , a.f_address AS Address
		FROM t_members a WITH (NOLOCK) LEFT JOIN t_members_group b WITH (NOLOCK) ON a.f_groupId = b.f_id
		WHERE a.f_id = @memberId
	END
GO
