Create view northwind.vw_orderdetail
AS
SELECT OrderID, ProductID, Round(cast(UnitPrice as float),2) as UnitPrice,  Quantity,Discount, entry_date 
FROM northwind.order_details;