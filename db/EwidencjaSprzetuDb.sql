USE master;
GO

IF DB_ID(N'EwidencjaSprzetuDb') IS NOT NULL
BEGIN
    ALTER DATABASE EwidencjaSprzetuDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EwidencjaSprzetuDb;
END
GO

CREATE DATABASE EwidencjaSprzetuDb;
GO

USE EwidencjaSprzetuDb;
GO

IF OBJECT_ID('dbo.Serwisy', 'U') IS NOT NULL DROP TABLE dbo.Serwisy;
IF OBJECT_ID('dbo.Przypisania', 'U') IS NOT NULL DROP TABLE dbo.Przypisania;
IF OBJECT_ID('dbo.Sprzety', 'U') IS NOT NULL DROP TABLE dbo.Sprzety;
IF OBJECT_ID('dbo.Pracownicy', 'U') IS NOT NULL DROP TABLE dbo.Pracownicy;
IF OBJECT_ID('dbo.Dzialy', 'U') IS NOT NULL DROP TABLE dbo.Dzialy;
IF OBJECT_ID('dbo.Lokalizacje', 'U') IS NOT NULL DROP TABLE dbo.Lokalizacje;
IF OBJECT_ID('dbo.Dostawcy', 'U') IS NOT NULL DROP TABLE dbo.Dostawcy;
GO

CREATE TABLE dbo.Dzialy (
    DzialId INT IDENTITY(1,1) CONSTRAINT PK_Dzialy PRIMARY KEY,
    Nazwa NVARCHAR(100) NOT NULL CONSTRAINT UQ_Dzialy_Nazwa UNIQUE
);
GO

CREATE TABLE dbo.Lokalizacje (
    LokalizacjaId INT IDENTITY(1,1) CONSTRAINT PK_Lokalizacje PRIMARY KEY,
    Nazwa NVARCHAR(120) NOT NULL CONSTRAINT UQ_Lokalizacje_Nazwa UNIQUE,
    Adres NVARCHAR(200) NULL
);
GO

CREATE TABLE dbo.Dostawcy (
    DostawcaId INT IDENTITY(1,1) CONSTRAINT PK_Dostawcy PRIMARY KEY,
    Nazwa NVARCHAR(160) NOT NULL CONSTRAINT UQ_Dostawcy_Nazwa UNIQUE,
    Email NVARCHAR(120) NULL,
    Telefon NVARCHAR(40) NULL
);
GO

CREATE TABLE dbo.Pracownicy (
    PracownikId INT IDENTITY(1,1) CONSTRAINT PK_Pracownicy PRIMARY KEY,
    DzialId INT NOT NULL,
    Imie NVARCHAR(60) NOT NULL,
    Nazwisko NVARCHAR(80) NOT NULL,
    Email NVARCHAR(120) NOT NULL CONSTRAINT UQ_Pracownicy_Email UNIQUE,
    Telefon NVARCHAR(40) NULL,
    Aktywny BIT NOT NULL CONSTRAINT DF_Pracownicy_Aktywny DEFAULT(1),
    CONSTRAINT FK_Pracownicy_Dzialy
        FOREIGN KEY (DzialId) REFERENCES dbo.Dzialy(DzialId)
);
GO

CREATE TABLE dbo.Sprzety (
    SprzetId INT IDENTITY(1,1) CONSTRAINT PK_Sprzety PRIMARY KEY,

    TypSprzetu NVARCHAR(50) NOT NULL,
    NumerEwidencyjny NVARCHAR(50) NOT NULL CONSTRAINT UQ_Sprzety_NumerEwidencyjny UNIQUE,
    NumerSeryjny NVARCHAR(100) NULL,

    Status INT NOT NULL CONSTRAINT DF_Sprzety_Status DEFAULT(0),
    CONSTRAINT CK_Sprzety_Status CHECK (Status IN (0,1,2,3)),

    DataZakupu DATE NULL,
    DataKoncaGwarancji DATE NULL,

    LokalizacjaId INT NULL,
    DostawcaId INT NULL,

    Uwagi NVARCHAR(500) NULL,

    Procesor NVARCHAR(100) NULL,
    RamGb INT NULL,
    DyskGb INT NULL,
    SystemOperacyjny NVARCHAR(100) NULL,

    PrzekatnaCala DECIMAL(4,1) NULL,
    Obudowa NVARCHAR(50) NULL,
    Rozdzielczosc NVARCHAR(50) NULL,
    Kolorowa BIT NULL,

    AdresIp NVARCHAR(50) NULL,
    AdresMac NVARCHAR(50) NULL,

    CONSTRAINT FK_Sprzety_Lokalizacje
        FOREIGN KEY (LokalizacjaId) REFERENCES dbo.Lokalizacje(LokalizacjaId),
    CONSTRAINT FK_Sprzety_Dostawcy
        FOREIGN KEY (DostawcaId) REFERENCES dbo.Dostawcy(DostawcaId)
);
GO

