CREATE PROCEDURE SP_SEARCH_SHOPPING_CART_BY_NAME
	@searchString NVARCHAR(50)
AS
BEGIN
	SELECT c.Id, c.[User], c.DateCreated
	FROM ShoppingCart c
	WHERE c.[User] LIKE '%' + @searchString + '%'
END
GO
