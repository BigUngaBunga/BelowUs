# Below Us

## Projektets Syfte
Detta är en kodbas för projektet Below Us.

För att testa uppkoppling så måste två instanser av programmet startas
Den ena instansen skall då välja "Host" knappen i huvudmenyn
Den andra instansen skall välja "Connect"
Därpå måste antingen en korrekt ip eller localhost skrivas, beroende på vilka datorer som kör applikationerna
Om spelaren som håller i servern inte har portforwardat så kan exempelvis applikationer som hamachi användas för att koppla upp mellan olika nätverk.

Spelarna börjar i byn och för att starta en ny omgång så måste en spelare klättra ned till ubåten, gå in i stationen genom att trycka Q,
och sedan trycka på knappen som finns i menyn som kommer upp.

Kontrollerna för spelarkaraktären är WASD för att röra sig, Q för att gå in i en station och det finns sedan en knapp i nedre vänstra hörnet för att lämna stationen.
Kontrollerna för ubåten är WS för att röra sig fram och bak A och D för att rotera ubåten och mellanslag för att vända på ubåten.
För att lamporna skall vara aktiva så måste generatorn producera ström.
För att ubåten skall kunna röra på sig så måste generatorn vara aktiv.
Syrekonsumtionen minskar om syrestationen är aktiv.
Spelarna förlorar om de får slut på liv eller syre.
Om bossen dödas så vinner spelarna.