CREATE TABLE dbo.Przypisania (
    PrzypisanieId INT IDENTITY(1,1) CONSTRAINT PK_Przypisania PRIMARY KEY,
    SprzetId INT NOT NULL,
    PracownikId INT NOT NULL,
    PrzypisanoDnia DATETIME2 NOT NULL CONSTRAINT DF_Przy_Przy DEFAULT(SYSDATETIME()),
    ZwroconoDnia DATETIME2 NULL,
    Uwagi NVARCHAR(500) NULL,

    CONSTRAINT FK_Przy_Sprzety
        FOREIGN KEY (SprzetId) REFERENCES dbo.Sprzety(SprzetId),
    CONSTRAINT FK_Przy_Pracownicy
        FOREIGN KEY (PracownikId) REFERENCES dbo.Pracownicy(PracownikId)
);
GO

CREATE TABLE dbo.Serwisy (
    SerwisId INT IDENTITY(1,1) CONSTRAINT PK_Serwisy PRIMARY KEY,
    SprzetId INT NOT NULL,
    DostawcaId INT NULL,

    WykonanoDnia DATETIME2 NOT NULL CONSTRAINT DF_Serwisy_Data DEFAULT(SYSDATETIME()),
    RodzajSerwisu NVARCHAR(120) NOT NULL,
    Opis NVARCHAR(1000) NULL,
    Koszt DECIMAL(10,2) NULL,

    CONSTRAINT FK_Serwisy_Sprzety
        FOREIGN KEY (SprzetId) REFERENCES dbo.Sprzety(SprzetId),
    CONSTRAINT FK_Serwisy_Dostawcy
        FOREIGN KEY (DostawcaId) REFERENCES dbo.Dostawcy(DostawcaId)
);
GO

CREATE INDEX IX_Pracownicy_DzialId ON dbo.Pracownicy(DzialId);

CREATE INDEX IX_Sprzety_LokalizacjaId ON dbo.Sprzety(LokalizacjaId);
CREATE INDEX IX_Sprzety_DostawcaId ON dbo.Sprzety(DostawcaId);

CREATE INDEX IX_Przy_SprzetId ON dbo.Przypisania(SprzetId);
CREATE INDEX IX_Przy_PracownikId ON dbo.Przypisania(PracownikId);

CREATE INDEX IX_Serwisy_SprzetId ON dbo.Serwisy(SprzetId);
GO

CREATE UNIQUE INDEX UX_Przy_JednoAktywneNaSprzet
ON dbo.Przypisania(SprzetId)
WHERE ZwroconoDnia IS NULL;
GO

USE EwidencjaSprzetuDb;

-- Działy
INSERT INTO dbo.Dzialy (Nazwa) VALUES
(N'Dział IT'),
(N'Dział Kadr'),
(N'Dział Księgowości'),
(N'Dział Sprzedaży');

-- Lokalizacje
INSERT INTO dbo.Lokalizacje (Nazwa, Adres) VALUES
(N'Biuro Rzeszów', N'ul. Przykładowa 1, Rzeszów'),
(N'Magazyn', N'ul. Magazynowa 10, Rzeszów'),
(N'Biuro Warszawa', N'ul. Przykładowa 20, Warszawa');

