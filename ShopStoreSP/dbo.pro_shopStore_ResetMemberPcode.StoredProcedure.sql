USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_shopStore_ResetMemberPcode]    Script Date: 2022/1/7 下午 06:17:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[pro_shopStore_ResetMemberPcode]
(
  @code VARCHAR(20),
  @mail VARCHAR(30)
)
AS

BEGIN

UPDATE t_members
SET
	f_pcode=@code

WHERE f_mail=@mail

END
GO
