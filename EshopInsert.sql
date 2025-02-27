INSERT INTO USERS (USER_ID, NAME, EMAIL, PASSWORD_HASH, SALT, TELEPHONE, ADDRESS)
VALUES
(1, 'Alice Smith', 'alice.smith@example.com', 0x1234, 'salt123', '123456789', '123 Main St'),
(2, 'Bob Johnson', 'bob.johnson@example.com', 0x5678, 'salt456', '987654321', '456 Elm St');


INSERT INTO STORES (STORE_ID, STORE_NAME, EMAIL, IMAGE, CATEGORY, USER_ID)
VALUES
(1, 'Handmade Earrings', 'earrings@example.com', 'earrings.jpg', 'Jewelry', 1),
(2, 'Cozy Scarves', 'scarves@example.com', 'scarves.jpg', 'Clothing', 1),
(3, 'Artistic Fanzines', 'fanzines@example.com', 'fanzines.jpg', 'Art', 2);



INSERT INTO PRODUCTS_CATEGORIES (CATEGORY_ID, CATEGORY_NAME)
VALUES
(1, 'Earrings'),
(2, 'Winter Clothing'),
(3, 'Zines');


INSERT INTO PRODUCTS (PRODUCT_ID, STORE_ID, PRODUCT_NAME, DESCRIPTION, IMAGE, PRICE, STOCK_QUANTITY)
VALUES
-- Earrings (Store 1)
(1, 1, 'Silver Hoop Earrings', 'Handmade silver hoop earrings.', 'hoop_earrings.jpg', 25.00, 10),
(2, 1, 'Beaded Earrings', 'Colorful beaded earrings.', 'beaded_earrings.jpg', 15.00, 20),

-- Scarves (Store 2)
(3, 2, 'Wool Scarf', 'Warm wool scarf in neutral tones.', 'wool_scarf.jpg', 35.00, 15),
(4, 2, 'Silk Scarf', 'Elegant silk scarf with floral patterns.', 'silk_scarf.jpg', 45.00, 10),

-- Fanzines (Store 3)
(5, 3, 'Indie Art Zine', 'A collection of indie art and poetry.', 'art_zine.jpg', 10.00, 30),
(6, 3, 'Music Fanzine', 'A fanzine dedicated to underground music.', 'music_zine.jpg', 12.00, 25);


INSERT INTO PROD_CAT (PRODUCT_ID, CATEGORY_ID)
VALUES
-- Earrings (Category 1: Jewelry)
(1, 1),
(2, 1),

-- Scarves (Category 2: Clothing)
(3, 2),
(4, 2),

-- Fanzines (Category 3: Art)
(5, 3),
(6, 3);


INSERT INTO PURCHASES (PURCHASE_ID, USER_ID, TOTAL_PRICE, PAYMENT_STATUS)
VALUES
(1, 1, 60.00, 'Completed'), -- Alice buys earrings and a scarf
(2, 2, 22.00, 'Completed'); -- Bob buys two fanzines


INSERT INTO PURCHASE_ITEMS (PURCHASE_ITEM_ID, PURCHASE_ID, PRODUCT_ID, QUANTITY, PRICE_AT_PURCHASE)
VALUES
-- Purchase 1: Alice buys earrings and a scarf
(1, 1, 1, 1, 25.00), -- Silver Hoop Earrings
(2, 1, 3, 1, 35.00), -- Wool Scarf

-- Purchase 2: Bob buys two fanzines
(3, 2, 5, 1, 10.00), -- Indie Art Zine
(4, 2, 6, 1, 12.00); -- Music Fanzine


INSERT INTO PURCHASE_VENDOR_MAPPING (PURCHASE_VENDOR_ID, PURCHASE_ID, VENDOR_ID, VENDOR_AMOUNT)
VALUES
-- Purchase 1: Alice buys from Store 1 and Store 2
(1, 1, 1, 25.00), -- Store 1 (Earrings)
(2, 1, 2, 35.00), -- Store 2 (Scarf)

-- Purchase 2: Bob buys from Store 3
(3, 2, 3, 22.00); -- Store 3 (Fanzines)


INSERT INTO PAYMENTS (PAYMENT_ID, PURCHASE_ID, USER_ID, PAYMENT_METHOD, TRANSACTION_ID, AMOUNT)
VALUES
(1, 1, 1, 'Credit Card', 'TXN12345', 60.00), -- Alice's payment
(2, 2, 2, 'PayPal', 'TXN67890', 22.00); -- Bob's payment


INSERT INTO STORE_PAYOUTS (PAYOUT_ID, STORE_ID, PURCHASE_ID, PAYOUT_AMOUNT, PAYOUT_STATUS, PAYOUT_METHOD)
VALUES
-- Payouts for Purchase 1
(1, 1, 1, 25.00, 'Paid', 'Bank Transfer'), -- Store 1 (Earrings)
(2, 2, 1, 35.00, 'Paid', 'Bank Transfer'), -- Store 2 (Scarf)

-- Payout for Purchase 2
(3, 3, 2, 22.00, 'Paid', 'PayPal'); -- Store 3 (Fanzines)

SELECT * FROM USERS;
SELECT * FROM STORES;
SELECT * FROM PRODUCTS;
SELECT * FROM PRODUCTS_CATEGORIES;
SELECT * FROM PROD_CAT;
SELECT * FROM PURCHASES;
SELECT * FROM PURCHASE_ITEMS;
SELECT * FROM PURCHASE_VENDOR_MAPPING;
SELECT * FROM PAYMENTS;
SELECT * FROM STORE_PAYOUTS;