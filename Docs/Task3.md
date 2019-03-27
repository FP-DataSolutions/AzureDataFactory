# Zadanie 3

W ramach tego zadania należy stworzyć potok służący do szyfrowania wszystkich przesłanych plików tekstowych. W ramach potoku należy:

- skonfigurować wyzwalacz uruchamiający potok, gdy na magazynie danych pojawi się nowy plik tekstowy (o rozszerzeniu *.txt) wewnątrz kontenera **to_encrypt**,

- dodać activity zapisujące do stworzonej wcześniej bazy danych informacje o nazwie pliku do zaszyfrowania, czasie jego spłynięcia do magazynu (stored procedure). Ponadto procedura powinna ustawić dwa wartości dwóch dodatkowych atrybutów mianowicie **IN_PROGRESS** (na **1**) oraz **COMPLETED** (na **0**) w celu zwizualizowania aktualnego stanu szyfrowanych plików,

- dodać kolejne activity uruchamiające przedstawioną poniżej funkcję szyfrującą (kod znajduje się na samym dole strony), za pośrednictwem usługi **Azure Functions**,

- dodać kolejne activity aktualizujące dodany wcześniej wpis ustawiający flagę **IN_PROGRESS** na **0** oraz **COMPLETED** na **1**,

```
// code will be here
```
