# EwidencjaSprzetuOOP
Wykorzystywany język, narzędzia i minimalne wymagania sprzętowe
Język i platforma:
• C# (.NET 10.0, TargetFramework: net10.0)
Narzędzia:
• Visual Studio 2022,
• Microsoft SQL Server (np. LocalDB/Express/Developer),
• SSMS 22 (opcjonalnie, do podglądu danych i generowania ERD),
• Git + GitHub (repozytorium).
Minimalne wymagania sprzętowe:
• System: Windows 10/11
• CPU: min. 2 rdzenie (zalecane 4)
• RAM: min. 8 GB (zalecane 16 GB)
• Dysk: min. 2 GB wolnego miejsca
• Zainstalowany .NET SDK 10.0 oraz SQL Server


Project Sdk =" Microsoft . NET . Sdk " >
2
3 < PropertyGroup >
4 < OutputType > Exe </ OutputType >
5 < TargetFramework > net10 .0 </ TargetFramework >
6 < ImplicitUsings > enable </ ImplicitUsings >
7 < Nullable > enable </ Nullable >
8 </ PropertyGroup >
9
10 < ItemGroup >
11 < None Remove =" db \ EwidencjaSprzetuDb . sql " / >
12 </ ItemGroup >
13
14 < ItemGroup >
15 < PackageReference Include =" Microsoft . Data . SqlClient " Version
="7.0.0 - preview3 .25342.7" / >
16 </ ItemGroup >
17
18 < ItemGroup >
19 < Content Include =" db \ EwidencjaSprzetuDb . sql " >
20 < CopyToOutputDirectory > PreserveNewest </ CopyToOutputDirectory >
21 </ Content >
22 </ ItemGroup >
23
24 < ItemGroup >
25 < None Update =" appsettings . json " >
26 < CopyToOutputDirectory > PreserveNewest </ CopyToOutputDirectory >
27 </ None >
28 </ ItemGroup >
29
30 </ Project >
