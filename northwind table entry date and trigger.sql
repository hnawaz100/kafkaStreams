use northwind;
ALTER TABLE orders
ADD COLUMN entry_date datetime NOT NULL DEFAULT CURRENT_TIMESTAMP;

use northwind;
CREATE TRIGGER order_updater BEFORE UPDATE ON orders FOR EACH ROW SET new.entry_date = CURRENT_TIMESTAMP;

use northwind;
ALTER TABLE customers ADD COLUMN id INT AUTO_INCREMENT UNIQUE FIRST;
ALTER TABLE customers
ADD COLUMN entry_date datetime NOT NULL DEFAULT CURRENT_TIMESTAMP;

use northwind;
CREATE TRIGGER order_updater BEFORE UPDATE ON customers FOR EACH ROW SET new.entry_date = CURRENT_TIMESTAMP;


use northwind;
ALTER TABLE `order details`
ADD COLUMN entry_date datetime NOT NULL DEFAULT CURRENT_TIMESTAMP;

use northwind;
CREATE TRIGGER orderdetail_updater BEFORE UPDATE ON northwind.`order details` FOR EACH ROW SET new.entry_date = CURRENT_TIMESTAMP;
