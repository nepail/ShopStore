USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_Manager_Member_SuspendByID]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[pro_shopStore_Manager_Member_SuspendByID]
  @memberId INT,
  @isSuspend BIT

AS

BEGIN

UPDATE t_members
SET
	f_isSuspend = @isSuspend
WHERE f_id=@memberId


END
GO
