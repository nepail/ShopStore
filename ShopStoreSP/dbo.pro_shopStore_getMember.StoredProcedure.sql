USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_getMember]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[pro_shopStore_getMember]
(
	@f_account NVARCHAR(20), 
    @f_pcode VARCHAR(20),
	@f_date Datetime
)
	
AS 

IF EXISTS(select top 1 1 from t_members WHERE f_account = @f_account and f_pcode = @f_pcode)
BEGIN

	DECLARE @id int

	SELECT @id = f_id
	FROM t_members 
	WHERE f_account = @f_account and f_pcode = @f_pcode

	SELECT *
	FROM t_members 
	WHERE f_account = @f_account and f_pcode = @f_pcode

	INSERT INTO t_login (f_memberid, f_state, f_date) VALUES (@id, 'login', @f_date)
END

GO
