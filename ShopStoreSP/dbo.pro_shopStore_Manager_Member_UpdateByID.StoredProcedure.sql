USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_Member_UpdateByID]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_Member_UpdateByID]
  @ID		 INT,
  @Name		 NVARCHAR(20),
  @NickName NVARCHAR(20),
  @Account	 VARCHAR(20),
  @Level	 INT,
  @Money	 INT,
  @IsSuspend INT

AS

BEGIN

UPDATE t_members
SET
	f_groupId=@Level
WHERE f_id=@ID


END
GO
