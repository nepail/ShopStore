USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_fr_getMember]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 取得會員資料
    日期: 2021-11-16

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_fr_getMember] (
				 @account NVARCHAR(20) ,
				 @pcode VARCHAR(20) ,
				 @date DATETIME
)
AS

	IF EXISTS (SELECT TOP 1 1
			FROM t_members WITH (NOLOCK)
			WHERE f_account = @account
				  AND
				  f_pcode = @pcode
		)
		BEGIN
			DECLARE @id INT
			SELECT @id = f_id FROM t_members WITH(NOLOCK)
			WHERE f_account = @account
				  AND
				  f_pcode = @pcode
			SELECT [f_id] , [f_name] , [f_nickName] , [f_account] , [f_pcode] , [f_phone] , [f_mail] , [f_createTime] , [f_address] , [f_groupId] , [f_updateTime] , [f_cash] , [f_isSuspend]
			FROM t_members WITH(NOLOCK)
			WHERE f_account = @account
				  AND
				  f_pcode = @pcode
			INSERT INTO t_login (f_memberId , f_state , f_date)
			VALUES (@id , 'login' , @date)
		END
GO
