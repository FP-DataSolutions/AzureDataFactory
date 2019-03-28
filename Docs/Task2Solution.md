# Zadanie 2

## Wgranie pliku z danymi na Azure Storage Manager

Przejdź do poratlu Azure, a następnie wybierz niedawno stworzony magazyn danych. Następnie kliknij w sekcję **Blobs** i dodaj nowy kontener o prywatnym dostępie o nazwie **parking**. Otwórz kontener, a następnie klinik w przycik **upload** w celu wgrania pliku *.csv z danymi.

![Upload datasetu do ASA](../Imgs/UploadDatasetASA.PNG)

## Data Factory UI

Przejdź od usługi Azure Data Factory. W sekcji **Overview** kliknij w przycisk **Author & Monitor**:

![Upload datasetu do ASA](../Imgs/AuthorAndMonitorADF.png)

W nowej karcie przeglądarki zostaniemy przekierowani do webowego designera Azure Data Factory:

![Upload datasetu do ASA](../Imgs/WebDesignerADF.PNG)

Azure Data Factory może poprosić o adres rezpozytorium git'owego. Na potrzeby szkolenia, gdzie ADF wykorzystywanty będzie eksperymentalnie, nie ma potrzeby konfiguracji repozytorium. W przypadku wykorzystywania go w projekcie zaleca się integrację z dowolnym repozotorium gitowym, np. ![GitHubem](https://azure.microsoft.com/en-us/blog/azure-data-factory-visual-tools-now-supports-github-integration/) .

## Konfiguracja Dataset'u + Linked Service (Azure Storage Account)

Aby stworzyć dataset kilkamy w ikonkę ołówka (author) znajdującą się po lewej stronie. Następnie klikamy w ikonkę plusa i wybieramy **Dataset**. Po prawej stronie powinien wysunąć się dodatkowy panel z wszystkimi dostępnymi źródłami, aktualnie wspieranymi przez ADF. Naszym zadaniem jest przekopiowanie danych z pliku *.csv, znajdującym się obecnie na magazynie danych, do bazy danych Azure SQL Server. Aby odfiltrować listę dostępnych źródeł wybierz zakładkę **Azure**, a następnie **Azure Blob Storage**:

![Upload datasetu do ASA](../Imgs/DataSetASA.png)

Następnie nazwij swój dataset, np. **ParkingASADataSet** i przejdź do zakładki **Connection**. Koniecznym będzie stworzenie tzw. **Linked service** za pomocą którego będziemy mogli połączyć się do magazynu danych. Kliknij w plus z napisem **New** znajdujący się po prawej stronie. Przechodzimy do konfiguracji serwisu. Nazwij swoje połączenie, a następnie w sekcji **Connect via integration runtime** kliknij w listę rozwijalną i wybierz opcję **New**. Teraz kliknij w kafelek **Azure Public** - będziemy działać tylko w obszarze chmury Azure:

![Azure Public Integration Runtime](../Imgs/IntegrationRuntimeAzureADF.png)

Nazwij swój runtime, a jako region wybierz **North Europe**. Wracamy do konfiguracji serwisu. Magazyn danych podlinkujemy za pomocą **Connection Stringa**. Magazyn powinien być dostępny po wybraniu subskrypcji:

![Nowy Linked Service](../Imgs/NewLinkedService.png)

Na samym dole znajduje się jeszcze przycisk **Test connection** sprwadzający jakość połączenia z magazynem danych.
Kiedy **Linked service** zostanie już podłączony do dataset'u możliwe będzie wskazanie na konkretny kontener oraz plik z danymi (przycisk **Browse**). Jako **Compression type** zostawiamy **None**. Przy pomocy przycisku **Preview  data** możesz podejrzeć dataset:

![Podejrzenie dataset'u](../Imgs/PreviewDataset.png)

Poprawnie skonfigorowane połączenie dataset'u powinno wyglądać następująco:

![Skonfigurowany dataset](../Imgs/ConfiguredDataSetConnection.png)

W zakładce **Schema** można zaimportować domyślą schemę dla danych lub nieco ją zmodyfikować.

Aby zapisać skonfigurowany dataset kliknij w zakładkę **Publish All**.

## Konfiguracja Dataset'u + Linked Service (Azure SQL Database)

Na podstawie instrukcji zawartej powyżej stwórz drugi dataset, tym razem dla bazy danych Azure SQL Databse. Konieczne będzie dodanie tabeli służącej do przechowywania załadowanych danych. Pamiętaj, żeby przy importowaniu schemy usunąć wszelkie atrybuty pełnioce rolę klucza.

Skypt SQL do stworzenia tabeli:

```sql
CREATE TABLE [dbo].[Parking]
(
	[Id] INT IDENTITY(1,1),
	[SystemCodeNumber] VARCHAR(255) NULL,
	[Capacity] INT NULL,
	[Occupancy] INT NULL,
	[LastUpdated] DATETIME NULL
)
```

## Potok do kopiowania danych

Aby dodać nowy potok kliknij ponowne w plus, a następnie kliknij w **pipeline**. Można również skorzystać z gotowego kreatora potoków służących do kopiowania danych wybierając ostatnią opcję **Copy Data**, aczkolwiek w zadaniu skupimy się na piewrszym wymienionym sposobie. Następnie z listy dostępnych **Activities** rozwiń zakładkę **Move & Transform** i przeciągnij element **Copy Data** na workspace. Teraz przechodzimy na zakładkę **Source**, w której wskazujemy odpowiedni **source dataset**. Następnie klikamy w zakładkę **Sink**. Na chwilę obecną nie ustawiamy żadnej procedury składowanej. Przechodzimy do zakładki **Mapping** i klikamy w przycisk **Import schemas**, powinniśmy uzyskać efekt podobny do przedstawionego poniżej:

![Mappowanie ADF](../Imgs/MappingADF.png)

W celu uruchomienia potoku kliknij w przycisk **Debug**. Po zakończeniu potoku sprawdź czy na bazie pojawiły się jakieś dane.