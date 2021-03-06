USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_editMemberPwd]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 更新訂單狀態或運送狀態
    日期: 2022-01-07

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_editMemberPwd] (
				 @pwd VARCHAR(20) ,
				 @mail VARCHAR(30)
)
AS
	BEGIN
		UPDATE t_members WITH(ROWLOCK) SET f_pcode = @pwd WHERE f_mail = @mail			  		
	END
GO
