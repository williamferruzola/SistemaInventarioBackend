-- =============================================================================
-- SECCIÓN SEED - Datos semilla
-- =============================================================================
-- Columnas del esquema actual:
--   Categories, Providers, Users, Products, ProductProviders, Orders, OrderItems, WishLists                                
-- =============================================================================
USE SistemaInventario;
GO

SET NOCOUNT ON;
BEGIN TRANSACTION;

BEGIN TRY

    -- Limpiar datos (orden respetando FKs)
    DELETE FROM OrderItems;
    DELETE FROM Orders;
    DELETE FROM WishLists;
    DELETE FROM ProductProviders;
    DELETE FROM Products;
    DELETE FROM Providers;
    DELETE FROM Categories;
    DELETE FROM Users;

    -- Reiniciar identidades para IDs predecibles (1, 2, 3...)
    DBCC CHECKIDENT ('OrderItems',       RESEED, 0);
    DBCC CHECKIDENT ('Orders',           RESEED, 0);
    DBCC CHECKIDENT ('WishLists',        RESEED, 0);
    DBCC CHECKIDENT ('ProductProviders', RESEED, 0);
    DBCC CHECKIDENT ('Products',         RESEED, 0);
    DBCC CHECKIDENT ('Providers',        RESEED, 0);
    DBCC CHECKIDENT ('Categories',       RESEED, 0);
    DBCC CHECKIDENT ('Users',            RESEED, 0);

    -- -------------------------------------------------------------------------
    -- IDs explícitos con IDENTITY_INSERT para evitar inconsistencias de FK
    -- cuando el contador de identidad no parte en 1.
    -- -------------------------------------------------------------------------

    -- 1. Categories
    SET IDENTITY_INSERT Categories ON;
    INSERT INTO Categories (Id, Name, Description, IsActive, CreatedAt) VALUES
        (1, N'Monitores',    N'Pantallas y monitores para computadora',       1, GETUTCDATE()),
        (2, N'Audio',        N'Equipos de sonido, audífonos y parlantes',   1, GETUTCDATE()),
        (3, N'Computadoras', N'Laptops, desktops y accesorios',             1, GETUTCDATE()),
        (4, N'Telefonía',    N'Smartphones y tablets',                      1, GETUTCDATE()),
        (5, N'Redes',        N'Routers, switches y cableado',               1, GETUTCDATE());
    SET IDENTITY_INSERT Categories OFF;

    -- 2. Providers
    SET IDENTITY_INSERT Providers ON;
    INSERT INTO Providers (Id, Name, ContactEmail, Phone, Address, IsActive, CreatedAt) VALUES
        (1, N'Proveedor A', N'contacto@proveedora.com', N'+593-99-000-0001', N'Guayaquil, Av. Juan Tanca Marengo', 1, GETUTCDATE()),
        (2, N'Proveedor B', N'contacto@proveedorb.com', N'+593-99-000-0002', N'Quito, Av. Amazonas',               1, GETUTCDATE()),
        (3, N'Proveedor C', N'contacto@proveedorc.com', N'+593-99-000-0003', N'Cuenca, Calle Larga',               1, GETUTCDATE());
    SET IDENTITY_INSERT Providers OFF;

    -- 3. Users (contraseña: P@ssw0rd123!)
    SET IDENTITY_INSERT Users ON;
    INSERT INTO Users (Id, FullName, Email, PasswordHash, Role, IsActive, CreatedAt) VALUES
        (1, N'Administrador',  N'admin@inventario.com',
         N'$2a$11$ol2LhB6icMXKhL7dBZjJbeJd63a3IopA7vtZRlq37bsWO.g0UnIQO',
         N'Admin', 1, GETUTCDATE()),
        (2, N'Comprador Demo', N'buyer@inventario.com',
         N'$2a$11$ol2LhB6icMXKhL7dBZjJbeJd63a3IopA7vtZRlq37bsWO.g0UnIQO',
         N'Buyer', 1, GETUTCDATE());
    SET IDENTITY_INSERT Users OFF;

    -- 4. Products
    SET IDENTITY_INSERT Products ON;
    INSERT INTO Products (Id, CategoryId, Name, Description, ImageUrl, IsActive, CreatedAt) VALUES
        (1, 1, N'Monitor 50 pulgadas 4K',
         N'Monitor UHD 4K con panel IPS, 144Hz, HDR400.',
         N'https://m.media-amazon.com/images/I/913Z-4-0WoL._AC_SY355_.jpg', 1, GETUTCDATE()),
        (2, 2, N'Equipo de Sonido 20000W',
         N'Sistema de audio 2.1 con woofer de 12 pulgadas y Bluetooth.',
         N'https://m.media-amazon.com/images/I/71UIyqt3MVL._AC_SX679_.jpg', 1, GETUTCDATE()),
        (3, 3, N'Laptop Gamer RTX 4060',
         N'Laptop con procesador Intel i7, 16GB RAM, SSD 512GB.',
         N'https://m.media-amazon.com/images/I/71sgAr9atBS._AC_SL1500_.jpg', 1, GETUTCDATE()),
        (4, 4, N'Smartphone 5G Pro',
         N'Pantalla AMOLED 6.7in, cámara 200MP, batería 5000mAh.',
         N'https://m.media-amazon.com/images/I/51vYwEbmqPL._AC_SX522_.jpg', 1, GETUTCDATE());
    SET IDENTITY_INSERT Products OFF;

    -- 5. ProductProviders (inventario por proveedor)
    SET IDENTITY_INSERT ProductProviders ON;
    INSERT INTO ProductProviders (Id, ProductId, ProviderId, Price, Stock, BatchNumber, IsActive, CreatedAt) VALUES
        -- Monitor 50" 4K
        (1,  1, 1, 250.00,  15, N'LOTE-2024-MON-001', 1, GETUTCDATE()),
        (2,  1, 2, 300.00,   8, N'LOTE-2024-MON-002', 1, GETUTCDATE()),
        (3,  1, 3, 200.00,  22, N'LOTE-2024-MON-003', 1, GETUTCDATE()),
        -- Equipo de Sonido
        (4,  2, 1, 150.00,  30, N'LOTE-2024-SND-001', 1, GETUTCDATE()),
        (5,  2, 2, 200.00,  12, N'LOTE-2024-SND-002', 1, GETUTCDATE()),
        (6,  2, 3, 100.00,  45, N'LOTE-2024-SND-003', 1, GETUTCDATE()),
        -- Laptop Gamer
        (7,  3, 1, 1200.00,  5, N'LOTE-2024-LPT-001', 1, GETUTCDATE()),
        (8,  3, 2, 1350.00,  3, N'LOTE-2024-LPT-002', 1, GETUTCDATE()),
        -- Smartphone 5G
        (9,  4, 1, 650.00,  20, N'LOTE-2024-SPH-001', 1, GETUTCDATE()),
        (10, 4, 3, 580.00,  18, N'LOTE-2024-SPH-002', 1, GETUTCDATE());
    SET IDENTITY_INSERT ProductProviders OFF;


    -- Sincronizar contadores de identidad tras IDs explícitos
    DBCC CHECKIDENT ('Categories',       RESEED, 5);
    DBCC CHECKIDENT ('Providers',        RESEED, 3);
    DBCC CHECKIDENT ('Users',            RESEED, 2);
    DBCC CHECKIDENT ('Products',         RESEED, 4);
    DBCC CHECKIDENT ('ProductProviders', RESEED, 10);

    COMMIT TRANSACTION;
    PRINT 'Datos semilla insertados correctamente.';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH
GO