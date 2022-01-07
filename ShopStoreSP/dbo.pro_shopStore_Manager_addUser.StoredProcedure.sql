USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_addUser]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_addUser]				
	@f_account varchar(20),
	@f_pcode varchar(20),
	@f_name nvarchar(20),
	@f_groupId int,
	@f_createTime datetime,
	@f_updateTime datetime
AS
BEGIN

INSERT INTO t_manager_users 

	 (f_account, f_pcode, f_name, f_groupId, f_createTime, f_updateTime)
VALUES(@f_account, @f_pcode, @f_name, @f_groupId, @f_createTime, @f_updateTime)
END
GO