-- Dostawcy
INSERT INTO dbo.Dostawcy (Nazwa, Email, Telefon) VALUES
(N'Komputronik', N'kontakt@komputronik.pl', N'123-456-789'),
(N'x-kom', N'kontakt@x-kom.pl', N'987-654-321'),
(N'Serwis IT-Pro', N'serwis@itpro.pl', N'555-666-777');

-- Pracownicy
INSERT INTO dbo.Pracownicy (DzialId, Imie, Nazwisko, Email, Telefon, Aktywny)
SELECT DzialId, N'Jan', N'Kowalski', N'jan.kowalski@firma.pl', N'600-100-200', 1
FROM dbo.Dzialy WHERE Nazwa = N'Dział IT';

INSERT INTO dbo.Pracownicy (DzialId, Imie, Nazwisko, Email, Telefon, Aktywny)
SELECT DzialId, N'Anna', N'Nowak', N'anna.nowak@firma.pl', N'600-300-400', 1
FROM dbo.Dzialy WHERE Nazwa = N'Dział Kadr';

INSERT INTO dbo.Pracownicy (DzialId, Imie, Nazwisko, Email, Telefon, Aktywny)
SELECT DzialId, N'Piotr', N'Wiśniewski', N'piotr.wisniewski@firma.pl', N'600-500-600', 1
FROM dbo.Dzialy WHERE Nazwa = N'Dział Sprzedaży';

DECLARE @L_BiuroRzeszow INT = (SELECT LokalizacjaId FROM dbo.Lokalizacje WHERE Nazwa = N'Biuro Rzeszów');
DECLARE @L_Magazyn     INT = (SELECT LokalizacjaId FROM dbo.Lokalizacje WHERE Nazwa = N'Magazyn');

DECLARE @D_Komputronik INT = (SELECT DostawcaId FROM dbo.Dostawcy WHERE Nazwa = N'Komputronik');
DECLARE @D_XKom        INT = (SELECT DostawcaId FROM dbo.Dostawcy WHERE Nazwa = N'x-kom');
DECLARE @D_ITPro       INT = (SELECT DostawcaId FROM dbo.Dostawcy WHERE Nazwa = N'Serwis IT-Pro');

INSERT INTO dbo.Sprzety
(TypSprzetu, NumerEwidencyjny, NumerSeryjny, Status, DataZakupu, DataKoncaGwarancji,
 LokalizacjaId, DostawcaId, Uwagi, Procesor, RamGb, DyskGb, SystemOperacyjny, PrzekatnaCala)
VALUES
(N'Laptop', N'EW-0001', N'SN-LAP-001', 1,
 DATEADD(DAY,-200,CAST(GETDATE() AS DATE)), DATEADD(DAY,45,CAST(GETDATE() AS DATE)),
 @L_BiuroRzeszow, @D_Komputronik, N'Laptop służbowy - dział IT', N'Intel Core i5', 16, 512, N'Windows 11 Pro', 15.6);

INSERT INTO dbo.Sprzety
(TypSprzetu, NumerEwidencyjny, NumerSeryjny, Status, DataZakupu, DataKoncaGwarancji,
 LokalizacjaId, DostawcaId, Uwagi, PrzekatnaCala, Rozdzielczosc)
VALUES
(N'Monitor', N'EW-0002', N'SN-MON-002', 1,
 DATEADD(DAY,-150,CAST(GETDATE() AS DATE)), DATEADD(DAY,80,CAST(GETDATE() AS DATE)),
 @L_BiuroRzeszow, @D_XKom, N'Monitor dla HR', 27.0, N'2560x1440');

INSERT INTO dbo.Sprzety
(TypSprzetu, NumerEwidencyjny, NumerSeryjny, Status, DataZakupu, DataKoncaGwarancji,
 LokalizacjaId, DostawcaId, Uwagi, Kolorowa)
VALUES
(N'Drukarka', N'EW-0003', N'SN-PRN-003', 2,
 DATEADD(DAY,-400,CAST(GETDATE() AS DATE)), DATEADD(DAY,20,CAST(GETDATE() AS DATE)),
 @L_Magazyn, @D_Komputronik, N'Drukarka magazynowa - obecnie w serwisie', 1);

