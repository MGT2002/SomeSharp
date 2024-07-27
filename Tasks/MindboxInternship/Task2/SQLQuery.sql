SELECT 
    p.ProductName,
    ISNULL(c.CategoryName, 'No Category') AS CategoryName
FROM 
    Products p
LEFT JOIN 
    ProductCategory pc ON p.ProductID = pc.ProductID
LEFT JOIN 
    Categories c ON pc.CategoryID = c.CategoryID;
