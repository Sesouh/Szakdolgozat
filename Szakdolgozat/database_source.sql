-- phpMyAdmin SQL Dump
-- version 4.8.4
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2019. Már 29. 15:39
-- Kiszolgáló verziója: 10.1.37-MariaDB
-- PHP verzió: 7.3.0


START TRANSACTION;




--
-- Adatbázis: `asd`
--

--
-- Eljárások
--
CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `bejovo_insert` (IN `inid` INT, IN `indarab` INT, IN `inpartner` INT, IN `infelvevo` VARCHAR(20), IN `inteljesitve` INT)  NO SQL
BEGIN
INSERT INTO tranzakcio(
    tranzakcio.item_id,
    tranzakcio.partner_id,
    tranzakcio.darabszam,
    tranzakcio.idopont,
	tranzakcio.ido_teljesitve) VALUES (
        inid,
        inpartner,
        indarab,
        CURRENT_TIMESTAMP,
    	IF(inteljesitve = 1,CURRENT_TIMESTAMP,NULL));

	CALL logolas(infelvevo,
                'Bejövő tranzakció hozzáadás',
                LAST_INSERT_ID(),
                " ID: bejövő tranzakció létrejött.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `felhasznalo_delete` (IN `innev` VARCHAR(20), IN `infelvevo` VARCHAR(20))  NO SQL
BEGIN
    DELETE FROM `felhasznalok`
    WHERE felhasznalonev = innev;
	
	CALL logolas(infelvevo,
                'Felhasználó törlés',
                innev,
                " nevű felhasználó törlődött.");
END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `felhasznalo_insert` (IN `innev` VARCHAR(20), IN `injelszo` TEXT, IN `injogosultsag` VARCHAR(10), IN `inaktivitas` VARCHAR(10), IN `infelvevo` VARCHAR(20))  NO SQL
BEGIN
INSERT INTO felhasznalok(
    felhasznalonev,
    jelszo,
    jogosultsag,
    aktivitas) VALUES (
        innev,
        injelszo,
        injogosultsag,
        inaktivitas);

	CALL logolas(infelvevo,
                'Felhasználó felvétel',
                innev,
                " nevű felhasználó létrejött.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `felhasznalo_ujaktivitas` (IN `innev` VARCHAR(20), IN `infelvevo` VARCHAR(20))  NO SQL
BEGIN
UPDATE felhasznalok
SET aktivitas = IF(aktivitas = 'aktiv', 'inaktiv', 'aktiv')
WHERE felhasznalonev = innev;

	CALL logolas(infelvevo,
                'Felhasználó módosítás',
                innev,
                " nevű felhasználó aktivitása megváltozott.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `felhasznalo_ujjelszo` (IN `innev` VARCHAR(20), IN `injelszo` TEXT, IN `infelvevo` VARCHAR(20))  NO SQL
BEGIN
UPDATE felhasznalok
SET jelszo = injelszo
WHERE felhasznalonev = innev;

	CALL logolas(infelvevo,
                'Felhasználó módosítás',
                innev,
                " nevű felhasználó jelszava megváltozott.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `felhasznalo_ujjogosultsag` (IN `innev` VARCHAR(20), IN `injog` VARCHAR(20), IN `infelvevo` VARCHAR(20))  NO SQL
BEGIN
UPDATE felhasznalok
SET jogosultsag = injog
WHERE felhasznalonev = innev;

	CALL logolas(infelvevo,
                'Felhasználó módosítás',
                innev,
                " nevű felhasználó jogosultsága megváltozott.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `item_delete` (IN `inid` INT(10), IN `innev` VARCHAR(20), IN `infelvevo` VARCHAR(20))  NO SQL
BEGIN
DELETE FROM item
WHERE id = inid;

	CALL logolas(infelvevo,
                'Tárgy törlés',
                innev,
                " tárgy törlődött.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `item_insert` (IN `innev` VARCHAR(30), IN `inleiras` TEXT, IN `ink_id` INT(11), IN `infelvevo` VARCHAR(20))  NO SQL
BEGIN
INSERT INTO item(
    nev,
    leiras,
    kategoria_id) VALUES (
        innev,
        inleiras,
        ink_id);
        
	CALL logolas(infelvevo,
                'Tárgy felvétel',
                innev,
                " tárgy létrejött.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `kategoria_delete` (IN `innev` VARCHAR(20), IN `infelvevo` VARCHAR(20))  NO SQL
BEGIN
DELETE FROM kategoria
WHERE nev = innev;

	CALL logolas(infelvevo,
                'Kategória törlés',
                innev,
                " kategória törlődött.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `kategoria_insert` (IN `innev` VARCHAR(20), IN `infelvevo` VARCHAR(20))  NO SQL
BEGIN
INSERT INTO kategoria(id,nev)
VALUES (NULL, innev);

	CALL logolas(infelvevo,
                'Kategória felvétel',
                innev,
                " kategória létrejött.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `kimeno_insert` (IN `inid` INT, IN `indarab` INT, IN `inpartner` INT, IN `infelvevo` VARCHAR(20), IN `inteljesitve` INT)  NO SQL
BEGIN
INSERT INTO tranzakcio(
    tranzakcio.item_id,
    tranzakcio.partner_id,
    tranzakcio.darabszam,
    tranzakcio.idopont,
	tranzakcio.ido_teljesitve) VALUES (
        inid,
        inpartner,
        indarab,
        CURRENT_TIMESTAMP,
    	IF(inteljesitve = 1,CURRENT_TIMESTAMP,NULL));

	CALL logolas(infelvevo,
                'Kimenő tranzakció hozzáadás',
                LAST_INSERT_ID(),
                " ID: kimenő tranzakció létrejött.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `logolas` (IN `felvevo` VARCHAR(20), IN `muvelet` TEXT, IN `adat` VARCHAR(40), IN `leiras` TEXT)  NO SQL
BEGIN
    INSERT INTO log(
        felhasznalo_nev,
        idopont,
        muvelet,
        reszletek) VALUES (
            felvevo,
            CURRENT_TIMESTAMP,
            muvelet,
            CONCAT(adat,leiras));
END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `partner_delete` (IN `inid` INT(10), IN `infelvevo` VARCHAR(20), IN `innev` VARCHAR(40))  NO SQL
BEGIN
DELETE FROM partner
WHERE id = inid;

	CALL logolas(infelvevo,
                'Partner törlés',
                innev,
                " partner törlődött.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `partner_insert` (IN `innev` VARCHAR(40), IN `incim` VARCHAR(100), IN `intelefon` VARCHAR(20), IN `inemail` VARCHAR(40), IN `infelvevo` VARCHAR(20))  NO SQL
BEGIN
INSERT INTO partner(
    nev,
    cim,
    telefon,
    email) VALUES (
        innev,
        incim,
        intelefon,
        inemail);

	CALL logolas(infelvevo,
                'Partner felvétel',
                innev,
                " partner létrejött.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `tranzakcio_delete` (IN `inid` INT, IN `infelvevo` VARCHAR(20), IN `inmuvelet` VARCHAR(30))  NO SQL
BEGIN
DELETE FROM tranzakcio
WHERE id = inid;

	CALL logolas(infelvevo,
                inmuvelet,
                inid,
                " ID: tranzakció törlődött.");

END;

CREATE DEFINER=`root`@`localhost` PROCEDURE IF NOT EXISTS `tranzakcio_update` (IN `inid` INT, IN `infelvevo` VARCHAR(20), IN `inmuvelet` VARCHAR(30))  NO SQL
BEGIN
UPDATE tranzakcio
SET ido_teljesitve = CURRENT_TIMESTAMP
WHERE id = inid;

	CALL logolas(infelvevo,
                inmuvelet,
                inid,
                " ID: tranzakció teljesült.");

END;


-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `felhasznalok`
--

CREATE TABLE IF NOT EXISTS `felhasznalok` (
  `id` int(11) NOT NULL,
  `felhasznalonev` varchar(16) COLLATE utf8_hungarian_ci NOT NULL,
  `jelszo` text COLLATE utf8_hungarian_ci NOT NULL,
  `aktivitas` enum('aktiv','inaktiv') COLLATE utf8_hungarian_ci NOT NULL,
  `jogosultsag` enum('admin','moderator','user') COLLATE utf8_hungarian_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;


--
-- Tábla szerkezet ehhez a táblához `item`
--

CREATE TABLE IF NOT EXISTS `item` (
  `id` int(11) NOT NULL,
  `nev` varchar(32) COLLATE utf8_hungarian_ci NOT NULL,
  `leiras` text COLLATE utf8_hungarian_ci NOT NULL,
  `kategoria_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `kategoria`
--

CREATE TABLE IF NOT EXISTS `kategoria` (
  `id` int(11) NOT NULL,
  `nev` varchar(32) COLLATE utf8_hungarian_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `log`
--

CREATE TABLE IF NOT EXISTS `log` (
  `id` int(11) NOT NULL,
  `felhasznalo_nev` varchar(16) COLLATE utf8_hungarian_ci NOT NULL,
  `muvelet` enum('Felhasználó törlés','Felhasználó felvétel','Felhasználó módosítás','Bejövő tranzakció hozzáadás','Bejövő tranzakció törlés','Kimenő tranzakció hozzáadás','Kimenő tranzakció törlés','Kategória törlés','Kategória felvétel','Partner törlés','Partner felvétel','Tárgy törlés','Tárgy felvétel','Bejövő tranzakció módosítás','Kimenő tranzakció módosítás') COLLATE utf8_hungarian_ci NOT NULL,
  `idopont` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `reszletek` text COLLATE utf8_hungarian_ci
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `partner`
--

CREATE TABLE IF NOT EXISTS `partner` (
  `id` int(11) NOT NULL,
  `nev` varchar(32) COLLATE utf8_hungarian_ci NOT NULL,
  `cim` varchar(64) COLLATE utf8_hungarian_ci NOT NULL,
  `telefon` varchar(16) COLLATE utf8_hungarian_ci NOT NULL,
  `email` varchar(64) COLLATE utf8_hungarian_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `tranzakcio`
--

CREATE TABLE IF NOT EXISTS `tranzakcio` (
  `id` int(11) NOT NULL,
  `partner_id` int(11) NOT NULL,
  `item_id` int(11) NOT NULL,
  `darabszam` int(11) NOT NULL,
  `idopont` datetime NOT NULL,
  `ido_teljesitve` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `felhasznalok`
--
ALTER TABLE `felhasznalok`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `felhasznalonev` (`felhasznalonev`);

--
-- A tábla indexei `item`
--
ALTER TABLE `item`
  ADD PRIMARY KEY (`id`);

--
-- A tábla indexei `kategoria`
--
ALTER TABLE `kategoria`
  ADD PRIMARY KEY (`id`);

--
-- A tábla indexei `log`
--
ALTER TABLE `log`
  ADD PRIMARY KEY (`id`);

--
-- A tábla indexei `partner`
--
ALTER TABLE `partner`
  ADD PRIMARY KEY (`id`);

--
-- A tábla indexei `tranzakcio`
--
ALTER TABLE `tranzakcio`
  ADD PRIMARY KEY (`id`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `felhasznalok`
--
ALTER TABLE `felhasznalok`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=0;

--
-- AUTO_INCREMENT a táblához `item`
--
ALTER TABLE `item`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=0;

--
-- AUTO_INCREMENT a táblához `kategoria`
--
ALTER TABLE `kategoria`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=0;

--
-- AUTO_INCREMENT a táblához `log`
--
ALTER TABLE `log`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=0;

--
-- AUTO_INCREMENT a táblához `partner`
--
ALTER TABLE `partner`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=0;

--
-- AUTO_INCREMENT a táblához `tranzakcio`
--
ALTER TABLE `tranzakcio`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=0;
COMMIT;

