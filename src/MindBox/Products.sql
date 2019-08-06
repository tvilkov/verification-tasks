/*
Assumed tables structure

Product
	Id Name

Category
	Id Name

ProductCategory
	ProductId CategoryId
*/

select distinct p.Name, c.name from Product as p
	left join ProductCategory as pc on p.Id = pc.ProductId
	left join Category as c on c.Id = pc.CategoryId
order by p.Name;