INSERT INTO dbo.Sprzety
(TypSprzetu, NumerEwidencyjny, NumerSeryjny, Status, DataZakupu, DataKoncaGwarancji,
 LokalizacjaId, DostawcaId, Uwagi, AdresIp, AdresMac)
VALUES
(N'Urządzenie sieciowe', N'EW-0004', N'SN-SW-004', 0,
 DATEADD(DAY,-60,CAST(GETDATE() AS DATE)), DATEADD(DAY,365,CAST(GETDATE() AS DATE)),
 @L_Magazyn, @D_XKom, N'Switch do wdrożenia', N'192.168.1.10', N'AA-BB-CC-11-22-33');

DECLARE @Sprzet_EW0001 INT = (SELECT SprzetId FROM dbo.Sprzety WHERE NumerEwidencyjny = N'EW-0001');
DECLARE @Sprzet_EW0002 INT = (SELECT SprzetId FROM dbo.Sprzety WHERE NumerEwidencyjny = N'EW-0002');

DECLARE @Jan  INT = (SELECT PracownikId FROM dbo.Pracownicy WHERE Email = N'jan.kowalski@firma.pl');
DECLARE @Anna INT = (SELECT PracownikId FROM dbo.Pracownicy WHERE Email = N'anna.nowak@firma.pl');

INSERT INTO dbo.Przypisania (SprzetId, PracownikId, PrzypisanoDnia, ZwroconoDnia, Uwagi)
VALUES
(@Sprzet_EW0001, @Jan, DATEADD(DAY,-20,SYSDATETIME()), DATEADD(DAY,-10,SYSDATETIME()), N'Historia testowa (zamknięte)');

INSERT INTO dbo.Przypisania (SprzetId, PracownikId, PrzypisanoDnia, ZwroconoDnia, Uwagi)
VALUES
(@Sprzet_EW0001, @Jan, DATEADD(DAY,-5,SYSDATETIME()), NULL, N'Przypisanie aktywne (do screenów)');

INSERT INTO dbo.Przypisania (SprzetId, PracownikId, PrzypisanoDnia, ZwroconoDnia, Uwagi)
VALUES
(@Sprzet_EW0002, @Anna, DATEADD(DAY,-3,SYSDATETIME()), NULL, N'Monitor przypisany do HR');

DECLARE @Sprzet_EW0003 INT = (SELECT SprzetId FROM dbo.Sprzety WHERE NumerEwidencyjny = N'EW-0003');

INSERT INTO dbo.Serwisy (SprzetId, DostawcaId, WykonanoDnia, RodzajSerwisu, Opis, Koszt)
VALUES
(@Sprzet_EW0003, @D_ITPro, DATEADD(DAY,-2,SYSDATETIME()), N'Naprawa', N'Wymiana rolki i czyszczenie mechanizmu', 230.00);

INSERT INTO dbo.Serwisy (SprzetId, DostawcaId, WykonanoDnia, RodzajSerwisu, Opis, Koszt)
VALUES
(@Sprzet_EW0001, @D_ITPro, DATEADD(DAY,-1,SYSDATETIME()), N'Przegląd', N'Przegląd okresowy, aktualizacje', 150.00);

UPDATE dbo.Sprzety
SET Status = 1
WHERE NumerEwidencyjny IN (N'EW-0001', N'EW-0002');

UPDATE dbo.Sprzety
SET Status = 2
WHERE NumerEwidencyjny = (N'EW-0003');

SELECT 'Dzialy' AS Tabela, COUNT(*) AS Ilosc FROM dbo.Dzialy
UNION ALL SELECT 'Pracownicy', COUNT(*) FROM dbo.Pracownicy
UNION ALL SELECT 'Lokalizacje', COUNT(*) FROM dbo.Lokalizacje
UNION ALL SELECT 'Dostawcy', COUNT(*) FROM dbo.Dostawcy
UNION ALL SELECT 'Sprzety', COUNT(*) FROM dbo.Sprzety
UNION ALL SELECT 'Przypisania', COUNT(*) FROM dbo.Przypisania
UNION ALL SELECT 'Serwisy', COUNT(*) FROM dbo.Serwisy;
