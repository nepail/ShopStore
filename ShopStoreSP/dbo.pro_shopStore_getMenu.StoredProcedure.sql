USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_getMenu]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[pro_shopStore_getMenu]
(
	@userid int
)
	
AS 

DECLARE @level int

--取得user的權限
SELECT @level = b.f_level FROM t_manager_users a WITH(NOLOCK)
JOIN t_manager_group AS b WITH(NOLOCK) ON a.f_groupid = b.f_id 
WHERE a.f_id = @userid 

IF @level is null
BEGIN
	SET @level = 1
END

--回傳結果
SELECT * FROM t_manager_menu WITH(NOLOCK)
WHERE f_level <= @level AND f_isdel = 0
order by f_issys, f_id

SELECT * FROM t_manager_menusub WITH(NOLOCK)
WHERE f_level <= @level AND f_isdel = 0
order by f_issys, f_id







--IF EXISTS(select top 1 1 from t_members WHERE f_account = @f_account and f_pwd = @f_pwd)
--BEGIN

--	DECLARE @id int

--	SELECT @id = f_id
--	FROM t_members 
--	WHERE f_account = @f_account and f_pwd = @f_pwd

--	SELECT *
--	FROM t_members 
--	WHERE f_account = @f_account and f_pwd = @f_pwd

--	INSERT INTO t_login (f_memberid, f_state, f_date) VALUES (@id, 'login', @f_date)
--END

GO
