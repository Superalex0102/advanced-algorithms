# Haladó Algoritmusok

## Labirintus útvonal keresés
A labirintus útvonal keresési problémát a hegymászó módszerrel próbáltam optimalizálni.

Bemenet:
```csharp
int[,] labyrinth = new int[,]
{
    { 0, 1, 0, 0, 0 },
    { 0, 0, 0, 1, 0 },
    { 0, 1, 0, 1, 0 },
    { 1, 1, 0, 1, 0 },
    { 0, 0, 0, 0, 0 }
};
```

Kimenet:
```shell
Megtalált útvonal:
(0, 0)
(0, 1)
(1, 1)
(2, 1)
(2, 2)
(2, 3)
(2, 4)
(3, 4)
(4, 4)

Labirintus:
* # . . .
* * * # .
. # * # .
# # * # .
. . * * *
```

Maximális iterációk száma: 1000

## Képszegmentálás
A képszegmentálás problémáját a K-meanssel próbáltam optimalizálni.

Bemenet:
<br>
<img src="https://github.com/user-attachments/assets/5aae03a6-621f-4b3a-9b8f-1962592294c2" width="200"/>

Kimenet:
<br>
<img src="https://github.com/user-attachments/assets/985d2443-e31b-4b9e-92ae-be3faf6e0332" width="200"/>

Klaszterek száma: 5

## Utazóügynök
Az utazóügynök problémáját a Tabu kereséssel próbáltam optimalizálni.

Bemenet:
```csharp
var cities = new List<City>
{
    new City(0, 60, 200),
    new City(1, 180, 200),
    new City(2, 80, 180),
    new City(3, 140, 180),
    new City(4, 20, 160),
    new City(5, 100, 160),
    new City(6, 200, 160),
    new City(7, 140, 140),
    new City(8, 40, 120),
    new City(9, 100, 120),
    new City(10, 180, 100),
    new City(11, 60, 80),
    new City(12, 120, 80),
    new City(13, 180, 60),
    new City(14, 20, 40),
    new City(15, 100, 40),
    new City(16, 200, 40),
    new City(17, 20, 20),
    new City(18, 60, 20),
    new City(19, 160, 20)
};
```

Kimenet:
```shell
Utazóügynök probléma megoldása Tabu kereséssel
Kezdeti megoldás távolsága: 2284.53577360781
Javulás az 0. iterációban: 1889.92156379834
Javulás az 1. iterációban: 1634.24605061491
Javulás az 2. iterációban: 1492.283246634
Javulás az 3. iterációban: 1400.93932971626
Javulás az 4. iterációban: 1330.20382753257
Javulás az 5. iterációban: 1268.41065946046
Javulás az 6. iterációban: 1222.81028722416
Javulás az 7. iterációban: 1189.43930786565
Javulás az 8. iterációban: 1161.15503661819
Javulás az 9. iterációban: 1132.31430342569
Javulás az 10. iterációban: 1085.61255360384
Javulás az 11. iterációban: 1083.55554619876
Javulás az 12. iterációban: 1060.82657059429

A keresés 113 iteráció után befejezodött.

Legjobb megoldás:
Útvonal: 18 -> 15 -> 19 -> 16 -> 7 -> 5 -> 4 -> 0 -> 2 -> 3 -> 1 -> 6 -> 10 -> 13 -> 12 -> 9 -> 8 -> 11 -> 14 -> 17
Teljes távolság: 1060.82657059429
```

A tabu lista mérete: 20
<br>
Maximális iterációk száma: 1000
<br>
Leállási feltétel: 100
