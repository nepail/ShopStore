USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_editMemberLevel]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 修改會員等級
    日期: 2022-01-03

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_editMemberLevel]
				 @ID INT ,
				 @Name NVARCHAR(20) ,
				 @NickName NVARCHAR(20) ,
				 @Account VARCHAR(20) ,
				 @Level INT ,
				 @Money INT ,
				 @IsSuspend INT
AS
	BEGIN
		UPDATE t_members WITH(ROWLOCK) SET f_groupId = @Level WHERE f_id = @ID			  		
	END
GO
