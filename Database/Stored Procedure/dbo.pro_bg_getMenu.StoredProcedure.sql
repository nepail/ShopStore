USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_getMenu]    Script Date: 2022/1/24 上午 09:14:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 新增產品
    日期: 2021-12-06

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_getMenu] (
				 @userid INT
)

AS
	DECLARE @level INT

	--取得user的權限
	SELECT @level = b.f_level
	FROM t_manager_users a WITH (NOLOCK) JOIN t_manager_group AS b WITH (NOLOCK) ON a.f_groupId = b.f_id
	WHERE a.f_id = @userid

	IF @level IS NULL
		BEGIN
			SET @level = 1
		END

	--回傳結果
	SELECT f_id , f_name , f_icon , f_level , f_isOpen , f_isSys , f_isDel
	FROM t_manager_menu WITH (NOLOCK)
	WHERE f_level <= @level
		  AND
		  f_isDel = 0
	ORDER BY f_isSys ,
	f_id

	SELECT f_id , f_menuid , f_name , f_controller , f_level , f_isOpen , f_isSys , f_isDel
	FROM t_manager_menuSub WITH (NOLOCK)
	WHERE f_level <= @level
		  AND
		  f_isDel = 0
	ORDER BY f_isSys ,
	f_id
GO
