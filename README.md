<!--Headlines -->

# **Front-End Festpunktfelddatenbank für die DB Netz AG**
[![pipeline status](https://gitlab.rz.htw-berlin.de/softwareentwicklungsprojekt/wise2021-22/team1/badges/master/pipeline.svg)](https://gitlab.rz.htw-berlin.de/softwareentwicklungsprojekt/wise2020-21/team8/-/commits/master)

## Inhalt

1.[Allgemein Information](#allgemein-info)

2.[Technologiestaks](#Technologiestaks)

3.[Benutzung](#Benutzung)

4.[Collaboration](#collaboration)

5.[FAQs](#faqs)

---

## **1. Allgemein Information**

### Ein Projekt in Kooperation mit der DB Netz AG im Rahmen des Software Entwicklungsprojektes der HTW Berlin für das SoSe 2021.

![Image](https://www.bing.com/images/search?view=detailV2&ccid=zI5ROXg2&id=4B4513DCFE61DFAA37DA9B158BDE22860901373E&thid=OIP.zI5ROXg22gaxLmKzJWiX4wHaD4&mediaurl=https%3a%2f%2fwww.bahn.com%2fcommon%2fview%2fstatic%2fv8%2fimg%2fsocial-media%2fdb_logo_sm_1200x630_2016.jpg&exph=630&expw=1200&q=deutsch+bahn+logo&simid=608052414764615496&ck=3AC7D33DF9C62B358F5BD0FDB41A1332&selectedIndex=33&FORM=IRPRST&ajaxhist=0)

| Unternehmen                              |                                                           |
| ---------------------------------------- | --------------------------------------------------------- |
| [HTW Berlin](https://www.htw-berlin.de/) | [Deutche Bahn AG](https://www.bahn.de/p/view/index.shtml) |

---

## **2. Technologiestaks**

Damit der FestpunktDB.Business seine Arbeit erledigen kann, werden folgende Technologien benötigt:

- [.NET Version 5.0] (https://dotnet.microsoft.com/download/dotnet-core) als grundlegendes Framework
- [Microsoft Access Runtime] (https://www.microsoft.com/en-us/download/confirmation.aspx?id=13255) zur Kommunikation mit der Datenbank-Datei
- [NuGet-Paket "EntityFrameworkCore.Jet(3.1.0-alpha.3)"] (https://www.nuget.org/packages/EntityFrameworkCore.Jet/3.1.0-alpha.3) für die Umsetztung und die Benutzung der zu stehenden Datenbank.
- [NuGet-Paket "OleDB (5.0.0)"] (https://www.nuget.org/packages/System.Data.OleDb/5.0.0?_src=template) als weiteres Abstraktionslayer zur vereinfachten Arbeit mit der Datenbank.
- [NuGet-Paket "Bcl.AsyncInterfaces(1.1.1)"] (https://www.nuget.org/packages/Microsoft.Bcl.AsyncInterfaces/1.1.1) zu Unterstützung paraleller Prozessen.
- [NuGet-Paket "EntityFrameworkCore(3.1.10)"] (https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/3.1.10)
  - [NuGet-Paket "EntityFrameworkCore.Disign(3.1.10)"]
  - [NuGet-Paket "EntityFrameworkCore.Relational(3.1.10)"]
  - [NuGet-Paket "EntityFrameworkCore.Tools(3.1.10)"]
- Access / Office als 32-bit Anwendung zur verfügung gemacht bei MS-Office
- [NuGet-Paket "DocumentFormat.OpenXml"] (https://www.nuget.org/packages/DocumentFormat.OpenXml) zu Unterstützung der Exportierung nach Excel


Damit der FestpunktDB.Tests seine Arbeit erledigen kann, werden folgende Technologien benötigt:

- [NuGet-Paket "NUnit( 3.13.2)"] (https://www.nuget.org/packages/NUnit/3.13.2) zur Erstellung des Importtestes
- [NuGet-PAket "NUnit3TestAdapter (4.1.0)"] (https://www.nuget.org/packages/NUnit3TestAdapter/4.1.0)



---

## **3. Benutzung**

Beim Start der Applikation öffnet sich ein Authentifizierungs Formular. Durch Anmeldung wird man ins Hauptformular weitergeleitet, wo die Datensätzen(PP) schon zu sehen sind.Um nun etwas in der Datenbank zu finden, muss eine Abfrage gemacht werden.

### **3.2. Benutzeroberfläsche**

Im Hauptformular sind folgende optionen zum Navigieren durch die einzelnen Formulare möglich.
Im oberen Menü gibt es folgende, bis jetzt funktionierende Reiter:

- Ansicht
  Wenn hier rauf geklickt wird, öffnet sich das Hauptformular
- Bearbeiten
  Wenn hier rauf geklickt wird, öffnet sich das Bearbeiten-Formular
- Import
  Wenn hier rauf geklickt wird, öffnet sich das Importformular
- Einstellungen
  Wenn hier rauf geklickt wird, öffnet sich das Einstellungsformular
- Export
  Wenn hier rauf geklickt wird, öffnet sich das Exportformular
### **3.3. Formulare/Fenster**

- #### AuthWindow
  Da sind die Login Daten zu geben um dementsprechend der Software mit entsprechen Atributen zu starten
- #### MainWindow

  Von hier aus wird alles navigiert. Durch schließen dieses Fensters wird die ganze App geschlossen.
  Auf der rechten Seite befindet sich die Fileview und Abfrage Filter. für jede _PAD_ in der _Tabelle PP_ werden die dazu gehörigen Datensätzen angezeigt, und die Skizzen auch wenn vorhanden sind.

  > Der Bearbeiter kann die fehlenden Skizzen **per Klick** auf die roten Ampel hinzufugen(muss er drauf achten, dass der Name entspricht).

  > Für die Haupttabelle PP kann der Benutzer die Schaltflächen in der unteren rechten Ecke verwenden, um zwischen den Seiten der Einträge zu navigieren.

  > Der Bearbeiter kann die Anzahl der angezeigte Datensätze pro Seite ändern , indem er eine Zahl auf das Feld **Anzahl der Einträge** und auf _Enter_ oder auf **OK** drucken.

  Unten Recht sind die Ampel zu sehen. beim Rot gibt es keins beim Grün wird die PDF _Skizze_ angezeigt.
  Ganz Links befinden sich die drei Tabellen _PP, PH, PL, PK, PS_.

  - #### Abfragen Filter auf der MainWindow

    Das Abfrage Filter wurde intelligent implementiert. direkt beim Schreiben werden die gesuchten Datensätzen gleichzeitig angezeigt

    > zurücksetzen: darauf wird das Filterformular zurückgesetzt.

  - #### Bearbeitung

    Hier können die Datensätze bearbeitet werden. Das heißt die Schlüsseln und die Attributen können per Textbox eingeben werden.
    dafür einen Datensatz oder mehrere Datensätze per **strg** + **select** in Hauptansicht auswählen.

    > PAD **Bestätigen**:
    > darauf wird die neue angegebene PAD eingefügt und in der DatenBank aktuallisiert

    > Massenbearbeiten **Bestätigen**: darauf wird die neue angegebene Attributen auf der aktuele Dantensatz in der Datenbank ersetzt

    > Löschen **Bestätigen**: darauf wird der Aktuel in schau Datensatz aus der Datenbank gelöscht (in einer anderen Datein verschoben)

  - #### Import

    == Datensätze Importieren

    > Mit Hilfe von Import Button kann man Excel, CSV, NAP oder DBB Datei in den Temporärtabellen importieren.
    > Die importierte Datensätze werden in dem Datagrid gezeigt. Dazu muss man die Tabelle, die gezeigt werden muss, von den Combobox auswählen.
    > Ausgewähle Datensätze Löschen Button => Mit dem Button kann man die ausgewälte Datensätze löschen.
    > Alles Löschen Button => Mit dem Button kann man alle Daten in den temporäre Tabellen löschen.
    > PArt erstetzen => Damit kann man eine PArt eingeben und alle PArt mit gegebene PArt in eine neuer PArt ersetzen
    > Speichern Button => Mit dem Speichern Button kann man alle Datensätze , die in den temporäre Tabelle sind, in die Haupttabellen speichern.

    == Konflikt Management
    > Das System holt die neue Punkte von der Temp-Tabelle ab und vergleicht die mit die Punkten, die in Datebank sind.
    > wenn es ein Konflikt findet, zeigt auf GUI ein MassageBox das es ein Konflikt gefunden hat.
    > Der User kann die Konfliktete Punkten in Konflikt-Window sehen und da über checkboxen die wünschte Punkten auswählen.
    > Mergen Button => Damit kann man die gewünschte Punkte auswählen 
    > Speichern Button => Damit kann man die Punkte ohne Konflikte in die Datenbank speichern. 
    > Speichern Button => Damit kann man das Konfliktsprozess abbrechen und das Konfliktswindow zumachen. 

    == Skizzen Importieren

    > Skizzen Import Button => Damit kann man .ppt Dateien in den Skizzen file des Datenbankes importieren.
    > Skizzen erzeugen => Damit kann man die importierte .ppt Datei in .jpg und .pdf konvertieren
    > Skizzen speichern Button => Damit kann man Skizzen in den richtigen Ordner speichern.
    > Hauptdatenbestand prüfen Button => Damit kann man prüfen, ob neu importierte Skizze schon in der DatenBank stehen. Und man kann die Ergebnisse   im Datagrid sehen.
    > Wenn man im Skizzendatagrid auf einzelne Zeilen klickt, kann man die Skizzen sehen.
     
  -  #### Export
     
        == Filtern

        > Datensatz kann mithilfe von verschiedenen Variablen gefiltert werden.\
        > Der Anwender hat die Möglichkeit die Ansicht zwischen mehreren Tabellen zu wechseln.

        == Skizzen exportieren
        > Skizzen können in den Formaten *.ppt, *.pdf und *.jpg exportiert.\
        > Anwender müssen den Pfad eingeben, wo sich die Skizzen befindet.\
        > Export Ausführen Button ==> startet den Export, der Anwender wählt dann seinen Speicherort. Die Skizzen werden in den Speicherort gespeichert.

        == Datensätze exportieren
        > Datensätze können in den Formaten *.xlsx, *.xls, *.nap, *.csv und *.dbb exportiert.\
        > Anwender hat die Möglichkeit auszuwählen, welche Tabellen er exportieren möchte.\
        > Es werden nur die Werte, nach denen vorher gefiltert wurde, exportiert.\
        > Der Anwender können Namen für Dateien eingeben.\
        > Export Ausführen Button ==> startet den Export, der Anwender wählt dann seinen Speicherort. Der Speicherort ändert sich nicht, bis der Anwender einen neuen auswählt.
### **3.4. Resourcen**

In der FestpunktDB.GUI befindet sich drei Resourcen Dictionary, welches Skizzen, die Datenbank, und die User Daten beinhaltet.

- In dem Resource Dictionary der Skizzen sind drei Ordner für die drei Skizzenarten (pdf,jpeg,ppt).
- geloeschteSkizzen sind drei Ordner für die drei Skizzenarten (pdf,jpeg,ppt). Wo die gelöschten Skizzen veschoben werden
- In der Datenmodell_FPF_NEU.accdb sind die Datensätzen gespeichert.
- In der UserVerwaltung.accdb sind User Daten gespeichert.

---

## **4. Collaboration**

<!--Table-->

| Name | Pseudo | School Email | Linkedin |
| ---- | ------ | ------------ | -------- |

Team WiSe 2020  
|Barth. Feudong|[Mr.Schaffman](https://www.instagram.com/mr.schaffman/)| s0570583@htw-berlin.de| [barth.Feuddong](https://www.linkedin.com/in/barth-feudong-97a519182/?originalSubdomain=de) |\
|Khue Nguyen |- |s0573655@htw-berlin.de |-|  

Team SoSe 2021\
|Uresha Matara Arachchige Don |- |s0569832@htw-berlin.de |-|\
|Sandra Fayad|- |s0563146@htw-berlin.de |-|\
|Mohammed Marie|- |s0567447@htw-berlin.de |-|\

Team WiSe 2021\
|Azim Izzum Ramadhani Mubarak |- |s0571801@htw-berlin.de |-|\
|Bashar Mustafa|- |s0568909@htw-berlin.de |-|\
|Christopher Schwarz|-|s0566068@htw-berlin.de |-|

---

## **5. FAQs**

- [x] Sprint 1 : Export von Datensätzen(.dbb, .xls, .xlsx), Überarbeitung der Filterfunktion
- [x] Sprint 2 : Implementierung von Unit Tests für Import, Export von Datensätzen(.csv, .nap), Verbesserung Exportfunktion(.dbb, .xls)
- [x] Sprint 3 : Implementierung von Unit Tests für Export, Skizze auf PAD Exportieren, GUI Verbesserung, 
```c#
Code Written in C#
```
